
namespace UpdateControls.Correspondence.Mementos
{
    public class CorrespondenceFactType
    {
        private string _typeName;
        private int _version;

        public CorrespondenceFactType(string typeName, int version)
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

        public override int GetHashCode()
        {
            return _typeName.GetHashCode() * 37 + _version;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != this.GetType())
                return false;
            CorrespondenceFactType that = (CorrespondenceFactType)obj;
            return
                this._typeName == that._typeName &&
                this._version == that._version;
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}", _typeName, _version);
        }
    }
}
