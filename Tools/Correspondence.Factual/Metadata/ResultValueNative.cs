using System;

namespace Correspondence.Factual.Metadata
{
    public class ResultValueNative : ResultValue
    {
        private NativeType _nativeType;

        public ResultValueNative(string type, Query query, Cardinality cardinality, NativeType nativeType)
            : base(type, query, cardinality)
        {
            _nativeType = nativeType;
        }

        public NativeType NativeType
        {
            get { return _nativeType; }
        }
    }
}
