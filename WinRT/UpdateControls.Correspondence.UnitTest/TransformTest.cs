using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.UnitTest.Model;

namespace UpdateControls.Correspondence.UnitTest
{
    [TestClass]
    public class TransformTest
    {
        private Community _community;
        private MockTransform _transform;
        private int _triggerCount;

        [TestInitialize]
        public void Initialize()
        {
            _community = new Community(new MemoryStorageStrategy())
                .Register<CorrespondenceModel>();
            _transform = new MockTransform();
            _triggerCount = 0;
            _community.FactReceived += delegate { _triggerCount++; };
        }

        [TestMethod]
        public void TransformServiceEnds()
        {
            bool canContinue = _community.InvokeTransform(_transform);
            Assert.IsFalse(canContinue);
        }

        [TestMethod]
        public void FactAddedBeforeServiceStarts()
        {
            Machine newMachine = _community.AddFact(new Machine());
            bool canContinue = _community.InvokeTransform(_transform);

            Assert.IsTrue(canContinue);
            Assert.AreSame(newMachine, _transform.Facts.Single());

            canContinue = _community.InvokeTransform(_transform);

            Assert.IsFalse(canContinue);
        }

        [TestMethod]
        public void FactAddedTriggersTheService()
        {
            bool canContinue = _community.InvokeTransform(_transform);
            Assert.IsFalse(canContinue);
            Assert.AreEqual(0, _triggerCount);

            Machine newMachine = _community.AddFact(new Machine());
            Assert.AreEqual(1, _triggerCount);
        }

        [TestMethod]
        public void FactReceivedTriggersTheService()
        {
            Community thatCommunity;
            User thisUser = null;
            User thatUser = null;

            MemoryCommunicationStrategy sharedCommunication = new MemoryCommunicationStrategy();
            _community
                .AddCommunicationStrategy(sharedCommunication)
                .Subscribe(() => thisUser);
            thisUser = _community.AddFact(new User("me"));
            thatCommunity = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(sharedCommunication)
                .Register<CorrespondenceModel>()
                .Subscribe(() => thatUser);
            thatUser = thatCommunity.AddFact(new User("me"));
            Assert.AreEqual(1, _triggerCount);

            Game game = thatCommunity.AddFact(new Game());
            thatCommunity.AddFact(new Player(thatUser, game, 0));
            Assert.AreEqual(1, _triggerCount);

            while (_community.Synchronize() || thatCommunity.Synchronize()) ;

            Assert.AreEqual(3, _triggerCount);
        }
    }
}
