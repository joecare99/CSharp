// ***********************************************************************
// Assembly         : BaseLib_netTests
// Author           : Mir
// Created          : 03-27-2023
//
// Last Modified By : Mir
// Last Modified On : 03-27-2023
// ***********************************************************************
// <copyright file="ClassHelperTests.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

/// <summary>
/// The Tests namespace.
/// </summary>
namespace BaseLib.Helper.Tests
{
    /// <summary>
    /// Enum PropEnum
    /// </summary>
    public enum PropEnum
    {
        /// <summary>
        /// The bool property
        /// </summary>
        boolProp,
        /// <summary>
        /// The double property
        /// </summary>
        doubleProp,
        /// <summary>
        /// The enum property
        /// </summary>
        enumProp,
        /// <summary>
        /// The float property
        /// </summary>
        floatProp,
        /// <summary>
        /// The int property
        /// </summary>
        intProp,
        /// <summary>
        /// The long property
        /// </summary>
        longProp,
        /// <summary>
        /// The object property
        /// </summary>
        objectProp,
        /// <summary>
        /// The short property
        /// </summary>
        shortProp,
        /// <summary>
        /// The string property
        /// </summary>
        stringProp,
        /// <summary>
        /// The unsigned integer property
        /// </summary>
        uintProp,
        /// <summary>
        /// The unsigned long property
        /// </summary>
        ulongProp,
        /// <summary>
        /// The unsigned short property
        /// </summary>
        ushortProp,
    }

