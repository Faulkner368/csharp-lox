namespace Lox
{
    /// <summary>
    /// Indicates a return statement was executed in a function.
    /// </summary>
    public class ReturnException : Exception
    {
        /// <summary>
        /// The value being returned.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Creates a new return exception.
        /// </summary>
        /// <param name="value"></param>
        public ReturnException(object value) : base(null, null)
        {
            Value = value;
        }
    }
}
