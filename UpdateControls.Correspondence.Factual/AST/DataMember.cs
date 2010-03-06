using System;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class DataMember
    {
        private int _lineNumber;
        private string _name;
        private DataType _type;

        public DataMember(int lineNumber, string name, DataType type)
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

        public DataType Type
        {
            get { return _type; }
        }
    }
}