    /// <summary>
    /// Defines test class ClassHelperTests.
    /// </summary>
    [TestClass()]
    public class ClassHelperTests
    {
        #region Properties
        #region TestData
        /// <summary>
        /// Gets the property test data.
        /// </summary>
        /// <value>The property test data.</value>
        static IEnumerable<object[]> PropTestData => new[] {
        new object[]{nameof(boolProp), true ,false},
        new object[]{nameof(boolProp), false, true},
        new object[]{nameof(intProp), 0,1},
        new object[]{nameof(intProp), 1,2},
        new object[]{nameof(intProp), -1,0},
        new object[]{nameof(intProp), int.MaxValue,int.MaxValue-1},
        new object[]{nameof(intProp), int.MinValue,int.MinValue+1},
        new object[]{nameof(uintProp), 0u,1u},
        new object[]{nameof(uintProp), 1u,2u},
        new object[]{nameof(uintProp), uint.MaxValue,uint.MaxValue-1u},
        new object[]{nameof(uintProp), uint.MinValue, uint.MinValue+1u },
        new object[]{nameof(floatProp), 0f,float.Epsilon},
        new object[]{nameof(floatProp), 1f,2f},
        new object[]{nameof(floatProp), float.MaxValue,float.MaxValue*0.99999f},
        new object[]{nameof(floatProp), float.MinValue, float.MinValue*0.99999f},
        new object[]{nameof(floatProp), float.PositiveInfinity,float.MaxValue},
        new object[]{nameof(floatProp), float.NegativeInfinity, float.MinValue},
        new object[]{nameof(floatProp), float.NaN, float.Epsilon*2f},
        new object[]{nameof(doubleProp), 0d, double.Epsilon},
        new object[]{nameof(doubleProp), 1d,2d},
        new object[]{nameof(doubleProp), double.MaxValue, double.MaxValue*0.99999d},
        new object[]{nameof(doubleProp), double.MinValue, double.MinValue*0.99999d},
        new object[]{nameof(doubleProp), double.PositiveInfinity, double.MaxValue},
        new object[]{nameof(doubleProp), double.NegativeInfinity, double.MinValue},
        new object[]{nameof(doubleProp), double.NaN, double.Epsilon*2d},
        new object[]{nameof(shortProp), (short)0, (short)1 },
        new object[]{nameof(shortProp), (short)1, (short)2 },
        new object[]{nameof(shortProp), (short)-1, (short)0 },
        new object[]{nameof(shortProp), short.MaxValue, (short)(short.MaxValue-1) },
        new object[]{nameof(shortProp), short.MinValue, (short)(short.MinValue+ 1) },
        new object[]{nameof(ushortProp), (ushort)0u, (ushort)1u },
        new object[]{nameof(ushortProp), (ushort)1u, (ushort)2u },
        new object[]{nameof(ushortProp), ushort.MaxValue, (ushort)(ushort.MaxValue- 1u) },
        new object[]{nameof(ushortProp), ushort.MinValue, (ushort)(ushort.MinValue+ 1u) },
        new object[]{nameof(longProp), (long)0, (long)1 },
        new object[]{nameof(longProp), (long)1, (long)2 },
        new object[]{nameof(longProp), (long)-1, (long)0 },
        new object[]{nameof(longProp), long.MaxValue, long.MaxValue-1 },
        new object[]{nameof(longProp), long.MinValue, long.MinValue+ 1 },
        new object[]{nameof(ulongProp), (ulong)0u, (ulong)1u },
        new object[]{nameof(ulongProp), (ulong)1u, (ulong)2u },
        new object[]{nameof(ulongProp), ulong.MaxValue, ulong.MaxValue- 1u },
        new object[]{nameof(ulongProp), ulong.MinValue, ulong.MinValue+ 1u },
        new object[]{nameof(enumProp), PropEnum.boolProp, PropEnum.doubleProp },
        new object[]{nameof(enumProp), PropEnum.intProp, PropEnum.uintProp },
        new object[]{nameof(enumProp), PropEnum.longProp, PropEnum.ulongProp },
        new object[]{nameof(enumProp), PropEnum.objectProp, PropEnum.enumProp },
        new object[]{nameof(stringProp), "","Hello"},
        new object[]{nameof(stringProp), "beautiful","new"},
        new object[]{nameof(stringProp), "and","wonderful"},
        new object[]{nameof(stringProp), "World","here" },
        new object[]{nameof(stringProp), "and","now !"},
        new object[]{nameof(objectProp), 0,0u},
        new object[]{nameof(objectProp), 1f,1d},
        new object[]{nameof(objectProp), (byte)2,2},
        new object[]{nameof(objectProp), float.PositiveInfinity,double.PositiveInfinity},
        new object[]{nameof(objectProp), float.Epsilon, double.Epsilon },
       };
        #endregion
        /// <summary>
        /// The bool prop1
        /// </summary>
        /// <autogeneratedoc />
        public bool boolProp1 = false;
        /// <summary>
        /// The int prop1
        /// </summary>
        /// <autogeneratedoc />
        public int intProp1 = default;
        /// <summary>
        /// The uint prop1
        /// </summary>
        /// <autogeneratedoc />
        public uint uintProp1 = default;
        /// <summary>
        /// The float prop1
        /// </summary>
        /// <autogeneratedoc />
        public float floatProp1 = default;
        /// <summary>
        /// The double prop1
        /// </summary>
        /// <autogeneratedoc />
        public double doubleProp1 = default;
        /// <summary>
        /// The short prop1
        /// </summary>
        /// <autogeneratedoc />
        public short shortProp1 = default;
        /// <summary>
        /// The ushort prop1
        /// </summary>
        /// <autogeneratedoc />
        public ushort ushortProp1 = default;
        /// <summary>
        /// The long prop1
        /// </summary>
        /// <autogeneratedoc />
        public long longProp1 = default;
        /// <summary>
        /// The ulong prop1
        /// </summary>
        /// <autogeneratedoc />
        public ulong ulongProp1 = default;
        /// <summary>
        /// The object prop1
        /// </summary>
        /// <autogeneratedoc />
        public object objectProp1 = new();
        /// <summary>
        /// The string prop1
        /// </summary>
        /// <autogeneratedoc />
        public string stringProp1 = "";
        /// <summary>
        /// The enum prop1
        /// </summary>
        /// <autogeneratedoc />
        public PropEnum enumProp1 = default;

