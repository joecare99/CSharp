// ***********************************************************************
// Assembly         : MVVM_BaseLibTests
// Author           : Mir
// Created          : 08-23-2022
//
// Last Modified By : Mir
// Last Modified On : 08-24-2022
// ***********************************************************************
// <copyright file="PropertyTests.cs" company="MVVM_BaseLibTests">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace BaseLib.Helper.Tests
{
    /// <summary>
    /// Struct TestStruct
    /// </summary>
    public struct TestStruct
    {
        /// <summary>
        /// The test int
        /// </summary>
        public int TestInt;
        /// <summary>
        /// The test string
        /// </summary>
        public string TestString;
        /// <summary>
        /// Parses the specified s.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>TestStruct.</returns>
        public static TestStruct Parse(string s)
        {
            var sp = s.Split(';');
            return new TestStruct() { TestInt = int.Parse(sp[0]), TestString = sp[1] };
        }
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return $"({TestInt};{TestString})";
        }
        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">Das Objekt, das mit der aktuellen Instanz verglichen werden soll.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is TestStruct t)
                return TestInt == t.TestInt && TestString == t.TestString;
            return false;
        }

        public static bool operator ==(TestStruct left, TestStruct right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TestStruct left, TestStruct right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return TestInt.GetHashCode() ^ TestString.GetHashCode();
        }
    }

    [TestClass]
    public class TestStructTests
    {
        [TestMethod]
        public void TestStructTest()
        {
            var ts = new TestStruct() { TestInt = 123, TestString = "Dada" };
            Assert.AreEqual("(123;Dada)", ts.ToString());
            Assert.AreEqual(ts, TestStruct.Parse("123;Dada"));
        }

        [TestMethod]
        public void TestEqOpTest2()
        {
            var ts = new TestStruct() { TestInt = 123, TestString = "Dada" };
            var ts2 = new TestStruct() { TestInt = 123, TestString = "Dada1" };
            Assert.IsFalse(ts == ts2);
        }

        [TestMethod]
        public void TestnEqualsTest()
        {
            var ts = new TestStruct() { TestInt = 123, TestString = "Dada" };
            Assert.AreNotEqual((object)ts, (object)3);
        }

        [TestMethod]
        public void TestUnEqOpTest2()
        {
            var ts = new TestStruct() { TestInt = 123, TestString = "Dada" };
            var ts2 = new TestStruct() { TestInt = 125, TestString = "Dada" };
            Assert.IsTrue(ts != ts2);
        }

        [TestMethod]
        public void GetHashTest()
        {
            var ts = new TestStruct() { TestInt = 123, TestString = "Dada" };
            var ts2 = new TestStruct() { TestInt = 124, TestString = "Dada" };
            Assert.AreNotEqual(ts.GetHashCode() , ts2.GetHashCode());
        }

    }
    /// <summary>
    /// Enum GameSound
    /// </summary>
    public enum TestEnum
    {
        /// <summary>
        /// The te apple
        /// </summary>
        teApple,
        /// <summary>
        /// The te pear
        /// </summary>
        tePear,
        /// <summary>
        /// The Cherry
        /// </summary>
        teCherry,
        /// <summary>
        /// The Raspberry
        /// </summary>
        teRaspberry
    }

    /// <summary>
    /// Defines test class PropertyTests.
    /// </summary>
    [TestClass]
    public class PropertyHelperTests2
    {

        /// <summary>
        /// The int property
        /// </summary>
        private int intProp;
        /// <summary>
        /// The float property
        /// </summary>
        private float floatProp;
        /// <summary>
        /// The double property
        /// </summary>
        private double doubleProp;
        /// <summary>
        /// The enum property
        /// </summary>
        private TestEnum enumProp;
        /// <summary>
        /// The object property
        /// </summary>
        private object? objProp;
        /// <summary>
        /// The string property
        /// </summary>
        private string strProp="";
        /// <summary>
        /// The structure property
        /// </summary>
        private TestStruct structProp;
        /// <summary>
        /// The data result
        /// </summary>
        private string DataResult="";
        private bool boolProp;

        public static IEnumerable<object[]> PropTestData => new[] 
        {
        new object[]{"object", TypeCode.Object, "123", true, @"c:	SetPropertyTest,	o:,	n:123
"},
        new object[]{"int", TypeCode.Int32, "123", true, @"c:	SetPropertyTest,	o:0,	n:123
"},
        new object[]{"int", TypeCode.Int32, "0", false, @""},

        new object[]{"bool", TypeCode.Boolean, "true", true, @"c:	SetPropertyTest,	o:False,	n:True
"},
        new object[]{"Bool", TypeCode.Boolean, "false", false, @""},
        new object[]{"float", TypeCode.Single, "123", true, @"c:	SetPropertyTest,	o:0,	n:123
"},
        new object[]{"float", TypeCode.Single, "0", false, @""},
        new object[]{"double", TypeCode.Double, "123", true, @"c:	SetPropertyTest,	o:0,	n:123
"},
        new object[]{"double", TypeCode.Double, "0", false, @""},
        new object[]{"string", TypeCode.String, "123", true, @"c:	SetPropertyTest,	o:,	n:123
"},
        new object[]{"string", TypeCode.String, "", false, @""},
        new object[]{"struct", TypeCode.Object, "123;Dada", true, @"c:	SetPropertyTest,	o:(0;),	n:(123;Dada)
"},
        new object[]{"struct", TypeCode.Object, "0;", false, @""},

        new object[]{"enum", TypeCode.Object, "teCherry", true, @"c:	SetPropertyTest,	o:teApple,	n:teCherry
"},
        new object[]{"enum", TypeCode.Object, "teApple", false, @""},
        new object[]{"enum", TypeCode.Object, "ExApple", true, @"c:	SetPropertyTest,	o:,	n:ExApple
"},
        new object[]{"enum", TypeCode.String, "vnNoVal", false, @""},
        new object[]{"enum", TypeCode.String, "vmNoVal", false, @""},
        new object[]{"enum", TypeCode.String, "vxNoVal", true, @"c:	SetPropertyTest,	o:,	n:vxNoVal
"},

        };
        public Exception? DoEx { get; private set; } = null;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestInitialize]
        public void Init()
        {
            intProp = 0;
            floatProp = 0.0f;
            doubleProp = 0d;
            strProp = "";
            structProp = new TestStruct
            {
                TestString = ""
            };
            objProp = null;
            enumProp = new TestEnum();
            boolProp = false;

            DataResult = "";
        }


        /// <summary>
        /// Sets the property test.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="tt">The tt.</param>
        /// <param name="value">The value.</param>
        /// <param name="bExp">if set to <c>true</c> [b exp].</param>
        /// <param name="expResult">The exp result.</param>
        [DataTestMethod()]
        [DynamicData(nameof(PropTestData))]
        public void SetPropertyTest(string Name, TypeCode tt, string value, bool bExp, string expResult)
        {
            switch (tt)
            {
                case TypeCode.Object when value.Contains(";"): // struct
                    Assert.AreEqual(bExp, PropertyHelper.SetProperty(ref structProp, TestStruct.Parse(value), DoAction));
                    Assert.AreEqual(expResult, DataResult,Name);
                    break;
                case TypeCode.Object when value.StartsWith("Ex"): // struct
                    DoEx = new NotSupportedException();
                    Assert.AreEqual(bExp, PropertyHelper.SetProperty(ref strProp, value, DoAction));
                    Assert.AreEqual(expResult, DataResult);
                    break;
                case TypeCode.String when value.StartsWith("vn"): // struct
                    Assert.AreEqual(bExp, PropertyHelper.SetProperty(ref strProp, value, (s) => false, DoAction));
                    Assert.AreEqual(expResult, DataResult);
                    break;
                case TypeCode.String when value.StartsWith("vm"): // struct
                    Assert.AreEqual(bExp, value.SetProperty(ref strProp, (s) => false, DoAction));
                    Assert.AreEqual(expResult, DataResult);
                    break;
                case TypeCode.String when value.StartsWith("vx"): // struct
                    Assert.AreEqual(bExp, value.SetProperty(ref strProp, DoAction));
                    Assert.AreEqual(expResult, DataResult);
                    break;
                case TypeCode.String:
                    Assert.AreEqual(bExp, PropertyHelper.SetProperty(ref strProp, value, DoAction));
                    Assert.AreEqual(expResult, DataResult);
                    break;
                case TypeCode.Object when value.StartsWith("te"):
                    Assert.AreEqual(bExp, PropertyHelper.SetProperty(ref enumProp, (TestEnum)Enum.Parse(typeof(TestEnum), value), DoAction));
                    Assert.AreEqual(expResult, DataResult);
                    break;
                case TypeCode.Double:
                    Assert.AreEqual(bExp, PropertyHelper.SetProperty(ref doubleProp, double.Parse(value), DoAction));
                    Assert.AreEqual(expResult, DataResult);
                    break;
                case TypeCode.Single:
                    Assert.AreEqual(bExp, PropertyHelper.SetProperty(ref floatProp, float.Parse(value), DoAction));
                    Assert.AreEqual(expResult, DataResult);
                    break;
                case TypeCode.Int32:
                    Assert.AreEqual(bExp, PropertyHelper.SetProperty(ref intProp, int.Parse(value), DoAction));
                    Assert.AreEqual(expResult, DataResult);
                    break;
                case TypeCode.Boolean:
                    Assert.AreEqual(bExp, PropertyHelper.SetProperty(ref boolProp, bool.Parse(value), DoAction));
                    Assert.AreEqual(expResult, DataResult);
                    break;
                case TypeCode.Object:
                    Assert.AreEqual(bExp, PropertyHelper.SetProperty(ref objProp, (object)value, DoAction));
                    Assert.AreEqual(expResult, DataResult);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Sets the property test.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="tt">The tt.</param>
        /// <param name="value">The value.</param>
        /// <param name="bExp">if set to <c>true</c> [b exp].</param>
        /// <param name="expResult">The exp result.</param>
        [DataTestMethod()]
        [DynamicData(nameof(PropTestData))]
        public void SetPropertyTest2(string Name, TypeCode tt, string value, bool bExp, string expResStub)
        {
            var expResult = expResStub.Replace(nameof(SetPropertyTest), nameof(SetPropertyTest2));
            switch (tt)
            {
                case TypeCode.Object when value.Contains(";"): // struct
                    Assert.AreEqual(bExp, PropertyHelper.SetProperty(TestStruct.Parse(value),(s) => structProp = s, structProp,DoAction));
                    Assert.AreEqual(expResult, DataResult);
                    break;
                case TypeCode.Object when value.StartsWith("Ex"): // struct
                    DoEx = new NotSupportedException();
                    Assert.AreEqual(bExp, PropertyHelper.SetProperty(value,(n)=>strProp=n, strProp, DoAction));
                    Assert.AreEqual(expResult, DataResult);
                    break;
                case TypeCode.String when value.StartsWith("vn"): // struct
                    Assert.AreEqual(bExp, PropertyHelper.SetProperty(value, setter: (n) => strProp = n, strProp,  (s) => false, DoAction));
                    Assert.AreEqual(expResult, DataResult);
                    break;
                case TypeCode.String when value.StartsWith("vm"): // struct
                    Assert.AreEqual(bExp, value.SetProperty((n) => strProp = n, strProp, (s) => false, DoAction));
                    Assert.AreEqual(expResult, DataResult);
                    break;
                case TypeCode.String when value.StartsWith("vx"): // struct
                    Assert.AreEqual(bExp, value.SetProperty((n) => strProp = n, strProp,DoAction));
                    Assert.AreEqual(expResult, DataResult);
                    break;
                case TypeCode.String:
                    Assert.AreEqual(bExp, PropertyHelper.SetProperty(value, (n) => strProp = n, strProp, DoAction));
                    Assert.AreEqual(expResult, DataResult);
                    break;
                case TypeCode.Object when value.StartsWith("te"):
                    Assert.AreEqual(bExp, PropertyHelper.SetProperty((TestEnum)Enum.Parse(typeof(TestEnum),value), (n) => enumProp = n, enumProp,DoAction));
                    Assert.AreEqual(expResult, DataResult);
                    break;
                case TypeCode.Double:
                    Assert.AreEqual(bExp, PropertyHelper.SetProperty(ref doubleProp, double.Parse(value), DoAction));
                    Assert.AreEqual(expResult, DataResult);
                    break;
                case TypeCode.Single:
                    Assert.AreEqual(bExp, PropertyHelper.SetProperty(ref floatProp, float.Parse(value), DoAction));
                    Assert.AreEqual(expResult, DataResult);
                    break;
                case TypeCode.Int32:
                    Assert.AreEqual(bExp, PropertyHelper.SetProperty(ref intProp, int.Parse(value), DoAction));
                    Assert.AreEqual(expResult, DataResult);
                    break;
                case TypeCode.Boolean:
                    Assert.AreEqual(bExp, PropertyHelper.SetProperty(ref boolProp, bool.Parse(value), DoAction));
                    Assert.AreEqual(expResult, DataResult);
                    break;
                case TypeCode.Object:
                    Assert.AreEqual(bExp, PropertyHelper.SetProperty(ref objProp, (object)value, DoAction));
                    Assert.AreEqual(expResult, DataResult);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Does the action.
        /// </summary>
        /// <typeparam name="Point">The type of the point.</typeparam>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        /// <param name="arg3">The arg3.</param>
        private void DoAction<T>(string arg1, T arg2, T arg3)
        {
            DataResult += $"c:\t{arg1},\to:{arg2},\tn:{arg3}\r\n";
            if (DoEx != null) throw DoEx;
        }
    }
}