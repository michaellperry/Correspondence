using System;
using QEDCode.LLOne;

namespace QEDCode.LLOne.Rules
{
    public abstract class RuleSequence<TSymbol, T> : Rule<TSymbol, T>
    {
        protected static TValue GetValue<TValue>(TokenStream<TSymbol> tokenStream, Rule<TSymbol, TValue> rule, string error)
        {
            if (!rule.Start(tokenStream.Lookahead.Symbol))
                throw new ParserException(error, tokenStream.Lookahead.LineNumber);
            return rule.Match(tokenStream);
        }
    }
}
