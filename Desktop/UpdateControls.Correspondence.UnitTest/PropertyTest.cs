using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.UnitTest.Model;
using UpdateControls.Correspondence.Memory;

namespace UpdateControls.Correspondence.UnitTest
{
    [TestClass]
    public class PropertyTest
    {
        private Community _community;
        private Community _otherCommunity;
        private User _alan;
        private User _otherAlan;

        [TestInitialize]
        public void Initialize()
        {
            var sharedCommunication = new MemoryCommunicationStrategy();
            _community = new Community(new MemoryStorageStrategy())
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

        private void Synchronize()
        {
            while (_community.Synchronize() || _otherCommunity.Synchronize()) ;
        }
    }
}
