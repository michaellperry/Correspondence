using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls.Correspondence.Factual.Metadata
{
    public class Class
    {
        private string _name;
        private List<Field> _fields = new List<Field>();

        public Class(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public IEnumerable<Field> Fields
        {
            get { return _fields; }
        }

        public Class AddField(Field field)
        {
            _fields.Add(field);
            return this;
        }
    }
}
