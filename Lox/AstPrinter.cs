using System.IO;
using System.Text;

namespace Lox
{
    /// <summary>
    /// Prints the AST in a readable format
    /// </summary>
    public class AstPrinter : Expr.IVisitor<string>, Stmt.IVisitor<string>
    {
        /// <summary>
        /// Prints a binary expression
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        /// <summary>
        /// Visits a binary expression
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public string VisitBinaryExpr(Binary expr)
        {
            return Parenthesise(expr.Op.Lexeme, expr.Left, expr.Right);
        }

        /// <summary>
        /// Visits a grouping expression
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public string VisitGroupingExpr(Grouping expr)
        {
            return Parenthesise("group", expr.Expression);
        }

        /// <summary>
        /// Visits a literal expression
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public string VisitLiteralExpr(Literal expr)
        {
            if (expr.Value == null) return "nil";
            return expr.Value.ToString() ?? "";
        }

        /// <summary>
        /// Visits a unary expression
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public string VisitUnaryExpr(Unary expr)
        {
            return Parenthesise(expr.Op.Lexeme, expr.Right);
        }

        /// <summary>
        /// Visits a variable expression
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public string VisitVariableExpr(Variable expr)
        {
            return expr.Name.Lexeme;
        }

        /// <summary>
        /// Visits an assignment expression
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public string VisitAssignExpr(Assign expr)
        {
            return Parenthesise("assign " + expr.Name.Lexeme, expr.Value);
        }

        /// <summary>
        /// Visits a logical expression
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public string VisitLogicalExpr(Logical expr)
        {
            return Parenthesise(expr.Op.Lexeme, expr.Left, expr.Right);
        }

        /// <summary>
        /// Visits a call expression
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public string VisitCallExpr(Call expr)
        {
            return Parenthesise2("call", expr.Callee, expr.Arguments);
        }

        /// <summary>
        /// Visits a get expression
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public string VisitGetExpr(Get expr)
        {
            return Parenthesise("get " + expr.Name.Lexeme, expr.Obj);
        }

        /// <summary>
        /// Visits a set expression
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public string VisitSetExpr(Set expr)
        {
            return Parenthesise("set " + expr.Name.Lexeme, expr.Obj, expr.Value);
        }

        /// <summary>
        /// Visits a 'this' expression
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public string VisitThisExpr(This expr)
        {
            return "this";
        }

        /// <summary>
        /// Visits a 'super' expression
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public string VisitSuperExpr(Super expr)
        {
            return Parenthesise2("super", expr.Method);
        }

        /// <summary>
        /// Visits an block statement
        /// </summary>
        /// <param name="stmt"></param>
        /// <returns></returns>
        public string VisitBlockStmt(Block stmt)
        {
            var builder = new StringBuilder();
            builder.Append("(block");

            foreach (var statement in stmt.Statements)
            {
                builder.Append(" ");
                builder.Append(statement.Accept(this));
            }

            builder.Append(")");

            return builder.ToString();
        }

        /// <summary>
        /// Visits an class statement
        /// </summary>
        /// <param name="stmt"></param>
        /// <returns></returns>
        public string VisitClassStmt(Class stmt)
        {
            var builder = new StringBuilder();
            builder.Append("(class ").Append(stmt.Name.Lexeme);
            
            if (stmt.Superclass != null)
            {
                builder.Append(" < ").Append(stmt.Superclass.Name.Lexeme);
            }
            foreach (var method in stmt.Methods)
            {
                builder.Append(" ");
                builder.Append(method.Accept(this));
            }
            
            builder.Append(")");
            
            return builder.ToString();
        }

        /// <summary>
        /// Visits an expression statement
        /// </summary>
        /// <param name="stmt"></param>
        /// <returns></returns>
        public string VisitExpressionStmt(Expression stmt)
        {
            return Parenthesise(";", stmt.Expr);
        }

        /// <summary>
        /// Visits a function statement
        /// </summary>
        /// <param name="stmt"></param>
        /// <returns></returns>
        public string VisitFunctionStmt(Function stmt)
        {
            var builder = new StringBuilder();
            builder.Append("(fun ").Append(stmt.Name.Lexeme).Append("(");

            foreach(var param in stmt.Parameters)
            {
                if (param != stmt.Parameters[0])
                {
                    builder.Append(", ");
                }

                builder.Append(param.Lexeme);
            }

            builder.Append(")");

            return builder.ToString();
        }

