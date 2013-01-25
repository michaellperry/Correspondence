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

        public async Task<long> Save(HistoricalTreeFact fact)
        {
            // Read the head pointers of the predecessors.
            long[] nextPointers = new long[fact.Predecessors.Count()];
            int i = 0;
            foreach (HistoricalTreePredecessor predecessor in fact.Predecessors)
            {
                SeekTo(predecessor.PredecessorFactId);
                long head = await ReadLong();
                nextPointers[i] = head;
                ++i;
            }

            // Write the file header.
            long position = SeekToEnd();
            if (position == 0)
            {
                await WriteBytes(new byte[] { 85, 79, 80, 65 });
                await Flush();
                position = 4;
            }

            // Write the new fact.
            await WriteLong(0L);
            await WriteInt(fact.Predecessors.Count());
            i = 0;
            foreach (HistoricalTreePredecessor predecessor in fact.Predecessors)
            {
                await WriteInt(predecessor.RoleId);
                await WriteLong(predecessor.PredecessorFactId);
                await WriteLong(nextPointers[i]);
                ++i;
            }
            await WriteInt(fact.FactTypeId);
            await WriteInt(fact.Data == null ? 0 : fact.Data.Length);
            if (fact.Data != null && fact.Data.Length > 0)
                await WriteBytes(fact.Data);
            await Flush();

            foreach (HistoricalTreePredecessor predecessor in fact.Predecessors)
            {
                SeekTo(predecessor.PredecessorFactId);
                await WriteLong(position);
                await Flush();
            }

            return position;
        }

        public async Task<HistoricalTreeFact> Load(long factId)
        {
            // Skip the head pointer.
            SeekTo(factId + 8);

            // Load the predecessors.
            int predecessorCount = await ReadInt();
            List<HistoricalTreePredecessor> predecessors = new List<HistoricalTreePredecessor>();
            for (int i = 0; i < predecessorCount; i++)
            {
                int roleId = await ReadInt();
                long predecessorFactId = await ReadLong();
                long next = await ReadLong();
                predecessors.Add(new HistoricalTreePredecessor(roleId, predecessorFactId));
            }

            // Load the fact type and data.
            int factTypeId = await ReadInt();
            int dataLength = await ReadInt();
            byte[] data = new byte[dataLength];
            await ReadBytes(data);
            return new HistoricalTreeFact(factTypeId, data)
                .SetPredecessors(predecessors);
        }

        public async Task<List<long>> GetPredecessorsInRole(long factId, int roleId)
        {
            SeekTo(factId + 8);

            int predecessorCount = await ReadInt();
            List<long> predecessors = new List<long>();
            for (int i = 0; i < predecessorCount; i++)
            {
                int predecessorRoleId = await ReadInt();
                long predecessorFactId = await ReadLong();
                long next = await ReadLong();
                if (predecessorRoleId == roleId)
                    predecessors.Add(predecessorFactId);
            }
            return predecessors;
        }

        public async Task<List<long>> GetSuccessorsInRole(long factId, int roleId)
        {
            List<long> successorFactIds = new List<long>();
            SeekTo(factId);

            long successorFactId = await ReadLong();
            while (successorFactId != 0)
            {
                SeekTo(successorFactId + sizeof(Int64));
                int predecessorCount = await ReadInt();
                int numberOfMatches = 0;
                long nextFactId = 0;
                for (int i = 0; i < predecessorCount; i++)
                {
                    int predecessorRoldId = await ReadInt();
                    long predecessorFactId = await ReadLong();
                    long next = await ReadLong();
                    if (predecessorFactId == factId)
                    {
                        if (predecessorRoldId == roleId)
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
