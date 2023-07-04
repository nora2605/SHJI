using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using SHJI.Lexer;
using SHJI.Parser;
using System.ANSIConsole;
using System.Runtime.CompilerServices;

namespace SHJI
{
    internal static partial class REPL
    {
        static readonly string HEADER = @$"SHJI Version {Assembly.GetExecutingAssembly().GetName().Version}";
        const string PROMPT = "jn> ";
        const string CONTINUE = "..> ";

        static bool exiting = false;
        static bool parser_debug = false;
        static int nestingLevel = 0;

        static string prevInput = "";

        public static void Start(bool parser_debug = false)
        {
            REPL.parser_debug = parser_debug;
            Console.WriteLine(HEADER);
            while (!exiting)
            {
                REP();
            }
        }

        static void REP()
        {
            Console.Write(PROMPT);
            string? line = Console.ReadLine();
            string input = "";
            if (line is null or "")
            {
                return;
            }
            if (line.StartsWith('.'))
            {
                switch (line[1..])
                {
                    case "exit":
                        exiting = true;
                        return;
                    case "repeat":
                        input = prevInput;
                        goto Parse;
                    default:
                        Console.WriteLine("Invalid REPL Command. To exit use .exit");
                        return;
                }
            }
            input = RegexReplEndBS().Match(line).Groups["line"].Value;
            Tokenizer nestChecker = new(input);
            nestingLevel = CountNestLevel(nestChecker);
            while (RegexIncompleteLine().IsMatch(line) || nestingLevel > 0)
            {
                Console.Write(CONTINUE);
                line = Console.ReadLine();
                if (line is null or "") break;
                input += $"\n{RegexReplEndBS().Match(line).Groups["line"].Value}";
                nestChecker = new(input);
                nestingLevel = CountNestLevel(nestChecker);
            }

            prevInput = input;
            Parse:
            Tokenizer lx = new(input);
            Parser.Parser ps = new(lx);
            AST.ASTRoot AST = ps.ParseProgram();
#if DEBUG
            if (parser_debug)
            {
                lx.Reset();
                Console.WriteLine("Lexer Output: ".Bold().Yellow());
                foreach (Token token in lx)
                    Console.Write($"{{{token.Type}, {token.Literal}, {token.Line}, {token.Column}}} ".Yellow());
                Console.WriteLine();
                Console.WriteLine("Parser Output: ".Bold().Green());
                Console.WriteLine(AST.JOHNSerialize().Green());
                Console.WriteLine("Reconstructed AST: ".Bold().Blue());
                Console.WriteLine(AST.ToString().Blue());
                if (ps.Errors.Length > 0)
                {
                    string a = ps.Errors.Select(e => e.ToString().Red()).Aggregate((a, b) => a + "\n" + b);
                    Console.WriteLine("Errors Encountered: ".Bold().Red());
                    Console.WriteLine(a);
                }
            }
#endif
            // JANEValue output = Interpreter.Evaluate(AST);
            // Console.WriteLine(output);
        }

        static int CountNestLevel(Tokenizer t)
        {
            int level = 0;
            foreach (Token token in t)
            {
                if (token.Type == TokenType.LBRACE) level++;
                else if (token.Type == TokenType.RBRACE) level--;
            }
            return level;
        }

        [GeneratedRegex(@".*[\\]\s*$")]
        private static partial Regex RegexIncompleteLine();
        [GeneratedRegex(@"(?<line>.*)[\\]\s*$|(?<line>.+$)")]
        private static partial Regex RegexReplEndBS();
    }
}
