namespace Lox
{
    /// <summary>
    /// A Lox class
    /// </summary>
    public class LoxClass : ILoxCallable
    {
        /// <summary>
        /// The name of the class
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Creates a new Lox class
        /// </summary>
        /// <param name="name"></param>
        public LoxClass(string name)
        {
            Name = name;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }

        /// <inheritdoc/>
        public int Arity()
        {
            return 0;
        }

        /// <inheritdoc/>
        public object? Call(Interpreter interpreter, List<object> arguments)
        {
            var instance = new LoxInstance(this);
            
            return instance;
        }
    }
}
