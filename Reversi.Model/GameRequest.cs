using System.Collections.Generic;

namespace Reversi.Model
{
	public partial class GameRequest
	{
		public Game CreateGame(GameRequest second)
		{
			return Community.AddFact(new Game(new List<GameRequest> { this, second }));
		}
	}
}
