using FluentAssertions;
using Xunit;

namespace Lox.Tests
{
    public class InterpreterTests
    {
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
    }
}
