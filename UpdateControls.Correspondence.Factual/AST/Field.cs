using System;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class Field : DataMember
    {
        public Field(int lineNumber, string name, DataType type)
            : base(lineNumber, name, type)
        {
        }
    }
}
