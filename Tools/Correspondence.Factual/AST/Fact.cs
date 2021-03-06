using System;
using System.Linq;
using System.Collections.Generic;

namespace Correspondence.Factual.AST
{
    public class Fact
    {
        private int _lineNumber;
        private string _name;
        private readonly int? _version;
        private readonly Alias _alias;
        private bool _unique;
        private bool _lock;
        private bool _identity;
        private List<FactMember> _members = new List<FactMember>();
        
        public Fact(string name, int lineNumber, int? version = null, Alias alias = null)
        {
            _name = name;
            _lineNumber = lineNumber;
            _version = version;
            _alias = alias;
        }

        public int LineNumber
        {
            get { return _lineNumber; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string Alias
        {
            get { return _alias != null ? _alias.Identifier : _name; }
        }

        public int? Version
        {
            get { return _version; }
        }

        public bool Unique
        {
            get { return _unique; }
            set { _unique = value; }
        }

        public bool Lock
        {
            get { return _lock; }
            set { _lock = value; }
        }

        public bool Principal
        {
            get { return _identity; }
            set { _identity = value; }
        }

        public Condition PurgeCondition { get; set; }

        public IEnumerable<FactMember> Members
        {
            get { return _members; }
        }

        public Fact AddMember(FactMember member)
        {
            _members.Add(member);
            return this;
        }

        public FactMember GetMemberByName(string name)
        {
            return _members.FirstOrDefault(m => m.Name == name);
        }
    }
}
