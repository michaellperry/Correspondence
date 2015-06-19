using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Correspondence.Data
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
        private byte[] _nodeData = new byte[1 + 4 + 8 + 8 + 8];
        private Stack<NodeReference> _nodeStack = new Stack<NodeReference>();

        public RedBlackTree(IRandomAccessStream randomAccessStream, NodeCache nodeCache) :
            base(randomAccessStream)
        {
            _nodeCache = nodeCache;
        }

        public async Task<List<long>> FindFactsAsync(int hashCode)
        {
            List<long> factIds = new List<long>();
            if (!Empty())
            {
                long current = await ReadRootAsync();
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
                await WriteRootAsync(sizeof(long));
                await AppendNodeAsync(NodeColor.Black, hashCode, factId);
            }
            else
            {
                // Find the inseration point.
                Node node;
                long parent = 0;
                long child = await ReadRootAsync();
                long sibling = 0;
                bool leftChild = true;

                do
                {
                    parent = child;
                    node = await ReadNodeAsync(parent);
                    _nodeStack.Push(new NodeReference
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
                long position = await AppendNodeAsync(NodeColor.Red, hashCode, factId);
                await UpdateNodeAtAsync(parent, n =>
                {
                    if (leftChild)
                        n.Left = position;
                    else
                        n.Right = position;
                });

                long left = 0;
                long right = 0;

                // Change colors and rotate.
                while (_nodeStack.Count >= 2)
                {
                    NodeReference parentReference = _nodeStack.Pop();
                    NodeReference grandparentReference = _nodeStack.Pop();

                    if (parentReference.Color == NodeColor.Black)
                        break;

                    Node uncleNode = await ReadNodeAsync(parentReference.Sibling);
                    if (uncleNode.Color == NodeColor.Red)
                    {
                        // Both the parent and uncle are red, so change both to black and the grandparent to red.
                        await UpdateNodeAtAsync(parentReference.Position, n =>
                        {
                            n.Color = NodeColor.Black;
                        });

                        uncleNode.Color = NodeColor.Black;
                        await WriteNodeAsync(parentReference.Sibling, uncleNode);

                        if (_nodeStack.Count != 0)
                        {
                            await UpdateNodeAtAsync(grandparentReference.Position, n =>
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
                            await UpdateNodeAtAsync(position, n =>
                            {
                                n.Color = NodeColor.Black;
                                n.Left = parentReference.Position;
                                n.Right = grandparentReference.Position;
                            });

                            await UpdateNodeAtAsync(parentReference.Position, n =>
                            {
                                n.Right = left;
                            });

                            await UpdateNodeAtAsync(grandparentReference.Position, n =>
                            {
                                n.Color = NodeColor.Red;
                                n.Left = right;
                            });

                            newRoot = position;
                        }
                        else if (leftChild && !parentReference.LeftChild)
                        {
                            await UpdateNodeAtAsync(position, n =>
                            {
                                n.Color = NodeColor.Black;
                                n.Left = grandparentReference.Position;
                                n.Right = parentReference.Position;
                            });

                            await UpdateNodeAtAsync(parentReference.Position, n =>
                            {
                                n.Left = right;
                            });

                            await UpdateNodeAtAsync(grandparentReference.Position, n =>
                            {
                                n.Color = NodeColor.Red;
                                n.Right = left;
                            });

                            newRoot = position;
                        }
                        else if (!leftChild && !parentReference.LeftChild)
                        {
                            await UpdateNodeAtAsync(parentReference.Position, n =>
                            {
                                n.Color = NodeColor.Black;
                                n.Left = grandparentReference.Position;
                            });

                            await UpdateNodeAtAsync(grandparentReference.Position, n =>
                            {
                                n.Color = NodeColor.Red;
                                n.Right = sibling;
                            });

                            newRoot = parentReference.Position;
                        }
                        else //if (leftChild && parentReference.LeftChild)
                        {
                            await UpdateNodeAtAsync(parentReference.Position, n =>
                            {
                                n.Color = NodeColor.Black;
                                n.Right = grandparentReference.Position;
                            });

                            await UpdateNodeAtAsync(grandparentReference.Position, n =>
                            {
                                n.Color = NodeColor.Red;
                                n.Left = sibling;
                            });

                            newRoot = parentReference.Position;
                        }

                        // Parent becomes the root.
                        if (_nodeStack.Count == 0)
                        {
                            await WriteRootAsync(newRoot);
                        }
                        else
                        {
                            NodeReference greatGrandParentReference = _nodeStack.Peek();
                            await UpdateNodeAtAsync(greatGrandParentReference.Position, n =>
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

        private struct CheckInvariantNodeResult
        {
            public int nodeCount;
            public int blackCount;
        }

        public async Task CheckInvariant()
        {
            if (!Empty())
            {
                int expectedCount = (int)((GetSize() - (ulong)8) / (ulong)(1 + 4 + 8 + 8 + 8));
                ulong totalSize = (ulong)8 + (ulong)expectedCount * (ulong)(1 + 4 + 8 + 8 + 8);
                if (GetSize() != totalSize)
                    throw new InvariantException(String.Format("The file size {0} does not contain a whole number of nodes.", GetSize()));

                SeekTo(0);
                long root = await ReadLongAsync();
                Node node = await ReadNodeAsync(root);
                if (node.Color != NodeColor.Black)
                    throw new InvariantException("Invariant violated: The root node is black.");
                var leftResult = await CheckInvariantNode(node.Color, node.Left, node.HashCode, true);
                var rightResult = await CheckInvariantNode(node.Color, node.Right, node.HashCode, false);
                int count = 1 + leftResult.nodeCount + rightResult.nodeCount;
                if (expectedCount != count)
                    throw new InvariantException(String.Format("Expected count {0}. Actual {1}.", expectedCount, count));
            }
        }

        private async Task<CheckInvariantNodeResult> CheckInvariantNode(NodeColor parentColor, long position, int parentHashCode, bool leftChild)
        {
            if (position == 0)
                return new CheckInvariantNodeResult { nodeCount = 0, blackCount = 0 };

            Node node = await ReadNodeAsync(position);
            if (parentColor == NodeColor.Red && node.Color != NodeColor.Black)
                throw new InvariantException("Invariant violated: both children of every red node are black.");

            if (leftChild && node.HashCode >= parentHashCode)
                throw new InvariantException("Invariant violated: left hash code is less than parent hash code.");
            if (!leftChild && node.HashCode < parentHashCode)
                throw new InvariantException("Invariant violated: right hash code is greater than or equal to parent hash code.");

            var leftResult = await CheckInvariantNode(node.Color, node.Left, node.HashCode, true);
            var rightResult = await CheckInvariantNode(node.Color, node.Right, node.HashCode, false);

            if (leftResult.blackCount != rightResult.blackCount)
                throw new InvariantException("Invariant violated: Every simple path from a given node to any of its descendant leaves contains the same number of black nodes.");
            return new CheckInvariantNodeResult
            {
                blackCount =
                    leftResult.blackCount +
                    (node.Color == NodeColor.Black ? 1 : 0),
                nodeCount = leftResult.nodeCount + rightResult.nodeCount + 1
            };
        }

        private async Task UpdateNodeAtAsync(long position, Action<Node> update)
        {
            Node node = await ReadNodeAsync(position);
            update(node);
            await WriteNodeAsync(position, node);
        }

        private async Task<Node> ReadNodeAsync(long position)
        {
            Node node = null;

            if (!_nodeCache.NodeByPosition.TryGetValue(position, out node))
            {
                SeekTo(position);
                await ReadBytesAsync(_nodeData);
                byte color = _nodeData[0];
                int hashCode = BitConverter.ToInt32(_nodeData, 1);
                long left = BitConverter.ToInt64(_nodeData, 1 + 4);
                long right = BitConverter.ToInt64(_nodeData, 1 + 4 + 8);
                long factId = BitConverter.ToInt64(_nodeData, 1 + 4 + 8 + 8);

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

        private async Task WriteNodeAsync(long position, RedBlackTree.Node node)
        {
            _nodeData[0] = (byte)node.Color;
            WriteInt(node.HashCode, _nodeData, 1);
            WriteLong(node.Left, _nodeData, 1 + 4);
            WriteLong(node.Right, _nodeData, 1 + 4 + 8);
            WriteLong(node.FactId, _nodeData, 1 + 4 + 8 + 8);

            SeekTo(position);
            await WriteBytesAsync(_nodeData);
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

        private async Task<long> AppendNodeAsync(NodeColor nodeColor, int hashCode, long factId)
        {
            long position = (long)GetSize();
            Node node = new Node
            {
                Color = nodeColor,
                HashCode = hashCode,
                Left = 0,
                Right = 0,
                FactId = factId
            };
            await WriteNodeAsync(position, node);
            _nodeCache.NodeByPosition.Add(position, node);
            return position;
        }

        private async Task<long> ReadRootAsync()
        {
            if (_nodeCache.Root == 0)
            {
                SeekTo(0);
                _nodeCache.Root = await ReadLongAsync();
            }
            return _nodeCache.Root;
        }

        private async Task WriteRootAsync(long root)
        {
            SeekTo(0);
            await WriteLongAsync(root);
            _nodeCache.Root = root;
        }
    }
}
