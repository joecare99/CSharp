﻿using BaseLib.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenFree.Helper.Tests
{
    [TestClass()]
    public class ObjectHelper2Tests
    {
        ControlArray<string> testArray = new() {
             { 2, "A" },
            { 3, "BB" },
            { 4, "CCC" },
            { 9, null! } };

        [TestMethod()]
        [DataRow(4,"DD")]
        [DataRow(1,"AAA")]
        [DataRow(0,"??")]
        public void SetIndexTest(int iAct, string sAct)
        { 
            testArray.SetIndex(sAct,iAct);
            Assert.AreEqual(sAct,testArray[iAct+1]);

        }

        [TestMethod()]
        [DataRow(1,"A")]
        [DataRow(2,"BB")]
        [DataRow(3,"CCC")]
        [DataRow(-1,"??")]
        [DataRow(-1,null)]
        public void GetIndexTest(int iExp,string sAct)
        {
            Assert.AreEqual(iExp,testArray.GetIndex(sAct));
        }
    }
}