using System;

namespace UpdateControls.Correspondence.Factual.Compiler.Rules
{
    public class RuleSequence2<T1, T2, T> : RuleSequence<T>
    {
        public delegate T Function(T1 v1, T2 v2);

        private Rule<T1> _rule1;
        private Rule<T2> _rule2;
        private string _error2;
        private Function _reduce;

        public RuleSequence2(Rule<T1> rule1, Rule<T2> rule2, string error2, Function reduce)
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
            T2 value2 = GetValue(tokenStream, _rule2, _error2);

            return _reduce(value1, value2);
        }
    }
}
