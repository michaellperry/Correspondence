using UpdateControls.Correspondence.Mementos;
using System.Diagnostics;
using System;

namespace UpdateControls.Correspondence
{
    public class Role<TTargetType> : RoleBase
    {
        public Role(string roleName)
        {
            Type definingType = new StackTrace().GetFrame(1).GetMethod().DeclaringType;
            _roleMemento = new RoleMemento(
                AttributeTypeStrategy.GetTypeFromCLRType(definingType),
                roleName,
                AttributeTypeStrategy.GetTypeFromCLRType(typeof(TTargetType)));
        }
    }
}
