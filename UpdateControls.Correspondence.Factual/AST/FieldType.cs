using System;

namespace UpdateControls.Correspondence.Factual.AST
{
    public abstract class FieldType
    {
        private Cardinality _cardinality;

        public FieldType(Cardinality cardinality)
        {
            _cardinality = cardinality;
        }

        public Cardinality Cardinality
        {
            get { return _cardinality; }
        }
    }
}
