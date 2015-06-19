using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Correspondence.Memory;
using Correspondence.UnitTest.Model;
using System.Threading.Tasks;

namespace Correspondence.UnitTest
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
                    .Register<Model.CorrespondenceModel>();
		        _playerTwoCommuniy = new Community(sharedStorage)
                    .AddCommunicationStrategy(new MemoryCommunicationStrategy())
                    .Register<Model.CorrespondenceModel>();
        }

        [TestMethod]
        public async Task WhenLogOnToOtherMachine_ShouldThrow()
        {
            Machine playerOneMachine = await _playerOneCommuniy.AddFactAsync(new Machine());
            User playerOne = await _playerOneCommuniy.AddFactAsync(new User("one"));

            try
            {
                await _playerTwoCommuniy.AddFactAsync(new LogOn(playerOne, playerOneMachine));
                Assert.Fail("AddFact did not throw.");
            }
            catch (CorrespondenceException ex)
            {
                Assert.AreEqual("A fact cannot be added to a different community than its predecessors.", ex.Message);
            }
        }

        [TestMethod]
        public async Task WhenOptionalPredecessorIsNull_ShouldNotThrow()
        {
            Machine playerOneMachine = await _playerOneCommuniy.AddFactAsync(new Machine());
            User playerOneUser = await _playerOneCommuniy.AddFactAsync(new User("one"));
            Game game = await _playerOneCommuniy.AddFactAsync(new Game());
            Player playerOnePlayer = await _playerOneCommuniy.AddFactAsync(new Player(playerOneUser, game, 0));
            Outcome outcome = await _playerOneCommuniy.AddFactAsync(new Outcome(game, null));
        }

        [TestMethod]
        public async Task WhenListPredecessorIsEmpty_ShouldNotThrow()
        {
            Machine playerOneMachine = await _playerOneCommuniy.AddFactAsync(new Machine());
            User playerOneUser = await _playerOneCommuniy.AddFactAsync(new User("one"));
            Game game = await _playerOneCommuniy.AddFactAsync(new Game());
            GameName gameName = await _playerOneCommuniy.AddFactAsync(new GameName(game, new List<GameName>(), "Fischer Spasky 1971, Game 3"));
        }

        [TestMethod]
        public async Task WhenSinglePredecessorHasNoCommunity_ShouldThrow()
        {
            Machine playerOneMachine = await _playerOneCommuniy.AddFactAsync(new Machine());
            User playerOneUser = await _playerOneCommuniy.AddFactAsync(new User("one"));
            Game game = new Game();
            try
            {
                Player playerOnePlayer = await _playerOneCommuniy.AddFactAsync(new Player(playerOneUser, game, 0));
                Assert.Fail("AddFact should have thrown.");
            }
            catch (CorrespondenceException ex)
            {
                Assert.AreEqual("A fact's predecessors must be added to the community first.", ex.Message);
            }
        }

        [TestMethod]
        public async Task WhenListPredecessorHasNoCommunity_ShouldThrow()
        {
            Machine playerOneMachine = await _playerOneCommuniy.AddFactAsync(new Machine());
            User playerOneUser = await _playerOneCommuniy.AddFactAsync(new User("one"));
            Game game = await _playerOneCommuniy.AddFactAsync(new Game());
            GameName gameName = new GameName(game, new List<GameName>(), "Fischer Spasky 1971, Game 3");
            try
            {
                GameName secondGame = await _playerOneCommuniy.AddFactAsync(new GameName(game, new List<GameName>() { gameName }, "Fischer Spasky 1971, Game 3"));
                Assert.Fail("AddFact should have thrown.");
            }
            catch (CorrespondenceException exception)
            {
                Assert.AreEqual("A fact's predecessors must be added to the community first.", exception.Message);
            }
        }

        [TestMethod]
        public async Task WhenListPredecessorFromADifferentCommunity_ShouldThrow()
        {
            Machine playerOneMachine = await _playerOneCommuniy.AddFactAsync(new Machine());
            User playerOneUser = await _playerOneCommuniy.AddFactAsync(new User("one"));
            Game playerOneGame = await _playerOneCommuniy.AddFactAsync(new Game());
            GameName gameName = await _playerOneCommuniy.AddFactAsync(new GameName(playerOneGame, new List<GameName>(), "Fischer Spasky 1971, Game 3"));

            Game playerTwoGame = await _playerTwoCommuniy.AddFactAsync(new Game());
			try
            {
                GameName secondGame = await _playerTwoCommuniy.AddFactAsync(new GameName(playerTwoGame, new List<GameName>() { gameName }, "Fischer Spasky 1971, Game 3"));
                Assert.Fail("AddFact should have thrown.");
            }
            catch (CorrespondenceException exception)
            {
				Assert.AreEqual("A fact cannot be added to a different community than its predecessors.", exception.Message);
            }
        }

        [TestMethod]
        public async Task WhenListPredecessorFromDifferentCommunities_ShouldThrow()
        {
            Machine playerOneMachine = await _playerOneCommuniy.AddFactAsync(new Machine());
            User playerOneUser = await _playerOneCommuniy.AddFactAsync(new User("one"));
            Game playerOneGame = await _playerOneCommuniy.AddFactAsync(new Game());
            GameName gameName = await _playerOneCommuniy.AddFactAsync(new GameName(playerOneGame, new List<GameName>(), "Fischer Spasky 1971, Game 3"));

            Game playerTwoGame = await _playerTwoCommuniy.AddFactAsync(new Game());
            GameName secondName = await _playerTwoCommuniy.AddFactAsync(new GameName(playerTwoGame, new List<GameName>(), "Fischer Spasky 1971, Game 3"));
			try
            {
                GameName secondGame = await _playerOneCommuniy.AddFactAsync(new GameName(playerOneGame, new List<GameName>() { gameName, secondName }, "Fischer Spasky 1971, Game 3"));
                Assert.Fail("AddFact should have thrown.");
            }
            catch (CorrespondenceException exception)
            {
				Assert.AreEqual("A fact cannot be added to a different community than its predecessors.", exception.Message);
            }
        }

        [TestMethod]
        public async Task WhenOptionalPredecessorIsNotPartOfCommunity_ShouldThrow()
        {
            Machine playerOneMachine = await _playerOneCommuniy.AddFactAsync(new Machine());
            User playerOneUser = await _playerOneCommuniy.AddFactAsync(new User("one"));
            Game game = await _playerOneCommuniy.AddFactAsync(new Game());
            Player playerOnePlayer = new Player(playerOneUser, game, 0);
            try
            {
                Outcome outcome = await _playerOneCommuniy.AddFactAsync(new Outcome(game, playerOnePlayer));
                Assert.Fail("AddFact should have thrown");
            }
            catch (CorrespondenceException exception)
            {
                Assert.AreEqual("A fact's predecessors must be added to the community first.", exception.Message);
            }
        }
    }
}
