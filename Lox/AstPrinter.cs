using System.IO;
using System.Text;

namespace Lox
{
    /// <summary>
    /// Prints the AST in a readable format
    /// </summary>
    public class AstPrinter : Expr.IVisitor<string>
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
        private string Parenthesise2(string name, Expr expr, List<Expr> exprs)
        {
            var builder = new StringBuilder();
            builder.Append("(").Append(name).Append(" ");
            builder.Append(expr.Accept(this));
         
            foreach (var e in exprs)
            {
                builder.Append(" ");
                builder.Append(e.Accept(this));
            
            }
            builder.Append(")");
            
            return builder.ToString();
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
