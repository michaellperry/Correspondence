using System.Linq;
using FactualLive.Models;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FactualLive.UnitTest
{
    [TestClass]
    public class DynamicModelTest : SilverlightTest
    {
        [TestInitialize]
        public void Initialize()
        {
		}

        [TestMethod]
        public void DisplayParseErrors()
        {
            var definition = FactualDefinition.Parse(@"fact Person {
                unique;
                }" );
            Assert.AreEqual("Key section is required.", definition.Errors.Single().Message);
        }
    }
}
