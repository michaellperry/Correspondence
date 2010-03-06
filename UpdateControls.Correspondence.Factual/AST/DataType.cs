using System;

namespace UpdateControls.Correspondence.Factual.AST
{
    public abstract class DataType
    {
        private int _lineNumber;
        private Cardinality _cardinality;

        public DataType(Cardinality cardinality, int lineNumber)
        {
            _lineNumber = lineNumber;
            _cardinality = cardinality;
        }

        public int LineNumber
        {
            get { return _lineNumber; }
        }

        public Cardinality Cardinality
        {
            get { return _cardinality; }
        }
    }
}
