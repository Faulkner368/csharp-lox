namespace Lox
{
    /// <summary>
    /// The resolver performs static analysis on the AST to resolve variable bindings.
    /// </summary>
    public class Resolver : Expr.IVisitor<object>, Stmt.IVisitor<object>
    {
        /// <summary>
        /// The interpreter to resolve variables for
        /// </summary>
        private readonly Interpreter _interpreter;

        /// <summary>
        /// The stack of scopes
        /// </summary>
        private readonly Stack<Dictionary<string, bool>> _scopes = new();

        /// <summary>
        /// The type of function currently being resolved
        /// </summary>
        private FunctionType _currentFunction = FunctionType.NONE;

        /// <summary>
        /// Creates a new resolver
        /// </summary>
        /// <param name="interpreter"></param>
        public Resolver(Interpreter interpreter)
        {
            _interpreter = interpreter; 
        }

        /// <summary>
        /// Resolves a list of statements
        /// </summary>
        /// <param name="statements"></param>
        public void Resolve(List<Stmt> statements)
        {
            foreach (var statement in statements)
            {
                Resolve(statement);
            }
        }

        /// <summary>
        /// Resolves a single statement
        /// </summary>
        /// <param name="stmt"></param>
        private void Resolve(Stmt stmt)
        {
            stmt.Accept(this);
        }

        /// <summary>
        /// Resolves a single expression
        /// </summary>
        /// <param name="expr"></param>
        private void Resolve(Expr expr)
        {
            expr.Accept(this);
        }

        /// <summary>
        /// The stack of scopes
        /// </summary>
        private void BeginScope()
        {
            _scopes.Push(new Dictionary<string, bool>());
        }

        /// <summary>
        /// Ends the current scope
        /// </summary>
        private void EndScope()
        {
            _scopes.Pop();
        }

        /// <summary>
        /// Defines a variable in the current scope
        /// </summary>
        /// <param name="name"></param>
        private void Declare(Token name)
        {
            if (_scopes.Count == 0)
            {
                return;
            }

            var scope = _scopes.Peek();
            if (scope.ContainsKey(name.Lexeme))
            {
                Lox.Error(name, "Variable with this name already declared in this scope.");
            }

            scope[name.Lexeme] = false;
        }

        /// <summary>
        /// Defines a variable in the current scope
        /// </summary>
        /// <param name="name"></param>
        private void Define(Token name)
        {
            if (_scopes.Count == 0)
            {
                return;
            }

            var scope = _scopes.Peek();
            scope[name.Lexeme] = true;
        }

        /// <summary>
        /// Resolve a local variable
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="name"></param>
        private void ResolveLocal(Expr expr, Token name)
        {
            for (var i = 0; i < _scopes.Count; i++)
            {
                if (_scopes.ElementAt(i).ContainsKey(name.Lexeme))
                {
                    _interpreter.Resolve(expr, i);

                    return;
                }
            }
        }

        /// <summary>
        /// Resolves a function's body
        /// </summary>
        /// <param name="function"></param>
        private void ResolveFunction(Function function, FunctionType type)
        {
            var enclosingFunction = _currentFunction;
            _currentFunction = type;

            BeginScope();

            foreach (var param in function.Parameters)
            {
                Declare(param);
                Define(param);

            }
            Resolve(function.Body);
            EndScope();

            _currentFunction = enclosingFunction;
        }

        /// <summary>
        /// Visits a block statement
        /// </summary>
        /// <param name="stmt"></param>
        /// <returns></returns>
        public object VisitBlockStmt(Block stmt)
        {
            BeginScope();
            Resolve(stmt.Statements);
            EndScope();

            return null;
        }

        /// <summary>
        /// Visits a variable declaration statement
        /// </summary>
        /// <param name="stmt"></param>
        /// <returns></returns>
        public object VisitVarStmt(Var stmt)
        {
            Declare(stmt.Name);

            if (stmt.Initialiser != null)
            {
                Resolve(stmt.Initialiser);
            }

            Define(stmt.Name);

            return null;
        }

        /// <summary>
        /// Visits a variable expression
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public object VisitVariableExpr(Variable expr)
        {
            if (_scopes.Count != 0
                && _scopes.Peek().TryGetValue(expr.Name.Lexeme, out var isDefined)
                && !isDefined)
            {
                Lox.Error(expr.Name, "Cannot read local variable in its own initialiser.");
            }

            ResolveLocal(expr, expr.Name);

            return null;
        }

        /// <summary>
        /// Visits an assignment expression
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public object VisitAssignExpr(Assign expr)
        {
            Resolve(expr.Value);
            ResolveLocal(expr, expr.Name);

            return null;
        }

        /// <summary>
        /// Resolves a function declaration
        /// </summary>
        /// <param name="stmt"></param>
        /// <returns></returns>
        public object VisitFunctionStmt(Function stmt)
        {
            Declare(stmt.Name);
            Define(stmt.Name);

            ResolveFunction(stmt, FunctionType.FUNCTION);

            return null;
        }

        /// <summary>
        /// Visits an expression statement
        /// </summary>
        /// <param name="stmt"></param>
        /// <returns></returns>
        public object VisitExpressionStmt(Expression stmt)
        {
            Resolve(stmt.Expr);

            return null;
        }

        /// <summary>
        /// Visits an if statement
        /// </summary>
        /// <param name="stmt"></param>
        /// <returns></returns>
        public object VisitIfStmt(If stmt)
        {
            Resolve(stmt.Condition);
            Resolve(stmt.ThenBranch);

            if (stmt.ElseBranch != null)
            {
                Resolve(stmt.ElseBranch);
            }

            return null;
        }

        /// <summary>
        /// Visits a print statement
        /// </summary>
        /// <param name="stmt"></param>
        /// <returns></returns>
        public object VisitPrintStmt(Print stmt)
        {
            Resolve(stmt.Expr);

            return null;
        }

        /// <summary>
        /// Visits a return statement
        /// </summary>
        /// <param name="stmt"></param>
        /// <returns></returns>
        public object VisitReturnStmt(Return stmt)
        {
            if (_currentFunction == FunctionType.NONE)
            {
                Lox.Error(stmt.Keyword, "Cannot return from top-level code.");
            }

            if (stmt.Value != null)
            {
                Resolve(stmt.Value);
            }

            return null;
        }

        /// <summary>
        /// Visits a while statement
        /// </summary>
        /// <param name="stmt"></param>
        /// <returns></returns>
        public object VisitWhileStmt(While stmt)
        {
            Resolve(stmt.Condition);
            Resolve(stmt.Body);

            return null;
        }

        /// <summary>
        /// Visits a binary expression
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public object VisitBinaryExpr(Binary expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);

            return null;
        }

        /// <summary>
        /// Visits a call expression
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public object VisitCallExpr(Call expr)
        {
            Resolve(expr.Callee);

            foreach (var argument in expr.Arguments)
            {
                Resolve(argument);
            }

            return null;
        }

        /// <summary>
        /// Visits a grouping expression
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public object VisitGroupingExpr(Grouping expr)
        {
            Resolve(expr.Expression);

            return null;
        }

        /// <summary>
        /// Visits a literal expression
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public object VisitLiteralExpr(Literal expr)
        {
            return null;
        }

        /// <summary>
        /// Visits a logical expression
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public object VisitLogicalExpr(Logical expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);

            return null;
        }

        /// <summary>
        /// Visits a unary expression
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public object VisitUnaryExpr(Unary expr)
        {
            Resolve(expr.Right);

            return null;
        }
    }
}
