using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GameModel;
using Reversi.GameLogic;
using UpdateControls.XAML;

namespace Reversi.Client.ViewModel
{
    public class GameViewModel
    {
        private Person _person;
        private GameQueue _gameQueue;

        public GameViewModel(Person person, GameQueue gameQueue)
        {
            _person = person;
            _gameQueue = gameQueue;
        }

        public ICommand JoinGame
        {
            get
            {
                return MakeCommand
                    .When(() =>
                        !_person.UnfinishedGames.Any() &&
                        !_person.OutstandingGameRequests.Any(
                            gameRequest => gameRequest.GameQueue == _gameQueue))
                    .Do(() => _gameQueue.CreateGameRequest(_person));
            }
        }

        public IEnumerable<RowViewModel> Rows
        {
            get
            {
                Game game = _person.UnfinishedGames.FirstOrDefault();
                Player player = game != null ? game.CreatePlayer(_person) : null;
                GameState gameState = new GameState(player);
                for (int row = 0; row < Square.NumberOfRows; row++)
                    yield return new RowViewModel(gameState, row);
            }
        }
    }
}
