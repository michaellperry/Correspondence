using UpdateControls.Correspondence.Mementos;
using System.Collections.Generic;
using System;
using System.Linq;

namespace UpdateControls.Correspondence
{
	/// <summary>
	/// </summary>
	public class PredecessorObj<TFact> : PredecessorBase
        where TFact : CorrespondenceFact
	{
		private RoleMemento _role;
        private FactID _factId;
		private TFact _fact;

		public PredecessorObj(
			CorrespondenceFact subject,
			Role<TFact> role,
			TFact obj ) :
            base(subject, true)
		{
            if (obj == null)
                throw new ArgumentException(string.Format("Predecessor {0} cannot be null.", role));

			_role = role.RoleMemento;
            _factId = obj.ID;
			_fact = obj;

            subject.SetPredecessor(_role, this);
		}

		public PredecessorObj(
			CorrespondenceFact subject,
			Role<TFact> role,
			FactMemento memento ) :
            base(subject, false)
		{
			_role = role.RoleMemento;

            List<FactID> facts = memento.GetPredecessorIdsByRole(_role).ToList();
            if (facts.Count < 1)
                throw new CorrespondenceException(string.Format("A fact was loaded with no predecessor in role {0}.", role));
            if (facts.Count > 1)
                throw new CorrespondenceException(string.Format("A fact was loaded with more than one predecessor in role {0}.", role));
            _factId = facts[0];
            subject.SetPredecessor(_role, this);
		}

		public TFact Fact
		{
            get { OnGet(); return _fact; }
		}

        internal override IEnumerable<FactID> InternalFactIds
        {
            get { yield return _factId; }
        }

        protected override void PopulateCache(Community community)
        {
            // Resovle the ID to an object.
            _fact = (TFact)community.GetFactByID(_factId);
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
