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
        public Environment? Enclosing;

        /// <summary>
        /// Initialises a new instance of the <see cref="Environment"/> class.
        /// </summary>
        private readonly Dictionary<string, object?> _values = new();

        /// <summary>
        /// Creates a new environment that encloses the given environment.
        /// </summary>
        /// <param name="environment"></param>
        public Environment(Environment environment)
        {
            Enclosing = environment;
        }

        /// <summary>
        /// Creates a new global environment.
        /// </summary>
        public Environment()
        {
            Enclosing = null;
        }

        /// <summary>
        /// Defines variables in the environment.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Define(string name, object? value)
        {
            _values[name] = value;
        }

        /// <summary>
        /// Returns the ancestor environment at the given distance.
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public Environment Ancestor(int distance)
        {
            var environment = this;

            for (var i = 0; i < distance; i++)
            {
                environment = environment!.Enclosing;
            }
            
            return environment!;
        }

        /// <summary>
        /// Gets the value of a variable from the environment.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="RuntimeError"></exception>
        public object? Get(Token name)
        {
            if (_values.ContainsKey(name.Lexeme))
            {
                return _values[name.Lexeme];
            }

            if (Enclosing != null)
            {
                return Enclosing.Get(name);
            }

            throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
        }

        /// <summary>
        /// Gets the value of a variable from an ancestor environment at the given distance.
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetAt(int distance, string name)
        {
            return Ancestor(distance)._values[name];
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

            if (Enclosing != null)
            {
                Enclosing.Assign(token, value);
                return;
            }

            throw new RuntimeError(token, $"Undefined variable '{token.Lexeme}'.");
        }

        /// <summary>
        /// Assigns a value to a variable in an ancestor environment at the given distance.
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AssignAt(int distance, Token name, object value)
        {
            Ancestor(distance)._values[name.Lexeme] = value;
        }
    }
}
