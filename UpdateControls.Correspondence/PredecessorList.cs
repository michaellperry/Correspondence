using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence
{
    public class PredecessorList<FactType> : PredecessorBase, IEnumerable<FactType>
        where FactType : CorrespondenceFact
    {
		private RoleMemento _role;
        private List<FactID> _factIds;
		private List<FactType> _facts;

		public PredecessorList(
			CorrespondenceFact subject,
			Role<FactType> role,
			IEnumerable<FactType> facts ) :
            base(subject, true)
		{
            if (facts.Any(o => o == null))
                throw new ArgumentException(string.Format("Predecessor list {0} cannot contain a null.", role));
            
            _role = role.RoleMemento;
			_facts = facts.ToList();
            _factIds = _facts.Select(o => o.ID).ToList();

			subject.SetPredecessor( _role, this );
		}

		public PredecessorList(
			CorrespondenceFact subject,
			Role<FactType> role,
			Memento memento ) :
            base(subject, false)
		{
			_role = role.RoleMemento;

            _factIds = memento.GetPredecessorIdsByRole(_role).ToList();
			subject.SetPredecessor( _role, this );
		}

        #region IEnumerable<FactType> Members

        public IEnumerator<FactType> GetEnumerator()
        {
            OnGet();
            return _facts.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            OnGet();
            return _facts.GetEnumerator();
        }

        #endregion

        internal override IEnumerable<FactID> InternalFactIds
        {
            get { return _factIds; }
        }

        protected override void PopulateCache(Community community)
        {
            // Resolve each ID to an object.
            _facts = _factIds
                .Select(id => community.GetFactByID(id))
                .Cast<FactType>()
                .ToList();
        }

        protected override void EmptyCache()
        {
            _facts = null;
        }
    }
}
