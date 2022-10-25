using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DemoLibrary.Tests
{
    [TestClass()]
    public class DataAccessTests
    {
        private Random _rnd;
        string[] sExp = new string[] {
            "Prof. Monique Taylor",
            "M.D. Andrew Thomas",
            "Karl Lee",
            "Dibl.Ing. Inez Jones",
            "Lenny Lee",
            "M.D. Frank Thomas",
            "Bob Widmark",
            "Lenny Widmark",
            "Prof. Karl Taylor",
            "Prof. Carla Taylor"
        };

        [TestInitialize]
        public void Init()
        {
            _rnd = new Random(0);
            DataAccess.GetNext = (mn,mx) => _rnd.Next(mn,mx);
        }

        [DataTestMethod()]
        public void GetPeopleTest()
        {
            var peoples = DataAccess.GetPeople();
            Assert.AreEqual(10,peoples.Count);
            for (int i = 0; i < peoples.Count; i++)
                Assert.AreEqual(sExp[i], peoples[i].FullName,$"p[{i}].Fullname");
        }

        [DataTestMethod()]
        [DataRow(0, "Prof. Monique Taylor")]
        [DataRow(1, "Dibl.Ing. Earl Jones")]
        [DataRow(2, "M.D. Norbert Miller")]
        [DataRow(3, "Earl Thomas")]
        [DataRow(4, "Norbert Widmark")]
        [DataRow(5, "Prof. Dr. Frank Garcia")]
        [DataRow(6, "M.D. Oscar Santiago")]
        [DataRow(7, "Georina Taylor")]
        [DataRow(8, "Paula Jones")]
        [DataRow(9, "Dr. Henry Miller")]
        [DataRow(10, "Dr. med. Richard Lee")]
        [DataRow(11, "M.D. Inez Smith")]
        public void GetPersonTest(int iVal, string sExp)
        {
            _rnd = new Random(iVal);
            Assert.AreEqual(sExp,DataAccess.GetPerson().FullName);
        }
    }
}