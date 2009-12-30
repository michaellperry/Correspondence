using System.Collections.Generic;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence
{
    public class FactMetadata
    {
        private List<CorrespondenceFactType> _convertableTypes;
        private List<RoleMemento> _pivotRoles;

        public FactMetadata(IEnumerable<CorrespondenceFactType> convertableTypes, IEnumerable<RoleMemento> pivotRoles)
        {
            _convertableTypes = new List<CorrespondenceFactType>(convertableTypes);
            _pivotRoles = new List<RoleMemento>(pivotRoles);
        }

        public IEnumerable<CorrespondenceFactType> ConvertableTypes
        {
            get { return _convertableTypes; }
        }

        public IEnumerable<RoleMemento> PivotRoles
        {
            get { return _pivotRoles; }
        }
    }
}
