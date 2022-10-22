using JCAMS.Core.DataOperations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JCAMS.Tests.Core.DataOperations
{
    [TestClass()]
    public class TDataHelpersTests
    {
        [TestMethod()]
        [TestProperty("Author", "JC")]
        public void HiByteTest()
        {
            for (short i = 0; i < 256; i++)
                for (short j = 0; j <= 255; j++)
                    Assert.AreEqual((byte)i, ((short)(j + 256 * i)).HiByte());
        }

        [TestMethod()]
        [TestProperty("Author", "JC")]
        public void HiByteTest1()
        {
            for (ushort i = 0; i < 256; i++)
                for (ushort j = 0; j <= 255; j++)
                    Assert.AreEqual((byte)i, ((ushort)(j + 256 * i)).HiByte());
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("", 0, 0u)]
        [DataRow("0xff", 0xff, 0u)]
        [DataRow("0xff00", 0xff00, 0u)]
        [DataRow("0xff0000", 0xff0000, 0u)]
        [DataRow("0xff000000", 0xff000000, 0x0u)]
        [DataRow("0xff00000000", 0xff00000000, 0xffu)]
        [DataRow("0xff0000000000", 0xff0000000000, 0xff00u)]
        [DataRow("0xff000000000000", 0xff000000000000, 0xff0000u)]
        [DataRow("0x7f00000000000000", 0x7f00000000000000, 0x7f000000u)]
        [DataRow("0xff-2", 0xff, 0u)]
        public void HiDWordTest(string name, long l, uint udExp)
        {
            Assert.AreEqual(udExp, l.HiDWord(), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("", 0u, 0u)]
        [DataRow("0xff", 0xffu, 0u)]
        [DataRow("0xff00", 0xff00u, 0u)]
        [DataRow("0xff0000", 0xff0000u, 0u)]
        [DataRow("0xff000000", 0xff000000u, 0x0u)]
        [DataRow("0xff00000000", 0xff00000000u, 0xffu)]
        [DataRow("0xff0000000000", 0xff0000000000u, 0xff00u)]
        [DataRow("0xff000000000000", 0xff000000000000u, 0xff0000u)]
        [DataRow("0xff00000000000000", 0xff00000000000000u, 0xff000000u)]
        //        [DataRow("0xff00000000000000", 0xf0000000000000000, 0)]
        [DataRow("0xff-2", 0xffu, 0u)]
        public void HiDWordTest1(string name, ulong l, uint udExp)
        {
            Assert.AreEqual(udExp, l.HiDWord(), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("", 0, (ushort)0u)]
        [DataRow("0xff", 0xff, (ushort)0u)]
        [DataRow("0xff00", 0xff00, (ushort)0u)]
        [DataRow("0xff0000", 0xff0000, (ushort)0xffu)]
        [DataRow("0xff000000", 0x7f000000, (ushort)0x7f00u)]
        public void HiWordTest(string name, int i, ushort usExp)
        {
            Assert.AreEqual(usExp, i.HiWord(), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("", 0u, (ushort)0u)]
        [DataRow("0xff", 0xffu, (ushort)0u)]
        [DataRow("0xff00", 0xff00u, (ushort)0u)]
        [DataRow("0xff0000", 0xff0000u, (ushort)0xffu)]
        [DataRow("0xff000000", 0xff000000u, (ushort)0xff00u)]
        public void HiWordTest1(string name, uint i, ushort usExp)
        {
            Assert.AreEqual(usExp, i.HiWord(), $"Test: {name}");
        }

        [TestMethod()]
        [TestProperty("Author", "JC")]
        public void LoByteTest()
        {
            for (short i = 0; i < 256; i++)
                for (short j = 0; j <= 255; j++)
                    Assert.AreEqual((byte)j, ((short)(j + 256 * i)).LoByte());
        }

        [TestMethod()]
        [TestProperty("Author", "JC")]
        public void LoByteTest1()
        {
            for (short i = 0; i < 256; i++)
                for (short j = 0; j <= 255; j++)
                    Assert.AreEqual((byte)j, ((ushort)(j + 256 * i)).LoByte());
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("", 0, 0u)]
        [DataRow("0xff", 0xff, 0xffu)]
        [DataRow("0xff00", 0xff00, 0xff00u)]
        [DataRow("0xff0000", 0xff0000, 0xff0000u)]
        [DataRow("0xff000000", 0xff000000, 0xff000000u)]
        [DataRow("0xff00000000", 0xff00000000, 0x0u)]
        [DataRow("0xff0000000000", 0xff0000000000, 0x00u)]
        [DataRow("0xff000000000000", 0xff000000000000, 0x0000u)]
        [DataRow("0xff00000000000000", 0x7f00000000000000, 0x000000u)]
        public void LoDWordTest(string name, long l, uint udExp)
        {
            Assert.AreEqual(udExp, l.LoDWord(), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("", 0u, 0u)]
        [DataRow("0xff", 0xffu, 0xffu)]
        [DataRow("0xff00", 0xff00u, 0xff00u)]
        [DataRow("0xff0000", 0xff0000u, 0xff0000u)]
        [DataRow("0xff000000", 0xff000000u, 0xff000000u)]
        [DataRow("0xff00000000", 0xff00000000u, 0x0u)]
        [DataRow("0xff0000000000", 0xff0000000000u, 0x00u)]
        [DataRow("0xff000000000000", 0xff000000000000u, 0x0000u)]
        [DataRow("0xff00000000000000", 0xff00000000000000u, 0x000000u)]
        public void LoDWordTest1(string name, ulong l, uint udExp)
        {
            Assert.AreEqual(udExp, l.LoDWord(), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("", 0, (ushort)0u)]
        [DataRow("0xff", 0xff, (ushort)0xffu)]
        [DataRow("0xff00", 0xff00, (ushort)0xff00u)]
        [DataRow("0xff0000", 0xff0000, (ushort)0u)]
        [DataRow("0xff000000", 0x7f000000, (ushort)0x00u)]
        public void LoWordTest(string name, int i, ushort usExp)
        {
            Assert.AreEqual(usExp, i.LoWord(), $"Test: {name}");
        }

        [DataTestMethod()]
        [TestProperty("Author", "JC")]
        [DataRow("", 0u, (ushort)0u)]
        [DataRow("0xff", 0xffu, (ushort)0xffu)]
        [DataRow("0xff00", 0xff00u, (ushort)0xff00u)]
        [DataRow("0xff0000", 0xff0000u, (ushort)0u)]
        [DataRow("0xff000000", 0xff000000u, (ushort)0x00u)]
        public void LoWordTest1(string name, uint i, ushort usExp)
        {
            Assert.AreEqual(usExp, i.LoWord(), $"Test: {name}");
        }
    }
}