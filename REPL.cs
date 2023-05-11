using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SHJI
{
    internal static class REPL
    {
        static string HEADER = @$"SHJI Version {Assembly.GetExecutingAssembly().GetName().Version}";
        const string PROMPT = "jn> ";

        public static void Start()
        {
            Console.WriteLine(HEADER);
            while (true)
            {
                Console.Write(PROMPT);
                string? line = Console.ReadLine();
                if (line is null or "")
                {
                    continue;
                }
                if (line is ".exit") { break; }
                Lexer lx = new(line);
                for (Token token = lx.NextToken(); token.Type != TokenType.EOF; token = lx.NextToken())
                {
                    Console.Write($"{{{token.Type}, {token.Literal}, {token.Line}, {token.Column}}} ");
                }
                Console.WriteLine();
            }
        }
    }
}
