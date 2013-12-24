using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.Queries
{
    public class Unpublisher
    {
        private QueryDefinition _messageFacts;
        private Condition _publishCondition;
        private RoleMemento _role;

        public Unpublisher(QueryDefinition messageFacts, Condition publishCondition, RoleMemento role)
        {
            _messageFacts = messageFacts;
            _publishCondition = publishCondition;
            _role = role;
        }

        public QueryDefinition MessageFacts
        {
            get { return _messageFacts; }
        }

        public Condition PublishCondition
        {
            get { return _publishCondition; }
        }

        public RoleMemento Role
        {
            get { return _role; }
        }
    }
}
