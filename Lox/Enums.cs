namespace Lox
{
    /// <summary>
    /// The different types of tokens that can be produced by the scanner
    /// </summary>
    public enum TokenType
    {
        // Single-character tokens.
        LEFT_PAREN,
        RIGHT_PAREN,
        LEFT_BRACE,
        RIGHT_BRACE,
        COMMA,
        DOT,
        MINUS,
        PLUS,
        SEMICOLON,
        SLASH,
        STAR,
        
        // One or two character tokens.
        BANG,
        BANG_EQUAL,
        EQUAL,
        EQUAL_EQUAL,
        GREATER,
        GREATER_EQUAL,
        LESS,
        LESS_EQUAL,
        
        // Literals.
        IDENTIFIER,
        STRING,
        NUMBER,
        
        // Keywords.
        AND,
        CLASS,
        ELSE,
        FALSE,
        FUN,
        FOR,
        IF,
        NIL,
        OR,
        PRINT,
        RETURN,
        SUPER,
        THIS,
        TRUE,
        VAR,
        WHILE,
        
        EOF
    }

    /// <summary>
    /// The type of function we are currently resolving
    /// </summary>
    public enum FunctionType
    {
        NONE,
        FUNCTION,
        INITIALISER,
        METHOD
    }

    /// <summary>
    /// The type of class we are currently resolving
    /// </summary>
    public enum ClassType
    {
        NONE,
        CLASS,
        SUBCLASS
    }
}
