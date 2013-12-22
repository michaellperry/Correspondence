
namespace QEDCode.LLOne.Rules
{
    public class RuleError<TSymbol> : Rule<TSymbol, bool>
    {
        private TSymbol _triggerSymbol;
        private string _errorMessage;

        public RuleError(TSymbol triggerSymbol, string errorMessage)
        {
            _triggerSymbol = triggerSymbol;
            _errorMessage = errorMessage;
        }

        public override bool Start(TSymbol symbol)
        {
            return symbol.Equals(_triggerSymbol);
        }

        public override bool Epsilon()
        {
            return true;
        }

        public override bool Match(TokenStream<TSymbol> tokenStream)
        {
            if (tokenStream.Lookahead.Symbol.Equals(_triggerSymbol))
                throw new ParserException(_errorMessage, tokenStream.Lookahead.LineNumber);
            else
                return false;
        }
    }
}
