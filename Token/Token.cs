using System;
using System.Collections.Generic;

namespace SHJI
{
    public enum TokenType
    {
        ILLEGAL,
        EOF,
        IDENT,
        INT,
        ASSIGN,
        PLUS,
        COLON,
        MINUS,
        BANG,
        ASTERISK,
        SLASH,
        LT,
        GT,
        EQ,
        NOT_EQ,
        COMMA,
        SEMICOLON,
        LPAREN,
        RPAREN,
        LBRACE,
        RBRACE,
        FUNCTION,
        LET,
        TRUE,
        FALSE,
        IF,
        ELSE,
        RETURN
    }

    public struct Token
    {
        public TokenType Type { get; set; }
        public string Literal { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }

        public Token(TokenType Type, string Literal)
        {
            this.Type = Type;
            this.Literal = Literal;
        }
        public Token(TokenType Type, string Literal, int Line, int Column) : this(Type, Literal)
        {
            this.Line = Line;
            this.Column = Column;
        }
    }

    public static class TokenConstants
    {
        public const string ILLEGAL = "ILLEGAL";
        public const string EOF = "EOF";
        public const string IDENT = "IDENT";
        public const string INT = "INT";
        public const string ASSIGN = "=";
        public const string PLUS = "+";
        public const string MINUS = "-";
        public const string BANG = "!";
        public const string ASTERISK = "*";
        public const string COLON = ":";
        public const string SLASH = "/";
        public const string LT = "<";
        public const string GT = ">";
        public const string EQ = "==";
        public const string NOT_EQ = "!=";
        public const string COMMA = ",";
        public const string SEMICOLON = ";";
        public const string LPAREN = "(";
        public const string RPAREN = ")";
        public const string LBRACE = "{";
        public const string RBRACE = "}";
        public const string FUNCTION = "FUNCTION";
        public const string LET = "LET";
        public const string TRUE = "TRUE";
        public const string FALSE = "FALSE";
        public const string IF = "IF";
        public const string ELSE = "ELSE";
        public const string RETURN = "RETURN";
    }

    public static class TokenLookup
    {
        private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
        {
            { "fn", TokenType.FUNCTION },
            { "let", TokenType.LET },
            { "true", TokenType.TRUE },
            { "false", TokenType.FALSE },
            { "if", TokenType.IF },
            { "else", TokenType.ELSE },
            { "return", TokenType.RETURN }
        };

        public static TokenType LookupIdent(string ident)
        {
            if (keywords.TryGetValue(ident, out TokenType tok))
            {
                return tok;
            }
            return TokenType.IDENT;
        }
    }
}