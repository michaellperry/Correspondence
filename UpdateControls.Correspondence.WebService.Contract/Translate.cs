using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.WebService.Contract
{
    public static class Translate
    {
        public static FactTreeMemento FactTreeToMemento(FactTree tree)
        {
            FactTreeMemento memento = new FactTreeMemento(tree.DatabaseId);
            foreach (Fact fact in tree.Facts)
                memento.Add(FactToMemento(fact));
            return memento;
        }

        public static IdentifiedFactMemento FactToMemento(Fact fact)
        {
            return new IdentifiedFactMemento(
                LongToFactID(fact.FactId),
                FactToMemento(fact.FactType, fact.Data, fact.Predecessors));
        }

        public static FactMemento FactToMemento(FactType factType, byte[] data, List<Predecessor> predecessors)
        {
            FactMemento memento = new FactMemento(FactTypeToMemento(factType));
            memento.Data = data;
            memento.AddPredecessors(predecessors.Select(predecessor => PredecessorToMemento(predecessor)));
            return memento;
        }

        public static CorrespondenceFactType FactTypeToMemento(FactType factType)
        {
            return new CorrespondenceFactType(factType.TypeName, factType.Version);
        }

        private static PredecessorMemento PredecessorToMemento(Predecessor predecessor)
        {
            return new PredecessorMemento(
                new RoleMemento(
                    FactTypeToMemento(predecessor.DeclaringType),
                    predecessor.RoleName,
                    FactTypeToMemento(predecessor.TargetType)),
                LongToFactID(predecessor.PredecessorId)
            );
        }

        public static FactID LongToFactID(long id)
        {
            return new FactID() { key = id };
        }

        public static FactTree MementoToFactTree(FactTreeMemento memento)
        {
            FactTree root = new FactTree()
            {
                DatabaseId = memento.DatabaseId,
                Facts = memento.Facts
                    .Select(fact => MementoToFact(fact))
                    .ToList()
            };
            return root;
        }

        public static Fact MementoToFact(IdentifiedFactMemento memento)
        {
            return new Fact()
            {
                FactId = memento.Id.key,
                FactType = new FactType()
                {
                    TypeName = memento.Memento.FactType.TypeName,
                    Version = memento.Memento.FactType.Version
                },
                Data = memento.Memento.Data,
                Predecessors = memento.Memento.Predecessors
                    .Select(predecessor => MementoToPredecessor(predecessor))
                    .ToList()
            };
        }

        public static Predecessor MementoToPredecessor(PredecessorMemento memento)
        {
            return new Predecessor()
            {
                DeclaringType = MementoToType(memento.Role.DeclaringType),
                RoleName = memento.Role.RoleName,
                TargetType = MementoToType(memento.Role.TargetType),
                PredecessorId = memento.ID.key
            };
        }

        public static FactType MementoToType(CorrespondenceFactType memento)
        {
            if (memento == null)
                return new FactType();

            return new FactType()
            {
                TypeName = memento.TypeName,
                Version = memento.Version
            };
        }
    }
}
