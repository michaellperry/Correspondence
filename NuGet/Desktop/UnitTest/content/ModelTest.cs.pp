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
        private Identity _identityFlynn;
        private Identity _identityAlan;

        [TestInitialize]
        public void Initialize()
        {
            var sharedCommunication = new MemoryCommunicationStrategy();
            _communityFlynn = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(sharedCommunication)
                .Register<CorrespondenceModel>()
                .Subscribe(() => _identityFlynn)
                .Subscribe(() => _identityFlynn.MessageBoards)
				;
            _communityAlan = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(sharedCommunication)
                .Register<CorrespondenceModel>()
                .Subscribe(() => _identityAlan)
                .Subscribe(() => _identityAlan.MessageBoards)
                ;

            _identityFlynn = _communityFlynn.AddFact(new Identity("flynn"));
            _identityAlan = _communityAlan.AddFact(new Identity("alan"));
            _identityFlynn.JoinMessageBoard("The Grid");
            _identityAlan.JoinMessageBoard("The Grid");
        }

        [TestMethod]
        public void InitiallyNoMessages()
        {
            Assert.IsFalse(_identityAlan.MessageBoards.Single().Messages.Any());
        }

        [TestMethod]
        public void FlynnSendsAMessage()
        {
            _identityFlynn.MessageBoards.Single().SendMessage("Reindeer flotilla");

            Synchronize();

            Message message = _identityAlan.MessageBoards.Single().Messages.Single();
            Assert.AreEqual("Reindeer flotilla", message.Text);
        }

        private void Synchronize()
        {
            while (_communityFlynn.Synchronize() || _communityAlan.Synchronize()) ;
        }
	}
}
