using SHJI.AST;
using SHJI.Lexer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SHJI.Parser
{
    internal class Parser
    {
        #region statics
        private delegate IExpression? PrefixParseFn();
        private delegate IExpression? InfixParseFn(IExpression? Left);
        private delegate IExpression? PostfixParseFn();

        private static readonly Dictionary<TokenType, OperatorPrecedence> priorities = new()
        {
            { TokenType.EQ, OperatorPrecedence.EQUALS },
            { TokenType.NOT_EQ, OperatorPrecedence.EQUALS },
            { TokenType.LT, OperatorPrecedence.COMPARE },
            { TokenType.GT, OperatorPrecedence.COMPARE },
            { TokenType.PLUS, OperatorPrecedence.SUM },
            { TokenType.MINUS, OperatorPrecedence.SUM },
            { TokenType.ASTERISK, OperatorPrecedence.PRODUCT },
            { TokenType.SLASH, OperatorPrecedence.PRODUCT },
            //{ TokenType.HAT, OperatorPrecedence.POWER },
            //{ TokenType.AND, OperatorPrecedence.COMPARE },
            //{ TokenType.OR, OperatorPrecedence.COMPARE },
            //{ TokenType.LTE, OperatorPrecedence.COMPARE },
            //{ TokenType.GTE, OperatorPrecedence.COMPARE },
            //{ TokenType.IN, OperatorPrecedence.COMPARE },
            //{ TokenType.LEFT_SHIFT, OperatorPrecedence.BITWISE },
            //{ TokenType.RIGHT_SHIFT, OperatorPrecedence.BITWISE },
            //{ TokenType.BITWISE_AND, OperatorPrecedence.BITWISE },
            //{ TokenType.BITWISE_OR, OperatorPrecedence.BITWISE },
            //{ TokenType.XOR, OperatorPrecedence.BITWISE }
        };


        private readonly Dictionary<TokenType, PrefixParseFn> UnaryParseFns;
        private readonly Dictionary<TokenType, InfixParseFn> BinaryParseFns;
        private readonly Dictionary<TokenType, PostfixParseFn> PostfixParseFns = new();

        internal readonly Tokenizer Tokenizer;
        internal static readonly IFormatProvider FormatProvider = new CultureInfo("en-US");

        Token curToken;
        Token peekToken;

        public ParserError[] Errors { get => errors.ToArray(); }
        private readonly List<ParserError> errors;
        #endregion

        public Parser(Tokenizer tokenizer)
        {
            this.Tokenizer = tokenizer;
            NextToken();
            NextToken();
            errors = new();

            UnaryParseFns = new()
            { 
                { TokenType.IDENT, ParseIdentifier },
                { TokenType.ABYSS, ParseIdentifier },
                { TokenType.INT, ParseIntegerLiteral },
                { TokenType.TRUE, ParseBoolean },
                { TokenType.FALSE, ParseBoolean },
                { TokenType.BANG, ParsePrefixExpression },
                { TokenType.MINUS, ParsePrefixExpression },
                { TokenType.LPAREN, ParseGroupedExpression },
                //{ TokenType.BITWISE_NEGATE, ParsePrefixExpression }
                //{ TokenType.INCR, ParsePrefixExpression }
                //{ TokenType.DECR, ParsePrefixExpression }
            };

            BinaryParseFns = new()
            {
                { TokenType.PLUS, ParseInfixExpression },
                { TokenType.MINUS, ParseInfixExpression },
                { TokenType.SLASH, ParseInfixExpression },
                { TokenType.ASTERISK, ParseInfixExpression },
                { TokenType.EQ, ParseInfixExpression },
                { TokenType.NOT_EQ, ParseInfixExpression },
                { TokenType.LT, ParseInfixExpression },
                { TokenType.GT, ParseInfixExpression }
            };
        }

        #region Parse Statements
        /// <summary>
        /// Parses the AST for the Program from the Tokens supplied by the Tokenizer of the Instance
        /// </summary>
        /// <returns>The AST of the Program</returns>
        public ASTRoot ParseProgram()
        {
            ASTRoot program = new();
            List<IStatement> statements = new();
            program.statements = Array.Empty<IStatement>();
            while (curToken.Type != TokenType.EOF)
            {
                IStatement? statement = ParseStatement();
                if (statement is not null)
                statements.Add(statement);
                NextToken();
            }
            program.statements = statements.ToArray();
            return program;
        }

        IStatement? ParseStatement()
        {
            return curToken.Type switch
            {
                TokenType.LET => ParseLetStatement(),
                TokenType.RETURN => ParseReturnStatement(),
                _ => ParseExpressionStatement()
            };
        }
        IStatement? ParseLetStatement()
        {
            LetStatement statement = new() { Token = curToken };
            if (!ExpectPeek(TokenType.IDENT))
            {
                PeekError(TokenType.IDENT, ParserErrorType.UnexpectedToken);
                return null;
            }

            statement.Name = new Identifier() { Token = curToken, Value = curToken.Literal };

            if (!ExpectPeek(TokenType.ASSIGN))
                return null;
            NextToken();

            IExpression? e = ParseExpression(OperatorPrecedence.LOWEST);
            if (e is null) return null;
            statement.Value = e;

            return statement;
        }

        IStatement? ParseReturnStatement()
        {
            ReturnStatement statement = new() { Token = curToken };
            NextToken();
            while (!CurTokenIs(TokenType.SEMICOLON) && !CurTokenIs(TokenType.EOL))
                NextToken();

            return statement;
        }

        IStatement? ParseExpressionStatement()
        {
            ExpressionStatement statement = new() { Token = curToken };
            IExpression? e = ParseExpression(OperatorPrecedence.LOWEST);
            if (e != null) statement.Expression = e;
            else return null;
            if (PeekTokenIs(TokenType.SEMICOLON) || PeekTokenIs(TokenType.EOL))
                NextToken();
            return statement;
        }
        #endregion

        #region Parse general Expressions
        IExpression? ParseExpression(OperatorPrecedence precedence)
        {
            if (!UnaryParseFns.TryGetValue(curToken.Type, out PrefixParseFn? prefix))
            {
                NoPrefixParseFnError(curToken.Type);
                return null;
            }
            else
            {
                IExpression? left = prefix();
                while (!PeekTokenIs(TokenType.EOL)  && !PeekTokenIs(TokenType.SEMICOLON) && precedence < PeekPrecedence())
                {
                    if (!BinaryParseFns.TryGetValue(peekToken.Type, out InfixParseFn? infix))
                    {
                        return left;
                    }
                    else
                    {
                        NextToken();
                        left = infix(left);
                    }
                }
                return left;
            }
        }
        IExpression? ParseGroupedExpression()
        {
            NextToken();
            var expr = ParseExpression(OperatorPrecedence.LOWEST);
            if (!ExpectPeek(TokenType.RPAREN)) return null;
            return expr;
        }

        IExpression? ParseIdentifier() => new Identifier() { Token = curToken, Value = curToken.Literal };
        #endregion

        #region Parse Types
        IExpression? ParseIntegerLiteral()
        {
            IntegerLiteral lit = new() { Token = curToken };
            if (!int.TryParse(curToken.Literal, FormatProvider, out lit.Value))
                return null;
            return lit;
        }

        IExpression? ParseBoolean()
        {
            AST.Boolean lit = new() { Token = curToken };
            if (!bool.TryParse(curToken.Literal, out lit.Value))
                return null;
            return lit;
        }
        #endregion

        #region Parse Operators
        IExpression? ParsePrefixExpression()
        {
            PrefixExpression prefix = new() { Token = curToken, Operator = curToken.Literal };
            NextToken();
            IExpression? e = ParseExpression(OperatorPrecedence.PREFIX);
            if (e is not null) prefix.Right = e;
            else return null;
            return prefix;
        }
        IExpression? ParseInfixExpression(IExpression? Left)
        {
            if (Left is null) return null;
            InfixExpression infix = new() { Token = curToken, Operator = curToken.Literal, Left = Left };
            OperatorPrecedence prec = CurPrecedence();
            NextToken();
            IExpression? e = ParseExpression(prec);
            if (e is not null) infix.Right = e;
            else return null;
            return infix;
        }
        #endregion

        #region Helpers

        void NextToken()
        {
            curToken = peekToken;
            peekToken = Tokenizer.NextToken();
        }
        bool CurTokenIs(TokenType type) => curToken.Type == type;
    
        bool PeekTokenIs(TokenType type) => peekToken.Type == type;
        bool ExpectPeek(TokenType type)
        {
            if (PeekTokenIs(type))
            {
                NextToken(); return true;
            }
            else return false;
        }
        
        OperatorPrecedence PeekPrecedence()
        {
            if (priorities.TryGetValue(peekToken.Type, out OperatorPrecedence prec))
            {
                return prec;
            }
            return OperatorPrecedence.LOWEST;
        }

        OperatorPrecedence CurPrecedence()
        {
            if (priorities.TryGetValue(curToken.Type, out OperatorPrecedence prec))
            {
                return prec;
            }
            return OperatorPrecedence.LOWEST;
        }

        void PeekError(TokenType expected, ParserErrorType type=ParserErrorType.Unspecified)
        {
            ParserError pe = new($"Expected next token to be {Token.ToString(expected)}, got {Token.ToString(peekToken.Type)} instead.", peekToken, type);
            errors.Add(pe);
        }

        void NoPrefixParseFnError(TokenType type)
        {
            ParserError pe = new($"No Prefix Parse Function for {type} found", curToken, ParserErrorType.NoParserFunction);
            errors.Add(pe);
        }
        #endregion
    }
    internal enum OperatorPrecedence
    {
        LOWEST = -1,
        EQUALS = 0,
        COMPARE = 1,
        BITWISE = 2,
        SUM = 3,
        PRODUCT = 4,
        PREFIX = 5,
        POWER = 6,
        CALL = 7
    }
}
