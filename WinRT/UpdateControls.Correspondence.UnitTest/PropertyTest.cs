using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.UnitTest.Model;

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

        [TestInitialize]
        public async Task Initialize()
        {
            var sharedCommunication = new MemoryCommunicationStrategy();
            _storageAlan = new MemoryStorageStrategy();
            _community = new Community(_storageAlan)
                .AddCommunicationStrategy(sharedCommunication)
                .Register<Model.CorrespondenceModel>()
                .Subscribe(() => _alan);
            _otherCommunity = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(sharedCommunication)
                .Register<Model.CorrespondenceModel>()
                .Subscribe(() => _otherAlan);

            _alan = await _community.AddFactAsync(new User("alan1"));
            _otherAlan = await _otherCommunity.AddFactAsync(new User("alan1"));
        }

        [TestMethod]
        public void CanGetDefaultMutableProperty()
        {
            string favoriteColor = _alan.FavoriteColor;

            Assert.AreEqual(default(string), favoriteColor);
        }

        [TestMethod]
        public void CanSetMutableProperty()
        {
            _alan.FavoriteColor = "Blue";
            string favoriteColor = _alan.FavoriteColor;

            Assert.AreEqual("Blue", favoriteColor);
        }

        [TestMethod]
        public void CanChangeMutableProperty()
        {
            _alan.FavoriteColor = "Blue";
            _alan.FavoriteColor = "Red";
            string favoriteColor = _alan.FavoriteColor;

            Assert.AreEqual("Red", favoriteColor);
        }

        [TestMethod]
        public void PropertySetFromOneCommunityHasNoConflict()
        {
            _alan.FavoriteColor = "Blue";
            _alan.FavoriteColor = "Red";

            Assert.IsFalse(_alan.FavoriteColor.InConflict);
        }

        [TestMethod]
        public async Task PropertySetFromTwoCommunitiesHasConflict()
        {
            _alan.FavoriteColor = "Blue";
            _otherAlan.FavoriteColor = "Red";
            await Synchronize();

            Assert.IsTrue(_alan.FavoriteColor.InConflict);
        }

        [TestMethod]
        public async Task CanSeeCandidatesOfConflict()
        {
            _alan.FavoriteColor = "Blue";
            _otherAlan.FavoriteColor = "Red";
            await Synchronize();

            IEnumerable<string> candidates = _alan.FavoriteColor.Candidates;
            Assert.IsTrue(candidates.Contains("Blue"));
            Assert.IsTrue(candidates.Contains("Red"));
        }

        [TestMethod]
        public void RedundantSetOfSimplePropertyDoesNotCreateAFact()
        {
            _alan.FavoriteColor = "Blue";
            int before = _storageAlan.GetFactCount();
            _alan.FavoriteColor = "Blue";
            int after = _storageAlan.GetFactCount();

            Assert.AreEqual(before, after);
        }

        [TestMethod]
        public void CanGetDefaultMutableFactProperty()
        {
            Color color = _alan.BetterFavoriteColor;

            Assert.IsNotNull(color);
            Assert.IsTrue(color.IsNull);
        }

        [TestMethod]
        public async Task CanSetMutableFactProperty()
        {
            _alan.BetterFavoriteColor = await _community.AddFactAsync(new Color("Blue"));
            Color color = _alan.BetterFavoriteColor;

            Assert.AreEqual("Blue", color.Name);
        }

        [TestMethod]
        public async Task CanChangeMutableFactProperty()
        {
            _alan.BetterFavoriteColor = await _community.AddFactAsync(new Color("Blue"));
            _alan.BetterFavoriteColor = await _community.AddFactAsync(new Color("Red"));
            Color color = _alan.BetterFavoriteColor;

            Assert.AreEqual("Red", color.Name);
        }

        [TestMethod]
        public async Task RedundantSetOfFactPropertyDoesNotCreateAFact()
        {
            _alan.BetterFavoriteColor = await _community.AddFactAsync(new Color("Blue"));
            int before = _storageAlan.GetFactCount();
            _alan.BetterFavoriteColor = await _community.AddFactAsync(new Color("Blue"));
            int after = _storageAlan.GetFactCount();

            Assert.AreEqual(before, after);
        }

        [TestMethod]
        public async Task FactPropertySetFromOneCommunityHasNoConflict()
        {
            _alan.BetterFavoriteColor = await _community.AddFactAsync(new Color("Blue"));
            _alan.BetterFavoriteColor = await _community.AddFactAsync(new Color("Red"));

            Assert.IsFalse(_alan.BetterFavoriteColor.InConflict);
        }

        [TestMethod]
        public async Task FactPropertySetFromTwoCommunitiesHasConflict()
        {
            _alan.BetterFavoriteColor = await _community.AddFactAsync(new Color("Blue"));
            _otherAlan.BetterFavoriteColor = await _otherCommunity.AddFactAsync(new Color("Red"));
            await Synchronize();

            Assert.IsTrue(_alan.BetterFavoriteColor.InConflict);
        }

        [TestMethod]
        public async Task CanSeeCandidatesOfFactConflict()
        {
            _alan.BetterFavoriteColor = await _community.AddFactAsync(new Color("Blue"));
            _otherAlan.BetterFavoriteColor = await _otherCommunity.AddFactAsync(new Color("Red"));
            await Synchronize();

            IEnumerable<Color> candidates = _alan.BetterFavoriteColor.Candidates;
            Assert.IsTrue(candidates.Any(color => color.Name == "Blue"));
            Assert.IsTrue(candidates.Any(color => color.Name == "Red"));
        }

        private async Task Synchronize()
        {
            while (
                await _community.SynchronizeAsync() ||
                await _otherCommunity.SynchronizeAsync()) ;
        }
    }
}
