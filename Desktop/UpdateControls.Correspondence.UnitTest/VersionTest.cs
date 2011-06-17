using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.UnitTest.Model;
using UpdateControls.Correspondence.Memory;

namespace UpdateControls.Correspondence.UnitTest
{
    [TestClass]
    public class VersionTest
    {
        private User _userAlan;
        private User _userFlynn;
        private Community _communityAlan;
        private Community _communityFlynn;
        private Player _playerAlan;
        private Player _playerFlynn;
        private MemoryStorageStrategy _storageFlynn;

        [TestInitialize]
        public void Initialize()
        {
            // Alan is on version 2. He can understand more facts that Flynn.
            MemoryCommunicationStrategy sharedCommunication = new MemoryCommunicationStrategy();
            _communityAlan = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(sharedCommunication)
                .Register<Model.CorrespondenceModel>()
                .Register<Model.Version2Model>()
                .Subscribe(() => _userAlan)
                .Subscribe(() => _userAlan.ActivePlayers.Select(player => player.Game));
            _storageFlynn = new MemoryStorageStrategy();
            _communityFlynn = new Community(_storageFlynn)
                .AddCommunicationStrategy(sharedCommunication)
                .Register<Model.CorrespondenceModel>()
                .Subscribe(() => _userFlynn)
                .Subscribe(() => _userFlynn.ActivePlayers.Select(player => player.Game));

            _userAlan = _communityAlan.AddFact(new User("alan1"));
            _userFlynn = _communityFlynn.AddFact(new User("flynn1"));
            _playerAlan = _userAlan.Challenge(_communityAlan.AddFact(new User("flynn1")));
            Synchronize();
            _playerFlynn = _userFlynn.ActivePlayers.Single();
        }

        [TestMethod]
        public void AlanCanSendAMessage()
        {
            int factCount = _storageFlynn.LoadAllFacts().Count();

            _communityAlan.AddFact(new Message(_playerAlan, "This system's got more bugs than a bait store."));
            Synchronize();

            // Flynn can't understand the fact, but it gets stored until he upgrades.
            int newFactCount = _storageFlynn.LoadAllFacts().Count();
            Assert.AreEqual(factCount + 1, newFactCount);
        }

        private void Synchronize()
        {
            while (_communityAlan.Synchronize() || _communityFlynn.Synchronize()) ;
        }
    }
}
