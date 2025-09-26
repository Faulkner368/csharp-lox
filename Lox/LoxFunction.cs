namespace Lox
{
    /// <summary>
    /// A Lox function
    /// </summary>
    public class LoxFunction : ILoxCallable
    {
        /// <summary>
        /// Creates a new Lox function declaration
        /// </summary>
        private readonly Function _declaration;

        /// <summary>
        /// The closure environment the function was declared in
        /// </summary>
        private readonly Environment _closure;

        /// <summary>
        /// Whether or not this function is an initialiser
        /// </summary>
        private readonly bool _isInitialiser;

        public LoxFunction(Function declaration, Environment closure, bool isInitialiser = false)
        {
            _declaration = declaration;
            _closure = closure;
            _isInitialiser = isInitialiser;
        }

        /// <summary>
        /// The number of arguments the function takes
        /// </summary>
        public int Arity()
        {
            return _declaration.Parameters.Count;
        }

        /// <summary>
        /// Calls the function
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var environment = new Environment(interpreter.Globals);

            for (var i = 0; i < _declaration.Parameters.Count; i++)
            {
                environment.Define(_declaration.Parameters[i].Lexeme, arguments[i]);
            }

            try
            {
                interpreter.ExecuteBlock(_declaration.Body, environment);
            }
            catch (ReturnException returnValue)
            {
                return returnValue.Value;
            }

            return null;
        }

        /// <summary>
        /// Returns a string representation of the function
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"<fn {_declaration.Name.Lexeme}>";
        }
    }
}
