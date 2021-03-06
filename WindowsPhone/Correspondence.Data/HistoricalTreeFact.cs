using System;
using System.Collections.Generic;

namespace Correspondence.Data
{
    public class HistoricalTreeFact
    {
        private int _factTypeId;
        private byte[] _data;
        private List<HistoricalTreePredecessor> _predecessors = new List<HistoricalTreePredecessor>();

        public HistoricalTreeFact(int factTypeId, byte[] data)
        {
            _factTypeId = factTypeId;
            _data = data;
        }

        public HistoricalTreeFact AddPredecessor(int roleId, long predecessorFactId)
        {
            _predecessors.Add(new HistoricalTreePredecessor(roleId, predecessorFactId));
            return this;
        }

        public HistoricalTreeFact SetPredecessors(List<HistoricalTreePredecessor> predecessors)
        {
            _predecessors = predecessors;
            return this;
        }

        public int FactTypeId
        {
            get { return _factTypeId; }
        }

        public byte[] Data
        {
            get { return _data; }
        }

        public IEnumerable<HistoricalTreePredecessor> Predecessors
        {
            get { return _predecessors; }
        }

        public long Size
        {
            get
            {
                return 8 + 4 +
                    _predecessors.Count * (4 + 8 + 8) +
                    4 + 4 +
                    (_data == null ? 0 : _data.Length);
            }
        }
    }
}
