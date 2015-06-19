using System;

namespace Correspondence.Factual.AST
{
    public class Field : DataMember
    {
        private bool _publish;
        private Condition _publishCondition;

        public Field(int lineNumber, string name, DataType type, bool publish, Condition publishCondition)
            : base(lineNumber, name, type)
        {
            _publish = publish;
            _publishCondition = publishCondition;
        }

        public FieldSecurityModifier SecurityModifier { get; set; }

        public bool Publish
        {
            get { return _publish; }
        }

        public Condition PublishCondition
        {
            get { return _publishCondition; }
        }
    }
}
