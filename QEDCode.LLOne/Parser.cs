using System;
using System.Collections.Generic;
using System.Linq;
using QEDCode.LLOne.Rules;

namespace QEDCode.LLOne
{
    public abstract class Parser<TSymbol, T>
    {
        private Lexer<TSymbol> _lexer;
        private TokenStream<TSymbol> _tokenStream;
        private Rule<TSymbol, T> _root;
        private string _errorStart;
        private string _errorEnd;
        private List<ParserError> _errors = new List<ParserError>();

        protected Parser(Lexer<TSymbol> lexer, Rule<TSymbol, T> root, string errorStart, string errorEnd)
        {
            _lexer = lexer;
            _tokenStream = new TokenStream<TSymbol>(lexer);
            _root = root;
            _errorStart = errorStart;
            _errorEnd = errorEnd;
        }

        protected static Rule<TSymbol, Token<TSymbol>> Terminal(TSymbol expectedSymbol)
        {
            return new RuleTerminal<TSymbol>(expectedSymbol);
        }

        protected static Rule<TSymbol, T> Reduce<TFrom, T>(Rule<TSymbol, TFrom> ruleFrom, Func<TFrom, T> reduce)
        {
            return new RuleReduce<TSymbol, TFrom, T>(ruleFrom, reduce);
        }

        protected static Rule<TSymbol, T> Optional<T>(Rule<TSymbol, T> rule, T defaultValue)
        {
            return new RuleOptional<TSymbol, T>(rule, defaultValue);
        }

        protected static Rule<TSymbol, T> Separated<T, TItem>(Rule<TSymbol, TItem> itemRule, TSymbol separator, Func<TItem, T> begin, Func<T, TItem, T> append)
        {
            return new RuleSeparated<TSymbol, T, TItem>(itemRule, separator, begin, append);
        }

        protected static Rule<TSymbol, T> Many<TItem, T>(Rule<TSymbol, T> headerRule, Rule<TSymbol, TItem> itemRule, Func<T, TItem, T> append)
        {
            return new RuleMany<TSymbol, TItem, T>(headerRule, itemRule, append);
        }

        protected static Rule<TSymbol, T> Sequence<T1, T2, T>(Rule<TSymbol, T1> rule1, Rule<TSymbol, T2> rule2, string error2, RuleSequence2<TSymbol, T1, T2, T>.Function reduce)
        {
            return new RuleSequence2<TSymbol, T1, T2, T>(rule1, rule2, error2, reduce);
        }

        protected static Rule<TSymbol, T> Sequence<T1, T2, T3, T>(Rule<TSymbol, T1> rule1, Rule<TSymbol, T2> rule2, string error2, Rule<TSymbol, T3> rule3, string error3, RuleSequence3<TSymbol, T1, T2, T3, T>.Function reduce)
        {
            return new RuleSequence3<TSymbol, T1, T2, T3, T>(rule1, rule2, error2, rule3, error3, reduce);
        }

        protected static Rule<TSymbol, T> Sequence<T1, T2, T3, T4, T>(Rule<TSymbol, T1> rule1, Rule<TSymbol, T2> rule2, string error2, Rule<TSymbol, T3> rule3, string error3, Rule<TSymbol, T4> rule4, string error4, RuleSequence4<TSymbol, T1, T2, T3, T4, T>.Function reduce)
        {
            return new RuleSequence4<TSymbol, T1, T2, T3, T4, T>(rule1, rule2, error2, rule3, error3, rule4, error4, reduce);
        }

        protected static Rule<TSymbol, T> Sequence<T1, T2, T3, T4, T5, T>(Rule<TSymbol, T1> rule1, Rule<TSymbol, T2> rule2, string error2, Rule<TSymbol, T3> rule3, string error3, Rule<TSymbol, T4> rule4, string error4, Rule<TSymbol, T5> rule5, string error5, RuleSequence5<TSymbol, T1, T2, T3, T4, T5, T>.Function reduce)
        {
            return new RuleSequence5<TSymbol, T1, T2, T3, T4, T5, T>(rule1, rule2, error2, rule3, error3, rule4, error4, rule5, error5, reduce);
        }

        protected static Rule<TSymbol, T> Sequence<T1, T2, T3, T4, T5, T6, T>(Rule<TSymbol, T1> rule1, Rule<TSymbol, T2> rule2, string error2, Rule<TSymbol, T3> rule3, string error3, Rule<TSymbol, T4> rule4, string error4, Rule<TSymbol, T5> rule5, string error5, Rule<TSymbol, T6> rule6, string error6, RuleSequence6<TSymbol, T1, T2, T3, T4, T5, T6, T>.Function reduce)
        {
            return new RuleSequence6<TSymbol, T1, T2, T3, T4, T5, T6, T>(rule1, rule2, error2, rule3, error3, rule4, error4, rule5, error5, rule6, error6, reduce);
        }

        public T Parse()
        {
            try
            {
                _tokenStream.Consume();

                if (!_root.Start(_tokenStream.Lookahead.Symbol))
                    throw new ParserException(_errorStart, _tokenStream.Lookahead.LineNumber);

                T rootNode = _root.Match(_tokenStream);

                if (!_tokenStream.Lookahead.Symbol.Equals(_lexer.EndOfFile))
                    throw new ParserException(_errorEnd, _tokenStream.Lookahead.LineNumber);

                return rootNode;
            }
            catch (ParserException e)
            {
                _errors.Add(new ParserError(e.Message, e.LineNumber));
                return default(T);
            }
        }

        public IEnumerable<ParserError> Errors
        {
            get { return _errors; }
        }
    }
}
