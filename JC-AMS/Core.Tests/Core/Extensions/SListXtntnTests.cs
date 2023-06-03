using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace JCAMS.Core.Extensions.Tests
{
    [TestClass()]
    public class SListXtntnTests
    {
        protected static IEnumerable<object?[]> ContainsCIData => new[]
        {
            new object?[] {"null,Null",null,null,false},
            new object?[] {"A,null",new List<string> {"A" },null,false},
            new object[] {"A,a",new List<string> {"A" },"a",true}, 
            new object[] {"A b,a",new List<string> {"A b" },"a",true},
            new object[] {"A b;c D,d",new List<string> {"A b","c D" },"d",false}, //??
            new object[] { "A b;c D,c", new List<string> {"A b","c D" },"c",true},
            new object[] { "A b;c D e F,d", new List<string> {"A b","c D e F" },"d",true},
        };

        [DataTestMethod()]
        [DynamicData(nameof(ContainsCIData))]
        public void ContainsCITest(string name, List<string> l, string s,bool xExp)
        {
            Assert.AreEqual(xExp,l.ContainsCI(s),$"Test: {name}");
        }

        [DataTestMethod()]
        [DynamicData(nameof(ContainsCIData))]
        public void ContainsCITest2(string name, List<string> l, string s, bool xExp)
        {
            Assert.AreEqual(xExp, SListXtntn.ContainsCI(l,s), $"Test: {name}");
        }

    }
}