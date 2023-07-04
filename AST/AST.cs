using SHJI.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
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

    internal struct LetExpression : IExpression
    {
        public Token Token { get; set; }
        public Identifier Name;
        public IExpression? Value;
        public readonly string TokenLiteral() => Token.Literal;

        public readonly override string ToString() => $"{TokenLiteral()} {Name}{(Value is null ? "" : $" = {Value}")}";
    }

    internal struct FunctionLiteral : IStatement
    {
        public Token Token { get; set; }
        public string[] Flags;
        public string Name;
        public Identifier[] Parameters;
        public string ReturnType;
        public BlockStatement Body;
        public readonly string TokenLiteral() => Token.Literal;
        public readonly override string ToString() => $"{TokenLiteral()} {(Flags.Length == 0 ? "" : Flags.Select(x => $"-{x} ").Aggregate((a, b) => a + b))}{Name}({Parameters.Select(x => x.ToString()).Aggregate((a, b) => $"{a}, {b}")}){(ReturnType is null ? "" : $" -> {ReturnType}")} {Body}";
    }

    internal struct ReturnStatement : IStatement
    {
        public Token Token { get; set; }
        public IExpression? ReturnValue;
        public readonly string TokenLiteral() => Token.Literal;

        public override readonly string ToString() => $"{TokenLiteral()} {(ReturnValue is null ? "" : ReturnValue)}";
    }

    internal struct ExpressionStatement : IStatement
    {
        public Token Token { get; set; }
        public IExpression Expression;
        public readonly string TokenLiteral() => Token.Literal;

        public override readonly string ToString() => $"{Expression}";
    }

    internal struct Identifier : IExpression
    {
        public Token Token { get; set; }
        public string Value;
        public string? Type;
        public readonly string TokenLiteral() => Token.Literal;
        public override readonly string ToString() => $"{Value}{(Type is null ? "" : $": {Type}")}";
    }

    internal struct CallExpression : IExpression
    {
        public Token Token { get; set; }
        public IExpression Function;
        public IExpression[] Arguments;

        public readonly string TokenLiteral() => Token.Literal;
        public override readonly string ToString() => $"{Function}({Arguments.Select(x => x.ToString()).Aggregate((a, b) => $"{a}, {b}")})";
    }

    internal struct IntegerLiteral : IExpression
    {
        public Token Token { get; set; }
        public int Value;

        public readonly string TokenLiteral() => Token.Literal;
        public override readonly string ToString() => TokenLiteral();
    }

    internal struct PrefixExpression : IExpression
    {
        public Token Token { get; set; }
        public string Operator;
        public IExpression Right;
        public readonly string TokenLiteral() => Token.Literal;
        public override readonly string ToString() => $"({Operator}{Right})";
    }

    internal struct InfixExpression : IExpression
    {
        public Token Token { get; set; }
        public string Operator;
        public IExpression Left;
        public IExpression Right;
        public readonly string TokenLiteral() => Token.Literal;
        public override readonly string ToString() => $"({Left} {Operator} {Right})";
    }

    internal struct PostfixExpression : IExpression
    {
        public Token Token { get; set; }
        public string Operator;
        public IExpression Left;
        public readonly string TokenLiteral() => Token.Literal;
        public override readonly string ToString() => $"({Left}{Operator})";
    }

    internal struct Boolean : IExpression
    {
        public Token Token { get; set; }
        public bool Value;
        public readonly string TokenLiteral() => Token.Literal;
        public override readonly string ToString() => TokenLiteral();
    }

    internal struct IfExpression : IExpression
    {
        public Token Token { get; set; }
        public IExpression Condition;
        public BlockStatement Cons; // If
        public BlockStatement? Alt; // Else

        public readonly string TokenLiteral() => Token.Literal;
        public override readonly string ToString() => $"if {Condition} {Cons} {(Alt is null ? "" : $"else {Alt}")}";
    }

    internal struct Ternary : IExpression
    {
        public Token Token { get; set; }
        public IExpression Condition;
        public IExpression TruePart;
        public IExpression FalsePart;
        public string TokenLiteral()
        {
            throw new NotImplementedException();
        }
        public override readonly string ToString() => $"{Condition} ? {TruePart} : {FalsePart}";
    }

    internal struct Abyss : IExpression
    {
        public Token Token { get; set; }
        public readonly string TokenLiteral() => Token.Literal;
        public override readonly string ToString() => "abyss";
        readonly string IASTNode.JOHNSerialize() => "#";
    }
}
