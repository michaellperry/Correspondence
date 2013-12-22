using UpdateControls.Correspondence.Mementos;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using UpdateControls.Fields;

namespace UpdateControls.Correspondence
{
	/// <summary>
	/// </summary>
	public class PredecessorObj<TFact> : PredecessorBase
        where TFact : CorrespondenceFact
	{
		private RoleMemento _role;
        private FactID _factId;
		private Independent<TFact> _fact;
        private Community _community;
        private TaskCompletionSource<CorrespondenceFact> _loaded;

		public PredecessorObj(
			CorrespondenceFact subject,
			Role role,
			TFact obj ) :
            base(subject, true)
		{
            if (obj == null)
                throw new ArgumentException(string.Format("Predecessor {0} cannot be null.", role));
            if (obj.InternalCommunity == null)
                throw new CorrespondenceException("A fact's predecessors must be added to the community first.");

            _community = obj.InternalCommunity;

			_role = role.RoleMemento;
            _factId = obj.ID;
            _fact = new Independent<TFact>(obj);

            subject.SetPredecessor(_role, this);
		}

		public PredecessorObj(
			CorrespondenceFact subject,
			Role role,
			FactMemento memento,
            Func<TFact> getUnloaded,
            Func<TFact> getNullInstance) :
            base(subject, false)
		{
			_role = role.RoleMemento;
            _fact = new Independent<TFact>(getUnloaded());
            _loaded = new TaskCompletionSource<CorrespondenceFact>();
            _fact.Value.SetLoadedTask(_loaded.Task);

            if (memento != null)
            {
                List<FactID> facts = memento.GetPredecessorIdsByRole(_role).ToList();
                if (facts.Count < 1)
                    throw new CorrespondenceException(string.Format("A fact was loaded with no predecessor in role {0}.", role));
                if (facts.Count > 1)
                    throw new CorrespondenceException(string.Format("A fact was loaded with more than one predecessor in role {0}.", role));
                _factId = facts[0];
            }
            subject.SetPredecessor(_role, this);
        }

        internal override Community Community
        {
            get { return _community; }
        }

        public TFact Fact
        {
            get
            {
                lock (this)
                {
                    OnGet();
                    return _fact;
                }
            }
		}

        internal override IEnumerable<FactID> InternalFactIds
        {
            get { yield return _factId; }
        }

        protected override async Task PopulateCacheAsync(Community community)
        {
            // Resovle the ID to an object.
            TFact fact = await community.GetFactByIDAsync(_factId) as TFact;
            lock (this)
            {
                _fact.Value = fact;
                if (_loaded != null)
                {
                    _loaded.SetResult(fact);
                    _loaded = null;
                }
            }
        }
	}
}
