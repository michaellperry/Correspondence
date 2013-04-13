using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.UnitTest.Model;

namespace UpdateControls.Correspondence.UnitTest
{
    [TestClass]
    public class AsyncNonNullTest : AsyncTest
    {
        private Community _community;
        private User _alan;
        private User _flynn;

        [TestInitialize]
        public void Initialize()
        {
            InitializeAsyncTest();
            var initializingCommunity = new Community(_memory)
                .Register<Model.CorrespondenceModel>();
            var alan = initializingCommunity.AddFact(new User("alan1"));
            var flynn = initializingCommunity.AddFact(new User("flynn1"));
            var game = initializingCommunity.AddFact(new Game());
            initializingCommunity.AddFact(new Player(alan, game, 0));
            initializingCommunity.AddFact(new Player(flynn, game, 1));

            _community = new Community(_memory)
                .Register<Model.CorrespondenceModel>();

            _alan = _community.AddFact(new User("alan1"));
            _flynn = _community.AddFact(new User("flynn1"));
        }

        [TestMethod]
        public void PropertiesNotYetLoadedAreNotLoadedObjects()
        {
            QuiescePeriodically();

            try
            {
                Color favoriteColor = _alan.BetterFavoriteColor;

                Assert.IsNotNull(favoriteColor);
                Assert.IsFalse(favoriteColor.IsLoaded);
            }
            finally
            {
                Done();
            }
        }

        [TestMethod]
        public void NullPropertiesAreNullObjects()
        {
            QuiescePeriodically();

            try
            {
                Color favoriteColor = _alan.BetterFavoriteColor.Ensure();

                Assert.IsNotNull(favoriteColor);
                Assert.IsTrue(favoriteColor.IsNull);
            }
            finally
            {
                Done();
            }
        }

        [TestMethod]
        public void FindFactReturnsUnloadedObject()
        {
            User user = _community.FindFact(new User("dillenger7"));

            Assert.IsFalse(user.IsLoaded);
        }

        [TestMethod]
        public void FindFactEventuallyBecomesANullObject()
        {
            User user = _community.FindFact(new User("dillenger7"));
            _memory.Quiesce();
            user = _community.FindFact(new User("dillenger7"));

            Assert.IsTrue(user.IsNull);
        }

        [TestMethod]
        public void CanEnsureFindFact()
        {
            QuiescePeriodically();

            try
            {
                User user = _community.FindFact(new User("dillenger7")).Ensure();
                Assert.IsTrue(user.IsLoaded);
                Assert.IsTrue(user.IsNull);
            }
            finally
            {
                Done();
            }
        }
    }
}
