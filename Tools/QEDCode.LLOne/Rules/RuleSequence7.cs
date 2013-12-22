using System;
using QEDCode.LLOne;

namespace QEDCode.LLOne.Rules
{
    public class RuleSequence7<TSymbol, T1, T2, T3, T4, T5, T6, T7, T> : RuleSequence<TSymbol, T>
    {
        public delegate T Function(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7);

        private Rule<TSymbol, T1> _rule1;
        private Rule<TSymbol, T2> _rule2;
        private string _error2;
        private Rule<TSymbol, T3> _rule3;
        private string _error3;
        private Rule<TSymbol, T4> _rule4;
        private string _error4;
        private Rule<TSymbol, T5> _rule5;
        private string _error5;
        private Rule<TSymbol, T6> _rule6;
        private string _error6;
        private Rule<TSymbol, T7> _rule7;
        private string _error7;
        private Function _reduce;

        public RuleSequence7(Rule<TSymbol, T1> rule1, Rule<TSymbol, T2> rule2, string error2, Rule<TSymbol, T3> rule3, string error3, Rule<TSymbol, T4> rule4, string error4, Rule<TSymbol, T5> rule5, string error5, Rule<TSymbol, T6> rule6, string error6, Rule<TSymbol, T7> rule7, string error7, Function reduce)
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
            _rule7 = rule7;
            _error7 = error7;
            _reduce = reduce;
        }

        public override bool Start(TSymbol symbol)
        {
            if (!_rule1.Epsilon())
                return _rule1.Start(symbol);
            if (_rule1.Start(symbol))
                return true;
            if (!_rule2.Epsilon())
                return _rule2.Start(symbol);
            if (_rule2.Start(symbol))
                return true;
            if (!_rule3.Epsilon())
                return _rule3.Start(symbol);
            if (_rule3.Start(symbol))
                return true;
            if (!_rule4.Epsilon())
                return _rule4.Start(symbol);
            if (_rule4.Start(symbol))
                return true;
            if (!_rule5.Epsilon())
                return _rule5.Start(symbol);
            if (_rule5.Start(symbol))
                return true;
            if (!_rule6.Epsilon())
                return _rule6.Start(symbol);
            if (_rule6.Start(symbol))
                return true;
            return _rule7.Start(symbol);
        }

        public override T Match(TokenStream<TSymbol> tokenStream)
        {
            T1 value1 = _rule1.Match(tokenStream);
            T2 value2 = GetValue(tokenStream, _rule2, _error2);
            T3 value3 = GetValue(tokenStream, _rule3, _error3);
            T4 value4 = GetValue(tokenStream, _rule4, _error4);
            T5 value5 = GetValue(tokenStream, _rule5, _error5);
            T6 value6 = GetValue(tokenStream, _rule6, _error6);
            T7 value7 = GetValue(tokenStream, _rule7, _error7);

            return _reduce(value1, value2, value3, value4, value5, value6, value7);
        }
    }
}
