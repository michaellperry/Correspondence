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
        private List<ParserError> _errors = new List<ParserError>();

        protected Parser(Lexer<TSymbol> lexer, Rule<TSymbol, T> root, string errorStart)
        {
            _lexer = lexer;
            _tokenStream = new TokenStream<TSymbol>(lexer);
            _root = root;
            _errorStart = errorStart;
        }

        protected static Rule<TSymbol, Token<TSymbol>> Terminal(TSymbol expectedSymbol)
        {
            return new RuleTerminal<TSymbol>(expectedSymbol);
        }

        protected static Rule<TSymbol, bool> Error(TSymbol triggerSymbol, string errorMessage)
        {
            return new RuleError<TSymbol>(triggerSymbol, errorMessage);
        }

        protected static Rule<TSymbol, TResult> Reduce<TFrom, TResult>(Rule<TSymbol, TFrom> ruleFrom, Func<TFrom, TResult> reduce)
        {
            return new RuleReduce<TSymbol, TFrom, TResult>(ruleFrom, reduce);
        }

        protected static Rule<TSymbol, TResult> Optional<TResult>(Rule<TSymbol, TResult> rule, TResult defaultValue)
        {
            return new RuleOptional<TSymbol, TResult>(rule, defaultValue);
        }

        protected static Rule<TSymbol, TResult> Separated<TResult, TItem>(Rule<TSymbol, TItem> itemRule, TSymbol separator, Func<TItem, TResult> begin, Func<TResult, TItem, TResult> append)
        {
            return new RuleSeparated<TSymbol, TResult, TItem>(itemRule, separator, begin, append);
        }

        protected static Rule<TSymbol, TResult> Many<TItem, TResult>(Rule<TSymbol, TResult> headerRule, Rule<TSymbol, TItem> itemRule, Func<TResult, TItem, TResult> append)
        {
            return new RuleMany<TSymbol, TItem, TResult>(headerRule, itemRule, append);
        }

        protected static Rule<TSymbol, TResult> Sequence<T1, T2, TResult>(Rule<TSymbol, T1> rule1, Rule<TSymbol, T2> rule2, string error2, RuleSequence2<TSymbol, T1, T2, TResult>.Function reduce)
        {
            return new RuleSequence2<TSymbol, T1, T2, TResult>(rule1, rule2, error2, reduce);
        }

        protected static Rule<TSymbol, TResult> Sequence<T1, T2, T3, TResult>(Rule<TSymbol, T1> rule1, Rule<TSymbol, T2> rule2, string error2, Rule<TSymbol, T3> rule3, string error3, RuleSequence3<TSymbol, T1, T2, T3, TResult>.Function reduce)
        {
            return new RuleSequence3<TSymbol, T1, T2, T3, TResult>(rule1, rule2, error2, rule3, error3, reduce);
        }

        protected static Rule<TSymbol, TResult> Sequence<T1, T2, T3, T4, TResult>(Rule<TSymbol, T1> rule1, Rule<TSymbol, T2> rule2, string error2, Rule<TSymbol, T3> rule3, string error3, Rule<TSymbol, T4> rule4, string error4, RuleSequence4<TSymbol, T1, T2, T3, T4, TResult>.Function reduce)
        {
            return new RuleSequence4<TSymbol, T1, T2, T3, T4, TResult>(rule1, rule2, error2, rule3, error3, rule4, error4, reduce);
        }

        protected static Rule<TSymbol, TResult> Sequence<T1, T2, T3, T4, T5, TResult>(Rule<TSymbol, T1> rule1, Rule<TSymbol, T2> rule2, string error2, Rule<TSymbol, T3> rule3, string error3, Rule<TSymbol, T4> rule4, string error4, Rule<TSymbol, T5> rule5, string error5, RuleSequence5<TSymbol, T1, T2, T3, T4, T5, TResult>.Function reduce)
        {
            return new RuleSequence5<TSymbol, T1, T2, T3, T4, T5, TResult>(rule1, rule2, error2, rule3, error3, rule4, error4, rule5, error5, reduce);
        }

        protected static Rule<TSymbol, TResult> Sequence<T1, T2, T3, T4, T5, T6, TResult>(Rule<TSymbol, T1> rule1, Rule<TSymbol, T2> rule2, string error2, Rule<TSymbol, T3> rule3, string error3, Rule<TSymbol, T4> rule4, string error4, Rule<TSymbol, T5> rule5, string error5, Rule<TSymbol, T6> rule6, string error6, RuleSequence6<TSymbol, T1, T2, T3, T4, T5, T6, TResult>.Function reduce)
        {
            return new RuleSequence6<TSymbol, T1, T2, T3, T4, T5, T6, TResult>(rule1, rule2, error2, rule3, error3, rule4, error4, rule5, error5, rule6, error6, reduce);
        }

        protected static Rule<TSymbol, TResult> Sequence<T1, T2, T3, T4, T5, T6, T7, TResult>(Rule<TSymbol, T1> rule1, Rule<TSymbol, T2> rule2, string error2, Rule<TSymbol, T3> rule3, string error3, Rule<TSymbol, T4> rule4, string error4, Rule<TSymbol, T5> rule5, string error5, Rule<TSymbol, T6> rule6, string error6, Rule<TSymbol, T7> rule7, string error7, RuleSequence7<TSymbol, T1, T2, T3, T4, T5, T6, T7, TResult>.Function reduce)
        {
            return new RuleSequence7<TSymbol, T1, T2, T3, T4, T5, T6, T7, TResult>(rule1, rule2, error2, rule3, error3, rule4, error4, rule5, error5, rule6, error6, rule7, error7, reduce);
        }

        public T Parse()
        {
            try
            {
                if (!_root.Start(_tokenStream.Lookahead.Symbol))
                    throw new ParserException(_errorStart, _tokenStream.Lookahead.LineNumber);

                T rootNode = _root.Match(_tokenStream);

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
