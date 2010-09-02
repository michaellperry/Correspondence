using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Memory;
using Reversi.Model;

namespace UpdateControls.Correspondence.UnitTest
{
    [TestClass]
    public class MultiCommunityTest
    {
        private Community _playerOneCommuniy;
        private Community _playerTwoCommuniy;

        [TestInitialize]
        public void Initialize()
        {
		        MemoryStorageStrategy sharedStorage = new MemoryStorageStrategy();
		        _playerOneCommuniy = new Community(sharedStorage)
                    .AddCommunicationStrategy(new MemoryCommunicationStrategy())
                    .RegisterAssembly(typeof(Machine));
		        _playerTwoCommuniy = new Community(sharedStorage)
                    .AddCommunicationStrategy(new MemoryCommunicationStrategy())
                    .RegisterAssembly(typeof(Machine));
        }

        [TestMethod]
        public void WhenLogOnToOtherMachine_ShouldThrow()
        {
            Machine playerOneMachine = _playerOneCommuniy.AddFact(new Machine());
            User playerOne = _playerOneCommuniy.AddFact(new User("one"));

            try
            {
                _playerTwoCommuniy.AddFact(new LogOn(playerOne, playerOneMachine));
                Assert.Fail("AddFact did not throw.");
            }
            catch (CorrespondenceException ex)
            {
                Assert.AreEqual("A fact cannot be added to a different community than its predecessors.", ex.Message);
            }
        }

        [TestMethod]
        public void WhenOptionalPredecessorIsNull_ShouldNotThrow()
        {
            Machine playerOneMachine = _playerOneCommuniy.AddFact(new Machine());
            User playerOneUser = _playerOneCommuniy.AddFact(new User("one"));
            Game game = _playerOneCommuniy.AddFact(new Game());
            Player playerOnePlayer = _playerOneCommuniy.AddFact(new Player(playerOneUser, game, 0));
            Outcome outcome = _playerOneCommuniy.AddFact(new Outcome(game, null));
        }

        [TestMethod]
        public void WhenListPredecessorIsEmpty_ShouldNotThrow()
        {
            Machine playerOneMachine = _playerOneCommuniy.AddFact(new Machine());
            User playerOneUser = _playerOneCommuniy.AddFact(new User("one"));
            Game game = _playerOneCommuniy.AddFact(new Game());
            GameName gameName = _playerOneCommuniy.AddFact(new GameName(game, new List<GameName>(), "Fischer Spasky 1971, Game 3"));
        }

        [TestMethod]
        public void WhenSinglePredecessorHasNoCommunity_ShouldThrow()
        {
            Machine playerOneMachine = _playerOneCommuniy.AddFact(new Machine());
            User playerOneUser = _playerOneCommuniy.AddFact(new User("one"));
            Game game = new Game();
            try
            {
                Player playerOnePlayer = _playerOneCommuniy.AddFact(new Player(playerOneUser, game, 0));
                Assert.Fail("AddFact should have thrown.");
            }
            catch (CorrespondenceException ex)
            {
                Assert.AreEqual("A fact's predecessors must be added to the community first.", ex.Message);
            }
        }

        [TestMethod]
        public void WhenListPredecessorHasNoCommunity_ShouldThrow()
        {
            Machine playerOneMachine = _playerOneCommuniy.AddFact(new Machine());
            User playerOneUser = _playerOneCommuniy.AddFact(new User("one"));
            Game game = _playerOneCommuniy.AddFact(new Game());
            GameName gameName = new GameName(game, new List<GameName>(), "Fischer Spasky 1971, Game 3");
            try
            {
                GameName secondGame = _playerOneCommuniy.AddFact(new GameName(game, new List<GameName>() { gameName }, "Fischer Spasky 1971, Game 3"));
                Assert.Fail("AddFact should have thrown.");
            }
            catch (CorrespondenceException exception)
            {
                Assert.AreEqual("A fact's predecessors must be added to the community first.", exception.Message);
            }
        }

        [TestMethod]
        public void WhenOptionalPredecessorIsNotPartOfCommunity_ShouldThrow()
        {
            Machine playerOneMachine = _playerOneCommuniy.AddFact(new Machine());
            User playerOneUser = _playerOneCommuniy.AddFact(new User("one"));
            Game game = _playerOneCommuniy.AddFact(new Game());
            Player playerOnePlayer = new Player(playerOneUser, game, 0);
            try
            {
                Outcome outcome = _playerOneCommuniy.AddFact(new Outcome(game, playerOnePlayer));
                Assert.Fail("AddFact should have thrown");
            }
            catch (CorrespondenceException exception)
            {
                Assert.AreEqual("A fact's predecessors must be added to the community first.", exception.Message);
            }
        }
    }
}
