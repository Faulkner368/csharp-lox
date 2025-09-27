using FluentAssertions;
using Xunit;

namespace Lox.Tests
{
    public class InterpreterTests
    {
        // Helper to handle platform-specific newlines
        private static string Lines(params string[] lines) =>
            string.Join(System.Environment.NewLine, lines);

        // --- Literals and Printing ---
        [Fact]
        public void Should_PrintLiterals()
        {
            TestHelper.RunLox("print 123;").Should().Be("123");
            TestHelper.RunLox("print \"hello\";").Should().Be("hello");
            TestHelper.RunLox("print true;").Should().Be("true");
            TestHelper.RunLox("print false;").Should().Be("false");
            TestHelper.RunLox("print nil;").Should().Be("nil");
        }

        // --- Variables and Scope ---
        [Fact]
        public void Should_HandleVariableDeclarationsAndAssignments()
        {
            TestHelper.RunLox("var a = 42; print a;").Should().Be("42");
            TestHelper.RunLox("var a; a = 99; print a;").Should().Be("99");
        }

        [Fact]
        public void Should_HandleBlockScope()
        {
            TestHelper.RunLox("var a = 1; { var a = 2; print a; } print a;")
                .Should().Be(Lines("2", "1"));
        }

        // --- Arithmetic and Operators ---
        [Fact]
        public void Should_EvaluateBinaryExpressions()
        {
            TestHelper.RunLox("print 1 + 2 * 3;").Should().Be("7");
            TestHelper.RunLox("print (1 + 2) * 3;").Should().Be("9");
            TestHelper.RunLox("print 5 - 2;").Should().Be("3");
            TestHelper.RunLox("print 8 / 2;").Should().Be("4");
        }

        [Fact]
        public void Should_EvaluateUnaryExpressions()
        {
            TestHelper.RunLox("print -5;").Should().Be("-5");
            TestHelper.RunLox("print !true;").Should().Be("false");
            TestHelper.RunLox("print !false;").Should().Be("true");
        }

        // --- Control Flow ---
        [Fact]
        public void Should_HandleIfElse()
        {
            TestHelper.RunLox("if (true) print 1; else print 2;")
                .Should().Be("1");
            TestHelper.RunLox("if (false) print 1; else print 2;")
                .Should().Be("2");
        }

