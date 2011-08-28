using System.Linq;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;
using System.Collections.Generic;

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
            bool saved = _strategy.Save(CreateSimpleFactMemento(), 0, out id);

            Assert.IsTrue(saved);
        }

        [TestMethod]
        public void SaveASimpleFactTwice()
        {
            FactID firstId;
            _strategy.Save(CreateSimpleFactMemento(), 0, out firstId);
            FactID secondId;

            Reload();

            bool saved = _strategy.Save(CreateSimpleFactMemento(), 0, out secondId);

            Assert.IsFalse(saved);
            Assert.AreEqual(firstId, secondId);
        }

        [TestMethod]
        public void SaveAndLoadASimpleFact()
        {
            FactID id;
            bool saved = _strategy.Save(CreateSimpleFactMemento(), 0, out id);

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

            _strategy.Save(CreateSimpleFactMemento(), 0, out predecessorId);
            _strategy.Save(CreateSuccessorFactMemento(predecessorId), 0, out id);

            Reload();

            FactMemento successor = _strategy.Load(id);

            Assert.AreEqual(successor.Predecessors.Single().ID, predecessorId);
        }

        [TestMethod]
        public void ShouldFindSimpleFact()
        {
            FactID id;
            FactMemento memento = CreateSimpleFactMemento();
            _strategy.Save(memento, 0, out id);

            Reload();

            FactID copyId;
            FactMemento copy = CreateSimpleFactMemento();
            bool saved = _strategy.Save(copy, 0, out copyId);

            Assert.IsFalse(saved);
            Assert.AreEqual(id, copyId);
        }

        [TestMethod]
        public void ShouldFindSuccessorFact()
        {
            FactID id;
            FactMemento memento = CreateSimpleFactMemento();
            _strategy.Save(memento, 0, out id);

            FactID successorId;
            FactMemento successor = CreateSuccessorFactMemento(id);
            _strategy.Save(successor, 0, out successorId);

            Reload();

            FactID successorCopyId;
            FactMemento successorCopy = CreateSuccessorFactMemento(id);
            bool saved = _strategy.Save(successorCopy, 0, out successorCopyId);

            Assert.IsFalse(saved);
            Assert.AreEqual(successorId, successorCopyId);
        }

        [TestMethod]
        public void WhenProtocolNameIsDifferent_PeerIdShouldBeDifferent()
        {
            int firstPeerId = _strategy.SavePeer("firstProtocol", "peerName");
            int secondPeerId = _strategy.SavePeer("secondProtocol", "peerName");

            Assert.AreNotEqual(firstPeerId, secondPeerId);
        }

        [TestMethod]
        public void WhenPeerNameIsDifferent_PeerIdShouldBeDifferent()
        {
            int firstPeerId = _strategy.SavePeer("protocol", "firstPeerName");
            int secondPeerId = _strategy.SavePeer("protocol", "secondPeerName");

            Assert.AreNotEqual(firstPeerId, secondPeerId);
        }

        [TestMethod]
        public void WhenProtocolAndPeerNameAreSame_PeerIdShouldBeSame()
        {
            int firstPeerId = _strategy.SavePeer("protocol", "peerName");
            int secondPeerId = _strategy.SavePeer("protocol", "peerName");

            Assert.AreEqual(firstPeerId, secondPeerId);
        }

        [TestMethod]
        public void ShouldSkipMessagesFromServer()
        {
            int peerId = 1;

            FactMemento predecessor = CreateSimpleFactMemento();
            FactID predecessorId;
            _strategy.Save(predecessor, peerId, out predecessorId);

            FactMemento localSuccessor = CreateSuccessorFactMemento(predecessorId, new byte[] { 0x23, 0x34 });
            FactID localSuccessorId;
            _strategy.Save(localSuccessor, 0, out localSuccessorId);

            FactMemento remoteSuccessor = CreateSuccessorFactMemento(predecessorId, new byte[] { 0x34, 0x45 });
            FactID remoteSuccessorId;
            _strategy.Save(remoteSuccessor, peerId, out remoteSuccessorId);

            IEnumerable<MessageMemento> messages = _strategy.LoadRecentMessagesForServer(peerId, new TimestampID(0L, 0L));

            Assert.AreEqual(peerId, messages.Count());
            Assert.AreEqual(localSuccessorId, messages.Single().FactId);
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
            return CreateSuccessorFactMemento(predecessorId, new byte[] { 0x12, 0xc3, 0x48, 0xf4 });
        }

        private static FactMemento CreateSuccessorFactMemento(FactID predecessorId, byte[] data)
        {
            CorrespondenceFactType factType = new CorrespondenceFactType(STR_SuccessorTypeName, 1);
            FactMemento fact = new FactMemento(factType)
            {
                Data = data
            };
            fact.AddPredecessor(
                new RoleMemento(
                    factType,
                    "user",
                    new CorrespondenceFactType(STR_TypeName, 1),
                    true),
                predecessorId,
                true);
            return fact;
        }

        private void Reload()
        {
            _strategy = IsolatedStorageStorageStrategy.Load();
        }
    }
}
