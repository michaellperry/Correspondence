using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Memory;

namespace $rootnamespace$
{
    [TestClass]
    public class ModelTest
    {
        private Community _communityFlynn;
        private Community _communityAlan;
        private Individual _individualFlynn;
        private Individual _individualAlan;

        [TestInitialize]
        public void Initialize()
        {
            var sharedCommunication = new MemoryCommunicationStrategy();
            _communityFlynn = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(sharedCommunication)
                .Register<CorrespondenceModel>()
                .Subscribe(() => _individualFlynn)
				;
            _communityAlan = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(sharedCommunication)
                .Register<CorrespondenceModel>()
                .Subscribe(() => _individualAlan)
                ;
        }

        private async Task CreateIndividualsAsync()
        {
            _individualFlynn = await _communityFlynn.AddFactAsync(
                new Individual("flynn"));
            _individualAlan = await _communityAlan.AddFactAsync(
                new Individual("alan"));
        }

        [TestMethod]
        public async Task MyTest()
        {
            await CreateIndividualsAsync();

            await SynchronizeAsync();

            Assert.Fail();
        }

        private async Task SynchronizeAsync()
        {
            while (await _communityFlynn.SynchronizeAsync() || await _communityAlan.SynchronizeAsync()) ;
        }
	}
}