        /// <summary>
        /// Gets or sets a value indicating whether [bool property].
        /// </summary>
        /// <value><c>true</c> if [bool property]; otherwise, <c>false</c>.</value>
        public bool boolProp { get => boolProp1; set => boolProp1 = value; }
        /// <summary>
        /// Gets or sets the int property.
        /// </summary>
        /// <value>The int property.</value>
        public int intProp { get => intProp1; set => intProp1 = value; }
        /// <summary>
        /// Gets or sets the uint property.
        /// </summary>
        /// <value>The uint property.</value>
        public uint uintProp { get => uintProp1; set => uintProp1 = value; }
        /// <summary>
        /// Gets or sets the float property.
        /// </summary>
        /// <value>The float property.</value>
        public float floatProp { get => floatProp1; set => floatProp1 = value; }
        /// <summary>
        /// Gets or sets the double property.
        /// </summary>
        /// <value>The double property.</value>
        public double doubleProp { get => doubleProp1; set => doubleProp1 = value; }
        /// <summary>
        /// Gets or sets the short property.
        /// </summary>
        /// <value>The short property.</value>
        public short shortProp { get => shortProp1; set => shortProp1 = value; }
        /// <summary>
        /// Gets or sets the ushort property.
        /// </summary>
        /// <value>The ushort property.</value>
        public ushort ushortProp { get => ushortProp1; set => ushortProp1 = value; }
        /// <summary>
        /// Gets or sets the long property.
        /// </summary>
        /// <value>The long property.</value>
        public long longProp { get => longProp1; set => longProp1 = value; }
        /// <summary>
        /// Gets or sets the ulong property.
        /// </summary>
        /// <value>The ulong property.</value>
        public ulong ulongProp { get => ulongProp1; set => ulongProp1 = value; }
        /// <summary>
        /// Gets or sets the object property.
        /// </summary>
        /// <value>The object property.</value>
        public object objectProp { get => objectProp1; set => objectProp1 = value; }
        /// <summary>
        /// Gets or sets the string property.
        /// </summary>
        /// <value>The string property.</value>
        public string stringProp { get => stringProp1; set => stringProp1 = value; }
        /// <summary>
        /// Gets or sets the enum property.
        /// </summary>
        /// <value>The enum property.</value>
        public PropEnum enumProp { get => enumProp1; set => enumProp1 = value; }
        /// <summary>
        /// Gets or sets the <see cref="System.Object" /> with the specified ix.
        /// </summary>
        /// <param name="ix">The ix.</param>
        /// <returns>System.Object.</returns>
        private object this[string ix] { get => GetValue(ix); set => SetVal(ix, value); }
        /// <summary>
        /// Gets or sets the <see cref="System.Object" /> with the specified ix.
        /// </summary>
        /// <param name="ix">The ix.</param>
        /// <returns>System.Object.</returns>

        private object this[PropEnum ix]
        {
            get => GetValue(ix.ToString());
            set => SetVal(ix.ToString(), value);
        }
        #endregion

        #region Methods
        #region Helper
        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="prop">The property.</param>
        /// <param name="value">The value.</param>
        private void SetVal(string prop, object value) => _ = prop switch
        {
            nameof(boolProp) => (object)(boolProp = (bool)value),
            nameof(intProp) => intProp = (int)value,
            nameof(uintProp) => uintProp = (uint)value,
            nameof(floatProp) => floatProp = (float)value,
            nameof(doubleProp) => doubleProp = (double)value,
            nameof(shortProp) => shortProp = (short)value,
            nameof(ushortProp) => ushortProp = (ushort)value,
            nameof(longProp) => longProp = (long)value,
            nameof(ulongProp) => ulongProp = (ulong)value,
            nameof(objectProp) => objectProp = value,
            nameof(stringProp) => stringProp = (string)value,
            nameof(enumProp) => enumProp = (PropEnum)value,
            nameof(boolProp1) => boolProp1 = (bool)value,
            nameof(intProp1) => intProp1 = (int)value,
            nameof(uintProp1) => uintProp1 = (uint)value,
            nameof(floatProp1) => floatProp1 = (float)value,
            nameof(doubleProp1) => doubleProp1 = (double)value,
            nameof(shortProp1) => shortProp1 = (short)value,
            nameof(ushortProp1) => ushortProp1 = (ushort)value,
            nameof(longProp1) => longProp1 = (long)value,
            nameof(ulongProp1) => ulongProp1 = (ulong)value,
            nameof(objectProp1) => objectProp1 = value,
            nameof(stringProp1) => stringProp1 = (string)value,
            nameof(enumProp1) => enumProp1 = (PropEnum)value,
            _ => throw new NotImplementedException(),
        };

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="prop">The property.</param>
        /// <returns>System.Object.</returns>
        private object GetValue(string prop) => prop switch
        {
            nameof(boolProp) => boolProp,
            nameof(intProp) => intProp,
            nameof(uintProp) => uintProp,
            nameof(floatProp) => floatProp,
            nameof(doubleProp) => doubleProp,
            nameof(shortProp) => shortProp,
            nameof(ushortProp) => ushortProp,
            nameof(longProp) => longProp,
            nameof(ulongProp) => ulongProp,
            nameof(objectProp) => objectProp,
            nameof(stringProp) => stringProp,
            nameof(enumProp) => enumProp,
            nameof(boolProp1) => boolProp1,
            nameof(intProp1) => intProp1,
            nameof(uintProp1) => uintProp1,
            nameof(floatProp1) => floatProp1,
            nameof(doubleProp1) => doubleProp1,
            nameof(shortProp1) => shortProp1,
            nameof(ushortProp1) => ushortProp1,
            nameof(longProp1) => longProp1,
            nameof(ulongProp1) => ulongProp1,
            nameof(objectProp1) => objectProp1,
            nameof(stringProp1) => stringProp1,
            nameof(enumProp1) => enumProp1,
            _ => throw new NotImplementedException(),
        };

