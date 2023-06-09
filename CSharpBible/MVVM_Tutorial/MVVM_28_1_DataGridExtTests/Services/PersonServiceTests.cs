﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM_28_1_DataGridExt.Models;
using System.Collections.Generic;
using System.Linq;

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