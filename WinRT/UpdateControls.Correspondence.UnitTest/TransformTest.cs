using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.UnitTest.Model;
using System.Threading.Tasks;

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
        public async Task FactAddedBeforeServiceStarts()
        {
            Machine newMachine = await _community.AddFactAsync(new Machine());
            bool canContinue = _community.InvokeTransform(_transform);

            Assert.IsTrue(canContinue);
            Assert.AreSame(newMachine, _transform.Facts.Single());

            canContinue = _community.InvokeTransform(_transform);

            Assert.IsFalse(canContinue);
        }

        [TestMethod]
        public async Task FactAddedTriggersTheService()
        {
            bool canContinue = _community.InvokeTransform(_transform);
            Assert.IsFalse(canContinue);
            Assert.AreEqual(0, _triggerCount);

            Machine newMachine = await _community.AddFactAsync(new Machine());
            Assert.AreEqual(1, _triggerCount);
        }

        [TestMethod]
        public async Task FactReceivedTriggersTheService()
        {
            Community thatCommunity;
            User thisUser = null;
            User thatUser = null;

            MemoryCommunicationStrategy sharedCommunication = new MemoryCommunicationStrategy();
            _community
                .AddCommunicationStrategy(sharedCommunication)
                .Subscribe(() => thisUser);
            thisUser = await _community.AddFactAsync(new User("me"));
            thatCommunity = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(sharedCommunication)
                .Register<CorrespondenceModel>()
                .Subscribe(() => thatUser);
            thatUser = await thatCommunity.AddFactAsync(new User("me"));
            Assert.AreEqual(1, _triggerCount);

            Game game = await thatCommunity.AddFactAsync(new Game());
            await thatCommunity.AddFactAsync(new Player(thatUser, game, 0));
            Assert.AreEqual(1, _triggerCount);

            while (
                await _community.SynchronizeAsync() ||
                await thatCommunity.SynchronizeAsync()) ;

            Assert.AreEqual(3, _triggerCount);
        }
    }
}
