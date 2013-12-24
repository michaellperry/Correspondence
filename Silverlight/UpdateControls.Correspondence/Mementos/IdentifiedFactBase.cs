using System;

namespace UpdateControls.Correspondence.Mementos
{
    public abstract class IdentifiedFactBase
    {
        private readonly FactID _id;

        public IdentifiedFactBase(FactID id)
        {
            _id = id;
        }

        public FactID Id
        {
            get { return _id; }
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
    }
}
