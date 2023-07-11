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
        public Token Token { get;  set; }

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
        public readonly override string ToString() => $"{Token.Literal} {Name}{(Value is null ? "" : $" = {Value}")}";
    }

    internal struct ArrayLiteral : IExpression
    {
        public Token Token { get; set; }
        public IExpression[] Elements;
        public readonly override string ToString() => $"[{(Elements.Length == 0 ? "" : Elements.Select(x => x.ToString()).Aggregate((a, b) => $"{a} {b}"))}]";
    }

    internal struct IndexingExpression : IExpression
    {
        public Token Token { get; set; }
        public IExpression Indexee;
        public IExpression Index;

        public readonly override string ToString() => $"{Indexee}[{Index}]";
    }

    internal struct FunctionLiteral : IStatement
    {
        public Token Token { get; set; }
        public string[] Flags;
        public string Name;
        public Identifier[] Parameters;
        public string ReturnType;
        public BlockStatement Body;
        public readonly override string ToString() => $"{Token.Literal} {(Flags.Length == 0 ? "" : Flags.Select(x => x.Length == 1 ? $"-{x} " : $"--{x} ").Aggregate((a, b) => a + b))}{Name}({Parameters.Select(x => x.ToString()).Aggregate((a, b) => $"{a}, {b}")}){(ReturnType is null ? "" : $" -> {ReturnType}")} {Body}";
    }

    internal struct ReturnStatement : IStatement
    {
        public Token Token { get; set; }
        public IExpression? ReturnValue;

        public override readonly string ToString() => $"{Token.Literal} {(ReturnValue is null ? "" : ReturnValue)}";
    }

    internal struct ExpressionStatement : IStatement
    {
        public Token Token { get; set; }
        public IExpression Expression;

        public override readonly string ToString() => $"{Expression}";
    }

    internal struct Identifier : IExpression
    {
        public Token Token { get; set; }
        public string Value;
        public string? Type;
        public override readonly string ToString() => $"{Value}{(Type is null ? "" : $": {Type}")}";
    }

    internal struct CallExpression : IExpression
    {
        public Token Token { get; set; }
        public IExpression Function;
        public IExpression[] Arguments;

        public override readonly string ToString() => $"{Function}({Arguments.Select(x => x.ToString()).Aggregate((a, b) => $"{a}, {b}")})";
    }

    internal struct IntegerLiteral : IExpression, INumberLiteral
    {
        public Token Token { get; set; }
        public int Value;
        public string? ImmediateCoalescion { get; set; }
        public override readonly string ToString() => $"{Token.Literal}{(ImmediateCoalescion is null ? "" : ImmediateCoalescion)}";
    }

    // Gets parsed only if the supplied integer is too long
    // pun not intended
    internal struct LongLiteral : IExpression, INumberLiteral
    {
        public Token Token { get; set; }
        public long Value;
        public string? ImmediateCoalescion { get; set; }
        public override readonly string ToString() => $"{Token.Literal}{(ImmediateCoalescion is null ? "" : ImmediateCoalescion)}";
    }

    internal struct Int128Literal : IExpression, INumberLiteral
    {
        public Token Token { get; set; }
        public Int128 Value;
        public string? ImmediateCoalescion { get; set; }
        public override readonly string ToString() => $"{Token.Literal}{(ImmediateCoalescion is null ? "" : ImmediateCoalescion)}";
    }

    internal struct UInt128Literal : IExpression, INumberLiteral
    {
        public Token Token { get; set; }
        public UInt128 Value;
        public string? ImmediateCoalescion { get; set; }
        public override readonly string ToString() => $"{Token.Literal}{(ImmediateCoalescion is null ? "" : ImmediateCoalescion)}";
    }
    // Double by default, can be made into f32 by attaching f or f32
    internal struct FloatLiteral : IExpression, INumberLiteral
    {
        public Token Token { get; set; }
        public double Value;
        public string? ImmediateCoalescion { get; set; }

        public override readonly string ToString() => $"{Token.Literal}{(ImmediateCoalescion is null ? "" : ImmediateCoalescion)}";
    }

    internal struct PrefixExpression : IExpression
    {
        public Token Token { get; set; }
        public string Operator;
        public IExpression Right;
        public override readonly string ToString() => $"({Operator}{Right})";
    }

    internal struct InfixExpression : IExpression
    {
        public Token Token { get; set; }
        public string Operator;
        public IExpression Left;
        public IExpression Right;
        public override readonly string ToString() => $"({Left} {Operator} {Right})";
    }

    internal struct PostfixExpression : IExpression
    {
        public Token Token { get; set; }
        public string Operator;
        public IExpression Left;
        public override readonly string ToString() => $"({Left}{Operator})";
    }

    internal struct Boolean : IExpression
    {
        public Token Token { get; set; }
        public bool Value;
        public override readonly string ToString() => Token.Literal;
    }

    internal struct IfExpression : IExpression
    {
        public Token Token { get; set; }
        public IExpression Condition;
        public BlockStatement Cons; // If
        public BlockStatement? Alt; // Else

        public override readonly string ToString() => $"if {Condition} {Cons} {(Alt is null ? "" : $"else {Alt}")}";
    }

    internal struct Assignment : IExpression
    {
        public Token Token { get; set; }
        public Identifier Name;
        public IExpression Value;
        public override readonly string ToString() => $"{Name} = {Value}";
    }

    internal struct ForLoop : IStatement
    {
        public Token Token { get; set; }
        public Identifier Iterator;
        public IExpression Enumerable;
        public BlockStatement LoopContent;

        public override readonly string ToString() => $"for let {Iterator} in {Enumerable} {LoopContent}";
    }

    internal struct CharLiteral : IExpression
    {
        public Token Token { get; set; }
        public string Value; // Needs to be able to contain escape sequences until Interpretation
        public override readonly string ToString() => $"'{Value}'";
    }

    internal struct RawStringLiteral : IExpression
    {
        public Token Token { get; set; }
        public string EscapedValue;

        public override readonly string ToString() => "raw\"" + EscapedValue + "\"";
    }

    internal struct VerbatimStringLiteral : IExpression
    {
        public Token Token { get; set; }
        public string EscapedValue;
        public override readonly string ToString() => $"@\"{EscapedValue}\"";
    }

    internal struct InterpolatedVerbatimStringLiteral : IExpression
    {
        public Token Token { get; set; }
        public IExpression[] Expressions;
        public override readonly string ToString() => $"@$\"{(Expressions.Length > 0 ? Expressions.Select(x => x.ToString()).Aggregate((a, b) => a + b) : "")}\"";
    }

    internal struct InterpolatedStringLiteral : IExpression
    {
        public Token Token { get; set; }
        public IExpression[] Expressions;
        public override readonly string ToString() => $"\"{(Expressions.Length > 0 ? Expressions.Select(x => x.ToString()).Aggregate((a, b) => a + b) : "")}\"";
    }

    internal struct StringContent : IExpression
    {
        public Token Token { get; set; }
        public string EscapedValue;
        public override readonly string ToString() => EscapedValue;
    }

    internal struct Interpolation : IExpression
    {
        public Token Token { get; set; }
        public IExpression Content;
        public override readonly string ToString() => "${" + Content.ToString() + "}";
    }

    internal struct Abyss : IExpression
    {
        public Token Token { get; set; }
        public override readonly string ToString() => "abyss";
        readonly string IASTNode.JOHNSerialize() => "#";
    }
}
