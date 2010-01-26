using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GameModel;
using Reversi.GameLogic;
using UpdateControls.XAML;
using UpdateControls;

namespace Reversi.Client.ViewModel
{
    public class GameViewModel
    {
        private Person _person;
        private GameQueue _gameQueue;

        private GameState _gameState;
        private Dependent _depGameState;

        public GameViewModel(Person person, GameQueue gameQueue)
        {
            _person = person;
            _gameQueue = gameQueue;
            _depGameState = new Dependent(UpdateGameState);

            _gameState = new GameState(null);
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

        public ICommand Resign
        {
            get
            {
                return MakeCommand
                    .When(() => _person.UnfinishedGames.Any())
                    .Do(() => _person.UnfinishedGames.First()
                        .Players.First(player => player.Person != _person).DeclareWinner());
            }
        }

        public PieceColor MyColor
        {
            get
            {
                _depGameState.OnGet();
                return _gameState.MyColor;
            }
        }

        public bool MyTurn
        {
            get
            {
                _depGameState.OnGet();
                return _gameState.MyTurn;
            }
        }

        public bool IWon
        {
            get
            {
                _depGameState.OnGet();
                return _gameState.IWon;
            }
        }

        public bool ILost
        {
            get
            {
                _depGameState.OnGet();
                return _gameState.ILost;
            }
        }

        public bool IDrew
        {
            get
            {
                _depGameState.OnGet();
                return _gameState.IDrew;
            }
        }

        public IEnumerable<RowViewModel> Rows
        {
            get
            {
                _depGameState.OnGet();
                for (int row = 0; row < Square.NumberOfRows; row++)
                    yield return new RowViewModel(_gameState, row);
            }
        }

        private void UpdateGameState()
        {
            Game game = _person.UnfinishedGames.FirstOrDefault();
            if (game != null)
                _gameState = new GameState(game.CreatePlayer(_person));
        }
    }
}
