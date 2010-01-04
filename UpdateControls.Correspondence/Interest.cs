using System;
using System.Collections.Generic;

namespace UpdateControls.Correspondence
{
    class Interest
    {
        private Func<IEnumerable<CorrespondenceFact>> _roots;

        public Interest(Func<IEnumerable<CorrespondenceFact>> roots)
        {
            _roots = roots;
        }

        public IEnumerable<CorrespondenceFact> Roots
        {
            get { return _roots(); }
        }
    }
}
