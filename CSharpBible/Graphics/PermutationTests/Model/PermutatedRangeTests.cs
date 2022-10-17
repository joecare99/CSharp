using Microsoft.VisualStudio.TestTools.UnitTesting;
using Permutation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permutation.Model.Tests
{
    [TestClass()]
    public class PermutatedRangeTests
    {
        [DataTestMethod()]
        [DataRow(1, 1,new int[] {0,0,0,0 } )]
        [DataRow(2, 2, new int[] { 0, 1, 0, 1 })]
        [DataRow(4, 4, new int[] { 1, 5, 2, 4 })]
        public void PermutatedRangeTest(int size,int pp, int[] results )
        {
            var pr=new PermutatedRange(size,pp);
            Assert.IsNotNull(pr);
            int[] s=new int[4];
            for (int i=0;i<size/2;i++)
            {
                s[0]+= pr[i];
                s[1] += pr[i+size/2];
                s[2] += pr[i*2];
                s[3] += pr[i*2 + 1];
            }
            Assert.AreEqual(results[0], s[0],$"{size},{pp}\t{0}");
            Assert.AreEqual(results[1], s[1], $"{size},{pp}\t{1}");
            Assert.AreEqual(results[2], s[2], $"{size},{pp}\t{2}");
            Assert.AreEqual(results[3], s[3], $"{size},{pp}\t{3}");
        }

        [TestMethod()]
        public void PermutatedRangeTest1()
        {
            const int size = 32700;
            for (int j = 1000; j < size * 0.9; j+=100)
            {
                var pr = new PermutatedRange(size, j);
                Assert.IsNotNull(pr);
                Int64[] s = new Int64[6];
                for (int i = 0; i < size / 2; i++)
                {
                    s[0] += pr[i];
                    s[1] += pr[i + size / 2];
                    s[2] += pr[i * 2];
                    s[3] += pr[i * 2 + 1];
                    s[4] += pr[i * 2 + i%4/2];
                    s[5] += pr[i * 2 + 1-i%4/2];
                }
                var m = ((Int64)size * (size - 1)) / 4;
                var r=0;
                for(int i = 0; i < 4 ; i++)
                {
                    r += (int)Math.Abs(s[i] - m);
                }
                Console.WriteLine($"{j}:{r}");
            }
        }
    }
}