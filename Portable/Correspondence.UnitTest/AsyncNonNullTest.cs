﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;
using Correspondence.Memory;
using Correspondence.UnitTest.Model;
using System;

namespace Correspondence.UnitTest
{
    [TestClass]
    public class AsyncNonNullTest : AsyncTest
    {
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

            Community = new Community(_memory)
                .Register<Model.CorrespondenceModel>();

            _alan = await Community.AddFactAsync(new User("alan1"));
            _flynn = await Community.AddFactAsync(new User("flynn1"));
        }

        [TestMethod]
        public async Task PropertiesNotYetLoadedAreNotLoadedObjects()
        {
            await Initialize();

            Color favoriteColor = _alan.BetterFavoriteColor;

            Assert.IsNotNull(favoriteColor);
            Assert.IsFalse(favoriteColor.IsLoaded);
        }

        [TestMethod]
        public async Task NullPropertiesAreNullObjects()
        {
            await Initialize();

            Color favoriteColor = await _alan.BetterFavoriteColor.EnsureAsync();

            Assert.IsNotNull(favoriteColor);
            Assert.IsTrue(favoriteColor.IsNull);
        }

        [TestMethod]
        public async Task FindFactReturnsUnloadedObject()
        {
            await Initialize();

            User user = Community.FindFact(new User("dillenger7"));

            Assert.IsFalse(user.IsLoaded);
        }

        [TestMethod]
        public async Task FindFactEventuallyBecomesANullObject()
        {
            await Initialize();

            User user = Community.FindFact(new User("dillenger7"));
            await QuiesceAllAsync();
            user = Community.FindFact(new User("dillenger7"));

            Assert.IsTrue(user.IsNull);
        }

        [TestMethod]
        public async Task CanEnsureFindFact()
        {
            await Initialize();

            User user = await Community.FindFact(new User("dillenger7")).EnsureAsync();
            Assert.IsTrue(user.IsLoaded);
            Assert.IsTrue(user.IsNull);
        }
    }
}
