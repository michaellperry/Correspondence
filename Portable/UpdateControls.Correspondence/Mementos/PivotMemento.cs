
namespace UpdateControls.Correspondence.Mementos
{
    public class PivotMemento
    {
        private FactID _pivotId;
        private TimestampID _timestamp;

        public PivotMemento(FactID pivotId, TimestampID timestamp)
        {
            _pivotId = pivotId;
            _timestamp = timestamp;
        }

        public FactID PivotId
        {
            get { return _pivotId; }
        }

        public TimestampID Timestamp
        {
            get { return _timestamp; }
        }
    }
}
