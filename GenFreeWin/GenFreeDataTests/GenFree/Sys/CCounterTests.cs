using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenFree.GenFree.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.Sys.Tests
{
    [TestClass()]
    public class CCounterTests
    {
        CCounter<int> testClass;
        CCounter<byte> testClass2;

        [TestInitialize]
        public void Init()
        {
            testClass = new((t) => --t, (t) => ++t);
            testClass2 = new(t=>--t,t=>++t);
        }
        [TestMethod()]
        public void CCounterTest()
        {
            Assert.IsNotNull(testClass);
            Assert.IsNotNull(testClass2);
        }

        [DataTestMethod()]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(16)]
        [DataRow(257)]
        public void DecTest1(int iC)
        {
            for (int i = 0; i < iC; i++)
            {
                Assert.AreEqual(-i, testClass.Value);
                testClass.Dec();
            }
            Assert.AreEqual(-iC, testClass.Value);
        }

        [TestMethod()]
        public void DecTest3()
        {
            var c = new CCounter<short>(null!, null!);
            Assert.AreEqual((short)0, c.Value);
            c.Dec();
            Assert.AreEqual((short)0, c.Value);
        }

        [DataTestMethod()]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(16)]
        [DataRow(257)]
        public void DecTest2(int iC)
        {
            for (int i = 0; i < iC; i++)
            {
                Assert.AreEqual((512-i)%256 , testClass2.Value);
                testClass2.Dec();
            }
            Assert.AreEqual((512 - iC) % 256, testClass2.Value);
        }

        [DataTestMethod()]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(16)]
        [DataRow(257)]
        public void IncTest1(int iC)
        {
            for (int i = 0; i < iC; i++)
            {
                Assert.AreEqual(i, testClass.Value);
                testClass.Inc();
            }
            Assert.AreEqual(iC, testClass.Value);
        }
        [TestMethod()]
        public void IncTest3()
        {
            var c = new CCounter<short>(null!, null!);
            Assert.AreEqual((short)0, c.Value);
            c.Inc();
            Assert.AreEqual((short)0, c.Value);
        }

        [TestMethod()]
        public void ResetTest()
        {
            Assert.AreEqual(0, testClass.Value);
            testClass.Inc();
            Assert.AreEqual(1, testClass.Value);
            testClass.Reset();
            Assert.AreEqual(0, testClass.Value);
        }
    }
}