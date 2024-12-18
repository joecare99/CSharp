using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.Collection.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TestStatements.UnitTesting;

namespace TestStatements.Collection.Generic.Tests
{
    /// <summary>
    /// Defines test class TestListTests.
    /// Implements the <see cref="ConsoleTestsBase" />
    /// </summary>
    /// <seealso cref="ConsoleTestsBase" />
    [TestClass()]
    public class TestListTests : ConsoleTestsBase
    {
        private readonly string cExpListMain =
"======================================================================\r\n## Show List<T> \r\n======================================================================\r\n\r\nID: 1234   Name: crank arm\r\nID: 1334   Name: chain ring\r\nID: 1434   Name: regular seat\r\nID: 1444   Name: banana seat\r\nID: 1534   Name: cassette\r\nID: 1634   Name: shift lever\r\n\r\n+----------------------------------------------------------\r\n| Show Contains\r\n+----------------------------------------------------------\r\n\nContains(\"1734\"): False\r\n\r\n+----------------------------------------------------------\r\n| Show Insert\r\n+----------------------------------------------------------\r\n\nInsert(2, \"1834\") (brake lever)\r\n\r\nID: 1234   Name: crank arm\r\nID: 1334   Name: chain ring\r\nID: 1834   Name: brake lever\r\nID: 1434   Name: regular seat\r\nID: 1444   Name: banana seat\r\nID: 1534   Name: cassette\r\nID: 1634   Name: shift lever\r\n\r\n+----------------------------------------------------------\r\n| Show Index\r\n+----------------------------------------------------------\r\n\nParts[3]: ID: 1434   Name: regular seat\r\n\r\n+----------------------------------------------------------\r\n| Show Remove\r\n+----------------------------------------------------------\r\n\nRemove(\"1534\") (cassette)\r\n\r\nID: 1234   Name: crank arm\r\nID: 1334   Name: chain ring\r\nID: 1834   Name: brake lever\r\nID: 1434   Name: regular seat\r\nID: 1444   Name: banana seat\r\nID: 1634   Name: shift lever\r\n\r\n+----------------------------------------------------------\r\n| Show RemoveAt\r\n+----------------------------------------------------------\r\n\nRemoveAt(3) (regular seat)\r\n\r\nID: 1234   Name: crank arm\r\nID: 1334   Name: chain ring\r\nID: 1834   Name: brake lever\r\nID: 1444   Name: banana seat\r\nID: 1634   Name: shift lever";
        private readonly string cExpShowContains = "+----------------------------------------------------------\r\n| Show Contains\r\n+----------------------------------------------------------\r\n\nContains(\"1734\"): False";
        private readonly string cExpShowInsert = "+----------------------------------------------------------\r\n| Show Insert\r\n+----------------------------------------------------------\r\n\nInsert(2, \"1834\") (brake lever)\r\n\r\nID: 1234   Name: crank arm\r\nID: 1334   Name: chain ring\r\nID: 1834   Name: brake lever\r\nID: 1434   Name: regular seat\r\nID: 1444   Name: banana seat\r\nID: 1534   Name: cassette\r\nID: 1634   Name: shift lever";
        private readonly string cExpShowIndex = "+----------------------------------------------------------\r\n| Show Index\r\n+----------------------------------------------------------\r\n\nParts[3]: ID: 1434   Name: regular seat";
        private readonly string cExpShowRemove1 = "+----------------------------------------------------------\r\n| Show Remove\r\n+----------------------------------------------------------\r\n\nRemove(\"1534\") (cassette)\r\n\r\nID: 1234   Name: crank arm\r\nID: 1334   Name: chain ring\r\nID: 1834   Name: brake lever\r\nID: 1434   Name: regular seat\r\nID: 1444   Name: banana seat\r\nID: 1634   Name: shift lever";
        private readonly string cExpShowRemove2 = "+----------------------------------------------------------\r\n| Show RemoveAt\r\n+----------------------------------------------------------\r\n\nRemoveAt(3) (regular seat)\r\n\r\nID: 1234   Name: crank arm\r\nID: 1334   Name: chain ring\r\nID: 1834   Name: brake lever\r\nID: 1444   Name: banana seat\r\nID: 1634   Name: shift lever";

        /// <summary>
        /// Defines the test method ListMainTest.
        /// </summary>
        [TestMethod()]
        public void ListMainTest()
        {
            AssertConsoleOutput(cExpListMain, TestList.ListMain);
        }
        /// <summary>
        /// Defines the test method ShowContainsTest.
        /// </summary>
        [TestMethod()]
        public void ShowContainsTest()
        {
            AssertConsoleOutput(cExpShowContains, TestList.ShowContains);
        }

