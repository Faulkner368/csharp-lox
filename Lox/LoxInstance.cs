namespace Lox
{
    /// <summary>
    /// A Lox instance
    /// </summary>
    public class LoxInstance
    {
        /// <summary>
        /// Creates a new Lox instance
        /// </summary>
        private LoxClass _klass;

        /// <summary>
        /// The fields of the instance
        /// </summary>
        private readonly Dictionary<string, object> _fields = new();

        /// <summary>
        /// The class of the instance
        /// </summary>
        /// <param name="klass"></param>
        public LoxInstance(LoxClass klass)
        {
            _klass = klass;
        }

        /// <summary>
        /// Sets a field on the instance
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="RuntimeError"></exception>
        public object Get(Token name)
        {
            if (_fields.TryGetValue(name.Lexeme, out var value))
            {
                return value;
            }
            
            throw new RuntimeError(name, $"Undefined property '{name.Lexeme}'.");
        }

        /// <summary>
        /// Sets a field on the instance
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Set(Token name, object value)
        {
            _fields[name.Lexeme] = value;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{_klass.Name} instance";
        }
    }
}
