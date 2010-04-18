using System;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class Set
    {
        private int _lineNumber;
        private string _name;
        private string _factName;
        private Path _leftPath;
        private Path _rightPath;
        private Condition _condition;

        public Set(string name, string factName, Path leftPath, Path rightPath, Condition condition, int lineNumber)
        {
            _lineNumber = lineNumber;
            _name = name;
            _factName = factName;
            _leftPath = leftPath;
            _rightPath = rightPath;
            _condition = condition;
        }

        public int LineNumber
        {
            get { return _lineNumber; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string FactName
        {
            get { return _factName; }
        }

        public Path LeftPath
        {
            get { return _leftPath; }
        }

        public Path RightPath
        {
            get { return _rightPath; }
        }

        public Condition Condition
        {
            get { return _condition; }
        }
    }
}
