namespace Tools
{
    /// <summary>
    /// Provides extension methods for various types.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Converts the first character of the input string to lowercase.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FirstCharToLower(this string input) =>
            string.IsNullOrEmpty(input)
                ? input
                : $"{char.ToLowerInvariant(input[0])}{input[1..]}";
    }
}
