using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Memory;

namespace $rootnamespace$
{
    [TestClass]
    public class ModelTest
    {
        private Community _communityFlynn;
        private Community _communityAlan;
        private Individual _individualFlynn;
        private Individual _individualAlan;

        [TestInitialize]
        public void Initialize()
        {
            var sharedCommunication = new MemoryCommunicationStrategy();
            _communityFlynn = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(sharedCommunication)
                .Register<CorrespondenceModel>()
                .Subscribe(() => _individualFlynn)
                .Subscribe(() => _individualFlynn.MessageBoards)
				;
            _communityAlan = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(sharedCommunication)
                .Register<CorrespondenceModel>()
                .Subscribe(() => _individualAlan)
                .Subscribe(() => _individualAlan.MessageBoards)
                ;

            _individualFlynn = _communityFlynn.AddFact(new Individual("flynn"));
            _individualAlan = _communityAlan.AddFact(new Individual("alan"));
            _individualFlynn.JoinMessageBoard("The Grid");
            _individualAlan.JoinMessageBoard("The Grid");
        }

        [TestMethod]
        public void InitiallyNoMessages()
        {
            Assert.IsFalse(_individualAlan.MessageBoards.Single().Messages.Any());
        }

        [TestMethod]
        public void FlynnSendsAMessage()
        {
            _individualFlynn.MessageBoards.Single().SendMessage("Reindeer flotilla");

            Synchronize();

            Message message = _individualAlan.MessageBoards.Single().Messages.Single();
            Assert.AreEqual("Reindeer flotilla", message.Text);
        }

        private void Synchronize()
        {
            while (_communityFlynn.Synchronize() || _communityAlan.Synchronize()) ;
        }
	}
}
