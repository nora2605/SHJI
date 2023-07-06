using SHJI.AST;
using SHJI.Lexer;
using static SHJI.Interpreter.IJaneObject;

namespace SHJI.Interpreter
{
    internal static class Interpreter
    {
        internal delegate IJaneObject JaneBinaryOperator(IJaneObject left, IJaneObject right);

        internal static bool returning = false;
        internal static Token processingToken;
        internal static JaneEnvironment environment = new();

        internal static IJaneObject Eval(IASTNode node)
        {
            processingToken = node.Token;
            IJaneObject result = node switch
            {
                // Statements
                ASTRoot r => EvalProgram(r.statements),
                ExpressionStatement es => Eval(es.Expression),
                ForLoop fl => EvalForLoop(fl),
                ReturnStatement rs => rs.ReturnValue is null ? JANE_ABYSS : Eval(rs.ReturnValue),
                // Expressions
                IntegerLiteral il => new JaneInt() { Value = il.Value },
                AST.Boolean b => b.Value ? JANE_TRUE : JANE_FALSE,
                PrefixExpression pex => EvalPrefixExpression(pex),
                InfixExpression inf => EvalInfixExpression(inf),
                BlockStatement s => EvalStatements(s.Statements),
                IfExpression ife => EvalIfExpression(ife),
                LetExpression le => EvalLetExpression(le),
                Identifier id => EvalIdentifier(id),
                Assignment ass => EvalAssignment(ass),
                _ => JANE_ABYSS,
            };
            return result;
        }

        internal static IJaneObject EvalForLoop(ForLoop fl)
        {
            var en = Eval(fl.Enumerable);
            // Don't care
            for (int i = 0; i < 100; i++)
            {
                environment[fl.Iterator.Value] = new JaneInt { Value = i };
                Eval(fl.LoopContent);
            }

            // Clean env
            return JANE_ABYSS;
        }

        internal static IJaneObject EvalAssignment(Assignment ass)
        {
            var expr = Eval(ass.Value);
            environment[ass.Name.Value] = expr;
            return expr;
        }

        internal static IJaneObject EvalLetExpression(LetExpression le)
        {
            if (environment.Has(le.Name.Value)) throw new RuntimeError($"Variable name already in use. Use .clear to clear the environment");
            IJaneObject val = le.Value is null ? JANE_UNINITIALIZED : Eval(le.Value);
            environment.Set(le.Name.Value, val);
            return val;
        }

        internal static IJaneObject EvalIdentifier(Identifier id)
        {
            var e = environment.Get(id.Value);
            if (e == JANE_UNINITIALIZED)
                throw new RuntimeError($"Variable \"{id.Value}\" was uninitialized or not found at access time", processingToken);
            return e;
        }

        internal static IJaneObject EvalProgram(IStatement[] stmts)
        {
            IJaneObject result = JANE_ABYSS;
            foreach (IStatement stmt in stmts)
            {
                result = Eval(stmt);
                if (stmt is ReturnStatement || returning)
                {
                    returning = false;
                    break;
                }
            }

            // If there is no top level statement, find and call the main method

            return result;
        }

        internal static IJaneObject EvalFunction(IStatement[] stmts)
        {
            IJaneObject result = JANE_ABYSS;
            foreach (IStatement stmt in stmts)
            {
                result = Eval(stmt);
                if (stmt is ReturnStatement || returning)
                {
                    returning = false;
                    break;
                }
            }

            // Clean up env
            return result;
        }

        internal static IJaneObject EvalStatements(IStatement[] stmts)
        {
            IJaneObject result = JANE_ABYSS;
            foreach (IStatement stmt in stmts)
            {
                result = Eval(stmt);
                if (stmt is ReturnStatement)
                {
                    returning = true;
                    break;
                }
                /* if (stmt is BreakStatement)
                {
                    break;
                } */
            }
            return result;
        }

