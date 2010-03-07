using System;
using UpdateControls.Correspondence.Factual;
using UpdateControls.Correspondence.Factual.Compiler;

namespace UpdateControls.Correspondence.Factual.Compiler.Rules
{
    public class RuleSequence6<T1, T2, T3, T4, T5, T6, T> : RuleSequence<T>
    {
        public delegate T Function(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6);

        private Rule<T1> _rule1;
        private Rule<T2> _rule2;
        private string _error2;
        private Rule<T3> _rule3;
        private string _error3;
        private Rule<T4> _rule4;
        private string _error4;
        private Rule<T5> _rule5;
        private string _error5;
        private Rule<T6> _rule6;
        private string _error6;
        private Function _reduce;

        public RuleSequence6(Rule<T1> rule1, Rule<T2> rule2, string error2, Rule<T3> rule3, string error3, Rule<T4> rule4, string error4, Rule<T5> rule5, string error5, Rule<T6> rule6, string error6, Function reduce)
        {
            _rule1 = rule1;
            _rule2 = rule2;
            _error2 = error2;
            _rule3 = rule3;
            _error3 = error3;
            _rule4 = rule4;
            _error4 = error4;
            _rule5 = rule5;
            _error5 = error5;
            _rule6 = rule6;
            _error6 = error6;
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
            T5 value5 = GetValue(tokenStream, _rule5, _error5);
            T6 value6 = GetValue(tokenStream, _rule6, _error6);

            return _reduce(value1, value2, value3, value4, value5, value6);
        }
    }
}
