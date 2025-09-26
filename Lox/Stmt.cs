namespace Lox
{
    /// <summary>
    /// The base class for all statement nodes in the AST.
    /// </summary>
    public abstract class Stmt
    {
        /// <summary>
        /// Accepts a visitor that can perform some operation on this statement node.
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
        /// different kinds of statement nodes in the abstract syntax tree (AST).
        /// </summary>
        /// <typeparam name="T">
        /// The return type produced by the visitor’s operation (for example,
        /// a computed value, a string representation, or void if no result).
        /// </typeparam>
        public interface IVisitor<T>
        {
            /// <summary>
            /// Visit a <see cref="Block"/> statement node.
            /// </summary>
            T VisitBlockStmt(Block stmt);

            /// <summary>
            /// Visit a <see cref="Expression"/> statement node.
            /// </summary>
            T VisitExpressionStmt(Expression stmt);

            /// <summary>
            /// Visit a <see cref="Print"/> statement node.
            /// </summary>
            T VisitPrintStmt(Print stmt);

            /// <summary>
            /// Visit a <see cref="Var"/> statement node.
            /// </summary>
            T VisitVarStmt(Var stmt);

        }
    }

    /// <summary>
    /// Represents a Block statement in the abstract syntax tree (AST)
    /// </summary>
    public class Block : Stmt
    {
        public List<Stmt> Statements;

        public Block(List<Stmt> statements)        {
            Statements = statements;
        }

        /// <summary>
        /// Accepts a visitor and dispatches the call to
        /// <see cref="IVisitor{T}.VisitBlockStmt(Block)"/> so the visitor
        /// can perform an operation specific to a Block node.
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
            return visitor.VisitBlockStmt(this);
        }
    }

    /// <summary>
    /// Represents a Expression statement in the abstract syntax tree (AST)
    /// </summary>
    public class Expression : Stmt
    {
        public Expr Expr;

        public Expression(Expr expr)        {
            Expr = expr;
        }

        /// <summary>
        /// Accepts a visitor and dispatches the call to
        /// <see cref="IVisitor{T}.VisitExpressionStmt(Expression)"/> so the visitor
        /// can perform an operation specific to a Expression node.
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
            return visitor.VisitExpressionStmt(this);
        }
    }

    /// <summary>
    /// Represents a Print statement in the abstract syntax tree (AST)
    /// </summary>
    public class Print : Stmt
    {
        public Expr Expr;

        public Print(Expr expr)        {
            Expr = expr;
        }

        /// <summary>
        /// Accepts a visitor and dispatches the call to
        /// <see cref="IVisitor{T}.VisitPrintStmt(Print)"/> so the visitor
        /// can perform an operation specific to a Print node.
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
            return visitor.VisitPrintStmt(this);
        }
    }

    /// <summary>
    /// Represents a Var statement in the abstract syntax tree (AST)
    /// </summary>
    public class Var : Stmt
    {
        public Token Name;

        public Expr Initialiser;

        public Var(Token name, Expr initialiser)        {
            Name = name;
            Initialiser = initialiser;
        }

        /// <summary>
        /// Accepts a visitor and dispatches the call to
        /// <see cref="IVisitor{T}.VisitVarStmt(Var)"/> so the visitor
        /// can perform an operation specific to a Var node.
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
            return visitor.VisitVarStmt(this);
        }
    }

}
