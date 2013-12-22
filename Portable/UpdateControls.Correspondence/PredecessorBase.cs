using System.Collections.Generic;
using System;
using UpdateControls.Correspondence.Mementos;
using System.Threading.Tasks;

namespace UpdateControls.Correspondence
{
    public abstract class PredecessorBase
    {
        private enum State
        {
            Unloaded,
            Loading,
            Loaded
        }

        private CorrespondenceFact _subject;
        private State _state;

        protected PredecessorBase(CorrespondenceFact subject, bool cached)
        {
            _subject = subject;
            _state = cached ? State.Loaded : State.Unloaded;
        }

        internal abstract Community Community { get; }
        internal abstract IEnumerable<FactID> InternalFactIds { get; }
        protected abstract Task PopulateCacheAsync(Community community);

        protected async void OnGet()
        {
            if (_state == State.Unloaded && _subject.InternalCommunity != null)
            {
                _state = State.Loading;
                // Cache the predecessor.
                await PopulateCacheAsync(_subject.InternalCommunity);
                _state = State.Loaded;
            }
        }
    }
}
