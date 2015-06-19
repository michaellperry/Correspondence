using Correspondence.Mementos;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Assisticant.Fields;

namespace Correspondence
{
    public class PredecessorOpt<TFact> : PredecessorBase
        where TFact : CorrespondenceFact
    {
        private RoleMemento _role;
        private FactID _factId;
		private Observable<TFact> _fact;
        private Community _community;
        private TaskCompletionSource<CorrespondenceFact> _loaded;

		public PredecessorOpt(
			CorrespondenceFact subject,
			Role role,
			TFact obj ) :
            base(subject, true)
		{
            if (!obj.IsNull && obj.InternalCommunity == null)
                throw new CorrespondenceException("A fact's predecessors must be added to the community first.");

			_role = role.RoleMemento;
            _factId = obj.IsNull ? new FactID() : obj.ID;
			_fact = new Observable<TFact>(obj);
            _community = obj.IsNull ? null : obj.InternalCommunity;

            subject.SetPredecessor(_role, this);
		}

        public PredecessorOpt(
            CorrespondenceFact subject,
            Role role,
            FactMemento memento,
            Func<TFact> getUnloaded,
            Func<TFact> getNullInstance) :
            base(subject, false)
        {
            _role = role.RoleMemento;
            _fact = new Observable<TFact>(getUnloaded());
            _loaded = new TaskCompletionSource<CorrespondenceFact>();
            _fact.Value.SetLoadedTask(_loaded.Task);

            if (memento != null)
            {
                List<FactID> facts = memento.GetPredecessorIdsByRole(_role).ToList();
                if (facts.Count > 1)
                    throw new CorrespondenceException(string.Format("A fact was loaded with more than one predecessor in role {0}.", role));
                if (facts.Count == 1)
                    _factId = facts[0];
                else
                    _factId = new FactID();

                if (_factId.key == 0)
                {
                    _fact.Value = getNullInstance();
                    _loaded.SetResult(_fact.Value);
                    _loaded = null;
                }
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
            get
            {
                if (_factId.key != 0)
                    yield return _factId;
            }
        }

        protected override async Task PopulateCacheAsync(Community community)
        {
            if (_factId.key != 0)
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
}
