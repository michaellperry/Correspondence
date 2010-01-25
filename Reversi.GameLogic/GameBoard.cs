using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;

namespace Reversi.GameLogic
{
    public class GameBoard
    {
        private const int NumberOfSquares = Square.NumberOfRows * Square.NumberOfColumns;

        private int _moveIndex;
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
                position.UpdateLegalMoves();
                return position;
            }
        }

        public int MoveIndex
        {
            get { return _moveIndex; }
        }

        public PieceColor PieceAt(Square square)
        {
            return _pieces[square.Index];
        }

        public PieceColor ToMove
        {
            get { return _toMove; }
        }

        public int BlackCount
        {
            get { return _pieces.Count(piece => piece == PieceColor.Black); }
        }

        public int WhiteCount
        {
            get { return _pieces.Count(piece => piece == PieceColor.White); }
        }

        public IEnumerable<Square> LegalMoves
        {
            get { return _legalMoves; }
        }

        public GameBoard AfterMove(Square square)
        {
            Debug.Assert(_legalMoves.Contains(square));

            GameBoard nextPosition = new GameBoard();
            nextPosition._moveIndex = _moveIndex + 1;

            nextPosition._pieces = new PieceColor[NumberOfSquares];
            Array.Copy(_pieces, nextPosition._pieces, NumberOfSquares);
            List<Square> capturedPieces = FlankedSquares(square).ToList();
            foreach (Square capturedPiece in capturedPieces)
                nextPosition._pieces[capturedPiece.Index] = _toMove;
            nextPosition._pieces[square.Index] = _toMove;

            nextPosition._toMove = GetOpposite();
            nextPosition.UpdateLegalMoves();

            return nextPosition;
        }

        private void UpdateLegalMoves()
        {
            _legalMoves = EvaluateLegalMoves().ToList();
            if (!_legalMoves.Any())
            {
                _toMove = GetOpposite();
                _legalMoves = EvaluateLegalMoves().ToList();
                if (!_legalMoves.Any())
                    _toMove = PieceColor.Empty;
            }
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
            return FlankedSquares(square).Any();
        }

        private IEnumerable<Square> FlankedSquares(Square center)
        {
            if (_pieces[center.Index] == PieceColor.Empty)
            {
                for (int direction = 0; direction < 9; direction++)
                {
                    int deltaRow = (direction / 3) - 1;
                    int deltaColumn = (direction % 3) - 1;
                    if (deltaRow != 0 || deltaColumn != 0)
                        foreach (Square square in FlankedSquaresInDirection(center, deltaRow, deltaColumn))
                            yield return square;
                }
            }
        }

        private IEnumerable<Square> FlankedSquaresInDirection(Square square, int deltaRow, int deltaColumn)
        {
            Square step = NextSquare(square, deltaRow, deltaColumn);
            PieceColor opposite = GetOpposite();

            if (step.IsOnBoard && _pieces[step.Index] == opposite)
            {
                List<Square> flankedPieces = new List<Square>();
                flankedPieces.Add(step);
                step = NextSquare(step, deltaRow, deltaColumn);

                while (step.IsOnBoard)
                {
                    if (_pieces[step.Index] == _toMove)
                        return flankedPieces;
                    if (_pieces[step.Index] == PieceColor.Empty)
                        return Enumerable.Empty<Square>();
                    flankedPieces.Add(step);
                    step = NextSquare(step, deltaRow, deltaColumn);
                }
                return Enumerable.Empty<Square>();
            }

            return Enumerable.Empty<Square>();
        }

        private static Square NextSquare(Square from, int deltaRow, int deltaColumn)
        {
            return new Square(from.Row + deltaRow, from.Column + deltaColumn);
        }

        private PieceColor GetOpposite()
        {
            return _toMove == PieceColor.Black ? PieceColor.White : PieceColor.Black;
        }

        public override string ToString()
        {
            StringBuilder boardPosition = new StringBuilder();
            for (int row = 0; row < Square.NumberOfRows; row++)
            {
                for (int column = 0; column < Square.NumberOfColumns; column++)
                {
                    Square square = new Square(row, column);
                    var piece = _pieces[square.Index];
                    boardPosition.Append(
                        piece == PieceColor.Black ? "B" :
                        piece == PieceColor.White ? "W" : ".");
                }
                boardPosition.AppendLine();
            }
             
            return boardPosition.ToString();
        }
    }
}
