using System.Linq;

namespace Reversi.Model
{
	public partial class Game
	{
		public Player CreatePlayer(Person person)
		{
			return Community.AddFact(new Player(person, this));
		}

		public GameRequest First
		{
			get { return GameRequests.ElementAt(0); }
		}

		public GameRequest Second
		{
			get { return GameRequests.ElementAt(1); }
		}

		public Outcome Outcome
		{
			get { return Outcomes.FirstOrDefault(); }
		}

		public void DeclareWinner(GameRequest winnersRequest)
		{
			var winner = Players.FirstOrDefault(player => player.Person == winnersRequest.Person);
			Community.AddFact(new Outcome(this, winner));
		}
	}
}
