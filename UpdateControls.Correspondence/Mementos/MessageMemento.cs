using System;

namespace UpdateControls.Correspondence.Mementos
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
    }
}
