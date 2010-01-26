using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Mementos;
using System;

namespace GameModel
{
    [CorrespondenceType]
    public class Game : CorrespondenceFact
    {
        public static Role<GameRequest> RoleGameRequest = new Role<GameRequest>("gameRequest", RoleRelationship.Pivot);
        private static Query QueryPlayers = new Query()
            .JoinSuccessors(Player.RoleGame);
        private static Query QueryMoves = new Query()
            .JoinSuccessors(Player.RoleGame)
            .JoinSuccessors(Move.RolePlayer);
        private static Query QueryOutcome = new Query()
            .JoinSuccessors(Outcome.RoleGame);
        public static Condition IsUnfinished = Condition.WhereIsEmpty(QueryOutcome);

        private PredecessorList<GameRequest> _gameRequest;

        private Result<Player> _players;
        private Result<Move> _moves;
        private Result<Outcome> _outcome;

        private Game()
        {
            _players = new Result<Player>(this, QueryPlayers);
            _moves = new Result<Move>(this, QueryMoves);
            _outcome = new Result<Outcome>(this, QueryOutcome);
        }

        public Game(GameRequest first, GameRequest second)
            : this()
        {
            _gameRequest = new PredecessorList<GameRequest>(this, RoleGameRequest, new List<GameRequest> { first, second });
        }

        public Game(FactMemento memento)
            : this()
        {
            _gameRequest = new PredecessorList<GameRequest>(this, RoleGameRequest, memento);
        }

        public GameRequest First
        {
            get { return _gameRequest.ElementAt(0); }
        }

        public GameRequest Second
        {
            get { return _gameRequest.ElementAt(1); }
        }

		public Player CreatePlayer(Person person)
		{
			return Community.AddFact(new Player(person, this));
		}

        public IEnumerable<Player> Players
        {
            get { return _players; }
        }

        public IEnumerable<Move> Moves
        {
            get { return _moves; }
        }

        public Outcome Outcome
        {
            get { return _outcome.FirstOrDefault(); }
        }

        public void DeclareDraw()
        {
            Community.AddFact(new Outcome(this, null));
        }
    }
}
