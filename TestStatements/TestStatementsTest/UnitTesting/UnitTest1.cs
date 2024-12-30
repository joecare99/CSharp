using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace TestStatementTest.UnitTesting
{
    /// <summary>
    /// Defines test class UnitTest1.
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
		static int[] NullSeries5(int f=1) => new int[] { 2*f, 1 * f, 3 * f, -1 * f, 0 * f };
		static int[] NullSeries3(int f=1) => new int[] { 1*f, 2*f, 3*f };
		static int[] NullSeries1(int f) => new int[] { 1*f };

		static IEnumerable<object[]> ReusableTestDataProperty => new[]
				{
						new object[] {-1, NullSeries5(), NullSeries5(-1), "Test Identity (5)" },
						new object[] {3, NullSeries3(), NullSeries3(3), "Test Multiply3 (3)" },
						new object[] {4, new[]{ 5 }, new[]{ 20 } }
					};

		/// <summary>
		/// Defines the test method TestMethod1.
		/// </summary>
		[TestMethod]
        public void TestMethod1()
        {
			Assert.IsTrue(true);
        }

		/// <summary>
		/// Datas the test open array.
		/// </summary>
		/// <param name="expected">The expected.</param>
		/// <param name="ints">The ints.</param>
		[DataTestMethod]
		[DataRow(10,new[] {1,2,3,4})]
		[DataRow(1, new[] { 1 })]
		[DataRow(0, new[] { 1, -1})]
		[DataRow(15, new[] { 1, 2, 3, 4, 5 })]
		public void DataTestOpenArray(int expected,int[] ints) {
			if (ints == null) Assert.Fail("Parameter is Null");
			else {
				Console.WriteLine($"ints = [{ArrayToString(ints)}]");
				Assert.AreEqual(expected, ints.Sum());
			}
		}

		/// <summary>
		/// Datas the test open array parameters.
		/// </summary>
		/// <param name="ints">The ints.</param>
		[DataTestMethod]
		[DataRow(10, 1, 2, 3, 4 )]
		[DataRow(15, 1, 2, 3, 4, 5 )]
		[DataRow(0)]
		[DataRow(1,1)]
		[DataRow(0, 1,-1)]
		public void DataTestOpenArrayParams(params int[] ints) {
			if (ints == null) Assert.Fail("Parameter is Null");
			else {
				var expected = ints[0];
				//Unneeded: Console.WriteLine($"ints = [{ArrayToString(ints)}]");
				Assert.AreEqual(expected, ints.Sum()/2);
			}
		}

		/// <summary>
		/// Datas the test property test.
		/// </summary>
		/// <param name="iStart">The i start.</param>
		/// <param name="iTest">The i test.</param>
		/// <param name="iExp">The i exp.</param>
		/// <param name="TestName">Name of the test.</param>
		[DataTestMethod()]
		[TestProperty("Author", "CR")]
		[DynamicData("ReusableTestDataProperty")]
		[DataRow(2, new[] {-1,0,1,2,3,4 }, new[] {-2,0,2,4,6,8 }, "Weitere DataRow")]
		public void DataTestPropertyTest(int iStart, int[] iTest, int[] iExp, string TestName = "") {
			Assert.AreEqual(iTest.Length, iExp.Length, TestName);
			Console.WriteLine($"iTest = [{ArrayToString(iTest)}]\tiExp = [{ArrayToString(iExp)}]");
			for (int i = 0; i < iTest.Length; i++) {
				Assert.AreEqual(iExp[i], iTest[i]*iStart);
			}
		}

		private string ArrayToString<T>(T[] ints) {
			var result = "";
			foreach (T t in ints) {
				result += $"{t}, ";
			}
			return result.TrimEnd(' ',',');
		}
	}
}
