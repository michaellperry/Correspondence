using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UpdateControls.Correspondence.Data
{
    public class RedBlackTree : StreamBasedDataStructure
    {
        public enum NodeColor : byte
        {
            Black = 0,
            Red = 1
        }

        public class Node
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

        public class NodeCache
        {
            public long Root;
            public Dictionary<long, Node> NodeByPosition = new Dictionary<long, Node>();
        }

        private NodeCache _nodeCache;

        public RedBlackTree(Stream stream, NodeCache nodeCache) :
            base(stream)
        {
            _nodeCache = nodeCache;
        }

        public IEnumerable<long> FindFacts(int hashCode)
        {
            if (_stream.Length > 0)
            {
                long current = ReadRoot();
                return FindFactsFrom(hashCode, current);
            }
            else
                return Enumerable.Empty<long>();
        }

        private IEnumerable<long> FindFactsFrom(int hashCode, long current)
        {
            if (current == 0)
                yield break;

            Node node;

            // Search for the beginning of the hash code.
            do
            {
                node = ReadNode(current);
                if (hashCode == node.HashCode)
                {
                    // Walk through all nodes with this hash code.
                    yield return node.FactId;
                    foreach (long leftId in FindFactsFrom(hashCode, node.Left))
                        yield return leftId;
                    foreach (long rightId in FindFactsFrom(hashCode, node.Right))
                        yield return rightId;
                    break;
                }

                if (hashCode > node.HashCode)
                    current = node.Right;
                else
                    current = node.Left;
            }
            while (current != 0);
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
				Stack<NodeReference> stack = new Stack<NodeReference>();
				Node node;
				long parent = 0;
				long child = ReadRoot();
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
                UpdateNodeAt(parent, n =>
                {
                    if (leftChild)
                        n.Left = position;
                    else
                        n.Right = position;
                });

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
                        UpdateNodeAt(parentReference.Position, n =>
                        {
                            n.Color = NodeColor.Black;
                        });

                        uncleNode.Color = NodeColor.Black;
                        WriteNode(parentReference.Sibling, uncleNode);

                        if (stack.Count != 0)
						{
                            UpdateNodeAt(grandparentReference.Position, n =>
                            {
                                n.Color = NodeColor.Red;
                            });
						}
					}
					else
					{
						// Parent is red but uncle is black. Need to rotate.
						long newRoot;
						if (!leftChild && parentReference.LeftChild)
						{
                            UpdateNodeAt(position, n =>
                            {
                                n.Color = NodeColor.Black;
                                n.Left = parentReference.Position;
                                n.Right = grandparentReference.Position;
                            });

                            UpdateNodeAt(parentReference.Position, n =>
                            {
                                n.Right = left;
                            });

                            UpdateNodeAt(grandparentReference.Position, n =>
                            {
                                n.Color = NodeColor.Red;
                                n.Left = right;
                            });

							newRoot = position;
						}
						else if (leftChild && !parentReference.LeftChild)
						{
                            UpdateNodeAt(position, n =>
                            {
                                n.Color = NodeColor.Black;
                                n.Left = grandparentReference.Position;
                                n.Right = parentReference.Position;
                            });

                            UpdateNodeAt(parentReference.Position, n =>
                            {
                                n.Left = right;
                            });

                            UpdateNodeAt(grandparentReference.Position, n =>
                            {
                                n.Color = NodeColor.Red;
                                n.Right = left;
                            });

							newRoot = position;
						}
						else if (!leftChild && !parentReference.LeftChild)
						{
                            UpdateNodeAt(parentReference.Position, n =>
                            {
                                n.Color = NodeColor.Black;
                                n.Left = grandparentReference.Position;
                            });

                            UpdateNodeAt(grandparentReference.Position, n =>
                            {
                                n.Color = NodeColor.Red;
                                n.Right = sibling;
                            });

							newRoot = parentReference.Position;
						}
						else //if (leftChild && parentReference.LeftChild)
						{
                            UpdateNodeAt(parentReference.Position, n =>
                            {
                                n.Color = NodeColor.Black;
                                n.Right = grandparentReference.Position;
                            });

                            UpdateNodeAt(grandparentReference.Position, n =>
                            {
                                n.Color = NodeColor.Red;
                                n.Left = sibling;
                            });

							newRoot = parentReference.Position;
						}

						// Parent becomes the root.
                        if (stack.Count == 0)
                        {
                            WriteRoot(newRoot);
                        }
                        else
                        {
                            NodeReference greatGrandParentReference = stack.Peek();
                            UpdateNodeAt(greatGrandParentReference.Position, n =>
                            {
                                if (grandparentReference.LeftChild)
                                    n.Left = newRoot;
                                else
                                    n.Right = newRoot;
                            });
                        }
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

        public void CheckInvariant()
        {
			if (_stream.Length > 0)
			{
                int expectedCount = (int)((GetSize() - 8) / (long)(1 + 4 + 8 + 8 + 8));
                long totalSize = (long)8 + expectedCount * (long)(1 + 4 + 8 + 8 + 8);
                if (GetSize() != totalSize)
                    throw new InvariantException(String.Format("The file size {0} does not contain a whole number of nodes.", GetSize()));

                _stream.Seek(0, SeekOrigin.Begin);
				long root = ReadLong();
				Node node = ReadNode(root);
				if (node.Color != NodeColor.Black)
					throw new InvariantException("Invariant violated: The root node is black.");

				int count = 1;
				CheckInvariantNode(node.Color, node.Left, ref count, node.HashCode, true);
				CheckInvariantNode(node.Color, node.Right, ref count, node.HashCode, false);

				if (expectedCount != count)
					throw new InvariantException(String.Format("Expected count {0}. Actual {1}.", expectedCount, count));
			}
        }

        private int CheckInvariantNode(NodeColor parentColor, long position, ref int count, int parentHashCode, bool leftChild)
        {
            if (position == 0)
				return 0;

			++count;
            Node node = ReadNode(position);
            if (parentColor == NodeColor.Red && node.Color != NodeColor.Black)
                throw new InvariantException("Invariant violated: both children of every red node are black.");

			if (leftChild && node.HashCode > parentHashCode)
				throw new InvariantException("Invariant violated: left hash code is less than parent hash code.");
			if (!leftChild && node.HashCode < parentHashCode)
				throw new InvariantException("Invariant violated: right hash code is greater than parent hash code.");

            int leftCount = CheckInvariantNode(node.Color, node.Left, ref count, node.HashCode, true);
			int rightCount = CheckInvariantNode(node.Color, node.Right, ref count, node.HashCode, false);

            if (leftCount != rightCount)
                throw new InvariantException("Invariant violated: Every simple path from a given node to any of its descendant leaves contains the same number of black nodes.");
            return leftCount + (node.Color == NodeColor.Black ? 1 : 0);
        }

        private void UpdateNodeAt(long position, Action<Node> update)
        {
            Node node = ReadNode(position);
            update(node);
            WriteNode(position, node);
        }

        private Node ReadNode(long position)
        {
            Node node = null;

            if (!_nodeCache.NodeByPosition.TryGetValue(position, out node))
            {
                _stream.Seek(position, SeekOrigin.Begin);
                var nodeData = new byte[1 + 4 + 8 + 8 + 8];
                ReadBytes(nodeData);
                byte color = nodeData[0];
                int hashCode = BitConverter.ToInt32(nodeData, 1);
                long left = BitConverter.ToInt64(nodeData, 1 + 4);
                long right = BitConverter.ToInt64(nodeData, 1 + 4 + 8);
                long factId = BitConverter.ToInt64(nodeData, 1 + 4 + 8 + 8);

                node = new Node
                {
                    Color = (NodeColor)color,
                    HashCode = hashCode,
                    Left = left,
                    Right = right,
                    FactId = factId
                };
                _nodeCache.NodeByPosition.Add(position, node);
            }
            return node;
        }

        private void WriteNode(long position, RedBlackTree.Node node)
        {
            var nodeData = new byte[1 + 4 + 8 + 8 + 8];
            nodeData[0] = (byte)node.Color;
            WriteInt(node.HashCode, nodeData, 1);
            WriteLong(node.Left, nodeData, 1 + 4);
            WriteLong(node.Right, nodeData, 1 + 4 + 8);
            WriteLong(node.FactId, nodeData, 1 + 4 + 8 + 8);

            SeekTo(position);
            WriteBytes(nodeData);
        }

        private void WriteInt(int value, Byte[] data, int start)
        {
            data[start + 0] = (byte)(value >> 0  & 0xff);
            data[start + 1] = (byte)(value >> 8  & 0xff);
            data[start + 2] = (byte)(value >> 16 & 0xff);
            data[start + 3] = (byte)(value >> 24 & 0xff);
        }

        private void WriteLong(long value, Byte[] data, int start)
        {
            data[start + 0] = (byte)(value >> 0  & 0xff);
            data[start + 1] = (byte)(value >> 8  & 0xff);
            data[start + 2] = (byte)(value >> 16 & 0xff);
            data[start + 3] = (byte)(value >> 24 & 0xff);
            data[start + 4] = (byte)(value >> 32 & 0xff);
            data[start + 5] = (byte)(value >> 40 & 0xff);
            data[start + 6] = (byte)(value >> 48 & 0xff);
            data[start + 7] = (byte)(value >> 56 & 0xff);
        }

        private long WriteNode(NodeColor nodeColor, int hashCode, long factId)
        {
            long position = GetSize();
            Node node = new Node
            {
                Color = nodeColor,
                HashCode = hashCode,
                Left = 0,
                Right = 0,
                FactId = factId
            };
            WriteNode(position, node);
            _nodeCache.NodeByPosition.Add(position, node);
            return position;
        }

        private long ReadRoot()
        {
            if (_nodeCache.Root == 0)
            {
                SeekTo(0);
                _nodeCache.Root = ReadLong();
            }
            return _nodeCache.Root;
        }

        private void WriteRoot(long root)
        {
            SeekTo(0);
            WriteLong(root);
            _nodeCache.Root = root;
        }
	}
}
