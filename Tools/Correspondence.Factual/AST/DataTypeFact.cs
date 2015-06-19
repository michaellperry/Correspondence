using System;

namespace Correspondence.Factual.AST
{
    public class DataTypeFact : DataType
    {
        private string _factName;

        public DataTypeFact(string factName, Cardinality cardinality, int lineNumber)
			: base(cardinality, lineNumber)
		{
			_factName = factName;
		}

        public string FactName
        {
            get { return _factName; }
        }
	}
}
