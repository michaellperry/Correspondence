using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Mementos;
using System.Collections.ObjectModel;

namespace UpdateControls.Correspondence.WebServiceClient
{
    public class MementoToContractTranslator
    {
        private FactTree _targetFactTree;

        private Dictionary<CorrespondenceFactType, int> _idByFactType = new Dictionary<CorrespondenceFactType, int>();
        private Dictionary<RoleMemento, int> _idByRole = new Dictionary<RoleMemento, int>();

        public MementoToContractTranslator(long databaseId)
        {
            _targetFactTree = new FactTree()
            {
                DatabaseId = databaseId,
                Facts = new ObservableCollection<Fact>(),
                Roles = new ObservableCollection<FactRole>(),
                Types = new ObservableCollection<FactType>()
            };
        }

        public void AddFact(IdentifiedFactMemento fact)
        {
            Fact newFact = new Fact()
            {
                FactId = fact.Id.key,
                FactTypeId = AddFactType(fact.Memento.FactType),
                Data = fact.Memento.Data,
                Predecessors = new ObservableCollection<Predecessor>()
            };
            foreach (PredecessorMemento predecessor in fact.Memento.Predecessors)
                newFact.Predecessors.Add(TranslatePredecessor(predecessor));
            _targetFactTree.Facts.Add(newFact);
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
                    TargetTypeId = AddFactType(role.TargetType),
                    IsPivot = role.IsPivot
                });
                _idByRole.Add(role, id);
            }

            return id;
        }
    }
}
