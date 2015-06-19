using System;

namespace Correspondence.Mementos
{
    public class MessageMemento
    {
        private FactID _pivotId;
        private FactID _factId;

        public MessageMemento(FactID pivotId, FactID factId)
        {
            _pivotId = pivotId;
            _factId = factId;
        }

        public FactID PivotId
        {
            get { return _pivotId; }
        }

        public FactID FactId
        {
            get { return _factId; }
        }

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;
			MessageMemento that = obj as MessageMemento;
			if (that == null)
				return false;
			return _pivotId.Equals(that._pivotId) && _factId.Equals(that._factId);
		}

		public override int GetHashCode()
		{
			return _pivotId.GetHashCode() * 37 + _factId.GetHashCode();
		}
    }
}