        /// <summary>
        /// Clears this instance.
        /// </summary>
        private void Clear()
        {
            boolProp = default;
            intProp = default;
            uintProp = default;
            floatProp = default;
            doubleProp = default;
            shortProp = default;
            ushortProp = default;
            longProp = default;
            ulongProp = default;
            stringProp = "";
            enumProp = default;
        }
        #endregion



        /// <summary>
        /// Sets up test.
        /// </summary>
        /// <param name="Prop">The property.</param>
        /// <param name="oVal">The o value.</param>
        /// <param name="oVal2">The o val2.</param>
        [DataTestMethod()]
        [DynamicData(nameof(PropTestData))]
        public void SetUpTest(string Prop, object oVal, object oVal2)
        {
            SetVal(Prop, oVal);
            Assert.AreEqual(oVal, GetValue(Prop));
            Assert.AreNotEqual(oVal2, GetValue(Prop));
            Clear();
            SetVal(Prop, oVal2);
            Assert.AreEqual(oVal2, GetValue(Prop));
            Assert.AreNotEqual(oVal, GetValue(Prop));
            this[Prop] = oVal;
            Assert.AreEqual(oVal, this[Prop]);
            var Prop2 = Prop+"1";
            SetVal(Prop2, oVal);
            Assert.AreEqual(oVal, GetValue(Prop2));
            Assert.AreNotEqual(oVal2, GetValue(Prop2));
            Clear();
            SetVal(Prop2, oVal2);
            Assert.AreEqual(oVal2, GetValue(Prop2));
            Assert.AreNotEqual(oVal, GetValue(Prop2));
            this[Prop2] = oVal;
            Assert.AreEqual(oVal, this[Prop2]);
            Assert.AreNotEqual(oVal2, this[Prop2]);
        }

        [DataTestMethod]
        [DataRow(nameof(boolProp),true)]
        [DataRow(null,false)]
        [DataRow("NoProp",false)]
        public void IsPropertyTest(string Prop,bool xExp)
        {
            Assert.AreEqual(xExp,this.IsProperty(Prop));
        }

        /// <summary>
        /// Gets the property test.
        /// </summary>
        /// <param name="Prop">The property.</param>
        /// <param name="oVal">The o value.</param>
        /// <param name="oVal2">The o val2.</param>
        [DataTestMethod()]
        [DynamicData(nameof(PropTestData))]
        public void GetPropTest(string Prop, object oVal, object oVal2)
        {
            SetVal(Prop, oVal);
            Assert.AreEqual(oVal, this.GetProp(Prop));
            Assert.AreNotEqual(oVal2, this.GetProp(Prop));
            SetVal(Prop, oVal);
            Assert.AreEqual(oVal, this.GetProp(Prop));
            Assert.AreNotEqual(oVal2, this.GetProp(Prop));
            Assert.AreEqual(null, this.GetProp(Prop + "_"));
        }

        /// <summary>
        /// Gets the property test.
        /// </summary>
        /// <param name="Prop">The property.</param>
        /// <param name="oVal">The o value.</param>
        /// <param name="oVal2">The o val2.</param>
        [DataTestMethod()]
        [DynamicData(nameof(PropTestData))]
        public void GetFieldTest(string Prop, object oVal, object oVal2)
        {
            SetVal(Prop, oVal);
            Assert.AreEqual(oVal, this.GetField(Prop+"1"));
            Assert.AreNotEqual(oVal2, this.GetField(Prop + "1"));
            SetVal(Prop, oVal);
            Assert.AreEqual(oVal, this.GetField(Prop + "1"));
            Assert.AreNotEqual(oVal2, this.GetField(Prop + "1"));
            Assert.AreEqual(null, this.GetField(Prop + "2"));
        }

