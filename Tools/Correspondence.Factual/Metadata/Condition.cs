using System;

namespace Correspondence.Factual.Metadata
{
    public class Condition
    {
        private ConditionModifier _modifier;
        private string _name;
        private string _type;

        public Condition(ConditionModifier modifier, string name, string type)
        {
            _modifier = modifier;
            _name = name;
            _type = type;
        }

        public ConditionModifier Modifier
        {
            get { return _modifier; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string Type
        {
            get { return _type; }
        }
    }
}
