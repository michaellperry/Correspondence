﻿using System.Collections.Generic;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class FactSection
    {
        private bool _unique;
        private bool _principal;
        private List<FactMember> _members = new List<FactMember>();

        public FactSection AddMember(FactMember keyMember)
        {
            _members.Add(keyMember);
            return this;
        }

        public FactSection SetUnique()
        {
            _unique = true;
            return this;
        }

        public FactSection SetPrincipal()
        {
            _principal = true;
            return this;
        }

        public Fact AddTo(Fact fact)
        {
            foreach (FactMember member in _members)
                fact.AddMember(member);
            if (_unique)
                fact.Unique = true;
            if (_principal)
                fact.Principal = true;
            return fact;
        }
    }
}