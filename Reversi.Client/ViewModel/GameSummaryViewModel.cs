using Reversi.GameLogic;

namespace Reversi.Client
{
    public class GameSummaryViewModel
    {
        public string OpponentName { get; private set; }
        public PieceColor MyColor { get; private set; }
        public int BlackScore { get; private set; }
        public int WhiteScore { get; private set; }
    }
}
