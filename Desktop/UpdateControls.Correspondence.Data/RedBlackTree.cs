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
				if (leftChild)
					offset = sizeof(byte) + sizeof(int);
				else
					offset = sizeof(byte) + sizeof(int) + sizeof(long);
				_stream.Seek(parent + offset, SeekOrigin.Begin);
				WriteLong(position);

				long left = 0;
				long right = 0;

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
						CheckByte((byte)NodeColor.Red);
						WriteByte((byte)NodeColor.Black);
						_stream.Seek(parentReference.Sibling, SeekOrigin.Begin);
						CheckByte((byte)NodeColor.Red);
						WriteByte((byte)NodeColor.Black);
						if (stack.Count != 0)
						{
							_stream.Seek(grandparentReference.Position, SeekOrigin.Begin);
							CheckByte((byte)NodeColor.Black);
							WriteByte((byte)NodeColor.Red);
						}
					}
					else
					{
						// Parent is red but uncle is black. Need to rotate.
						long newRoot;
						if (!leftChild && parentReference.LeftChild)
						{
							_stream.Seek(position, SeekOrigin.Begin);
							CheckByte((byte)NodeColor.Red);
							WriteByte((byte)NodeColor.Black);
							_stream.Seek(position + sizeof(byte) + sizeof(int), SeekOrigin.Begin);
							CheckLong(left);
							WriteLong(parentReference.Position);
							CheckLong(right);
							WriteLong(grandparentReference.Position);
							_stream.Seek(parentReference.Position + sizeof(byte) + sizeof(int) + sizeof(long), SeekOrigin.Begin);
							CheckLong(position);
							WriteLong(left);
							_stream.Seek(grandparentReference.Position, SeekOrigin.Begin);
							CheckByte((byte)NodeColor.Black);
							WriteByte((byte)NodeColor.Red);
							_stream.Seek(grandparentReference.Position + sizeof(byte) + sizeof(int), SeekOrigin.Begin);
							CheckLong(parentReference.Position);
							WriteLong(right);
							newRoot = position;
						}
						else if (leftChild && !parentReference.LeftChild)
						{
							_stream.Seek(position, SeekOrigin.Begin);
							CheckByte((byte)NodeColor.Red);
							WriteByte((byte)NodeColor.Black);
							_stream.Seek(position + sizeof(byte) + sizeof(int), SeekOrigin.Begin);
							CheckLong(left);
							WriteLong(grandparentReference.Position);
							CheckLong(right);
							WriteLong(parentReference.Position);
							_stream.Seek(parentReference.Position + sizeof(byte) + sizeof(int), SeekOrigin.Begin);
							CheckLong(position);
							WriteLong(right);
							_stream.Seek(grandparentReference.Position, SeekOrigin.Begin);
							CheckByte((byte)NodeColor.Black);
							WriteByte((byte)NodeColor.Red);
							_stream.Seek(grandparentReference.Position + sizeof(byte) + sizeof(int) + sizeof(long), SeekOrigin.Begin);
							CheckLong(parentReference.Position);
							WriteLong(left);
							newRoot = position;
						}
						else if (!leftChild && !parentReference.LeftChild)
						{
							_stream.Seek(parentReference.Position, SeekOrigin.Begin);
							WriteByte((byte)NodeColor.Black);
							_stream.Seek(parentReference.Position + sizeof(byte) + sizeof(int), SeekOrigin.Begin);
							WriteLong(grandparentReference.Position);
							_stream.Seek(grandparentReference.Position, SeekOrigin.Begin);
							WriteByte((byte)NodeColor.Red);
							_stream.Seek(grandparentReference.Position + sizeof(byte) + sizeof(int) + sizeof(long), SeekOrigin.Begin);
							WriteLong(sibling);
							newRoot = parentReference.Position;
						}
						else //if (leftChild && parentReference.LeftChild)
						{
							_stream.Seek(parentReference.Position, SeekOrigin.Begin);
							WriteByte((byte)NodeColor.Black);
							_stream.Seek(parentReference.Position + sizeof(byte) + sizeof(int) + sizeof(long), SeekOrigin.Begin);
							WriteLong(grandparentReference.Position);
							_stream.Seek(grandparentReference.Position, SeekOrigin.Begin);
							WriteByte((byte)NodeColor.Red);
							_stream.Seek(grandparentReference.Position + sizeof(byte) + sizeof(int), SeekOrigin.Begin);
							WriteLong(sibling);
							newRoot = parentReference.Position;
						}

						// Parent becomes the root.
						if (stack.Count == 0)
							_stream.Seek(0, SeekOrigin.Begin);
						else
						{
							NodeReference greatGrandParentReference = stack.Peek();
							_stream.Seek(
								greatGrandParentReference.Position +
								sizeof(byte) + sizeof(int) +
								(grandparentReference.LeftChild ? 0 : sizeof(long)),
								SeekOrigin.Begin);
						}
						WriteLong(newRoot);
						break;
					}
					if (parentReference.LeftChild)
					{
						left = parentReference.Position;
						right = parentReference.Sibling;
					}
					else
					{
						right = parentReference.Position;
						left = parentReference.Sibling;
					}
					leftChild = grandparentReference.LeftChild;
					sibling = grandparentReference.Sibling;
					position = grandparentReference.Position;
				}
			}
		}

        public void CheckInvariant(int expectedCount)
        {
			if (_stream.Length > 0)
			{
				_stream.Seek(0, SeekOrigin.Begin);
				long root = ReadLong();
				Node node = ReadNode(root);
				if (node.Color != NodeColor.Black)
					throw new ApplicationException("Invariant violated: The root node is black.");

				int count = 1;
				CheckInvariantNode(node.Color, node.Left, ref count, node.HashCode, true);
				CheckInvariantNode(node.Color, node.Right, ref count, node.HashCode, false);

				if (expectedCount != count)
					throw new ApplicationException(String.Format("Expected count {0}. Actual {1}.", expectedCount, count));
			}
			else if (expectedCount != 0)
				throw new ApplicationException(String.Format("Expected count {0}. Actual 0.", expectedCount));
        }

        private int CheckInvariantNode(NodeColor parentColor, long position, ref int count, int parentHashCode, bool leftChild)
        {
            if (position == 0)
				return 0;

			++count;
            Node node = ReadNode(position);
            if (parentColor == NodeColor.Red && node.Color != NodeColor.Black)
                throw new ApplicationException("Invariant violated: both children of every red node are black.");

			if (leftChild && node.HashCode >= parentHashCode)
				throw new ApplicationException("Invariant violated: left hash code is less than parent hash code.");
			if (!leftChild && node.HashCode < parentHashCode)
				throw new ApplicationException("Invariant violated: right hash code is greater than or equal to parent hash code.");

            int leftCount = CheckInvariantNode(node.Color, node.Left, ref count, node.HashCode, true);
			int rightCount = CheckInvariantNode(node.Color, node.Right, ref count, node.HashCode, false);

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

		private void CheckByte(byte expected)
		{
			byte b = ReadByte();
			if (b != expected)
				throw new ApplicationException(String.Format("Incorrect byte overwritten. Expected {0}. Actual {1}.", expected, b));
			_stream.Seek(-sizeof(byte), SeekOrigin.Current);
		}

		private void CheckLong(long expected)
		{
			long l = ReadLong();
			if (l != expected)
				throw new ApplicationException(String.Format("Incorrect long overwritten. Expected {0}. Actual {1}.", expected, l));
			_stream.Seek(-sizeof(long), SeekOrigin.Current);
		}
	}
}
