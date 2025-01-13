using System;
using System.Collections.Generic;
using TranspilerLib.Interfaces.Code;
using System.Diagnostics;
using TranspilerLib.Data;

namespace TranspilerLib.Models.Scanner.Tests
{
    [TestClass()]
    public class IECTokenHandlerTests
    {
        ITokenHandler testHandler;

        [TestInitialize]
        public void Init()
        {
            testHandler = new IECTokenHandler()
            {
                reservedWords = IECCode.ReservedWords2,
                blockWords = IECCode.IECBlocksWords,
                sysTypes = IECCode.IECSysTypes
            };
        }

        [TestMethod()]
        public void InitTest()
        {
            Assert.IsNotNull(testHandler);
            Assert.IsInstanceOfType(testHandler, typeof(ITokenHandler));
            Assert.IsInstanceOfType(testHandler, typeof(IECTokenHandler));
        }


        [TestMethod()]
        [DataRow(-1, false,"")]
        [DataRow(0, true, "HandleDefault")]
        [DataRow(1, true, "HandleAlpha")]
        [DataRow(2, true, "HandleLineComments")]
        [DataRow(3, true, "HandleBlockComments")]
        [DataRow(4, true, "HandleStrings")]
        [DataRow(5, true, "HandleStrings")]
        [DataRow(6, true, "HandleStrings")]
        [DataRow(7, true, "<.cctor>b__*")]
        [DataRow(8, true, "HandleNumbers")]
        [DataRow(9, false, "")]
        public void TryGetValueTest(int iAct, bool xAct,string sExpName)
        {
            Assert.AreEqual(xAct, testHandler.TryGetValue(iAct, out var handler));
            if (xAct)
            {
                Assert.IsNotNull(handler);
                Assert.IsInstanceOfType(handler, typeof(Action<ICodeBase.TokenDelegate?, string, TokenizeData>));
               
                Debug.WriteLine(handler.Method.Name);
                if (!sExpName.EndsWith("*"))
                Assert.AreEqual(sExpName, handler.Method.Name);
                else
                {
                    Assert.AreEqual(sExpName.Substring(0, sExpName.Length - 1),handler.Method.Name.Substring(0, sExpName.Length - 1));
                }
            }
        }

        [TestMethod()]
        public void HandleDefaultTest()
        {
            Assert.IsTrue(testHandler.TryGetValue(0, out var handler));
        }

        [TestMethod()]
        public void HandleAlphaTest()
        {
            Assert.IsTrue(testHandler.TryGetValue(1, out var handler));
        }

        [TestMethod()]
        public void HandleLineCommentsTest()
        {
            Assert.IsTrue(testHandler.TryGetValue(2, out var handler));
        }

        [TestMethod()]
        public void HandleBlockCommentsTest()
        {
            Assert.IsTrue(testHandler.TryGetValue(3, out var handler));
        }

        [TestMethod()]
        public void HandleStringsTest()
        {
            Assert.IsTrue(testHandler.TryGetValue(4, out var handler));
        }

        [TestMethod()]
        [DataRow(8,1,"0 ",1,0,1, "0")]
        [DataRow(8, 1, "0a", 1, 1, 1,"0")]
        [DataRow(8, 1, "0ab", 1, 1, 1, "0")]
        [DataRow(8, 1, "1.", 1, 8, 2, "1.")]
        [DataRow(8, 1, "1.0", 0, 8, 1,"")]
        [DataRow(8, 1, "1..", 1, 0, 1, "1")]
        [DataRow(8, 1, "1..5", 1, 0, 1, "1")]
        [DataRow(8, 1, "2e", 1, 8, 2,"2e")]
        [DataRow(8, 1, "2E5", 0, 8, 1,"")]
        [DataRow(8, 1, "e-", 1, 8, 2, "e-")]
        [DataRow(8, 1, "E-2", 0, 8, 1,"")]
        [DataRow(8, 1, "3-", 2, 0, 1,"3")]
        [DataRow(8, 1, "3-e", 2, 0, 1,"3")]
        public void HandleNumbersTest(int iState, int iPos, string sAct, int iExpCnt, int iExpState, int iExpPos,string sExpCode)
        {
            // Arrange
            Assert.IsTrue(testHandler.TryGetValue(8, out var handler));
            var data = new TokenizeData() { State = iState, Pos = iPos, Pos2=0 };
            int iCnt = 0;
            List<TokenData> lData = new();

            // Act
            handler((d) => { iCnt++;lData.Add(d); }, sAct, data);

            // Assert
            Assert.AreEqual(iExpCnt, iCnt,nameof(iCnt));
            Assert.AreEqual(iExpState, data.State, nameof(data.State));
            Assert.AreEqual(iExpPos, data.Pos, nameof(data.Pos));
            Assert.AreEqual(iExpCnt, lData.Count, nameof(lData.Count));
            if (lData.Count >0)
            {
                Assert.AreEqual(CodeBlockType.Number, lData[0].type);
                Assert.AreEqual(0, lData[0].Level);
                Assert.AreEqual(0, lData[0].Pos);
                Assert.AreEqual(sExpCode, lData[0].Code);
            }
        }

        [TestMethod()]
        [DataRow(0, 0, "Test", 0, 0, 0)]
        [DataRow(1, 0, "Test", 0, 1, 0)]
        [DataRow(1, 0, "};", 0, 4, 0)]

        public void AnonymousHandlerTest(int iState,int iPos, string sAct,int iExpCnt, int iExpState, int iExpPos)
        {
            // Arrange
            Assert.IsTrue(testHandler.TryGetValue(7, out var handler));
            var data = new TokenizeData() {State = iState,Pos = iPos  };
            int iCnt = 0;

            // Act
            handler((d)=> iCnt++, sAct, data);

            // Assert
            Assert.AreEqual(iExpCnt, iCnt);
            Assert.AreEqual(iExpState, data.State);
            Assert.AreEqual(iExpPos, data.Pos);
        }
    }
}