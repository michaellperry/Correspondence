using System.Linq;
using System.Threading.Tasks;

namespace UpdateControls.Correspondence.UnitTest.Model
{
	public partial class Game
	{
		public async Task<Player> CreatePlayerAsync(User person)
		{
			return await Community.AddFactAsync(new Player(person, this, 0));
		}

		public Outcome Outcome
		{
			get { return Outcomes.FirstOrDefault(); }
		}

		public void DeclareWinner(Player winner)
		{
			Community.AddFactAsync(new Outcome(this, winner));
		}
	}
}
