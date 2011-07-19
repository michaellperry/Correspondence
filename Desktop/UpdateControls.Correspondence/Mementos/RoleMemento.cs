
namespace UpdateControls.Correspondence.Mementos
{
    public class RoleMemento
    {
        private CorrespondenceFactType _declaringType;
        private string _roleName;
        private CorrespondenceFactType _targetType;
        private bool _isPivot;

        public RoleMemento(CorrespondenceFactType declaringType, string roleName, CorrespondenceFactType targetType, bool isPivot)
        {
            _declaringType = declaringType;
            _roleName = roleName;
            _targetType = targetType;
            _isPivot = isPivot;
        }

        public CorrespondenceFactType DeclaringType
        {
            get { return _declaringType; }
        }

        public string RoleName
        {
            get { return _roleName; }
        }

        public CorrespondenceFactType TargetType
        {
            get { return _targetType; }
        }

        public bool IsPivot
        {
            get { return _isPivot; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != this.GetType())
                return false;
            RoleMemento that = (RoleMemento)obj;
            return
                this._declaringType.Equals(that._declaringType) &&
                this._roleName == that._roleName;
        }

        public override int GetHashCode()
        {
            int hash = _declaringType.GetHashCode();
            hash ^= Crc32.GetHashOfString(_roleName);
            return hash;
        }

        public override string ToString()
        {
            return string.Format("{0}.{1} = {2}", _declaringType, _roleName, _targetType);
        }
    }
}
