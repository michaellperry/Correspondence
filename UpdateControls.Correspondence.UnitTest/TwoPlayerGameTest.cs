using System.Linq;
using GameModel;
using GameService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Memory;

namespace UpdateControls.Correspondence.UnitTest
{
	/// <summary>
	/// Summary description for TwoPlayerGameTest
	/// </summary>
	[TestClass]
	public class TwoPlayerGameTest
	{
		public TestContext TestContext { get; set; }

		private Community _community;
		private GameQueue _gameQueue;
		private Client _alice;
		private Client _bob;
		private GameQueueService _service;

		[TestInitialize]
		public void Initialize()
		{
			_community = new Community(new MemoryStorageStrategy())
				.RegisterAssembly(typeof(GameQueue));
			_alice = new Client(_community);
			_bob = new Client(_community);
			_gameQueue = _community.AddFact(new GameQueue("mygamequeue"));
			_service = new GameQueueService(_gameQueue);

			_alice.CreateGameRequest();
			_bob.CreateGameRequest();
			_service.Process(_service.Queue.ToList());
		}

		[TestMethod]
		public void BobMakesAMove()
		{
			_bob.MakeMove(0);

			_bob.AssertHasMove(0, _bob.Person.Unique);
		}

		[TestMethod]
		public void AliceSeesBobsMove()
		{
			_bob.MakeMove(0);

			_alice.AssertHasMove(0, _bob.Person.Unique);
		}

		[TestMethod]
		public void AliceMakesAMoveInResponse()
		{
			_bob.MakeMove(0);
			_alice.MakeMove(1);

			_alice.AssertHasMove(1, _alice.Person.Unique);
		}

		[TestMethod]
		public void BobSeesAlicesResponse()
		{
			_bob.MakeMove(0);
			_alice.MakeMove(1);

			_bob.AssertHasMove(1, _alice.Person.Unique);
		}
	}
}
