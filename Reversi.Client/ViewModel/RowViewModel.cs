using System;
using System.Collections.Generic;
using Reversi.GameLogic;

namespace Reversi.Client.ViewModel
{
    public class RowViewModel
    {
        private GameState _gameState;
        private int _row;

        public RowViewModel(GameState gameState, int row)
        {
            _gameState = gameState;
            _row = row;
        }

        public IEnumerable<SquareViewModel> Squares
        {
            get
            {
                for (int column = 0; column < Square.NumberOfColumns; column++)
                {
                    yield return new SquareViewModel(_gameState, new Square(_row, column));
                }
            }
        }
    }
}
