using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.POXClient.Contract;

namespace UpdateControls.Correspondence.POXClient
{
    public class MementoToContractTranslator
    {
        private FactTree _targetFactTree;

        private Dictionary<CorrespondenceFactType, int> _idByFactType = new Dictionary<CorrespondenceFactType, int>();
        private Dictionary<RoleMemento, int> _idByRole = new Dictionary<RoleMemento, int>();
		private List<FactType> _types = new List<FactType>();
		private List<FactRole> _roles = new List<FactRole>();
		private List<Fact> _facts = new List<Fact>();

        public MementoToContractTranslator(long databaseId)
        {
            _targetFactTree = new FactTree()
            {
                DatabaseId = databaseId
            };
        }

        public void AddFact(IdentifiedFactMemento fact)
        {
            _facts.Add(new Fact()
            {
                FactId = fact.Id.key,
                FactTypeId = AddFactType(fact.Memento.FactType),
                Data = fact.Memento.Data,
                Predecessors = fact.Memento.Predecessors
                    .Select(predecessor => TranslatePredecessor(predecessor))
                    .ToArray()
            });
        }

		public void Finish()
		{
			_targetFactTree.Types = _types.ToArray();
			_targetFactTree.Roles = _roles.ToArray();
			_targetFactTree.Facts = _facts.ToArray();
		}

        public FactTree TargetFactTree
        {
            get { return _targetFactTree; }
        }

        private Predecessor TranslatePredecessor(PredecessorMemento predecessorMemento)
        {
            return new Predecessor()
            {
                PredecessorId = predecessorMemento.ID.key,
                RoleId = AddRole(predecessorMemento.Role),
                IsPivot = predecessorMemento.IsPivot
            };
        }

        private int AddFactType(CorrespondenceFactType factType)
        {
            int id;
            if (!_idByFactType.TryGetValue(factType, out id))
            {
                id = _types.Count;
                _types.Add(new FactType()
                {
                    TypeId = id,
                    TypeName = factType.TypeName,
                    Version = factType.Version
                });
                _idByFactType.Add(factType, id);
            }

            return id;
        }

        private int AddRole(RoleMemento role)
        {
            int id;
            if (!_idByRole.TryGetValue(role, out id))
            {
                id = _roles.Count;
                _roles.Add(new FactRole()
                {
                    RoleId = id,
                    RoleName = role.RoleName,
                    DeclaringTypeId = AddFactType(role.DeclaringType),
                    IsPivot = role.IsPivot
                });
                _idByRole.Add(role, id);
            }

            return id;
        }
    }
}
