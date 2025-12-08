using BaseLib.Model.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using static BaseLib.Helper.TestHelper;

namespace Basic_Del01_Action.Models.Tests
{
    [TestClass()]
    public class CQuickSortTests : BaseTest
    {
        static IEnumerable<object[]> TestQSortData => new[]
        {
            new object[] {1, new[] { 1 }, new[] { @"Privot[0]= 1: 1
Split at 0: 1
" } },
            new object[] {2, new[] { 1, 1 }, new[] { @"Privot[0]= 1: 1, 1
Split at 0: 1, 1
Privot[0]= 1: 1
Split at 0: 1
" } },
            new object[] {20, new[] { 1, 2 }, new[] { @"Privot[0]= 1: 1, 2
Split at 0: 1, 2
Privot[0]= 2: 2
Split at 0: 2
" } },
            new object[] {21, new[] { 2, 1 }, new[] { @"Privot[0]= 2: 2, 1
SW 0,1: 2, 1
Split at 1: 1, 2
" } },
            new object[] {3, new[] { 2, 2, 1 }, new[] { @"Privot[1]= 2: 2, 2, 1
SW 0,2: 2, 2, 1
Split at 1: 1, 2, 2
Privot[0]= 2: 2
Split at 0: 2
" } },
        new object[] {30, new[] { 2, 1, 2 }, new[] { @"Privot[1]= 1: 2, 1, 2
SW 0,1: 2, 1, 2
Split at 0: 1, 2, 2
Privot[0]= 2: 2, 2
Split at 0: 2, 2
Privot[0]= 2: 2
Split at 0: 2
" } },
        new object[] {31, new[] { 1, 2, 3 }, new[] { @"Privot[1]= 2: 1, 2, 3
Split at 1: 1, 2, 3
Privot[0]= 3: 3
Split at 0: 3
" } },
        new object[] {32, new[] { 3, 2, 1 }, new[] { @"Privot[1]= 2: 3, 2, 1
SW 0,2: 3, 2, 1
Split at 1: 1, 2, 3
Privot[0]= 3: 3
Split at 0: 3
" } },
         new object[] {33, new[] { 2, 2, 1 }, new[] { @"Privot[1]= 2: 2, 2, 1
SW 0,2: 2, 2, 1
Split at 1: 1, 2, 2
Privot[0]= 2: 2
Split at 0: 2
" } },
         new object[] {34, new[] { 2, 1, 2 }, new[] { @"Privot[1]= 1: 2, 1, 2
SW 0,1: 2, 1, 2
Split at 0: 1, 2, 2
Privot[0]= 2: 2, 2
Split at 0: 2, 2
Privot[0]= 2: 2
Split at 0: 2
" } },
            new object[] {4, new[] { 1, 2, 3, 4 }, new[] { @"Privot[1]= 2: 1, 2, 3, 4
Split at 1: 1, 2, 3, 4
Privot[0]= 3: 3, 4
Split at 0: 3, 4
Privot[0]= 4: 4
Split at 0: 4
" } },
        new object[] {40, new[] { 4, 3, 2, 1 }, new[] { @"Privot[1]= 3: 4, 3, 2, 1
SW 0,3: 4, 3, 2, 1
SW 1,2: 1, 3, 2, 4
Split at 2: 1, 2, 3, 4
Privot[0]= 1: 1, 2
Split at 0: 1, 2
Privot[0]= 2: 2
Split at 0: 2
Privot[0]= 4: 4
Split at 0: 4
" } },
        new object[] {41, new[] { 3, 4, 1, 2 }, new[] { @"Privot[1]= 4: 3, 4, 1, 2
SW 1,3: 3, 4, 1, 2
Split at 3: 3, 2, 1, 4
Privot[1]= 2: 3, 2, 1
SW 0,2: 3, 2, 1
Split at 1: 1, 2, 3
Privot[0]= 3: 3
Split at 0: 3
" } },
        new object[] {42, new[] { 3, 4, 2, 1 }, new[] { @"Privot[1]= 4: 3, 4, 2, 1
SW 1,3: 3, 4, 2, 1
Split at 3: 3, 1, 2, 4
Privot[1]= 1: 3, 1, 2
SW 0,1: 3, 1, 2
Split at 0: 1, 3, 2
Privot[0]= 3: 3, 2
SW 0,1: 3, 2
Split at 1: 2, 3
" } },
         new object[] {43, new[] { 3, 2, 2, 1 }, new[] { @"Privot[1]= 2: 3, 2, 2, 1
SW 0,3: 3, 2, 2, 1
Split at 1: 1, 2, 2, 3
Privot[0]= 2: 2, 3
Split at 0: 2, 3
Privot[0]= 3: 3
Split at 0: 3
" } },
         new object[] {44, new[] { 1, 2, 1, 2 }, new[] { @"Privot[1]= 2: 1, 2, 1, 2
SW 1,2: 1, 2, 1, 2
Split at 2: 1, 1, 2, 2
Privot[0]= 1: 1, 1
Split at 0: 1, 1
Privot[0]= 1: 1
Split at 0: 1
Privot[0]= 2: 2
Split at 0: 2
" } },
        new object[] {5, new[] { 1, 2, 3, 4, 5 }, new[] { @"Privot[2]= 3: 1, 2, 3, 4, 5
Split at 2: 1, 2, 3, 4, 5
Privot[0]= 1: 1, 2
Split at 0: 1, 2
Privot[0]= 2: 2
Split at 0: 2
Privot[0]= 4: 4, 5
Split at 0: 4, 5
Privot[0]= 5: 5
Split at 0: 5
" } },
         new object[] {50, new[] { 5, 4, 3, 2, 1 }, new[] { @"Privot[2]= 3: 5, 4, 3, 2, 1
SW 0,4: 5, 4, 3, 2, 1
SW 1,3: 1, 4, 3, 2, 5
Split at 2: 1, 2, 3, 4, 5
Privot[0]= 1: 1, 2
Split at 0: 1, 2
Privot[0]= 2: 2
Split at 0: 2
Privot[0]= 4: 4, 5
Split at 0: 4, 5
Privot[0]= 5: 5
Split at 0: 5
" } },
        new object[] {51, new[] { 1, 3, 2, 5, 4 }, new[] { @"Privot[2]= 2: 1, 3, 2, 5, 4
SW 1,2: 1, 3, 2, 5, 4
Split at 1: 1, 2, 3, 5, 4
Privot[1]= 5: 3, 5, 4
SW 1,2: 3, 5, 4
Split at 2: 3, 4, 5
Privot[0]= 3: 3, 4
Split at 0: 3, 4
Privot[0]= 4: 4
Split at 0: 4
" } },
        new object[] {52, new[] { 1, 2, 3, 5, 4 }, new[] { @"Privot[2]= 3: 1, 2, 3, 5, 4
Split at 2: 1, 2, 3, 5, 4
Privot[0]= 1: 1, 2
Split at 0: 1, 2
Privot[0]= 2: 2
Split at 0: 2
Privot[0]= 5: 5, 4
SW 0,1: 5, 4
Split at 1: 4, 5
" } },
        };

        [DataTestMethod]
        [DynamicData(nameof(TestQSortData))]
        // Convert DataRow to IEnumerable<object[]>
        public void QuickSortTest(int _, int[] data, string[] asExp)
        {
            int[] expected = data.OrderBy(x => x).ToArray();
            ((Span<int>)data).QuickSort(DoLog);
            CollectionAssert.AreEqual(expected, data);
            AssertAreEqual(asExp[0], DebugLog);
        }

        [DataTestMethod]
        [DynamicData(nameof(TestQSortData))]
        public void QuickSortTest2(int _, int[] data, string[] __)
        {
            int[] expected = data.OrderBy(x => x).ToArray();
            ((Span<int>)data).QuickSort();
            CollectionAssert.AreEqual(expected, data);
            Assert.AreEqual("", DebugLog);
        }
    }
}
