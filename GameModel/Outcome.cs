using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Mementos;

namespace GameModel
{
    [CorrespondenceType]
    public class Outcome : CorrespondenceFact
    {
        public static Role<Game> RoleGame = new Role<Game>("game");
        public static Role<Player> RoleWinner = new Role<Player>("winner");

        private PredecessorObj<Game> _game;
        private PredecessorObj<Player> _winner;

        public Outcome(Game game, Player winner)
        {
            _game = new PredecessorObj<Game>(this, RoleGame, game);
            _winner = new PredecessorObj<Player>(this, RoleWinner, winner);
        }

        public Outcome(FactMemento memento)
        {
            _game = new PredecessorObj<Game>(this, RoleGame, memento);
            _winner = new PredecessorObj<Player>(this, RoleWinner, memento);
        }

        public Game Game
        {
            get { return _game.Fact; }
        }

        public Player Winner
        {
            get { return _winner.Fact; }
        }
    }
}
