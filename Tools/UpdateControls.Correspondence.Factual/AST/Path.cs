using System;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class Path
    {
        private bool _absolute;
        private string _relativeTo;
        private List<string> _segments = new List<string>();

        public Path(bool absolute, string relativeTo)
        {
            _absolute = absolute;
            _relativeTo = relativeTo;
        }

        public bool Absolute
        {
            get { return _absolute; }
        }

        public string RelativeTo
        {
            get { return _relativeTo; }
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
