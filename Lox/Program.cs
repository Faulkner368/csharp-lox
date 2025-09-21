using System.Text;

namespace Lox
{
    /// <summary>
    /// The csLox interpreter
    /// </summary>
    class Program
    {
        /// <summary>
        /// Whether or not an error has occurred
        /// </summary>
        private static bool _hadError = false;

        /// <summary>
        /// Entry point to csLox interpreter
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: csLox [script]");
                Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
        }

        /// <summary>
        /// Interprets a file as the given path
        /// </summary>
        /// <param name="path"></param>
        private static void RunFile(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            Run(Encoding.Default.GetString(bytes));

            if (_hadError)
            {
                Environment.Exit(65);
            }
        }

        /// <summary>
        /// Runs an interactive prompt
        /// </summary>
        private static void RunPrompt()
        {
            TextReader input = Console.In;
            
            for (;;)
            {
                Console.WriteLine("> ");
                var line = input.ReadLine();
                
                if (line == null)
                {
                    break;
                }

                Run(line);
                _hadError = false;
            }
        }

        /// <summary>
        /// Runs the given source code
        /// </summary>
        /// <param name="source"></param>
        private static void Run(string source)
        {
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();
            var parser = new Parser(tokens);
            var expression = parser.Parse();

            if (_hadError || expression == null)
            {
                return;
            }

            Console.WriteLine(new AstPrinter().Print(expression));
        }

        /// <summary>
        /// Reports an error at the given line with the given message
        /// </summary>
        /// <param name="line"></param>
        /// <param name="message"></param>
        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        /// <summary>
        /// Reports an error at the given token with the given message
        /// </summary>
        /// <param name="token"></param>
        /// <param name="message"></param>
        public static void Error(Token token, string message)
        {
            if (token.Type == TokenType.EOF)
            {
                Report(token.Line, " at end", message);
            }
            else
            {
                Report(token.Line, $" at '{token.Lexeme}'", message);
            }
        }

        /// <summary>
        /// Reports an error at the given line and location with the given message
        /// </summary>
        /// <param name="line"></param>
        /// <param name="where"></param>
        /// <param name="message"></param>
        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
            _hadError = true;
        }
    }
}