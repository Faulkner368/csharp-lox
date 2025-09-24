namespace Lox
{
    /// <summary>
    /// The environment that holds variable bindings.
    /// </summary>
    public class Environment
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="Environment"/> class.
        /// </summary>
        private readonly Dictionary<string, object> _values = new();

        /// <summary>
        /// Defines variables in the environment.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Define(string name, object value)
        {
            _values[name] = value;
        }

        /// <summary>
        /// Gets the value of a variable from the environment.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="RuntimeError"></exception>
        public object Get(Token token)
        {
            if (_values.ContainsKey(token.Lexeme))
            {
                return _values[token.Lexeme];
            }
            
            throw new RuntimeError(null, $"Undefined variable '{token.Lexeme}'.");
        }
    }
}
