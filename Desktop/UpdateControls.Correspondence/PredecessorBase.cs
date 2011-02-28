using System.Collections.Generic;
using System;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence
{
    public abstract class PredecessorBase
    {
        private CorrespondenceFact _subject;
        private bool _cached;

        protected PredecessorBase(CorrespondenceFact subject, bool cached)
        {
            _subject = subject;
            _cached = cached;
        }

        internal abstract Community Community { get; }
        internal abstract IEnumerable<FactID> InternalFactIds { get; }
        protected abstract void PopulateCache(Community community);
        protected abstract void EmptyCache();
        internal abstract Type FactType { get; }

        protected void OnGet()
        {
            if (!_cached)
            {
                // Cache the predecessor.
                _cached = true;
                PopulateCache(_subject.InternalCommunity);
            }
        }
    }
}
