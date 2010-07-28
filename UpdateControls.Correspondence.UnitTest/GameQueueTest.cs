using Reversi.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Predassert;
using UpdateControls.Correspondence.Memory;

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
            GameQueue gameQueue = _community.AddFact(new GameQueue("mygamequeue"));
            GameQueue gameQueueAgain = _community.AddFact(new GameQueue("mygamequeue"));

            Pred.Assert(gameQueueAgain, Is.SameAs(gameQueue));
        }

        [TestMethod]
        public void GameQueueIsDifferentiatedByIdentifier()
        {
            GameQueue gameQueue = _community.AddFact(new GameQueue("mygamequeue"));
            GameQueue gameQueueOther = _community.AddFact(new GameQueue("myothergamequeue"));

            Pred.Assert(gameQueueOther, Is.NotSameAs(gameQueue));
        }
    }
}
