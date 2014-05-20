using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls.Correspondence.Factual.Metadata
{
    public class Segment
    {
        private readonly string _name;
        private readonly string _type;
        
        public Segment(string name, string type)
        {
            _name = name;
            _type = type;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Type
        {
            get { return _type; }
        }
    }
}
