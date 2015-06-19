using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Correspondence.Memory;
using Correspondence.UnitTest.Model;
using Correspondence.Strategy;
using Correspondence.Mementos;
using System.IO;
using Correspondence.FieldSerializer;
using System.Threading.Tasks;

namespace Correspondence.UnitTest
{
    [TestClass]
    public class RedundantFactsTest
    {
        private static CorrespondenceFactType PlayerFactType = new CorrespondenceFactType("Correspondence.UnitTest.Model.Player", -1815448412);
		private static CorrespondenceFactType GameFactType = new CorrespondenceFactType("Correspondence.UnitTest.Model.Game", 2);
		private static CorrespondenceFactType UserFactType = new CorrespondenceFactType("Correspondence.UnitTest.Model.User", 8);

        private MemoryCommunicationStrategy _memoryCommunicationStrategy;
        private RecordingCommunicationStrategy _recorder;
		private Community _community;
		private User _michael;

		private long _nextFactId = 1L;

		public async Task Initialize()
		{
			_memoryCommunicationStrategy = new MemoryCommunicationStrategy();
			_recorder = new RecordingCommunicationStrategy(_memoryCommunicationStrategy);
			_community = new Community(new MemoryStorageStrategy())
				.AddCommunicationStrategy(_recorder)
                .Register<Model.CorrespondenceModel>()
				.Subscribe(() => _michael);
            _michael = await CreateUserAsync("michael");
		}

        [TestMethod]
        public async Task WhenAFactIsPublishedToTwoPredecessors_ThenThatFactIsPostedOnce()
        {
            await Initialize();

            Game game = await CreateGameAsync();
			Player player = await game.CreatePlayerAsync(_michael);
            while (await _community.SynchronizeAsync()) ;

            IEnumerable<FactTreeMemento> postedTrees = _recorder.Posted;
            Assert.AreEqual(1, postedTrees.Count());
            Assert.AreEqual(3, _recorder.Posted.ElementAt(0).Facts.Count());
        }

		[TestMethod]
		public async Task WhenAFactIsReceived_ThenThatFactIsNotPosted()
		{
            await Initialize();

            FactTreeMemento gameTreeMemento = new FactTreeMemento(0L);
			IdentifiedFactMemento gameMemento = CreateGameMemento();
			IdentifiedFactMemento userMemento = CreateUserMemento("michael");
			IdentifiedFactMemento playerMemento = CreatePlayerMemento(gameMemento.Id, userMemento.Id, 1);
			gameTreeMemento.Add(gameMemento);
			gameTreeMemento.Add(userMemento);
			gameTreeMemento.Add(playerMemento);
			_memoryCommunicationStrategy.Post(gameTreeMemento, new List<UnpublishMemento>());

			Assert.AreEqual(0, _michael.ActivePlayers.Count());
            while (await _community.SynchronizeAsync()) ;
			Assert.AreEqual(1, _michael.ActivePlayers.Count());

            IEnumerable<FactTreeMemento> postedTrees = _recorder.Posted;
            Assert.AreEqual(0, postedTrees.Count());
		}

		private async Task<Game> CreateGameAsync()
		{
            return await _community.AddFactAsync(new Game());
		}

		private async Task<User> CreateUserAsync(string userName)
		{
            return await _community.AddFactAsync(new User(userName));
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
				gameFactId,
                true);
			playerMemento.Memento.AddPredecessor(
				new RoleMemento(PlayerFactType, "user", UserFactType, true),
				userFactId,
                true);
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
            StringFieldSerializer serializer = new StringFieldSerializer();
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(memoryStream);
            serializer.WriteData(writer, value);
			writer.Flush();
			return memoryStream.ToArray();
		}

		private static byte[] IntToBinary(int value)
		{
            IntFieldSerializer serializer = new IntFieldSerializer();
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(memoryStream);
            serializer.WriteData(writer, value);
			writer.Flush();
			return memoryStream.ToArray();
		}

		private static byte[] GuidToBinary(Guid value)
		{
			return value.ToByteArray();
		}
	}


}
