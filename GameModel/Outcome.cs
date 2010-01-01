using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Mementos;

namespace GameModel
{
    [CorrespondenceType]
    public class Outcome : CorrespondenceFact
    {
        public static Role<Game> RoleGame = new Role<Game>("game");
        public static Role<GameRequest> RoleWinner = new Role<GameRequest>("winner");

        private PredecessorObj<Game> _game;
        private PredecessorObj<GameRequest> _winner;

        public Outcome(Game game, GameRequest winner)
        {
            _game = new PredecessorObj<Game>(this, RoleGame, game);
            _winner = new PredecessorObj<GameRequest>(this, RoleWinner, winner);
        }

        public Outcome(FactMemento memento)
        {
            _game = new PredecessorObj<Game>(this, RoleGame, memento);
            _winner = new PredecessorObj<GameRequest>(this, RoleWinner, memento);
        }

        public Game Game
        {
            get { return _game.Fact; }
        }

        public GameRequest Winner
        {
            get { return _winner.Fact; }
        }
    }
}
