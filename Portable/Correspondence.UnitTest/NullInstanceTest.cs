﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Correspondence.UnitTest.Model;
using System.Linq;
using System.Threading.Tasks;
using Correspondence.Memory;
using Correspondence.UnitTest.Fakes;

namespace Correspondence.UnitTest
{
    [TestClass]
    public class NullInstanceTest
    {
        [TestMethod]
        public void QueryResultsAreEmpty()
        {
            var user = User.GetNullInstance();

            bool any = user.ActivePlayers.Any();

            Assert.IsFalse(any);
        }

        [TestMethod]
        public async Task QueryResultsCanBeEnsured()
        {
            var user = User.GetNullInstance();

            bool any = user.ActivePlayers.Any();

            Assert.IsFalse(any);
        }

        [TestMethod]
        public void PropertyValuesAreDefault()
        {
            var user = User.GetNullInstance();

            string favoriteColor = user.FavoriteColor;

            Assert.IsNull(favoriteColor);
        }

        [TestMethod]
        public void PropertyReferencesAreNullObjects()
        {
            var user = User.GetNullInstance();

            Color favoriteColor = user.BetterFavoriteColor;

            Assert.IsNotNull(favoriteColor);
            Assert.IsTrue(favoriteColor.IsNull);
        }

        [TestMethod]
        public async Task PropertyReferencesCanBeEnsured()
        {
            var user = User.GetNullInstance();

            Color favoriteColor = await user.BetterFavoriteColor.EnsureAsync();

            Assert.IsNotNull(favoriteColor);
            Assert.IsTrue(favoriteColor.IsNull);
        }

        [TestMethod]
        public async Task CanSubscribeToANullFact()
        {
            var community = new Community(new MemoryStorageStrategy());
            community.AddAsynchronousCommunicationStrategy(new FakeCommunicationStrategy());
            community.Subscribe(() => User.GetNullInstance());

            community.BeginReceiving();
            await Task.Delay(100);
            Assert.IsFalse(community.Synchronizing);
        }
    }
}
