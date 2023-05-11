using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace SHJI.Lexer
{
    internal class Lexer
    {
        private readonly string _input;
        private int _position;
        private int _readingPosition;
        private char _ch;

        public Lexer(string input)
        {
            this._input = input;
            ReadChar();

        }
        protected void ReadChar()
        {
            if (_position >= _input.Length)
            {
                _ch = '\0';
            }
            else
            {
                _ch = _input[_readingPosition];
            }
            _position = _readingPosition++;
        }

        public Token NextToken()
        {
            Token? tok = null;
            switch (_ch)
            {
                case '=':
                    tok = new Token(TokenType.ASSIGN, _ch.ToString());
                    break;
                case ';':
                    tok = new Token(TokenType.SEMICOLON, _ch.ToString()); break;
                case '(':
                    tok = new Token(TokenType.LPAREN, _ch.ToString()); break;
                case ')':
                    tok = new Token(TokenType.RPAREN, _ch.ToString()); break;
                case '{':
                    tok = new Token(TokenType.LBRACE, _ch.ToString()); break;
                case '}':
                    break;
            }
            return tok ?? new Token();
        }
    }
}
