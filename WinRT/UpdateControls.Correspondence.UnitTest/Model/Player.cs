
namespace UpdateControls.Correspondence.UnitTest.Model
{
	public partial class Player
	{
		public void MakeMove(int index, int square)
		{
			Community.AddFactAsync(new Move(this, index, square));
		}
	}
}
