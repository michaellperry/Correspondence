using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls.Correspondence.Factual.Metadata
{
    public class Class
    {
        private string _name;

        public Class(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }
}
