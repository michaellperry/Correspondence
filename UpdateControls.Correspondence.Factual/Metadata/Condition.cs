using System;

namespace UpdateControls.Correspondence.Factual.Metadata
{
    public class Condition
    {
        private ConditionModifier _modifier;
        private Query _query;

        public Condition(ConditionModifier modifier, Query query)
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
