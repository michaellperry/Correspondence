using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Mementos;
using System.Linq;

namespace UpdateControls.Correspondence.IsolatedStorage.UnitTest
{
    [TestClass]
    public class SaveAndLoadTest : SilverlightTest
    {
        private const string STR_TypeName = "IM.Model.User";
        private const string STR_SuccessorTypeName = "IM.Model.Message";
        private IsolatedStorageStorageStrategy _strategy;

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
            bool saved = _strategy.Save(CreateSimpleFactMemento(), out secondId);

            Assert.IsFalse(saved);
            Assert.AreEqual(firstId, secondId);
        }

        [TestMethod]
        public void SaveAndLoadASimpleFact()
        {
            FactID id;
            bool saved = _strategy.Save(CreateSimpleFactMemento(), out id);
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

            FactMemento successor = _strategy.Load(id);

            Assert.AreEqual(successor.Predecessors.Single().ID, predecessorId);
        }

        private static FactMemento CreateSimpleFactMemento()
        {
            CorrespondenceFactType factType = new CorrespondenceFactType(STR_TypeName, 1);
            return new FactMemento(factType)
            {
                Data = new byte[0]
            };
        }

        private static FactMemento CreateSuccessorFactMemento(FactID predecessorId)
        {
            CorrespondenceFactType factType = new CorrespondenceFactType(STR_SuccessorTypeName, 1);
            FactMemento fact = new FactMemento(factType)
            {
                Data = new byte[0]
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
    }
}
