using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Predassert;

namespace Reversi.GameLogic.UnitTest
{
    [TestClass]
    public class FirstMoveTest
    {
        public TestContext TestContext { get; set; }

        private Square _blacksMove;
        private GameBoard _gameBoard;

        [TestInitialize]
        public void Initialize()
        {
            _blacksMove = new Square(2, 3);
            _gameBoard = GameBoard.OpeningPosition.AfterMove(_blacksMove);
        }

        [TestMethod]
        public void WhenBlackPlays_ShouldBeModifiedPosition()
        {
            Pred.Assert(_gameBoard.PieceAt(_blacksMove), Is.EqualTo(PieceColor.Black));
            Pred.Assert(_gameBoard.PieceAt(new Square(3, 3)), Is.EqualTo(PieceColor.Black));
            Pred.Assert(_gameBoard.PieceAt(new Square(3, 4)), Is.EqualTo(PieceColor.Black));
            Pred.Assert(_gameBoard.PieceAt(new Square(4, 3)), Is.EqualTo(PieceColor.Black));
            Pred.Assert(_gameBoard.PieceAt(new Square(4, 4)), Is.EqualTo(PieceColor.White));
            for (int row = 0; row < Square.NumberOfRows; row++)
            {
                for (int column = 0; column < Square.NumberOfColumns; column++)
                {
                    Square square = new Square(row, column);
                    bool centerRow = 3 <= row && row <= 4;
                    bool centerColumn = 3 <= column && column <= 4;
                    if ((!centerRow || !centerColumn) && !square.Equals(_blacksMove))
                        Pred.Assert(_gameBoard.PieceAt(square), Is.EqualTo(PieceColor.Empty));
                }
            }
        }

        [TestMethod]
        public void WhenBlackPlays_ShouldBeWhitesTurn()
        {
            Pred.Assert(_gameBoard.ToMove, Is.EqualTo(PieceColor.White));
        }

        [TestMethod]
        public void WhenBlackPlays_ShouldAllowWhiteToMove()
        {
            Pred.Assert(_gameBoard.LegalMoves, Contains<Square>.That(Is.EqualTo(new Square(2, 2))));
            Pred.Assert(_gameBoard.LegalMoves, Contains<Square>.That(Is.EqualTo(new Square(2, 4))));
            Pred.Assert(_gameBoard.LegalMoves, Contains<Square>.That(Is.EqualTo(new Square(4, 2))));

            Pred.Assert(_gameBoard.LegalMoves.Count(), Is.EqualTo(3));
        }
    }
}
