using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Memory;
using Reversi.Model;
using UpdateControls.Correspondence.Strategy;
using UpdateControls.Correspondence.Mementos;
using System.IO;

namespace UpdateControls.Correspondence.UnitTest
{
    [TestClass]
    public class RedundantFactsTest
    {
		private static CorrespondenceFactType PlayerFactType = new CorrespondenceFactType("Reversi.Model.Player", 1);
		private static CorrespondenceFactType GameFactType = new CorrespondenceFactType("Reversi.Model.Game", 1);
		private static CorrespondenceFactType UserFactType = new CorrespondenceFactType("Reversi.Model.User", 1);

        private MemoryCommunicationStrategy _memoryCommunicationStrategy;
        private RecordingCommunicationStrategy _recorder;
		private Community _community;
		private User _michael;

		private long _nextFactId = 1L;

		[TestInitialize]
		public void Initialize()
		{
			_memoryCommunicationStrategy = new MemoryCommunicationStrategy();
			_recorder = new RecordingCommunicationStrategy(_memoryCommunicationStrategy);
			_community = new Community(new MemoryStorageStrategy())
				.AddCommunicationStrategy(_recorder)
				.RegisterAssembly(typeof(Game))
				.Subscribe(() => _michael);
			_michael = CreateUser("michael");
		}

        [TestMethod]
        public void WhenAFactIsPublishedToTwoPredecessors_ThenThatFactIsPostedOnce()
        {
			Game game = CreateGame();
			Player player = game.CreatePlayer(_michael);
			while (_community.Synchronize());

            IEnumerable<FactTreeMemento> postedTrees = _recorder.Posted;
            Assert.AreEqual(1, postedTrees.Count());
            Assert.AreEqual(3, _recorder.Posted.ElementAt(0).Facts.Count());
        }

		[TestMethod]
		public void WhenAFactIsReceived_ThenThatFactIsNotPosted()
		{
			FactTreeMemento gameTreeMemento = new FactTreeMemento(0L);
			IdentifiedFactMemento gameMemento = CreateGameMemento();
			IdentifiedFactMemento userMemento = CreateUserMemento("michael");
			IdentifiedFactMemento playerMemento = CreatePlayerMemento(gameMemento.Id, userMemento.Id, 1);
			gameTreeMemento.Add(gameMemento);
			gameTreeMemento.Add(userMemento);
			gameTreeMemento.Add(playerMemento);
			_memoryCommunicationStrategy.Post(gameTreeMemento);

			Assert.AreEqual(0, _michael.ActivePlayers.Count());
			while (_community.Synchronize());
			Assert.AreEqual(1, _michael.ActivePlayers.Count());

            IEnumerable<FactTreeMemento> postedTrees = _recorder.Posted;
            Assert.AreEqual(0, postedTrees.Count());
		}

		private Game CreateGame()
		{
			return _community.AddFact(new Game());
		}

		private User CreateUser(string userName)
		{
			return _community.AddFact(new User(userName));
		}

		private IdentifiedFactMemento CreateGameMemento()
		{
			return new IdentifiedFactMemento(
				new FactID { key = GetNextFactId() },
				new FactMemento(GameFactType)
				{
					Data = GuidToBinary(Guid.NewGuid())
				});
		}

		private IdentifiedFactMemento CreateUserMemento(string userName)
		{
			return new IdentifiedFactMemento(
				new FactID { key = GetNextFactId() },
				new FactMemento(UserFactType)
				{
					Data = StringToBinary(userName)
				});
		}

		private IdentifiedFactMemento CreatePlayerMemento(FactID gameFactId, FactID userFactId, int playerIndex)
		{
			IdentifiedFactMemento playerMemento = new IdentifiedFactMemento(
				new FactID { key = GetNextFactId() },
				new FactMemento(PlayerFactType)
				{
					Data = IntToBinary(playerIndex)
				});
			playerMemento.Memento.AddPredecessor(
				new RoleMemento(PlayerFactType, "game", GameFactType, true),
				gameFactId);
			playerMemento.Memento.AddPredecessor(
				new RoleMemento(PlayerFactType, "user", UserFactType, true),
				userFactId);
			return playerMemento;
		}

		private long GetNextFactId()
		{
			long gameFactId = _nextFactId;
			++_nextFactId;
			return gameFactId;
		}

		private static byte[] StringToBinary(string value)
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(memoryStream);
			writer.Write(value);
			writer.Flush();
			return memoryStream.ToArray();
		}

		private static byte[] IntToBinary(int value)
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(memoryStream);
			writer.Write(value);
			writer.Flush();
			return memoryStream.ToArray();
		}

		private static byte[] GuidToBinary(Guid value)
		{
			return value.ToByteArray();
		}
	}


}
