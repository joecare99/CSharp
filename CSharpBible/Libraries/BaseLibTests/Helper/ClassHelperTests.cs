using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace BaseLib.Helper.Tests
{
    public enum PropEnum
    {
        boolProp,
        doubleProp,
        enumProp,
        floatProp,
        intProp,
        longProp,
        objectProp,
        shortProp,
        stringProp,
        uintProp,
        ulongProp,
        ushortProp,
    }

    [TestClass()]
    public class ClassHelperTests
    {
        public bool boolProp { get; set; } = false;
        public int intProp { get; set; } = default;
        public uint uintProp { get; set; } = default;
        public float floatProp { get; set; } = default;
        public double doubleProp { get; set; } = default;
        public short shortProp { get; set; } = default;
        public ushort ushortProp { get; set; } = default;
        public long longProp { get; set; } = default;
        public ulong ulongProp { get; set; } = default;
        public object objectProp { get; set; } = new();
        public string stringProp { get; set; } = "";
        public PropEnum enumProp { get; set; } = default;

        private object this[string ix] { get => GetValue(ix); set => SetVal(ix, value); }
        private object this[PropEnum ix]
        {
            get => GetValue(ix.ToString());
            set => SetVal(ix.ToString(), value);
        }

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
            nameof(stringProp) => stringProp = (string)value,
            nameof(enumProp) => enumProp = (PropEnum)value,
            _ => throw new NotImplementedException(),
        };

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
            nameof(stringProp) => stringProp,
            nameof(enumProp) => enumProp,
            _ => throw new NotImplementedException(),
        };

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
        };
    

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
        }

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

        [DataTestMethod()]
        [DynamicData(nameof(PropTestData))]
        public void GetPropTest1(string Prop, object oVal, object oVal2)
        {
            SetVal(Prop, oVal);
            Assert.AreEqual(oVal, this.GetProp(Prop,oVal2));
            Assert.AreNotEqual(oVal2, this.GetProp(Prop,oVal2));
            SetVal(Prop, oVal2);
            Assert.AreEqual(oVal2, this.GetProp(Prop,oVal));
            Assert.AreNotEqual(oVal, this.GetProp(Prop,oVal));
            SetVal(Prop, oVal);
            Assert.AreEqual(oVal2, this.GetProp(Prop+"_", oVal2));
            Assert.AreNotEqual(oVal, this.GetProp(Prop + "_", oVal2));
        }

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
            this.SetProp(Prop+"_", oVal);
            Assert.AreEqual(oVal2, GetValue(Prop));
        }

        [TestMethod()]
        public void SetPropTest2()
        {
            this.SetProp(nameof(stringProp), (string)null!);
            Assert.AreEqual(null, stringProp);
            this.SetProp(nameof(objectProp), "Hallo");
            Assert.AreEqual(null, stringProp);
        }

        [TestMethod()]
        public void GetPropTest2()
        {
            Assert.ThrowsException<ArgumentNullException>(()=> this.GetProp<object,object>(nameof(objectProp), null!));
        }

        [DataTestMethod()]
        [DataRow(5,new object[] {1,3,5,7,9 },2)]
        [DataRow(1, new object[] { 1, 3, 5, 7, 9 }, 0)]
        [DataRow(20, new object[] { 1, 3, 5, 7, 9 }, -1)]
        [DataRow("World", new string[] { "Hello", "beautiful", "new", "World", "!" }, 3)]
        [DataRow("Hello", new string[] { "Hello", "beautiful", "new", "World", "!" }, 0)]
        [DataRow("Bumlux", new string[] { "Hello", "beautiful", "new", "World", "!" }, -1)]
        public void IndexOfTest(object item, object[] arr, int iExp)
        {
            Assert.AreEqual(iExp,arr.IndexOf(item));
        }

        [DataTestMethod()]
        [DataRow(nameof(intProp),PropEnum.intProp)]
        [DataRow(nameof(boolProp), PropEnum.boolProp)]
        [DataRow(nameof(enumProp), PropEnum.enumProp)]
        [DataRow(nameof(objectProp), PropEnum.objectProp)]
        [DataRow(nameof(stringProp), PropEnum.stringProp)]
        [DataRow("BlaBla", null)]
        public void EnumMemberTest(string name,PropEnum? peExp)
        {
            Assert.AreEqual(peExp,typeof(PropEnum).EnumMember(name));
        }

        [TestMethod()]
        public void EnumMemberTest2()
        {
            Assert.ThrowsException<ArgumentException>(()=> typeof(string).EnumMember("Hallo"));
        }

    }
}