using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace UpdateControls.Correspondence.Factual.Compiler
{
    public class Lexer
    {
        private static Regex IdentifierExpression = new Regex("^[a-zA-Z_][a-zA-Z0-9_]*$");

        private TextReader _input;
        private Dictionary<string, Symbol> _keywords = new Dictionary<string, Symbol>();
        private Dictionary<string, Symbol> _punctuation = new Dictionary<string, Symbol>();

        private int _lineNumber = 1;

        public Lexer(TextReader input)
        {
            _input = input;
        }

        public Token NextToken()
        {
            int nextCharacter = _input.Peek();
            while (nextCharacter == ' ' || nextCharacter == '\t' || nextCharacter == '\r' || nextCharacter == '\n')
            {
                if (nextCharacter == '\n')
                    ++_lineNumber;
                _input.Read();
                nextCharacter = _input.Peek();
            }

            if (nextCharacter == -1)
                return new Token(Symbol.EndOfFile, string.Empty, _lineNumber);

            else if (
                ('a' <= nextCharacter && nextCharacter <= 'z') ||
                ('A' <= nextCharacter && nextCharacter <= 'Z') ||
                nextCharacter == '_'
                )
            {
                StringBuilder identifier = new StringBuilder();
                identifier.Append((char)_input.Read());
                nextCharacter = _input.Peek();
                while (
                    ('a' <= nextCharacter && nextCharacter <= 'z') ||
                    ('A' <= nextCharacter && nextCharacter <= 'Z') ||
                    ('0' <= nextCharacter && nextCharacter <= '9') ||
                    nextCharacter == '_'
                    )
                {
                    identifier.Append((char)_input.Read());
                    nextCharacter = _input.Peek();
                }
                Symbol keywordSymbol;
                if (_keywords.TryGetValue(identifier.ToString(), out keywordSymbol))
                    return new Token(keywordSymbol, identifier.ToString(), _lineNumber);
                else
                    return new Token(Symbol.Identifier, identifier.ToString(), _lineNumber);
            }

            else
            {
                string punctuationText = ((char)_input.Read()).ToString();
                Symbol punctuationSymbol;
                if (_punctuation.TryGetValue(punctuationText, out punctuationSymbol))
                    return new Token(punctuationSymbol, punctuationText, _lineNumber);
                else
                    throw new FactualException(string.Format("Unknown symbol {0}.", punctuationText), _lineNumber);
            }
        }

        public Lexer AddSymbol(string text, Symbol symbol)
        {
            if (IdentifierExpression.IsMatch(text))
                _keywords.Add(text, symbol);
            else if (text.Length == 1)
                _punctuation.Add(text, symbol);
            else
                throw new Exception("The lexer can only handle keywords and single character punctuation.");
            return this;
        }
    }
}
