using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Memory;
using System.Linq;
using UpdateControls.Correspondence.UnitTest.Model;

namespace UpdateControls.Correspondence.UnitTest
{
    [TestClass]
    public class RemoteIdTest
    {
        private static CorrespondenceFactType TYPE_Game = new CorrespondenceFactType("UpdateControls.Correspondence.UnitTest.Model.Game", 1);
        private static CorrespondenceFactType TYPE_Outcome = new CorrespondenceFactType("UpdateControls.Correspondence.UnitTest.Model.Outcome", 1);

        [TestMethod]
        public void CanReceiveATreeWithARemoteId()
        {
            FactTreeMemento messageBody = new FactTreeMemento(0)
                .Add(new IdentifiedFactRemote(new FactID { key = 1 }, new FactID { key = 42 }))
                .Add(new IdentifiedFactMemento(new FactID { key = 2 }, new FactMemento(TYPE_Outcome)
                    {
                        Data = new byte[0]
                    }
                    .AddPredecessor(new RoleMemento(TYPE_Outcome, "game", TYPE_Game, true), new FactID { key = 1 }, true)
                ));

            Game game = null;
            var storage = new MemoryStorageStrategy();
            var communication = new MockCommunicationStrategy()
                .Returns(messageBody);
            var community = new Community(storage)
                .AddCommunicationStrategy(communication)
                .Register<CorrespondenceModel>()
                .Subscribe(() => game);
            game = community.AddFact(new Game());
            storage.SaveShare(1, new FactID { key = 42 }, new FactID { key = 1 });

            community.Synchronize();

            Outcome outcome = game.Outcomes.Single();
            Assert.IsNull(outcome.Winner);
        }

        [TestMethod]
        public void WillCallGetWithARemoteFactID()
        {
            Game sourceGame = new Game();
            Game game = null;
            var storage = new MemoryStorageStrategy();
            var communication = new MockCommunicationStrategy()
                .Returns(new FactTreeMemento(0)
                    .Add(new IdentifiedFactMemento(new FactID { key = 42 }, new FactMemento(TYPE_Game)
                    {
                        Data = sourceGame.Unique.ToByteArray()
                    }
                    ))
                )
                .Returns(new FactTreeMemento(0));
            var community = new Community(storage)
                .AddCommunicationStrategy(communication)
                .Register<CorrespondenceModel>()
                .Subscribe(() => game);
            game = community.AddFact(sourceGame);

            // On the first synchronize, we get the game memento.
            bool gotSomething = community.Synchronize();
            Assert.IsTrue(gotSomething);

            // On the second synchronize, we ask for successors of the game.
            gotSomething = community.Synchronize();
            Assert.IsFalse(gotSomething);

            // We knew the remote ID, so we used it.
            Assert.AreEqual(42, ((IdentifiedFactRemote)(communication.PivotTree.Facts.Single())).RemoteId.key);
        }
    }
}
