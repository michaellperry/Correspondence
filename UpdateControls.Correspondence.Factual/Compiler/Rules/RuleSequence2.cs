using System;

namespace UpdateControls.Correspondence.Factual.Compiler.Rules
{
    public class RuleSequence2<T1, T2, T> : Rule<T>
    {
        private Rule<T1> _rule1;
        private Rule<T2> _rule2;
        private string _error2;
        private Func<T1, T2, T> _reduce;

        public RuleSequence2(Rule<T1> rule1, Rule<T2> rule2, string error2, Func<T1, T2, T> reduce)
        {
            _rule1 = rule1;
            _rule2 = rule2;
            _error2 = error2;
            _reduce = reduce;
        }

        public override bool Start(Symbol symbol)
        {
            return _rule1.Start(symbol);
        }

        public override T Match(TokenStream tokenStream)
        {
            T1 value1 = _rule1.Match(tokenStream);

            if (!_rule2.Start(tokenStream.Lookahead.Symbol))
                throw new FactualException(_error2, tokenStream.Lookahead.LineNumber);
            T2 value2 = _rule2.Match(tokenStream);

            return _reduce(value1, value2);
        }
    }
}
