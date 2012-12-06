using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace QEDCode.LLOne
{
    public class Lexer<TSymbol>
    {
        private static Regex IdentifierExpression = new Regex("^[a-zA-Z_][a-zA-Z0-9_]*$");

        private TextReader _input;
        private TSymbol _identifier;
        private TSymbol _endOfFile;
        private Dictionary<string, TSymbol> _keywords = new Dictionary<string, TSymbol>();
        private Dictionary<string, TSymbol> _punctuation = new Dictionary<string, TSymbol>();

        private int _lineNumber = 1;

        public Lexer(TextReader input, TSymbol identifier, TSymbol endOfFile)
        {
            _input = input;
            _identifier = identifier;
            _endOfFile = endOfFile;
        }

        public Token<TSymbol> NextToken()
        {
            int nextCharacter = _input.Peek();
            if (nextCharacter == '\n')
                ++_lineNumber;

            SkipWhitespace(ref nextCharacter);
            if (nextCharacter == -1)
                return new Token<TSymbol>(_endOfFile, string.Empty, _lineNumber);

            if (
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
                TSymbol keywordSymbol;
                if (_keywords.TryGetValue(identifier.ToString(), out keywordSymbol))
                    return new Token<TSymbol>(keywordSymbol, identifier.ToString(), _lineNumber);
                else
                    return new Token<TSymbol>(_identifier, identifier.ToString(), _lineNumber);
            }

            string punctuationText = ((char)_input.Read()).ToString();
            TSymbol punctuationSymbol;
            if (_punctuation.TryGetValue(punctuationText, out punctuationSymbol))
                return new Token<TSymbol>(punctuationSymbol, punctuationText, _lineNumber);
            else
                throw new ParserException(string.Format("Unknown symbol {0}.", punctuationText), _lineNumber);
        }

        private void SkipWhitespace(ref int nextCharacter)
        {
            while (true)
            {
                while (nextCharacter == ' ' || nextCharacter == '\t' || nextCharacter == '\r' || nextCharacter == '\n')
                {
                    nextCharacter = Consume();
                }

                if (nextCharacter == '/')
                {
                    nextCharacter = Consume();
                    if (nextCharacter == '/')
                    {
                        // Single line comment.
                        do
                        {
                            nextCharacter = Consume();
                        } while (nextCharacter != '\n');
                    }
                    else if (nextCharacter == '*')
                    {
                        // Multi line comment.
                        do
                        {
                            do
                            {
                                nextCharacter = Consume();
                            } while (nextCharacter != '*');
                            nextCharacter = Consume();
                        } while (nextCharacter != '/');
                    }
                }
                else
                    break;
            }
        }

        private int Consume()
        {
            _input.Read();
            int nextCharacter = _input.Peek();
            if (nextCharacter == '\n')
                ++_lineNumber;
            return nextCharacter;
        }

        public Lexer<TSymbol> AddSymbol(string text, TSymbol symbol)
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
