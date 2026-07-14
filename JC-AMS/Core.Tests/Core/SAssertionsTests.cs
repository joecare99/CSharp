using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;

namespace JCAMS.Core.Tests
{
    [TestClass()]
    public class SAssertionsTests
    {
        private MemoryStream listenerStream = default;

        [TestInitialize]
        public void Init()
        {
            listenerStream = new MemoryStream();
#if NET6_0_OR_GREATER
#else
            Debug.Listeners.Clear();
            Debug.Listeners.Add(new TextWriterTraceListener(listenerStream,"Debug"));
#endif
        }

        [DataTestMethod()]
        public void AssertTest()
        {
            SAssertions.Assert(true);
            Assert.AreEqual(0, listenerStream.Length);
            Assert.Throws<Exception>(() => SAssertions.Assert(false, "Some Message2"));
            Assert.AreEqual(0, listenerStream.Length);
        }

        [TestMethod()]
        public void AssertTest1()
        {
            SAssertions.Assert(true, "Some Message");
            Assert.AreEqual(0, listenerStream.Length);
            Assert.Throws<Exception>(() => SAssertions.Assert(false, "Some Message2"));
        }

        [TestMethod()]
        public void AssertTest2()
        {
            SAssertions.Assert(true, "Some Message", "Detail");
            Assert.Throws<Exception>(() => SAssertions.Assert(false, "Some Message2","Detail"));
        }
    }
}