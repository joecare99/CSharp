using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using GenFree.Interfaces;
using System.Collections;
using static BaseLib.Helper.TestHelper;

namespace GenFree.Sys.Tests;

[TestClass()]
public class CArrayProxyTests
{
    private CArrayProxy<string> _testClass;
    private string[] _testData = new string[] { "1", "2", "3" };

    [TestInitialize]
    public void Init()
    {
        _testClass = new CArrayProxy<string>(i => (int)i < _testData.Length ? _testData[(int)i] : null, (i, s) => _testData[(int)i] = s);
        _ = _testClass.getenum!();
        _testClass.getenum = () => _testData.AsEnumerable().GetEnumerator();
    }
    [TestMethod()]
    public void CArrayProxyTest()
    {
        Assert.IsNotNull(_testClass);
        Assert.IsNotNull(_testData);
        Assert.IsInstanceOfType(_testClass, typeof(CArrayProxy<string>));
        Assert.IsInstanceOfType(_testData, typeof(string[]));
        Assert.IsInstanceOfType(_testClass, typeof(IArrayProxy<string>));
        Assert.IsInstanceOfType(_testClass, typeof(IEnumerable<string>));
    }

    [TestMethod()]
    public void GetEnumeratorTest()
    {
        Assert.IsNotNull(_testClass.GetEnumerator());
        Assert.IsInstanceOfType(_testClass.GetEnumerator(), typeof(IEnumerator<string>));
        Assert.IsNotNull((_testClass as IEnumerable).GetEnumerator());
        Assert.IsInstanceOfType((_testClass as IEnumerable).GetEnumerator(), typeof(IEnumerator));
        _testClass.getenum = null;
        Assert.IsNull(_testClass.GetEnumerator());
        Assert.IsNull((_testClass as IEnumerable).GetEnumerator());

    }

    [DataTestMethod()]
    [DataRow(0, "1")]
    [DataRow(1, "2")]
    [DataRow(2, "3")]
    [DataRow(3, null)]
    public void GetValueTest(int iIdx, string sExp)
    {
        Assert.AreEqual(sExp, _testClass[iIdx]);
    }

    [DataTestMethod()]
    [DataRow(0, "A")]
    [DataRow(1, "B")]
    [DataRow(2, "C")]
    public void SetValueTest(int iIdx, string sExp)
    {
        _testClass[iIdx] = sExp;
        Assert.AreEqual(sExp, _testData[iIdx]);
        for (int i = 0; i < _testData.Length; i++)
        {
            if (i != iIdx)
            {
                Assert.AreEqual((i + 1).ToString(), _testData[i]);
            }
        }
    }
    [DataTestMethod()]
    [DataRow(0, "A")]
    [DataRow(1, "B")]
    [DataRow(2, "C")]
    public void SetValue2Test(int iIdx, string sExp)
    {
        _testClass = new(i => $"{i}");
        _testClass[iIdx] = sExp;
        for (int i = 0; i < _testData.Length; i++)
        {
            Assert.AreEqual((i + 1).ToString(), _testData[i]);
        }
    }

    [TestMethod()]
    public void ToArrayTest()
    {
        AssertAreEqual(_testData, _testClass);
    }


}