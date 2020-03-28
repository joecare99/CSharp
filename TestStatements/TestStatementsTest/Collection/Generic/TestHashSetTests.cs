﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.Collection.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TestStatements.Collection.Generic.Tests
{
    [TestClass()]
    public class TestHashSetTests
    {
        private readonly string cExpected1= "======================================================================\r\n## Show HashSet<T> \r\n"+
            "======================================================================\r\nevenNumbers contains 5 elements: { 0 2 4 6 8 }\r\n"+
            "oddNumbers contains 5 elements: { 1 3 5 7 9 }\r\nnumbers UnionWith oddNumbers...\r\n"+
            "numbers contains 10 elements: { 0 2 4 6 8 1 3 5 7 9 }";

        [TestMethod()]
        public void ShowHashSetTest()
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                TestHashSet.ShowHashSet();

                var result = sw.ToString().Trim();
                Assert.AreEqual(cExpected1, result);
            }
        }
    }
}