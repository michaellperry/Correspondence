using System.Collections.Generic;
using Correspondence.Mementos;

namespace Correspondence
{
    public class FactMetadata
    {
        private List<CorrespondenceFactType> _convertableTypes;

        public FactMetadata(IEnumerable<CorrespondenceFactType> convertableTypes)
        {
            _convertableTypes = new List<CorrespondenceFactType>(convertableTypes);
        }

        public IEnumerable<CorrespondenceFactType> ConvertableTypes
        {
            get { return _convertableTypes; }
        }
    }
}
