using System.Collections.Generic;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence
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
