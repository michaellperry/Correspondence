using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UpdateControls.Correspondence.Data
{
    public class HistoricalTree : IDisposable
    {
        private byte[] _readBuffer = new byte[8];
        private Stream _stream;

        public HistoricalTree(Stream stream)
        {
            _stream = stream;
        }

        public long Save(HistoricalTreeFact fact)
        {
            // Read the head pointers of the predecessors.
            long[] nextPointers = new long[fact.Predecessors.Count()];
            int i = 0;
            foreach (HistoricalTreePredecessor predecessor in fact.Predecessors)
            {
                _stream.Seek(predecessor.PredecessorFactId, SeekOrigin.Begin);
                long head = ReadLong();
                nextPointers[i] = head;
                ++i;
            }

            // Write the file header.
            long position = _stream.Seek(0, SeekOrigin.End);
            if (position == 0)
            {
                _stream.Write(new byte[] { 85, 79, 80, 65 }, 0, 4);
                _stream.Flush();
                position = 4;
            }

            // Write the new fact.
            WriteLong(0L);
            WriteInt(fact.Predecessors.Count());
            i = 0;
            foreach (HistoricalTreePredecessor predecessor in fact.Predecessors)
            {
                WriteInt(predecessor.RoleId);
                WriteLong(predecessor.PredecessorFactId);
                WriteLong(nextPointers[i]);
                ++i;
            }
            WriteInt(fact.FactTypeId);
            WriteInt(fact.Data.Length);
            _stream.Write(fact.Data, 0, fact.Data.Length);
            _stream.Flush();

            foreach (HistoricalTreePredecessor predecessor in fact.Predecessors)
            {
                _stream.Seek(predecessor.PredecessorFactId, SeekOrigin.Begin);
                WriteLong(position);
                _stream.Flush();
            }

            return position;
        }

        public HistoricalTreeFact Load(long factId)
        {
            // Skip the head pointer.
            _stream.Seek(factId + 8, SeekOrigin.Begin);

            // Load the predecessors.
            int predecessorCount = ReadInt();
            List<HistoricalTreePredecessor> predecessors = new List<HistoricalTreePredecessor>();
            for (int i = 0; i < predecessorCount; i++)
            {
                int roleId = ReadInt();
                long predecessorFactId = ReadLong();
                long next = ReadLong();
                predecessors.Add(new HistoricalTreePredecessor(roleId, predecessorFactId));
            }

            // Load the fact type and data.
            int factTypeId = ReadInt();
            int dataLength = ReadInt();
            byte[] data = new byte[dataLength];
            _stream.Read(data, 0, dataLength);

            return new HistoricalTreeFact(factTypeId, data)
                .SetPredecessors(predecessors);
        }

        public List<long> GetPredecessorsInRole(long factId, int roleId)
        {
            _stream.Seek(factId + 8, SeekOrigin.Begin);

            int predecessorCount = ReadInt();
            List<long> predecessors = new List<long>();
            for (int i = 0; i < predecessorCount; i++)
            {
                int predecessorRoleId = ReadInt();
                long predecessorFactId = ReadLong();
                long next = ReadLong();
                if (predecessorRoleId == roleId)
                    predecessors.Add(predecessorFactId);
            }
            return predecessors;
        }

        public IEnumerable<long> GetSuccessorsInRole(long factId, int roleId)
        {
            _stream.Seek(factId, SeekOrigin.Begin);

            long successorFactId = ReadInt();
            while (successorFactId != 0)
            {
                _stream.Seek(successorFactId + 8, SeekOrigin.Begin);
                int predecessorCount = ReadInt();
                int numberOfMatches = 0;
                long nextFactId = 0;
                for (int i = 0; i < predecessorCount; i++)
                {
                    int predecessorRoldId = ReadInt();
                    long predecessorFactId = ReadLong();
                    long next = ReadLong();
                    if (predecessorFactId == factId)
                    {
                        if (predecessorRoldId == roleId)
                            ++numberOfMatches;
                        nextFactId = next;
                    }
                }
                for (int i = 0; i < numberOfMatches; i++)
                    yield return successorFactId;

                // After I yield, I don't know where the stream pointer
                // is anymore, since the coroutine could reenter. But
                // that's OK, since the next thing I do is seek.
                successorFactId = nextFactId;
            }
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        private int ReadInt()
        {
            _stream.Read(_readBuffer, 0, 4);
            return BitConverter.ToInt32(_readBuffer, 0);
        }

        private void WriteInt(int value)
        {
            byte[] writeBuffer = BitConverter.GetBytes(value);
            _stream.Write(writeBuffer, 0, 4);
        }

        private long ReadLong()
        {
            _stream.Read(_readBuffer, 0, 8);
            return BitConverter.ToInt64(_readBuffer, 0);
        }

        private void WriteLong(long value)
        {
            byte[] writeBuffer = BitConverter.GetBytes(value);
            _stream.Write(writeBuffer, 0, 8);
        }
    }
}
