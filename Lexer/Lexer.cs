using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace SHJI
{
    internal class Lexer
    {
        private readonly string input;
        private int position;
        private int readPosition;
        private char ch;

        private int line = 1;
        private int column = 0;

        public Lexer(string input)
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
            if (ch == '\n')
            {
                line++;
                column = 0;
            }
            position = readPosition++;
        }

        public Token NextToken()
        {
            SkipWhitespace();
            Token tok = new Token();
            switch (ch)
            {
                case '=':
                    if (PeekChar() == '=')
                    {
                        char cc = ch;
                        ReadChar();
                        string Literal = cc.ToString() + ch.ToString();
                        tok = newTokWithLC(TokenType.EQ, Literal);
                    }
                    else
                    {
                        tok = newTokWithLC(TokenType.ASSIGN, ch);
                    }
                    break;
                case '+':
                    tok = newTokWithLC(TokenType.PLUS, ch); break;
                case ':':
                    tok = newTokWithLC(TokenType.COLON, ch); break;
                case '-':
                    tok = newTokWithLC(TokenType.MINUS, ch); break;
                case '!':
                    if (PeekChar() == '=')
                    {
                        char cc = ch;
                        ReadChar();
                        string literal = cc.ToString() + ch.ToString();
                        tok = newTokWithLC(TokenType.NOT_EQ, literal);
                    }
                    else
                    {
                        tok = newTokWithLC(TokenType.BANG, ch);
                    }
                    break;
                case '/':
                    tok = newTokWithLC(TokenType.SLASH, ch);
                    break;
                case '*':
                    tok = newTokWithLC(TokenType.ASTERISK, ch);
                    break;
                case '<':
                    tok = newTokWithLC(TokenType.LT, ch);
                    break;
                case '>':
                    tok = newTokWithLC(TokenType.GT, ch);
                    break;
                case ';':
                    tok = newTokWithLC(TokenType.SEMICOLON, ch);
                    break;
                case ',':
                    tok = newTokWithLC(TokenType.COMMA, ch);
                    break;
                case '{':
                    tok = newTokWithLC(TokenType.LBRACE, ch);
                    break;
                case '}':
                    tok = newTokWithLC(TokenType.RBRACE, ch);
                    break;
                case '(':
                    tok = newTokWithLC(TokenType.LPAREN, ch);
                    break;
                case ')':
                    tok = newTokWithLC(TokenType.RPAREN, ch);
                    break;
                case '\0':
                    tok = newTokWithLC(TokenType.EOF, "");
                    break;
                default:
                    if (IsLetter(ch))
                    {
                        string lit = ReadWhile((ch) => IsLetter(ch) || IsDigit(ch));
                        tok = new Token(TokenLookup.LookupIdent(lit), lit, line, column - lit.Length);
                        return tok;
                    }
                    else if (IsDigit(ch))
                    {
                        string lit = ReadWhile(IsDigit);
                        tok = new Token(TokenType.INT, lit, line, column - lit.Length);
                        return tok;
                    }
                    else
                    {
                        tok = newTokWithLC(TokenType.ILLEGAL, ch);
                    }
                    break;
            }
            ReadChar();

            return tok;
        }
        private void SkipWhitespace()
        {
            while (ch == ' ' || ch == '\t' || ch == '\n' || ch == '\r')
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

        private bool IsLetter(char ch) { return ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch == '_'; }
        private bool IsDigit(char ch) { return ch >= '0' && ch <= '9'; }

        private Token newTokWithLC(TokenType Type, char ident)
        {
            return new Token(Type, ident.ToString(), line, column);
        }
        private Token newTokWithLC(TokenType Type, string ident)
        {
            return new Token(Type, ident, line, column);
        }
    }
}
