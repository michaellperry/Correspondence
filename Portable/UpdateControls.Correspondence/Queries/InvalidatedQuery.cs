
namespace UpdateControls.Correspondence.Queries
{
    public class InvalidatedQuery
    {
        private CorrespondenceFact _targetObject;
        private QueryDefinition _invalidQuery;

        public InvalidatedQuery(CorrespondenceFact targetObject, QueryDefinition invalidQuery)
        {
            _targetObject = targetObject;
            _invalidQuery = invalidQuery;
        }

        public void Invalidate()
        {
            _targetObject.InvalidateQuery(_invalidQuery);
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            InvalidatedQuery that = obj as InvalidatedQuery;
            if (that == null)
                return false;
            return _targetObject.Equals(that._targetObject) && _invalidQuery.Equals(_invalidQuery);
        }

        public override int GetHashCode()
        {
            return _invalidQuery.GetHashCode() * 37 + _targetObject.GetHashCode();
        }
    }
}
