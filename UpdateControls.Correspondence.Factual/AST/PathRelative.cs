using System;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class PathRelative : Path
    {
        private List<string> _segments = new List<string>();

        public IEnumerable<string> Segments
        {
            get { return _segments; }
        }

        public void AddSegment(string segment)
        {
            _segments.Add(segment);
        }
    }
}
