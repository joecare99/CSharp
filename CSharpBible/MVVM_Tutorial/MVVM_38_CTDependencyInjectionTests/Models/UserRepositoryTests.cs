using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM_38_CTDependencyInjection.Models.Interfaces;
using NSubstitute;
using System;
using System.Linq;

namespace MVVM_38_CTDependencyInjection.Models.Tests
{
    [TestClass]
    public class UserRepositoryTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        private ILog _log;
        private UserRepository testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void TestInitialize()
        {
            _log = Substitute.For<ILog>();
            testModel = new(_log);
        }

        [TestMethod()]
        public void SetupTest()
        {
            Assert.IsNotNull(testModel);
            Assert.IsInstanceOfType(testModel, typeof(UserRepository));
            Assert.IsInstanceOfType(testModel, typeof(IUserRepository));
        }

        [TestMethod()]
        public void GetPermissionTest()
        {
            Assert.AreEqual(EPermission.All,testModel.GetPermission(Guid.Empty,Guid.Empty));
        }

        //[TestMethod()]
        //public void RemoveUserTest()
        //{
        //    Assert.Fail();
        //}
        [DataTestMethod()]
        [DataRow("Dave","DavesPw",    "01234567-89AB-CDEF-0123-456789ABCDEF")]
        [DataRow("Joe", "JoesPW",     "11234567-89AB-CDEF-0123-456789ABCDEF")]
        [DataRow("Peter", "PetersPW", "21234567-89AB-CDEF-0123-456789ABCDEF")]
        [DataRow("Dummy", "",         "00000000-0000-0000-0000-000000000000")]
        public void GetUserTest(string sName,string sPassW, string sExp)
        {
            Assert.AreEqual(new Guid(sExp), testModel.GetUser(sName, sPassW));
        }


        [DataTestMethod()]
        [DataRow("Dave",true)]
        [DataRow("Joe",true)]
        [DataRow("Peter",true)]
        [DataRow("Dummy",false)]
        public void GetUsersTest(string sName,bool xExp)
        {
            var t = testModel.GetUsers();
            Assert.IsNotNull(t);
            Assert.AreEqual(xExp,t.Contains(sName));
        }
    }
}
