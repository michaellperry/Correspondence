using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Predassert;

namespace Reversi.GameLogic.UnitTest
{
    [TestClass]
    public class ImmutabilityTest
    {
        public TestContext TestContext { get; set; }

        private GameBoard _gameBoard;

        [TestInitialize]
        public void Initialize()
        {
            _gameBoard = GameBoard.OpeningPosition;
            _gameBoard.AfterMove(new Square(2, 3));
        }

        [TestMethod]
        public void WhenNewGameBoardIsSpawned_ShouldBeOpeningPosition()
        {
            Pred.Assert(_gameBoard.PieceAt(new Square(3, 3)), Is.EqualTo(PieceColor.White));
            Pred.Assert(_gameBoard.PieceAt(new Square(3, 4)), Is.EqualTo(PieceColor.Black));
            Pred.Assert(_gameBoard.PieceAt(new Square(4, 3)), Is.EqualTo(PieceColor.Black));
            Pred.Assert(_gameBoard.PieceAt(new Square(4, 4)), Is.EqualTo(PieceColor.White));
            for (int row = 0; row < Square.NumberOfRows; row++)
            {
                for (int column = 0; column < Square.NumberOfColumns; column++)
                {
                    bool centerRow = 3 <= row && row <= 4;
                    bool centerColumn = 3 <= column && column <= 4;
                    if (!centerRow || !centerColumn)
                        Pred.Assert(_gameBoard.PieceAt(new Square(row, column)), Is.EqualTo(PieceColor.Empty));
                }
            }
        }

        [TestMethod]
        public void WhenNewGameBoardIsSpawned_ShouldBeBlacksTurn()
        {
            Pred.Assert(_gameBoard.ToMove, Is.EqualTo(PieceColor.Black));
        }

        [TestMethod]
        public void WhenNewGameBoardIsSpawned_ShouldAllowBlackToMove()
        {
            Pred.Assert(_gameBoard.LegalMoves, Contains<Square>.That(Is.EqualTo(new Square(2, 3))));
            Pred.Assert(_gameBoard.LegalMoves, Contains<Square>.That(Is.EqualTo(new Square(3, 2))));
            Pred.Assert(_gameBoard.LegalMoves, Contains<Square>.That(Is.EqualTo(new Square(4, 5))));
            Pred.Assert(_gameBoard.LegalMoves, Contains<Square>.That(Is.EqualTo(new Square(5, 4))));

            Pred.Assert(_gameBoard.LegalMoves.Count(), Is.EqualTo(4));
        }
    }
}
