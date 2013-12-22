using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls.Correspondence.Factual.Metadata
{
    public class Predecessor
    {
        private string _name;
        private Cardinality _cardinality;
        private string _factType;
		private bool _isPivot;
        private List<Condition> _publishConditions = new List<Condition>();

        public Predecessor(string name, Cardinality cardinality, string factType, bool isPivot)
        {
            _name = name;
            _cardinality = cardinality;
            _factType = factType;
            _isPivot = isPivot;
        }

        public string Name
        {
            get { return _name; }
        }

        public Cardinality Cardinality
        {
            get { return _cardinality; }
        }

        public string FactType
        {
            get { return _factType; }
        }

		public bool IsPivot
		{
			get { return _isPivot; }
		}

        public IEnumerable<Condition> PublishConditions
        {
            get { return _publishConditions; }
        }

        public void AddPublishCondition(Condition publishCondition)
        {
            _publishConditions.Add(publishCondition);
        }

        public int ComputeHash()
        {
            unchecked
            {
                return (Crc32.GetHashOfString(_name) * 3 + (int)_cardinality) * 2 +
                    (_isPivot ? 1 : 0);
            }
        }
	}
}
