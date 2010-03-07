using System;

namespace UpdateControls.Correspondence.Factual.AST
{
    public abstract class DataMember : FactMember
    {
        private DataType _type;

        public DataMember(int lineNumber, string name, DataType type)
            : base(name, lineNumber)
        {
            _type = type;
        }

        public DataType Type
        {
            get { return _type; }
        }
    }
}
