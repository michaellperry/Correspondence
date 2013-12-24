using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.Strategy
{
    public interface ITransform
    {
        FactID? LastFactId { get; }
        void Transform(CorrespondenceFact nextFact, FactID nextFactId, Func<CorrespondenceFact, FactID> idOfFact);
    }
}
