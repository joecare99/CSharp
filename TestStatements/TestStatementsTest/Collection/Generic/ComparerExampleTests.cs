using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.Collection.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TestStatements.Collection.Generic.Tests
{
    [TestClass()]
    public class ComparerExampleTests
    {
        private readonly string cExpComparerExampleMain = "===================================================================" +
            "===\r\n## Comparer<T> \r\n======================================================================\r\n\r\n" +
            "+----------------------------------------------------------\r\n| Show Sort with BoxLengthFirst - comparer\r\n" +
            "+----------------------------------------------------------\r\nH - L - W\r\n==========\r\n10\t2\t10\r\n" +
            "10\t4\t16\r\n24\t4\t18\r\n2\t6\t8\r\n8\t6\t20\r\n14\t6\t6\r\n16\t6\t16\r\n2\t8\t4\r\n2\t8\t12\r\n6\t10\t2\r\n" +
            "12\t10\t8\r\n18\t10\t4\r\n4\t12\t20\r\n8\t12\t4\r\n12\t12\t12\r\n6\t18\t2\r\n18\t18\t12\r\n4\t20\t14\r\n" +
            "8\t20\t10\r\n4\t24\t8\r\n\r\n+----------------------------------------------------------\r\n" +
            "| Show Sort with default - comparer\r\n+----------------------------------------------------------\r\n\r\n" +
            "H - L - W\r\n==========\r\n2\t6\t8\r\n2\t8\t4\r\n2\t8\t12\r\n4\t12\t20\r\n4\t20\t14\r\n4\t24\t8\r\n6\t10\t2\r\n" +
            "6\t18\t2\r\n8\t6\t20\r\n8\t12\t4\r\n8\t20\t10\r\n10\t2\t10\r\n10\t4\t16\r\n12\t10\t8\r\n12\t12\t12\r\n14\t6\t6\r\n" +
            "16\t6\t16\r\n18\t10\t4\r\n18\t18\t12\r\n24\t4\t18\r\n\r\n" +
            "+----------------------------------------------------------\r\n| Show use of explicit - comparer\r\n" +
            "+----------------------------------------------------------\r\n\r\n-1";
        private readonly string cExpShowSortWithLengthFirstComparer =
            "+----------------------------------------------------------\r\n| Show Sort with BoxLengthFirst - comparer\r\n" +
            "+----------------------------------------------------------\r\nH - L - W\r\n==========\r\n10\t2\t10\r\n" +
            "10\t4\t16\r\n24\t4\t18\r\n2\t6\t8\r\n8\t6\t20\r\n14\t6\t6\r\n16\t6\t16\r\n2\t8\t4\r\n2\t8\t12\r\n6\t10\t2\r\n" +
            "12\t10\t8\r\n18\t10\t4\r\n4\t12\t20\r\n8\t12\t4\r\n12\t12\t12\r\n6\t18\t2\r\n18\t18\t12\r\n4\t20\t14\r\n" +
            "8\t20\t10\r\n4\t24\t8";
        private readonly string cExpShowSortwithDefaultComparer =
            "+----------------------------------------------------------\r\n| Show Sort with default - comparer\r\n" +
            "+----------------------------------------------------------\r\n\r\nH - L - W\r\n==========\r\n2\t6\t8\r\n" +
            "2\t8\t4\r\n2\t8\t12\r\n4\t12\t20\r\n4\t20\t14\r\n4\t24\t8\r\n6\t10\t2\r\n6\t18\t2\r\n8\t6\t20\r\n8\t12\t4\r\n" +
            "8\t20\t10\r\n10\t2\t10\r\n10\t4\t16\r\n12\t10\t8\r\n12\t12\t12\r\n14\t6\t6\r\n16\t6\t16\r\n18\t10\t4\r\n" +
            "18\t18\t12\r\n24\t4\t18";
        private readonly string cExpShowLengthFirstComparer = "+----------------------------------------------------------\r\n" +
            "| Show use of explicit - comparer\r\n+----------------------------------------------------------\r\n\r\n-1";

        [TestMethod()]
        public void ComparerExampleMainTest()
        {
            AssertConsoleOutputArgs(cExpComparerExampleMain, null, ComparerExample.ComparerExampleMain);
        }

        [TestMethod()]
        public void ShowSortWithLengthFirstComparerTest()
        {
            AssertConsoleOutput(cExpShowSortWithLengthFirstComparer, ComparerExample.ShowSortWithLengthFirstComparer);
        }

        [TestMethod()]
        public void ShowSortwithDefaultComparerTest()
        {
            AssertConsoleOutput(cExpShowSortwithDefaultComparer, ComparerExample.ShowSortwithDefaultComparer);
        }

        [TestMethod()]
        public void ShowLengthFirstComparerTest()
        {
            AssertConsoleOutput(cExpShowLengthFirstComparer, ComparerExample.ShowLengthFirstComparer);
        }

        private static void AssertConsoleOutput(string Expected, CrossAppDomainDelegate ToTest)
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                ToTest?.Invoke();

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }
        private static void AssertConsoleOutputArgs(string Expected, string[] Args, Action<String[]> ToTest)
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                ToTest?.Invoke(Args);

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
        }

        [TestMethod()]
        public void CompareTest()
        {
            BoxLengthFirst LengthFirst = new BoxLengthFirst();

            Comparer<Box> bc = (Comparer<Box>)LengthFirst;


            Assert.AreEqual(-1, LengthFirst.Compare(new Box(2, 6, 8), new Box(10, 12, 14)));
            Assert.AreEqual(-1, bc.Compare(new Box(2, 6, 8), new Box(10, 12, 14)));
            Assert.AreEqual(1, LengthFirst.Compare(new Box(10, 12, 14), new Box(2, 6, 8)));
            Assert.AreEqual(1, bc.Compare(new Box(10, 12, 14), new Box(2, 6, 8)));
            Assert.AreEqual(1, LengthFirst.Compare(new Box(10, 12, 14), new Box(2, 12, 8)));
            Assert.AreEqual(1, bc.Compare(new Box(10, 12, 14), new Box(2, 12, 8)));
            Assert.AreEqual(1, LengthFirst.Compare(new Box(10, 12, 14), new Box(10, 12, 8)));
            Assert.AreEqual(1, bc.Compare(new Box(10, 12, 14), new Box(10, 12, 8)));
            Assert.AreEqual(0, LengthFirst.Compare(new Box(10, 12, 14), new Box(10, 12, 14)));
            Assert.AreEqual(0, bc.Compare(new Box(10, 12, 14), new Box(10, 12, 14)));
        }

        [TestMethod()]
        public void CompareTest1()
        {
            BoxComp lBoxComp = new BoxComp();

            IComparer<Box> bc = (IComparer<Box>)lBoxComp;

            Assert.AreEqual(-1, lBoxComp.Compare(new Box(2, 6, 8), new Box(10, 12, 14)));
            Assert.AreEqual(-1, bc.Compare(new Box(2, 6, 8), new Box(10, 12, 14)));
            Assert.AreEqual(1, lBoxComp.Compare(new Box(10, 12, 14), new Box(2, 6, 8)));
            Assert.AreEqual(1, bc.Compare(new Box(10, 12, 14), new Box(2, 6, 8)));

            Assert.AreEqual(1, lBoxComp.Compare(new Box(10, 12, 14), new Box(2, 12, 8)));
            Assert.AreEqual(1, bc.Compare(new Box(10, 12, 14), new Box(2, 12, 8)));
            Assert.AreEqual(-1, lBoxComp.Compare(new Box(2, 12, 14), new Box(12, 12, 8)));
            Assert.AreEqual(-1, bc.Compare(new Box(2, 12, 14), new Box(12, 12, 8)));
            Assert.AreEqual(1, lBoxComp.Compare(new Box(10, 12, 14), new Box(10, 12, 8)));
            Assert.AreEqual(1, bc.Compare(new Box(10, 12, 14), new Box(10, 12, 8)));
            Assert.AreEqual(-1, lBoxComp.Compare(new Box(10, 12, 8), new Box(10, 12, 9)));
            Assert.AreEqual(-1, bc.Compare(new Box(10, 12, 8), new Box(10, 12, 9)));
            Assert.AreEqual(0, lBoxComp.Compare(new Box(10, 12, 14), new Box(10, 12, 14)));
            Assert.AreEqual(0, bc.Compare(new Box(10, 12, 14), new Box(10, 12, 14)));
        }
    }
}