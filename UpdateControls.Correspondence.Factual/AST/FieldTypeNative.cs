using System;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class FieldTypeNative : FieldType
    {
        private NativeType _nativeType;

        public FieldTypeNative(NativeType nativeType, Cardinality cardinality)
            : base(cardinality)
        {
            _nativeType = nativeType;
        }         

        public NativeType NativeType
        {
            get { return _nativeType; }
        }
    }
}