        /// <summary>
        /// Defines the test method ShowInsertTest.
        /// </summary>
        [TestMethod()]
        public void ShowInsertTest()
        {
            AssertConsoleOutput(cExpShowInsert, TestList.ShowInsert);
        }

        /// <summary>
        /// Defines the test method ShowIndexTest.
        /// </summary>
        [TestMethod()]
        public void ShowIndexTest()
        {
            AssertConsoleOutput(cExpShowIndex, TestList.ShowIndex);
            AssertConsoleOutput(cExpShowIndex, TestList.ShowIndex);
        }

        /// <summary>
        /// Defines the test method ShowRemove1Test.
        /// </summary>
        [TestMethod()]
        public void ShowRemove1Test()
        {
            AssertConsoleOutput(cExpShowRemove1, TestList.ShowRemove1);
            AssertConsoleOutput(cExpShowRemove1, TestList.ShowRemove1);
        }

        /// <summary>
        /// Defines the test method ShowRemove2Test.
        /// </summary>
        [TestMethod()]
        public void ShowRemove2Test()
        {
            AssertConsoleOutput(cExpShowRemove2, TestList.ShowRemove2);
            AssertConsoleOutput(cExpShowRemove2, TestList.ShowRemove2);
        }

        [DataTestMethod]
        [DataRow("0", null, "Is null")]
        [DataRow("1",new string[]{}, "Capacity: 0\r\nCount: 0")]
        [DataRow("2", new string[] {"Test" }, "Capacity: 1\r\nCount: 1")]
        [DataRow("3", new string[] {"this","List" }, "Capacity: 2\r\nCount: 2")]
        public void ShowStatusTest(string part1, object oVal, string sExp)
        {
            List<string>? lsVal = oVal is string[] aS ? aS.ToList() : (List<string>?)null;
            AssertConsoleOutput(sExp,()=> DinosaurExample.ShowStatus(lsVal));
        }

        [DataTestMethod]
        [DataRow("Part1",1,"Part2",2,false)]
        [DataRow("Part1", 1, "Part2", 1, true)]
        [DataRow("Part1", 1, "Part1", 2, false)]
        [DataRow("Part1", 1, "Part1", 1, true)]
        [DataRow("Part1", 1, "", 1, false)]
        public void PartEqualsTest(string part1,int iId1, string part2,int iId2,bool xExp)
        {
            var p1 = new Part() { PartId=iId1, PartName=part1 };
            var p2 = !string.IsNullOrEmpty(part2) ? new Part() { PartId = iId2, PartName = part2 } : null;
            Assert.AreEqual(xExp,p1.Equals((object?)p2));
        }

        [DataTestMethod]
        [DataRow("Part1", 1, "Part2", 2, false)]
        [DataRow("Part1", 1, "Part2", 1, true)]
        [DataRow("Part1", 1, "Part1", 2, false)]
        [DataRow("Part1", 1, "Part1", 1, true)]
        [DataRow("Part1", 1, "", 1, false)]
        public void PartEquals2Test(string part1, int iId1, string part2, int iId2, bool xExp)
        {
            var p1 = new Part() { PartId = iId1, PartName = part1 };
            var p2 = !string.IsNullOrEmpty(part2) ? new Part() { PartId = iId2, PartName = part2 } : null;
            Assert.AreEqual(xExp, p1.Equals((Part?)p2));
        }

        [DataTestMethod]
        [DataRow("Part1",1,1)]
        [DataRow("Part2", 1, 1)]
        [DataRow("", 0, 0)]
        [DataRow("", 1, 1)]
        public void PartHashCodeTest(string part1, int iId1,  int iExp)
        {
            var p1 = new Part() { PartId = iId1, PartName = part1 };
             Assert.AreEqual(iExp, p1.GetHashCode());
        }

        /// <summary>
        /// Defines the test method ShowRemove2Test.
        /// </summary>
        [DataTestMethod()]
        [DataRow(0,"")]
        [DataRow(1, "Tyrannosaurus\r\nAmargasaurus\r\nMamenchisaurus\r\nDeinonychus\r\nCompsognathus")]
        [DataRow(2, "Tyrannosaurus\r\nAmargasaurus\r\nMamenchisaurus\r\nDeinonychus\r\nCompsognathus")]
        public void CreateTestDataTest(int iVal, string asExp)
        {
            DinosaurExample.Clear();
            for (int i = 0; i < iVal; i++)
                DinosaurExample.CreateTestData();
                
            AssertConsoleOutput(asExp,()=> DinosaurExample.ShowList(DinosaurExample.Dinosaurs));
        }

    }
}
