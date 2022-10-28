using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace HelloWorld.Tests
{
    [TestClass()]
    public class ProgramTests
    {
        private readonly string csExp = "Hello World!\r\n";

        [TestMethod()]
        public void MainTest()
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                Program.Main(new string[] { });
                sw.Flush();
                Assert.AreEqual(csExp, sw.ToString());

            }
        }
    }
}