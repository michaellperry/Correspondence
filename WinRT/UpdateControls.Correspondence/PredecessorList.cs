using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Fields;

namespace UpdateControls.Correspondence
{
    public class PredecessorList<TFact> : PredecessorBase, IEnumerable<TFact>
        where TFact : CorrespondenceFact
    {
		private RoleMemento _role;
        private List<FactID> _factIds;
		private Independent<List<TFact>> _facts;
        private Community _community;
        private TaskCompletionSource<PredecessorList<TFact>> _loaded;

		public PredecessorList(
			CorrespondenceFact subject,
			Role role,
			IEnumerable<TFact> facts ) :
            base(subject, true)
		{
            if (facts.Any(o => o == null))
                throw new ArgumentException(string.Format("Predecessor list {0} cannot contain a null.", role));

            if (facts.Any(o => o.InternalCommunity == null))
                throw new CorrespondenceException("A fact's predecessors must be added to the community first.");

            _community = facts.Select(fact => fact.InternalCommunity).FirstOrDefault();

			if (facts.Any(o => o.InternalCommunity != _community))
				throw new CorrespondenceException("A fact cannot be added to a different community than its predecessors.");
            
            _role = role.RoleMemento;
			_facts = new Independent<List<TFact>>(facts.ToList());
            _factIds = _facts.Value.Select(o => o.ID).ToList();

			subject.SetPredecessor( _role, this );
		}

		public PredecessorList(
			CorrespondenceFact subject,
			Role role,
            FactMemento memento,
            TFact unloaded,
            TFact nullInstance) :
            base(subject, false)
		{
			_role = role.RoleMemento;
            _facts = new Independent<List<TFact>>(new List<TFact>());
            _loaded = new TaskCompletionSource<PredecessorList<TFact>>();

            if (memento != null)
            {
                _factIds = memento.GetPredecessorIdsByRole(_role).ToList();
            }
            subject.SetPredecessor(_role, this);
        }

        public Task<PredecessorList<TFact>> EnsureAsync()
        {
            lock (this)
            {
                if (_loaded != null)
                    return _loaded.Task;
                else
                    return Task.FromResult(this);
            }
        }

        internal override Community Community
        {
            get { return _community; }
        }

        #region IEnumerable<FactType> Members

        public IEnumerator<TFact> GetEnumerator()
        {
            lock (this)
            {
                OnGet();
                return _facts.Value.GetEnumerator();
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            lock (this)
            {
                OnGet();
                return _facts.Value.GetEnumerator();
            }
        }

        #endregion

        internal override IEnumerable<FactID> InternalFactIds
        {
            get { return _factIds; }
        }

        protected override async Task PopulateCacheAsync(Community community)
        {
            // Resolve each ID to an object.
            List<TFact> facts = new List<TFact>();
            foreach (var id in _factIds)
            {
                TFact fact = await community.GetFactByIDAsync(id) as TFact;
                facts.Add(fact);
            }
            lock (this)
            {
                _facts.Value = facts;
                if (_loaded != null)
                {
                    _loaded.SetResult(this);
                    _loaded = null;
                }
            }
        }
    }
}
