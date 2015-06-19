using System;
using System.Collections.Generic;

namespace Correspondence.Factual.Metadata
{
    public class Analyzed
    {
        private string _name;
        private List<Class> _classes = new List<Class>();

        public Analyzed(string name)
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
