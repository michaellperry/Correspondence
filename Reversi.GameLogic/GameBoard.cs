using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reversi.GameLogic
{
    public class GameBoard
    {
        private const int NumberOfSquares = Square.NumberOfRows * Square.NumberOfColumns;

        private PieceColor[] _pieces = new PieceColor[NumberOfSquares];
        private PieceColor _toMove;
        private List<Square> _legalMoves = new List<Square>();

        private GameBoard()
        {
        }

        public static GameBoard OpeningPosition
        {
            get
            {
                GameBoard position = new GameBoard();
                for (int i = 0; i < NumberOfSquares; i++)
                    position._pieces[i] = PieceColor.Empty;
                position._pieces[new Square(3, 3).Index] = PieceColor.White;
                position._pieces[new Square(3, 4).Index] = PieceColor.Black;
                position._pieces[new Square(4, 3).Index] = PieceColor.Black;
                position._pieces[new Square(4, 4).Index] = PieceColor.White;
                position._toMove = PieceColor.Black;
                position._legalMoves = position.EvaluateLegalMoves().ToList();
                return position;
            }
        }

        public PieceColor PieceAt(Square square)
        {
            return _pieces[square.Index];
        }

        public PieceColor ToMove
        {
            get { return _toMove; }
        }

        public IEnumerable<Square> LegalMoves
        {
            get { return _legalMoves; }
        }

        private IEnumerable<Square> EvaluateLegalMoves()
        {
            for (int row = 0; row < Square.NumberOfRows; row++)
            {
                for (int column = 0; column < Square.NumberOfColumns; column++)
                {
                    Square square = new Square(row, column);
                    if (IsLegalMove(square))
                        yield return square;
                }
            }
        }

        private bool IsLegalMove(Square square)
        {
            if (_pieces[square.Index] == PieceColor.Empty)
            {
                for (int direction = 0; direction < 9; direction++)
                {
                    int deltaRow = (direction / 3) - 1;
                    int deltaColumn = (direction % 3) - 1;
                    if ((deltaRow != 0 || deltaColumn != 0))
                        if (IsFlank(square, deltaRow, deltaColumn))
                            return true;
                }
            }
            return false;
        }

        private bool IsFlank(Square square, int deltaRow, int deltaColumn)
        {
            Square step = NextSquare(square, deltaRow, deltaColumn);
            PieceColor opposite = _toMove == PieceColor.Black ? PieceColor.White : PieceColor.Black;
            if (!step.IsOnBoard || _pieces[step.Index] != opposite)
                return false;

            step = NextSquare(step, deltaRow, deltaColumn);
            while (step.IsOnBoard)
            {
                if (_pieces[step.Index] == _toMove)
                    return true;
                if (_pieces[step.Index] == PieceColor.Empty)
                    return false;
                step = NextSquare(step, deltaRow, deltaColumn);
            }
            return false;
        }

        private static Square NextSquare(Square from, int deltaRow, int deltaColumn)
        {
            return new Square(from.Row + deltaRow, from.Column + deltaColumn);
        }
    }
}
