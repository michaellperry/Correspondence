using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace Reversi.GameLogic.UnitTest
{
    [TestClass]
    public class FindShortGameTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [Ignore]
        public void SearchForShortestGame()
        {
            var queue = new Queue<Game>();
            queue.Enqueue(Game.OriginalGame.AfterMove(new Square(2, 3)));

            while (queue.Any())
            {
                var currentPosition = queue.Dequeue();
                foreach (Square move in currentPosition.Position.LegalMoves)
                {
                    var nextPosition = currentPosition.AfterMove(move);
                    if (nextPosition.Position.ToMove == PieceColor.Empty)
                    {
                        Console.WriteLine(string.Join(", ",
                            nextPosition.Moves.Select(m => m.ToString()).ToArray()));
                        Console.WriteLine(nextPosition.Position);
                        return;
                    }
                    queue.Enqueue(nextPosition);
                }
            }
        }

        [TestMethod]
        [Ignore]
        public void SearchForForcedPass()
        {
            var queue = new Queue<Game>();
            queue.Enqueue(Game.OriginalGame.AfterMove(new Square(2, 3)));

            while (queue.Any())
            {
                var currentPosition = queue.Dequeue();
                foreach (Square move in currentPosition.Position.LegalMoves)
                {
                    PieceColor playerWhoMoved = currentPosition.Position.ToMove;
                    var nextPosition = currentPosition.AfterMove(move);
                    if (nextPosition.Position.ToMove == playerWhoMoved)
                    {
                        Console.WriteLine(string.Join(", ",
                            nextPosition.Moves.Select(m => m.ToString()).ToArray()));
                        Console.WriteLine(nextPosition.Position);
                        return;
                    }
                    queue.Enqueue(nextPosition);
                }
            }
        }
    }
}
