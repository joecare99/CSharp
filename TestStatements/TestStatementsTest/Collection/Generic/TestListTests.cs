using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.Collection.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TestStatements.ConsoleAsserts;

namespace TestStatements.Collection.Generic.Tests
{
    [TestClass()]
    public class TestListTests : TestConsole
    {
        private readonly string cExpListMain =
"======================================================================\r\n## Show List<T> \r\n======================================================================\r\n\r\nID: 1234   Name: crank arm\r\nID: 1334   Name: chain ring\r\nID: 1434   Name: regular seat\r\nID: 1444   Name: banana seat\r\nID: 1534   Name: cassette\r\nID: 1634   Name: shift lever\r\n\r\n+----------------------------------------------------------\r\n| Show Contains\r\n+----------------------------------------------------------\r\n\nContains(\"1734\"): False\r\n\r\n+----------------------------------------------------------\r\n| Show Insert\r\n+----------------------------------------------------------\r\n\nInsert(2, \"1834\") (brake lever)\r\n\r\nID: 1234   Name: crank arm\r\nID: 1334   Name: chain ring\r\nID: 1834   Name: brake lever\r\nID: 1434   Name: regular seat\r\nID: 1444   Name: banana seat\r\nID: 1534   Name: cassette\r\nID: 1634   Name: shift lever\r\n\r\n+----------------------------------------------------------\r\n| Show Index\r\n+----------------------------------------------------------\r\n\nParts[3]: ID: 1434   Name: regular seat\r\n\r\n+----------------------------------------------------------\r\n| Show Remove\r\n+----------------------------------------------------------\r\n\nRemove(\"1534\") (cassette)\r\n\r\nID: 1234   Name: crank arm\r\nID: 1334   Name: chain ring\r\nID: 1834   Name: brake lever\r\nID: 1434   Name: regular seat\r\nID: 1444   Name: banana seat\r\nID: 1634   Name: shift lever\r\n\r\n+----------------------------------------------------------\r\n| Show RemoveAt\r\n+----------------------------------------------------------\r\n\nRemoveAt(3) (regular seat)\r\n\r\nID: 1234   Name: crank arm\r\nID: 1334   Name: chain ring\r\nID: 1834   Name: brake lever\r\nID: 1444   Name: banana seat\r\nID: 1634   Name: shift lever";
        private readonly string cExpShowContains = "+----------------------------------------------------------\r\n| Show Contains\r\n+----------------------------------------------------------\r\n\nContains(\"1734\"): False";
        private readonly string cExpShowInsert = "+----------------------------------------------------------\r\n| Show Insert\r\n+----------------------------------------------------------\r\n\nInsert(2, \"1834\") (brake lever)\r\n\r\nID: 1234   Name: crank arm\r\nID: 1334   Name: chain ring\r\nID: 1834   Name: brake lever\r\nID: 1434   Name: regular seat\r\nID: 1444   Name: banana seat\r\nID: 1534   Name: cassette\r\nID: 1634   Name: shift lever";
        private readonly string cExpShowIndex = "+----------------------------------------------------------\r\n| Show Index\r\n+----------------------------------------------------------\r\n\nParts[3]: ID: 1434   Name: regular seat";
        private readonly string cExpShowRemove1 = "+----------------------------------------------------------\r\n| Show Remove\r\n+----------------------------------------------------------\r\n\nRemove(\"1534\") (cassette)\r\n\r\nID: 1234   Name: crank arm\r\nID: 1334   Name: chain ring\r\nID: 1834   Name: brake lever\r\nID: 1434   Name: regular seat\r\nID: 1444   Name: banana seat\r\nID: 1634   Name: shift lever";
        private readonly string cExpShowRemove2 = "+----------------------------------------------------------\r\n| Show RemoveAt\r\n+----------------------------------------------------------\r\n\nRemoveAt(3) (regular seat)\r\n\r\nID: 1234   Name: crank arm\r\nID: 1334   Name: chain ring\r\nID: 1834   Name: brake lever\r\nID: 1444   Name: banana seat\r\nID: 1634   Name: shift lever";

        [TestMethod()]
        public void ListMainTest()
        {
            AssertConsoleOutput(cExpListMain, TestList.ListMain);
        }
        [TestMethod()]
        public void ShowContainsTest()
        {
            AssertConsoleOutput(cExpShowContains, TestList.ShowContains);
        }

        [TestMethod()]
        public void ShowInsertTest()
        {
            AssertConsoleOutput(cExpShowInsert, TestList.ShowInsert);
        }

        [TestMethod()]
        public void ShowIndexTest()
        {
            AssertConsoleOutput(cExpShowIndex, TestList.ShowIndex);
        }

        [TestMethod()]
        public void ShowRemove1Test()
        {
            AssertConsoleOutput(cExpShowRemove1, TestList.ShowRemove1);
        }

        [TestMethod()]
        public void ShowRemove2Test()
        {
            AssertConsoleOutput(cExpShowRemove2, TestList.ShowRemove2);
        }

       
    }
}