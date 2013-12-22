using System;

namespace UpdateControls.Correspondence.Factual.Metadata
{
    public class Field
    {
        private string _name;
        private Cardinality _cardinality;
        private NativeType _nativeType;

        public Field(string name, Cardinality cardinality, NativeType nativeType)
        {
            _name = name;
            _cardinality = cardinality;
            _nativeType = nativeType;
        }

        public string Name
        {
            get { return _name; }
        }

        public Cardinality Cardinality
        {
            get { return _cardinality; }
        }

        public NativeType NativeType
        {
            get { return _nativeType; }
        }

        public int ComputeHash()
        {
            unchecked
            {
                return (int)_nativeType * 3 + (int)_cardinality;
            }
        }
    }
}
