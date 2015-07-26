using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Correspondence.Factual.AST
{
    public class Alias
    {
        private readonly string _identifier;

        public Alias(string identifier)
        {
            _identifier = identifier;
        }

        public string Identifier
        {
            get { return _identifier; }
        }
    }
}
