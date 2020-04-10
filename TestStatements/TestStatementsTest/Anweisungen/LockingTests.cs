using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace TestStatements.Anweisungen.Tests
{
    [TestClass()]
    public class LockingTests
    {
        private readonly string cExpected1= "Insufficient funds";

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