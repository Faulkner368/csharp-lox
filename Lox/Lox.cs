using System.Text;

namespace Lox
{
    /// <summary>
    /// The csLox interpreter
    /// </summary>
    class Lox
    {
        /// <summary>
        /// The interpreter instance
        /// </summary>
        private static readonly Interpreter _interpreter = new Interpreter();

        /// <summary>
        /// Whether or not an error has occurred
        /// </summary>
        private static bool _hadError = false;

        /// <summary>
        /// Whether or not a runtime error has occurred
        /// </summary>
        private static bool _hadRuntimeError = false;

        /// <summary>
        /// Entry point to csLox interpreter
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: csLox [script]");
                System.Environment.Exit(64);
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
                System.Environment.Exit(65);
            }

            if (_hadRuntimeError)
            {
                System.Environment.Exit(70);
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
                _hadError = false;

                Console.WriteLine("> ");
                var scanner = new Scanner(input.ReadLine() ?? "");
                var tokens = scanner.ScanTokens();

                var parser = new Parser(tokens);
                object syntax = parser.ParseRepl();

                if (_hadError || syntax == null)
                {
                    continue;
                }

                if (syntax is List<Stmt> list)
                {
                    _interpreter.Interpret((List<Stmt>)syntax);
                }
                else if (syntax is Expr expr)
                {
                    var result = _interpreter.Interpret((Expr)syntax);

                    if (result != null)
                    {
                        Console.WriteLine($"= {result}");
                    }
                }
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
            var statements = parser.Parse();

            if (_hadError || statements == null || statements.Count == 0)
            {
                return;
            }

            var resolver = new Resolver(_interpreter);
            resolver.Resolve(statements);

            if (_hadError)
            {
                return;
            }

            _interpreter.Interpret(statements);
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

        /// <summary>
        /// Reports a runtime error
        /// </summary>
        /// <param name="error"></param>
        public static void RuntimeError(RuntimeError error)
        {
            var message = error?.Message ?? "Runtime error.";
            if (error?.Token != null)
            {
                Console.Error.WriteLine($"{message}\n[line {error.Token.Line}]");
            }
            else
            {
                Console.Error.WriteLine(message);
            }

            _hadRuntimeError = true;
        }
    }
}