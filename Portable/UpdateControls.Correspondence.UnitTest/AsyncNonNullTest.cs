﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
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

        public async Task Initialize()
        {
            InitializeAsyncTest();
            var initializingCommunity = new Community(_memory)
                .Register<Model.CorrespondenceModel>();
            var alan = await initializingCommunity.AddFactAsync(new User("alan1"));
            var flynn = await initializingCommunity.AddFactAsync(new User("flynn1"));
            var game = await initializingCommunity.AddFactAsync(new Game());
            await initializingCommunity.AddFactAsync(new Player(alan, game, 0));
            await initializingCommunity.AddFactAsync(new Player(flynn, game, 1));

            _community = new Community(_memory)
                .Register<Model.CorrespondenceModel>();

            _alan = await _community.AddFactAsync(new User("alan1"));
            _flynn = await _community.AddFactAsync(new User("flynn1"));
        }

        [TestMethod]
        public async Task PropertiesNotYetLoadedAreNotLoadedObjects()
        {
            await Initialize();

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
        public async Task NullPropertiesAreNullObjects()
        {
            await Initialize();

            QuiescePeriodically();

            try
            {
                Color favoriteColor = await _alan.BetterFavoriteColor.EnsureAsync();

                Assert.IsNotNull(favoriteColor);
                Assert.IsTrue(favoriteColor.IsNull);
            }
            finally
            {
                Done();
            }
        }

        [TestMethod]
        public async Task FindFactReturnsUnloadedObject()
        {
            await Initialize();

            User user = _community.FindFact(new User("dillenger7"));

            Assert.IsFalse(user.IsLoaded);
        }

        [TestMethod]
        public async Task FindFactEventuallyBecomesANullObject()
        {
            await Initialize();

            User user = _community.FindFact(new User("dillenger7"));
            _memory.Quiesce();
            user = _community.FindFact(new User("dillenger7"));

            Assert.IsTrue(user.IsNull);
        }

        [TestMethod]
        public async Task CanEnsureFindFact()
        {
            await Initialize();

            QuiescePeriodically();

            try
            {
                User user = await _community.FindFact(new User("dillenger7")).EnsureAsync();
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