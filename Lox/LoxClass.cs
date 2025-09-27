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
        /// The superclass of the class, if any
        /// </summary>
        public readonly LoxClass Superclass;

        /// <summary>
        /// The methods of the class
        /// </summary>
        private readonly Dictionary<string, LoxFunction> _methods;

        /// <summary>
        /// Creates a new Lox class
        /// </summary>
        /// <param name="name"></param>
        public LoxClass(string name, LoxClass superclass, Dictionary<string, LoxFunction> methods)
        {
            Name = name;
            Superclass = superclass;
            _methods = methods;
        }

        /// <summary>
        /// Finds a method on the class
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public LoxFunction FindMethod(string name)
        {
            if (_methods.ContainsKey(name))
            {
                return _methods[name];
            }

            if (Superclass != null)
            {
                return Superclass.FindMethod(name);
            }

            return null;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }

        /// <inheritdoc/>
        public int Arity()
        {
            var initialiser = FindMethod("init");
            if (initialiser == null)
            {
                return 0;
            }

            return initialiser.Arity();
        }

        /// <inheritdoc/>
        public object? Call(Interpreter interpreter, List<object> arguments)
        {
            var instance = new LoxInstance(this);
            var initialiser = FindMethod("init");
            if (initialiser != null)
            {
                initialiser.Bind(instance).Call(interpreter, arguments);
            }

            return instance;
        }
    }
}
