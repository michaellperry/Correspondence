using UpdateControls.Correspondence.Mementos;
using System.Collections.Generic;
using System;
using System.Linq;

namespace UpdateControls.Correspondence
{
    public class PredecessorOpt<TFact> : PredecessorBase
        where TFact : CorrespondenceFact
    {
		private RoleMemento _role;
        private FactID _factId;
		private TFact _fact;

		public PredecessorOpt(
			CorrespondenceFact subject,
			Role<TFact> role,
			TFact obj ) :
            base(subject, true)
		{
			_role = role.RoleMemento;
            _factId = obj == null ? new FactID() : obj.ID;
			_fact = obj;

            subject.SetPredecessor(_role, this);
		}

        public PredecessorOpt(
            CorrespondenceFact subject,
            Role<TFact> role,
            FactMemento memento) :
            base(subject, false)
        {
            _role = role.RoleMemento;

            List<FactID> facts = memento.GetPredecessorIdsByRole(_role).ToList();
            if (facts.Count > 1)
                throw new CorrespondenceException(string.Format("A fact was loaded with more than one predecessor in role {0}.", role));
            if (facts.Count == 1)
                _factId = facts[0];
            else
                _factId = new FactID();
            subject.SetPredecessor(_role, this);
        }

		public TFact Fact
		{
            get { OnGet(); return _fact; }
		}

        internal override IEnumerable<FactID> InternalFactIds
        {
            get
            {
                if (_factId.key != 0)
                    yield return _factId;
            }
        }

        protected override void PopulateCache(Community community)
        {
            if (_factId.key != 0)
            {
                // Resovle the ID to an object.
                _fact = (TFact)community.GetFactByID(_factId);
            }
        }

        protected override void EmptyCache()
        {
            _fact = null;
        }

        internal override Type FactType
        {
            get { return typeof(TFact); }
        }
	}
}
