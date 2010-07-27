using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Mementos;
using System;

/**
/ For use with http://graphviz.org/
digraph "Reversi.Model"
{
    rankdir=BT
    Service -> GameQueue [color="red"]
    GameRequest -> GameQueue
    GameRequest -> Person
    Game -> GameRequest [label="  *"]
    Player -> Person
    Player -> Game [color="red"]
    Move -> Player
    Outcome -> Game
    Outcome -> Player [label="  ?"]
}
**/

namespace Reversi.Model
{
    [CorrespondenceType]
    public partial class Person : CorrespondenceFact
    {
        // Roles

        // Queries
        public static Query QueryOutstandingGameRequests = new Query()
            .JoinSuccessors(GameRequest.RolePerson, Condition.WhereIsEmpty(GameRequest.QueryIsOutstanding)
            )
            ;
        public static Query QueryUnfinishedGames = new Query()
            .JoinSuccessors(GameRequest.RolePerson)
            .JoinSuccessors(Game.RoleGameRequests, Condition.WhereIsEmpty(Game.QueryIsUnfinished)
            )
            ;

        // Predicates

        // Predecessors

        // Fields

        // Results
        private Result<GameRequest> _outstandingGameRequests;
        private Result<Game> _unfinishedGames;

        // Business constructor
        public Person(
            )
        {
            InitializeResults();
        }

        // Hydration constructor
        public Person(FactMemento memento)
        {
            InitializeResults();
        }

        // Result initializer
        private void InitializeResults()
        {
            _outstandingGameRequests = new Result<GameRequest>(this, QueryOutstandingGameRequests);
            _unfinishedGames = new Result<Game>(this, QueryUnfinishedGames);
        }

        // Predecessor access

        // Field access

        // Query result access
        public IEnumerable<GameRequest> OutstandingGameRequests
        {
            get { return _outstandingGameRequests; }
        }
        public IEnumerable<Game> UnfinishedGames
        {
            get { return _unfinishedGames; }
        }
    }
    
    [CorrespondenceType]
    public partial class GameQueue : CorrespondenceFact
    {
        // Roles

        // Queries
        public static Query QueryOutstandingGameRequests = new Query()
            .JoinSuccessors(GameRequest.RoleGameQueue, Condition.WhereIsEmpty(GameRequest.QueryIsOutstanding)
            )
            ;

        // Predicates

        // Predecessors

        // Fields
        [CorrespondenceField]
        private string _identifier;

        // Results
        private Result<GameRequest> _outstandingGameRequests;

        // Business constructor
        public GameQueue(
            string identifier
            )
        {
            InitializeResults();
            _identifier = identifier;
        }

        // Hydration constructor
        public GameQueue(FactMemento memento)
        {
            InitializeResults();
        }

        // Result initializer
        private void InitializeResults()
        {
            _outstandingGameRequests = new Result<GameRequest>(this, QueryOutstandingGameRequests);
        }

        // Predecessor access

        // Field access
        public string Identifier
        {
            get { return _identifier; }
        }

        // Query result access
        public IEnumerable<GameRequest> OutstandingGameRequests
        {
            get { return _outstandingGameRequests; }
        }
    }
    
    [CorrespondenceType]
    public partial class Service : CorrespondenceFact
    {
        // Roles
        public static Role<GameQueue> RoleGameQueue = new Role<GameQueue>("gameQueue", RoleRelationship.Pivot);

        // Queries

        // Predicates

        // Predecessors
        private PredecessorObj<GameQueue> _gameQueue;

        // Fields

        // Results

        // Business constructor
        public Service(
            GameQueue gameQueue
            )
        {
            InitializeResults();
            _gameQueue = new PredecessorObj<GameQueue>(this, RoleGameQueue, gameQueue);
        }

        // Hydration constructor
        public Service(FactMemento memento)
        {
            InitializeResults();
            _gameQueue = new PredecessorObj<GameQueue>(this, RoleGameQueue, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
        }

        // Predecessor access
        public GameQueue GameQueue
        {
            get { return _gameQueue.Fact; }
        }

        // Field access

        // Query result access
    }
    
    [CorrespondenceType]
    public partial class GameRequest : CorrespondenceFact
    {
        // Roles
        public static Role<GameQueue> RoleGameQueue = new Role<GameQueue>("gameQueue");
        public static Role<Person> RolePerson = new Role<Person>("person");

        // Queries
        public static Query QueryGames = new Query()
            .JoinSuccessors(Game.RoleGameRequests)
            ;
        public static Query QueryIsOutstanding = new Query()
            .JoinSuccessors(Game.RoleGameRequests)
            ;

        // Predicates
        public static Condition IsOutstanding = Condition.WhereIsEmpty(QueryIsOutstanding);

