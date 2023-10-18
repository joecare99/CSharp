using Microsoft.VisualStudio.TestTools.UnitTesting;
using BaseLib.Helper.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BaseLib.Helper.TestHelper;

namespace BaseLib.Helper.MVVM.Tests
{
    [TestClass()]
    public class CRandomTests
    {
        private CRandom _testClass;

        [TestInitialize]
        public void Init()
        {
            _testClass = new CRandom();
            _testClass.Seed(0);
        }

        [TestMethod()]
        public void CRandomTest()
        {
            Assert.IsNotNull(_testClass);
            Assert.IsInstanceOfType(_testClass, typeof(IRandom));
            Assert.IsInstanceOfType(_testClass, typeof(CRandom));
        }

        [DataTestMethod()]
        [DataRow(0, 1, new[] { 0, 0, 0, 0, 0 })]
        [DataRow(0, 2, new[] { 1, 1, 1, 1, 0 })]

        public void NextTest(int imin, int iMax,int[] asExp)
        {
            var aoResult = new int[asExp.Length];
            for (int i = 0; i < asExp.Length; i++)
            {
                aoResult[i] = _testClass.Next(imin, iMax);
            }
            AssertAreEqual(asExp, aoResult);
        }

        [DataTestMethod()]
        [DataRow(new[] { 0.7262432699679598, 0.8173253595909687, 0.7680226893946634,0.5581611914365372, 0.2060331540210327 })]
        public void NextDoubleTest(double[] adExp)
        {
            var adResult = new double[adExp.Length];
            for (int i = 0; i < adExp.Length; i++)
            {
                adResult[i] = _testClass.NextDouble();
            }
            AssertAreEqual(adExp, adResult);
        }

        [DataTestMethod()]
        [DataRow(new[] { 1559595546, 1755192844, 1649316166, 1198642031, 442452829 })]
        public void NextIntTest(int[] asExp)
        {
            var aoResult = new int[asExp.Length];
            for (int i = 0; i < asExp.Length; i++)
            {
                aoResult[i] = _testClass.NextInt();
            }
            AssertAreEqual(asExp, aoResult);
        }

        [DataTestMethod()]
        [DataRow(0, new[] { 1559595546, 1755192844, 1649316166, 1198642031, 442452829 })]
        [DataRow(1, new[] { 534011718, 237820880, 1002897798, 1657007234, 1412011072 })]
        [DataRow(int.MaxValue, new[] { 1559595546, 1755192844, 1649316172, 1198642031, 442452829 })]

        public void SeedTest(int iSeed, int[] asExp)
        {
            _testClass.Seed(iSeed);
            var aoResult = new int[asExp.Length];
            for (int i = 0; i < asExp.Length; i++)
            {
                aoResult[i] = _testClass.NextInt();
            }       
            AssertAreEqual(asExp, aoResult); 
        }
    }
}