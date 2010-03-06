
namespace UpdateControls.Correspondence.Factual.Compiler
{
    public class Token
    {
        private Symbol _symbol;
        private string _value;
        private int _lineNumber;

        public Token(Symbol symbol, string value, int lineNumber)
        {
            _symbol = symbol;
            _value = value;
            _lineNumber = lineNumber;
        }

        public Symbol Symbol
        {
            get { return _symbol; }
        }

        public string Value
        {
            get { return _value; }
        }

        public int LineNumber
        {
            get { return _lineNumber; }
        }
    }
}
