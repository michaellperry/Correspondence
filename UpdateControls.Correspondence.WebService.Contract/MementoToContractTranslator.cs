using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.WebService.Contract
{
    public class MementoToContractTranslator
    {
        private FactTree _targetFactTree;

        private Dictionary<CorrespondenceFactType, int> _idByFactType = new Dictionary<CorrespondenceFactType, int>();
        private Dictionary<RoleMemento, int> _idByRole = new Dictionary<RoleMemento, int>();

        public MementoToContractTranslator(long databaseId, long timestampID)
        {
            _targetFactTree = new FactTree()
            {
                DatabaseId = databaseId,
                Types = new List<FactType>(),
                Roles = new List<FactRole>(),
                Facts = new List<Fact>(),
                Timestamp = timestampID
            };
        }

        public void AddFact(IdentifiedFactMemento fact)
        {
            _targetFactTree.Facts.Add(new Fact()
            {
                FactId = fact.Id.key,
                FactTypeId = AddFactType(fact.Memento.FactType),
                Data = fact.Memento.Data,
                Predecessors = fact.Memento.Predecessors
                    .Select(predecessor => TranslatePredecessor(predecessor))
                    .ToList()
            });
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
                RoleId = AddRole(predecessorMemento.Role)
            };
        }

        private int AddFactType(CorrespondenceFactType factType)
        {
            int id;
            if (!_idByFactType.TryGetValue(factType, out id))
            {
                id = _targetFactTree.Types.Count;
                _targetFactTree.Types.Add(new FactType()
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
                id = _targetFactTree.Roles.Count;
                _targetFactTree.Roles.Add(new FactRole()
                {
                    RoleId = id,
                    RoleName = role.RoleName,
                    DeclaringTypeId = AddFactType(role.DeclaringType),
                    TargetTypeId = role.TargetType == null ? 0 : AddFactType(role.TargetType),
                    IsPivot = role.IsPivot
                });
                _idByRole.Add(role, id);
            }

            return id;
        }
    }
}
