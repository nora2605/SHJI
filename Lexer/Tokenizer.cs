using System.Collections;

namespace SHJI.Lexer
{
    internal class Tokenizer : IEnumerable<Token>, IEnumerator<Token>
    {
        private readonly string input;
        private int position;
        private int readPosition;
        private char ch;

        private int line = 1;
        private int column = 0;

        private Token current;

        public Tokenizer(string input)
        {
            this.input = input;
            ReadChar();
        }
        protected void ReadChar()
        {
            if (readPosition >= input.Length)
            {
                ch = '\0';
            }
            else
            {
                ch = input[readPosition];
            }
            column++;
            position = readPosition++;
        }

        public Token NextToken()
        {
            SkipWhitespace();
            Token tok = new();
            switch (ch)
            {
                case '\n':
                    line++; column = 0;
                    tok = NewTokenLC(TokenType.EOL, "\\n"); break;
                case '=':
                    if (PeekChar() == '=')
                    {
                        ReadChar();
                        tok = NewTokenLC(TokenType.EQ, "==");
                    }
                    else if (PeekChar() == '>') {
                        ReadChar();
                        tok = NewTokenLC(TokenType.DOUBLEARROW, "=>");
                    }
                    else
                    {
                        tok = NewTokenLC(TokenType.ASSIGN, ch);
                    }
                    break;
                case '+':
                    if (PeekChar() == '+')
                    {
                        ReadChar();
                        tok = NewTokenLC(TokenType.INCREMENT, "++");
                    }
                    else tok = NewTokenLC(TokenType.PLUS, ch);
                    break;
                case ':':
                    tok = NewTokenLC(TokenType.COLON, ch); break;
                case '-':
                    if (PeekChar() == '-')
                    {
                        ReadChar();
                        tok = NewTokenLC(TokenType.DECREMENT, "--");
                    }
                    else if (PeekChar() == '>')
                    {
                        ReadChar();
                        tok = NewTokenLC(TokenType.SINGLEARROW, "->");
                    }
                    else tok = NewTokenLC(TokenType.MINUS, ch);
                    break;
                case '!':
                    if (PeekChar() == '=')
                    {
                        ReadChar();
                        tok = NewTokenLC(TokenType.NOT_EQ, "!=");
                    }
                    else tok = NewTokenLC(TokenType.BANG, ch);
                    break;
                case '/':
                    tok = NewTokenLC(TokenType.SLASH, ch);
                    break;
                case '^':
                    tok = NewTokenLC(TokenType.HAT, ch); break;
                case '*':
                    tok = NewTokenLC(TokenType.ASTERISK, ch);
                    break;
                case '<':
                    if (PeekChar() == '=')
                    {
                        ReadChar();
                        tok = NewTokenLC(TokenType.LTE, "<=");
                    }
                    else tok = NewTokenLC(TokenType.LT, ch);
                    break;
                case '>':
                    if (PeekChar() == '=')
                    {
                        ReadChar();
                        tok = NewTokenLC(TokenType.GTE, ">=");
                    }
                    else tok = NewTokenLC(TokenType.GT, ch);
                    break;
                case ';':
                    tok = NewTokenLC(TokenType.SEMICOLON, ch);
                    break;
                case ',':
                    tok = NewTokenLC(TokenType.COMMA, ch);
                    break;
                case '{':
                    tok = NewTokenLC(TokenType.LBRACE, ch);
                    break;
                case '}':
                    tok = NewTokenLC(TokenType.RBRACE, ch);
                    break;
                case '(':
                    tok = NewTokenLC(TokenType.LPAREN, ch);
                    break;
                case ')':
                    tok = NewTokenLC(TokenType.RPAREN, ch);
                    break;
                case '[':
                    tok = NewTokenLC(TokenType.LSQB, ch);
                    break;
                case ']':
                    tok = NewTokenLC(TokenType.RSQB, ch);
                    break;
                case '\0':
                    tok = NewTokenLC(TokenType.EOF, "");
                    break;
                case '"':
                    throw new NotImplementedException("No strings yet :(");
                default:
                    if (IsLetter(ch))
                    {
                        string lit = ReadWhile((ch) => IsLetter(ch) || IsDigit(ch) || ch == '_' || ch == '-' || ch == '.');
                        tok = new Token(TokenLookup.LookupIdent(lit), lit, line, column - lit.Length);
                        current = tok;
                        return tok;
                    }
                    else if (IsDigit(ch))
                    {
                        string lit = ReadWhile(IsDigit);
                        tok = new Token(TokenType.INT, lit, line, column - lit.Length);
                        current = tok;
                        return tok;
                    }
                    else
                    {
                        tok = NewTokenLC(TokenType.ILLEGAL, ch);
                    }
                    break;
            }
            ReadChar();
            current = tok;
            return tok;
        }
        private void SkipWhitespace()
        {
            while (ch == ' ' || ch == '\t' || ch == '\r')
            {
                ReadChar();
            }
        }

        private char PeekChar()
        {
            if (readPosition >= input.Length)
            {
                return '\0';
            }
            else
            {
                return input[readPosition];
            }
        }

        private string ReadWhile(Func<char, bool> Condition)
        {
            int initPosition = position;
            while (Condition(ch))
            {
                ReadChar();
            }
            return input[initPosition..position];
        }

        private static bool IsLetter(char ch) { return ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch == '_'; }
        private static bool IsDigit(char ch) { return ch >= '0' && ch <= '9'; }

        private Token NewTokenLC(TokenType Type, char ident)
        {
            return new Token(Type, ident.ToString(), line, column - 1);
        }
        private Token NewTokenLC(TokenType Type, string ident)
        {
            return new Token(Type, ident, line, column - ident.Length);
        }

        public Token Current { get => current; }
        object IEnumerator.Current => current;
        public bool MoveNext()
        {
            NextToken();
            if (current.Type != TokenType.EOF) return true;
            return false;
        }

        public void Reset()
        {
            position = 0;
            current = new Token();
            readPosition = 0;
            ch = '\0';
            line = 1;
            column = 0;
            ReadChar();
        }

        public IEnumerator<Token> GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => this;

        public void Dispose() => GC.SuppressFinalize(this);
    }
}
