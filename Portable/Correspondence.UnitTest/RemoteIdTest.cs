using Microsoft.VisualStudio.TestTools.UnitTesting;
using Correspondence.Mementos;
using Correspondence.Memory;
using System.Linq;
using Correspondence.UnitTest.Model;
using System.Threading.Tasks;

namespace Correspondence.UnitTest
{
    [TestClass]
    public class RemoteIdTest
    {
        private static CorrespondenceFactType TYPE_Game = new CorrespondenceFactType("Correspondence.UnitTest.Model.Game", 2);
        private static CorrespondenceFactType TYPE_Outcome = new CorrespondenceFactType("Correspondence.UnitTest.Model.Outcome", 887939540);

        [TestMethod]
        public async Task CanReceiveATreeWithARemoteId()
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
            game = await community.AddFactAsync(new Game());
            await storage.SaveShareAsync(1, new FactID { key = 42 }, new FactID { key = 1 });

            await community.SynchronizeAsync();

            Outcome outcome = game.Outcomes.Single();
            Assert.IsNotNull(outcome.Winner);
            Assert.IsTrue(outcome.Winner.IsNull);
        }

        [TestMethod]
        public async Task WillCallGetWithARemoteFactID()
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
            game = await community.AddFactAsync(sourceGame);

            // On the first synchronize, we get the game memento.
            bool gotSomething = await community.SynchronizeAsync();
            Assert.IsTrue(gotSomething);

            // On the second synchronize, we ask for successors of the game.
            gotSomething = await community.SynchronizeAsync();
            Assert.IsFalse(gotSomething);

            // We knew the remote ID, so we used it.
            Assert.AreEqual(42, ((IdentifiedFactRemote)(communication.PivotTree.Facts.Single())).RemoteId.key);
        }
    }
}
