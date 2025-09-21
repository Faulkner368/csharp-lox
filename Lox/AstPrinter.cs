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
