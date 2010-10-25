using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UpdateControls.Correspondence.Data
{
    public class RedBlackTree : StreamBasedDataStructure
    {
        private enum NodeColor : byte
        {
            Black = 0,
            Red = 1
        }

        private struct Node
        {
            public byte Color;
            public int HashCode;
            public long Left;
            public long Right;
            public long FactId;
        }

        public RedBlackTree(Stream stream) :
            base(stream)
        {
            _stream = stream;
        }

        public IEnumerable<long> FindFacts(int hashCode)
        {
            if (_stream.Length > 0)
            {
                long current = 0;
                Node node;

                // Search for the beginning of the hash code.
                do
                {
                    node = ReadNode(current);
                    if (hashCode == node.HashCode)
                    {
                        // Walk through all nodes with this hash code.
                        yield return node.FactId;
                        current = node.Right;
                        while (current != 0)
                        {
                            node = ReadNode(current);
                            if (hashCode == node.HashCode)
                            {
                                yield return node.FactId;
                                current = node.Right;
                            }
                            else
                            {
                                current = node.Left;
                            }
                        }
                        break;
                    }

                    if (hashCode > node.HashCode)
                        current = node.Right;
                    else
                        current = node.Left;
                }
                while (current != 0);
            }
        }

        public void AddFact(int hashCode, long factId)
        {
            if (_stream.Length == 0)
            {
                WriteNode(NodeColor.Black, hashCode, factId);
            }
            else
            {
                // Find the inseration point.
                long current = 0;
                long next = 0;
                Node node;

                do
                {
                    current = next;
                    node = ReadNode(current);
                    if (hashCode >= node.HashCode)
                        next = node.Right;
                    else
                        next = node.Left;
                }
                while (next != 0);

                // Insert a new node.
                long position = WriteNode(NodeColor.Red, hashCode, factId);
                int offset;
                if (hashCode >= node.HashCode)
                    offset = sizeof(byte) + sizeof(int) + sizeof(long);
                else
                    offset = sizeof(byte) + sizeof(int);
                _stream.Seek(current + offset, SeekOrigin.Begin);
                WriteLong(position);
            }
        }

        public void CheckInvariant()
        {
            if (_stream.Length > 0)
            {
                Node node = ReadNode(0);
                if (node.Color != (byte)NodeColor.Black)
                    throw new ApplicationException("Invariant violated: The root node is black.");

                CheckInvariantNode((NodeColor)node.Color, node.Left);
                CheckInvariantNode((NodeColor)node.Color, node.Right);
            }
        }

        private int CheckInvariantNode(NodeColor parentColor, long position)
        {
            if (position == 0)
                return 0;

            Node node = ReadNode(position);
            if (parentColor == NodeColor.Red && node.Color != (byte)NodeColor.Black)
                throw new ApplicationException("Invariant violated: both children of every red node are black.");

            int leftCount = CheckInvariantNode((NodeColor)node.Color, node.Left);
            int rightCount = CheckInvariantNode((NodeColor)node.Color, node.Right);

            if (leftCount != rightCount)
                throw new ApplicationException("Invariant violated: Every simple path from a given node to any of its descendant leaves contains the same number of black nodes.");
            return leftCount + (node.Color == (byte)NodeColor.Black ? 1 : 0);
        }

        private Node ReadNode(long position)
        {
            _stream.Seek(position, SeekOrigin.Begin);
            byte color = ReadByte();
            int hashCode = ReadInt();
            long left = ReadLong();
            long right = ReadLong();
            long factId = ReadLong();

            return new Node
            {
                Color = color,
                HashCode = hashCode,
                Left = left,
                Right = right,
                FactId = factId
            };
        }

        private long WriteNode(NodeColor nodeColor, int hashCode, long factId)
        {
            long position = _stream.Seek(0, SeekOrigin.End);
            WriteByte((byte)nodeColor);
            WriteInt(hashCode);
            WriteLong(0);
            WriteLong(0);
            WriteLong(factId);
            return position;
        }
    }
}
