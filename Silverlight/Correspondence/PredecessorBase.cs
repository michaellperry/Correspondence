using System.Collections.Generic;
using System;
using Correspondence.Mementos;

namespace Correspondence
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
            if (!_cached && _subject.InternalCommunity != null)
            {
                // Cache the predecessor.
                _cached = true;
                PopulateCache(_subject.InternalCommunity);
            }
        }
    }
}
