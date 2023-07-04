using SHJI.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
#if DEBUG
using System.Reflection;
#endif

namespace SHJI.AST
{
    internal interface IStatement : IASTNode
    {
        public Token Token { get; set; }
    }
    internal interface IExpression : IASTNode
    {
        public Token Token { get; set; }
    }

    internal struct BlockStatement : IASTNode
    {
        public IStatement[] Statements;
        public Token Token;

        public BlockStatement(IStatement single)
        {
            Statements = new[] { single };    
        }

        public readonly string TokenLiteral() => Token.Literal;
        public override readonly string ToString() => $"{{{(Statements.Length == 0 ? "" : Statements.Select(s => "\n\t" + s.ToString()).Aggregate((a, b) => a + b))}\n}}";
    }

    internal interface IASTNode
    {
        public string TokenLiteral();
        public string ToString() => TokenLiteral();

        public string JOHNSerialize() {
            string output = "{";
#if DEBUG
            Type t = GetType();
            var fs = t.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var f in fs)
            {
                string? repr = (f.GetValue(this) as IASTNode)?.JOHNSerialize();
                string? arrrepr = null;
                if (repr is not null) goto Outputter;
                IASTNode[]? arr = f.GetValue(this) as IASTNode[];
                if (arr is not null && arr.Length != 0)
                    arrrepr = "[" + 
                        (f.GetValue(this) as IASTNode[])?
                            .Select(x => " " + x.JOHNSerialize())
                            .Aggregate((a, b) => a + b)
                        + "]";
                Outputter:
                output += $"{f.Name} {repr ?? arrrepr ?? $"\"{f.GetValue(this)}\""} ";
            }
#endif
            return output[..^1] + "}";
        }
    }
}
