using System;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class Path
    {
        private bool _absolute;
        private List<string> _segments = new List<string>();

        public Path(bool absolute)
        {
            _absolute = absolute;
        }

        public bool Absolute
        {
            get { return _absolute; }
        }

        public IEnumerable<string> Segments
        {
            get { return _segments; }
        }

        public Path AddSegment(string segment)
        {
            _segments.Add(segment);
            return this;
        }
    }
}
