using SHJI.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SHJI.AST
{
    internal interface IStatement : IASTNode
    {
    }
    internal interface IExpression : IASTNode
    {
    }

    internal interface IASTNode
    {
        public string TokenLiteral();
        public string ToString() => TokenLiteral();

        public string JOHNSerialize() {
            string output = "{";
            Type t = GetType();
            var fs = t.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var f in fs)
            {
                output += $"{f.Name} {(
                    (f.GetValue(this) as IASTNode)?.JOHNSerialize() ?? $"\"{f.GetValue(this)}\""
                )} ";
            }
            return output[..^1] + "}";
        }
    }
}
