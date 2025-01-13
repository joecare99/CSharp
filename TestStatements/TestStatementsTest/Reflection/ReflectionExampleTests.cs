using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.UnitTesting;

namespace TestStatements.Reflection.Tests
{
    /// <summary>
    /// Defines test class ReflectionExampleTests.
    /// Implements the <see cref="ConsoleTestsBase" />
    /// </summary>
    /// <seealso cref="ConsoleTestsBase" />
    [TestClass()]
    public class ReflectionExampleTests : ConsoleTestsBase
    {
        private readonly string cExpExampleMain = @"======================================================================
## Example for Reflection (ReflectionExample)
======================================================================

Attributes for type ReflectionExample:
   ...is public
   ...is AutoLayout
   ...is a class
   ...is abstract

Attributes for type NestedClass:
   ...is nested and protected
   ...is AutoLayout
   ...is a class
   ...is sealed

Attributes for type INested:
   ...is nested and public
   ...is AutoLayout
   ...is an interface
   ...is abstract

Attributes for type S:
   ...is not public
   ...is SequentialLayout
   ...is a value type
   ...is sealed";

        /// <summary>
        /// Defines the test method ExampleMainTest.
        /// </summary>
        [TestMethod()]
        public void ExampleMainTest()
        {
            AssertConsoleOutput(cExpExampleMain,ReflectionExample.ExampleMain);
        }
    }
}