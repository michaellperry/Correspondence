using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Predassert;

namespace Reversi.GameLogic.UnitTest
{
    [TestClass]
    public class EdgeOfBoardTest
    {
        public TestContext TestContext { get; set; }

        private GameBoard _position;

        [TestInitialize]
        public void Initialize()
        {
            _position = GameBoard.OpeningPosition;
        }

        [TestMethod]
        public void WhenAtEdgeOfBoard_CannotBeFlanked()
        {
            MakeMove(new Square(2, 3));
            MakeMove(new Square(2, 4));
            MakeMove(new Square(1, 5));
            MakeMove(new Square(4, 2));
            MakeMove(new Square(3, 5));
            MakeMove(new Square(0, 6));

            Pred.Assert(_position.ToMove, Is.EqualTo(PieceColor.Black));
            Pred.Assert(_position.LegalMoves, ContainsNo<Square>.That(Is.EqualTo(new Square(5, 1))));
        }

        private void MakeMove(Square move)
        {
            _position = _position.AfterMove(move);
            Console.WriteLine(_position);
        }
    }
}
