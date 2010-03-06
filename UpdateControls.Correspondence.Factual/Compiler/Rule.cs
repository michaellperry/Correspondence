using System;

namespace UpdateControls.Correspondence.Factual.Compiler
{
    public abstract class Rule<T>
    {
        public abstract bool Start(Symbol symbol);
        public abstract T Match(TokenStream tokenStream);
    }
}
