namespace Lox
{
    /// <summary>
    /// Something that can be called, like a function or class
    /// </summary>
    public interface ILoxCallable
    {
        /// <summary>
        /// The number of arguments the function takes
        /// </summary>
        int Arity();

        /// <summary>
        /// The number of arguments the function takes
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        object? Call(Interpreter interpreter, List<object> arguments);
    }
}
