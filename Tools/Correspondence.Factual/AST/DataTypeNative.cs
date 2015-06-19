using System;

namespace Correspondence.Factual.AST
{
    public class DataTypeNative : DataType
    {
        private NativeType _nativeType;

        public DataTypeNative(NativeType nativeType, Cardinality cardinality, int lineNumber)
            : base(cardinality, lineNumber)
        {
            _nativeType = nativeType;
        }         

        public NativeType NativeType
        {
            get { return _nativeType; }
        }
    }
}
