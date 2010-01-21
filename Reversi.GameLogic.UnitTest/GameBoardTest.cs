using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Predassert;

namespace Reversi.GameLogic.UnitTest
{
    [TestClass]
    public class GameBoardTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void WhenGameStarts_ShouldBeOpeningPosition()
        {
            GameBoard gameBoard = GameBoard.OpeningPosition;

            Pred.Assert(gameBoard.PieceAt(new Square(3, 3)), Is.EqualTo(PieceColor.White));
            Pred.Assert(gameBoard.PieceAt(new Square(3, 4)), Is.EqualTo(PieceColor.Black));
            Pred.Assert(gameBoard.PieceAt(new Square(4, 3)), Is.EqualTo(PieceColor.Black));
            Pred.Assert(gameBoard.PieceAt(new Square(4, 4)), Is.EqualTo(PieceColor.White));
            for (int row = 0; row < Square.NumberOfRows; row++)
            {
                for (int column = 0; column < Square.NumberOfColumns; column++)
                {
                    bool centerRow = 3 <= row && row <= 4;
                    bool centerColumn = 3 <= column && column <= 4;
                    if (!centerRow || !centerColumn)
                        Pred.Assert(gameBoard.PieceAt(new Square(row, column)), Is.EqualTo(PieceColor.Empty));
                }
            }
        }

        [TestMethod]
        public void WhenGameStarts_ShouldBeBlacksTurn()
        {
            GameBoard gameBoard = GameBoard.OpeningPosition;

            Pred.Assert(gameBoard.ToMove, Is.EqualTo(PieceColor.Black));
        }

        [TestMethod]
        public void WhenGameStarts_ShouldAllowBlackToMove()
        {
            GameBoard gameBoard = GameBoard.OpeningPosition;

            Pred.Assert(gameBoard.LegalMoves, Contains<Square>.That(Is.EqualTo(new Square(2, 3))));
            Pred.Assert(gameBoard.LegalMoves, Contains<Square>.That(Is.EqualTo(new Square(3, 2))));
            Pred.Assert(gameBoard.LegalMoves, Contains<Square>.That(Is.EqualTo(new Square(4, 5))));
            Pred.Assert(gameBoard.LegalMoves, Contains<Square>.That(Is.EqualTo(new Square(5, 4))));

            Pred.Assert(gameBoard.LegalMoves.Count(), Is.EqualTo(4));
        }
    }
}
