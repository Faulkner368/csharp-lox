namespace Lox
{
    /// <summary>
    /// The base class for all expression nodes in the AST.
    /// </summary>
    public abstract class Expr
    {
        /// <summary>
        /// Accepts a visitor that can perform some operation on this expression node.
        /// </summary>
        /// <typeparam name="T">
        /// The return type produced by the visitor’s operation.
        /// </typeparam>
        /// <param name="visitor">
        /// The visitor instance that implements the operation to perform.
        /// </param>
        /// <returns>
        /// The result of the visitor’s operation, with the type determined by <typeparamref name="T"/>.
        /// </returns>
        public abstract T Accept<T>(IVisitor<T> visitor);

        /// <summary>
        /// Defines the visitor interface for traversing or operating on
        /// different kinds of expression nodes in the abstract syntax tree (AST).
        /// </summary>
        /// <typeparam name="T">
        /// The return type produced by the visitor’s operation (for example,
        /// a computed value, a string representation, or void if no result).
        /// </typeparam>
        public interface IVisitor<T>
        {
            /// <summary>
            /// Visit a <see cref="Assign"/> expression node.
            /// </summary>
            T VisitAssignExpr(Assign expr);

            /// <summary>
            /// Visit a <see cref="Binary"/> expression node.
            /// </summary>
            T VisitBinaryExpr(Binary expr);

            /// <summary>
            /// Visit a <see cref="Grouping"/> expression node.
            /// </summary>
            T VisitGroupingExpr(Grouping expr);

            /// <summary>
            /// Visit a <see cref="Literal"/> expression node.
            /// </summary>
            T VisitLiteralExpr(Literal expr);

            /// <summary>
            /// Visit a <see cref="Unary"/> expression node.
            /// </summary>
            T VisitUnaryExpr(Unary expr);

            /// <summary>
            /// Visit a <see cref="Variable"/> expression node.
            /// </summary>
            T VisitVariableExpr(Variable expr);

        }
    }

    /// <summary>
    /// Represents a Assign expression in the abstract syntax tree (AST)
    /// </summary>
    public class Assign : Expr
    {
        public Token Name;

        public Expr Value;

        public Assign(Token name, Expr value)        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Accepts a visitor and dispatches the call to
        /// <see cref="IVisitor{T}.VisitAssignExpr(Assign)"/> so the visitor
        /// can perform an operation specific to a binary expression node.
        /// </summary>
        /// <typeparam name="T">
        /// The return type produced by the visitor’s operation.
        /// </typeparam>
        /// <param name="visitor">
        /// The visitor instance performing the operation.
        /// </param>
        /// <returns>
        /// The result of the visitor’s operation.
        /// </returns>
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitAssignExpr(this);
        }
    }

    /// <summary>
    /// Represents a Binary expression in the abstract syntax tree (AST)
    /// </summary>
    public class Binary : Expr
    {
        public Expr Left;

        public Token Op;

        public Expr Right;

        public Binary(Expr left, Token op, Expr right)        {
            Left = left;
            Op = op;
            Right = right;
        }

        /// <summary>
        /// Accepts a visitor and dispatches the call to
        /// <see cref="IVisitor{T}.VisitBinaryExpr(Binary)"/> so the visitor
        /// can perform an operation specific to a binary expression node.
        /// </summary>
        /// <typeparam name="T">
        /// The return type produced by the visitor’s operation.
        /// </typeparam>
        /// <param name="visitor">
        /// The visitor instance performing the operation.
        /// </param>
        /// <returns>
        /// The result of the visitor’s operation.
        /// </returns>
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitBinaryExpr(this);
        }
    }

    /// <summary>
    /// Represents a Grouping expression in the abstract syntax tree (AST)
    /// </summary>
    public class Grouping : Expr
    {
        public Expr Expression;

        public Grouping(Expr expression)        {
            Expression = expression;
        }

        /// <summary>
        /// Accepts a visitor and dispatches the call to
        /// <see cref="IVisitor{T}.VisitGroupingExpr(Grouping)"/> so the visitor
        /// can perform an operation specific to a binary expression node.
        /// </summary>
        /// <typeparam name="T">
        /// The return type produced by the visitor’s operation.
        /// </typeparam>
        /// <param name="visitor">
        /// The visitor instance performing the operation.
        /// </param>
        /// <returns>
        /// The result of the visitor’s operation.
        /// </returns>
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }
    }

    /// <summary>
    /// Represents a Literal expression in the abstract syntax tree (AST)
    /// </summary>
    public class Literal : Expr
    {
        public object Value;

        public Literal(object value)        {
            Value = value;
        }

        /// <summary>
        /// Accepts a visitor and dispatches the call to
        /// <see cref="IVisitor{T}.VisitLiteralExpr(Literal)"/> so the visitor
        /// can perform an operation specific to a binary expression node.
        /// </summary>
        /// <typeparam name="T">
        /// The return type produced by the visitor’s operation.
        /// </typeparam>
        /// <param name="visitor">
        /// The visitor instance performing the operation.
        /// </param>
        /// <returns>
        /// The result of the visitor’s operation.
        /// </returns>
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }
    }

    /// <summary>
    /// Represents a Unary expression in the abstract syntax tree (AST)
    /// </summary>
    public class Unary : Expr
    {
        public Token Op;

        public Expr Right;

        public Unary(Token op, Expr right)        {
            Op = op;
            Right = right;
        }

        /// <summary>
        /// Accepts a visitor and dispatches the call to
        /// <see cref="IVisitor{T}.VisitUnaryExpr(Unary)"/> so the visitor
        /// can perform an operation specific to a binary expression node.
        /// </summary>
        /// <typeparam name="T">
        /// The return type produced by the visitor’s operation.
        /// </typeparam>
        /// <param name="visitor">
        /// The visitor instance performing the operation.
        /// </param>
        /// <returns>
        /// The result of the visitor’s operation.
        /// </returns>
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }
    }

    /// <summary>
    /// Represents a Variable expression in the abstract syntax tree (AST)
    /// </summary>
    public class Variable : Expr
    {
        public Token Name;

        public Variable(Token name)        {
            Name = name;
        }

        /// <summary>
        /// Accepts a visitor and dispatches the call to
        /// <see cref="IVisitor{T}.VisitVariableExpr(Variable)"/> so the visitor
        /// can perform an operation specific to a binary expression node.
        /// </summary>
        /// <typeparam name="T">
        /// The return type produced by the visitor’s operation.
        /// </typeparam>
        /// <param name="visitor">
        /// The visitor instance performing the operation.
        /// </param>
        /// <returns>
        /// The result of the visitor’s operation.
        /// </returns>
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitVariableExpr(this);
        }
    }

}
