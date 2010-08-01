using System.Linq;

namespace Reversi.Model
{
	public partial class Game
	{
		public Player CreatePlayer(User person)
		{
			return Community.AddFact(new Player(person, this, 0));
		}

		public Outcome Outcome
		{
			get { return Outcomes.FirstOrDefault(); }
		}

		public void DeclareWinner(Player winner)
		{
			Community.AddFact(new Outcome(this, winner));
		}
	}
}
