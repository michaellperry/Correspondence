using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;
using System;
using System.IO;

/**
/ For use with http://graphviz.org/
digraph "UpdateControls.Correspondence.UnitTest.Model"
{
    rankdir=BT
    LogOn -> User
    LogOn -> Machine
    LogOff -> LogOn
    GameName -> Game
    GameName -> GameName [label="  *"]
    Player -> User [color="red"]
    Player -> Game [color="red"]
    Move -> Player
    Outcome -> Game
    Outcome -> Player [label="  ?"]
}
**/

namespace UpdateControls.Correspondence.UnitTest.Model
{
    public partial class Machine : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				Machine newFact = new Machine(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._unique = (Guid)_fieldSerializerByType[typeof(Guid)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Machine fact = (Machine)obj;
				_fieldSerializerByType[typeof(Guid)].WriteData(output, fact._unique);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.Machine", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles

        // Queries
        public static Query QueryActiveLogOns = new Query()
            .JoinSuccessors(LogOn.RoleMachine, Condition.WhereIsEmpty(LogOn.QueryIsActive)
            )
            ;

        // Predicates

        // Predecessors

        // Unique
        private Guid _unique;

        // Fields

        // Results
        private Result<LogOn> _activeLogOns;

        // Business constructor
        public Machine(
            )
        {
            _unique = Guid.NewGuid();
            InitializeResults();
        }

        // Hydration constructor
        private Machine(FactMemento memento)
        {
            InitializeResults();
        }

        // Result initializer
        private void InitializeResults()
        {
            _activeLogOns = new Result<LogOn>(this, QueryActiveLogOns);
        }

        // Predecessor access

        // Field access
		public Guid Unique { get { return _unique; } }


        // Query result access
        public IEnumerable<LogOn> ActiveLogOns
        {
            get { return _activeLogOns; }
        }
    }
    
    public partial class User : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				User newFact = new User(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._userName = (string)_fieldSerializerByType[typeof(string)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				User fact = (User)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._userName);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.User", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles

        // Queries
        public static Query QueryActivePlayers = new Query()
            .JoinSuccessors(Player.RoleUser, Condition.WhereIsEmpty(Player.QueryIsActive)
            )
            ;
        public static Query QueryFinishedPlayers = new Query()
            .JoinSuccessors(Player.RoleUser, Condition.WhereIsNotEmpty(Player.QueryIsNotActive)
            )
            ;

        // Predicates

        // Predecessors

        // Fields
        private string _userName;

        // Results
        private Result<Player> _activePlayers;
        private Result<Player> _finishedPlayers;

        // Business constructor
        public User(
            string userName
            )
        {
            InitializeResults();
            _userName = userName;
        }

        // Hydration constructor
        private User(FactMemento memento)
        {
            InitializeResults();
        }

        // Result initializer
        private void InitializeResults()
        {
            _activePlayers = new Result<Player>(this, QueryActivePlayers);
            _finishedPlayers = new Result<Player>(this, QueryFinishedPlayers);
        }

        // Predecessor access

        // Field access
        public string UserName
        {
            get { return _userName; }
        }

        // Query result access
        public IEnumerable<Player> ActivePlayers
        {
            get { return _activePlayers; }
        }
        public IEnumerable<Player> FinishedPlayers
        {
            get { return _finishedPlayers; }
        }
    }
    
    public partial class LogOn : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				LogOn newFact = new LogOn(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._unique = (Guid)_fieldSerializerByType[typeof(Guid)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				LogOn fact = (LogOn)obj;
				_fieldSerializerByType[typeof(Guid)].WriteData(output, fact._unique);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.LogOn", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles
        public static Role RoleUser = new Role(new RoleMemento(
			_correspondenceFactType,
			"user",
			new CorrespondenceFactType("UpdateControls.Correspondence.UnitTest.Model.User", 1),
			false));
        public static Role RoleMachine = new Role(new RoleMemento(
			_correspondenceFactType,
			"machine",
			new CorrespondenceFactType("UpdateControls.Correspondence.UnitTest.Model.Machine", 1),
			false));

        // Queries
        public static Query QueryIsActive = new Query()
            .JoinSuccessors(LogOff.RoleLogOn)
            ;

        // Predicates
        public static Condition IsActive = Condition.WhereIsEmpty(QueryIsActive);

        // Predecessors
        private PredecessorObj<User> _user;
        private PredecessorObj<Machine> _machine;

        // Unique
        private Guid _unique;

        // Fields

        // Results

        // Business constructor
        public LogOn(
            User user
            ,Machine machine
            )
        {
            _unique = Guid.NewGuid();
            InitializeResults();
            _user = new PredecessorObj<User>(this, RoleUser, user);
            _machine = new PredecessorObj<Machine>(this, RoleMachine, machine);
        }

        // Hydration constructor
        private LogOn(FactMemento memento)
        {
            InitializeResults();
            _user = new PredecessorObj<User>(this, RoleUser, memento);
            _machine = new PredecessorObj<Machine>(this, RoleMachine, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public User User
        {
            get { return _user.Fact; }
        }
        public Machine Machine
        {
            get { return _machine.Fact; }
        }

        // Field access
		public Guid Unique { get { return _unique; } }


        // Query result access
    }
    
    public partial class LogOff : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				LogOff newFact = new LogOff(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				LogOff fact = (LogOff)obj;
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.LogOff", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles
        public static Role RoleLogOn = new Role(new RoleMemento(
			_correspondenceFactType,
			"logOn",
			new CorrespondenceFactType("UpdateControls.Correspondence.UnitTest.Model.LogOn", 1),
			false));

        // Queries

        // Predicates

        // Predecessors
        private PredecessorObj<LogOn> _logOn;

        // Fields

        // Results

        // Business constructor
        public LogOff(
            LogOn logOn
            )
        {
            InitializeResults();
            _logOn = new PredecessorObj<LogOn>(this, RoleLogOn, logOn);
        }

        // Hydration constructor
        private LogOff(FactMemento memento)
        {
            InitializeResults();
            _logOn = new PredecessorObj<LogOn>(this, RoleLogOn, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public LogOn LogOn
        {
            get { return _logOn.Fact; }
        }

        // Field access

        // Query result access
    }
    
    public partial class Game : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				Game newFact = new Game(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._unique = (Guid)_fieldSerializerByType[typeof(Guid)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Game fact = (Game)obj;
				_fieldSerializerByType[typeof(Guid)].WriteData(output, fact._unique);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.Game", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles

        // Queries
        public static Query QueryPlayers = new Query()
            .JoinSuccessors(Player.RoleGame)
            ;
        public static Query QueryMoves = new Query()
            .JoinSuccessors(Player.RoleGame)
            .JoinSuccessors(Move.RolePlayer)
            ;
        public static Query QueryOutcomes = new Query()
            .JoinSuccessors(Outcome.RoleGame)
            ;

        // Predicates

        // Predecessors

        // Unique
        private Guid _unique;

        // Fields

        // Results
        private Result<Player> _players;
        private Result<Move> _moves;
        private Result<Outcome> _outcomes;

        // Business constructor
        public Game(
            )
        {
            _unique = Guid.NewGuid();
            InitializeResults();
        }

        // Hydration constructor
        private Game(FactMemento memento)
        {
            InitializeResults();
        }

        // Result initializer
        private void InitializeResults()
        {
            _players = new Result<Player>(this, QueryPlayers);
            _moves = new Result<Move>(this, QueryMoves);
            _outcomes = new Result<Outcome>(this, QueryOutcomes);
        }

        // Predecessor access

        // Field access
		public Guid Unique { get { return _unique; } }


        // Query result access
        public IEnumerable<Player> Players
        {
            get { return _players; }
        }
        public IEnumerable<Move> Moves
        {
            get { return _moves; }
        }
        public IEnumerable<Outcome> Outcomes
        {
            get { return _outcomes; }
        }
    }
    
    public partial class GameName : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				GameName newFact = new GameName(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._name = (string)_fieldSerializerByType[typeof(string)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				GameName fact = (GameName)obj;
				_fieldSerializerByType[typeof(string)].WriteData(output, fact._name);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.GameName", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles
        public static Role RoleGame = new Role(new RoleMemento(
			_correspondenceFactType,
			"game",
			new CorrespondenceFactType("UpdateControls.Correspondence.UnitTest.Model.Game", 1),
			false));
        public static Role RolePrior = new Role(new RoleMemento(
			_correspondenceFactType,
			"prior",
			new CorrespondenceFactType("UpdateControls.Correspondence.UnitTest.Model.GameName", 1),
			false));

        // Queries

        // Predicates

        // Predecessors
        private PredecessorObj<Game> _game;
        private PredecessorList<GameName> _prior;

        // Fields
        private string _name;

        // Results

        // Business constructor
        public GameName(
            Game game
            ,IEnumerable<GameName> prior
            ,string name
            )
        {
            InitializeResults();
            _game = new PredecessorObj<Game>(this, RoleGame, game);
            _prior = new PredecessorList<GameName>(this, RolePrior, prior);
            _name = name;
        }

        // Hydration constructor
        private GameName(FactMemento memento)
        {
            InitializeResults();
            _game = new PredecessorObj<Game>(this, RoleGame, memento);
            _prior = new PredecessorList<GameName>(this, RolePrior, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Game Game
        {
            get { return _game.Fact; }
        }
        public IEnumerable<GameName> Prior
        {
            get { return _prior; }
        }
     
        // Field access
        public string Name
        {
            get { return _name; }
        }

        // Query result access
    }
    
    public partial class Player : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				Player newFact = new Player(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._index = (int)_fieldSerializerByType[typeof(int)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Player fact = (Player)obj;
				_fieldSerializerByType[typeof(int)].WriteData(output, fact._index);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.Player", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles
        public static Role RoleUser = new Role(new RoleMemento(
			_correspondenceFactType,
			"user",
			new CorrespondenceFactType("UpdateControls.Correspondence.UnitTest.Model.User", 1),
			true));
        public static Role RoleGame = new Role(new RoleMemento(
			_correspondenceFactType,
			"game",
			new CorrespondenceFactType("UpdateControls.Correspondence.UnitTest.Model.Game", 1),
			true));

        // Queries
        public static Query QueryMoves = new Query()
            .JoinSuccessors(Move.RolePlayer)
            ;
        public static Query QueryIsActive = new Query()
            .JoinPredecessors(Player.RoleGame)
            .JoinSuccessors(Outcome.RoleGame)
            ;
        public static Query QueryIsNotActive = new Query()
            .JoinPredecessors(Player.RoleGame)
            .JoinSuccessors(Outcome.RoleGame)
            ;

        // Predicates
        public static Condition IsActive = Condition.WhereIsEmpty(QueryIsActive);
        public static Condition IsNotActive = Condition.WhereIsNotEmpty(QueryIsNotActive);

        // Predecessors
        private PredecessorObj<User> _user;
        private PredecessorObj<Game> _game;

        // Fields
        private int _index;

        // Results
        private Result<Move> _moves;

        // Business constructor
        public Player(
            User user
            ,Game game
            ,int index
            )
        {
            InitializeResults();
            _user = new PredecessorObj<User>(this, RoleUser, user);
            _game = new PredecessorObj<Game>(this, RoleGame, game);
            _index = index;
        }

        // Hydration constructor
        private Player(FactMemento memento)
        {
            InitializeResults();
            _user = new PredecessorObj<User>(this, RoleUser, memento);
            _game = new PredecessorObj<Game>(this, RoleGame, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
            _moves = new Result<Move>(this, QueryMoves);
        }

        // Predecessor access
        public User User
        {
            get { return _user.Fact; }
        }
        public Game Game
        {
            get { return _game.Fact; }
        }

        // Field access
        public int Index
        {
            get { return _index; }
        }

        // Query result access
        public IEnumerable<Move> Moves
        {
            get { return _moves; }
        }
    }
    
    public partial class Move : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				Move newFact = new Move(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
						newFact._index = (int)_fieldSerializerByType[typeof(int)].ReadData(output);
						newFact._square = (int)_fieldSerializerByType[typeof(int)].ReadData(output);
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Move fact = (Move)obj;
				_fieldSerializerByType[typeof(int)].WriteData(output, fact._index);
				_fieldSerializerByType[typeof(int)].WriteData(output, fact._square);
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.Move", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles
        public static Role RolePlayer = new Role(new RoleMemento(
			_correspondenceFactType,
			"player",
			new CorrespondenceFactType("UpdateControls.Correspondence.UnitTest.Model.Player", 1),
			false));

        // Queries

        // Predicates

        // Predecessors
        private PredecessorObj<Player> _player;

        // Fields
        private int _index;
        private int _square;

        // Results

        // Business constructor
        public Move(
            Player player
            ,int index
            ,int square
            )
        {
            InitializeResults();
            _player = new PredecessorObj<Player>(this, RolePlayer, player);
            _index = index;
            _square = square;
        }

        // Hydration constructor
        private Move(FactMemento memento)
        {
            InitializeResults();
            _player = new PredecessorObj<Player>(this, RolePlayer, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Player Player
        {
            get { return _player.Fact; }
        }

        // Field access
        public int Index
        {
            get { return _index; }
        }
        public int Square
        {
            get { return _square; }
        }

        // Query result access
    }
    
    public partial class Outcome : CorrespondenceFact
    {
		// Factory
		internal class CorrespondenceFactFactory : ICorrespondenceFactFactory
		{
			private IDictionary<Type, IFieldSerializer> _fieldSerializerByType;

			public CorrespondenceFactFactory(IDictionary<Type, IFieldSerializer> fieldSerializerByType)
			{
				_fieldSerializerByType = fieldSerializerByType;
			}

			public CorrespondenceFact CreateFact(FactMemento memento)
			{
				Outcome newFact = new Outcome(memento);

				// Create a memory stream from the memento data.
				using (MemoryStream data = new MemoryStream(memento.Data))
				{
					using (BinaryReader output = new BinaryReader(data))
					{
					}
				}

				return newFact;
			}

			public void WriteFactData(CorrespondenceFact obj, BinaryWriter output)
			{
				Outcome fact = (Outcome)obj;
			}
		}

		// Type
		internal static CorrespondenceFactType _correspondenceFactType = new CorrespondenceFactType(
			"UpdateControls.Correspondence.UnitTest.Model.Outcome", 1);

		protected override CorrespondenceFactType GetCorrespondenceFactType()
		{
			return _correspondenceFactType;
		}

        // Roles
        public static Role RoleGame = new Role(new RoleMemento(
			_correspondenceFactType,
			"game",
			new CorrespondenceFactType("UpdateControls.Correspondence.UnitTest.Model.Game", 1),
			false));
        public static Role RoleWinner = new Role(new RoleMemento(
			_correspondenceFactType,
			"winner",
			new CorrespondenceFactType("UpdateControls.Correspondence.UnitTest.Model.Player", 1),
			false));

        // Queries

        // Predicates

        // Predecessors
        private PredecessorObj<Game> _game;
        private PredecessorOpt<Player> _winner;

        // Fields

        // Results

        // Business constructor
        public Outcome(
            Game game
            ,Player winner
            )
        {
            InitializeResults();
            _game = new PredecessorObj<Game>(this, RoleGame, game);
            _winner = new PredecessorOpt<Player>(this, RoleWinner, winner);
        }

        // Hydration constructor
        private Outcome(FactMemento memento)
        {
            InitializeResults();
            _game = new PredecessorObj<Game>(this, RoleGame, memento);
            _winner = new PredecessorOpt<Player>(this, RoleWinner, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public Game Game
        {
            get { return _game.Fact; }
        }
        public Player Winner
        {
            get { return _winner.Fact; }
        }

        // Field access

        // Query result access
    }
    

	public class CorrespondenceModule : ICorrespondenceModule
	{
		public void RegisterAllFactTypes(Community community, IDictionary<Type, IFieldSerializer> fieldSerializerByType)
		{
			community.AddType(
				Machine._correspondenceFactType,
				new Machine.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Machine._correspondenceFactType }));
			community.AddQuery(
				Machine._correspondenceFactType,
				Machine.QueryActiveLogOns.QueryDefinition);
			community.AddType(
				User._correspondenceFactType,
				new User.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { User._correspondenceFactType }));
			community.AddQuery(
				User._correspondenceFactType,
				User.QueryActivePlayers.QueryDefinition);
			community.AddQuery(
				User._correspondenceFactType,
				User.QueryFinishedPlayers.QueryDefinition);
			community.AddType(
				LogOn._correspondenceFactType,
				new LogOn.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { LogOn._correspondenceFactType }));
			community.AddQuery(
				LogOn._correspondenceFactType,
				LogOn.QueryIsActive.QueryDefinition);
			community.AddType(
				LogOff._correspondenceFactType,
				new LogOff.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { LogOff._correspondenceFactType }));
			community.AddType(
				Game._correspondenceFactType,
				new Game.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Game._correspondenceFactType }));
			community.AddQuery(
				Game._correspondenceFactType,
				Game.QueryPlayers.QueryDefinition);
			community.AddQuery(
				Game._correspondenceFactType,
				Game.QueryMoves.QueryDefinition);
			community.AddQuery(
				Game._correspondenceFactType,
				Game.QueryOutcomes.QueryDefinition);
			community.AddType(
				GameName._correspondenceFactType,
				new GameName.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { GameName._correspondenceFactType }));
			community.AddType(
				Player._correspondenceFactType,
				new Player.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Player._correspondenceFactType }));
			community.AddQuery(
				Player._correspondenceFactType,
				Player.QueryMoves.QueryDefinition);
			community.AddQuery(
				Player._correspondenceFactType,
				Player.QueryIsActive.QueryDefinition);
			community.AddQuery(
				Player._correspondenceFactType,
				Player.QueryIsNotActive.QueryDefinition);
			community.AddType(
				Move._correspondenceFactType,
				new Move.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Move._correspondenceFactType }));
			community.AddType(
				Outcome._correspondenceFactType,
				new Outcome.CorrespondenceFactFactory(fieldSerializerByType),
				new FactMetadata(new List<CorrespondenceFactType> { Outcome._correspondenceFactType }));
		}
	}
}
