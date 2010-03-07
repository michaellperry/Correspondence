using System;
using UpdateControls.Correspondence.Factual;
using UpdateControls.Correspondence.Factual.Compiler;

namespace UpdateControls.Correspondence.Factual.Compiler.Rules
{
    public abstract class RuleSequence<T> : Rule<T>
    {
        protected static TValue GetValue<TValue>(TokenStream tokenStream, Rule<TValue> rule, string error)
        {
            if (!rule.Start(tokenStream.Lookahead.Symbol))
                throw new FactualException(error, tokenStream.Lookahead.LineNumber);
            return rule.Match(tokenStream);
        }
    }
}
