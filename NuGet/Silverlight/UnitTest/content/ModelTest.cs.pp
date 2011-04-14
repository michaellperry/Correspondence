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
		//private MyFact _fact;
		//private MyFact _otherFact;

        [TestInitialize]
        public void Initialize()
        {
            var sharedCommunication = new MemoryCommunicationStrategy();
            _community = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(sharedCommunication)
                //.Register<Model.CorrespondenceModel>()
                //.Subscribe(() => _fact)
				;
            _otherCommunity = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(sharedCommunication)
                //.Register<Model.CorrespondenceModel>()
                //.Subscribe(() => _otherFact)
				;

			//_fact = _community.AddFact(new MyFact());
			//_otherFact = _otherCommunity.AddFact(new MyFact());
		}
	}
}
