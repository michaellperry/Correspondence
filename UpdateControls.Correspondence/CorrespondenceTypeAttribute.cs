using System;

namespace UpdateControls.Correspondence
{
    public class CorrespondenceTypeAttribute : Attribute
    {
        private string _typeName = null;
        private int _version = 1;
        private bool _pivot = false;

        public CorrespondenceTypeAttribute()
        {
        }

        public CorrespondenceTypeAttribute(int version)
        {
            _version = version;
        }

        public CorrespondenceTypeAttribute(string typeName, int version)
        {
            _typeName = typeName;
            _version = version;
        }

        public string TypeName
        {
            get { return _typeName; }
        }

        public int Version
        {
            get { return _version; }
        }

        public bool Pivot
        {
            get { return _pivot; }
            set { _pivot = value; }
        }
    }
}
