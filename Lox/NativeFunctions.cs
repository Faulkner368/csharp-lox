namespace Lox
{
    /// <summary>
    /// A native function that returns the current time in seconds since the epoch
    /// </summary>
    public class ClockLoxCallable : ILoxCallable
    {
        /// <summary>
        /// The number of arguments the function takes
        /// </summary>
        public int Arity() => 0;

        /// <summary>
        /// Calls the function with the given arguments
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public object Call(Interpreter interpreter, List<object> arguments)
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0;
        }

        /// <summary>
        /// Returns a string representation of the function
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "<native fn>";
        }
    }
}
