using static Lox.TokenType;

namespace Lox
{
    /// <summary>
    /// The interpreter that evaluates expressions in the AST.
    /// </summary>
    public class Interpreter : Expr.IVisitor<object>
    {
        public void Interpret(Expr expression)
        {
            try
            {
                var value = Evaluate(expression);
                
                Console.WriteLine(Stringify(value));
            }
            catch (RuntimeError error)
            {
                Program.RuntimeError(error);
            }
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Interpreter"/> class.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public object VisitLiteralExpr(Literal expr)
        {
            return expr.Value;
        }

        /// <summary>
        /// Visit a grouping expression node.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public object VisitGroupingExpr(Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        /// <summary>
        /// Visit a unary expression node.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public object VisitUnaryExpr(Unary expr)
        {
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
        /// Visit a binary expression node.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public object VisitBinaryExpr(Binary expr)
        {
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
                    CheckNumberOperands(expr.Op, left, right);
                    return !Equals(left, right);
                case EQUAL_EQUAL:
                    CheckNumberOperands(expr.Op, left, right);
                    return Equals(left, right);
            }

            // Unreachable.
            return null;
        }

        /// <summary>
        /// Evaluates the given expression and returns the result.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        private object Evaluate(Expr expr)
        {
            return expr.Accept(this);
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