        // Predecessors
        private PredecessorObj<GameQueue> _gameQueue;
        private PredecessorObj<Person> _person;

        // Unique
        [CorrespondenceField]
        private Guid _unique;

        // Fields

        // Results
        private Result<Game> _games;

        // Business constructor
        public GameRequest(
            GameQueue gameQueue
            ,Person person
            )
        {
            _unique = Guid.NewGuid();
            InitializeResults();
            _gameQueue = new PredecessorObj<GameQueue>(this, RoleGameQueue, gameQueue);
            _person = new PredecessorObj<Person>(this, RolePerson, person);
        }

        // Hydration constructor
        public GameRequest(FactMemento memento)
        {
            InitializeResults();
            _gameQueue = new PredecessorObj<GameQueue>(this, RoleGameQueue, memento);
            _person = new PredecessorObj<Person>(this, RolePerson, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
            _games = new Result<Game>(this, QueryGames);
        }

        // Predecessor access
        public GameQueue GameQueue
        {
            get { return _gameQueue.Fact; }
        }
        public Person Person
        {
            get { return _person.Fact; }
        }

        // Field access

        // Query result access
        public IEnumerable<Game> Games
        {
            get { return _games; }
        }
    }
    
    [CorrespondenceType]
    public partial class Game : CorrespondenceFact
    {
        // Roles
        public static Role<GameRequest> RoleGameRequests = new Role<GameRequest>("gameRequests");

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
        public static Query QueryIsUnfinished = new Query()
            .JoinSuccessors(Outcome.RoleGame)
            ;

        // Predicates
        public static Condition IsUnfinished = Condition.WhereIsEmpty(QueryIsUnfinished);

        // Predecessors
        private PredecessorList<GameRequest> _gameRequests;

        // Fields

        // Results
        private Result<Player> _players;
        private Result<Move> _moves;
        private Result<Outcome> _outcomes;

        // Business constructor
        public Game(
            IEnumerable<GameRequest> gameRequests
            )
        {
            InitializeResults();
            _gameRequests = new PredecessorList<GameRequest>(this, RoleGameRequests, gameRequests);
        }

        // Hydration constructor
        public Game(FactMemento memento)
        {
            InitializeResults();
            _gameRequests = new PredecessorList<GameRequest>(this, RoleGameRequests, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
            _players = new Result<Player>(this, QueryPlayers);
            _moves = new Result<Move>(this, QueryMoves);
            _outcomes = new Result<Outcome>(this, QueryOutcomes);
        }

        // Predecessor access
        public IEnumerable<GameRequest> GameRequests
        {
            get { return _gameRequests; }
        }
     
        // Field access

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
    
    [CorrespondenceType]
    public partial class Player : CorrespondenceFact
    {
        // Roles
        public static Role<Person> RolePerson = new Role<Person>("person");
        public static Role<Game> RoleGame = new Role<Game>("game", RoleRelationship.Pivot);

        // Queries
        public static Query QueryMoves = new Query()
            .JoinSuccessors(Move.RolePlayer)
            ;

        // Predicates

        // Predecessors
        private PredecessorObj<Person> _person;
        private PredecessorObj<Game> _game;

        // Fields

        // Results
        private Result<Move> _moves;

        // Business constructor
        public Player(
            Person person
            ,Game game
            )
        {
            InitializeResults();
            _person = new PredecessorObj<Person>(this, RolePerson, person);
            _game = new PredecessorObj<Game>(this, RoleGame, game);
        }

        // Hydration constructor
        public Player(FactMemento memento)
        {
            InitializeResults();
            _person = new PredecessorObj<Person>(this, RolePerson, memento);
            _game = new PredecessorObj<Game>(this, RoleGame, memento);
        }

        // Result initializer
        private void InitializeResults()
        {
            _moves = new Result<Move>(this, QueryMoves);
        }

        // Predecessor access
        public Person Person
        {
            get { return _person.Fact; }
        }
        public Game Game
        {
            get { return _game.Fact; }
        }

        // Field access

        // Query result access
        public IEnumerable<Move> Moves
        {
            get { return _moves; }
        }
	}
    
    [CorrespondenceType]
    public partial class Move : CorrespondenceFact
    {
        // Roles
        public static Role<Player> RolePlayer = new Role<Player>("player");

        // Queries

        // Predicates

        // Predecessors
        private PredecessorObj<Player> _player;

        // Fields
        [CorrespondenceField]
        private int _index;
        [CorrespondenceField]
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
        public Move(FactMemento memento)
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
    
    [CorrespondenceType]
    public partial class Outcome : CorrespondenceFact
    {
        // Roles
        public static Role<Game> RoleGame = new Role<Game>("game");
        public static Role<Player> RoleWinner = new Role<Player>("winner");

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
        public Outcome(FactMemento memento)
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
    
}
