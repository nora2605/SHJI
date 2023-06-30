using SHJI.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SHJI.AST
{
    internal struct ASTRoot : IASTNode
    {
        public IStatement[] statements;

        public readonly string TokenLiteral()
        {
            if (statements.Length > 0)
                return statements[0].TokenLiteral();
            else
                return "";
        }

        public override readonly string ToString()
        {
            string output = "";
            foreach (var s in statements)
            {
                output += s.ToString() + "\n";
            }

            return output;
        }

        public readonly string JOHNSerialize()
        {
            string output = "[\n";
            foreach (var s in statements)
            {
                output += "\t" + s.JOHNSerialize() + "\n";
            }
            return output + "]";
        }
    }

    internal struct LetStatement : IStatement
    {
        public Token Token;
        public Identifier Name;
        public IExpression Value;
        public readonly string TokenLiteral() => Token.Literal;

        public readonly override string ToString() => $"{TokenLiteral()} {Name} = {Value}";
    }

    internal struct ReturnStatement : IStatement
    {
        public Token Token;
        public IExpression ReturnValue;
        public readonly string TokenLiteral() => Token.Literal;

        public override readonly string ToString() => $"{TokenLiteral()} {ReturnValue}";
    }

    internal struct ExpressionStatement : IStatement
    {
        public Token Token;
        public IExpression Expression;
        public readonly string TokenLiteral() => Token.Literal;

        public override readonly string ToString() => $"{Expression}";
    }

    internal struct Identifier : IExpression
    {
        public Token Token;
        public string Value;
        public readonly string TokenLiteral() => Token.Literal;
        public override readonly string ToString() => Value;
    }

    internal struct IntegerLiteral : IExpression
    {
        public Token Token;
        public int Value;

        public readonly string TokenLiteral() => Token.Literal;
        public override readonly string ToString() => TokenLiteral();
    }

    internal struct PrefixExpression : IExpression
    {
        public Token Token;
        public string Operator;
        public IExpression Right;
        public readonly string TokenLiteral() => Token.Literal;
        public override readonly string ToString() => $"({Operator}{Right})";
    }

    internal struct InfixExpression : IExpression
    {
        public Token Token;
        public string Operator;
        public IExpression Left;
        public IExpression Right;
        public readonly string TokenLiteral() => Token.Literal;
        public override readonly string ToString() => $"({Left} {Operator} {Right})";
    }

    internal struct Boolean : IExpression
    {
        public Token Token;
        public bool Value;
        public readonly string TokenLiteral() => Token.Literal;
        public override readonly string ToString() => TokenLiteral();
    }

    internal struct Ternary : IExpression
    {
        public IExpression Condition;
        public IExpression TruePart;
        public IExpression FalsePart;
        public string TokenLiteral()
        {
            throw new NotImplementedException();
        }
        public override readonly string ToString() => $"{Condition} ? {TruePart} : {FalsePart}";
    }
}
