using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TestStatements.DataTypes.Tests
{
    [TestClass()]
    public class EnumTestTests
    {
        private string cExpected1 =
            "======================================================================\r\n## Enumerations \r\n" +
            "======================================================================\r\n" +
            "The days of the week, and their corresponding values in the Days Enum are:\r\n" +
            "Saturday   = 0\r\nSunday     = 1\r\nMonday     = 2\r\nTuesday    = 3\r\nWednesday  = 4\r\n" +
            "Thursday   = 5\r\nFriday     = 6\r\n\r\n" +
            "Enums can also be created which have values that represent some meaningful amount.\r\n" +
            "The BoilingPoints Enum defines the following items, and corresponding values:\r\nCelsius    = 100\r\n" +
            "Fahrenheit = 212\r\n\r\nmyColors holds a combination of colors. Namely: Red, Blue, Yellow";

        [TestMethod()]
        public void MainTestTest()
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                EnumTest.MainTest();

                var result = sw.ToString().Trim();
                Assert.AreEqual(cExpected1, result);
            }
        }
    }
}