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
            GitFullTree(out fullTree, out identifiedMemento);
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
            GitFullTree(out fullTree, out identifiedMemento);
            FactTreeMemento mergedTree = emptyTree.Merge(fullTree);

            Assert.IsNotNull(mergedTree);
            IEnumerable<IdentifiedFactMemento> facts = mergedTree.Facts;
            Assert.IsNotNull(facts);
            Assert.AreEqual(1, facts.Count());
            IdentifiedFactMemento resultMemento = facts.Single();
            Assert.AreEqual(identifiedMemento, resultMemento);
        }

        [TestMethod]
        public void GivenFullTreeAndOneEmptyTreeIsTheResultTheSameAsTheFullTree()
        {
            FactTreeMemento emptyTree = new FactTreeMemento(0, 0);
            FactTreeMemento fullTree;
            IdentifiedFactMemento identifiedMemento;
            GitFullTree(out fullTree, out identifiedMemento);
            FactTreeMemento mergedTree = emptyTree.Merge(fullTree);

            Assert.IsNotNull(mergedTree);
            IEnumerable<IdentifiedFactMemento> facts = mergedTree.Facts;
            Assert.IsNotNull(facts);
            Assert.AreEqual(1, facts.Count());
            IdentifiedFactMemento resultMemento = facts.Single();
            Assert.AreEqual(identifiedMemento, resultMemento);
        }

        [TestMethod]
        public void GivenTwoTreesTheResultsTimeStampShouldBeTheMoreRecent()
        {
            FactTreeMemento emptyTree = new FactTreeMemento(0, 0);
            FactTreeMemento fullTree;
            IdentifiedFactMemento identifiedMemento;
            GitFullTree(out fullTree, out identifiedMemento);
            FactTreeMemento mergedTree = emptyTree.Merge(fullTree);

            Assert.IsNotNull(mergedTree);
            IEnumerable<IdentifiedFactMemento> facts = mergedTree.Facts;
            Assert.IsNotNull(facts);
            Assert.AreEqual(1, facts.Count());
            IdentifiedFactMemento resultMemento = facts.Single();
            Assert.AreEqual(identifiedMemento, resultMemento);
            Assert.Fail;
        }

        [TestMethod]
        public void GivenTwoTreesTheResultsDataBaseIdShouldBeTheGreater()
        {
            FactTreeMemento emptyTree = new FactTreeMemento(0, 0);
            FactTreeMemento fullTree;
            IdentifiedFactMemento identifiedMemento;
            GitFullTree(out fullTree, out identifiedMemento);
            FactTreeMemento mergedTree = emptyTree.Merge(fullTree);

            Assert.IsNotNull(mergedTree);
            IEnumerable<IdentifiedFactMemento> facts = mergedTree.Facts;
            Assert.IsNotNull(facts);
            Assert.AreEqual(1, facts.Count());
            IdentifiedFactMemento resultMemento = facts.Single();
            Assert.AreEqual(identifiedMemento, resultMemento);
            Assert.Fail;
        }

        //test capture timestamp the merge should be the most recent of the two
        private static void GitFullTree(out FactTreeMemento fullTree, out IdentifiedFactMemento identifiedMemento)
        {
            fullTree = new FactTreeMemento(1, 2);
            CorrespondenceFactType correspondenceFactType = new CorrespondenceFactType("", 3);
            FactMemento memento = new FactMemento(correspondenceFactType);
            FactID factId = new FactID();
            identifiedMemento = new IdentifiedFactMemento(factId, memento);
            fullTree.Add(identifiedMemento);
        }
    }
}
