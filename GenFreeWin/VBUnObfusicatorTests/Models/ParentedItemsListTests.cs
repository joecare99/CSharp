using Microsoft.VisualStudio.TestTools.UnitTesting;
using VBUnObfusicator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.CompilerServices;

namespace VBUnObfusicator.Models.Tests
{
    public interface ITestItf : IHasParents<ITestItf>, IEquatable<ITestItf> 
    {
        int iData { get; set; }
        ITestItf this[int idx] { get; }

        void Add(ITestItf item);
    }

    public class TestClass : ITestItf
    {
        public TestClass(int v)
        {
            iData = v;
            _list = new CSCode.ParentedItemsList<ITestItf>(this);
        }

        public ITestItf this[int idx] => _list[idx];

        public ITestItf Parent { get; set; }
        public int iData { get ; set ; }

        private CSCode.ParentedItemsList<ITestItf> _list;

        public bool Equals(ITestItf? other)
        {
            if (other is null)
                return false;
            return iData == other.iData;
        }

        public void Add(ITestItf item)
        {
            _list.Add(item);
        }
    }

    [TestClass()]
    public class ParentedItemsListTests : ITestItf
    {
        public ITestItf Parent { get ;  set ; }
        public int iData { get; set; }

        public ITestItf this[int idx] => testClass[idx];

        CSCode.ParentedItemsList<ITestItf> testClass;


        [TestInitialize()]
        public void Init()
        {
            testClass = new(this);
        }

        [TestMethod()]
        public void IsReadonlyTest()
        {
            Assert.AreEqual(false,testClass.IsReadOnly);
        }

        [TestMethod()]
        public void ParentedItemsListTest()
        {
            AddTest();
            Assert.IsNotNull(testClass.GetEnumerator());
            Assert.IsInstanceOfType(testClass.GetEnumerator(),typeof(IEnumerator<ITestItf>));
        }

        [TestMethod()]
        public void IndexOfTest()
        {
            AddTest();
            Assert.AreEqual(0, testClass.IndexOf(new TestClass(3)));
            Assert.AreEqual(-1, testClass.IndexOf(new TestClass(2)));
            Assert.AreEqual(-1, testClass.IndexOf(this));
        }

        [TestMethod()]
        public void InsertTest()
        {
            Assert.AreEqual(0, testClass.Count);
            testClass.Insert(0,new TestClass(3));
            testClass.Insert(1,new TestClass(5));
            testClass[1].Add(new TestClass(11));
            testClass[1].Add(new TestClass(7));
            testClass.Insert(2,testClass[1][1]);
            Assert.AreEqual(3, testClass.Count);
        }

        [TestMethod()]
        public void RemoveAtTest()
        {
            AddTest();
            testClass.RemoveAt(0);
            Assert.AreEqual(2, testClass.Count);
            testClass.RemoveAt(1);
            Assert.AreEqual(1, testClass.Count);
            testClass.RemoveAt(0);
            Assert.AreEqual(0, testClass.Count);
        }

        [TestMethod()]
        public void AddTest()
        {
            Assert.AreEqual(0, testClass.Count);
            testClass.Add(new TestClass(3));
            testClass.Add(new TestClass(5));
            testClass[1].Add(new TestClass(11));
            testClass[1].Add(new TestClass(7));
            testClass.Add(testClass[1][1]);
            Assert.AreEqual(3, testClass.Count);
        }

        [TestMethod()]
        public void ClearTest()
        {
            AddTest();
            testClass.Clear();
            Assert.AreEqual(0, testClass.Count);
        }

        [TestMethod()]
        public void ContainsTest()
        {
            AddTest();
            Assert.AreEqual(true, testClass.Contains( new TestClass(3)));
            Assert.AreEqual(false, testClass.Contains(new TestClass(2)));
            Assert.AreEqual(false, testClass.Contains(this));
        }

        [TestMethod()]
        public void CopyToTest()
        {
            AddTest();
            testClass.CopyTo(new ITestItf[3], 0);
        }

        [TestMethod()]
        public void RemoveTest()
        {
            AddTest();
            testClass.Remove(new TestClass(3));
        }

        [TestMethod()]
        public void GetEnumeratorTest()
        {
            Assert.IsNotNull(((IEnumerable)testClass).GetEnumerator());
            Assert.IsInstanceOfType(((IEnumerable)testClass).GetEnumerator(), typeof(IEnumerator));
        }

        public bool Equals(ITestItf? other)
        {
            if (other is null)
                return false;
            return iData == other.iData;
        }

        public void Add(ITestItf item)
        {
            throw new NotImplementedException();
        }
    }
}