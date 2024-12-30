using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestStatements.UnitTesting;

namespace TestStatements.Anweisungen.Tests
{
    [TestClass()]
    public class SwitchStatementTests:ConsoleTestsBase
    {
        [DataTestMethod()]
        [DataRow(0, "Case 2")]
        [DataRow(1, "Default case")]
        [DataRow(2, "Case 2")]
        [DataRow(3, "Default case")]
        [DataRow(4, "Case 2")]
        [DataRow(5, "Case 1")]
        [DataRow(6, "Case 2")]
        public void SwitchExample1Test(int seed,string sExp)
        {
            var rnd = new Random(seed);
            SwitchStatement.random = () => rnd;
            AssertConsoleOutput(sExp, SwitchStatement.SwitchExample1);
        }

        [DataTestMethod()]
        [DataRow(0, "The color is green")]
        [DataRow(1, "The color is unknown.")]
        [DataRow(2, "The color is blue")]
        [DataRow(3, "The color is red")]
        [DataRow(4, "The color is blue")]
        [DataRow(5, "The color is red")]
        [DataRow(6, "The color is blue")]
        public void SwitchExample2Test(int seed, string sExp)
        {
            var rnd = new Random(seed);
            SwitchStatement.random = () => rnd;
            AssertConsoleOutput(sExp, SwitchStatement.SwitchExample2);
        }

        [DataTestMethod()]
        [DataRow(0, "Case 2")]
        [DataRow(1, "An unexpected value (0)")]
        [DataRow(2, "Case 3")]
        [DataRow(3, "Case 1")]
        [DataRow(4, "Case 3")]
        [DataRow(5, "Case 1")]
        [DataRow(6, "Case 3")]
        public void SwitchExample3Test(int seed, string sExp)
        {
            var rnd = new Random(seed);
            SwitchStatement.random = () => rnd;
            AssertConsoleOutput(sExp, SwitchStatement.SwitchExample3);
        }

        [DataTestMethod()]
        [DataRow(0, "The sum of 8 die is 36")]
        [DataRow(1, "The sum of 8 die is 27")]
        [DataRow(2, "The sum of 8 die is 39")]
        [DataRow(3, "The sum of 8 die is 30")]
        [DataRow(4, "The sum of 8 die is 26")]
        [DataRow(5, "The sum of 8 die is 26")]
        [DataRow(6, "The sum of 8 die is 36")]
        public void SwitchExample4Test(int seed, string sExp)
        {
            var rnd = new Random(seed);
            SwitchStatement.random = () => rnd;
            AssertConsoleOutput(sExp, SwitchStatement.SwitchExample4);
        }

        [DataTestMethod()]
        [DataRow(0, "The weekend")]
        [DataRow(1, "The first day of the work week.")]
        [DataRow(2, "The middle of the work week.")]
        [DataRow(3, "The middle of the work week.")]
        [DataRow(4, "The middle of the work week.")]
        [DataRow(5, "The last day of the work week.")]
        [DataRow(6, "The weekend")]
        [DataRow(7, "The weekend")]
        public void SwitchExample5Test(int seed, string sExp)
        {
           // var rnd = new Random(seed);
            SwitchStatement.GetNow = () => DateTime.Parse("2023-01-01").AddDays(seed);
            AssertConsoleOutput(sExp, SwitchStatement.SwitchExample5);
        }

        [DataTestMethod()]
        public void SwitchExample6Test()
        {
            AssertConsoleOutput(@"An array with 5 elements.
4 items", SwitchStatement.SwitchExample6);

        }
        [DataTestMethod()]
        public void SwitchExample7Test()
        {
            AssertConsoleOutput(@"An array with 5 elements.
4 items
Null passed to this method.", SwitchStatement.SwitchExample7);

        }
    }
}