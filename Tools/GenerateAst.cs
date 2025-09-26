namespace Tools
{
    /// <summary>
    /// Generates the Abstract Syntax Tree (AST) classes for the project.
    /// </summary>
    class GenerateAst
    {
        /// <summary>
        /// The main entry point for the AST generation tool.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: generate_ast <output_directory>");
                Environment.Exit(64);
            }

            var outputDir = args[0];

            DefineAst(outputDir, "Expr", new List<string>()
            {
                "Assign   : Token Name, Expr Value",
                "Binary   : Expr Left, Token Op, Expr Right",
                "Grouping : Expr Expression",
                "Literal  : object Value",
                "Logical  : Expr Left, Token Op, Expr Right",
                "Unary    : Token Op, Expr Right",
                "Variable : Token Name"
            }, "expression");

            DefineAst(outputDir, "Stmt", new List<string>()
            {
                "Block      : List<Stmt> Statements",
                "Expression : Expr Expr",
                "If         : Expr Condition, Stmt ThenBranch, Stmt? ElseBranch",
                "Print      : Expr Expr",
                "Var        : Token Name, Expr Initialiser"
            }, "statement");
        }

        /// <summary>
        /// Defines the AST classes and writes them to a file.
        /// </summary>
        /// <param name="outputDir"></param>
        /// <param name="baseName"></param>
        /// <param name="types"></param>
        /// <param name="nodeType"></param>
        private static void DefineAst(string outputDir, string baseName, List<string> types, string nodeType)
        {
            var path = Path.Combine(outputDir, $"{baseName}.cs");
            using var writer = new StreamWriter(path);

            //writer.WriteLine("using System;");
            //writer.WriteLine("using System.Collections.Generic;");
            //writer.WriteLine();
            writer.WriteLine("namespace Lox");
            writer.WriteLine("{");

            // The base abstract class.
            writer.WriteLine($"    /// <summary>");
            writer.WriteLine($"    /// The base class for all {nodeType} nodes in the AST.");
            writer.WriteLine($"    /// </summary>");
            writer.WriteLine($"    public abstract class " + baseName);
            writer.WriteLine("    {");
            writer.WriteLine($"        /// <summary>");
            writer.WriteLine($"        /// Accepts a visitor that can perform some operation on this {nodeType} node.");
            writer.WriteLine($"        /// </summary>");
            writer.WriteLine($"        /// <typeparam name=\"T\">");
            writer.WriteLine($"        /// The return type produced by the visitor’s operation.");
            writer.WriteLine($"        /// </typeparam>");
            writer.WriteLine($"        /// <param name=\"visitor\">");
            writer.WriteLine($"        /// The visitor instance that implements the operation to perform.");
            writer.WriteLine($"        /// </param>");
            writer.WriteLine($"        /// <returns>");
            writer.WriteLine($"        /// The result of the visitor’s operation, with the type determined by <typeparamref name=\"T\"/>.");
            writer.WriteLine($"        /// </returns>");
            writer.WriteLine($"        public abstract T Accept<T>(IVisitor<T> visitor);");

            DefineVistor(writer, baseName, types, nodeType);

            writer.WriteLine("    }");
            writer.WriteLine();

            // The AST classes.
            foreach (var type in types)
            {
                var className = type.Split(':')[0].Trim();
                var fields = type.Split(':')[1].Trim();
                DefineType(writer, baseName, className, fields, nodeType);
            }

            writer.WriteLine("}");
            writer.Close();
        }

        /// <summary>
        /// Defines the Visitor interface and writes it to the provided StreamWriter.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="baseName"></param>
        /// <param name="types"></param>
        /// <param name="nodeType"></param>
        private static void DefineVistor(StreamWriter writer, string baseName, List<string> types, string nodeType)
        {
            writer.WriteLine();
            writer.WriteLine($"        /// <summary>");
            writer.WriteLine($"        /// Defines the visitor interface for traversing or operating on");
            writer.WriteLine($"        /// different kinds of {nodeType} nodes in the abstract syntax tree (AST).");
            writer.WriteLine($"        /// </summary>");
            writer.WriteLine($"        /// <typeparam name=\"T\">");
            writer.WriteLine($"        /// The return type produced by the visitor’s operation (for example,");
            writer.WriteLine($"        /// a computed value, a string representation, or void if no result).");
            writer.WriteLine($"        /// </typeparam>");
            writer.WriteLine($"        public interface IVisitor<T>");
            writer.WriteLine("        {");

            // The visitor methods for each concrete class.
            foreach (var type in types)
            {
                var typeName = type.Split(':')[0].Trim();
                writer.WriteLine("            /// <summary>");
                writer.WriteLine($"            /// Visit a <see cref=\"{typeName}\"/> {nodeType} node.");
                writer.WriteLine("            /// </summary>");
                writer.WriteLine($"            T Visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
                writer.WriteLine();
            }
            writer.WriteLine("        }");
        }

        /// <summary>
        /// Defines a single AST class and writes it to the provided StreamWriter.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="baseName"></param>
        /// <param name="className"></param>
        /// <param name="fieldList"></param>
        /// <param name="nodeType"></param>
        private static void DefineType(StreamWriter writer, string baseName, string className, string fieldList, string nodeType)
        {
            writer.WriteLine("    /// <summary>");
            writer.WriteLine($"    /// Represents a {className} {nodeType} in the abstract syntax tree (AST)");
            writer.WriteLine("    /// </summary>");
            writer.WriteLine($"    public class {className} : {baseName}");
            writer.WriteLine("    {");
            
            // Fields.
            var fields = fieldList.Split(", ");
            foreach (var field in fields)
            {
                writer.WriteLine($"        public {field};");
                writer.WriteLine();
            }

            // Constructor.
            writer.Write($"        public {className}(");

            for (int i = 0; i < fields.Length; i++)
            {
                var argumentType = fields[i].Split(" ")[0];
                var argumentName = fields[i].Split(" ")[1].FirstCharToLower();
                writer.Write($"{argumentType} {argumentName}");

                if (i < fields.Length - 1)
                {
                    writer.Write(", ");
                }
            }

            writer.Write(")");
            writer.WriteLine("        {");
            
            // Store parameters in fields.
            foreach (var field in fields)
            {
                var name = field.Split(' ')[1];
                writer.WriteLine($"            {name} = {name.FirstCharToLower()};");
            }
            writer.WriteLine("        }");

            // Visitor pattern.
            writer.WriteLine();
            writer.WriteLine($"        /// <summary>");
            writer.WriteLine($"        /// Accepts a visitor and dispatches the call to");
            writer.WriteLine("        /// <see cref=\"IVisitor{T}" + $".Visit{className}{baseName}({className})\"/> so the visitor");
            writer.WriteLine($"        /// can perform an operation specific to a {className} node.");
            writer.WriteLine($"        /// </summary>");
            writer.WriteLine($"        /// <typeparam name=\"T\">");
            writer.WriteLine($"        /// The return type produced by the visitor’s operation.");
            writer.WriteLine($"        /// </typeparam>");
            writer.WriteLine($"        /// <param name=\"visitor\">");
            writer.WriteLine($"        /// The visitor instance performing the operation.");
            writer.WriteLine($"        /// </param>");
            writer.WriteLine($"        /// <returns>");
            writer.WriteLine($"        /// The result of the visitor’s operation.");
            writer.WriteLine($"        /// </returns>");
            writer.WriteLine("        public override T Accept<T>(IVisitor<T> visitor)");
            writer.WriteLine("        {");
            writer.WriteLine($"            return visitor.Visit{className}{baseName}(this);");
            writer.WriteLine("        }");

            writer.WriteLine("    }");
            writer.WriteLine();
        }
    }
}