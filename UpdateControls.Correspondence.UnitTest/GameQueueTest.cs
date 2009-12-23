using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Memory;
using GameModel;
using Predassert;

namespace UpdateControls.Correspondence.UnitTest
{
    [TestClass]
    public class GameQueueTest
    {
        public TestContext TestContext { get; set; }

        private Community _community;

        [TestInitialize]
        public void Initialize()
        {
            _community = new Community(new MemoryStorageStrategy())
                .RegisterAssembly(typeof(GameQueue));
        }

        [TestMethod]
        public void GameQueueIsIdentifiedByIdentifier()
        {
            GameQueue gameQueue = _community.AddFact(new GameQueue("http://mydomain.com/mygamequeue"));
            GameQueue gameQueueAgain = _community.AddFact(new GameQueue("http://mydomain.com/mygamequeue"));

            Pred.Assert(gameQueueAgain, Is.SameAs(gameQueue));
        }

        [TestMethod]
        public void GameQueueIsDifferentiatedByIdentifier()
        {
            GameQueue gameQueue = _community.AddFact(new GameQueue("http://mydomain.com/mygamequeue"));
            GameQueue gameQueueOther = _community.AddFact(new GameQueue("http://mydomain.com/myothergamequeue"));

            Pred.Assert(gameQueueOther, Is.NotSameAs(gameQueue));
        }
    }
}
