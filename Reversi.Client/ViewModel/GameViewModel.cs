using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Reversi.Model;
using Reversi.GameLogic;
using UpdateControls.XAML;
using UpdateControls;
using System;
using Reversi.Client.Synchronization;

namespace Reversi.Client.ViewModel
{
    public class GameViewModel
    {
        private Person _person;
        private GameQueue _gameQueue;
        private SynchronizationThread _synchronizationThread;

        private GameState _gameState;
        private Dependent _depGameState;

        public GameViewModel(Person person, GameQueue gameQueue, SynchronizationThread synchronizationThread)
        {
            _person = person;
            _gameQueue = gameQueue;
            _synchronizationThread = synchronizationThread;

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
                    .Do(delegate
                        {
                            Game game = _person.UnfinishedGames.First();
                            GameRequest otherRequest = game.GameRequests.FirstOrDefault(
                                request => request.Person != _person);
                            game.DeclareWinner(otherRequest);
                        });
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

        public string LastError
        {
            get
            {
                return _synchronizationThread.LastError;
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
