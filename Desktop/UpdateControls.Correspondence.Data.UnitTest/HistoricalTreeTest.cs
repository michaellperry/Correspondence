using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace UpdateControls.Correspondence.Data.UnitTest
{
    [TestClass]
    public class HistoricalTreeTest
    {
        private MemoryStream _memoryStream;
        private HistoricalTree _tree;

        [TestInitialize]
        public void Initialize()
        {
            _memoryStream = new MemoryStream();
            _tree = new HistoricalTree(_memoryStream);
        }

        [TestMethod]
        public void InsertRootNode()
        {
            int roleId = 42;
            long factId = _tree.Save(new HistoricalTreeFact(1, new byte[0]));
            List<long> predecessors = _tree.GetPredecessorsInRole(factId, roleId);
            Assert.AreEqual(0, predecessors.Count);
            List<long> successors = _tree.GetSuccessorsInRole(factId, roleId).ToList();
            Assert.AreEqual(0, successors.Count);

            Assert.AreEqual(4, factId);
            byte[] actual = _memoryStream.ToArray();
            Assert.AreEqual(85, actual[0]); // U
            Assert.AreEqual(79, actual[1]); // O
            Assert.AreEqual(80, actual[2]); // P
            Assert.AreEqual(65, actual[3]); // A
        }

        [TestMethod]
        public void InsertChildNode()
        {
            int roleId = 42;
            long rootFactId = _tree.Save(new HistoricalTreeFact(1, new byte[0]));
            long childFactId = _tree.Save(new HistoricalTreeFact(2, new byte[0])
                .AddPredecessor(roleId, rootFactId));

            List<long> predecessors = _tree.GetPredecessorsInRole(childFactId, roleId);
            Assert.AreEqual(1, predecessors.Count);
            Assert.AreEqual(rootFactId, predecessors[0]);

            List<long> successors = _tree.GetSuccessorsInRole(rootFactId, roleId).ToList();
            Assert.AreEqual(1, successors.Count);
            Assert.AreEqual(childFactId, successors[0]);
        }

        [TestMethod]
        public void InsertTwoChildNodes()
        {
            int roleId = 42;
            long rootFactId = _tree.Save(new HistoricalTreeFact(1, new byte[0]));
            long firstChildFactId = _tree.Save(new HistoricalTreeFact(2, new byte[0])
                .AddPredecessor(roleId, rootFactId));
            long secondChildFactId = _tree.Save(new HistoricalTreeFact(2, new byte[0])
                .AddPredecessor(roleId, rootFactId));

            {
                List<long> predecessors = _tree.GetPredecessorsInRole(firstChildFactId, roleId);
                Assert.AreEqual(1, predecessors.Count);
                Assert.AreEqual(rootFactId, predecessors[0]);
            }

            {
                List<long> predecessors = _tree.GetPredecessorsInRole(secondChildFactId, roleId);
                Assert.AreEqual(1, predecessors.Count);
                Assert.AreEqual(rootFactId, predecessors[0]);
            }

            {
                List<long> successors = _tree.GetSuccessorsInRole(rootFactId, roleId).ToList();
                Assert.AreEqual(2, successors.Count);
                Assert.AreEqual(secondChildFactId, successors[0]);
                Assert.AreEqual(firstChildFactId, successors[1]);
            }
        }

        [TestMethod]
        public void InsertChildWithDifferentRole()
        {
            int firstRoleId = 42;
            int secondRoleId = 43;
            long rootFactId = _tree.Save(new HistoricalTreeFact(1, new byte[0]));
            long firstChildFactId = _tree.Save(new HistoricalTreeFact(2, new byte[0])
                .AddPredecessor(firstRoleId, rootFactId));
            long secondChildFactId = _tree.Save(new HistoricalTreeFact(2, new byte[0])
                .AddPredecessor(secondRoleId, rootFactId));

            {
                List<long> predecessors = _tree.GetPredecessorsInRole(firstChildFactId, firstRoleId);
                Assert.AreEqual(1, predecessors.Count);
                Assert.AreEqual(rootFactId, predecessors[0]);
                Assert.IsFalse(_tree.GetPredecessorsInRole(firstChildFactId, secondRoleId).Any());
            }

            {
                List<long> predecessors = _tree.GetPredecessorsInRole(secondChildFactId, secondRoleId);
                Assert.AreEqual(1, predecessors.Count);
                Assert.AreEqual(rootFactId, predecessors[0]);
                Assert.IsFalse(_tree.GetPredecessorsInRole(secondChildFactId, firstRoleId).Any());
            }

            {
                List<long> successors = _tree.GetSuccessorsInRole(rootFactId, firstRoleId).ToList();
                Assert.AreEqual(1, successors.Count);
                Assert.AreEqual(firstChildFactId, successors[0]);
            }

            {
                List<long> successors = _tree.GetSuccessorsInRole(rootFactId, secondRoleId).ToList();
                Assert.AreEqual(1, successors.Count);
                Assert.AreEqual(secondChildFactId, successors[0]);
            }
        }

        [TestMethod]
        public void CanReadFact()
        {
            long factId = _tree.Save(new HistoricalTreeFact(37, Encoding.ASCII.GetBytes("Blob")));
            HistoricalTreeFact fact = _tree.Load(factId);

            Assert.AreEqual("Blob", Encoding.ASCII.GetString(fact.Data));
            Assert.AreEqual(37, fact.FactTypeId);
            Assert.AreEqual(0, fact.Predecessors.Count());
        }
    }
}
