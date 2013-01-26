using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Incentives.Model;
using Incentives.ViewModel;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UpdateControls.Correspondence.FileStream;

namespace UpdateControls.Correspondence.PerformanceTest
{
    [TestClass]
    public class FileStreamPerformance
    {
        private Community _community;

        [TestInitialize]
        public async Task Initialize()
        {
            var storage = new FileStreamStorageStrategy();
            _community = new Community(storage);
            _community.Register<CorrespondenceModel>();
        }

        [TestMethod]
        public async Task GenerateCategories()
        {
            var company = await _community.AddFactAsync(new Company("improvingEnterprises"));
            var quarter = await _community.AddFactAsync(new Quarter(company, new DateTime(2013, 1, 1)));

            var categoryGenerator = new CategoryGenerator(_community, company, quarter);
            await categoryGenerator.GenerateAsync();
        }
    }
}
