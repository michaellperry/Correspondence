using System;
using UpdateControls.Correspondence.Factual.AST;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class NativeTypeAtLine
    {
        private readonly NativeType _nativeType;
        private readonly int _lineNumber;

        public NativeTypeAtLine(NativeType nativeType, int lineNumber)
        {
            _nativeType = nativeType;
            _lineNumber = lineNumber;
        }

        public NativeType NativeType
        {
            get { return _nativeType; }
        }

        public int LineNumber
        {
            get { return _lineNumber; }
        }
    }
}
