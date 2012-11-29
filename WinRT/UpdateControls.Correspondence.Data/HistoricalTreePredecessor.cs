
namespace UpdateControls.Correspondence.Data
{
    public class HistoricalTreePredecessor
    {
        private int _roleId;
        private long _predecessorFactId;

        public HistoricalTreePredecessor(int roleId, long predecessorFactId)
        {
            _roleId = roleId;
            _predecessorFactId = predecessorFactId;
        }

        public int RoleId
        {
            get { return _roleId; }
        }

        public long PredecessorFactId
        {
            get { return _predecessorFactId; }
        }
    }
}
