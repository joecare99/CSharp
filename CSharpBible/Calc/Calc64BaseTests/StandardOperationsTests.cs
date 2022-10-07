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
    public class StandardOperationsTests
    {
        private enum Exceptions
        {
            None,
            eDivByZeroEx
        }

        [DataTestMethod()]
        [DataRow("+", "1", new Int64[] { 0, 0 },true,new Int64[] {0L,0L })]
        [DataRow("+", "2", new Int64[] { 1L, 1L }, true, new Int64[] { 2L, 1L })]
        [DataRow("+", "3", new Int64[] { 1L, -1L }, true, new Int64[] { 0L, -1L })]
        [DataRow("+", "4", new Int64[] { 1000000000000L, -999999999999L }, true, new Int64[] { 1L, -999999999999L })]
        [DataRow("+", "5", new Int64[] { Int64.MaxValue, 1L }, true, new Int64[] { -9223372036854775808L, 1L })]
        [DataRow("-", "1", new Int64[] { 0L, 0L }, true, new Int64[] { 0L, 0L })]
        [DataRow("-", "2", new Int64[] { 1L, 1L }, true, new Int64[] { 0L, 1L })]
        [DataRow("-", "3", new Int64[] { -1L, 1L }, true, new Int64[] { 2L, 1L })]
        [DataRow("-", "4", new Int64[] { 1000000000000L, 999999999999L }, true, new Int64[] { -1L, 999999999999L })]
        [DataRow("-", "5", new Int64[] { 1L, Int64.MinValue }, true, new Int64[] { 9223372036854775807L, -9223372036854775808L })]
        [DataRow("*", "1", new Int64[] { 0L, 0L }, true, new Int64[] { 0L, 0L })]
        [DataRow("*", "2", new Int64[] { 1L, -2L }, true, new Int64[] { -2L, -2L })]
        [DataRow("*", "3", new Int64[] { -2L, 1L }, true, new Int64[] { -2L, 1L })]
        [DataRow("*", "4", new Int64[] { -3L, -2L }, true, new Int64[] { 6L, -2L })]
        [DataRow("*", "5", new Int64[] { 1000000000L, 999999999L }, true, new Int64[] { 999999999000000000L, 999999999L })]
        [DataRow("*", "6", new Int64[] { Int64.MinValue, -10L }, true, new Int64[] { 0L, -10L })] // ??
        [DataRow("/", "1", new Int64[] { 0L, 0L }, true, new Int64[] { 0L, 0L },(object)Exceptions.eDivByZeroEx)]
        [DataRow("/", "2", new Int64[] { 1L, -2L }, true, new Int64[] { -2L, -2L })]
        [DataRow("/", "3", new Int64[] { -2L, 1L }, true, new Int64[] { 0L, 1L })]
        [DataRow("/", "4", new Int64[] { -2L, -3L }, true, new Int64[] { 1L, -3L })]
        [DataRow("/", "5", new Int64[] {  999999999L, 1000000000L }, true, new Int64[] { 1L, 1000000000L })]
        [DataRow("/", "6", new Int64[] { -10L, Int64.MinValue }, true, new Int64[] { Int64.MaxValue / 10L, Int64.MinValue })] // ??
        [DataRow("^", "1", new Int64[] { 0L, 0L }, true, new Int64[] { 1L, 0L })]
        [DataRow("^", "2", new Int64[] { 1L, -2L }, true, new Int64[] { -2L, -2L })]
        [DataRow("^", "3", new Int64[] { -2L, 1L }, true, new Int64[] { 1L, 1L })]
        [DataRow("^", "4", new Int64[] { 2L, 3L }, true, new Int64[] { 9L, 3L })]
        [DataRow("^", "5", new Int64[] { 3L, 2000000L}, true, new Int64[] { 8000000000000000000L, 2000000L })] //??
        [DataRow("^", "6", new Int64[] { Int64.MaxValue, 1L }, true, new Int64[] { 1L, 1L })] // ??
        [DataRow("±", "1", new Int64[] { 0L }, true, new Int64[] { 0L })]
        [DataRow("±", "2", new Int64[] { 1L }, true, new Int64[] { -1L })]
        [DataRow("±", "3", new Int64[] { -2L }, true, new Int64[] { 2L })]
        [DataRow("±", "4", new Int64[] { long.MaxValue }, true, new Int64[] { -long.MaxValue })]
        [DataRow("±", "5", new Int64[] { long.MinValue }, true, new Int64[] { long.MinValue })] //??
        [DataRow("~", "1", new Int64[] { 0L }, true, new Int64[] { -1L })]
        [DataRow("~", "2", new Int64[] { 1L }, true, new Int64[] { -2L })]
        [DataRow("~", "3", new Int64[] { -2L }, true, new Int64[] { 1L })]
        [DataRow("~", "4", new Int64[] { long.MaxValue }, true, new Int64[] { long.MinValue })]
        [DataRow("~", "5", new Int64[] { long.MinValue }, true, new Int64[] { long.MaxValue })] //??
        [DataRow("&", "1", new Int64[] { 0L, 0L }, true, new Int64[] { 0L, 0L })]
        [DataRow("&", "2", new Int64[] { 1L, -2L }, true, new Int64[] { 0L, -2L })]
        [DataRow("&", "3", new Int64[] { -2L, 1L }, true, new Int64[] { 0L, 1L })]
        [DataRow("&", "4", new Int64[] { -3L, -2L }, true, new Int64[] { -4L, -2L })]
        [DataRow("&", "5", new Int64[] { 1000000000L, 999999999L }, true, new Int64[] { 999999488L, 999999999L })]
        [DataRow("&", "6", new Int64[] { Int64.MinValue, -10L }, true, new Int64[] { Int64.MinValue, -10L })] 
        [DataRow("&", "7", new Int64[] { Int64.MinValue, 10L }, true, new Int64[] { 0, 10L })] 
        [DataRow("|", "1", new Int64[] { 0, 0 }, true, new Int64[] { 0L, 0L })]
        [DataRow("|", "2", new Int64[] { 1L, 1L }, true, new Int64[] { 1L, 1L })]
        [DataRow("|", "3", new Int64[] { 1L, -1L }, true, new Int64[] { -1L, -1L })]
        [DataRow("|", "4", new Int64[] { 1000000000000L, -999999999999L }, true, new Int64[] { -4095L, -999999999999L })]
        [DataRow("|", "5", new Int64[] { Int64.MaxValue, 1L }, true, new Int64[] { Int64.MaxValue, 1L })]
        [DataRow("x", "1", new Int64[] { 0, 0 }, true, new Int64[] { 0L, 0L })]
        [DataRow("x", "2", new Int64[] { 1L, 1L }, true, new Int64[] { 0L, 1L })]
        [DataRow("x", "3", new Int64[] { 1L, -1L }, true, new Int64[] { -2L, -1L })]
        [DataRow("x", "4", new Int64[] { 1000000000000L, -999999999999L }, true, new Int64[] { -8191L, -999999999999L })]
        [DataRow("x", "5", new Int64[] { Int64.MaxValue, 1L }, true, new Int64[] { Int64.MaxValue-1, 1L })]
        [DataRow("&~", "1", new Int64[] { 0L, 0L }, true, new Int64[] { 0L, 0L })]
        [DataRow("&~", "2", new Int64[] { 1L, -2L }, true, new Int64[] { -2L, -2L })]
        [DataRow("&~", "3", new Int64[] { -2L, 1L }, true, new Int64[] { 1L, 1L })]
        [DataRow("&~", "4", new Int64[] { -3L, -2L }, true, new Int64[] { 2L, -2L })]
        [DataRow("&~", "5", new Int64[] { 1000000000L, 999999999L }, true, new Int64[] { 511, 999999999L })]
        [DataRow("&~", "6", new Int64[] { Int64.MinValue, -10L }, true, new Int64[] { Int64.MaxValue-9, -10L })]
        [DataRow("&~", "7", new Int64[] { Int64.MinValue, 10L }, true, new Int64[] { 10, 10L })]
        [DataRow("|~", "1", new Int64[] { 0, 0 }, true, new Int64[] { -1L, 0L })]
        [DataRow("|~", "2", new Int64[] { 1L, 1L }, true, new Int64[] { -1L, 1L })]
        [DataRow("|~", "3", new Int64[] { 1L, -1L }, true, new Int64[] { -1L, -1L })]
        [DataRow("|~", "4", new Int64[] { 1000000000000L, -999999999999L }, true, new Int64[] { -999999995905L, -999999999999L })]
        [DataRow("|~", "5", new Int64[] { Int64.MaxValue, 1L }, true, new Int64[] { Int64.MinValue+1, 1L })]
        [DataRow("x~", "1", new Int64[] { 0, 0 }, true, new Int64[] { -1L, 0L })]
        [DataRow("x~", "2", new Int64[] { 1L, 1L }, true, new Int64[] { -1L, 1L })]
        [DataRow("x~", "3", new Int64[] { 1L, -1L }, true, new Int64[] { 1L, -1L })]
        [DataRow("x~", "4", new Int64[] { 1000000000000L, -999999999999L }, true, new Int64[] { 8190L, -999999999999L })]
        [DataRow("x~", "5", new Int64[] { Int64.MaxValue, 1L }, true, new Int64[] { Int64.MinValue + 1, 1L })]
        public void GetAllTest1(string sOp,string s, Int64[] argL,bool xResult, Int64[] erg, object e = null)
        {
            var co = StandardOperations.GetAll().First((o)=>o.ShortDescr==sOp);
            Assert.IsNotNull(co);
            var arg = new object[argL.Length];
            for (int i = 0; i < arg.Length; i++)
                arg[i] = argL[i];
            if (e==null)
            Assert.AreEqual(xResult, co.Execute(ref arg));
            else
                switch ((Exceptions)e)
                {
                    case Exceptions.eDivByZeroEx : Assert.ThrowsException<DivideByZeroException>(()=> co.Execute(ref arg));break;
                    default: Assert.Fail();break;
                }
            for (int i = 0; i < arg.Length; i++)
                Assert.AreEqual(erg[i], arg[i],$"erg[{i}]({erg[i]}) == arg[{i}]({arg[i]})");
        }
    }
}