using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lox
{
    /// <summary>
    /// Indicates a parse error.
    /// </summary>
    public class ParseError : Exception { }

    /// <summary>
    /// Indicates a runtime error.
    /// </summary>
    public class RuntimeError : Exception 
    {
        /// <summary>
        /// The token where the error occurred.
        /// </summary>
        public Token Token { get; }

        /// <summary>
        /// Creates a new runtime error.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="message"></param>
        public RuntimeError(Token token, string message) : base(message)
        {
            Token = token;
        }
    }
}