        internal static IJaneObject EvalIfExpression(IfExpression ife)
        {
            IJaneObject cond = Eval(ife.Condition);
            if (cond is JaneBool jb)
            {
                if (jb.Value) return Eval(ife.Cons);
                else return ife.Alt is null ? JANE_ABYSS : Eval(ife.Alt);
            }
            else throw new RuntimeError("Condition is not a Boolean", processingToken);
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

            // Insert type magic

            // make this.... general
            // I don't want to limit the operators perse but parsing new operators would require a dynamic parser and HONESTLY i can't be arsed
            // I think with basic arithmetic, rational, binary and boolean and even a special concatenate operator that you can use for lists, paths and all that
            // The language is already stacked pretty well.

            // Full list of (infix) operators TO BE IMPLEMENTED as of July 2023:
            // +, -, *, /, ^, %, >, <, >=, <=, ==, !=, &&, ||, xor
            // ~, &, |, x0r (binary xor), .. (range operator), ?????? (up to debate, type coercion), :: (pufferfish operator, chains outer function calls)

            // Except for the type coercion and pufferfish operator, the operator behavior can be individually adjusted for every user defined type

            // An expression of type a <operator>= b will be evaluated as a = a <operator> b
            // Meaning it is valid to write var === true (technically, you would never use this probably)
            // A special case is var ?= a : b (Flipflop operator) which just switches between values so you don't have to modulo stuff
            // although a circular list or special type for that would PROBABLY be better IDK

            // Note on postfixes:
            // Implement the switcheroo-operator: !! postfix to change a boolean from true to false or from false to true
            return e.Operator switch
            {
                "+" => EvalPlusInfix(left, right),
                "-" => EvalMinusInfix(left, right),
                "/" => EvalSlashInfix(left, right),
                "*" => EvalAsteriskInfix(left, right),
                "^" => EvalHatInfix(left, right),
                ">" => EvalGTInfix(left, right),
                ">=" => EvalGTEInfix(left, right),
                "<" => EvalLTInfix(left, right),
                "<=" => EvalLTEInfix(left, right),
                "==" => EvalEQInfix(left, right),
                "!=" => EvalNOT_EQInfix(left, right),
                "~" => EvalConcatInfix(left, right),
                _ => throw new RuntimeError($"Operator {e.Operator} is unknown or not implemented", processingToken),
            } ?? throw new RuntimeError($"Operator {e.Operator} not implemented for operands of type {left.Type()} and {right.Type()}", processingToken);
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
            throw new RuntimeError($"Operator + not implemented for operands of type {left.Type()} and {right.Type()}", processingToken);
        }
        internal static IJaneObject? EvalMinusInfix(IJaneObject left, IJaneObject right)
        {
            if (left is JaneInt oLeftInt)
            {
                if (right is JaneInt oRightInt) return new JaneInt { Value = oLeftInt.Value - oRightInt.Value };
            }
            return null;
        }
        internal static IJaneObject? EvalSlashInfix(IJaneObject left, IJaneObject right)
        {
            if (left is JaneInt oLeftInt)
            {
                if (right is JaneInt oRightInt) return new JaneInt { Value = oLeftInt.Value / oRightInt.Value };
            }
            return null;
        }
        internal static IJaneObject? EvalAsteriskInfix(IJaneObject left, IJaneObject right)
        {
            if (left is JaneInt oLeftInt)
            {
                if (right is JaneInt oRightInt) return new JaneInt { Value = oLeftInt.Value * oRightInt.Value };
            }
            return null;
        }

        internal static IJaneObject? EvalHatInfix(IJaneObject left, IJaneObject right)
        {
            if (left is JaneInt oLeftInt)
            {
                if (right is JaneInt oRightInt) return new JaneInt { Value = IntPow(oLeftInt.Value, oRightInt.Value) };
            }
            return null;
        }

        internal static IJaneObject? EvalGTInfix(IJaneObject left, IJaneObject right)
        {
            if (left is JaneInt oLeftInt)
            {
                if (right is JaneInt oRightInt) return oLeftInt.Value > oRightInt.Value ? JANE_TRUE : JANE_FALSE;
            }
            return null;
        }

        internal static IJaneObject? EvalGTEInfix(IJaneObject left, IJaneObject right)
        {
            if (left is JaneInt oLeftInt)
            {
                if (right is JaneInt oRightInt) return oLeftInt.Value >= oRightInt.Value ? JANE_TRUE : JANE_FALSE;
            }
            return null;
        }

        internal static IJaneObject? EvalLTInfix(IJaneObject left, IJaneObject right)
        {
            if (left is JaneInt oLeftInt)
            {
                if (right is JaneInt oRightInt) return oLeftInt.Value < oRightInt.Value ? JANE_TRUE : JANE_FALSE;
            }
            return null;
        }

        internal static IJaneObject? EvalLTEInfix(IJaneObject left, IJaneObject right)
        {
            if (left is JaneInt oLeftInt)
            {
                if (right is JaneInt oRightInt) return oLeftInt.Value <= oRightInt.Value ? JANE_TRUE : JANE_FALSE;
            }
            return null;
        }

        internal static IJaneObject? EvalEQInfix(IJaneObject left, IJaneObject right)
        {
            if (left is JaneInt oLeftInt)
            {
                if (right is JaneInt oRightInt) return oLeftInt.Value == oRightInt.Value ? JANE_TRUE : JANE_FALSE;
            }
            if (left is JaneBool oLeftBool)
            {
                if (right is JaneBool oRightBool) return oLeftBool.Value == oRightBool.Value ? JANE_TRUE : JANE_FALSE;
            }
            return left == right ? JANE_TRUE : null;
        }

        internal static IJaneObject? EvalNOT_EQInfix(IJaneObject left, IJaneObject right)
        {
            var eq = EvalEQInfix(left, right);
            return eq == null ? null : eq == JANE_TRUE ? JANE_FALSE : JANE_TRUE;
        }

        internal static IJaneObject? EvalConcatInfix(IJaneObject left, IJaneObject right)
        {
            if (left is JaneString oLeftString)
            {
                if (right is JaneString oRightString) return new JaneString { Value = oLeftString.Value + oRightString.Value };
            }
            if (left is JaneChar oLeftChar)
            {
                if (right is JaneChar oRightChar) return new JaneString { Value = oLeftChar.Value.ToString() + oRightChar.Value.ToString() };
            }
            return null;
        }

        internal static int IntPow(int @base, int exponent)
        {
            if (exponent == 0) return 1;
            if (@base == 0) return 0;
            if (@base == 1) return 1;
            if (exponent < 0) return 0;
            else return @base * IntPow(@base, exponent - 1);
        }

        internal static IJaneObject EvalBangPrefixOperator(IJaneObject e)
        {
            if (e == JANE_TRUE) return JANE_FALSE;
            else if (e == JANE_FALSE) return JANE_TRUE;
            else if (e == JANE_ABYSS) return JANE_TRUE;
            throw new RuntimeError($"Cannot implicitly convert {e.Type()} into Boolean", processingToken);
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
            _ => throw new RuntimeError($"{e.Type()} is not a signed number type", processingToken)
        };
        #endregion
    }
}
