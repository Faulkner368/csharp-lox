using System;
using static Lox.TokenType;

namespace Lox
{
    /// <summary>
    /// Parser for Lox language.
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// Enables or disables debug output.
        /// </summary>
        private bool _debug = false;

        /// <summary>
        /// Whether expressions are currently allowed.
        /// </summary>
        private bool _allowExpression;

        /// <summary>
        /// Whether an expression has been found.
        /// </summary> 
        private bool _foundExpression = false;

        /// <summary>
        /// Initialises a new instance of the <see cref="Parser"/> class.
        /// </summary>
        private readonly List<Token> _tokens;

        /// <summary>
        /// Initialises a new instance of the <see cref="Parser"/> class.
        /// </summary>
        private int _current = 0;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        /// <summary>
        /// Parses the tokens and returns the resulting expression.
        /// </summary>
        /// <returns></returns>
        public List<Stmt> Parse()
        {
            if (_debug) Console.WriteLine("Parse()");

            var statements = new List<Stmt>();

            while(!IsAtEnd())
            {
                statements.Add(Declaration());
            }

            return statements;
        }

        /// <summary>
        /// Parses the tokens and returns the resulting expression.
        /// </summary>
        /// <returns></returns>
        private Expr Expression()
        {
            if (_debug) Console.WriteLine("Expression()");

            return Assignment();
        }

        /// <summary>
        /// Parses a declaration.
        /// </summary>
        /// <returns></returns>
        private Stmt Declaration()
        {
            if (_debug) Console.WriteLine("Declaration()");

            try
            {
                if (Match(VAR))
                {
                    return VarDeclaration();
                }

                return Statement();
            }
            catch (ParseError)
            {
                Synchronise();

                return null;
            }
        }

        /// <summary>
        /// Parses a print statement.
        /// </summary>
        /// <returns></returns>
        private Stmt Statement()
        {
            if (_debug) Console.WriteLine("Statement()");

            if (Match(FOR))
            {
                return ForStatement();
            }

            if (Match(IF))
            {
                return IfStatement();
            }

            if (Match(PRINT))
            {
                return PrintStatement();
            }

            if (Match(WHILE))
            {
                return WhileStatement();
            }

            if (Match(LEFT_BRACE))
            {
                return new Block(Block());
            }

            return ExpressionStatement();
        }

        /// <summary>
        /// Parses a for statement.
        /// </summary>
        /// <returns></returns>
        private Stmt ForStatement()
        {
            if (_debug) Console.WriteLine("ForStatement()");
            
            Consume(LEFT_PAREN, "Expect '(' after 'for'.");
            
            Stmt initialiser;
            if (Match(SEMICOLON))
            {
                initialiser = null;
            }
            else if (Match(VAR))
            {
                initialiser = VarDeclaration();
            }
            else
            {
                initialiser = ExpressionStatement();
            }

            Expr condition = null;
            if (!Check(SEMICOLON))
            {
                condition = Expression();
            }
            
            Consume(SEMICOLON, "Expect ';' after loop condition.");
            
            Expr increment = null;
            if (!Check(RIGHT_PAREN))
            {
                increment = Expression();
            }
            
            Consume(RIGHT_PAREN, "Expect ')' after for clauses.");
            
            var body = Statement();
            
            if (increment != null)
            {
                body = new Block(new List<Stmt>
                {
                    body,
                    new Expression(increment)
                });
            }
            
            if (condition == null)
            {
                condition = new Literal(true);
            }
            
            body = new While(condition, body);
            
            if (initialiser != null)
            {
                body = new Block(new List<Stmt> { initialiser, body });
            }
            
            return body;
        }

        /// <summary>
        /// Parses an if statement.
        /// </summary>
        /// <returns></returns>
        private Stmt IfStatement()
        {
            if (_debug) Console.WriteLine("IfStatement()");
            
            Consume(LEFT_PAREN, "Expect '(' after 'if'.");
            var condition = Expression();
            Consume(RIGHT_PAREN, "Expect ')' after if condition.");
            
            var thenBranch = Statement();
            Stmt elseBranch = null;
            if (Match(ELSE))
            {
                elseBranch = Statement();
            }
            
            return new If(condition, thenBranch, elseBranch);
        }

        /// <summary>
        /// Parses a print statement.
        /// </summary>
        /// <returns></returns>
        private Stmt PrintStatement()
        {
            if (_debug) Console.WriteLine("PrintStatement()");

            var value = Expression();
            Consume(SEMICOLON, "Expect ';' after value.");
            
            return new Print(value);
        }

