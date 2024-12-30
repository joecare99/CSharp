using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace TestStatements.SystemNS.Xml.Tests
{
    /// <summary>
    /// Defines test class XmlNSTests.
    /// </summary>
    [TestClass()]
    public class XmlNSTests
    {
        private const string Expected = "<?xml version=\"1.0\" encoding=\"UTF8\"?><ROOT Charset=\"UTF8\"><Node>Node Text</Node><Node2>Node2 Text</Node2></ROOT>";
        //                          "<?xml version=\"1.0\" encoding=\"UTF8\"?><ROOT Charset=\"UTF8\"><Node>Node Text</Node><Node2>Node2 Text</Node2></ROOT>"
        /// <summary>
        /// Defines the test method XmlTest1Test.
        /// </summary>
        [TestMethod()]
        public void XmlTest1Test()
        {
            XmlNS.XmlTest1();
        }

        /// <summary>
        /// Defines the test method XmlTest2Test.
        /// </summary>
        [TestMethod()]
        public void XmlTest2Test()
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                XmlNS.XmlTest1();

                var result = sw.ToString().Trim();
                Assert.AreEqual(Expected, result);
            }
                
        }
    }
}