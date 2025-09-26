namespace Lox
{
    /// <summary>
    /// The environment that holds variable bindings.
    /// </summary>
    public class Environment
    {
        /// <summary>
        /// The enclosing environment, if any.
        /// </summary>
        private Environment? _enclosing;

        /// <summary>
        /// Initialises a new instance of the <see cref="Environment"/> class.
        /// </summary>
        private readonly Dictionary<string, object> _values = new();

        /// <summary>
        /// Creates a new environment that encloses the given environment.
        /// </summary>
        /// <param name="environment"></param>
        public Environment(Environment environment)
        {
            _enclosing = environment;
        }

        /// <summary>
        /// Creates a new global environment.
        /// </summary>
        public Environment()
        {
            _enclosing = null;
        }

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
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="RuntimeError"></exception>
        public object Get(Token name)
        {
            if (_values.ContainsKey(name.Lexeme))
            {
                return _values[name.Lexeme];
            }

            if (_enclosing != null)
            {
                return _enclosing.Get(name);
            }

            throw new RuntimeError(null, $"Undefined variable '{name.Lexeme}'.");
        }

        /// <summary>
        /// Assigns a value to an existing variable in the environment.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="value"></param>
        /// <exception cref="RuntimeError"></exception>
        public void Assign(Token token, object value)
        {
            if (_values.ContainsKey(token.Lexeme))
            {
                _values[token.Lexeme] = value;
                return;
            }

            if (_enclosing != null)
            {
                _enclosing.Assign(token, value);
                return;
            }

            throw new RuntimeError(token, $"Undefined variable '{token.Lexeme}'.");
        }
    }
}
