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
    public class ModelTest : SilverlightTest
    {
        private Community _community;
        private Community _otherCommunity;
        //private Identity _identity;
        //private Identity _otherIdentity;

        [TestInitialize]
        public void Initialize()
        {
            var sharedCommunication = new MemoryCommunicationStrategy();
            _community = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(sharedCommunication)
                //.Register<Model.CorrespondenceModel>()
                //.Subscribe(() => _identity)
				;
            _otherCommunity = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(sharedCommunication)
                //.Register<Model.CorrespondenceModel>()
                //.Subscribe(() => _otherIdentity)
				;

            //_identity = _community.AddFact(new Identity("mike"));
            //_otherIdentity = _otherCommunity.AddFact(new Identity("mike"));
		}

        //[TestMethod]
        //public void ThroughtIsInitiallyNull()
        //{
        //    Assert.IsNull(_otherIdentity.Me.Value);
        //}

        //[TestMethod]
        //public void OtherDeviceReceivesThougths()
        //{
        //    _identity.Me = _community.AddFact(new Thought());
        //
        //    Synchronize();
        //
        //    Assert.IsNotNull(_otherIdentity.Me.Value);
        //}

        private void Synchronize()
        {
            while (_community.Synchronize() || _otherCommunity.Synchronize()) ;
        }
	}
}
