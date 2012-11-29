using UpdateControls.Correspondence.Queries;

namespace UpdateControls.Correspondence.Conditions
{
    public class Clause
    {
		private bool _isEmpty;
		private QueryDefinition _subQuery;

        internal Clause(bool isEmpty, Query subQuery)
		{
			_isEmpty = isEmpty;
			_subQuery = subQuery.QueryDefinition;
		}

        public bool IsEmpty
        {
            get { return _isEmpty; }
        }

        public QueryDefinition SubQuery
        {
            get { return _subQuery; }
        }

        public string ToString(string prior)
        {
            return (_isEmpty ? "empty " : "not empty ") + _subQuery.ToString(prior);
        }

        public override string ToString()
        {
            return ToString("this");
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            Clause that = obj as Clause;
            if (that == null)
                return false;
            return this._isEmpty.Equals(that._isEmpty) && this._subQuery.Equals(that._subQuery);
        }

        public override int GetHashCode()
        {
            return _subQuery.GetHashCode() * 2 + (_isEmpty ? 1 : 0);
        }
    }
}
