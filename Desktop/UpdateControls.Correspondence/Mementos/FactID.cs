using System;

namespace UpdateControls.Correspondence.Mementos
{
    public struct FactID : IComparable<FactID>
    {
        public Int64 key;

        public int CompareTo(FactID that)
        {
            return this.key.CompareTo(that.key);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != this.GetType())
                return false;
            FactID that = (FactID)obj;
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
