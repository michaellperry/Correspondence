using System;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class Fact
    {
        private int _lineNumber;
        private string _name;
        private List<FactMember> _members = new List<FactMember>();

        public Fact(string name, int lineNumber)
        {
            _name = name;
            _lineNumber = lineNumber;
        }

        public int LineNumber
        {
            get { return _lineNumber; }
        }

        public string Name
        {
            get { return _name; }
        }

        public IEnumerable<FactMember> Members
        {
            get { return _members; }
        }

        public Fact AddMember(FactMember member)
        {
            _members.Add(member);
            return this;
        }
    }
}
