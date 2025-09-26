using static Lox.TokenType;

namespace Lox
{
    /// <summary>
    /// The interpreter that evaluates expressions in the AST.
    /// </summary>
    public class Interpreter : Expr.IVisitor<object>, Stmt.IVisitor<object>
    {
        /// <summary>
        /// Enables or disables debug output.
        /// </summary>
        private bool _debug = false;

        /// <summary>
        /// The global environment that holds built-in functions.
        /// </summary>
        public Environment Globals = new();

        /// <summary>
        /// The environment that holds variable bindings.
        /// </summary>
        private Environment _environment;

        /// <summary>
        /// A mapping of expressions to their resolved depth in the environment chain.
        /// </summary>
        private readonly Dictionary<Expr, int> _locals = new();

        /// <summary>
        /// Initialises a new instance of the <see cref="Interpreter"/> class.
        /// </summary>
        public Interpreter()
        {
            _environment = Globals;
            Globals.Define("clock", new ClockLoxCallable());
        }

        /// <summary>
        /// Interprets a list of statements.
        /// </summary>
        /// <param name="statements"></param>
        public void Interpret(List<Stmt> statements)
        {
            if (_debug) Console.WriteLine("Interpret()");

            try
            {
                foreach (var stmt in statements)
                {
                    Execute(stmt);
                }
            }
            catch (RuntimeError error)
            {
                Lox.RuntimeError(error);
            }
        }

