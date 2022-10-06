using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestStatements.UnitTesting;

namespace TestStatements.UnitTesting.Tests
{
	/// <summary>
	/// Defines test class DataTests.
	/// Implements the <see cref="ConsoleTestsBase" />
	/// </summary>
	/// <seealso cref="ConsoleTestsBase" />
	[TestClass()]
    public class DataTests : ConsoleTestsBase
    {
		static int[] NullSeries5() => new int[] { 2, 1, 3, -1, 0 };
		static int[] NullSeries3() => new int[] { 2, 1, 3 };
		static (int,int)[] Tupels1(int vx,int vy) => new (int,int)[] { (-2+vx,2 + vy), (2 + vx, 2 + vy), (2 + vx, -2 + vy), (-2 + vx, -2 + vy) };
		static (int, int)[] Tupels2(int vx, int vy) => new (int, int)[] { (0 + vx, 2 + vy), (2 + vx, 0 + vy), (-2 + vx, 0 + vy), (0 + vx, -2 + vy) };

		static IEnumerable<object[]> ReusableTestDataProperty => new[]
				{
						new object[] {1, NullSeries5(), NullSeries5(), "Test Identity (5)" },
						new object[] {3, NullSeries3(), new int[] { 6, 3, 9 }, "Test Multiply3 (3)" },
						new object[] {4, new[]{ 5 }, new[]{ 20 } }
					};

		static IEnumerable<object[]> ReusableTestDataProperty2 => new[]
		{
						new object[] {0, NullSeries5(), new int[] { 0, 0, 0, 0, 0 },"Test Zero (5)" },
						new object[] {-3, NullSeries3(), new int[] { -6, -3, -9 }, "Test Multiply(-3) (3)" },
						new object[] {1, new[]{ 5 }, new[]{5 } }
					};

		static IEnumerable<object[]> ReusableTestDataMethod() => new[]
			{
					new object[] {1,  new[] { 2 },  new[] { 2 } },
					new object[] {4,  new[] { 5 },  new[] { 20 } }
				};
		static IEnumerable<object[]> ReusableTestDataMethod2() => new[]
			{
					new object[] {0,  new[] { 2 },  new[] { 0 } },
					new object[] {-4,  new[] { 5 },  new[] { -20 } }
				};
		/// <summary>
		/// Dummies this instance.
		/// </summary>
		public void Dummy() { }

		/// <summary>
		/// Defines the test method AssemblyExampleTest.
		/// </summary>
		[TestMethod()]
		public void AssemblyExampleTest() {
			Assert.IsNotNull(new AssemblyExample(5));
		}


		/// <summary>
		/// Samples the data property test.
		/// </summary>
		/// <param name="iStart">The i start.</param>
		/// <param name="iTest">The i test.</param>
		/// <param name="iExp">The i exp.</param>
		/// <param name="TestName">Name of the test.</param>
		[DataTestMethod()]
		[TestProperty("Author","CR")]
		[DynamicData("ReusableTestDataProperty")]
		[DynamicData("ReusableTestDataProperty2")]
		public void SampleDataPropertyTest(int iStart,int[] iTest,int[] iExp,string TestName="")
        {
            AssemblyExample l = new AssemblyExample(iStart) ;
			Assert.AreEqual(iTest.Length, iExp.Length,TestName);
			for (int i = 0; i < iTest.Length; i++) {
				Assert.AreEqual(iExp[i], l.SampleMethod(iTest[i]));
			}
        }

		/// <summary>
		/// Samples the data method test.
		/// </summary>
		/// <param name="iStart">The i start.</param>
		/// <param name="iTest">The i test.</param>
		/// <param name="iExp">The i exp.</param>
		/// <param name="TestName">Name of the test.</param>
		[DataTestMethod()]
		[DynamicData("ReusableTestDataMethod", DynamicDataSourceType.Method)]
		[DynamicData("ReusableTestDataMethod2", DynamicDataSourceType.Method)]
		public void SampleDataMethodTest(int iStart, int[] iTest, int[] iExp, string TestName="") {
			AssemblyExample l = new AssemblyExample(iStart);
			Assert.AreEqual(iTest.Length, iExp.Length, TestName);
			for (int i = 0; i < iTest.Length; i++) {
				Assert.AreEqual(iExp[i], l.SampleMethod(iTest[i]));
			}
		}

		/// <summary>
		/// Samples the method test.
		/// </summary>
		/// <param name="iStart">The i start.</param>
		/// <param name="iTest1">The i test1.</param>
		/// <param name="iExp1">The i exp1.</param>
		/// <param name="iTest2">The i test2.</param>
		/// <param name="iExp2">The i exp2.</param>
		/// <param name="iTest3">The i test3.</param>
		/// <param name="iExp3">The i exp3.</param>
		[DataTestMethod()]
		[DataRow(3, 2, 6, 1, 3, 3, 9)]
		[DataRow(4, 2, 8, 1, 4, 3, 12)]
		[DataRow(1, 2, 2, 1, 1, 3, 3)]
		[DataRow(-1, 2, -2, 1, -1, 3, -3)]
		[DataRow(0, 2, 0, 1, 0, 3, 0)]
		public void SampleMethodTest(int iStart, int iTest1, int iExp1, int iTest2, int iExp2, int iTest3, int iExp3) {
			AssemblyExample l = new AssemblyExample(iStart);
			Assert.AreEqual(iExp1, l.SampleMethod(iTest1));
			Assert.AreEqual(iExp2, l.SampleMethod(iTest2));
			Assert.AreEqual(iExp3, l.SampleMethod(iTest3));
		}

	}
}
