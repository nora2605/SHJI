using SHJI.AST;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Numerics;
using static SHJI.Interpreter.IJaneObject;

namespace SHJI.Interpreter
{
    internal static class Interpreter
    {
        internal static IJaneObject Eval(IASTNode node)
        {
            IJaneObject result = node switch
            {
                // Statements
                ASTRoot => EvalStatements(((ASTRoot)node).statements),
                ExpressionStatement => Eval(((ExpressionStatement)node).Expression),
                // Expressions
                IntegerLiteral => new JaneInt() { Value = ((IntegerLiteral)node).Value },
                AST.Boolean => ((AST.Boolean)node).Value ? JANE_TRUE : JANE_FALSE,
                PrefixExpression => EvalPrefixExpression((PrefixExpression)node),
                InfixExpression => EvalInfixExpression((InfixExpression)node),
                _ => JANE_ABYSS,
            };
            return result;
        }

        internal static IJaneObject EvalStatements(IStatement[] stmts)
        {
            IJaneObject result = JANE_ABYSS;
            foreach (IStatement stmt in stmts)
            {
                result = Eval(stmt);
            }
            return result;
        }

        internal static IJaneObject EvalPrefixExpression(PrefixExpression prefixExpression)
        {
            IJaneObject right = Eval(prefixExpression.Right);
            return prefixExpression.Operator switch
            {
                "!" => EvalBangPrefixOperator(right),
                "-" => EvalMinusPrefixOperator(right),
                _ => JANE_ABYSS,
            };
        }

        internal static IJaneObject EvalInfixExpression(InfixExpression e)
        {
            IJaneObject left = Eval(e.Left);
            IJaneObject right = Eval(e.Right);

            return e.Operator switch
            {
                "+" => EvalPlusInfix(left, right),
                "-" => EvalMinusInfix(left, right),
                "/" => EvalSlashInfix(left, right),
                "*" => EvalAsteriskInfix(left, right),
                "^" => EvalHatInfix(left, right),
                //">" => EvalGTInfix(left, right),
                //">=" => EvalGTEInfix(left, right),
                //"<" => EvalLTInfix(left, right),
                //"<=" => EvalLTEInfix(left, right),
                //"==" => EvalEQInfix(left, right),
                //"!=" => EvalNOT_EQInfix(left, right),
                //"~" => EvalConcatInfix(left, right),
                _ => throw new RuntimeError($"Operator {e.Operator} is unknown or not implemented"),
            } ;
        }
        #region Operator hell
        internal static IJaneObject EvalPlusInfix(IJaneObject left, IJaneObject right)
        {
            // Nasty
            if (left is JaneInt oLeftInt)
            {
                if (right is JaneInt oRightInt) return new JaneInt { Value = oLeftInt.Value + oRightInt.Value };
                // Soon to be added more? Maybe I'll find a better way
                // or just use like a static Jane definition file for all operators
                // probably not here but for the compiler definitely
                // because like
                // one will be able to declare custom operators eventually
                // and if those are stored in some way i might as well store my shit someway
            }
            throw new RuntimeError($"Operator + not implemented for operands of type {left.Type()} and {right.Type()}");
        }
        internal static IJaneObject EvalMinusInfix(IJaneObject left, IJaneObject right)
        {
            if (left is JaneInt oLeftInt)
            {
                if (right is JaneInt oRightInt) return new JaneInt { Value = oLeftInt.Value - oRightInt.Value };
            }
            throw new RuntimeError($"Operator - not implemented for operands of type {left.Type()} and {right.Type()}");
        }
        internal static IJaneObject EvalSlashInfix(IJaneObject left, IJaneObject right)
        {
            if (left is JaneInt oLeftInt)
            {
                if (right is JaneInt oRightInt) return new JaneInt { Value = oLeftInt.Value / oRightInt.Value };
            }
            throw new RuntimeError($"Operator / not implemented for operands of type {left.Type()} and {right.Type()}");
        }
        internal static IJaneObject EvalAsteriskInfix(IJaneObject left, IJaneObject right)
        {
            if (left is JaneInt oLeftInt)
            {
                if (right is JaneInt oRightInt) return new JaneInt { Value = oLeftInt.Value * oRightInt.Value };
            }
            throw new RuntimeError($"Operator * not implemented for operands of type {left.Type()} and {right.Type()}");
        }

        internal static IJaneObject EvalHatInfix(IJaneObject left, IJaneObject right)
        {
            if (left is JaneInt oLeftInt)
            {
                if (right is JaneInt oRightInt) return new JaneInt128 { Value = IntPow(oLeftInt.Value, oRightInt.Value) };
            }
            throw new RuntimeError($"Operator ^ not implemented for operands of type {left.Type()} and {right.Type()}");
        }

        internal static Int128 IntPow(int @base, int exponent)
        {
            if (exponent < 0) return @base == 1 ? 1 : 0;
            else if (exponent == 0) return 1;
            else return @base * IntPow(@base, exponent - 1);
        }

        internal static IJaneObject EvalBangPrefixOperator(IJaneObject e)
        {
            if (e == JANE_TRUE) return JANE_FALSE;
            else if (e == JANE_FALSE) return JANE_TRUE;
            else if (e == JANE_ABYSS) return JANE_TRUE;
            throw new RuntimeError($"Cannot implicitly convert {e.Type()} into Boolean");
        }

        internal static IJaneObject EvalMinusPrefixOperator(IJaneObject e) => e switch
        {
            JaneDouble @double => new JaneDouble { Value = -@double.Value },
            JaneFloat @float => new JaneFloat { Value = -@float.Value },
            JaneSByte @sbyte => new JaneSByte { Value = (sbyte)(-@sbyte.Value) },
            JaneShort @short => new JaneShort { Value = (short)(-@short.Value) },
            JaneInt @int => new JaneInt { Value = -@int.Value },
            JaneLong @long => new JaneLong { Value = -@long.Value },
            JaneInt128 @int128 => new JaneInt128 { Value = -@int128.Value },
            _ => throw new RuntimeError($"{e.Type()} is not a signed number type")
        };
        #endregion
    }
}
