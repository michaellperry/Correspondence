using System;
using System.Collections.Generic;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.Debug
{
    public class TypeDescriptor
    {
        private readonly CorrespondenceFactType _type;
        private readonly Func<CorrespondenceFactType, int, List<FactDescriptor>> _getFacts;

        public TypeDescriptor(
            CorrespondenceFactType type,
            Func<CorrespondenceFactType, int, List<FactDescriptor>> getFacts)
        {
            _type = type;
            _getFacts = getFacts;
        }

        public IEnumerable<FactDescriptor> Facts
        {
            get { return _getFacts(_type, 0); }
        }

        public PageDescriptor More
        {
            get { return new PageDescriptor(_type, _getFacts, 1); }
        }

        public override string ToString()
        {
            return _type.ToString();
        }
    }
}
