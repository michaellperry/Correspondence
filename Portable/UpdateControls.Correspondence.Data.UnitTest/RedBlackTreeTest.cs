using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace UpdateControls.Correspondence.Data.UnitTest
{
    [TestClass]
    public class RedBlackTreeTest
    {
        private RedBlackTree _redBlackTree;

        [TestInitialize]
        public void Initialize()
        {
            MemoryStream memoryStream = new MemoryStream();
            _redBlackTree = new RedBlackTree(memoryStream, new RedBlackTree.NodeCache());
        }

        [TestMethod]
        public void SearchEmptyTree()
        {
            int hashCode = 234;
            IEnumerable<long> factIds = _redBlackTree.FindFacts(hashCode);
            Assert.IsFalse(factIds.Any(), "There should be no facts in the tree.");
			_redBlackTree.CheckInvariant();
        }

        [TestMethod]
        public void SearchTreeWithOneNode()
        {
            int hashCode = 234;
            long factId = 4L;
            _redBlackTree.AddFact(hashCode, factId);

            IEnumerable<long> factIds = _redBlackTree.FindFacts(hashCode);
            Assert.AreEqual(1, factIds.Count());
            Assert.AreEqual(factId, factIds.Single());
			_redBlackTree.CheckInvariant();
		}

        [TestMethod]
        public void SearchBeforeTreeWithOneNode()
        {
            int hashCode = 234;
            long factId = 4L;
            _redBlackTree.AddFact(hashCode, factId);

            IEnumerable<long> factIds = _redBlackTree.FindFacts(hashCode-1);
            Assert.AreEqual(0, factIds.Count());
			_redBlackTree.CheckInvariant();
		}

        [TestMethod]
        public void SearchAfterTreeWithOneNode()
        {
            int hashCode = 234;
            long factId = 4L;
            _redBlackTree.AddFact(hashCode, factId);

            IEnumerable<long> factIds = _redBlackTree.FindFacts(hashCode + 1);
            Assert.AreEqual(0, factIds.Count());
			_redBlackTree.CheckInvariant();
		}

        [TestMethod]
        public void SearchTreeWithTwoNodes()
        {
            int hashCode1 = 234;
            long factId1 = 4L;
            _redBlackTree.AddFact(hashCode1, factId1);
            int hashCode2 = 345;
            long factId2 = 16L;
            _redBlackTree.AddFact(hashCode2, factId2);

            IEnumerable<long> factIds1 = _redBlackTree.FindFacts(hashCode1);
            Assert.AreEqual(1, factIds1.Count());
            Assert.AreEqual(factId1, factIds1.Single());
            IEnumerable<long> factIds2 = _redBlackTree.FindFacts(hashCode2);
            Assert.AreEqual(1, factIds2.Count());
            Assert.AreEqual(factId2, factIds2.Single());
			_redBlackTree.CheckInvariant();
		}

        [TestMethod]
        public void SearchBeforeTreeWithTwoNodes()
        {
            int hashCode1 = 234;
            long factId1 = 4L;
            _redBlackTree.AddFact(hashCode1, factId1);
            int hashCode2 = 345;
            long factId2 = 16L;
            _redBlackTree.AddFact(hashCode2, factId2);

            IEnumerable<long> factIds = _redBlackTree.FindFacts(hashCode1 - 1);
            Assert.AreEqual(0, factIds.Count());
			_redBlackTree.CheckInvariant();
		}

        [TestMethod]
        public void SearchMiddleTreeWithTwoNodes()
        {
            int hashCode1 = 234;
            long factId1 = 4L;
            _redBlackTree.AddFact(hashCode1, factId1);
            int hashCode2 = 345;
            long factId2 = 16L;
            _redBlackTree.AddFact(hashCode2, factId2);

            IEnumerable<long> factIds = _redBlackTree.FindFacts(hashCode1 + 1);
            Assert.AreEqual(0, factIds.Count());
			_redBlackTree.CheckInvariant();
		}

        [TestMethod]
        public void SearchAfterTreeWithTwoNodes()
        {
            int hashCode1 = 234;
            long factId1 = 4L;
            _redBlackTree.AddFact(hashCode1, factId1);
            int hashCode2 = 345;
            long factId2 = 16L;
            _redBlackTree.AddFact(hashCode2, factId2);

            IEnumerable<long> factIds = _redBlackTree.FindFacts(hashCode1 + 1);
            Assert.AreEqual(0, factIds.Count());
			_redBlackTree.CheckInvariant();
		}

        [TestMethod]
        public void SearchTreeWithTwoNodesSameHash()
        {
            int hashCode = 234;
            long factId1 = 4L;
            _redBlackTree.AddFact(hashCode, factId1);
            long factId2 = 16L;
            _redBlackTree.AddFact(hashCode, factId2);

            List<long> factIds = _redBlackTree.FindFacts(hashCode).ToList();
            Assert.AreEqual(2, factIds.Count);
            Assert.IsTrue(factIds.Contains(factId1));
            Assert.IsTrue(factIds.Contains(factId2));
			_redBlackTree.CheckInvariant();
		}

        [TestMethod]
        public void InOrderInsertion()
        {
            for (int i = 0; i < 16; i++)
            {
				WriteNode(i);
				_redBlackTree.CheckInvariant();
			}

			for (int i = 0; i < 16; i++)
			{
				Assert.AreEqual((long)i, _redBlackTree.FindFacts(i).Single());
			}
		}

        [TestMethod]
        public void ReverseOrderInsertion()
        {
            for (int i = 15; i >= 0; i--)
            {
				WriteNode(i);
				_redBlackTree.CheckInvariant();
			}

			for (int i = 0; i < 16; i++)
			{
				Assert.AreEqual((long)i, _redBlackTree.FindFacts(i).Single());
			}
		}

        [TestMethod]
        public void OutsideInInsertion()
        {
			for (int i = 0; i < 16; i++)
			{
				WriteNode(i);
				_redBlackTree.CheckInvariant();
				WriteNode(31 - i);
				_redBlackTree.CheckInvariant();
			}

			for (int i = 0; i < 32; i++)
			{
				Assert.AreEqual((long)i, _redBlackTree.FindFacts(i).Single());
			}
		}

		[TestMethod]
		public void InsideOutInsertion()
		{
			for (int i = 15; i >= 0; i--)
			{
				WriteNode(i);
				_redBlackTree.CheckInvariant();
				WriteNode(31 - i);
				_redBlackTree.CheckInvariant();
			}

			for (int i = 0; i < 32; i++)
			{
				Assert.AreEqual((long)i, _redBlackTree.FindFacts(i).Single());
			}
		}

		[TestMethod]
		public void ShuffleForwardInsertion()
		{
			for (int i = 0; i < 16; i++)
			{
				WriteNode(i);
				_redBlackTree.CheckInvariant();
				WriteNode(i + 16);
				_redBlackTree.CheckInvariant();
			}

			for (int i = 0; i < 32; i++)
			{
				Assert.AreEqual((long)i, _redBlackTree.FindFacts(i).Single());
			}
		}

		[TestMethod]
		public void ShuffleBackwardInsertion()
		{
			for (int i = 15; i >= 0; i--)
			{
				WriteNode(i);
				_redBlackTree.CheckInvariant();
				WriteNode(i + 16);
				_redBlackTree.CheckInvariant();
			}

			for (int i = 0; i < 32; i++)
			{
				Assert.AreEqual((long)i, _redBlackTree.FindFacts(i).Single());
			}
		}

		[TestMethod]
		public void RandomInsertion()
		{
			Random rand = new Random(39869);
			for (int i = 0; i < 2000; i++)
			{
				WriteNode(rand.Next());
				_redBlackTree.CheckInvariant();
			}
		}

		private void WriteNode(int i)
		{
			//System.Diagnostics.Debug.WriteLine(String.Format("Write node {0}.", i));
			_redBlackTree.AddFact(i, i);
		}
	}
}
