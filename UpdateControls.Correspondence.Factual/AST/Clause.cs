using System;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class Clause
    {
        private int _lineNumber;
        private ConditionModifier _existence;
        private string _name;
        private string _predicateName;

        public Clause(int lineNumber, ConditionModifier existence, string name, string predicateName)
        {
            _lineNumber = lineNumber;
            _existence = existence;
            _name = name;
            _predicateName = predicateName;
        }

        public int LineNumber
        {
            get { return _lineNumber; }
        }

        public ConditionModifier Existence
        {
            get { return _existence; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string PredicateName
        {
            get { return _predicateName; }
        }
    }
}
