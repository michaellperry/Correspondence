using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.Debugging
{
    public class PageDescriptor
    {
        private readonly CorrespondenceFactType _type;
        private readonly Func<CorrespondenceFactType, int, List<FactDescriptor>> _getFacts;
        private readonly int _page;

        public PageDescriptor(
            CorrespondenceFactType type,
            Func<CorrespondenceFactType, int, List<FactDescriptor>> getFacts,
            int page)
        {
            _type = type;
            _getFacts = getFacts;
            _page = page;
        }

        public IEnumerable<FactDescriptor> Facts
        {
            get { return _getFacts(_type, _page); }
        }

        public PageDescriptor More
        {
            get { return new PageDescriptor(_type, _getFacts, _page + 1); }
        }
    }
}
