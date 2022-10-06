using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.Anweisungen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatements.Anweisungen.Tests
{
    /// <summary>
    /// Defines test class AccountTests.
    /// </summary>
    [TestClass()]
    public class AccountTests
    {
        private Account Faccount = null;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestInitialize()]
        public void Init()
        {
            Faccount = new Account();
        }

        /// <summary>
        /// Defines the test method TestSetup.
        /// </summary>
        [TestMethod()]
        public void TestSetup()
        {
            Assert.IsNotNull(Faccount);
            Assert.AreEqual(0, Faccount.Balance);
        }

        /// <summary>
        /// Defines the test method AccountTest.
        /// </summary>
        [TestMethod()]
        public void AccountTest()
        {
            Assert.IsInstanceOfType(Faccount, typeof(Account));
        }

        /// <summary>
        /// Defines the test method WithdrawTest.
        /// </summary>
        [TestMethod()]
        public void WithdrawTest()
        {
            Faccount.Withdraw(-50);
            Assert.AreEqual(50,Faccount.Balance);
            Faccount.Withdraw(20);
            Assert.AreEqual(30, Faccount.Balance);
            Assert.ThrowsException<Exception>(delegate { Faccount.Withdraw(31); });
            Assert.AreEqual(30, Faccount.Balance);
        }
    }
}
