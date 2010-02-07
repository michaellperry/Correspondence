using System;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.WebService.Contract
{
    public static class Translate
    {
        public static FactTreeMemento FactTreeToMemento(FactTree tree)
        {
            ContractToMementoTranslator translator = new ContractToMementoTranslator(tree.DatabaseId);
            foreach (FactType type in tree.Types)
                translator.AddFactType(type);
            foreach (FactRole role in tree.Roles)
                translator.AddRole(role);
            foreach (Fact fact in tree.Facts)
                translator.AddFact(fact);
            return translator.TargetFactTree;
        }

        public static FactTree MementoToFactTree(FactTreeMemento memento)
        {
            var translator = new MementoToContractTranslator(memento.DatabaseId);
            foreach (IdentifiedFactMemento fact in memento.Facts)
                translator.AddFact(fact);
            return translator.TargetFactTree;
        }
    }
}
