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
            _redBlackTree = new RedBlackTree(memoryStream);
        }

        [TestCleanup]
        public void CheckInvariant()
        {
            _redBlackTree.CheckInvariant();
        }

        [TestMethod]
        public void SearchEmptyTree()
        {
            int hashCode = 234;
            IEnumerable<long> factIds = _redBlackTree.FindFacts(hashCode);
            Assert.IsFalse(factIds.Any(), "There should be no facts in the tree.");
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
        }

        [TestMethod]
        public void SearchBeforeTreeWithOneNode()
        {
            int hashCode = 234;
            long factId = 4L;
            _redBlackTree.AddFact(hashCode, factId);

            IEnumerable<long> factIds = _redBlackTree.FindFacts(hashCode-1);
            Assert.AreEqual(0, factIds.Count());
        }

        [TestMethod]
        public void SearchAfterTreeWithOneNode()
        {
            int hashCode = 234;
            long factId = 4L;
            _redBlackTree.AddFact(hashCode, factId);

            IEnumerable<long> factIds = _redBlackTree.FindFacts(hashCode + 1);
            Assert.AreEqual(0, factIds.Count());
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
        }

        [TestMethod]
        public void InOrderInsertion()
        {
            for (int i = 0; i < 16; i++)
            {
                _redBlackTree.AddFact(i, i);
                _redBlackTree.CheckInvariant();
            }
        }
    }
}
