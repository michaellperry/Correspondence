using System;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class DataTypeFact : DataType
    {
        private string _factName;
		private bool _isPivot;

		public DataTypeFact(string factName, Cardinality cardinality, bool isPivot, int lineNumber)
			: base(cardinality, lineNumber)
		{
			_factName = factName;
			_isPivot = isPivot;
		}

        public string FactName
        {
            get { return _factName; }
        }

		public bool IsPivot
		{
			get { return _isPivot; }
		}
	}
}
