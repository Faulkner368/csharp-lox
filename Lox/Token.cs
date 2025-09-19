namespace Lox
{
    /// <summary>
    /// A token produced by the scanner
    /// </summary>
    internal class Token
    {
        /// <summary>
        /// The type of the token
        /// </summary>
        public TokenType Type { get; }

        /// <summary>
        /// The lexeme (string) of the token
        /// </summary>
        public string Lexeme { get; }

        /// <summary>
        /// The literal value of the token, if any
        /// </summary>
        public object? Literal { get; }

        /// <summary>
        /// The line the token was found on
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Creates a new token
        /// </summary>
        /// <param name="type"></param>
        /// <param name="lexeme"></param>
        /// <param name="literal"></param>
        /// <param name="line"></param>
        public Token(TokenType type, string lexeme, object? literal, int line)
        {
            Type = type;
            Lexeme = lexeme;
            Literal = literal;
            Line = line;
        }

        /// <summary>
        /// Returns a string representation of the token
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Type} {Lexeme} {Literal}";
        }
    }
}
