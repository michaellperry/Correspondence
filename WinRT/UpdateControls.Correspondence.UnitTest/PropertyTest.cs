using System.Collections.Generic;
using System.Linq;
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
        public void Initialize()
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

            _alan = _community.AddFact(new User("alan1"));
            _otherAlan = _otherCommunity.AddFact(new User("alan1"));
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
        public void PropertySetFromTwoCommunitiesHasConflict()
        {
            _alan.FavoriteColor = "Blue";
            _otherAlan.FavoriteColor = "Red";
            Synchronize();

            Assert.IsTrue(_alan.FavoriteColor.InConflict);
        }

        [TestMethod]
        public void CanSeeCandidatesOfConflict()
        {
            _alan.FavoriteColor = "Blue";
            _otherAlan.FavoriteColor = "Red";
            Synchronize();

            IEnumerable<string> candidates = _alan.FavoriteColor.Candidates;
            Assert.IsTrue(candidates.Contains("Blue"));
            Assert.IsTrue(candidates.Contains("Red"));
        }

        [TestMethod]
        public void RedundantSetOfSimplePropertyDoesNotCreateAFact()
        {
            _alan.FavoriteColor = "Blue";
            int before = _storageAlan.LoadAllFacts().Count();
            _alan.FavoriteColor = "Blue";
            int after = _storageAlan.LoadAllFacts().Count();

            Assert.AreEqual(before, after);
        }

        [TestMethod]
        public void CanGetDefaultMutableFactProperty()
        {
            Color color = _alan.BetterFavoriteColor;

            Assert.IsNull(color);
        }

        [TestMethod]
        public void CanSetMutableFactProperty()
        {
            _alan.BetterFavoriteColor = _community.AddFact(new Color("Blue"));
            Color color = _alan.BetterFavoriteColor;

            Assert.AreEqual("Blue", color.Name);
        }

        [TestMethod]
        public void CanChangeMutableFactProperty()
        {
            _alan.BetterFavoriteColor = _community.AddFact(new Color("Blue"));
            _alan.BetterFavoriteColor = _community.AddFact(new Color("Red"));
            Color color = _alan.BetterFavoriteColor;

            Assert.AreEqual("Red", color.Name);
        }

        [TestMethod]
        public void RedundantSetOfFactPropertyDoesNotCreateAFact()
        {
            _alan.BetterFavoriteColor = _community.AddFact(new Color("Blue"));
            int before = _storageAlan.LoadAllFacts().Count();
            _alan.BetterFavoriteColor = _community.AddFact(new Color("Blue"));
            int after = _storageAlan.LoadAllFacts().Count();

            Assert.AreEqual(before, after);
        }

        [TestMethod]
        public void FactPropertySetFromOneCommunityHasNoConflict()
        {
            _alan.BetterFavoriteColor = _community.AddFact(new Color("Blue"));
            _alan.BetterFavoriteColor = _community.AddFact(new Color("Red"));

            Assert.IsFalse(_alan.BetterFavoriteColor.InConflict);
        }

        [TestMethod]
        public void FactPropertySetFromTwoCommunitiesHasConflict()
        {
            _alan.BetterFavoriteColor = _community.AddFact(new Color("Blue"));
            _otherAlan.BetterFavoriteColor = _otherCommunity.AddFact(new Color("Red"));
            Synchronize();

            Assert.IsTrue(_alan.BetterFavoriteColor.InConflict);
        }

        [TestMethod]
        public void CanSeeCandidatesOfFactConflict()
        {
            _alan.BetterFavoriteColor = _community.AddFact(new Color("Blue"));
            _otherAlan.BetterFavoriteColor = _otherCommunity.AddFact(new Color("Red"));
            Synchronize();

            IEnumerable<Color> candidates = _alan.BetterFavoriteColor.Candidates;
            Assert.IsTrue(candidates.Any(color => color.Name == "Blue"));
            Assert.IsTrue(candidates.Any(color => color.Name == "Red"));
        }

        private void Synchronize()
        {
            while (_community.Synchronize() || _otherCommunity.Synchronize()) ;
        }
    }
}
