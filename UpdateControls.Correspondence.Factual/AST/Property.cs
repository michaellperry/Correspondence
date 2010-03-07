using System;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class Property : DataMember
    {
        public Property(int lineNumber, string name, DataType type)
            : base(lineNumber, name, type)
        {
        }
    }
}
