﻿using System;
using System.Diagnostics;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence
{
    public class Role<TTargetType> : RoleBase
    {
        public Role(string roleName, RoleRelationship metadata)
            : base(GetRoleMemento(roleName), metadata)
        {
        }

        public Role(string roleName)
            : base(GetRoleMemento(roleName), RoleRelationship.Local)
        {
        }

        private static RoleMemento GetRoleMemento(string roleName)
        {
            Type definingType = new StackTrace().GetFrame(2).GetMethod().DeclaringType;
            RoleMemento roleMemento = new RoleMemento(
                AttributeTypeStrategy.GetTypeFromCLRType(definingType),
                roleName,
                AttributeTypeStrategy.GetTypeFromCLRType(typeof(TTargetType)));
            return roleMemento;
        }
    }
}
