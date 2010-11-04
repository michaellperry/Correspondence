using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Memory;
using Reversi.Model;
using UpdateControls.Correspondence.Strategy;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.UnitTest
{
    [TestClass]
    public class RedundantFactsTest
    {
        [TestMethod]
        public void WhenAFactIsPublishedToTwoPredecessors_ThenThatFactIsPostedOnce()
        {
            RecordingCommunicationStrategy recorder = new RecordingCommunicationStrategy(
                new MemoryCommunicationStrategy());
            Community community = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(recorder)
                .RegisterAssembly(typeof(Game));

            Game game = community.AddFact(new Game());
            User michael = community.AddFact(new User("michael"));
            Player player = game.CreatePlayer(michael);
            community.Synchronize();

            IEnumerable<FactTreeMemento> postedTrees = recorder.Posted;
            Assert.AreEqual(1, postedTrees.Count());
            Assert.AreEqual(3, recorder.Posted.ElementAt(0).Facts.Count());
        }
    }


}