        /// <summary>
        /// Gets the property test1.
        /// </summary>
        /// <param name="Prop">The property.</param>
        /// <param name="oVal">The o value.</param>
        /// <param name="oVal2">The o val2.</param>
        [DataTestMethod()]
        [DynamicData(nameof(PropTestData))]
        public void GetPropTest1(string Prop, object oVal, object oVal2)
        {
            SetVal(Prop, oVal);
            Assert.AreEqual(oVal, this.GetProp(Prop, oVal2));
            Assert.AreNotEqual(oVal2, this.GetProp(Prop, oVal2));
            SetVal(Prop, oVal2);
            Assert.AreEqual(oVal2, this.GetProp(Prop, oVal));
            Assert.AreNotEqual(oVal, this.GetProp(Prop, oVal));
            SetVal(Prop, oVal);
            Assert.AreEqual(oVal2, this.GetProp(Prop + "_", oVal2));
            Assert.AreNotEqual(oVal, this.GetProp(Prop + "_", oVal2));
        }

        /// <summary>
        /// Sets the property test.
        /// </summary>
        /// <param name="Prop">The property.</param>
        /// <param name="oVal">The o value.</param>
        /// <param name="oVal2">The o val2.</param>
        [DataTestMethod()]
        [DynamicData(nameof(PropTestData))]
        public void SetPropTest(string Prop, object oVal, object oVal2)
        {
            this.SetProp(Prop, oVal);
            Assert.AreEqual(oVal, GetValue(Prop));
            Assert.AreNotEqual(oVal2, GetValue(Prop));
            this.SetProp(Prop, oVal2);
            Assert.AreEqual(oVal2, GetValue(Prop));
            Assert.AreNotEqual(oVal, GetValue(Prop));
            this.SetProp(Prop + "_", oVal);
            Assert.AreEqual(oVal2, GetValue(Prop));
        }

        /// <summary>
        /// Defines the test method SetPropTest2.
        /// </summary>
        [TestMethod()]
        public void SetPropTest2()
        {
            this.SetProp(nameof(stringProp), (string)null!);
            Assert.AreEqual(null, stringProp);
            this.SetProp(nameof(objectProp), "Hallo");
            TestHelper.AssertAreEqual(new object(), objectProp,new string[] { });
        }

        /// <summary>
        /// Defines the test method GetPropTest2.
        /// </summary>
        [TestMethod()]
        public void GetPropTest2()
        {
#if NET5_0_OR_GREATER
            Assert.AreEqual(objectProp, this.GetProp<object,object>(nameof(objectProp), null!));
#else
            Assert.ThrowsException<ArgumentNullException>(() => this.GetProp<object, object>(nameof(objectProp), null!));
#endif
        }

        /// <summary>
        /// Indexes the of test.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="arr">The arr.</param>
        /// <param name="iExp">The i exp.</param>
        [DataTestMethod()]
        [DataRow(5, new object[] { 1, 3, 5, 7, 9 }, 2)]
        [DataRow(1, new object[] { 1, 3, 5, 7, 9 }, 0)]
        [DataRow(20, new object[] { 1, 3, 5, 7, 9 }, -1)]
        [DataRow("World", new string[] { "Hello", "beautiful", "new", "World", "!" }, 3)]
        [DataRow("Hello", new string[] { "Hello", "beautiful", "new", "World", "!" }, 0)]
        [DataRow("Bumlux", new string[] { "Hello", "beautiful", "new", "World", "!" }, -1)]
        public void IndexOfTest(object item, object[] arr, int iExp)
        {
            Assert.AreEqual(iExp, arr.IndexOf(item));
        }

        /// <summary>
        /// Enums the member test.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="peExp">The pe exp.</param>
        [DataTestMethod()]
        [DataRow(nameof(intProp), PropEnum.intProp)]
        [DataRow(nameof(boolProp), PropEnum.boolProp)]
        [DataRow(nameof(enumProp), PropEnum.enumProp)]
        [DataRow(nameof(objectProp), PropEnum.objectProp)]
        [DataRow(nameof(stringProp), PropEnum.stringProp)]
        [DataRow("BlaBla", null)]
        public void EnumMemberTest(string name, PropEnum? peExp)
        {
            Assert.AreEqual(peExp, typeof(PropEnum).EnumMember(name));
        }

        /// <summary>
        /// Defines the test method EnumMemberTest2.
        /// </summary>
        [TestMethod()]
        public void EnumMemberTest2()
        {
            Assert.ThrowsException<ArgumentException>(() => typeof(string).EnumMember("Hallo"));
        }
        #endregion
    }
}