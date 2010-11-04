using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.UnitTest
{
    [TestClass]
    public class FactTreeMementoMergeTest
    {
        [TestMethod]
        public void GivenTwoEmptyTreesWhenTheyAreMergedTheResultIsEmpty()
        {
            FactTreeMemento firstTree = new FactTreeMemento(0,0);
            FactTreeMemento secondTree = new FactTreeMemento(0,0);
            FactTreeMemento mergedTree = firstTree.Merge(secondTree);

            Assert.IsNotNull(mergedTree);
            IEnumerable<IdentifiedFactMemento> facts = mergedTree.Facts;
            Assert.IsNotNull(facts);
            Assert.IsFalse(facts.Any());
        }

        [TestMethod]
        public void GivenOneEmptyTreeAndOneFullTreeIsTheResultTheSameAsTheFullTree()
        {
            FactTreeMemento emptyTree = new FactTreeMemento(0, 0);
            FactTreeMemento fullTree;
            IdentifiedFactMemento identifiedMemento;
            GetFullTree(out fullTree, out identifiedMemento);
            FactTreeMemento mergedTree = fullTree.Merge(emptyTree);

            Assert.IsNotNull(mergedTree);
            IEnumerable<IdentifiedFactMemento> facts = mergedTree.Facts;
            Assert.IsNotNull(facts);
            Assert.AreEqual(1,facts.Count());
            IdentifiedFactMemento resultMemento = facts.Single();
            Assert.AreEqual(identifiedMemento, resultMemento);
        }

        [TestMethod]
        public void GivenFullTreeAndOneEmptyTreeIsTheResultTheSameAsTheFullTree()
        {
            FactTreeMemento emptyTree = new FactTreeMemento(0, 0);
            FactTreeMemento fullTree;
            IdentifiedFactMemento identifiedMemento;
            GetFullTree(out fullTree, out identifiedMemento);
            FactTreeMemento mergedTree = emptyTree.Merge(fullTree);

            Assert.IsNotNull(mergedTree);
            IEnumerable<IdentifiedFactMemento> facts = mergedTree.Facts;
            Assert.IsNotNull(facts);
            Assert.AreEqual(1, facts.Count());
            IdentifiedFactMemento resultMemento = facts.Single();
            Assert.AreEqual(identifiedMemento, resultMemento);
        }

        [TestMethod]
        public void GivenEmptyTreeAndOneFullTreeIsTheResultTheSameAsTheFullTree()
        {
            FactTreeMemento emptyTree = new FactTreeMemento(0, 0);
            FactTreeMemento fullTree;
            IdentifiedFactMemento identifiedMemento;
            GetFullTree(out fullTree, out identifiedMemento);
            FactTreeMemento mergedTree = emptyTree.Merge(fullTree);

            Assert.IsNotNull(mergedTree);
            IEnumerable<IdentifiedFactMemento> facts = mergedTree.Facts;
            Assert.IsNotNull(facts);
            Assert.AreEqual(1, facts.Count());
            IdentifiedFactMemento resultMemento = facts.Single();
            Assert.AreEqual(identifiedMemento, resultMemento);
        }

        [TestMethod]
        public void GivenEarlyAndLateTreeTheResultsTimeStampShouldBeTheMoreRecent()
        {
            FactTreeMemento earlyTree = new FactTreeMemento(0, 0);
            FactTreeMemento lateTree = new FactTreeMemento(0, 1);

            FactTreeMemento mergedTree = earlyTree.Merge(lateTree);
            Assert.AreEqual(0, mergedTree.DatabaseId);
            Assert.AreEqual(1, mergedTree.Timestamp);
        }

        [TestMethod]
        public void GivenLateAndEarlyTreeTheResultsTimeStampShouldBeTheMoreRecent()
        {
            FactTreeMemento earlyTree = new FactTreeMemento(0, 0);
            FactTreeMemento lateTree = new FactTreeMemento(0, 1);

            FactTreeMemento mergedTree = lateTree.Merge(earlyTree);
            Assert.AreEqual(0, mergedTree.DatabaseId);
            Assert.AreEqual(1, mergedTree.Timestamp);
        }

        [TestMethod]
        public void GivenEarlyAndLateTreeTheResultsDatabaseShouldBeTheMoreRecent()
        {
            FactTreeMemento earlyTree = new FactTreeMemento(0, 7);
            FactTreeMemento lateTree = new FactTreeMemento(1, 3);

            FactTreeMemento mergedTree = earlyTree.Merge(lateTree);
            Assert.AreEqual(1, mergedTree.DatabaseId);
            Assert.AreEqual(3, mergedTree.Timestamp);
        }

        [TestMethod]
        public void GivenLateAndEarlyTreeTheResultsDatabaseShouldBeTheMoreRecent()
        {
            FactTreeMemento earlyTree = new FactTreeMemento(0, 7);
            FactTreeMemento lateTree = new FactTreeMemento(1, 3);

            FactTreeMemento mergedTree = lateTree.Merge(earlyTree);
            Assert.AreEqual(1, mergedTree.DatabaseId);
            Assert.AreEqual(3, mergedTree.Timestamp);
        }

        [TestMethod]
        public void GivenTwoTreesWithTheSameFactsWhenMergedThenTheResultHasNoDuplicates()
        {
            FactTreeMemento firstTree = CreateTreeWithOneFact();
            FactTreeMemento secondTree = CreateTreeWithOneFact();

            FactTreeMemento mergedTree = firstTree.Merge(secondTree);

            Assert.AreEqual(1, mergedTree.Facts.Count());
        }

        //test capture timestamp the merge should be the most recent of the two
        private static void GetFullTree(out FactTreeMemento fullTree, out IdentifiedFactMemento identifiedMemento)
        {
            fullTree = new FactTreeMemento(1, 2);
            CorrespondenceFactType correspondenceFactType = new CorrespondenceFactType("", 3);
            FactMemento memento = new FactMemento(correspondenceFactType);
            FactID factId = new FactID();
            identifiedMemento = new IdentifiedFactMemento(factId, memento);
            fullTree.Add(identifiedMemento);
        }

        private static FactTreeMemento CreateTreeWithOneFact()
        {
            FactTreeMemento tree = new FactTreeMemento(0, 0);
            tree.Add(new IdentifiedFactMemento(new FactID { key = 43 }, new FactMemento(new CorrespondenceFactType("Reversi.Game", 1))));
            return tree;
        }
    }
}
