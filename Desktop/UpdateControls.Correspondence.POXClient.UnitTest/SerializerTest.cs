using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.POXClient.Contract;
using UpdateControls.Correspondence.Mementos;
using System.Text;

namespace UpdateControls.Correspondence.POXClient.UnitTest
{
    [TestClass]
    public class SerializerTest
    {
        private CorrespondenceFactType TYPE_IdentityService = new CorrespondenceFactType("FacetedWorlds.Reversi.Model.IdentityService", 1);
        private CorrespondenceFactType TYPE_Identity = new CorrespondenceFactType("FacetedWorlds.Reversi.Model.Identity", 1);
        private CorrespondenceFactType TYPE_EnableToastNotification = new CorrespondenceFactType("FacetedWorlds.Reversi.Model.EnableToastNotification", 1);

        [TestMethod]
        public void CanSerializeGetRequest()
        {
            GetRequest getRequest = new GetRequest
            {
                Domain = "LnldvHcytc2Jhv",
                ClientGuid = "25a0ff9b-478c-4bf7-8ce5-0e87832f0ff5",
                PivotTree = new FactTree
                {
                    DatabaseId = 1,
                    Facts = new Fact[]
                    {
                        new Fact
                        {
                            Data = Convert.FromBase64String("BG1pa2U="),
                            FactId = 23,
                            FactTypeId = 1,
                            Predecessors = new Predecessor[0]
                        }
                    },
                    Roles = new FactRole[0],
                    Types = new FactType[]
                    {
                        new FactType
                        {
                            TypeId = 1,
                            TypeName = "Reversi.Model.User",
                            Version = 1
                        }
                    }
                },
                PivotId = 23,
                Timestamp = 34
            };

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("c", "http://correspondence.updatecontrols.com/pox/1.0");
            XmlSerializer getRequestSerializer = new XmlSerializer(typeof(GetRequest));
            MemoryStream outputStream = new MemoryStream();
            getRequestSerializer.Serialize(outputStream, getRequest, namespaces);
            string getRequestXml;
            using (StreamReader reader = new StreamReader(new MemoryStream(outputStream.ToArray())))
            {
                getRequestXml = reader.ReadToEnd();
            }

            ShouldEqual(
                "<?xml version=\"1.0\"?>\r\n" +
                "<c:GetRequest xmlns:c=\"http://correspondence.updatecontrols.com/pox/1.0\">\r\n" +
                "  <Domain>LnldvHcytc2Jhv</Domain>\r\n" +
                "  <PivotTree>\r\n" +
                "    <DatabaseId>1</DatabaseId>\r\n" +
                "    <Facts>\r\n" +
                "      <Fact>\r\n" +
                "        <Data>BG1pa2U=</Data>\r\n" +
                "        <FactId>23</FactId>\r\n" +
                "        <FactTypeId>1</FactTypeId>\r\n" +
                "        <Predecessors />\r\n" +
                "      </Fact>\r\n" +
                "    </Facts>\r\n" +
                "    <Roles />\r\n" +
                "    <Types>\r\n" +
                "      <FactType>\r\n" +
                "        <TypeId>1</TypeId>\r\n" +
                "        <TypeName>Reversi.Model.User</TypeName>\r\n" +
                "        <Version>1</Version>\r\n" +
                "      </FactType>\r\n" +
                "    </Types>\r\n" +
                "  </PivotTree>\r\n" +
                "  <PivotId>23</PivotId>\r\n" +
                "  <Timestamp>34</Timestamp>\r\n" +
                "  <ClientGuid>25a0ff9b-478c-4bf7-8ce5-0e87832f0ff5</ClientGuid>\r\n" +
                "</c:GetRequest>",
                getRequestXml);
        }

        [TestMethod]
        public void XMLSerializationIsBig()
        {
            MementoToContractTranslator translator = new MementoToContractTranslator(0);
            translator.AddFact(new IdentifiedFactMemento(
                new FactID { key = 1L },
                new FactMemento(TYPE_Identity)
                {
                    Data = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())
                }
            ));
            translator.AddFact(new IdentifiedFactMemento(
                new FactID { key = 2L },
                new FactMemento(TYPE_EnableToastNotification)
                {
                    Data = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())
                }
                .AddPredecessor(
                    new RoleMemento(TYPE_EnableToastNotification, "identity", TYPE_Identity, true),
                    new FactID { key = 1L },
                    true
                )
            ));
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("c", "http://correspondence.updatecontrols.com/pox/1.0");
            XmlSerializer getRequestSerializer = new XmlSerializer(typeof(FactTree));
            MemoryStream outputStream = new MemoryStream();
            getRequestSerializer.Serialize(outputStream, translator.TargetFactTree, namespaces);

            byte[] xmlSerialized = outputStream.ToArray();

            Assert.AreEqual(391, xmlSerialized.Length);
        }

        private void ShouldEqual(string expected, string actual)
        {
            if (actual != expected)
            {
                int index = 0;
                int column = 1;
                int line = 1;
                while (
                    index < actual.Length &&
                    index < expected.Length &&
                    actual[index] == expected[index])
                {
                    if (expected[index] == '\n')
                    {
                        column = 1;
                        ++line;
                    }
                    else
                    {
                        ++column;
                    }
                    index++;
                }
                Assert.Fail(String.Format(
                    "Strings differ at line {0}, column {1}. Expected \"{2}\", actual \"{3}\".\r\n{4}", 
                    line,
                    column,
                    Snippet(expected, index),
                    Snippet(actual, index),
                    actual));
                Assert.AreEqual(expected, actual);
            }
        }

        private static string Snippet(string value, int position)
        {
            int startingIndex = position - 9;
            if (startingIndex < 0)
                startingIndex = 0;
            int endingIndex = position + 9;
            if (endingIndex > value.Length)
                endingIndex = value.Length;
            return value.Substring(startingIndex, endingIndex - startingIndex);
        }
    }
}
