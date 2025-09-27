# csLox

A C# implementation of the Lox language from Bob Nystrom’s excellent book [_Crafting Interpreters_](https://craftinginterpreters.com/).

This project ports the **jlox** tree-walk interpreter from Java into idiomatic C#, following the book closely while adapting to .NET where necessary.

---

## 📖 About

Lox is a small, dynamically-typed, object-oriented language designed for learning interpreters.  
This implementation includes:

- **Scanner** → converts source into tokens
- **Parser** → builds an abstract syntax tree (AST)
- **Interpreter** → tree-walk execution of the AST
- **Expressions**: arithmetic, logic, grouping, equality
- **Statements**: print, block, if/else, while, for, return
- **Variables & Scope**: lexical scoping with nested environments
- **Functions**: user-defined, first-class, closures supported
- **Classes**: methods, inheritance, `this`, `super`
- **Native function**: `clock()`

---

## 📂 Project Structure

├─ Lox/
│ ├─ AstPrinter.cs
│ ├─ Enums.cs
│ ├─ Environment.cs
│ ├─ Errors.cs
│ ├─ Expr.cs
│ ├─ ILoxCallable.cs
│ ├─ Interpreter.cs
│ ├─ Lox.cs
│ ├─ Lox.csproj
│ ├─ LoxClass.cs
│ ├─ LoxFunction.cs
│ ├─ LoxInstance.cs
│ ├─ NativeFunctions.cs
│ ├─ Parser.cs
│ ├─ Resolver.cs
│ ├─ ReturnException.cs
│ ├─ Scanner.cs
│ ├─ Stmt.cs
│ └─ Token.cs
├─ Tools/
├─ Extensions.cs
└─ GenerateAst.cs

---

## 🚀 Usage

Run a Lox script:

```powershell
dotnet run -- path/to/script.lox
```

Start an interactive REPL:

```powershell
dotnet run
```

Example session:

```powershell
> print 1 + 2 * 3;
7
> fun add(a, b) { return a + b; }
> print add(10, 20);
30
```

---

## 📜 License

This project is a port and educational exercise.
Please respect the license terms of [_Crafting Interpreters_](https://craftinginterpreters.com/).
