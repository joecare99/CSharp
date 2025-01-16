using BaseLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BaseLib_netTests.Interfaces
{
    class TestIPClass : IParentedObject<List<object>>
    {
        private List<object>? _parent=null;
        public string Name { get; set; }
        public List<object>? GetParent()
            => _parent;

        public void SetParent(List<object>? value, [CallerMemberName] string CallerMember = "")
        {
            _parent?.Remove(this);
            _parent= value;
            value?.Add(this);
        }

        public override string ToString()
        {
            return $"PObj({Name})";
        }
    }

    [TestClass]
    public class IParentedObjectTests
    {
        private IParentedObject<List<object>>[] _child;
        private readonly List<object> _par1=new();
        private readonly List<object> _par2=new();

        [TestInitialize]
        public void Init()
        {
            _child = new IParentedObject<List<object>>[4];
            _child[0] = new TestIPClass() { Name = "1" };
            _child[1] = new TestIPClass() { Name = "2" };
            _child[2] = new TestIPClass() { Name = "3" };
            _child[3] = new TestIPClass() { Name = "4" };
        }

#if NET5_0_OR_GREATER
        [TestMethod]
        public void ParentTest() {
            Assert.IsNull(_child[0].Parent);
            Assert.AreEqual(0, _par1.Count);
            Assert.AreEqual(0, _par2.Count);
            _child[0].Parent = _par1;
            Assert.AreEqual(_par1, _child[0].Parent);
            Assert.AreEqual(1, _par1.Count);
            Assert.AreEqual(0, _par2.Count);
            Assert.IsTrue(_par1.Contains(_child[0]));
            _child[0].Parent = _par2;
            Assert.AreEqual(_par2, _child[0].Parent);
            Assert.AreEqual(1, _par2.Count);
            Assert.AreEqual(0, _par1.Count);
            Assert.IsTrue(_par2.Contains(_child[0]));
        }
#endif

        [TestMethod]
        public void SetParentTest()
        {
            Assert.IsNull(_child[0].GetParent());
            Assert.AreEqual(0, _par1.Count);
            Assert.AreEqual(0, _par2.Count);
            _child[0].SetParent(_par1);
            Assert.AreEqual(_par1, _child[0].GetParent());
            Assert.AreEqual(1, _par1.Count);
            Assert.AreEqual(0, _par2.Count);
            Assert.IsTrue(_par1.Contains(_child[0]));
            _child[0].SetParent(_par2);
            Assert.AreEqual(_par2, _child[0].GetParent());
            Assert.AreEqual(1, _par2.Count);
            Assert.AreEqual(0, _par1.Count);
            Assert.IsTrue(_par2.Contains(_child[0]));
        }

    }
}
