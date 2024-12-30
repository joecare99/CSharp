using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenFree.Sys.Tests;

[TestClass()]
public class CCounterTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    CCounter<int> testClass;
    CCounter<byte> testClass2;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    [TestInitialize]
    public void Init()
    {
        testClass = new((t) => --t, (t) => ++t);
        testClass2 = new(t => --t, t => ++t);
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
            Assert.AreEqual((512 - i) % 256, testClass2.Value);
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