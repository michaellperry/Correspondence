using System;

namespace Correspondence.Factual.AST
{
    public class Property : DataMember
    {
        private bool _publish;

        public Property(int lineNumber, string name, DataType type, bool publish)
            : base(lineNumber, name, type)
        {
            _publish = publish;
        }

        public bool Publish
        {
            get { return _publish; }
        }
    }
}
