using System;
using System.Collections.Generic;

namespace Reversi.GameLogic.UnitTest
{
    public class Game
    {
        private List<Square> _moves;
        private GameBoard _position;

        public static Game OriginalGame
        {
            get
            {
                Game game = new Game();
                game._moves = new List<Square>();
                game._position = GameBoard.OpeningPosition;
                return game;
            }
        }

        private Game()
        {
        }

        public Game AfterMove(Square move)
        {
            Game next = new Game();
            next._position = _position.AfterMove(move);
            next._moves = new List<Square>(_moves);
            next._moves.Add(move);
            return next;
        }

        public IEnumerable<Square> Moves
        {
            get { return _moves; }
        }

        public GameBoard Position
        {
            get { return _position; }
        }
    }
}