        /// <summary>
        /// Parses a variable declaration.
        /// </summary>
        /// <returns></returns>
        private Stmt VarDeclaration()
        {
            if (_debug) Console.WriteLine("VarDeclaration()");

            Token name = Consume(IDENTIFIER, "Expect variable name.");

            Expr initialiser = null;
            if (Match(EQUAL))
            {
                initialiser = Expression();
            }

            Consume(SEMICOLON, "Expect ';' after variable declaration.");
            
            return new Var(name, initialiser);
        }

        /// <summary>
        /// Parses a while statement.
        /// </summary>
        /// <returns></returns>
        private Stmt WhileStatement()
        {
            if (_debug) Console.WriteLine("WhileStatement()");
            
            Consume(LEFT_PAREN, "Expect '(' after 'while'.");
            var condition = Expression();
            Consume(RIGHT_PAREN, "Expect ')' after condition.");
            var body = Statement();
            
            return new While(condition, body);
        }

        /// <summary>
        /// Parses an expression statement.
        /// </summary>
        /// <returns></returns>
        private Stmt ExpressionStatement()
        {
            if (_debug) Console.WriteLine("ExpressionStatement()");

            var expr = Expression();

            if (_allowExpression && IsAtEnd())
            {
                _foundExpression = true;
            }
            else
            {
                Consume(SEMICOLON, "Expect ';' after expression.");
            }

            return new Expression(expr);
        }

        /// <summary>
        /// Parses a block of statements.
        /// </summary>
        /// <returns></returns>
        private List<Stmt> Block()
        {
            if (_debug) Console.WriteLine("Block()");
            
            var statements = new List<Stmt>();
            
            while (!Check(RIGHT_BRACE) && !IsAtEnd())
            {
                statements.Add(Declaration());
            }
            
            Consume(RIGHT_BRACE, "Expect '}' after block.");
            
            return statements;
        }

        /// <summary>
        /// Parses an assignment expression.
        /// </summary>
        /// <returns></returns>
        private Expr Assignment()
        {
            if (_debug) Console.WriteLine("Assignment()");

            var expr = Or();

            if (Match(EQUAL))
            {
                var equals = Previous();
                var value = Assignment();

                if (expr is Variable variable)
                {
                    var name = variable.Name;
                    
                    return new Assign(name, value);
                }

                Error(equals, "Invalid assignment target.");
            }

            return expr;
        }

        /// <summary>
        /// Parses an or expression.
        /// </summary>
        /// <returns></returns>
        private Expr Or()
        {
            if (_debug) Console.WriteLine("Or()");
            
            var expr = And();
            
            while (Match(OR))
            {
                var op = Previous();
                var right = And();
                expr = new Logical(expr, op, right);
            }
            
            return expr;
        }

        /// <summary>
        /// Parses an and expression.
        /// </summary>
        /// <returns></returns>
        private Expr And()
        {
            if (_debug) Console.WriteLine("And()");
            
            var expr = Equality();
            
            while (Match(AND))
            {
                var op = Previous();
                var right = Equality();
                expr = new Logical(expr, op, right);
            }
            
            return expr;
        }

        /// <summary>
        /// Parses an equality expression.
        /// </summary>
        /// <returns></returns>
        private Expr Equality()
        {
            if (_debug) Console.WriteLine("Equality()");

            var expr = Comparison();

            while(Match(BANG_EQUAL, EQUAL_EQUAL))
            {
                Token op = Previous();
                var right = Comparison();
                expr = new Binary(expr, op, right);
            }

            return expr;
        }

