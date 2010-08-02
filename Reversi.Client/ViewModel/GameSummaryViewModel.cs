using Reversi.GameLogic;
using System;
using Reversi.Model;
using System.Linq;

namespace Reversi.Client.ViewModel
{
    public class GameSummaryViewModel
    {
        private Player _player;

        private GameState _gameState;

        public GameSummaryViewModel(Player player)
        {
            _player = player;
            _gameState = new GameState(player);
        }

        public Player Player
        {
            get { return _player; }
        }

        public bool MyTurn
        {
            get { return _gameState.MyTurn; }
        }

        public string OpponentName
        {
            get
            {
                Player opponent = _player.Game.Players.FirstOrDefault(p => p != _player);
                return opponent == null ? null : opponent.User.UserName;
            }
        }

        public PieceColor MyColor
        {
            get
            {
                return _player.Index == 0 ? PieceColor.Black : PieceColor.White;
            }
        }

        public int BlackCount
        {
            get { return _gameState.BlackCount; }
        }

        public int WhiteCount
        {
            get { return _gameState.WhiteCount; }
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;
            GameSummaryViewModel that = obj as GameSummaryViewModel;
            if (that == null)
                return false;
            return that._player == this._player;
        }

        public override int GetHashCode()
        {
            return _player.GetHashCode();
        }
    }
}
