using System;
using UpdateControls.Correspondence.Factual.Compiler.Rules;

namespace UpdateControls.Correspondence.Factual.Compiler
{
    public abstract class Parser<T>
    {
        private TokenStream _tokenStream;
        private Rule<T> _root;
        private string _errorStart;
        private string _errorEnd;

        protected Parser(Lexer lexer, Rule<T> root, string errorStart, string errorEnd)
        {
            _tokenStream = new TokenStream(lexer);
            _root = root;
            _errorStart = errorStart;
            _errorEnd = errorEnd;
        }

        protected static Rule<Token> Terminal(Symbol expectedSymbol)
        {
            return new RuleTerminal(expectedSymbol);
        }

        protected static Rule<T> Reduce<TFrom, T>(Rule<TFrom> ruleFrom, Func<TFrom, T> reduce)
        {
            return new RuleReduce<TFrom, T>(ruleFrom, reduce);
        }

        protected static Rule<T> Optional<T>(Rule<T> rule, T defaultValue)
        {
            return new RuleOptional<T>(rule, defaultValue);
        }

        protected static Rule<T> Separated<T, TItem>(Rule<TItem> itemRule, Symbol separator, Func<TItem, T> begin, Func<T, TItem, T> append)
        {
            return new RuleSeparated<T, TItem>(itemRule, separator, begin, append);
        }

        protected static Rule<T> Many<TItem, T>(Rule<T> headerRule, Rule<TItem> itemRule, Func<T, TItem, T> append)
        {
            return new RuleMany<TItem, T>(headerRule, itemRule, append);
        }

        protected static Rule<T> Sequence<T1, T2, T>(Rule<T1> rule1, Rule<T2> rule2, string error2, RuleSequence2<T1, T2, T>.Function reduce)
        {
            return new RuleSequence2<T1, T2, T>(rule1, rule2, error2, reduce);
        }

        protected static Rule<T> Sequence<T1, T2, T3, T>(Rule<T1> rule1, Rule<T2> rule2, string error2, Rule<T3> rule3, string error3, RuleSequence3<T1, T2, T3, T>.Function reduce)
        {
            return new RuleSequence3<T1, T2, T3, T>(rule1, rule2, error2, rule3, error3, reduce);
        }

        protected static Rule<T> Sequence<T1, T2, T3, T4, T>(Rule<T1> rule1, Rule<T2> rule2, string error2, Rule<T3> rule3, string error3, Rule<T4> rule4, string error4, RuleSequence4<T1, T2, T3, T4, T>.Function reduce)
        {
            return new RuleSequence4<T1, T2, T3, T4, T>(rule1, rule2, error2, rule3, error3, rule4, error4, reduce);
        }

        protected static Rule<T> Sequence<T1, T2, T3, T4, T5, T>(Rule<T1> rule1, Rule<T2> rule2, string error2, Rule<T3> rule3, string error3, Rule<T4> rule4, string error4, Rule<T5> rule5, string error5, RuleSequence5<T1, T2, T3, T4, T5, T>.Function reduce)
        {
            return new RuleSequence5<T1, T2, T3, T4, T5, T>(rule1, rule2, error2, rule3, error3, rule4, error4, rule5, error5, reduce);
        }

        protected static Rule<T> Sequence<T1, T2, T3, T4, T5, T6, T>(Rule<T1> rule1, Rule<T2> rule2, string error2, Rule<T3> rule3, string error3, Rule<T4> rule4, string error4, Rule<T5> rule5, string error5, Rule<T6> rule6, string error6, RuleSequence6<T1, T2, T3, T4, T5, T6, T>.Function reduce)
        {
            return new RuleSequence6<T1, T2, T3, T4, T5, T6, T>(rule1, rule2, error2, rule3, error3, rule4, error4, rule5, error5, rule6, error6, reduce);
        }

        public T Parse()
        {
            _tokenStream.Consume();

            if (!_root.Start(_tokenStream.Lookahead.Symbol))
                throw new FactualException(_errorStart, _tokenStream.Lookahead.LineNumber);

            T rootNode = _root.Match(_tokenStream);

            if (_tokenStream.Lookahead.Symbol != Symbol.EndOfFile)
                throw new FactualException(_errorEnd, _tokenStream.Lookahead.LineNumber);

            return rootNode;
        }
    }
}
