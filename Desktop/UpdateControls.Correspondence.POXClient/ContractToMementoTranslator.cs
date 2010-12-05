using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.POXClient.Contract;

namespace UpdateControls.Correspondence.POXClient
{
    public class ContractToMementoTranslator
    {
        private FactTreeMemento _targetFactTree;

        private Dictionary<int, CorrespondenceFactType> _typeById = new Dictionary<int, CorrespondenceFactType>();
        private Dictionary<int, RoleMemento> _roleById = new Dictionary<int, RoleMemento>();

        public ContractToMementoTranslator(long databaseId)
        {
            _targetFactTree = new FactTreeMemento(databaseId);
        }

        public void AddFactType(FactType factType)
        {
            _typeById.Add(factType.TypeId, new CorrespondenceFactType(factType.TypeName, factType.Version));
        }

        public void AddRole(FactRole role)
        {
            CorrespondenceFactType declaringType = GetFactType(role.DeclaringTypeId);
            CorrespondenceFactType targetType = role.TargetTypeId == 0 ? null : GetFactType(role.TargetTypeId);
            _roleById.Add(role.RoleId, new RoleMemento(declaringType, role.RoleName, targetType, role.IsPivot));
        }

        public void AddFact(Fact fact)
        {
            FactMemento memento = new FactMemento(GetFactType(fact.FactTypeId));
            memento.Data = fact.Data;
            memento.AddPredecessors(fact.Predecessors.Select(predecessor => PredecessorToMemento(predecessor)));

            _targetFactTree.Add(
                new IdentifiedFactMemento(
                    new FactID() { key = fact.FactId },
                    memento));
        }

        private PredecessorMemento PredecessorToMemento(Predecessor predecessor)
        {
            return new PredecessorMemento(
                GetRole(predecessor.RoleId),
                new FactID() { key = predecessor.PredecessorId }
            );
        }

        public FactTreeMemento TargetFactTree
        {
            get { return _targetFactTree; }
        }

        private CorrespondenceFactType GetFactType(int factTypeId)
        {
            CorrespondenceFactType factType;
            if (!_typeById.TryGetValue(factTypeId, out factType))
                throw new CorrespondenceException(string.Format("Fact type {0} was not provided.", factTypeId));

            return factType;
        }

        private RoleMemento GetRole(int roleId)
        {
            RoleMemento role;
            if (!_roleById.TryGetValue(roleId, out role))
                throw new CorrespondenceException(string.Format("Role {0} was not provided.", roleId));

            return role;
        }
    }
}
