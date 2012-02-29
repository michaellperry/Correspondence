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

        [TestMethod]
        public void NoParseErrors()
        {
            var definition = FactualDefinition.Parse(@"fact Person {
                key:
                unique;
                }");
            Assert.AreEqual(0, definition.Errors.Count());
        }

        [TestMethod]
        public void DisplayAnalysisErrors()
        {
            var definition = FactualDefinition.Parse(@"fact Person {
                key:
                  unique;

                mutable:
                  string Name;

                query:
                  Person* siblings {
                    Person p : p.parent = this.parent
                  }
                }");

            Assert.AreEqual("The member \"Person.parent\" is not defined.", definition.Errors.Single().Message);
        }
    }
}
