﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
#if NET6_0_OR_GREATER
            Assert.ThrowsException<Exception>(() => SAssertions.Assert(false, "Some Message2"));
#else
            SAssertions.Assert(false);
#endif
            Assert.AreEqual(0, listenerStream.Length);
        }

        [TestMethod()]
        public void AssertTest1()
        {
            SAssertions.Assert(true, "Some Message");
            Assert.AreEqual(0, listenerStream.Length);
#if NET6_0_OR_GREATER
            Assert.ThrowsException<Exception>(() => SAssertions.Assert(false, "Some Message2"));
#else
            SAssertions.Assert(false, "Some Message2");
#endif
        }

        [TestMethod()]
        public void AssertTest2()
        {
            SAssertions.Assert(true, "Some Message", "Detail");
#if NET6_0_OR_GREATER
            Assert.ThrowsException<Exception>(() => SAssertions.Assert(false, "Some Message2","Detail"));
#else
            SAssertions.Assert(false, "Some Message2", "Detail");
#endif
        }
    }
}