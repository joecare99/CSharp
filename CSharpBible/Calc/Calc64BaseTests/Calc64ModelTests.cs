using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calc64Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc64Base.Tests
{
    [TestClass()]
    public class Calc64ModelTests
    {
        Calc64Model testModel = null!;
        private string DebugLog = "";

        [TestInitialize]
        public void Init()
        {
            testModel = new Calc64Model();
            testModel.CalcOperationChanged += OnCalcOpChg;
            testModel.CalcOperationError += OnCalcOpErr;
            DebugLog = "";
        }

        private void OnCalcOpErr(object? sender, Exception e)
        {
            throw new NotImplementedException();
        }

        private void OnCalcOpChg(object? sender, (string prop, object? oldVal, object? newVal) e)
        {
            DoLog($"COpChg({sender},{e.prop},{e.oldVal} => {e.newVal})");
        }

        private void DoLog(string v)
        {
            DebugLog+=$"{v}{Environment.NewLine}";
        }

        [TestMethod()]
        public void Calc64ModelTest()
        {
            Assert.IsNotNull(testModel);
            Assert.IsInstanceOfType(testModel,typeof(Calc64));
            Assert.IsInstanceOfType(testModel, typeof(Calc64Model));
            Assert.AreEqual(0L,testModel.Accumulator);
            Assert.AreEqual(0L, testModel.Register);
            Assert.AreEqual(0L, testModel.Memory);
            Assert.AreEqual("", DebugLog);
        }

        [TestMethod()]
        [DataRow(Calc64Model.EOpMode.CalcResult)]
        [DataRow(Calc64Model.EOpMode.MemRetrieve)]
        [DataRow(Calc64Model.EOpMode.BinaryNot)]
        [DataRow(Calc64Model.EOpMode.Power)]
        public void OperationModeTest(Calc64Model.EOpMode eVal)
        {
            Assert.AreEqual(Calc64Model.EOpMode.NoMode, testModel.OperationMode);
            testModel.OperationMode = eVal;
            Assert.AreEqual(eVal, testModel.OperationMode);
            Assert.AreEqual($"COpChg(Calc64Base.Calc64Model,OperationMode,NoMode => {eVal})\r\n", DebugLog);
        }

        [TestMethod()]
        [DataRow(Calc64Model.EOpMode.NoMode, "")]
        [DataRow(Calc64Model.EOpMode.CalcResult,"=")]
        [DataRow(Calc64Model.EOpMode.MemRetrieve,"MR")]
        [DataRow(Calc64Model.EOpMode.BinaryNot,"~")]
        [DataRow(Calc64Model.EOpMode.Power,"^")]
        public void GetShortDescTest(Calc64Model.EOpMode eVal,string sExp)
        {
            Assert.AreEqual(sExp, Calc64Model.GetShortDesc(eVal));
        }

        [DataTestMethod()]
        [DataRow(" 0",new int[] {0},null,0L,new string[] {"" })]
        [DataRow(" 98", new int[] { 9,8 }, null, 98L, new string[] { @"COpChg(Calc64Base.Calc64Model,Accumulator,0 => 9)
COpChg(Calc64Base.Calc64Model,Accumulator,9 => 98)
" })]
        [DataRow(" 123", new int[] { 1,2,3 }, null, 123L, new string[] { @"COpChg(Calc64Base.Calc64Model,Accumulator,0 => 1)
COpChg(Calc64Base.Calc64Model,Accumulator,1 => 12)
COpChg(Calc64Base.Calc64Model,Accumulator,12 => 123)
" })]
        [DataRow("0", new int[] { 0 }, 16, 0L, new string[] { "" })]
        [DataRow("0x98", new int[] { 9, 8 }, 16, 0x98L, new string[] { @"COpChg(Calc64Base.Calc64Model,Accumulator,0 => 9)
COpChg(Calc64Base.Calc64Model,Accumulator,9 => 152)
" })]
        [DataRow("0x123", new int[] { 1, 2, 3 }, 16, 0x123L, new string[] { @"COpChg(Calc64Base.Calc64Model,Accumulator,0 => 1)
COpChg(Calc64Base.Calc64Model,Accumulator,1 => 18)
COpChg(Calc64Base.Calc64Model,Accumulator,18 => 291)
" })]
        [DataRow("b0", new int[] { 0 }, 2, 0L, new string[] { "" })]
        [DataRow("b10", new int[] { 1,0 }, 2, 2L, new string[] { @"COpChg(Calc64Base.Calc64Model,Accumulator,0 => 1)
COpChg(Calc64Base.Calc64Model,Accumulator,1 => 2)
" })]
        [DataRow("b110010100101000", new int[] { 1,1,0,0,1,0,1,0,0,1,0,1,0,0 }, 2, 12948L, new string[] { @"COpChg(Calc64Base.Calc64Model,Accumulator,0 => 1)
COpChg(Calc64Base.Calc64Model,Accumulator,1 => 3)
COpChg(Calc64Base.Calc64Model,Accumulator,3 => 6)
COpChg(Calc64Base.Calc64Model,Accumulator,6 => 12)
COpChg(Calc64Base.Calc64Model,Accumulator,12 => 25)
COpChg(Calc64Base.Calc64Model,Accumulator,25 => 50)
COpChg(Calc64Base.Calc64Model,Accumulator,50 => 101)
COpChg(Calc64Base.Calc64Model,Accumulator,101 => 202)
COpChg(Calc64Base.Calc64Model,Accumulator,202 => 404)
COpChg(Calc64Base.Calc64Model,Accumulator,404 => 809)
COpChg(Calc64Base.Calc64Model,Accumulator,809 => 1618)
COpChg(Calc64Base.Calc64Model,Accumulator,1618 => 3237)
COpChg(Calc64Base.Calc64Model,Accumulator,3237 => 6474)
COpChg(Calc64Base.Calc64Model,Accumulator,6474 => 12948)
" })]
        [DataRow("b12345", new int[] { 1, 2,3,4,5 }, 2, 1L, new string[] { "COpChg(Calc64Base.Calc64Model,Accumulator,0 => 1)\r\n" })]
        public void ButtonTest(string name, int[] iVal,int? iBase,long lExp,string[] asExp)
        {
            foreach (int i in iVal)
            {
                if (iBase == null)
                    testModel.Button(i);
                else
                    testModel.Button(i, iBase.Value);
            }
            Assert.AreEqual(lExp,testModel.Accumulator,name);
            Assert.AreEqual(asExp[0],DebugLog);
        }

        [DataTestMethod()]
        [DataRow(1,0,new int[] { -1},0,0,0,new string[] { "COpChg(Calc64Base.Calc64Model,Accumulator,1 => 0)\r\n" })]
        [DataRow(2, 1, new int[] { 2,1 }, 3, 0, 0, new string[] { @"COpChg(Calc64Base.Calc64Model,OperationMode,NoMode => Plus)
COpChg(Calc64Base.Calc64Model,Register,0 => 2)
COpChg(Calc64Base.Calc64Model,Accumulator,2 => 1)
COpChg(Calc64Base.Calc64Model,Accumulator,1 => 3)
COpChg(Calc64Base.Calc64Model,OperationMode,Plus => CalcResult)
COpChg(Calc64Base.Calc64Model,Register,2 => 0)
" })]
        [DataRow(-2, 0, new int[] { 9 }, 1, 0, 0, new string[] { @"COpChg(Calc64Base.Calc64Model,Accumulator,-2 => 1)
" })]
        [DataRow(4, 1, new int[] { 2, -3 }, 0, 0, 0, new string[] { @"COpChg(Calc64Base.Calc64Model,OperationMode,NoMode => Plus)
COpChg(Calc64Base.Calc64Model,Register,0 => 4)
COpChg(Calc64Base.Calc64Model,Accumulator,4 => 1)
COpChg(Calc64Base.Calc64Model,Accumulator,1 => 0)
COpChg(Calc64Base.Calc64Model,Register,4 => 0)
COpChg(Calc64Base.Calc64Model,OperationMode,Plus => NoMode)
" })]
        [DataRow(5, 0, new int[] { -2 }, 5, 0, 0, new string[] { @"" })]
        [DataRow(6, 1, new int[] { 3, 3 }, 5, 5, 0, new string[] { @"COpChg(Calc64Base.Calc64Model,OperationMode,NoMode => Minus)
COpChg(Calc64Base.Calc64Model,Register,0 => 6)
COpChg(Calc64Base.Calc64Model,Accumulator,6 => 1)
COpChg(Calc64Base.Calc64Model,Accumulator,1 => 5)
COpChg(Calc64Base.Calc64Model,Register,6 => 5)
" })]
        [DataRow(7, 1, new int[] { 4, 9,1 }, -14, 0, 0, new string[] { @"COpChg(Calc64Base.Calc64Model,OperationMode,NoMode => Multiply)
COpChg(Calc64Base.Calc64Model,Register,0 => 7)
COpChg(Calc64Base.Calc64Model,Accumulator,7 => 1)
COpChg(Calc64Base.Calc64Model,Accumulator,1 => -2)
COpChg(Calc64Base.Calc64Model,Accumulator,-2 => -14)
COpChg(Calc64Base.Calc64Model,OperationMode,Multiply => CalcResult)
COpChg(Calc64Base.Calc64Model,Register,7 => 0)
" })]
        public void OperationTest(long lVar1,long lVar2, int[] iOp,long lXAkk,long lXReg, long lXMem, string[] asExp)
        {
            testModel.Accumulator = lVar1;
            bool xFirst = lVar2!=0;
            DebugLog = "";
            foreach (int i in iOp)
            { 
                testModel.Operation(i);
                if (xFirst) testModel.Accumulator = lVar2;
                xFirst = false;
            }
            Assert.AreEqual(lXAkk,testModel.Accumulator);
            Assert.AreEqual(lXReg, testModel.Register);
            Assert.AreEqual(lXMem, testModel.Memory);
            Assert.AreEqual(asExp[0],DebugLog);
        }

        [TestMethod()]
        public void BackSpaceTest()
        {
            testModel.Accumulator = 12345L;
            testModel.BackSpace();
            testModel.BackSpace();
            Assert.AreEqual(0L, testModel.Accumulator);
        }
    }
}