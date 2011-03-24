using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class KeySection
    {
        private List<FactMember> _members = new List<FactMember>();

        public KeySection AddMember(FactMember keyMember)
        {
            _members.Add(keyMember);
            return this;
        }

        public Fact AddTo(Fact fact)
        {
            foreach (FactMember member in _members)
                fact.AddMember(member);
            return fact;
        }
    }
}
