using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DemoLibrary.Tests
{
    [TestClass()]
    public class DataAccessTests
    {
        private Random _rnd;

        [TestInitialize]
        public void Init()
        {
            _rnd = new Random(0);
            _ = DataAccess.GetNext(0, 5);
            DataAccess.GetNext = (mn, mx) => _rnd.Next(mn, mx);
        }

        [DataTestMethod()]
        [DataRow(0, "Prof. Steve Taylor")]
        [DataRow(1, "Dr. med. Richard Santiago")]
        [DataRow(2, "M.D. Andrew Hernandez")]
        [DataRow(3, "Dr. med. Dany Garcia")]
        [DataRow(4, "M.D. Paula Garcia")]
        [DataRow(5, "Quentin Lee")]
        [DataRow(6, "Dipl.Ing. Quentin Miller")]
        [DataRow(7, "Quentin Smith")]
        [DataRow(8, "M.D. Henry Lee")]
        [DataRow(9, "Dr. med. Karl Widmark")]
        public void GetPeopleTest(int iNr, string sExp)
        {
            var peoples = DataAccess.GetPeople();
            Assert.AreEqual(10, peoples.Count);
            for (int i = 0; i < peoples.Count; i++)
                Assert.AreEqual(sExp, peoples[iNr].FullName, $"p[{iNr}].FullName");
        }

        [DataTestMethod()]
        [DataRow(0, "Prof. Steve Taylor")]
        [DataRow(1, "Dipl.Ing. Georgina Jones")]
        [DataRow(2, "M.D. Urban Miller")]
        [DataRow(3, "Henry Thomas")]
        [DataRow(4, "Victor Widmark")]
        [DataRow(5, "Prof. Dr. Inez Garcia")]
        [DataRow(6, "M.D. Walter Santiago")]
        [DataRow(7, "John Taylor")]
        [DataRow(8, "Xavier Jones")]
        [DataRow(9, "Dr. Karl Miller")]
        [DataRow(10, "Dr. med. Yvonne Lee")]
        [DataRow(11, "M.D. Lenny Smith")]
        public void GetPersonTest(int iVal, string sExp)
        {
            _rnd = new Random(iVal);
            Assert.AreEqual(sExp, DataAccess.GetPerson().FullName);
        }
    }
}