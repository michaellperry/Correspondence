using System;

namespace UpdateControls.Correspondence.Factual.Compiler.Rules
{
    public class RuleOr<T> : Rule<T>
    {
        private Rule<T> _rule1;
        private Rule<T> _rule2;

        public RuleOr(Rule<T> rule1, Rule<T> rule2)
        {
            _rule1 = rule1;
            _rule2 = rule2;
        }

        public override bool Start(Symbol symbol)
        {
            bool start1 = _rule1.Start(symbol);
            bool start2 = _rule2.Start(symbol);
            if (start1 && start2)
                throw new FactualException(string.Format("The grammar is ambiguous for symbol {0}. It is not LALR(1).", symbol), 0);
            return start1 || start2;
        }

        public override T Match(TokenStream tokenStream)
        {
            if (_rule1.Start(tokenStream.Lookahead.Symbol))
                return _rule1.Match(tokenStream);
            else
                return _rule2.Match(tokenStream);
        }
    }
}
