using System;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.Factual.Metadata
{
    public class Join
    {
        private Direction _direction;
        private string _type;
        private string _name;
        private List<Condition> _conditions = new List<Condition>();

        public Join(Direction direction, string type, string name)
        {
            _direction = direction;
            _type = type;
            _name = name;
        }

        public Direction Direction
        {
            get { return _direction; }
        }

        public string Type
        {
            get { return _type; }
        }

        public string Name
        {
            get { return _name; }
        }

        public IEnumerable<Condition> Conditions
        {
            get { return _conditions; }
        }

        public Join AddCondition(Condition condition)
        {
            _conditions.Add(condition);
            return this;
        }
    }
}
