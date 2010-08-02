using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Reversi.GameLogic;
using Reversi.Model;
using UpdateControls.XAML;

namespace Reversi.Client.ViewModel
{
    public class GameViewModel
    {
        private Player _player;
        private GameState _gameState;

        public GameViewModel(Player player)
        {
            _player = player;
            _gameState = new GameState(player);
        }

        public ICommand Resign
        {
            get
            {
                return MakeCommand
                    .Do(() => _player.Game.DeclareWinner(
                        _player.Game.Players.FirstOrDefault(p => p != _player)));
            }
        }

        public PieceColor MyColor
        {
            get { return _gameState.MyColor; }
        }

        public bool MyTurn
        {
            get { return _gameState.MyTurn; }
        }

        public bool IWon
        {
            get { return _gameState.IWon; }
        }

        public bool ILost
        {
            get { return _gameState.ILost; }
        }

        public bool IDrew
        {
            get { return _gameState.IDrew; }
        }

        public IEnumerable<RowViewModel> Rows
        {
            get
            {
                for (int row = 0; row < Square.NumberOfRows; row++)
                    yield return new RowViewModel(_gameState, row);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;
            GameViewModel that = obj as GameViewModel;
            if (that == null)
                return false;
            return this._player == that._player;
        }

        public override int GetHashCode()
        {
            return _player.GetHashCode();
        }
    }
}
