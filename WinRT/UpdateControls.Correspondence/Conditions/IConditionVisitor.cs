using System;
using System.Threading.Tasks;
using UpdateControls.Correspondence.Queries;

namespace UpdateControls.Correspondence.Conditions
{
    public interface IConditionVisitor
    {
        Task VisitAndAsync(Condition left, Condition right);
        Task VisitSimpleAsync(bool isEmpty, QueryDefinition subQuery);
    }
}
