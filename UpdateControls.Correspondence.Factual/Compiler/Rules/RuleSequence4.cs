using System;

namespace UpdateControls.Correspondence.Factual.Compiler.Rules
{
    public class RuleSequence4<T1, T2, T3, T4, T> : RuleSequence<T>
    {
        public delegate T Function(T1 v1, T2 v2, T3 v3, T4 v4);

        private Rule<T1> _rule1;
        private Rule<T2> _rule2;
        private string _error2;
        private Rule<T3> _rule3;
        private string _error3;
        private Rule<T4> _rule4;
        private string _error4;
        private Function _reduce;

        public RuleSequence4(Rule<T1> rule1, Rule<T2> rule2, string error2, Rule<T3> rule3, string error3, Rule<T4> rule4, string error4, Function reduce)
        {
            _rule1 = rule1;
            _rule2 = rule2;
            _error2 = error2;
            _rule3 = rule3;
            _error3 = error3;
            _rule4 = rule4;
            _error4 = error4;
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
            T3 value3 = GetValue(tokenStream, _rule3, _error3);
            T4 value4 = GetValue(tokenStream, _rule4, _error4);

            return _reduce(value1, value2, value3, value4);
        }
    }
}
