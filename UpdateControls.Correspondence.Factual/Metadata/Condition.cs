using System;

namespace UpdateControls.Correspondence.Factual.Metadata
{
    public class Condition
    {
        private ConditionModifier _modifier;
        private string _name;

        public Condition(ConditionModifier modifier, string name)
        {
            _modifier = modifier;
            _name = name;
        }

        public ConditionModifier Modifier
        {
            get { return _modifier; }
        }

        public string Name
        {
            get { return _name; }
        }
    }
}