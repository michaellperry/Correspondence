using System.Collections.Generic;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class FactSection
    {
        private bool _unique;
        private bool _lock;
        private bool _principal;
        private Path _toPath;
        private Path _fromPath;
        private List<FactMember> _members = new List<FactMember>();

        public FactSection AddMember(FactMember keyMember)
        {
            _members.Add(keyMember);
            return this;
        }

        public FactSection SetUnique()
        {
            _unique = true;
            return this;
        }

        public FactSection SetLock()
        {
            _lock = true;
            return this;
        }

        public FactSection SetPrincipal()
        {
            _principal = true;
            return this;
        }

        public FactSection SetToPath(Path path)
        {
            _toPath = path;
            return this;
        }

        public FactSection SetFromPath(Path path)
        {
            _fromPath = path;
            return this;
        }

        public Fact AddTo(Fact fact)
        {
            foreach (FactMember member in _members)
                fact.AddMember(member);
            if (_unique)
                fact.Unique = true;
            if (_lock)
                fact.Lock = true;
            if (_principal)
                fact.Principal = true;
            if (_toPath != null)
                fact.ToPath = _toPath;
            if (_fromPath != null)
                fact.FromPath = _fromPath;
            return fact;
        }
    }
}
