using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.UnitTest.Model;
using System.Threading.Tasks;

namespace UpdateControls.Correspondence.UnitTest
{
    [TestClass]
    public class PropertyTest
    {
        private Community _community;
        private Community _otherCommunity;
        private User _alan;
        private User _otherAlan;
        private MemoryStorageStrategy _storageAlan;

        public async Task Initialize()
        {
            var sharedCommunication = new MemoryCommunicationStrategy();
            _storageAlan = new MemoryStorageStrategy();
            _community = new Community(_storageAlan)
                .AddCommunicationStrategy(sharedCommunication)
                .Register<Model.CorrespondenceModel>()
                .Subscribe(() => _alan);
            _community.SetDesignMode();
            _otherCommunity = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(sharedCommunication)
                .Register<Model.CorrespondenceModel>()
                .Subscribe(() => _otherAlan);
            _otherCommunity.SetDesignMode();

            _alan = await _community.AddFactAsync(new User("alan1"));
            _otherAlan = await _otherCommunity.AddFactAsync(new User("alan1"));
        }

        [TestMethod]
        public async Task CanGetDefaultMutableProperty()
        {
            await Initialize();

            string favoriteColor = _alan.FavoriteColor;

            Assert.AreEqual(default(string), favoriteColor);
        }

        [TestMethod]
        public async Task CanSetMutableProperty()
        {
            await Initialize();

            _alan.FavoriteColor = "Blue";
            string favoriteColor = _alan.FavoriteColor;

            Assert.AreEqual("Blue", favoriteColor);
        }

        [TestMethod]
        public async Task CanChangeMutableProperty()
        {
            await Initialize();

            _alan.FavoriteColor = "Blue";
            _alan.FavoriteColor = "Red";

            string favoriteColor = _alan.FavoriteColor;

            Assert.AreEqual("Red", favoriteColor);
        }

        [TestMethod]
        public async Task PropertySetFromOneCommunityHasNoConflict()
        {
            await Initialize();

            _alan.FavoriteColor = "Blue";
            _alan.FavoriteColor = "Red";

            Assert.IsFalse(_alan.FavoriteColor.InConflict);
        }

        [TestMethod]
        public async Task PropertySetFromTwoCommunitiesHasConflict()
        {
            await Initialize();

            _alan.FavoriteColor = "Blue";
            _otherAlan.FavoriteColor = "Red";
            await SynchronizeAsync();

            Assert.IsTrue(_alan.FavoriteColor.InConflict);
        }

        [TestMethod]
        public async Task CanSeeCandidatesOfConflict()
        {
            await Initialize();

            _alan.FavoriteColor = "Blue";
            _otherAlan.FavoriteColor = "Red";
            await SynchronizeAsync();

            var fc = await _alan.FavoriteColor.EnsureAsync();
            IEnumerable<string> candidates = fc.Candidates;
            Assert.IsTrue(candidates.Contains("Blue"));
            Assert.IsTrue(candidates.Contains("Red"));
        }

        [TestMethod]
        public async Task RedundantSetOfSimplePropertyDoesNotCreateAFact()
        {
            await Initialize();

            _alan.FavoriteColor = "Blue";
            int before = _storageAlan.LoadAllFacts().Count();
            _alan.FavoriteColor = "Blue";
            int after = _storageAlan.LoadAllFacts().Count();

            Assert.AreEqual(before, after);
        }

        [TestMethod]
        public async Task CanGetDefaultMutableFactProperty()
        {
            await Initialize();

            Color color = _alan.BetterFavoriteColor;

            Assert.IsNotNull(color);
            Assert.IsTrue(color.IsNull);
        }

        [TestMethod]
        public async Task CanSetMutableFactProperty()
        {
            await Initialize();

            _alan.BetterFavoriteColor = await _community.AddFactAsync(new Color("Blue"));
            Color color = _alan.BetterFavoriteColor;

            Assert.AreEqual("Blue", color.Name);
        }

        [TestMethod]
        public async Task CanChangeMutableFactProperty()
        {
            await Initialize();

            _alan.BetterFavoriteColor = await _community.AddFactAsync(new Color("Blue"));
            _alan.BetterFavoriteColor = await _community.AddFactAsync(new Color("Red"));

            Color color = await _alan.BetterFavoriteColor.EnsureAsync();

            Assert.AreEqual("Red", color.Name);
        }

        [TestMethod]
        public async Task RedundantSetOfFactPropertyDoesNotCreateAFact()
        {
            await Initialize();

            _alan.BetterFavoriteColor = await _community.AddFactAsync(new Color("Blue"));
            int before = _storageAlan.LoadAllFacts().Count();
            _alan.BetterFavoriteColor = await _community.AddFactAsync(new Color("Blue"));
            int after = _storageAlan.LoadAllFacts().Count();

            Assert.AreEqual(before, after);
        }

        [TestMethod]
        public async Task FactPropertySetFromOneCommunityHasNoConflict()
        {
            await Initialize();

            _alan.BetterFavoriteColor = await _community.AddFactAsync(new Color("Blue"));
            _alan.BetterFavoriteColor = await _community.AddFactAsync(new Color("Red"));

            Assert.IsFalse(_alan.BetterFavoriteColor.InConflict);
        }

        [TestMethod]
        public async Task FactPropertySetFromTwoCommunitiesHasConflict()
        {
            await Initialize();

            _alan.BetterFavoriteColor = await _community.AddFactAsync(new Color("Blue"));
            _otherAlan.BetterFavoriteColor = await _otherCommunity.AddFactAsync(new Color("Red"));
            await SynchronizeAsync();

            Assert.IsTrue(_alan.BetterFavoriteColor.InConflict);
        }

        [TestMethod]
        public async Task CanSeeCandidatesOfFactConflict()
        {
            await Initialize();

            _alan.BetterFavoriteColor = await _community.AddFactAsync(new Color("Blue"));
            _otherAlan.BetterFavoriteColor = await _otherCommunity.AddFactAsync(new Color("Red"));
            await SynchronizeAsync();

            var candidates = _alan.BetterFavoriteColor.Candidates;
            var first = await candidates.ElementAt(0).EnsureAsync();
            var second = await candidates.ElementAt(1).EnsureAsync();
            Assert.IsTrue(second.Name == "Blue");
            Assert.IsTrue(first.Name == "Red");
        }

        private async Task SynchronizeAsync()
        {
            while (await _community.SynchronizeAsync() || await _otherCommunity.SynchronizeAsync()) ;
        }
    }
}
