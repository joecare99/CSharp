using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.Collection.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TestStatements.UnitTesting;

namespace TestStatements.Collection.Generic.Tests {
	/// <summary>
	/// Defines test class ComparerExampleTests.
	/// Implements the <see cref="ConsoleTestsBase" />
	/// </summary>
	/// <seealso cref="ConsoleTestsBase" />
	[TestClass()]
	public class ComparerExampleTests :ConsoleTestsBase {
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

		/// <summary>
		/// Defines the test method ComparerExampleMainTest.
		/// </summary>
		[TestMethod()]
		public void ComparerExampleMainTest() {
			AssertConsoleOutputArgs(cExpComparerExampleMain, null, ComparerExample.ComparerExampleMain);
		}

		/// <summary>
		/// Defines the test method ShowSortWithLengthFirstComparerTest.
		/// </summary>
		[TestMethod()]
		public void ShowSortWithLengthFirstComparerTest() {
			AssertConsoleOutput(cExpShowSortWithLengthFirstComparer, ComparerExample.ShowSortWithLengthFirstComparer);
		}

		/// <summary>
		/// Defines the test method ShowSortwithDefaultComparerTest.
		/// </summary>
		[TestMethod()]
		public void ShowSortwithDefaultComparerTest() {
			AssertConsoleOutput(cExpShowSortwithDefaultComparer, ComparerExample.ShowSortwithDefaultComparer);
		}

		/// <summary>
		/// Defines the test method ShowLengthFirstComparerTest.
		/// </summary>
		[TestMethod()]
		public void ShowLengthFirstComparerTest() {
			AssertConsoleOutput(cExpShowLengthFirstComparer, ComparerExample.ShowLengthFirstComparer);
		}

		/// <summary>
		/// Defines the test method CompareTest.
		/// </summary>
		[TestMethod()]
		public void CompareTest() {
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

		/// <summary>
		/// Compares the test d.
		/// </summary>
		/// <param name="expected">The expected.</param>
		/// <param name="box1_1">The box1 1.</param>
		/// <param name="box1_2">The box1 2.</param>
		/// <param name="box1_3">The box1 3.</param>
		/// <param name="box2_1">The box2 1.</param>
		/// <param name="box2_2">The box2 2.</param>
		/// <param name="box2_3">The box2 3.</param>
		[DataTestMethod()]
		[DataRow(-1, 2, 6, 8, 10,12,14)]
		[DataRow(1, 10, 12, 14, 2, 6, 8)]
		[DataRow(1, 10, 12, 14, 2, 12, 8)]
		[DataRow(-1, 2, 12, 14, 12, 12, 8)]
		[DataRow(1, 10, 12, 14, 10, 12, 8)]
		[DataRow(-1, 10, 12, 8, 10, 12, 9)]
		[DataRow(0, 10, 12, 14, 10, 12, 14)]
		public void CompareTestD(int expected, int box1_1, int box1_2, int box1_3, int box2_1, int box2_2, int box2_3) {
			BoxLengthFirst LengthFirst = new BoxLengthFirst();

			Comparer<Box> bc = (Comparer<Box>)LengthFirst;
			Box box1 = new Box(box1_1, box1_2, box1_3);
			Box box2 = new Box(box2_1, box2_2, box2_3);
			Assert.AreEqual(expected, LengthFirst.Compare(box1, box2));
			Assert.AreEqual(expected, bc.Compare(box1, box2));

		}

		/// <summary>
		/// Defines the test method CompareTest1.
		/// </summary>
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
