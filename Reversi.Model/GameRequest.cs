using System.Collections.Generic;
using System.Linq;

namespace Reversi.Model
{
	public partial class GameRequest
	{
		public Game CreateGame(GameRequest second)
		{
			return Community.AddFact(new Game(new List<GameRequest> { this, second }));
		}

		public Game Game
		{
			get { return Games.FirstOrDefault(); }
		}
	}
}
