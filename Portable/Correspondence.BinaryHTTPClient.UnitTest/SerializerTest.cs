﻿using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Correspondence.Mementos;
using System;
using System.Text;

namespace Correspondence.BinaryHTTPClient.UnitTest
{
    [TestClass]
    public class SerializerTest
    {
        private CorrespondenceFactType TYPE_IdentityService = new CorrespondenceFactType("FacetedWorlds.Reversi.Model.IdentityService", 1);
        private CorrespondenceFactType TYPE_Identity = new CorrespondenceFactType("FacetedWorlds.Reversi.Model.Identity", 1);
        private CorrespondenceFactType TYPE_EnableToastNotification = new CorrespondenceFactType("FacetedWorlds.Reversi.Model.EnableToastNotification", 1);

        [TestMethod]
        public void CanSerializeAnEmptyTree()
        {
            FactTreeMemento factTree = new FactTreeMemento(0);
            FactTreeMemento deserializedFactTree = Deserialize(Serialize(factTree));

            Assert.AreEqual(factTree, deserializedFactTree);
        }

        [TestMethod]
        public void CanSerializeASingleton()
        {
            FactTreeMemento factTree = new FactTreeMemento(0);
            factTree.Add(new IdentifiedFactMemento(
                new FactID { key = 1L },
                new FactMemento(TYPE_IdentityService)
                {
                    Data = new byte[0]
                }
            ));
            FactTreeMemento deserializedFactTree = Deserialize(Serialize(factTree));

            Assert.AreEqual(factTree, deserializedFactTree);
        }

        [TestMethod]
        public void CanSerializeARootFact()
        {
            FactTreeMemento factTree = new FactTreeMemento(0);
            factTree.Add(new IdentifiedFactMemento(
                new FactID { key = 1L },
                new FactMemento(TYPE_Identity)
                {
                    Data = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())
                }
            ));
            FactTreeMemento deserializedFactTree = Deserialize(Serialize(factTree));

            Assert.AreEqual(factTree, deserializedFactTree);
        }

        [TestMethod]
        public void CanSerializeASubFact()
        {
            FactTreeMemento factTree = new FactTreeMemento(0);
            factTree.Add(new IdentifiedFactMemento(
                new FactID { key = 1L },
                new FactMemento(TYPE_Identity)
                {
                    Data = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())
                }
            ));
            factTree.Add(new IdentifiedFactMemento(
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
            FactTreeMemento deserializedFactTree = Deserialize(Serialize(factTree));

            Assert.AreEqual(factTree, deserializedFactTree);
            Assert.AreEqual(
                ((IdentifiedFactMemento)factTree.Facts.ElementAt(1)).Memento.Predecessors.ElementAt(0).IsPivot,
                ((IdentifiedFactMemento)deserializedFactTree.Facts.ElementAt(1)).Memento.Predecessors.ElementAt(0).IsPivot);
        }

        [TestMethod]
        public void CanSerializeARemoteFactId()
        {
            FactTreeMemento factTree = new FactTreeMemento(0)
                .Add(new IdentifiedFactRemote(new FactID { key = 1 }, new FactID { key = 42 }));
            FactTreeMemento deserializedFactTree = Deserialize(Serialize(factTree));
        }

        [TestMethod]
        public void BinarySerializationIsSmall()
        {
            FactTreeMemento factTree = new FactTreeMemento(0);
            factTree.Add(new IdentifiedFactMemento(
                new FactID { key = 1L },
                new FactMemento(TYPE_Identity)
                {
                    Data = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())
                }
            ));
            factTree.Add(new IdentifiedFactMemento(
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
            byte[] binarySerialized = Serialize(factTree);

            Assert.AreEqual(238, binarySerialized.Length);
        }

        private static byte[] Serialize(FactTreeMemento factTree)
        {
            using (MemoryStream data = new MemoryStream())
            {
                using (BinaryWriter output = new BinaryWriter(data))
                {
                    new FactTreeSerlializer().SerlializeFactTree(factTree, output);
                }
                return data.ToArray();
            }
        }

        private static FactTreeMemento Deserialize(byte[] buffer)
        {
            using (MemoryStream newMemoryStream = new MemoryStream(buffer))
            {
                using (BinaryReader reader = new BinaryReader(newMemoryStream))
                {
                    return new FactTreeSerlializer().DeserializeFactTree(reader);
                }
            }
        }
    }
}
