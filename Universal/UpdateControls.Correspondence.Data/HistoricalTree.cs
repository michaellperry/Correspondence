using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace UpdateControls.Correspondence.Data
{
    public class HistoricalTree : StreamBasedDataStructure
    {
        public HistoricalTree(IRandomAccessStream randomAccessStream) :
            base(randomAccessStream)
        {
        }

        public async Task<long> SaveAsync(HistoricalTreeFact fact)
        {
            // Read the head pointers of the predecessors.
            long[] nextPointers = new long[fact.Predecessors.Count()];
            int i = 0;
            foreach (HistoricalTreePredecessor predecessor in fact.Predecessors)
            {
                SeekTo(predecessor.PredecessorFactId);
                long head = await ReadLongAsync();
                nextPointers[i] = head;
                ++i;
            }

            // Write the file header.
            long position = SeekToEnd();
            if (position == 0)
            {
                await WriteBytesAsync(new byte[] { 85, 79, 80, 65 });
                position = 4;
            }

            // Write the new fact.
            MemoryStream memory = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(memory))
            {
                writer.Write(0L);
                writer.Write(fact.Predecessors.Count());
                i = 0;
                foreach (HistoricalTreePredecessor predecessor in fact.Predecessors)
                {
                    writer.Write(predecessor.RoleId);
                    writer.Write(predecessor.PredecessorFactId);
                    writer.Write(nextPointers[i]);
                    ++i;
                }
                writer.Write(fact.FactTypeId);
                writer.Write(fact.Data == null ? 0 : fact.Data.Length);
                if (fact.Data != null && fact.Data.Length > 0)
                    writer.Write(fact.Data);
                writer.Flush();

                await WriteBytesAsync(memory.ToArray());
            }

            foreach (HistoricalTreePredecessor predecessor in fact.Predecessors)
            {
                SeekTo(predecessor.PredecessorFactId);
                await WriteLongAsync(position);
            }

            return position;
        }

        public async Task<HistoricalTreeFact> LoadAsync(long factId)
        {
            // Skip the head pointer.
            SeekTo(factId + 8);

            // Load the predecessors.
            int predecessorCount = await ReadIntAsync();
            List<HistoricalTreePredecessor> predecessors = new List<HistoricalTreePredecessor>();
            for (int i = 0; i < predecessorCount; i++)
            {
                int roleId = await ReadIntAsync();
                long predecessorFactId = await ReadLongAsync();
                long next = await ReadLongAsync();
                predecessors.Add(new HistoricalTreePredecessor(roleId, predecessorFactId));
            }

            // Load the fact type and data.
            int factTypeId = await ReadIntAsync();
            int dataLength = await ReadIntAsync();
            byte[] data = null;
            if (dataLength > 0)
            {
                data = new byte[dataLength];
                await ReadBytesAsync(data);
            }
            return new HistoricalTreeFact(factTypeId, data)
                .SetPredecessors(predecessors);
        }

        public async Task<List<long>> GetPredecessorsInRoleAsync(long factId, int roleId)
        {
            SeekTo(factId + 8);

            int predecessorCount = await ReadIntAsync();
            List<long> predecessors = new List<long>();
            for (int i = 0; i < predecessorCount; i++)
            {
                int predecessorRoleId = await ReadIntAsync();
                long predecessorFactId = await ReadLongAsync();
                long next = await ReadLongAsync();
                if (predecessorRoleId == roleId)
                    predecessors.Add(predecessorFactId);
            }
            return predecessors;
        }

        public async Task<List<long>> GetSuccessorsInRoleAsync(long factId, int roleId)
        {
            List<long> successorFactIds = new List<long>();
            SeekTo(factId);

            long successorFactId = await ReadLongAsync();
            while (successorFactId != 0)
            {
                SeekTo(successorFactId + sizeof(Int64));
                int predecessorCount = await ReadIntAsync();
                int numberOfMatches = 0;
                long nextFactId = 0;
                for (int i = 0; i < predecessorCount; i++)
                {
                    int predecessorRoldId = await ReadIntAsync();
                    long predecessorFactId = await ReadLongAsync();
                    long next = await ReadLongAsync();
                    if (predecessorFactId == factId)
                    {
                        if (predecessorRoldId == roleId || roleId == -1)
                            ++numberOfMatches;
                        nextFactId = next;
                    }
                }
                for (int i = 0; i < numberOfMatches; i++)
                    successorFactIds.Add(successorFactId);

                // After I yield, I don't know where the stream pointer
                // is anymore, since the coroutine could reenter. But
                // that's OK, since the next thing I do is seek.
                successorFactId = nextFactId;
            }
            return successorFactIds;
        }
    }
}
