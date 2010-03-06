using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class Identifier
    {
        private int _lineNumber;
        private string _text;

        public Identifier(int lineNumber, string text)
        {
            _lineNumber = lineNumber;
            _text = text;
        }
    }
}
