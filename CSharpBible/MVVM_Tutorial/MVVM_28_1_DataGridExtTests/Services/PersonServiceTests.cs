using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BaseLib.Interfaces;
using BaseLib.Helper;
using MVVM.View.Extension;
using MVVM_28_1_DataGridExt.Models;

namespace MVVM_28_1_DataGridExt.Services.Tests
{
    [TestClass]
    public class PersonServiceTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        PersonService testService;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init()
        {
            IoC.GetReqSrv = (t) => t switch
            {
                _ when t == typeof(IRandom) => new CRandom(),
                _ => throw new System.NotImplementedException()
            };
            testService = new PersonService();
        }

        [TestMethod]
        public void GetPersonsTest()
        {
            var result=testService.GetPersons();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result,typeof(IEnumerable<Person>));
            Assert.AreEqual(14, result.Count());
        }

        [TestMethod]
        public void GetDepartmentsTest()
        {
            var result = testService.GetDepartments();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IEnumerable<Department>));
            Assert.AreEqual(4, result.Count());
        }


    }
}