        /// <summary>
        /// Interprets a single expression and returns the result as a string.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public string Interpret(Expr expression)
        {
            try
            {
                var value = Evaluate(expression);
                return Stringify(value);
            }
            catch (RuntimeError error)
            {
                Lox.RuntimeError(error);

                return null;
            }
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Interpreter"/> class.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public object VisitLiteralExpr(Literal expr)
        {
            if (_debug) Console.WriteLine($"VisitLiteralExpr(): {expr.Value}");

            return expr.Value;
        }

        /// <summary>
        /// Visit a logical expression node.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public object VisitLogicalExpr(Logical expr)
        {
            if (_debug) Console.WriteLine($"VisitLogicalExpr(): {expr.Left} {expr.Op} {expr.Right}");
            
            var left = Evaluate(expr.Left);
            
            if (expr.Op.Type == OR)
            {
                if (IsTruthy(left))
                {
                    return left;
                }
            }
            else // AND
            {
                if (!IsTruthy(left))
                {
                    return left;
                }
            }
            
            return Evaluate(expr.Right);
        }

        public object VisitSetExpr(Set expr)
        {
            if (_debug) Console.WriteLine($"VisitSetExpr(): {expr.Obj}.{expr.Name} = {expr.Value}");
            
            var obj = Evaluate(expr.Obj);

            if (obj is not LoxInstance instance)
            {
                throw new RuntimeError(expr.Name, "Only instances have fields.");
            }
            
            var value = Evaluate(expr.Value);
            instance.Set(expr.Name, value);
            
            return value;
        }

        /// <summary>
        /// Visit a grouping expression node.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public object VisitGroupingExpr(Grouping expr)
        {
            if (_debug) Console.WriteLine($"VisitGroupingExpr(): {expr.Expression}");

            return Evaluate(expr.Expression);
        }

        /// <summary>
        /// Visit a unary expression node.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public object VisitUnaryExpr(Unary expr)
        {
            if (_debug) Console.WriteLine($"VisitUnaryExpr(): {expr.Op} {expr.Right}");

            var right = Evaluate(expr.Right);

            switch (expr.Op.Type)
            {
                case MINUS:
                    CheckNumberOperand(expr.Op, right);
                    return -(double)right;
                case BANG:
                    return !IsTruthy(right);
            }

            // Unreachable.
            return null;
        }

        /// <summary>
        /// Visit a variable expression node.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public object VisitVariableExpr(Variable expr)
        {
            if (_debug) Console.WriteLine($"VisitVariableExpr(): {expr.Name}");

            return LookUpVariable(expr.Name, expr);
        }

        private object LookUpVariable(Token name, Expr expr)
        {
            if (_locals.TryGetValue(expr, out int distance))
            {
                return _environment.GetAt(distance, name.Lexeme);
            }
            else
            {
                return Globals.Get(name);
            }
        }

        /// <summary>
        /// Visit a binary expression node.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        /// <exception cref="RuntimeError"></exception>"
        public object VisitBinaryExpr(Binary expr)
        {
            if (_debug) Console.WriteLine($"VisitBinaryExpr(): {expr.Left} {expr.Op} {expr.Right}");

            var left = Evaluate(expr.Left);
            var right = Evaluate(expr.Right);

            switch (expr.Op.Type)
            {
                case MINUS:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left - (double)right;
                case SLASH:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left / (double)right;
                case STAR:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left * (double)right;
                case PLUS:
                    if (left is double l && right is double r)
                    {
                        return l + r;
                    }
                    if (left is string ls && right is string rs)
                    {
                        return ls + rs;
                    }

                    throw new RuntimeError(expr.Op, "Operands must be two numbers or two strings.");

                    break;

                case GREATER:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left > (double)right;
                case GREATER_EQUAL:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left >= (double)right;
                case LESS:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left < (double)right;
                case LESS_EQUAL:
                    CheckNumberOperands(expr.Op, left, right);
                    return (double)left <= (double)right;

                case BANG_EQUAL:
                    return !Equals(left, right);
                case EQUAL_EQUAL:
                    return Equals(left, right);
            }

            // Unreachable.
            return null;
        }

        /// <summary>
        /// Visit a call expression node.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        /// <exception cref="RuntimeError"></exception>
        public object VisitCallExpr(Call expr)
        {
            if (_debug) Console.WriteLine($"VisitCallExpr(): {expr.Callee}({string.Join(", ", expr.Arguments)})");
            
            var callee = Evaluate(expr.Callee);
            
            var arguments = new List<object>();
            foreach (var argument in expr.Arguments)
            {
                arguments.Add(Evaluate(argument));
            
            }
            
            if (callee is not ILoxCallable callable)
            {
                throw new RuntimeError(expr.Paren, "Can only call functions and classes.");
            }

            if (arguments.Count != callable.Arity())
            {
                throw new RuntimeError(expr.Paren, $"Expected {callable.Arity()} arguments but got {arguments.Count}.");
            }

            return callable.Call(this, arguments);
        }

        public object VisitGetExpr(Get expr)
        {
            if (_debug) Console.WriteLine($"VisitGetExpr(): {expr.Obj}.{expr.Name}");
            
            var obj = Evaluate(expr.Obj);
            if (obj is LoxInstance instance)
            {
                return instance.Get(expr.Name);
            }
            
            throw new RuntimeError(expr.Name, "Only instances have properties.");
        }
        /// <summary>
        /// Evaluates the given expression and returns the result.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        private object Evaluate(Expr expr)
        {
            if (_debug) Console.WriteLine("Evaluate()");

            return expr.Accept(this);
        }

        /// <summary>
        /// Executes the given statement.
        /// </summary>
        /// <param name="stmt"></param>
        private void Execute(Stmt stmt)
        {
            if (_debug) Console.WriteLine("Execute()");
            
            stmt.Accept(this);
        }

        /// <summary>
        /// Resolves a variable expression to a specific depth in the environment chain.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="depth"></param>
        public void Resolve(Expr expr, int depth)
        {
            _locals[expr] = depth;  
        }

        /// <summary>
        /// Executes a block of statements in the given environment.
        /// </summary>
        /// <param name="statements"></param>
        /// <param name="environment"></param>
        public void ExecuteBlock(List<Stmt> statements, Environment environment)
        {
            if (_debug) Console.WriteLine("ExecuteBlock()");

            var previous = _environment;
            try
            {
                _environment = environment;
                foreach (var statement in statements)
                {
                    Execute(statement);
                }
            }
            finally
            {
                _environment = previous;
            }
        }

        /// <summary>
        /// Executes a block of statements in a new environment.
        /// </summary>
        /// <param name="stmt"></param>
        /// <returns></returns>
        public object VisitBlockStmt(Block stmt)
        {
            if (_debug) Console.WriteLine("VisitBlockStmt()");

            ExecuteBlock(stmt.Statements, new Environment(_environment));
            
            return null;
        }

        /// <summary>
        /// Visit a class declaration statement node.
        /// </summary>
        /// <param name="stmt"></param>
        /// <returns></returns>
        public object VisitClassStmt(Class stmt)
        {
            if (_debug) Console.WriteLine($"VisitClassStmt(): {stmt.Name}");
            
           _environment.Define(stmt.Name.Lexeme, null);
            var klass = new LoxClass(stmt.Name.Lexeme);
            _environment.Assign(stmt.Name, klass);

            return null;
        }

        /// <summary>
        /// Visit an expression statement node.
        /// </summary>
        /// <param name="stmt"></param>
        public object VisitExpressionStmt(Expression stmt)
        {
            if (_debug) Console.WriteLine($"VisitExpressionStmt(): {stmt.Expr}");

            Evaluate(stmt.Expr);

            return null;
        }

        /// <summary>
        /// Visit a function declaration statement node.
        /// </summary>
        /// <param name="stmt"></param>
        /// <returns></returns>
        public object VisitFunctionStmt(Function stmt)
        {
            if (_debug)
            {
                Console.WriteLine($"VisitFunctionStmt(): {stmt.Name}({string.Join(", ", stmt.Parameters)}) {{ ... }}");
            }
            
            var function = new LoxFunction(stmt, _environment, false);
            _environment.Define(stmt.Name.Lexeme, function);
            
            return null;
        }

        /// <summary>
        /// Visit an if statement node.
        /// </summary>
        /// <param name="stmt"></param>
        /// <returns></returns>
        public object VisitIfStmt(If stmt)
        {
            if (_debug) Console.WriteLine($"VisitIfStmt(): {stmt.Condition} ? {stmt.ThenBranch} : {stmt.ElseBranch}");
            
            if (IsTruthy(Evaluate(stmt.Condition)))
            {
                Execute(stmt.ThenBranch);
            }
            else if (stmt.ElseBranch != null)
            {
                Execute(stmt.ElseBranch);
            }
            
            return null;
        }

        /// <summary>
        /// Visit a print statement node.
        /// </summary>
        /// <param name="stmt"></param>
        public object VisitPrintStmt(Print stmt)
        {
            if (_debug) Console.WriteLine($"VisitPrintStmt(): {stmt.Expr}");

            var value = Evaluate(stmt.Expr);
            Console.WriteLine(Stringify(value));

            return null;
        }

        /// <summary>
        /// Visit a return statement node.
        /// </summary>
        /// <param name="stmt"></param>
        /// <returns></returns>
        /// <exception cref="ReturnException"></exception>
        public object VisitReturnStmt(Return stmt)
        {
            if (_debug) Console.WriteLine($"VisitReturnStmt(): {stmt.Value}");
            
            object value = null;
            if (stmt.Value != null)
            {
                value = Evaluate(stmt.Value);
            }
            
            throw new ReturnException(value);
        }

        /// <summary>
        /// Visit a variable declaration statement node.
        /// </summary>
        /// <param name="stmt"></param>
        /// <returns></returns>
        public object VisitVarStmt(Var stmt)
        {
            if (_debug) Console.WriteLine($"VisitVarStmt(): {stmt.Name}");

            object value = null;
            if (stmt.Initialiser != null)
            {
                value = Evaluate(stmt.Initialiser);
            }

            _environment.Define(stmt.Name.Lexeme, value);

            return null;
        }

        /// <summary>
        /// Visit a while statement node.
        /// </summary>
        /// <param name="stmt"></param>
        /// <returns></returns>
        public object VisitWhileStmt(While stmt)
        {
            
            if (_debug) Console.WriteLine($"VisitWhileStmt(): {stmt.Condition} {{ {stmt.Body} }}");
            
            while (IsTruthy(Evaluate(stmt.Condition)))
            {
                Execute(stmt.Body);
            }
            
            return null;
        }

        /// <summary>
        /// Visit a assign statement node.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public object VisitAssignExpr(Assign expr)
        {
            if (_debug) Console.WriteLine($"VisitAssignExpr(): {expr.Name} = {expr.Value}");

            var value = Evaluate(expr.Value);
            
            if (_locals.TryGetValue(expr, out int distance))
            {
                _environment.AssignAt(distance, expr.Name, value);
            }
            else
            {
                Globals.Assign(expr.Name, value);
            }

            return value;
        }

        /// <summary>
        /// Evaluates a truthy value.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool IsTruthy(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is bool b)
            {
                return b;
            }

            return true;
        }

        /// <summary>
        /// Evaluates equality between two objects.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private bool IsEqual(object a, object b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        /// <summary>
        /// Checks if the operand is a number.
        /// </summary>
        /// <param name="op"></param>
        /// <param name="operand"></param>
        /// <exception cref="RuntimeError"></exception>
        private void CheckNumberOperand(Token op, object operand)
        {
            if (operand is double)
            {
                return;
            }

            throw new RuntimeError(op, "Operand must be a number.");
        }

        /// <summary>
        /// Checks if both operands are numbers.
        /// </summary>
        /// <param name="op"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <exception cref="RuntimeError"></exception>
        private void CheckNumberOperands(Token op, object left, object right)
        {
            if (left is double && right is double)
            {
                return;
            }

            throw new RuntimeError(op, "Operands must be numbers.");
        }

        /// <summary>
        /// Converts the given object to a string representation.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private string Stringify(object obj)
        {
            if (obj == null)
            {
                return "nil";
            }

            if (obj is double d)
            {
                var text = d.ToString();

                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }

                return text;
            }

            return obj.ToString() ?? "";
        }
    }
}
