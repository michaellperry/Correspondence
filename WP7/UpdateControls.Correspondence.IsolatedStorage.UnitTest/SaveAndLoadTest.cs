using System.Linq;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.IsolatedStorage.UnitTest
{
    [TestClass]
    public class SaveAndLoadTest : SilverlightTest
    {
        private const string STR_TypeName = "IM.Model.User";
        private const string STR_SuccessorTypeName = "IM.Model.Message";
        private IStorageStrategy _strategy;

        [TestInitialize]
        public void Initialize()
        {
            IsolatedStorageStorageStrategy.DeleteAll();
            _strategy = IsolatedStorageStorageStrategy.Load();
        }

        [TestMethod]
        [ExpectedException(typeof(CorrespondenceException))]
        public void LoadANonExistantFact()
        {
            FactMemento fact = _strategy.Load(new FactID() { key = 287658 });
        }

        [TestMethod]
        public void SaveASimpleFact()
        {
            FactID id;
            bool saved = _strategy.Save(CreateSimpleFactMemento(), out id);

            Assert.IsTrue(saved);
        }

        [TestMethod]
        public void SaveASimpleFactTwice()
        {
            FactID firstId;
            _strategy.Save(CreateSimpleFactMemento(), out firstId);
            FactID secondId;

            Reload();

            bool saved = _strategy.Save(CreateSimpleFactMemento(), out secondId);

            Assert.IsFalse(saved);
            Assert.AreEqual(firstId, secondId);
        }

        [TestMethod]
        public void SaveAndLoadASimpleFact()
        {
            FactID id;
            bool saved = _strategy.Save(CreateSimpleFactMemento(), out id);

            Reload();

            FactMemento factMemento = _strategy.Load(id);

            Assert.AreEqual(STR_TypeName, factMemento.FactType.TypeName);
            Assert.AreEqual(1, factMemento.FactType.Version);
        }

        [TestMethod]
        public void ShouldLoadPredecessor()
        {
            FactID id;
            FactID predecessorId;

            _strategy.Save(CreateSimpleFactMemento(), out predecessorId);
            _strategy.Save(CreateSuccessorFactMemento(predecessorId), out id);

            Reload();

            FactMemento successor = _strategy.Load(id);

            Assert.AreEqual(successor.Predecessors.Single().ID, predecessorId);
        }

        [TestMethod]
        public void ShouldFindSimpleFact()
        {
            FactID id;
            FactMemento memento = CreateSimpleFactMemento();
            _strategy.Save(memento, out id);

            Reload();

            FactID copyId;
            FactMemento copy = CreateSimpleFactMemento();
            bool saved = _strategy.Save(copy, out copyId);

            Assert.IsFalse(saved);
            Assert.AreEqual(id, copyId);
        }

        [TestMethod]
        public void ShouldFindSuccessorFact()
        {
            FactID id;
            FactMemento memento = CreateSimpleFactMemento();
            _strategy.Save(memento, out id);

            FactID successorId;
            FactMemento successor = CreateSuccessorFactMemento(id);
            _strategy.Save(successor, out successorId);

            Reload();

            FactID successorCopyId;
            FactMemento successorCopy = CreateSuccessorFactMemento(id);
            bool saved = _strategy.Save(successorCopy, out successorCopyId);

            Assert.IsFalse(saved);
            Assert.AreEqual(successorId, successorCopyId);
        }

        private static FactMemento CreateSimpleFactMemento()
        {
            CorrespondenceFactType factType = new CorrespondenceFactType(STR_TypeName, 1);
            return new FactMemento(factType)
            {
                Data = new byte[] { 0x12, 0xc3, 0x48, 0xf4 }
            };
        }

        private static FactMemento CreateSuccessorFactMemento(FactID predecessorId)
        {
            CorrespondenceFactType factType = new CorrespondenceFactType(STR_SuccessorTypeName, 1);
            FactMemento fact = new FactMemento(factType)
            {
                Data = new byte[] { 0x12, 0xc3, 0x48, 0xf4 }
            };
            fact.AddPredecessor(
                new RoleMemento(
                    factType, 
                    "user", 
                    new CorrespondenceFactType(STR_TypeName, 1), 
                    true),
                predecessorId);
            return fact;
        }

        private void Reload()
        {
            _strategy = IsolatedStorageStorageStrategy.Load();
        }
    }
}
