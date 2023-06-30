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
                    default:
                        Console.WriteLine("Invalid REPL Command. To exit use .exit");
                        return;
                }
            }
            string input = RegexReplEndBS().Match(line).Groups["line"].Value;
            while (RegexIncompleteLine().IsMatch(line))
            {
                Console.Write(CONTINUE);
                line = Console.ReadLine();
                if (line is null) break;
                input += $"\n{RegexReplEndBS().Match(line).Groups["line"].Value}";
            }

            Tokenizer lx = new(input);
            Parser.Parser ps = new(lx);
            AST.ASTRoot AST = ps.ParseProgram();
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
            }
            // JANEValue output = Interpreter.Evaluate(AST);
            // Console.WriteLine(output);
        }

        [GeneratedRegex(@".*\\\s*$")]
        private static partial Regex RegexIncompleteLine();
        [GeneratedRegex(@"(?<line>.*)\\\s*$|(?<line>.+$)")]
        private static partial Regex RegexReplEndBS();
    }
}
