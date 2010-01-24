using System;

namespace Reversi.GameLogic
{
    public class Square
    {
        public const int NumberOfRows = 8;
        public const int NumberOfColumns = 8;

        private int _row;
        private int _column;

        public Square(int row, int column)
        {
            _row = row;
            _column = column;
        }

        public int Row
        {
            get { return _row; }
        }

        public int Column
        {
            get { return _column; }
        }

        public int Index
        {
            get { return _row * NumberOfColumns + _column; }
        }

        public bool IsOnBoard
        {
            get
            {
                return
                    0 <= _row && _row < NumberOfRows &&
                    0 <= _column && _column < NumberOfColumns;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;
            Square that = obj as Square;
            if (that == null)
                return false;
            return _row == that._row && _column == that._column;
        }

        public override int GetHashCode()
        {
            return _row * NumberOfColumns + _column;
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", _row, _column);
        }
    }
}
