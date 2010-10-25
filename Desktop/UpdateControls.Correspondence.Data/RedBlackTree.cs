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
            public NodeColor Color;
            public int HashCode;
            public long Left;
            public long Right;
            public long FactId;
        }

        private struct NodeReference
        {
            public long Position;
            public long Sibling;
            public NodeColor Color;
            public bool LeftChild;
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
                _stream.Seek(0, SeekOrigin.Begin);
                long current = ReadLong();
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
                WriteLong(sizeof(long));
                WriteNode(NodeColor.Black, hashCode, factId);
            }
            else
            {
                // Find the inseration point.
                _stream.Seek(0, SeekOrigin.Begin);

                Stack<NodeReference> stack = new Stack<NodeReference>();
                Node node;
                long parent = 0;
                long child = ReadLong();
                long sibling = 0;
                bool leftChild = true;

                do
                {
                    parent = child;
                    node = ReadNode(parent);
                    stack.Push(new NodeReference
                    {
                        Position = parent, 
                        Color = node.Color, 
                        Sibling = sibling, 
                        LeftChild = leftChild
                    });
                    if (hashCode >= node.HashCode)
                    {
                        child = node.Right;
                        sibling = node.Left;
                        leftChild = false;
                    }
                    else
                    {
                        child = node.Left;
                        sibling = node.Right;
                        leftChild = true;
                    }
                }
                while (child != 0);

                // Insert a new node.
                long position = WriteNode(NodeColor.Red, hashCode, factId);
                int offset;
                if (hashCode >= node.HashCode)
                    offset = sizeof(byte) + sizeof(int) + sizeof(long);
                else
                    offset = sizeof(byte) + sizeof(int);
                _stream.Seek(parent + offset, SeekOrigin.Begin);
                WriteLong(position);

                // Change colors and rotate.
                while (stack.Count >= 2)
                {
                    NodeReference parentReference = stack.Pop();
                    NodeReference grandparentReference = stack.Pop();

                    if (parentReference.Color == NodeColor.Black)
                        break;

                    Node uncleNode = ReadNode(parentReference.Sibling);
                    if (uncleNode.Color == NodeColor.Red)
                    {
                        // Both the parent and uncle are red, so change both to black and the grandparent to red.
                        _stream.Seek(parentReference.Position, SeekOrigin.Begin);
                        WriteByte((byte)NodeColor.Black);
                        _stream.Seek(parentReference.Sibling, SeekOrigin.Begin);
                        WriteByte((byte)NodeColor.Black);
                        _stream.Seek(grandparentReference.Position, SeekOrigin.Begin);
                        WriteByte((byte)(stack.Count == 0 ? NodeColor.Black : NodeColor.Red));
                    }
                    else
                    {
                        // Parent is red but uncle is black. Need to rotate.
                        if (!leftChild && parentReference.LeftChild)
                        {

                        }
                        break;
                    }
                    leftChild = grandparentReference.LeftChild;
                }
            }
        }

        public void CheckInvariant()
        {
            if (_stream.Length > 0)
            {
                _stream.Seek(0, SeekOrigin.Begin);
                long root = ReadLong();
                Node node = ReadNode(root);
                if (node.Color != NodeColor.Black)
                    throw new ApplicationException("Invariant violated: The root node is black.");

                CheckInvariantNode(node.Color, node.Left);
                CheckInvariantNode(node.Color, node.Right);
            }
        }

        private int CheckInvariantNode(NodeColor parentColor, long position)
        {
            if (position == 0)
                return 0;

            Node node = ReadNode(position);
            if (parentColor == NodeColor.Red && node.Color != NodeColor.Black)
                throw new ApplicationException("Invariant violated: both children of every red node are black.");

            int leftCount = CheckInvariantNode(node.Color, node.Left);
            int rightCount = CheckInvariantNode(node.Color, node.Right);

            if (leftCount != rightCount)
                throw new ApplicationException("Invariant violated: Every simple path from a given node to any of its descendant leaves contains the same number of black nodes.");
            return leftCount + (node.Color == NodeColor.Black ? 1 : 0);
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
                Color = (NodeColor)color,
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
