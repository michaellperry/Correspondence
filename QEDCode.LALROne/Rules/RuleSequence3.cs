using System;

namespace QEDCode.LALROne.Rules
{
    public class RuleSequence3<TSymbol, T1, T2, T3, T> : RuleSequence<TSymbol, T>
    {
        public delegate T Function(T1 v1, T2 v2, T3 v3);

        private Rule<TSymbol, T1> _rule1;
        private Rule<TSymbol, T2> _rule2;
        private string _error2;
        private Rule<TSymbol, T3> _rule3;
        private string _error3;
        private Function _reduce;

        public RuleSequence3(Rule<TSymbol, T1> rule1, Rule<TSymbol, T2> rule2, string error2, Rule<TSymbol, T3> rule3, string error3, Function reduce)
        {
            _rule1 = rule1;
            _rule2 = rule2;
            _error2 = error2;
            _rule3 = rule3;
            _error3 = error3;
            _reduce = reduce;
        }

        public override bool Start(TSymbol symbol)
        {
            return _rule1.Start(symbol);
        }

        public override T Match(TokenStream<TSymbol> tokenStream)
        {
            T1 value1 = _rule1.Match(tokenStream);
            T2 value2 = GetValue(tokenStream, _rule2, _error2);
            T3 value3 = GetValue(tokenStream, _rule3, _error3);

            return _reduce(value1, value2, value3);
        }
    }
}