        /// <summary>
        /// Checks if the current token is of the given type.
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        private bool Match(params TokenType[] types)
        {
            foreach (TokenType type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the current token and advances to the next one.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool Check(TokenType type)
        {
            if (IsAtEnd())
            {
                return false;
            }

            return Peek().Type == type;
        }

        /// <summary>
        /// Returns the current token without consuming it.
        /// </summary>
        /// <returns></returns>
        private Token Advance()
        {
            if (!IsAtEnd())
            {
                _current++;
            }

            return Previous();
        }

        /// <summary>
        /// Returns true if the parser has reached the end of the token list.
        /// </summary>
        /// <returns></returns>
        private bool IsAtEnd()
        {
            return Peek().Type == EOF;
        }

        /// <summary>
        /// Returns the current token without consuming it.
        /// </summary>
        /// <returns></returns>
        private Token Peek()
        {
            return _tokens[_current];
        }

        /// <summary>
        /// Returns the previous token.
        /// </summary>
        /// <returns></returns>
        private Token Previous()
        {
            return _tokens[_current - 1];
        }

        /// <summary>
        /// Checks if the current token matches any of the given types.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private Expr Comparison()
        {
            if (_debug) Console.WriteLine("Comparison()");

            var expr = Term();

            while(Match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL))
            {
                Token op = Previous();
                var right = Term();
                expr = new Binary(expr, op, right);
            }

            return expr;
        }

        /// <summary>
        /// Parses a term expression.
        /// </summary>
        /// <returns></returns>
        private Expr Term()
        {
            if (_debug) Console.WriteLine("Term()");

            var factor = Factor();

            while(Match(MINUS, PLUS))
            {
                Token op = Previous();
                var right = Factor();
                factor = new Binary(factor, op, right);
            }

            return factor;  
        }

        /// <summary>
        /// Parses a factor expression.
        /// </summary>
        /// <returns></returns>
        private Expr Factor()
        {
            if (_debug) Console.WriteLine("Factor()");

            var expr = Unary();

            while(Match(SLASH, STAR))
            {
                Token op = Previous();
                var right = Unary();
                expr = new Binary(expr, op, right);
            }

            return expr;
        }

        /// <summary>
        /// Parses a unary expression.
        /// </summary>
        /// <returns></returns>
        private Expr Unary()
        {
            if (_debug) Console.WriteLine("Unary()");

            while (Match(BANG, MINUS))
            {
                Token op = Previous();
                var right = Unary();
                return new Unary(op, right);
            }

            return Call();
        }

        /// <summary>
        /// Finishes parsing a function call expression.
        /// </summary>
        /// <param name="callee"></param>
        /// <returns></returns>
        private Expr FinishCall(Expr callee)
        {
            if (_debug) Console.WriteLine("FinishCall()");
            
            var arguments = new List<Expr>();
            
            if (!Check(RIGHT_PAREN))
            {
                do
                {
                    if (arguments.Count >= 255)
                    {
                        Error(Peek(), "Can't have more than 255 arguments.");
                    }
            
                    arguments.Add(Expression());

                } while (Match(COMMA));
            }
            
            var paren = Consume(RIGHT_PAREN, "Expect ')' after arguments.");
            
            return new Call(callee, paren, arguments);
        }

        /// <summary>
        /// Parses a function call expression.
        /// </summary>
        /// <returns></returns>
        private Expr Call()
        {
            if (_debug) Console.WriteLine("Call()");
            
            var expr = Primary();
            
            while (true)
            {
                if (Match(LEFT_PAREN))
                {
                    expr = FinishCall(expr);
                }
                else
                {
                    break;
                }
            }
            
            return expr;
        }

        /// <summary>
        /// Parses a primary expression.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ParseError"></exception>
        private Expr Primary()
        {
            if (_debug) Console.WriteLine("Primary()");

            if (Match(FALSE))
            {
                return new Literal(false);
            }

            if (Match(TRUE))
            {
                return new Literal(true);
            }

            if (Match(NIL))
            {
                return new Literal(null);
            }

            if (Match(NUMBER, STRING))
            {
                return new Literal(Previous().Literal);
            }

            if (Match(IDENTIFIER))
            {
                return new Variable(Previous());
            }   

            if (Match(LEFT_PAREN))
            {
                var expr = Expression();
                Consume(RIGHT_PAREN, "Expect ')' after expression.");
                return new Grouping(expr);
            }

            throw Error(Peek(), "Expect expression.");
        }

        /// <summary>
        /// Consumes the current token if it matches the given type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <exception cref="ParseError"></exception>
        private Token Consume(TokenType type, string message)
        {
            if (Check(type))
            {
                return Advance();
            }

            throw Error(Peek(), message);
        }

        /// <summary>
        /// Creates a new parse error with the given token and message.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private ParseError Error(Token token, string message)
        {
            Lox.Error(token, message);
            return new ParseError();
        }

        /// <summary>
        /// Synchronises the parser by discarding tokens until it reaches a statement boundary.
        /// </summary>
        private void Synchronise()
        {
            if (_debug) Console.WriteLine("Synchronise()");

            Advance();

            while (!IsAtEnd())
            {
                if (Previous().Type == SEMICOLON)
                {
                    return;
                }

                switch (Peek().Type)
                {
                    case CLASS:
                    case FUN:
                    case VAR:
                    case FOR:
                    case IF:
                    case WHILE:
                    case PRINT:
                    case RETURN:
                        return;
                }

                Advance();
            }
        }

        /// <summary>
        /// Parses a REPL input, allowing for single expressions.
        /// </summary>
        /// <returns></returns>
        public object ParseRepl()
        {
            _allowExpression = true;
            
            var statements = new List<Stmt>();
            while (!IsAtEnd())
            {
                statements.Add(Declaration());

                if (_foundExpression)
                {
                    var last = statements.LastOrDefault();
                    if (last is Expression exprStmt)
                    {
                        return exprStmt.Expr;
                    }

                    _allowExpression = false;
                }
            }

            return statements;
        }
    }
}
