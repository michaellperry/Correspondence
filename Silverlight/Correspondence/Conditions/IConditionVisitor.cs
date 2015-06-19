using System;
using Correspondence.Queries;

namespace Correspondence.Conditions
{
    public interface IConditionVisitor
    {
        void VisitAnd(Condition left, Condition right);
        void VisitSimple(bool isEmpty, QueryDefinition subQuery);
    }
}
