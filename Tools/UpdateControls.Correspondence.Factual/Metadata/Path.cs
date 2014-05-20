using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls.Correspondence.Factual.Metadata
{
    public class Path
    {
        private List<Segment> _segments = new List<Segment>();

        public IEnumerable<Segment> Segments
        {
            get { return _segments; }
        }

        public void AddSegment(string name, string type)
        {
            _segments.Add(new Segment(name, type));
        }
    }
}
