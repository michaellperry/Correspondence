using System;

namespace Correspondence.Mementos
{
    public struct TimestampID
    {
        private Int64 _databaseId;
        private Int64 _key;

        public TimestampID(Int64 databaseId, Int64 key)
        {
            _databaseId = databaseId;
            _key = key;
        }

        public Int64 DatabaseId
        {
            get { return _databaseId; }
        }

        public Int64 Key
        {
            get { return _key; }
        }

        public int CompareTo(TimestampID that)
        {
            if (_databaseId < that._databaseId)
                return -1;
            if (_databaseId > that._databaseId)
                return 1;
            if (_key < that._key)
                return -1;
            if (_key > that._key)
                return 1;
            return 0;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != this.GetType())
                return false;
            TimestampID that = (TimestampID)obj;
            return
                this._databaseId == that._databaseId &&
                this._key == that._key;
        }

        public override int GetHashCode()
        {
            return (int)(_databaseId * 37 + _key);
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", _databaseId, _key);
        }
    }
}
