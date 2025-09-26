namespace Lox
{
    /// <summary>
    /// A Lox function
    /// </summary>
    public class LoxFunction : ILoxCallable
    {
        /// <summary>
        /// The number of arguments the function takes
        /// </summary>
        public int Arity()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calls the function
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        /// <returns></returns>
        public object Call(Interpreter interpreter, List<object> arguments)
        {
            throw new NotImplementedException();
        }
    }
}
