using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Memory;
using System.Linq;

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
            FactTreeMemento messageBody = new FactTreeMemento(0);
            messageBody.Add(new IdentifiedFactRemote(new FactID { key = 1 }, new FactID { key = 42 }));
            messageBody.Add(new IdentifiedFactMemento(new FactID { key = 2 }, new FactMemento(TYPE_Outcome)
                {
                    Data = new byte[0]
                }
                .AddPredecessor(new RoleMemento(TYPE_Outcome, "game", TYPE_Game, true), new FactID { key = 1 }, true)
            ));

            Model.Game game = null;
            Community community = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(new MockCommunicationStrategy(messageBody))
                .Register<Model.CorrespondenceModel>()
                .Subscribe(() => game);
            game = community.AddFact(new Model.Game());

            community.Synchronize();

            Model.Outcome outcome = game.Outcomes.Single();
            Assert.IsNull(outcome.Winner);
        }
    }
}
