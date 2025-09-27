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
            T? VisitBlockStmt(Block stmt);

            /// <summary>
            /// Visit a <see cref="Class"/> statement node.
            /// </summary>
            T? VisitClassStmt(Class stmt);

            /// <summary>
            /// Visit a <see cref="Expression"/> statement node.
            /// </summary>
            T? VisitExpressionStmt(Expression stmt);

            /// <summary>
            /// Visit a <see cref="Function"/> statement node.
            /// </summary>
            T? VisitFunctionStmt(Function stmt);

            /// <summary>
            /// Visit a <see cref="If"/> statement node.
            /// </summary>
            T? VisitIfStmt(If stmt);

            /// <summary>
            /// Visit a <see cref="Print"/> statement node.
            /// </summary>
            T? VisitPrintStmt(Print stmt);

            /// <summary>
            /// Visit a <see cref="Return"/> statement node.
            /// </summary>
            T? VisitReturnStmt(Return stmt);

            /// <summary>
            /// Visit a <see cref="Var"/> statement node.
            /// </summary>
            T? VisitVarStmt(Var stmt);

            /// <summary>
            /// Visit a <see cref="While"/> statement node.
            /// </summary>
            T? VisitWhileStmt(While stmt);

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
            return visitor.VisitBlockStmt(this)!;
        }
    }

    /// <summary>
    /// Represents a Class statement in the abstract syntax tree (AST)
    /// </summary>
    public class Class : Stmt
    {
        public Token Name;

        public Variable? Superclass;

        public List<Function> Methods;

        public Class(Token name, Variable? superclass, List<Function> methods)        {
            Name = name;
            Superclass = superclass;
            Methods = methods;
        }

        /// <summary>
        /// Accepts a visitor and dispatches the call to
        /// <see cref="IVisitor{T}.VisitClassStmt(Class)"/> so the visitor
        /// can perform an operation specific to a Class node.
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
            return visitor.VisitClassStmt(this)!;
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
            return visitor.VisitExpressionStmt(this)!;
        }
    }

    /// <summary>
    /// Represents a Function statement in the abstract syntax tree (AST)
    /// </summary>
    public class Function : Stmt
    {
        public Token Name;

        public List<Token> Parameters;

        public List<Stmt> Body;

        public Function(Token name, List<Token> parameters, List<Stmt> body)        {
            Name = name;
            Parameters = parameters;
            Body = body;
        }

        /// <summary>
        /// Accepts a visitor and dispatches the call to
        /// <see cref="IVisitor{T}.VisitFunctionStmt(Function)"/> so the visitor
        /// can perform an operation specific to a Function node.
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
            return visitor.VisitFunctionStmt(this)!;
        }
    }

    /// <summary>
    /// Represents a If statement in the abstract syntax tree (AST)
    /// </summary>
    public class If : Stmt
    {
        public Expr Condition;

        public Stmt ThenBranch;

        public Stmt? ElseBranch;

        public If(Expr condition, Stmt thenBranch, Stmt? elseBranch)        {
            Condition = condition;
            ThenBranch = thenBranch;
            ElseBranch = elseBranch;
        }

        /// <summary>
        /// Accepts a visitor and dispatches the call to
        /// <see cref="IVisitor{T}.VisitIfStmt(If)"/> so the visitor
        /// can perform an operation specific to a If node.
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
            return visitor.VisitIfStmt(this)!;
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
            return visitor.VisitPrintStmt(this)!;
        }
    }

    /// <summary>
    /// Represents a Return statement in the abstract syntax tree (AST)
    /// </summary>
    public class Return : Stmt
    {
        public Token Keyword;

        public Expr? Value;

        public Return(Token keyword, Expr? value)        {
            Keyword = keyword;
            Value = value;
        }

        /// <summary>
        /// Accepts a visitor and dispatches the call to
        /// <see cref="IVisitor{T}.VisitReturnStmt(Return)"/> so the visitor
        /// can perform an operation specific to a Return node.
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
            return visitor.VisitReturnStmt(this)!;
        }
    }

    /// <summary>
    /// Represents a Var statement in the abstract syntax tree (AST)
    /// </summary>
    public class Var : Stmt
    {
        public Token Name;

        public Expr? Initialiser;

        public Var(Token name, Expr? initialiser)        {
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
            return visitor.VisitVarStmt(this)!;
        }
    }

    /// <summary>
    /// Represents a While statement in the abstract syntax tree (AST)
    /// </summary>
    public class While : Stmt
    {
        public Expr Condition;

        public Stmt Body;

        public While(Expr condition, Stmt body)        {
            Condition = condition;
            Body = body;
        }

        /// <summary>
        /// Accepts a visitor and dispatches the call to
        /// <see cref="IVisitor{T}.VisitWhileStmt(While)"/> so the visitor
        /// can perform an operation specific to a While node.
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
            return visitor.VisitWhileStmt(this)!;
        }
    }

}
