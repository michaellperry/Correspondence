using System;

namespace UpdateControls.Correspondence
{
    public class CorrespondenceFieldAttribute : Attribute
    {
        private int _minVersion = 1;

        public CorrespondenceFieldAttribute()
        {
        }

        public CorrespondenceFieldAttribute(int minVersion)
        {
            _minVersion = minVersion;
        }

        public int MinVersion
        {
            get { return _minVersion; }
        }
    }
}
