using Microsoft.VisualStudio.TestTools.UnitTesting;
using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Helper.Tests
{
    [TestClass()]
    public class ListItemExtensionsTests
    {
        private IList _testSData;
        private IList _testIData;
        private IList _testNData;

        [TestInitialize]
        public void TestInitialize()
        {
            _testSData = new List<ListItem>() { new("A", "1"), new("B", "2"), new("C", "3") };
            _testIData = new List<ListItem>() { new("D", 1), new("E", 2), new("F", 3) };
            _testNData = new List<int>() { 4, 5, 6 };
        }

        [TestMethod()]
        public void TestSetup()
        {
            Assert.IsNotNull(_testSData);
            Assert.IsInstanceOfType(_testSData, typeof(IList));
            Assert.IsInstanceOfType(_testSData, typeof(List<ListItem>));
            Assert.IsNotNull(_testIData);
            Assert.IsInstanceOfType(_testSData, typeof(IList));
            Assert.IsInstanceOfType(_testSData, typeof(List<ListItem>));
            Assert.IsNotNull(_testNData);
            Assert.IsInstanceOfType(_testNData, typeof(IList));
            Assert.IsInstanceOfType(_testNData, typeof(List<int>));
        }

        [DataTestMethod()]
        [DataRow(nameof(_testSData), 0, "1")]
        [DataRow(nameof(_testSData), 1, "2")]
        [DataRow(nameof(_testSData), 2, "3")]
        [DataRow(nameof(_testSData), 3, null)]
        [DataRow(nameof(_testIData), 0, 1)]
        [DataRow(nameof(_testIData), 1, 2)]
        [DataRow(nameof(_testIData), 2, 3)]
        [DataRow(nameof(_testIData), 3, null)]
        [DataRow(nameof(_testNData), 0, null)]
        [DataRow(nameof(_testNData), 1, null)]
        [DataRow(nameof(_testNData), 2, null)]
        [DataRow(nameof(_testNData), 3, null)]
        public void ItemDataTest(string actData, int iAct, object? oExp)
        {
            var act = actData switch
            {
                nameof(_testSData) => _testSData,
                nameof(_testIData) => _testIData,
                nameof(_testNData) => _testNData,
                _ => throw new ArgumentException("Invalid test data")
            };
            Assert.AreEqual(oExp, act.ItemData(iAct));
        }

        [DataTestMethod()]
        [DataRow(nameof(_testSData), 0, "1")]
        [DataRow(nameof(_testSData), 1, "2")]
        [DataRow(nameof(_testSData), 2, "3")]
        [DataRow(nameof(_testSData), 3, null)]
        [DataRow(nameof(_testIData), 1, null)]
        [DataRow(nameof(_testNData), 1, null)]
        public void ItemDataTest1_1(string actData, int iAct, string oExp)
        {
            var act = actData switch
            {
                nameof(_testSData) => _testSData,
                nameof(_testIData) => _testIData,
                nameof(_testNData) => _testNData,
                _ => throw new ArgumentException("Invalid test data")
            };
            Assert.AreEqual(oExp, act.ItemData<string>(iAct));
        }

        [DataTestMethod()]
        [DataRow(nameof(_testIData), 0, 1)]
        [DataRow(nameof(_testIData), 1, 2)]
        [DataRow(nameof(_testIData), 2, 3)]
        [DataRow(nameof(_testIData), 3, 0)]
        [DataRow(nameof(_testSData), 1, 0)]
        [DataRow(nameof(_testNData), 1, 0)]
        public void ItemDataTest1_2(string actData, int iAct, int oExp)
        {
            var act = actData switch
            {
                nameof(_testSData) => _testSData,
                nameof(_testIData) => _testIData,
                nameof(_testNData) => _testNData,
                _ => throw new ArgumentException("Invalid test data")
            };
            Assert.AreEqual(oExp, act.ItemData<int>(iAct));
        }

        [DataTestMethod()]
        [DataRow(nameof(_testSData), 0, "1")]
        [DataRow(nameof(_testSData), 1, "2")]
        [DataRow(nameof(_testSData), 2, "3")]
        [DataRow(nameof(_testSData), 3, null)]
        [DataRow(nameof(_testIData), 0, 1)]
        [DataRow(nameof(_testIData), 1, 2)]
        [DataRow(nameof(_testIData), 2, 3)]
        [DataRow(nameof(_testIData), 3, null)]
        [DataRow(nameof(_testNData), 0, null)]
        [DataRow(nameof(_testNData), 1, null)]
        [DataRow(nameof(_testNData), 2, null)]
        [DataRow(nameof(_testNData), 3, null)]
        public void ItemDataTest2(string actData, int iAct, object? oExp)
        {
            var act = actData switch
            {
                nameof(_testSData) => _testSData,
                nameof(_testIData) => _testIData,
                nameof(_testNData) => _testNData,
                _ => throw new ArgumentException("Invalid test data")
            };
            Assert.AreEqual(oExp, iAct < act.Count ? act[iAct].ItemData() : null);
        }

        [DataTestMethod()]
        [DataRow(nameof(_testSData), 0, "1")]
        [DataRow(nameof(_testSData), 1, "2")]
        [DataRow(nameof(_testSData), 2, "3")]
        [DataRow(nameof(_testSData), 3, null)]
        [DataRow(nameof(_testIData), 1, null)]
        [DataRow(nameof(_testNData), 1, null)]
        public void ItemDataTest3_1(string actData, int iAct, string oExp)
        {
            var act = actData switch
            {
                nameof(_testSData) => _testSData,
                nameof(_testIData) => _testIData,
                nameof(_testNData) => _testNData,
                _ => throw new ArgumentException("Invalid test data")
            };
            Assert.AreEqual(oExp, iAct < act.Count ? act[iAct].ItemData<string>() : null);
        }

        [DataTestMethod()]
        [DataRow(nameof(_testIData), 0, 1)]
        [DataRow(nameof(_testIData), 1, 2)]
        [DataRow(nameof(_testIData), 2, 3)]
        [DataRow(nameof(_testIData), 3, 0)]
        [DataRow(nameof(_testSData), 1, 0)]
        [DataRow(nameof(_testNData), 1, 0)]
        public void ItemDataTest3_2(string actData, int iAct, int oExp)
        {
            var act = actData switch
            {
                nameof(_testSData) => _testSData,
                nameof(_testIData) => _testIData,
                nameof(_testNData) => _testNData,
                _ => throw new ArgumentException("Invalid test data")
            };
            Assert.AreEqual(oExp, iAct < act.Count ? act[iAct].ItemData<int>() : default);
        }

        [DataTestMethod()]
        [DataRow(nameof(_testSData), 0, "A")]
        [DataRow(nameof(_testSData), 1, "B")]
        [DataRow(nameof(_testSData), 2, "C")]
        [DataRow(nameof(_testSData), 3, "")]
        [DataRow(nameof(_testIData), 0, "D")]
        [DataRow(nameof(_testIData), 1, "E")]
        [DataRow(nameof(_testIData), 2, "F")]
        [DataRow(nameof(_testIData), 3, "")]
        [DataRow(nameof(_testNData), 0, "")]
        [DataRow(nameof(_testNData), 1, "")]
        [DataRow(nameof(_testNData), 2, "")]
        [DataRow(nameof(_testNData), 3, "")]
        public void ItemStringTest(string actData, int iAct, string oExp)
        {
            var act = actData switch
            {
                nameof(_testSData) => _testSData,
                nameof(_testIData) => _testIData,
                nameof(_testNData) => _testNData,
                _ => throw new ArgumentException("Invalid test data")
            };
            Assert.AreEqual(oExp, act.ItemString(iAct));
        }


        [DataTestMethod()]
        [DataRow(nameof(_testSData), 0, "A")]
        [DataRow(nameof(_testSData), 1, "B")]
        [DataRow(nameof(_testSData), 2, "C")]
        [DataRow(nameof(_testSData), 3, "")]
        [DataRow(nameof(_testIData), 0, "D")]
        [DataRow(nameof(_testIData), 1, "E")]
        [DataRow(nameof(_testIData), 2, "F")]
        [DataRow(nameof(_testIData), 3, "")]
        [DataRow(nameof(_testNData), 0, "")]
        [DataRow(nameof(_testNData), 1, "")]
        [DataRow(nameof(_testNData), 2, "")]
        [DataRow(nameof(_testNData), 3, "")]
        public void SetStringTest(string actData, int iAct, string oExp)
        {
            var act = actData switch
            {
                nameof(_testSData) => _testSData,
                nameof(_testIData) => _testIData,
                nameof(_testNData) => _testNData,
                _ => throw new ArgumentException("Invalid test data")
            };
            act.SetString(iAct, $"'{oExp}'");
            Assert.AreEqual(iAct < act.Count && act != _testNData ? $"'{oExp}'" : "", act.ItemString(iAct));
        }
    }
}