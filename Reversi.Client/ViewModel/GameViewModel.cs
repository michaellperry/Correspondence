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
        private SynchronizationThread _synchronizationThread;

        private GameState _gameState;
        private Dependent _depGameState;

        public GameViewModel(SynchronizationThread synchronizationThread)
        {
            _synchronizationThread = synchronizationThread;

            _depGameState = new Dependent(UpdateGameState);

            _gameState = new GameState(null);
        }

        public ICommand JoinGame
        {
            get
            {
                return null;
            }
        }

        public ICommand Resign
        {
            get
            {
                return null;
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
        }
    }
}
