using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestStatements.UnitTesting;

namespace TestStatements.Threading.Tasks.Tests
{
    /// <summary>
    /// Defines test class TaskExampleTests.
    /// Implements the <see cref="ConsoleTestsBase" />
    /// </summary>
    /// <seealso cref="ConsoleTestsBase" />
    [TestClass()]
    public class TaskExampleTests : ConsoleTestsBase
    {
        private readonly string cExpExampleMain =
            "Sending Pings\r\n5 ping attempts failed\r\nSending Pings\r\n5 ping attempts failed\r\nGenerate Numbers\r\nMean: 503,10, n = 1,000\r\nMean: 507,36, n = 1,000\r\nMean: 474,88, n = 1,000\r\nMean: 495,56, n = 1,000\r\nMean: 491,22, n = 1,000\r\nMean: 506,53, n = 1,000\r\nMean: 499,74, n = 1,000\r\nMean: 503,98, n = 1,000\r\nMean: 493,25, n = 1,000\r\nMean: 490,20, n = 1,000\r\n\nMean of Means: 496,00, n = 10,000\r\nGenerate Numbers\r\nMean: 500,63, n = 1,000\r\nMean: 502,50, n = 1,000\r\nMean: 499,13, n = 1,000\r\nMean: 500,59, n = 1,000\r\nMean: 496,34, n = 1,000\r\nMean: 499,99, n = 1,000\r\nMean: 512,83, n = 1,000\r\nMean: 511,17, n = 1,000\r\nMean: 503,20, n = 1,000\r\nMean: 509,64, n = 1,000\r\n\nMean of Means: 503,00, n = 10,000";
        private readonly string cExpExampleMain1 =
            "Sending Pings\r\n5 ping attempts failed";
        private readonly string cExpExampleMain2 =
            "Sending Pings\r\n5 ping attempts failed";
        private readonly string cExpExampleMain3 =
            "Generate Numbers\r\nMean: 503,10, n = 1,000\r\nMean: 507,36, n = 1,000\r\nMean: 474,88, n = 1,000\r\nMean: 495,56, n = 1,000\r\nMean: 491,22, n = 1,000\r\nMean: 506,53, n = 1,000\r\nMean: 499,74, n = 1,000\r\nMean: 503,98, n = 1,000\r\nMean: 493,25, n = 1,000\r\nMean: 490,20, n = 1,000\r\n\nMean of Means: 496,00, n = 10,000";
        private readonly string cExpExampleMain4 =
            "Generate Numbers\r\nMean: 503,10, n = 1,000\r\nMean: 507,36, n = 1,000\r\nMean: 474,88, n = 1,000\r\nMean: 495,56, n = 1,000\r\nMean: 491,22, n = 1,000\r\nMean: 506,53, n = 1,000\r\nMean: 499,74, n = 1,000\r\nMean: 503,98, n = 1,000\r\nMean: 493,25, n = 1,000\r\nMean: 490,20, n = 1,000\r\n\nMean of Means: 496,00, n = 10,000";

        /// <summary>
        /// Defines the test method ExampleMainTest.
        /// </summary>
        [TestMethod()]
        public void ExampleMainTest()
        {
            TaskExample.random = new Random(1234567);
            var result = GetConsoleOutput(TaskExample.ExampleMain);
            Assert.IsTrue(result.Contains("Sending Pings"));
            Assert.IsTrue(result.Contains("Generate Numbers"));
            Assert.IsTrue(result.Contains("Mean of Means:"));
        }

        /// <summary>
        /// Defines the test method ExampleMain1Test.
        /// </summary>
        [TestMethod()]
        public void ExampleMain1Test()
        {
            var result = GetConsoleOutput(TaskExample.ExampleMain1);
            Assert.IsTrue(result.StartsWith("Sending Pings\n"));
            Assert.IsTrue(result.Contains("ping attempts") || result.Contains("All ping attempts succeeded."));
        }

        /// <summary>
        /// Defines the test method ExampleMain2Test.
        /// </summary>
        [TestMethod()]
        public void ExampleMain2Test()
        {
            var result = GetConsoleOutput(TaskExample.ExampleMain2);
            Assert.IsTrue(result.StartsWith("Sending Pings\n"));
            Assert.IsTrue(result.Contains("ping attempts") || result.Contains("All ping attempts succeeded."));
        }

        /// <summary>
        /// Defines the test method ExampleMain3Test.
        /// </summary>
        [TestMethod()]
        public void ExampleMain3Test()
        {
            TaskExample.random = new Random(1234567);
            var result = GetConsoleOutput(TaskExample.ExampleMain3);
            Assert.IsTrue(result.StartsWith("Generate Numbers\n"));
            Assert.IsTrue(result.Contains("Mean of Means:"));
            Assert.AreEqual(10, result.Split('\n').Count(l => l.StartsWith("Mean:")));
        }

        /// <summary>
        /// Defines the test method ExampleMain4Test.
        /// </summary>
        [TestMethod()]
        public void ExampleMain4Test()
        {
            TaskExample.random = new Random(1234567);
            var result = GetConsoleOutput(TaskExample.ExampleMain4);
            Assert.IsTrue(result.StartsWith("Generate Numbers\n"));
            Assert.IsTrue(result.Contains("Mean of Means:"));
            Assert.AreEqual(10, result.Split('\n').Count(l => l.StartsWith("Mean:")));
        }
    }
}