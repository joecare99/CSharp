using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using TestStatements.Constants;

namespace TestStatementTest.ConstantTests
{
    [TestClass()]
    public class ConstantsTests
    {
        private const int cExpLoremIpsum = -466537496;//-687218507;// -466537496;//1742248702
        private const int cExpHelloWorld = -1497658439;//485358840;// 1253652992;

        [DataTestMethod()]
        [DataRow(1,Constants.LoremIpsum,typeof(string),5050, DisplayName = "Lorem Ipsum ...")]
        [DataRow(2,Constants.dGoldenCut, typeof(double),1, DisplayName = "the golden cut 1.61...")]
        [DataRow(3, Constants.HelloWorld, typeof(string),13, DisplayName = "Just 'Hello World !'")]
        [DataRow(4, Constants.Header, typeof(string), 150, DisplayName = "The 1st Header")]
        [DataRow(5, Constants.Header2, typeof(string), 129, DisplayName = "The 2nd Header")]
        public void TestRealConst(int i, object o,Type tExpType,int iExpVal)
        {
            Assert.IsNotNull(o);
            Assert.IsInstanceOfType(o,tExpType);
            if (o is string s)
            { 
                Assert.AreEqual(iExpVal,s.Length,$".Length{i}"); 
            }
        }

        [TestMethod]
        public void TestGoldenCut()
        {
            Assert.AreEqual(5d, (Constants.dGoldenCut * 2d - 1d) * (Constants.dGoldenCut * 2d - 1d), 1e-14);
            Assert.AreEqual(Constants.dGoldenCut2, Constants.dGoldenCut, 1e-14);
        }

        [TestMethod]
        public void TestHelloWorld()
        {
            Assert.IsTrue(Constants.HelloWorld.StartsWith("Hello"));
            Assert.IsTrue(Constants.HelloWorld.EndsWith("!"));
            Assert.IsTrue(Constants.HelloWorld.Contains("World"));
            Assert.AreEqual(13, Constants.HelloWorld.Length, "length");
            Assert.AreEqual(2, Constants.HelloWorld.Count((c) => c == ' '), "Spaces");
            Assert.AreEqual("Hello World !", Constants.HelloWorld);
            Assert.AreEqual(cExpHelloWorld, Constants.HelloWorld.GetHashCode(), "Hash");
        }

        [DataTestMethod]
        [DataRow(1, "Hello", true)]
        [DataRow(2, "!", true)]
        [DataRow(3, "World", true)]
        [DataRow(4, "Hash:", cExpHelloWorld)]
        [DataRow(5, "Length", 13)]
        [DataRow(6, "\n", 0)]
        [DataRow(7, " ", 2)]
        [DataRow(8, "Content", "Hello World !")]
        public void TestHelloWorld2(int i, string sVal, object oExp)
        {
            switch (i)
            {
                case 1: Assert.AreEqual(oExp, Constants.HelloWorld.StartsWith(sVal), $"Startswith{sVal}"); break;
                case 2: Assert.AreEqual(oExp, Constants.HelloWorld.EndsWith(sVal), "Endswith"); break;
                case 3: Assert.AreEqual(oExp, Constants.HelloWorld.Contains(sVal)); break;
                case 4: Assert.AreEqual(oExp, Constants.HelloWorld.GetHashCode(), "Hash"); break;
                case 5: Assert.AreEqual(oExp, Constants.HelloWorld.Length, "length"); break;
                case 6: Assert.AreEqual(oExp, Constants.HelloWorld.Count((c) => c == sVal[0]), "(new) Lines"); break;
                case 7: Assert.AreEqual(oExp, Constants.HelloWorld.Count((c) => c == sVal[0]), "Spaces"); break;
                case 8: Assert.AreEqual(oExp, Constants.HelloWorld, "Content"); break;
            }
        }

        [TestMethod]
        public void TestLoremIpsum()
        {
            Assert.IsTrue(Constants.LoremIpsum.StartsWith("Lorem ipsum"),"Startswith");
            Assert.IsTrue(Constants.LoremIpsum.EndsWith("."),"Endswith");
            Assert.IsTrue(Constants.LoremIpsum.Contains("class"));
            Assert.AreEqual(5050, Constants.LoremIpsum.Length, "length");
            Assert.AreEqual(71, Constants.LoremIpsum.Count((c) => c == '\n'), "(new) Lines");
            Assert.AreEqual(673, Constants.LoremIpsum.Count((c)=>c==' '), "Spaces");
            Assert.AreEqual(cExpLoremIpsum, Constants.LoremIpsum.GetHashCode(),"Hash");
        }

        [DataTestMethod]
        [DataRow(1, "Lorem ipsum",true)]
        [DataRow(2, ".", true)]
        [DataRow(3, "class", true)]
        [DataRow(4, "Hash:", cExpLoremIpsum)]
        [DataRow(5, "Length", 5050)]
        [DataRow(6, "\n", 71)]
        [DataRow(7, " ", 673)]
        public void TestLoremIpsum2(int i,string sVal,object oExp)
        {
            switch (i)
            {
                case 1: Assert.AreEqual(oExp, Constants.LoremIpsum.StartsWith(sVal), $"Startswith{sVal}"); break;
                case 2: Assert.AreEqual(oExp, Constants.LoremIpsum.EndsWith(sVal), "Endswith");break; 
                case 3: Assert.AreEqual(oExp, Constants.LoremIpsum.Contains(sVal));break;
                case 4: Assert.AreEqual(oExp, Constants.LoremIpsum.GetHashCode(), "Hash");                    break;
                case 5: Assert.AreEqual(oExp, Constants.LoremIpsum.Length, "length");                    break;
                case 6: Assert.AreEqual(oExp, Constants.LoremIpsum.Count((c) => c == sVal[0]), "(new) Lines");                    break;
                case 7: Assert.AreEqual(oExp, Constants.LoremIpsum.Count((c) => c == sVal[0]), "Spaces");break;
            }
        }
    }
}
