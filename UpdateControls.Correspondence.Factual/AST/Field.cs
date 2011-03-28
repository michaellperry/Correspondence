using System;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class Field : DataMember
    {
        private bool _publish;

        public Field(int lineNumber, string name, DataType type, bool publish)
            : base(lineNumber, name, type)
        {
            _publish = publish;
        }

        public FieldSecurityModifier SecurityModifier { get; set; }

        public bool Publish
        {
            get { return _publish; }
        }
    }
}
