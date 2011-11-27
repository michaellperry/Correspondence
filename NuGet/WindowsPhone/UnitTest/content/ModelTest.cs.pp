using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Silverlight.Testing;
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
				;
            _communityAlan = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(sharedCommunication)
                .Register<CorrespondenceModel>()
                .Subscribe(() => _identityAlan)
				;

            _identityFlynn = _communityFlynn.AddFact(new Identity("flynn"));
            _identityAlan = _communityAlan.AddFact(new Identity("alan"));
		}

        [TestMethod]
        public void InitiallyNoMessages()
        {
            Assert.IsFalse(_identityFlynn.Messages.Any());
        }

        [TestMethod]
        public void AlanSendsFlynnAMessage()
        {
            _identityAlan.SendMessage("flynn", "Reindeer flotilla");

            Synchronize();

            Assert.AreEqual("Reindeer flotilla", _identityFlynn.Messages.Single().Text);
        }

        private void Synchronize()
        {
            while (_communityFlynn.Synchronize() || _communityAlan.Synchronize()) ;
        }
	}
}
