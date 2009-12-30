using System;
using System.Collections.Generic;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence
{
    public class FactMetadata
    {
        private List<RoleMemento> _pivotRoles;

        public FactMetadata(IEnumerable<RoleMemento> pivotRoles)
        {
            _pivotRoles = new List<RoleMemento>(pivotRoles);
        }

        public IEnumerable<RoleMemento> PivotRoles
        {
            get { return _pivotRoles; }
        }
    }
}
