using System;
using TranspilerLib.Interfaces.Code;
using System.Diagnostics;
using System.Collections.Generic;
using TranspilerLib.Data;

namespace TranspilerLib.Models.Scanner.Tests
{
    [TestClass()]
    public class CSTokenHandlerTests
    {
        ITokenHandler testHandler;

        [TestInitialize]
        public void Init()
        {
            testHandler = new CSTokenHandler()
            {
                stringEndChars = CSCode.stringEndChars,
                reservedWords = CSCode.ReservedWords,
            };
        }

        [TestMethod()]
        public void InitTest()
        {
            Assert.IsNotNull(testHandler);
            Assert.IsInstanceOfType(testHandler, typeof(ITokenHandler));
            Assert.IsInstanceOfType(testHandler, typeof(CSTokenHandler));
        }

        [TestMethod()]
        [DataRow(-1, false, "")]
        [DataRow(0, true, "HandleDefault")]
        [DataRow(1, true, "HandleStrings")]
        [DataRow(2, true, "HandleLineComments")]
        [DataRow(3, true, "HandleBlockComments")]
        [DataRow(4, true, "HandleStrings")]
        [DataRow(5, true, "HandleStrings")]
        [DataRow(6, true, "<.cctor>b__*")]
        [DataRow(7, false, "")]
        public void TryGetValueTest(int iAct, bool xAct, string sExpName)
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
                    Assert.AreEqual(sExpName.Substring(0, sExpName.Length - 1), handler.Method.Name.Substring(0, sExpName.Length - 1));
                }
            }
        }

        [TestMethod()]
        [DataRow(0, 1, "_  ", 0, 0, 1, "")]
        [DataRow(0, 3, " if ", 1, 0, 3, "if")]
        [DataRow(0, 1, "_\" ", 1, 1, 1, "_")]
        [DataRow(0, 2, "_$\" ", 1, 4, 2, "_")]
        [DataRow(0, 2, "_@\" ", 1, 5, 2, "_")]
        [DataRow(0, 1, "_/ ", 0, 0, 1, "")]
        [DataRow(0, 1, "_/*", 1, 3, 2, "_")]
        [DataRow(0, 1, "_//", 1, 2, 2, "_")]
        [DataRow(0, 1, "_: ", 1, 0, 1, "_:")]
        [DataRow(0, 1, "_; ", 1, 0, 1, "_;")]
        [DataRow(0, 6, "goto _; ", 1, 0, 6, "goto _;")]
        [DataRow(0, 1, "_? ", 0, 0, 1, "")]
        [DataRow(0, 1, "_{ ", 2, 0, 1, "_")]
        [DataRow(0, 1, "_} ", 2, 0, 1, "_")]
        [DataRow(0, 1, "_a ", 0, 0, 1, "")]
        [DataRow(0, 1, ")a ", 1, 0, 1, ")")]
        public void HandleDefaultTest(int iState, int iPos, string sAct, int iExpCnt, int iExpState, int iExpPos, string sExpCode)
        {
            // Arrange
            Assert.IsTrue(testHandler.TryGetValue(iState, out var handler));
            var data = new TokenizeData() { State = iState, Pos = iPos, Pos2 = 0, flag=sExpCode=="if" };
            int iCnt = 0;
            List<TokenData> lData = new();

            // Act
            handler((d) => { iCnt++; lData.Add(d); }, sAct, data);

            // Assert
            Assert.AreEqual(iExpCnt, iCnt, nameof(iCnt));
            Assert.AreEqual(iExpState, data.State, nameof(data.State));
            Assert.AreEqual(iExpPos, data.Pos, nameof(data.Pos));
            Assert.AreEqual(iExpCnt, lData.Count, nameof(lData.Count));
            if (lData.Count > 0)
            {
                if (sExpCode.EndsWith(':'))
                    Assert.AreEqual(CodeBlockType.Label, lData[0].type);
                else if (sExpCode.StartsWith("goto"))
                    Assert.AreEqual(CodeBlockType.Goto, lData[0].type);
                else
                    Assert.AreEqual(CodeBlockType.Operation, lData[0].type);

                Assert.AreEqual(0, lData[0].Level);
                Assert.AreEqual(0, lData[0].Pos);
                Assert.AreEqual(sExpCode, lData[0].Code);
            }
        }

        [TestMethod()]
        [DataRow(3, 1, "0 ", 0, 3, 1, "")]
        [DataRow(3, 1, " */", 1, 0, 2, "*/")] //?
        [DataRow(3, 0, "*/ ", 1, 0, 1, "*/")]
        [DataRow(3, 0, "**/ ", 0, 3, 0, "")]
        [DataRow(3, 1, "*/ ", 0, 3, 1, "")]
        public void HandleBlockCommentsTest(int iState, int iPos, string sAct, int iExpCnt, int iExpState, int iExpPos, string sExpCode)
        {
            // Arrange
            Assert.IsTrue(testHandler.TryGetValue(iState, out var handler));
            var data = new TokenizeData() { State = iState, Pos = iPos, Pos2 = 0 };
            int iCnt = 0;
            List<TokenData> lData = new();

            // Act
            handler((d) => { iCnt++; lData.Add(d); }, sAct, data);

            // Assert
            Assert.AreEqual(iExpCnt, iCnt, nameof(iCnt));
            Assert.AreEqual(iExpState, data.State, nameof(data.State));
            Assert.AreEqual(iExpPos, data.Pos, nameof(data.Pos));
            Assert.AreEqual(iExpCnt, lData.Count, nameof(lData.Count));
            if (lData.Count > 0)
            {
                Assert.AreEqual(CodeBlockType.Comment, lData[0].type);
                Assert.AreEqual(0, lData[0].Level);
                Assert.AreEqual(0, lData[0].Pos);
                Assert.AreEqual(sExpCode, lData[0].Code);
            }
        }
        [TestMethod()]
        [DataRow(2, 1, "0 ", 1, 0, 1, "0")]
        [DataRow(2, 1, "0 3", 0, 2, 1, "")]
        [DataRow(2, 1, "x\r\n", 1, 0, 1, "x")] //?
        [DataRow(2, 0, "\r ", 1, 0, 0, "")]
        public void HandleLineCommentsTest(int iState, int iPos, string sAct, int iExpCnt, int iExpState, int iExpPos, string sExpCode)
        {
            // Arrange
            Assert.IsTrue(testHandler.TryGetValue(iState, out var handler));
            var data = new TokenizeData() { State = iState, Pos = iPos, Pos2 = 0 };
            int iCnt = 0;
            List<TokenData> lData = new();

            // Act
            handler((d) => { iCnt++; lData.Add(d); }, sAct, data);

            // Assert
            Assert.AreEqual(iExpCnt, iCnt, nameof(iCnt));
            Assert.AreEqual(iExpState, data.State, nameof(data.State));
            Assert.AreEqual(iExpPos, data.Pos, nameof(data.Pos));
            Assert.AreEqual(iExpCnt, lData.Count, nameof(lData.Count));
            if (lData.Count > 0)
            {
                Assert.AreEqual(CodeBlockType.LComment, lData[0].type);
                Assert.AreEqual(0, lData[0].Level);
                Assert.AreEqual(0, lData[0].Pos);
                Assert.AreEqual(sExpCode, lData[0].Code);
            }
        }
        [TestMethod()]
        [DataRow(1, 1, "0 ", 0, 1, 1, "0")]
        [DataRow(1, 1, "0 3", 0, 1, 1, "")]
        [DataRow(1, 0, "\r ", 1, 0, 0, "")]
        [DataRow(1, 1, "x\r\n", 1, 0, 1, "x")] //?
        [DataRow(1, 1, "x\"", 1, 0, 1, "x\"")]
        [DataRow(1, 1, "x\\\"1", 0, 1, 2, "")] //?
        [DataRow(4, 0, "{3", 0, 6, 0, "")]
        [DataRow(4, 0, "{{", 0, 4, 1, "")]
        [DataRow(5, 0, "\r ", 0, 5, 0, "")]
        [DataRow(5, 0, "{3", 0, 5, 0, "")]
        [DataRow(5, 1, "x\"", 1, 0, 1, "x\"")]
        [DataRow(5, 1, "x\"\"", 0, 5, 2, "")]
        [DataRow(5, 1, "x\\\"1", 0, 5, 1, "")] //?
        public void HandleStringsTest(int iState, int iPos, string sAct, int iExpCnt, int iExpState, int iExpPos, string sExpCode)
        {
            // Arrange
            Assert.IsTrue(testHandler.TryGetValue(iState, out var handler));
            var data = new TokenizeData() { State = iState, Pos = iPos, Pos2 = 0 };
            int iCnt = 0;
            List<TokenData> lData = new();

            // Act
            handler((d) => { iCnt++; lData.Add(d); }, sAct, data);

            // Assert
            Assert.AreEqual(iExpCnt, iCnt, nameof(iCnt));
            Assert.AreEqual(iExpState, data.State, nameof(data.State));
            Assert.AreEqual(iExpPos, data.Pos, nameof(data.Pos));
            Assert.AreEqual(iExpCnt, lData.Count, nameof(lData.Count));
            if (lData.Count > 0)
            {
                Assert.AreEqual(CodeBlockType.String, lData[0].type);
                Assert.AreEqual(0, lData[0].Level);
                Assert.AreEqual(0, lData[0].Pos);
                Assert.AreEqual(sExpCode, lData[0].Code);
            }
        }

        [TestMethod()]
        [DataRow(0, 0, "Test", 0, 0, 0)]
        [DataRow(1, 0, "Test", 0, 1, 0)]
        [DataRow(1, 0, "};", 0, 4, 0)]
        public void AnonymousHandlerTest(int iState, int iPos, string sAct, int iExpCnt, int iExpState, int iExpPos)
        {
            // Arrange
            Assert.IsTrue(testHandler.TryGetValue(6, out var handler));
            var data = new TokenizeData() { State = iState, Pos = iPos };
            int iCnt = 0;

            // Act
            handler((d) => iCnt++, sAct, data);

            // Assert
            Assert.AreEqual(iExpCnt, iCnt);
            Assert.AreEqual(iExpState, data.State);
            Assert.AreEqual(iExpPos, data.Pos);
        }
    }
}