        /// <summary>
        /// Visits a if statement
        /// </summary>
        /// <param name="stmt"></param>
        /// <returns></returns>
        public string VisitIfStmt(If stmt)
        {
            if (stmt.ElseBranch == null)
            {
                return Parenthesise2("if", stmt.Condition, stmt.ThenBranch);
            }
            else
            {
                return Parenthesise2("if-else", stmt.Condition, stmt.ThenBranch, stmt.ElseBranch);
            }
        }

        /// <summary>
        /// Visits a print statement
        /// </summary>
        /// <param name="stmt"></param>
        /// <returns></returns>
        public string VisitPrintStmt(Print stmt)
        {
            return Parenthesise("print", stmt.Expr);
        }

        /// <summary>
        /// Visits a return statement
        /// </summary>
        /// <param name="stmt"></param>
        /// <returns></returns>
        public string VisitReturnStmt(Return stmt)
        {
            if (stmt.Value == null)
            {
                return "(return)";
            }
            else
            {
                return Parenthesise("return", stmt.Value);
            }
        }

        /// <summary>
        /// Visits a var statement
        /// </summary>
        /// <param name="stmt"></param>
        /// <returns></returns>
        public string VisitVarStmt(Var stmt)
        {
            if (stmt.Initialiser == null)
            {
                return Parenthesise2("var", stmt.Name);
            }
            
            return Parenthesise2("var", stmt.Name, "=", stmt.Initialiser);
        }

        /// <summary>
        /// Visits a while statement
        /// </summary>
        /// <param name="stmt"></param>
        /// <returns></returns>
        public string VisitWhileStmt(While stmt)
        {
            return Parenthesise2("while", stmt.Condition, stmt.Body);
        }

        /// <summary>
        /// Parenthesises the expression
        /// </summary>
        /// <param name="name"></param>
        /// <param name="exprs"></param>
        /// <returns></returns>
        private string Parenthesise(string name, params Expr[] exprs)
        {
            var builder = new StringBuilder();
            builder.Append("(").Append(name);

            foreach (var expr in exprs)
            {
                builder.Append(" ");
                builder.Append(expr.Accept(this));
            }

            builder.Append(")");
            return builder.ToString();
        }

        /// <summary>
        /// Parenthesises the expression with one required and multiple additional expressions
        /// </summary>
        /// <param name="name"></param>
        /// <param name="expr"></param>
        /// <param name="exprs"></param>
        /// <returns></returns>
        private string Parenthesise2(string name, params object[] parts)
        {
            var builder = new StringBuilder();
            builder.Append("(").Append(name);
            Transform(builder, parts);
            builder.Append(")");

            return builder.ToString();
        }

        /// <summary>
        /// Transforms the expression to a string and writes it to the given writer
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="parts"></param>
        private void Transform(StringBuilder builder, params object[] parts)
        {
            foreach (var part in parts)
            {
                builder.Append(" ");
                if (part is Expr expr)
                {
                    builder.Append(expr.Accept(this));
                }
                else if (part is Stmt stmt)
                {
                    builder.Append(stmt.Accept(this));   
                }
                else if (part is Token token)
                {
                    builder.Append(token.Lexeme);
                }
                else if (part is List<object> list)
                {
                    Transform(builder, list.ToList<object>());
                }
                else
                {
                    builder.Append(part.ToString());
                }
            }
        }

        /// <summary>
        /// Tests the AST printer
        /// </summary>
        public static void TestPrint()
        {
            // ( -123 ) * ( 45.67 )
            Expr expression = new Binary(
                new Unary(
                    new Token(TokenType.MINUS, "-", null, 1),
                    new Literal(123)),
                new Token(TokenType.STAR, "*", null, 1),
                new Grouping(
                    new Literal(45.67)));

            var printer = new AstPrinter();
            var result = printer.Print(expression);
            Console.WriteLine(result);
        }
    }
}
