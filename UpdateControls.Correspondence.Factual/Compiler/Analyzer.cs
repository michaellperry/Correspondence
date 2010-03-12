using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M = UpdateControls.Correspondence.Factual.Metadata;
using A = UpdateControls.Correspondence.Factual.AST;

namespace UpdateControls.Correspondence.Factual.Compiler
{
    public class Analyzer
    {
        private A.Namespace _root;

        public Analyzer(A.Namespace root)
        {
            _root = root;
        }

        public M.Namespace Analyze()
        {
            M.Namespace result = new M.Namespace(_root.Identifier);
            foreach (A.Fact fact in _root.Facts)
            {
                result.AddClass(new M.Class(fact.Name));
            }
            return result;
        }
    }
}
