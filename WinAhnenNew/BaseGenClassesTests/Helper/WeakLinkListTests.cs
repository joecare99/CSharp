using Microsoft.VisualStudio.TestTools.UnitTesting;
using BaseGenClasses.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseGenClasses.Helper.Tests;

[TestClass()]
public class WeakLinkListTests
{
    private WeakLinkList<TestClass> _testClass;

    [TestInitialize()]
    public void Initialize()
    {
        _testClass = new WeakLinkList<TestClass>();
    }

    private void Arrange(int[] objects)
    {
        TestClass test;
        int i = -1;
        foreach (var obj in objects)
        {
            i++;
            _testClass.Add(test = new TestClass(obj));
            if (obj < 0)
            {
                _testClass[i] = null;
                test.Dispose();
                GC.Collect();
            }
        }
    }

    [TestMethod()]
    public void IsReadOnlyTest()
    {
        Assert.AreEqual(false, _testClass.IsReadOnly);
    }

    [TestMethod()]
    [DataRow(new[] { 1, 2, 3, 4 })]
    [DataRow(new[] { 1, 2, -1, 3, 4 })]

    public void AddTest(int[] objects)
    {
        Arrange(objects);

        Assert.AreEqual(objects.Length, _testClass.Count);
    }


    [TestMethod()]
    [DataRow(new[] { 1, 2, 3, 4 })]
    [DataRow(new[] { 1, 2, -1, 3, 4 })]
    public void ClearTest(int[] objects)
    {
        Arrange(objects);
        _testClass.Clear();
        Assert.AreEqual(0, _testClass.Count);
    }

    [TestMethod()]
    [DataRow(new[] { 1, 2, 3, 4 }, 2, true)]
    [DataRow(new[] { 1, 2, 3, 4 }, 5, false)]
    [DataRow(new[] { 1, 2, -1, 3, 4 }, 1, false)]
    public void ContainsTest(int[] objects, int oAct, bool xExp)
    {
        // Arrange
        Arrange(objects);

        // Act & Assert
        Assert.AreEqual(xExp, _testClass.Contains(new TestClass(oAct)));
    }

    [TestMethod()]
    [DataRow(new[] { 1, 2, 3, 4 })]
    [DataRow(new[] { 1, 2, -1, 3, 4 })]
    public void CopyToTest(int[] objects)
    {
        Arrange(objects);

        TestClass[] array = new TestClass[4];
        _testClass.CopyTo(array, 0);
        Assert.AreEqual(4, array.Length);

    }

    [TestMethod()]
    [DataRow(new[] { 1, 2, 3, 4 })]
    [DataRow(new[] { 1, 2, -1, 3, 4 })]
    public void GetEnumeratorTest(int[] ints)
    {
        Arrange(ints);

        foreach (var obj in _testClass)
        {
            Assert.IsTrue(ints.ToList().Contains((int)obj.IntObj));
        }
    }
    [TestMethod()]
    [DataRow(new[] { 1, 2, 3, 4 })]
    [DataRow(new[] { 1, 2, -1, 3, 4 })]
    public void GetEnumeratorTest2(int[] ints)
    {
        Arrange(ints);

        foreach (var obj in _testClass as System.Collections.IEnumerable)
        {
            Assert.IsTrue(ints.ToList().Contains((obj as TestClass)?.IntObj ?? -1));
        }
    }

    [TestMethod()]
    [DataRow(new[] { 1, 2, 3, 4 }, 2, 1)]
    [DataRow(new[] { 1, 2, 3, 4 }, 5, -1)]
    [DataRow(new[] { 1, 2, -1, 3, 4 }, 1, -1)]
    public void IndexOfTest(int[] aInts, int oAct, int iExp)
    {
        Arrange(aInts);
 
        Assert.AreEqual(iExp, _testClass.IndexOf(new TestClass(oAct)));
    }

    [TestMethod()]
    [DataRow(new[] { 1, 2, 3, 4 }, 2, 5)]
 //   [DataRow(new[] { 1, 2, 3, 4 }, 5, -1)]
    [DataRow(new[] { 1, 2, -1, 3, 4 }, 1, -1)]
    public void InsertTest(int[] aInts, int iAct, int iAct2)
    {
        Arrange(aInts);

        _testClass.Insert(iAct, new TestClass(iAct2));
        Assert.AreEqual(true, _testClass.Contains(new TestClass(iAct2)));
        Assert.AreEqual(iAct, _testClass.IndexOf(new TestClass(iAct2)));
    }

    [TestMethod()]
    [DataRow(new[] { 1, 2, 3, 4 }, 5, -1)]
    public void InsertTest2(int[] aInts, int iAct, int iAct2)
    {
        Arrange(aInts);

        Assert.ThrowsException<ArgumentOutOfRangeException>(()=> _testClass.Insert(iAct, new TestClass(iAct2)));
    }

    [TestMethod()]
    [DataRow(new[] { 1, 2, 3, 4 }, 2, true)]
    [DataRow(new[] { 1, 2, 3, 4 }, 5, false)]
    [DataRow(new[] { 1, 2, -1, 3, 4 }, 1, false)]
    public void RemoveTest(int[] aInts, int iAct,bool xExp)
    {
        Arrange(aInts);

        Assert.AreEqual(xExp, _testClass.Remove( new TestClass(iAct)));
        Assert.AreEqual(false, _testClass.Contains(new TestClass(iAct)));
    }

    [TestMethod()]
    [DataRow(new[] { 1, 2, 3, 4 }, 2, true)]
//    [DataRow(new[] { 1, 2, 3, 4 }, 5, false)]
    [DataRow(new[] { 1, 2, -1, 3, 4 }, 1, false)]
    public void RemoveAtTest(int[] aInts, int iAct,bool xExp)
    {
        Arrange(aInts);

        _testClass.RemoveAt(iAct);
    }

    [TestMethod()]
    [DataRow(new[] { 1, 2, 3, 4 }, 5, false)]
    public void RemoveAtTest2(int[] aInts, int iAct,bool xExp)
    {
        Arrange(aInts);

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => _testClass.RemoveAt(iAct));
    }

    [TestMethod()]
    [DataRow(new[] { 1, 2, 3, 4 }, 2, 3)]
    [DataRow(new[] { 1, 2, 3, 4 }, 3, 4)]
    [DataRow(new[] { 1, 2, -1, 3, 4 }, 1, null)]
    [DataRow(new[] { 1, 2, -1, 3, 4 }, 2, null)]
    public void IndexTest(int[] aInts, int iAct, int? iExp)
    {
        Arrange(aInts);

        Assert.AreEqual(iExp,_testClass[iAct]?.IntObj);
    }

    [TestMethod()]
    [DataRow(new[] { 1, 2, 3, 4 }, 2, 5)]
    [DataRow(new[] { 1, 2, 3, 4 }, 1, 6)]
    public void SetThisTest(int[] aInts, int iAct, int iExp)
    {
        Arrange(aInts);

        _testClass[iAct] = new TestClass(iExp);
        Assert.AreEqual(4,_testClass.Count);
        Assert.AreEqual(iExp, _testClass[iAct]?.IntObj);
    }


}

internal class TestClass(int _int) : IDisposable
{
    public int IntObj { get; } = _int;

    public void Dispose() { }

    public override bool Equals(object? obj) 
        => obj is TestClass tc ? tc.IntObj.Equals(IntObj) : IntObj.Equals(obj);
}

