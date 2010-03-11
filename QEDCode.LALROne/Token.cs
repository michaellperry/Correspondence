
namespace QEDCode.LALROne
{
    public class Token<TSymbol>
    {
        private TSymbol _symbol;
        private string _value;
        private int _lineNumber;

        public Token(TSymbol symbol, string value, int lineNumber)
        {
            _symbol = symbol;
            _value = value;
            _lineNumber = lineNumber;
        }

        public TSymbol Symbol
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
