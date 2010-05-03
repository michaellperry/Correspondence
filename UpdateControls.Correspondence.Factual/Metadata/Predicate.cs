using System;

namespace UpdateControls.Correspondence.Factual.Metadata
{
    public class Predicate
    {
        private ConditionModifier _modifier;
        private Query _query;

        public Predicate(ConditionModifier modifier, Query query)
        {
            _modifier = modifier;
            _query = query;
        }

        public ConditionModifier Modifier
        {
            get { return _modifier; }
        }

        public Query Query
        {
            get { return _query; }
        }
    }
}
