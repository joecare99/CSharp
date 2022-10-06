using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace TestStatements.Anweisungen.Tests
{
    /// <summary>
    /// Defines test class LockingTests.
    /// </summary>
    [TestClass()]
    public class LockingTests
    {
        /// <summary>
        /// The c expected1
        /// </summary>
        private readonly string cExpected1= "Insufficient funds";

        /// <summary>
        /// Defines the test method DoLockTestTest.
        /// </summary>
        [TestMethod()]
        public void DoLockTestTest()
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                string[] TestArgs = { };
                Locking.DoLockTest(TestArgs);

                var result = sw.ToString().Trim();
                Assert.AreEqual(cExpected1, result);
            }   
        }
    }
}