using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reversi.Model
{
    public partial class User
    {
        public Player Challenge(string opponentName)
        {
            return Challenge(Community.AddFact(new User(opponentName)));
        }

        public Player Challenge(User other)
        {
            Game game = Community.AddFact(new Game());
            Player player = Community.AddFact(new Player(this, game, 0));
            Community.AddFact(new Player(other, game, 1));

            return player;
        }
    }
}
