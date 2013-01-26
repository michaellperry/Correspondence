using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Streams;

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

        public RedBlackTree(IRandomAccessStream randomAccessStream) :
            base(randomAccessStream)
        {
        }

        public async Task<List<long>> FindFactsAsync(int hashCode)
        {
            List<long> factIds = new List<long>();
            if (!Empty())
            {
                SeekTo(0);
                long current = await ReadLongAsync();
                Node node;

                // Search for the beginning of the hash code.
                do
                {
                    node = await ReadNodeAsync(current);
                    if (hashCode == node.HashCode)
                    {
                        // Walk through all nodes with this hash code.
                        factIds.Add(node.FactId);
                        current = node.Right;
                        while (current != 0)
                        {
                            node = await ReadNodeAsync(current);
                            if (hashCode == node.HashCode)
                            {
                                factIds.Add(node.FactId);
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
            return factIds;
        }

        public async Task AddFactAsync(int hashCode, long factId)
		{
            if (Empty())
			{
                await WriteLongAsync(sizeof(long));
                await WriteNodeAsync(NodeColor.Black, hashCode, factId);
			}
			else
			{
				// Find the inseration point.
				SeekTo(0);

				Stack<NodeReference> stack = new Stack<NodeReference>();
				Node node;
				long parent = 0;
                long child = await ReadLongAsync();
				long sibling = 0;
				bool leftChild = true;

				do
				{
					parent = child;
                    node = await ReadNodeAsync(parent);
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
                long position = await WriteNodeAsync(NodeColor.Red, hashCode, factId);
				int offset;
				if (leftChild)
					offset = sizeof(byte) + sizeof(int);
				else
					offset = sizeof(byte) + sizeof(int) + sizeof(long);
				SeekTo(parent + offset);
                await WriteLongAsync(position);

				long left = 0;
				long right = 0;

				// Change colors and rotate.
				while (stack.Count >= 2)
				{
					NodeReference parentReference = stack.Pop();
					NodeReference grandparentReference = stack.Pop();

					if (parentReference.Color == NodeColor.Black)
						break;

                    Node uncleNode = await ReadNodeAsync(parentReference.Sibling);
					if (uncleNode.Color == NodeColor.Red)
					{
						// Both the parent and uncle are red, so change both to black and the grandparent to red.
						SeekTo(parentReference.Position);
                        await WriteByteAsync((byte)NodeColor.Black);
						SeekTo(parentReference.Sibling);
                        await WriteByteAsync((byte)NodeColor.Black);
						if (stack.Count != 0)
						{
							SeekTo(grandparentReference.Position);
                            await WriteByteAsync((byte)NodeColor.Red);
						}
					}
					else
					{
						// Parent is red but uncle is black. Need to rotate.
						long newRoot;
						if (!leftChild && parentReference.LeftChild)
						{
							SeekTo(position);
                            await WriteByteAsync((byte)NodeColor.Black);
							SeekTo(position + sizeof(byte) + sizeof(int));
                            await WriteLongAsync(parentReference.Position);
                            await WriteLongAsync(grandparentReference.Position);
							SeekTo(parentReference.Position + sizeof(byte) + sizeof(int) + sizeof(long));
                            await WriteLongAsync(left);
							SeekTo(grandparentReference.Position);
                            await WriteByteAsync((byte)NodeColor.Red);
							SeekTo(grandparentReference.Position + sizeof(byte) + sizeof(int));
                            await WriteLongAsync(right);
							newRoot = position;
						}
						else if (leftChild && !parentReference.LeftChild)
						{
							SeekTo(position);
                            await WriteByteAsync((byte)NodeColor.Black);
							SeekTo(position + sizeof(byte) + sizeof(int));
                            await WriteLongAsync(grandparentReference.Position);
                            await WriteLongAsync(parentReference.Position);
							SeekTo(parentReference.Position + sizeof(byte) + sizeof(int));
                            await WriteLongAsync(right);
							SeekTo(grandparentReference.Position);
                            await WriteByteAsync((byte)NodeColor.Red);
							SeekTo(grandparentReference.Position + sizeof(byte) + sizeof(int) + sizeof(long));
                            await WriteLongAsync(left);
							newRoot = position;
						}
						else if (!leftChild && !parentReference.LeftChild)
						{
							SeekTo(parentReference.Position);
                            await WriteByteAsync((byte)NodeColor.Black);
							SeekTo(parentReference.Position + sizeof(byte) + sizeof(int));
                            await WriteLongAsync(grandparentReference.Position);
							SeekTo(grandparentReference.Position);
                            await WriteByteAsync((byte)NodeColor.Red);
							SeekTo(grandparentReference.Position + sizeof(byte) + sizeof(int) + sizeof(long));
                            await WriteLongAsync(sibling);
							newRoot = parentReference.Position;
						}
						else //if (leftChild && parentReference.LeftChild)
						{
							SeekTo(parentReference.Position);
                            await WriteByteAsync((byte)NodeColor.Black);
							SeekTo(parentReference.Position + sizeof(byte) + sizeof(int) + sizeof(long));
                            await WriteLongAsync(grandparentReference.Position);
							SeekTo(grandparentReference.Position);
                            await WriteByteAsync((byte)NodeColor.Red);
							SeekTo(grandparentReference.Position + sizeof(byte) + sizeof(int));
                            await WriteLongAsync(sibling);
							newRoot = parentReference.Position;
						}

						// Parent becomes the root.
						if (stack.Count == 0)
							SeekTo(0);
						else
						{
							NodeReference greatGrandParentReference = stack.Peek();
							SeekTo(
								greatGrandParentReference.Position +
								sizeof(byte) + sizeof(int) +
								(grandparentReference.LeftChild ? 0 : sizeof(long)));
						}
                        await WriteLongAsync(newRoot);
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

        //public async Task CheckInvariant(int expectedCount)
        //{
        //    if (!Empty())
        //    {
        //        SeekTo(0);
        //        long root = await ReadLong();
        //        Node node = await ReadNode(root);
        //        if (node.Color != NodeColor.Black)
        //            throw new InvariantException("Invariant violated: The root node is black.");
        //        int count = 1;
        //        await CheckInvariantNode(node.Color, node.Left, ref count, node.HashCode, true);
        //        await CheckInvariantNode(node.Color, node.Right, ref count, node.HashCode, false);
        //        if (expectedCount != count)
        //            throw new InvariantException(String.Format("Expected count {0}. Actual {1}.", expectedCount, count));
        //    }
        //    else if (expectedCount != 0)
        //        throw new InvariantException(String.Format("Expected count {0}. Actual 0.", expectedCount));
        //}

        //private async Task<int> CheckInvariantNode(NodeColor parentColor, long position, ref int count, int parentHashCode, bool leftChild)
        //{
        //    if (position == 0)
        //        return 0;

        //    ++count;
        //    Node node = await ReadNode(position);
        //    if (parentColor == NodeColor.Red && node.Color != NodeColor.Black)
        //        throw new InvariantException("Invariant violated: both children of every red node are black.");

        //    if (leftChild && node.HashCode >= parentHashCode)
        //        throw new InvariantException("Invariant violated: left hash code is less than parent hash code.");
        //    if (!leftChild && node.HashCode < parentHashCode)
        //        throw new InvariantException("Invariant violated: right hash code is greater than or equal to parent hash code.");

        //    int leftCount = await CheckInvariantNode(node.Color, node.Left, ref count, node.HashCode, true);
        //    int rightCount = await CheckInvariantNode(node.Color, node.Right, ref count, node.HashCode, false);

        //    if (leftCount != rightCount)
        //        throw new InvariantException("Invariant violated: Every simple path from a given node to any of its descendant leaves contains the same number of black nodes.");
        //    return leftCount + (node.Color == NodeColor.Black ? 1 : 0);
        //}

        private async Task<Node> ReadNodeAsync(long position)
        {
            SeekTo(position);
            byte color = await ReadByteAsync();
            int hashCode = await ReadIntAsync();
            long left = await ReadLongAsync();
            long right = await ReadLongAsync();
            long factId = await ReadLongAsync();

            return new Node
            {
                Color = (NodeColor)color,
                HashCode = hashCode,
                Left = left,
                Right = right,
                FactId = factId
            };
        }

        private async Task<long> WriteNodeAsync(NodeColor nodeColor, int hashCode, long factId)
        {
            long position = SeekToEnd();
            await WriteByteAsync((byte)nodeColor);
            await WriteIntAsync(hashCode);
            await WriteLongAsync(0);
            await WriteLongAsync(0);
            await WriteLongAsync(factId);
            return position;
        }
    }
}
