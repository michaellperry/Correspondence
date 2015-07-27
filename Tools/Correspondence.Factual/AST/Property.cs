using System;

namespace Correspondence.Factual.AST
{
    public class Property : DataMember
    {
        private bool _publish;
        private readonly int? _version;
        private readonly Alias _alias;
        
        public Property(int lineNumber, string name, DataType type, bool publish, int? version = null, Alias alias = null)
            : base(lineNumber, name, type)
        {
            _publish = publish;
            _version = version;
            _alias = alias;
        }

        public bool Publish
        {
            get { return _publish; }
        }

        public int? Version
        {
            get { return _version; }
        }

        public Alias Alias
        {
            get { return _alias; }
        }
    }
}
