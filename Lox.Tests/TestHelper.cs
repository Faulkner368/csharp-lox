namespace Lox.Tests
{
    public static class TestHelper
    {
        public static string RunLox(string source)
        {
            var output = new StringWriter();
            Console.SetOut(output);
            Console.SetError(output);

            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();
            var parser = new Parser(tokens);
            var statements = parser.Parse();

            var interpreter = new Interpreter();

            var resolver = new Resolver(interpreter);
            resolver.Resolve(statements);

            interpreter.Interpret(statements);

            return output.ToString().Trim();
        }
    }
}
