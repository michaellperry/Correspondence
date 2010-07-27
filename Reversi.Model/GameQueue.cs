
namespace Reversi.Model
{
	public partial class GameQueue
	{
		public GameRequest CreateGameRequest(Person person)
		{
			return Community.AddFact(new GameRequest(this, person));
		}
	}
}
