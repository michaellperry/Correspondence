using System.Collections.Generic;
using System.Linq;

namespace UpdateControls.Correspondence.Mementos
{
    /// <summary>
	/// </summary>
	public class FactMemento
	{
        private CorrespondenceFactType _factType;
		private List<PredecessorMemento> _predecessors = new List<PredecessorMemento>();
		private byte[] _data;

		public FactMemento(CorrespondenceFactType factType)
		{
            _factType = factType;
		}

		public CorrespondenceFactType FactType
		{
			get { return _factType; }
		}

        public IEnumerable<PredecessorMemento> Predecessors
		{
			get { return _predecessors; }
		}

        public IEnumerable<FactID> GetPredecessorIdsByRole(RoleMemento role)
        {
            return _predecessors.Where(p => p.Role.Equals(role)).Select(p => p.ID);
        }

		public void AddPredecessors(IEnumerable<PredecessorMemento> predecessors)
		{
			_predecessors.AddRange(predecessors);
		}

        public void AddPredecessor(RoleMemento role, FactID id)
        {
            PredecessorMemento predecessor = new PredecessorMemento(role, id);
            if (!_predecessors.Contains(predecessor))
                _predecessors.Add(predecessor);
        }

		public byte[] Data
		{
			get { return _data; }
			set { _data = value; }
		}

		public override int GetHashCode()
		{
			int hash = _factType.GetHashCode();
			foreach ( PredecessorMemento entry in _predecessors )
				hash = hash * 37 + entry.GetHashCode();
			foreach ( byte b in _data )
				hash = hash * 37 + b;

			return hash;
		}

		public override bool Equals(object obj)
		{
			if ( obj == null )
				return false;
			if ( obj.GetType() != this.GetType() )
				return false;
			FactMemento that = (FactMemento)obj;
			if ( !this._factType.Equals(that._factType) )
				return false;
            if ( !this._predecessors.SequenceEqual(that._predecessors) )
				return false;
            if (this._data.Length != that._data.Length)
                return false;
            for (int index = 0; index < this._data.Length; ++index)
            {
                if (this._data[index] != that._data[index])
                    return false;
            }

			return true;
		}
	}
}
