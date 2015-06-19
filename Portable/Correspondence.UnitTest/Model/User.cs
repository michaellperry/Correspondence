using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Correspondence.UnitTest.Model
{
    public partial class User
    {
        public async Task<Player> ChallengeAsync(string opponentName)
        {
            return await ChallengeAsync(await Community.AddFactAsync(new User(opponentName)));
        }

        public async Task<Player> ChallengeAsync(User other)
        {
            Game game = await Community.AddFactAsync(new Game());
            Player player = await Community.AddFactAsync(new Player(this, game, 0));
            await Community.AddFactAsync(new Player(other, game, 1));

            return player;
        }
    }
}
