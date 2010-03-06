using System;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class Field
    {
        private int _lineNumber;
        private string _name;
        private FieldType _type;

        public Field(int lineNumber, string name, FieldType type)
        {
            _lineNumber = lineNumber;
            _name = name;
            _type = type;
        }

        public int LineNumber
        {
            get { return _lineNumber; }
        }

        public string Name
        {
            get { return _name; }
        }

        public FieldType Type
        {
            get { return _type; }
        }
    }
}
