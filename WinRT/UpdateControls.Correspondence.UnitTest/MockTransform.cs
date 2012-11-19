using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls.Correspondence.Strategy;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.UnitTest
{
    public class MockTransform : ITransform
    {
        private FactID? _lastFactId;
        private List<CorrespondenceFact> _facts = new List<CorrespondenceFact>();

        public FactID? LastFactId
        {
            get { return _lastFactId; }
        }

        public void Transform(CorrespondenceFact nextFact, FactID nextFactId, Func<CorrespondenceFact, FactID> idOfFact)
        {
            _facts.Add(nextFact);
            _lastFactId = nextFactId;
        }

        public List<CorrespondenceFact> Facts
        {
            get { return _facts; }
        }
    }
}
