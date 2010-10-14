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
            List<long> predecessors = _tree.GetPredecessors(factId, roleId);
            Assert.AreEqual(0, predecessors.Count);
            List<long> successors = _tree.GetSuccessors(factId, roleId).ToList();
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

            List<long> predecessors = _tree.GetPredecessors(childFactId, roleId);
            Assert.AreEqual(1, predecessors.Count);
            Assert.AreEqual(rootFactId, predecessors[0]);

            List<long> successors = _tree.GetSuccessors(rootFactId, roleId).ToList();
            Assert.AreEqual(1, successors.Count);
            Assert.AreEqual(childFactId, successors[0]);
        }
    }
}
