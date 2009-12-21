using System;
using UpdateControls.Correspondence.Queries;

namespace UpdateControls.Correspondence.Conditions
{
    public interface IConditionVisitor
    {
        void VisitAnd(Condition left, Condition right);
        void VisitSimple(bool isEmpty, QueryDefinition subQuery);
    }
}
