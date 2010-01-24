using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Predassert;

namespace Reversi.GameLogic.UnitTest
{
    [TestClass]
    public class ForcedPassTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void WhenNoLegalMovesForPlayer_ShouldBeForcedToPass()
        {
            GameBoard gameBoard = GameBoard.OpeningPosition
                .AfterMove(new Square(2,3))
                .AfterMove(new Square(2,2))
                .AfterMove(new Square(2,1))
                .AfterMove(new Square(1,1))
                .AfterMove(new Square(4,5))
                .AfterMove(new Square(2,0))
                .AfterMove(new Square(0,0))
                .AfterMove(new Square(0,2));

            Pred.Assert(gameBoard.ToMove, Is.EqualTo(PieceColor.White));
        }
    }
}
