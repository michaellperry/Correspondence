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

        public Predecessor(string name, Cardinality cardinality, string factType)
        {
            _name = name;
            _cardinality = cardinality;
            _factType = factType;
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
    }
}
