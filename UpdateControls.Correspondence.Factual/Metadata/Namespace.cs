using System;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.Factual.Metadata
{
    public class Namespace
    {
        private string _name;
        private List<Class> _classes = new List<Class>();

        public Namespace(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public IEnumerable<Class> Classes
        {
            get { return _classes; }
        }

        public void AddClass(Class newClass)
        {
            _classes.Add(newClass);
        }
    }
}
