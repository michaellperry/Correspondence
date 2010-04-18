using System;

namespace QEDCode.LLOne.Rules
{
    public class RuleSequence2<TSymbol, T1, T2, T> : RuleSequence<TSymbol, T>
    {
        public delegate T Function(T1 v1, T2 v2);

        private Rule<TSymbol, T1> _rule1;
        private Rule<TSymbol, T2> _rule2;
        private string _error2;
        private Function _reduce;

        public RuleSequence2(Rule<TSymbol, T1> rule1, Rule<TSymbol, T2> rule2, string error2, Function reduce)
        {
            _rule1 = rule1;
            _rule2 = rule2;
            _error2 = error2;
            _reduce = reduce;
        }

        public override bool Start(TSymbol symbol)
        {
            if (!_rule1.Epsilon())
                return _rule1.Start(symbol);
            if (_rule1.Start(symbol))
                return true;
            return _rule2.Start(symbol);
        }

        public override T Match(TokenStream<TSymbol> tokenStream)
        {
            T1 value1 = _rule1.Match(tokenStream);
            T2 value2 = GetValue(tokenStream, _rule2, _error2);

            return _reduce(value1, value2);
        }
    }
}
