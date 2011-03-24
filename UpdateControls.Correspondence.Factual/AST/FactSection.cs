using System.Collections.Generic;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class FactSection
    {
        private List<FactMember> _members = new List<FactMember>();

        public FactSection AddMember(FactMember keyMember)
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
