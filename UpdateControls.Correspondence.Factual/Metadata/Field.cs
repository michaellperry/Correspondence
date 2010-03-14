using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls.Correspondence.Factual.Metadata
{
    public class Field
    {
        private string _name;
        private Cardinality _cardinality;
        private NativeType _dataType;

        public Field(string name, Cardinality cardinality, NativeType dataType)
        {
            _name = name;
            _cardinality = cardinality;
            _dataType = dataType;
        }

        public string Name
        {
            get { return _name; }
        }

        public Cardinality Cardinality
        {
            get { return _cardinality; }
        }

        public NativeType DataType
        {
            get { return _dataType; }
        }
    }
}
