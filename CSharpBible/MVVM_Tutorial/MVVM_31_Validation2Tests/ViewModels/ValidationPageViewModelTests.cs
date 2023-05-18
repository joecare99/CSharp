using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM_31_Validation1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_31_Validation1.ViewModels.Tests
{
    [TestClass()]
    public class ValidationPageViewModelTests
    {
        ValidationPageViewModel testModel;

        [TestInitialize]
        public void Init()
        {
            testModel = new();
        }

        [DataTestMethod()]
        [DataRow("",1,false)]
        [DataRow("DS", 2, false)]
        [DataRow("DS1234", 0, true)]
        public void TestUsernameTest(string sVal, int iErg, bool xExp)
        {
            bool f(string s) => testModel.TestUsername(s);
            switch (iErg)
            {
                case 1: Assert.ThrowsException<ArgumentNullException>(() => f(sVal)); break;
                case 2: Assert.ThrowsException<ArgumentException>(() => f(sVal)); break;
                default: Assert.AreEqual(xExp, f(sVal)); break;
            }
        }
    }
}