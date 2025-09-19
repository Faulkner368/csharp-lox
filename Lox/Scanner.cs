using System.Data.Common;

namespace Lox
{
    /// <summary>
    /// A scanner for the Lox programming language
    /// </summary>
    internal class Scanner
    {
        /// <summary>
        /// The source code to scan
        /// </summary>
        private readonly string _source;

        /// <summary>
        /// The list of tokens scanned from the source code
        /// </summary>
        private readonly List<Token> _tokens = new List<Token>();

        /// <summary>
        /// The start index of the current lexeme being scanned
        /// </summary>
        private int _start = 0;

        /// <summary>
        /// The current index in the source code being scanned
        /// </summary>
        private int _current = 0;

        /// <summary>
        /// The current line number being scanned
        /// </summary>
        private int _line = 1;

        /// <summary>
        /// A dictionary of keywords and their corresponding token types
        /// </summary>
        private static readonly Dictionary<string, TokenType> _keywords = new()
        {
            { "and", TokenType.AND },
            { "class", TokenType.CLASS },
            { "else", TokenType.ELSE },
            { "false", TokenType.FALSE },
            { "for", TokenType.FOR },
            { "fun", TokenType.FUN },
            { "if", TokenType.IF },
            { "nil", TokenType.NIL },
            { "or", TokenType.OR },
            { "print", TokenType.PRINT },
            { "return", TokenType.RETURN },
            { "super", TokenType.SUPER },
            { "this", TokenType.THIS },
            { "true", TokenType.TRUE },
            { "var", TokenType.VAR },
            { "while", TokenType.WHILE }
        };

        /// <summary>
        /// Creates a new scanner for the given source code
        /// </summary>
        /// <param name="source"></param>
        public Scanner(string source)
        {
            _source = source;
        }

        /// <summary>
        /// Scans the tokens from the source code
        /// </summary>
        /// <returns>
        /// A list of tokens scanned from the source code
        /// </returns>
        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                _start = _current;
                ScanToken();
            }

            _tokens.Add(new Token(TokenType.EOF, "", null, 0));

            return _tokens;
        }

        /// <summary>
        /// Scans a single token from the source code
        /// </summary>
        /// <returns></returns>
        private bool IsAtEnd()
        {
            return _current >= _source.Length;
        }

        /// <summary>
        /// Scans the next token and adds it to the list of tokens
        /// </summary>
        private void ScanToken()
        {
            char c = Advance();

            switch (c)
            {
                case '(':
                    AddToken(TokenType.LEFT_PAREN);
                    break;
                case ')':
                    AddToken(TokenType.RIGHT_PAREN);
                    break;
                case '{':
                    AddToken(TokenType.LEFT_BRACE);
                    break;
                case '}':
                    AddToken(TokenType.RIGHT_BRACE);
                    break;
                case ',': 
                    AddToken(TokenType.COMMA);
                    break;
                case '.':
                    AddToken(TokenType.DOT);
                    break;
                case '-':
                    AddToken(TokenType.MINUS);
                    break;
                case '+':
                    AddToken(TokenType.PLUS);
                    break;
                case ';':
                    AddToken(TokenType.SEMICOLON);
                    break;
                case '*':
                    AddToken(TokenType.STAR);
                    break;
                case '!':
                    AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=':
                    AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '<':
                    AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case '>':
                    AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;
                case '/':
                    if (Match('/'))
                    {
                        // A comment goes until the end of the line
                        while (Peek() != '\n' && !IsAtEnd())
                        {
                            Advance();
                        }
                    }
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }
                    break;
                
                case ' ':
                case '\r':
                case '\t':
                    // Ignore whitespace
                    break;

                case '\n':
                    _line++;
                    break;

                case '"':
                    StringLiteral();
                    break;



                default:
                    if (IsDigit(c))
                    {
                        NumberLiteral();
                    }
                    else if (IsAlpha(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        Program.Error(_line, "Unexpected character.");
                    }

                    break;
            }
        }

        /// <summary>
        /// Scans an identifier and adds it to the list of tokens
        /// </summary>
        private void Identifier()
        {
            while (IsAlphaNumeric(Peek()))
            {
                Advance();
            }

            var text = _source.Substring(_start, _current - _start);
            var type = _keywords.ContainsKey(text)
                ? _keywords[text]
                : TokenType.IDENTIFIER;

            AddToken(type);
        }

        /// <summary>
        /// Scans an identifier and adds it to the list of tokens
        /// </summary>
        private void NumberLiteral()
        {
            while (IsDigit(Peek()))
            {
                Advance();
            }

            // Look for a fractional part
            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                // Consume the "."
                Advance();

                while (IsDigit(Peek()))
                {
                    Advance();
                }
            }

            var value = double.Parse(_source.Substring(_start, _current - _start));
            AddToken(TokenType.NUMBER, value);
        }

        /// <summary>
        /// Scans a string literal and adds it to the list of tokens
        /// </summary>
        private void StringLiteral()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n')
                {
                    _line++;
                }

                Advance();
            }

            if (IsAtEnd())
            {
                Program.Error(_line, "Unterminated string.");
                return;
            }

            // Advance the closing "
            Advance();

            // Trim the surrounding quotes
            var value = _source.Substring(_start + 1, _current - _start - 2);
            AddToken(TokenType.STRING, value);
        }

        /// <summary>
        /// Consumes the next character if it matches the expected character
        /// </summary>
        /// <param name="expected"></param>
        /// <returns></returns>
        private bool Match(char expected)
        {
            if (IsAtEnd())
            {
                return false;
            }

            if (_source[_current] != expected)
            {
                return false;
            }

            _current++;
            
            return true;
        }

        /// <summary>
        /// Scans a number literal and adds it to the list of tokens
        /// </summary>
        /// <returns></returns>
        private char Peek()
        {
            if (IsAtEnd())
            {
                return '\0';
            }
            return _source[_current];
        }

        /// <summary>
        /// Scans a number literal and adds it to the list of tokens
        /// </summary>
        /// <returns></returns>
        private char PeekNext()
        {
            if (_current + 1 >= _source.Length)
            {
                return '\0';
            }

            return _source[_current + 1];
        }

        /// <summary>
        /// Scans an identifier and adds it to the list of tokens
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z') ||
                   c == '_';
        }

        /// <summary>
        /// Scans an identifier and adds it to the list of tokens
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        /// <summary>
        /// Scans a number literal and adds it to the list of tokens
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        /// <summary>
        /// Advances the current index and returns the next character
        /// </summary>
        private char Advance()
        {
            return _source[_current++];
        }

        /// <summary>
        /// Adds a token to the list of tokens
        /// </summary>
        /// <param name="type"></param>
        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        /// <summary>
        /// Adds a token to the list of tokens
        /// </summary>
        /// <param name="type"></param>
        /// <param name="literal"></param>
        private void AddToken(TokenType type, object? literal)
        {
            var text = _source.Substring(_start, _current - _start);
            _tokens.Add(new Token(type, text, literal, _line));
        }
    }
}