        [Fact]
        public void Should_HandleWhileLoop()
        {
            TestHelper.RunLox(@"
                var i = 0;
                while (i < 3) { print i; i = i + 1; }
            ").Should().Be(Lines("0", "1", "2"));
        }

        [Fact]
        public void Should_HandleForLoop()
        {
            TestHelper.RunLox(@"
                for (var i = 1; i <= 3; i = i + 1) { print i; }
            ").Should().Be(Lines("1", "2", "3"));
        }

        // --- Functions and Closures ---
        [Fact]
        public void Should_CallFunction()
        {
            TestHelper.RunLox(@"
                fun add(a, b) { return a + b; }
                print add(2, 3);
            ").Should().Be("5");
        }

        [Fact]
        public void Should_HandleClosures()
        {
            TestHelper.RunLox(@"
                fun makeCounter() { 
                    var i = 0; 
                    fun c() { i = i + 1; return i; } 
                    return c; 
                }
                var counter = makeCounter();
                print counter();
                print counter();
            ").Should().Be(Lines("1", "2"));
        }

        [Fact]
        public void Should_ReturnFromFunction()
        {
            TestHelper.RunLox(@"
                fun f() { return 123; }
                print f();
            ").Should().Be("123");
        }

        // --- Classes and Inheritance ---
        [Fact]
        public void Should_CreateClassAndCallMethod()
        {
            TestHelper.RunLox(@"
                class Foo { sayHi() { print ""hi""; } }
                Foo().sayHi();
            ").Should().Be("hi");
        }

        [Fact]
        public void Should_HandleThis()
        {
            TestHelper.RunLox(@"
                class Foo { say() { print this; } }
                print Foo();
            ").Should().Be("Foo instance");
        }

        [Fact]
        public void Should_HandleSuper()
        {
            TestHelper.RunLox(@"
                class A { method() { print ""A""; } }
                class B < A { method() { super.method(); print ""B""; } }
                B().method();
            ").Should().Be(Lines("A", "B"));
        }

        // --- Logical Operators ---
        [Fact]
        public void Should_HandleLogicalOperators()
        {
            TestHelper.RunLox("print true or false;").Should().Be("true");
            TestHelper.RunLox("print false or false;").Should().Be("false");
            TestHelper.RunLox("print true and false;").Should().Be("false");
            TestHelper.RunLox("print true and true;").Should().Be("true");
        }

        // --- Builtins ---
        [Fact]
        public void Should_CallClockBuiltin()
        {
            var result = TestHelper.RunLox("print clock();");
            result.Should().NotBeNull();
            double.Parse(result).Should().BeGreaterThan(0);
        }

        // --- Error Cases ---

        [Fact]
        public void Should_ReportUndefinedVariable()
        {
            var result = TestHelper.RunLox("print foo;");
            result.Should().Contain("Undefined variable 'foo'");
        }

        [Fact]
        public void Should_ReportArityMismatch()
        {
            var result = TestHelper.RunLox("fun f(a) { print a; } f();");
            result.Should().Contain("Expected 1 arguments but got 0.");
        }

        [Fact]
        public void Should_ReportInvalidOperand()
        {
            var result = TestHelper.RunLox("print \"a\" - \"b\";");
            result.Should().Contain("Operands must be numbers");
        }

        [Fact]
        public void Should_ReportInvalidAssignmentTarget()
        {
            var result = TestHelper.RunLox("123 = 45;");
            result.Should().Contain("Invalid assignment target");
        }
        // === Literals and Expressions ===

        [Fact]
        public void NumberLiteral()
        {
            var result = TestHelper.RunLox("print 123;");
            result.Should().Be("123");
        }

        [Fact]
        public void StringLiteral()
        {
            var result = TestHelper.RunLox("print \"hello\";");
            result.Should().Be("hello");
        }

        [Fact]
        public void BooleanLiteral()
        {
            var result = TestHelper.RunLox("print true;");
            result.Should().Be("true");
        }

        [Fact]
        public void NilLiteral()
        {
            var result = TestHelper.RunLox("print nil;");
            result.Should().Be("nil");
        }

        [Fact]
        public void GroupingAndUnary()
        {
            var result = TestHelper.RunLox("print -(1 + 2) * 3;");
            result.Should().Be("-9");
        }

        [Fact]
        public void ComparisonAndEquality()
        {
            var result = TestHelper.RunLox("print 3 > 2 == true;");
            result.Should().Be("true");
        }

        // === Variables ===

        [Fact]
        public void VariableDeclaration()
        {
            var result = TestHelper.RunLox("var a = 42; print a;");
            result.Should().Be("42");
        }

        [Fact]
        public void VariableShadowing()
        {
            var result = TestHelper.RunLox("var a = 1; { var a = 2; print a; } print a;");
            result.Should().Be("2\r\n1");
        }

        [Fact]
        public void UndefinedVariable()
        {
            var result = TestHelper.RunLox("print foo;");
            result.Should().Contain("Undefined variable 'foo'");
        }

        // === Functions ===

        [Fact]
        public void FunctionDefinitionAndCall()
        {
            var result = TestHelper.RunLox("fun add(a, b) { return a + b; } print add(2, 3);");
            result.Should().Be("5");
        }

        [Fact]
        public void ClosureCapturesVariable()
        {
            var result = TestHelper.RunLox(@"
                fun makeCounter() {
                  var i = 0;
                  fun count() {
                    i = i + 1;
                    return i;
                  }
                  return count;
                }
                var counter = makeCounter();
                print counter(); 
                print counter();
            ");
            result.Should().Be("1\r\n2");
        }

        [Fact]
        public void RecursionFactorial()
        {
            var result = TestHelper.RunLox(@"
                fun fact(n) {
                  if (n <= 1) return 1;
                  return n * fact(n - 1);
                }
                print fact(5);
            ");
            result.Should().Be("120");
        }

        // === Control Flow ===

        [Fact]
        public void IfElse()
        {
            var result = TestHelper.RunLox("if (true) print 1; else print 2;");
            result.Should().Be("1");
        }

        [Fact]
        public void WhileLoop()
        {
            var result = TestHelper.RunLox(@"
                var i = 0;
                while (i < 3) {
                  print i;
                  i = i + 1;
                }
            ");
            result.Should().Be("0\r\n1\r\n2");
        }

        [Fact]
        public void ForLoop()
        {
            var result = TestHelper.RunLox(@"
                for (var i = 1; i <= 3; i = i + 1) {
                  print i;
                }
            ");
            result.Should().Be("1\r\n2\r\n3");
        }

        // --- Classes: Initializers and Methods ---
        [Fact]
        public void Should_CallClassInitializer()
        {
            TestHelper.RunLox(@"
        class Foo {
            init(a) { this.a = a; }
        }
        var f = Foo(42);
        print f.a;
    ").Should().Be("42");
        }

        [Fact]
        public void Should_ReturnFromMethod()
        {
            TestHelper.RunLox(@"
        class Bar {
            get() { return 7; }
        }
        print Bar().get();
    ").Should().Be("7");
        }

        // --- Superclass Chaining ---
        [Fact]
        public void Should_HandleSuperInMultiLevelInheritance()
        {
            TestHelper.RunLox(@"
        class A { method() { print ""A""; } }
        class B < A { method() { super.method(); print ""B""; } }
        class C < B { method() { super.method(); print ""C""; } }
        C().method();
    ").Should().Be(Lines("A", "B", "C"));
        }

        // --- Edge Cases ---
        [Fact]
        public void Should_HandleEmptyBlock()
        {
            TestHelper.RunLox(@"
        { }
        print 123;
    ").Should().Be("123");
        }

        [Fact]
        public void Should_HandleEmptyFunction()
        {
            TestHelper.RunLox(@"
        fun f() { }
        print f();
    ").Should().Be("nil");
        }

        [Fact]
        public void Should_HandleNestedLoops()
        {
            TestHelper.RunLox(@"
        for (var i = 1; i <= 2; i = i + 1) {
            for (var j = 1; j <= 2; j = j + 1) {
                print i * j;
            }
        }
    ").Should().Be(Lines("1", "2", "2", "4"));
        }
    }
}
