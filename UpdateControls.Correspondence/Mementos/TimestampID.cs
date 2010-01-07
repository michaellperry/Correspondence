using System;

namespace UpdateControls.Correspondence.Mementos
{
    public struct TimestampID
    {
        public Int64 key;

        public int CompareTo(TimestampID that)
        {
            return this.key.CompareTo(that.key);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != this.GetType())
                return false;
            TimestampID that = (TimestampID)obj;
            return this.key == that.key;
        }

        public override int GetHashCode()
        {
            return (int)key;
        }

        public override string ToString()
        {
            return key.ToString();
        }
    }
}
