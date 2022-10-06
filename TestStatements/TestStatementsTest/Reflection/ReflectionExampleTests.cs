using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly string cExpExampleMain =
            "Attributes for type ReflectionExample:\r\n   ...is public\r\n   ...is AutoLayout\r\n   ...is a class\r\n   ...is abstract\r\n\r\nAttributes for type NestedClass:\r\n   ...is nested and protected\r\n   ...is AutoLayout\r\n   ...is a class\r\n   ...is sealed\r\n\r\nAttributes for type INested:\r\n   ...is nested and public\r\n   ...is AutoLayout\r\n   ...is an interface\r\n   ...is abstract\r\n\r\nAttributes for type S:\r\n   ...is not public\r\n   ...is SequentialLayout\r\n   ...is a value type\r\n   ...is sealed";

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