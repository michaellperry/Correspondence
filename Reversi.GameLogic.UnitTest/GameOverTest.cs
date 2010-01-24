using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Predassert;

namespace Reversi.GameLogic.UnitTest
{
    [TestClass]
    public class GameOverTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void WhenNoLegalMovesLeft_ShouldBeGameOver()
        {
            GameBoard gameBoard = GameBoard.OpeningPosition
                .AfterMove(new Square(2, 3))
                .AfterMove(new Square(2, 2))
                .AfterMove(new Square(2, 1))
                .AfterMove(new Square(1, 3))
                .AfterMove(new Square(0, 4))
                .AfterMove(new Square(5, 3))
                .AfterMove(new Square(6, 3))
                .AfterMove(new Square(2, 4))
                .AfterMove(new Square(3, 5));

            Pred.Assert(gameBoard.ToMove, Is.EqualTo(PieceColor.Empty));
        }
    }
}
