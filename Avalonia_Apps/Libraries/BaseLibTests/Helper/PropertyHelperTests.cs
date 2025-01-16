using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BaseLib.Helper.Tests;

[TestClass()]
public class PropertyHelperTests
{
    public enum eValidReact
    {
        OK = 0,
        NIO,
        GeneralException,
        ArgumentException,
    }

    static IEnumerable<object[]> TestPropData => new[] {
        new object[]{"00 Empty", 0, "", eValidReact.OK, "", ""},
        new object[]{"01-Test", 0, "Test", eValidReact.OK, "Test", ""},
        new object[] { "20-1 Empty", 2, "", eValidReact.OK, "", "" },
        new object[] { "20-2 Test", 2, "", eValidReact.NIO, "", "" },
        new object[] { "20-2 Test", 2, "", eValidReact.GeneralException, "", "" },
        new object[] { "21-1 Empty", 2, "Test", eValidReact.OK, "Test",new string[] { 
                "Validate: TestString2(Test), React:OK\r\n",
                "Validate: TestString3(Test), React:OK\r\n",
                "Validate: TestString_2(Test), React:OK\r\n",
                "Validate: TestString_3(Test), React:OK\r\n"} },
        new object[] { "21-2 Test", 2, "Test", eValidReact.NIO, "", new string[] {
                "Validate: TestString2(Test), React:NIO\r\n",
                "Validate: TestString3(Test), React:NIO\r\n",
                "Validate: TestString_2(Test), React:NIO\r\n",
                "Validate: TestString_3(Test), React:NIO\r\n"} },
        new object[] { "21-3 GEx", 2, "Test", eValidReact.GeneralException, "", new string[] {
                "Validate: TestString2(Test), React:GeneralException\r\n",
                "Validate: TestString3(Test), React:GeneralException\r\n",
                "Validate: TestString_2(Test), React:GeneralException\r\n",
                "Validate: TestString_3(Test), React:GeneralException\r\n"} },
        new object[] { "21-4 AEx", 2, "Test", eValidReact.ArgumentException, "", new string[] {
                "Validate: TestString2(Test), React:ArgumentException\r\n",
                "Validate: TestString3(Test), React:ArgumentException\r\n",
                "Validate: TestString_2(Test), React:ArgumentException\r\n",
                "Validate: TestString_3(Test), React:ArgumentException\r\n"} },
        new object[] { "30-1 Empty", 3, "", eValidReact.OK, "", "" },
        new object[] { "30-2 Test", 3, "", eValidReact.NIO, "", "" },
        new object[] { "31-1 Test", 3, "Test", eValidReact.OK, "Test", new string[] {
            "StrAct: TestString4; ; Test\r\n","StrAct: TestString5; ; Test\r\n",
            "StrAct: TestString_4; ; Test\r\n","StrAct: TestString_5; ; Test\r\n"}},
        new object[] { "31-2 Test2", 3, "Test2", eValidReact.NIO, "Test2", new string[] {
            "StrAct: TestString4; ; Test2\r\n","StrAct: TestString5; ; Test2\r\n",
            "StrAct: TestString_4; ; Test2\r\n","StrAct: TestString_5; ; Test2\r\n"} },
        new object[] { "31-3 GEx", 3, "Test", eValidReact.GeneralException, "Test", new string[] {
            "StrAct: TestString4; ; Test\r\n","StrAct: TestString5; ; Test\r\n",
            "StrAct: TestString_4; ; Test\r\n","StrAct: TestString_5; ; Test\r\n"} },
        new object[] { "31-4 AEx", 3, "Test", eValidReact.ArgumentException, "Test", new string[] {
            "StrAct: TestString4; ; Test\r\n","StrAct: TestString5; ; Test\r\n",
            "StrAct: TestString_4; ; Test\r\n","StrAct: TestString_5; ; Test\r\n"} },
        new object[] { "40-1 Empty", 4, "", eValidReact.OK, "", "" },
        new object[] { "40-2 Test ", 4, "", eValidReact.NIO, "", "" },
        new object[] { "41-1 Test ", 4, "Test", eValidReact.OK, "Test",  new string[] {
                    "Validate: TestString6(Test), React:OK\r\nStrAct: TestString6; ; Test\r\n",
                    "Validate: TestString7(Test), React:OK\r\nStrAct: TestString7; ; Test\r\n",
                    "Validate: TestString_6(Test), React:OK\r\nStrAct: TestString_6; ; Test\r\n",
                    "Validate: TestString_7(Test), React:OK\r\nStrAct: TestString_7; ; Test\r\n" } },
        new object[] { "41-2 Test2", 4, "Test", eValidReact.NIO, "", new string[] {
                "Validate: TestString6(Test), React:NIO\r\n",
                "Validate: TestString7(Test), React:NIO\r\n",
                "Validate: TestString_6(Test), React:NIO\r\n",
                "Validate: TestString_7(Test), React:NIO\r\n"} },
        new object[] { "41-3 GEx", 4, "Test", eValidReact.GeneralException, "",new string[] {
                "Validate: TestString6(Test), React:GeneralException\r\n",
                "Validate: TestString7(Test), React:GeneralException\r\n",
                "Validate: TestString_6(Test), React:GeneralException\r\n",
                "Validate: TestString_7(Test), React:GeneralException\r\n"} },
        new object[] { "41-4 AEx", 4, "Test", eValidReact.ArgumentException, "", new string[] {
                "Validate: TestString6(Test), React:ArgumentException\r\n",
                "Validate: TestString7(Test), React:ArgumentException\r\n",
                "Validate: TestString_6(Test), React:ArgumentException\r\n",
                "Validate: TestString_7(Test), React:ArgumentException\r\n"}  },
        new object[] { "42-2 Test2", 4, "Test2", eValidReact.OK, "Test2", new string[] {
                    "Validate: TestString6(Test2), React:OK\r\nStrAct: TestString6; ; Test2\r\n",
                    "Validate: TestString7(Test2), React:OK\r\nStrAct: TestString7; ; Test2\r\n",
                    "Validate: TestString_6(Test2), React:OK\r\nStrAct: TestString_6; ; Test2\r\n",
                    "Validate: TestString_7(Test2), React:OK\r\nStrAct: TestString_7; ; Test2\r\n" } },
    };

    private string _testString = "";
    private int _testInt;
    private float _testFloat;
    private double _testDouble;
    private string DebugResult = "";
    private TypeCode _testEnum;
    private eValidReact valReact;



    public string TestString { get => _testString; set => PropertyHelper.SetProperty(ref _testString, value); }
    public string TestString1 { get => _testString; set => value.SetProperty(ref _testString); }
    public string TestString2 { get => _testString; set => PropertyHelper.SetProperty(ref _testString, value, (s) => ValidateString(s)); }
    public string TestString3 { get => _testString; set => value.SetProperty(ref _testString, (s) => ValidateString(s)); }
    public string TestString4 { get => _testString; set => PropertyHelper.SetProperty(ref _testString, value, StringAct); }
    public string TestString5 { get => _testString; set => value.SetProperty(ref _testString, StringAct); }

    public string TestString6
    {
        set
           => PropertyHelper.SetProperty(ref _testString, value, (s) => ValidateString(s), StringAct);
    }

    public string TestString7
    {
        set => value.SetProperty(ref _testString, (s) => ValidateString(s), StringAct);
    }

    public string TestString_0 { get => _testString; set => PropertyHelper.SetProperty(value, (s) => TestString = s, TestString); }
    public string TestString_1 { get => _testString; set => value.SetProperty((s) => TestString = s, TestString); }
    public string TestString_2 { get => _testString; set => PropertyHelper.SetProperty(value, (s) => TestString = s, TestString, (s)=>ValidateString(s)); }
    public string TestString_3 { get => _testString; set => value.SetProperty((s) => TestString = s, TestString, (s) => ValidateString(s)); }
    public string TestString_4 { get => _testString; set => PropertyHelper.SetProperty(value, (s) => TestString = s, TestString, StringAct); }
    public string TestString_5 { get => _testString; set => value.SetProperty((s) => TestString = s, TestString, StringAct); }

    public string TestString_6
    {
        set
           => PropertyHelper.SetProperty(value, (s) => TestString = s, TestString, (s) => ValidateString(s), StringAct);
    }

    public string TestString_7
    {
        set => value.SetProperty((s) => TestString = s, TestString, (s) => ValidateString(s), StringAct);
    }

    public int TestInt { get => _testInt; set => PropertyHelper.SetProperty(ref _testInt, value); }
    public float TestFloat { get => _testFloat; set => PropertyHelper.SetProperty(ref _testFloat, value); }
    public double TestDouble { get => _testDouble; set => PropertyHelper.SetProperty(ref _testDouble, value); }

    public TypeCode TestEnum { get => _testEnum; set => PropertyHelper.SetProperty(ref _testEnum, value); }

    private void StringAct(string arg1, string arg2, string arg3)
    {
        DebugResult += $"StrAct: {arg1}; {arg2}; {arg3}{Environment.NewLine}";
        switch (valReact)
        {
            case eValidReact.OK:
                return;
            case eValidReact.GeneralException:
                throw new Exception("A general exception occurred");
            case eValidReact.ArgumentException:
                throw new ArgumentException($"Argument ({arg1}) not valid!");
            default:
                return;

        }
    }

    private bool ValidateString(string arg1 = "",[CallerMemberName] string propertyName = "")
    {
        DebugResult += $"Validate: {propertyName}({arg1}), React:{valReact}{Environment.NewLine}";
        return valReact switch
        {
            eValidReact.OK => true,
            eValidReact.GeneralException => throw new Exception("A general exception occurred"),
            eValidReact.ArgumentException => throw new ArgumentException($"Argument ({arg1}) not valid!"),
            _ => false,
        };
    }
    public void SetPropertyTest(string name, int iTs, string sVal, eValidReact eReact, string sExp, string sDebExp, Func<string> Setter)
    {
        valReact = eReact;
        bool xCh = sVal != _testString;
        bool eRIsEx = xCh 
            && eReact is not (eValidReact.OK or eValidReact.NIO) 
            && iTs is 2 or 4;
        switch (eReact)
        {
            case eValidReact.GeneralException when eRIsEx: Assert.ThrowsException<Exception>(Setter, $"{name}.T2"); break;
            case eValidReact.ArgumentException when eRIsEx: Assert.ThrowsException<ArgumentException>(Setter, $"{name}.T2"); break;
            default: Setter(); break;
        }
        Assert.AreEqual(sExp, _testString, $"{name}.Result");
        Assert.AreEqual(sDebExp, DebugResult, $"{name}.DebRes");
    }

    [DataTestMethod()]
    [TestProperty("Author", "J.C.")]
    [TestCategory("SetData")]
    [DynamicData(nameof(TestPropData))]
    public void SetPropertyTest1(string name, int iTs, string sVal, eValidReact eReact, string sExp, object sDebExp)
        => SetPropertyTest(name, iTs, sVal, eReact, sExp, sDebExp switch { string s => s, string[] a => a[0], _ => "" }, () => iTs switch {
        2 => TestString2 = sVal,
        3 => TestString4 = sVal,
        4 => TestString6 = sVal,
        _ => TestString = sVal});

    [DataTestMethod()]
    [TestProperty("Author", "J.C.")]
    [TestCategory("SetData")]
    [DynamicData(nameof(TestPropData))]
    public void SetPropertyTest2(string name, int iTs, string sVal, eValidReact eReact, string sExp, object sDebExp)
        => SetPropertyTest(name, iTs, sVal, eReact, sExp, sDebExp switch { string s => s, string[] a => a[1], _ => "" }, () => iTs switch {
            2 => TestString3 = sVal,
            3 => TestString5 = sVal,
            4 => TestString7 = sVal,
            _ => TestString1 = sVal
        });

    [DataTestMethod()]
    [TestProperty("Author", "J.C.")]
    [TestCategory("SetData")]
    [DynamicData(nameof(TestPropData))]
    public void SetPropertyTest3(string name, int iTs, string sVal, eValidReact eReact, string sExp, object sDebExp)
        => SetPropertyTest(name, iTs, sVal, eReact, sExp, sDebExp switch { string s => s, string[] a => a[2], _ => "" }, () => iTs switch {
            2 => TestString_2 = sVal,
            3 => TestString_4 = sVal,
            4 => TestString_6 = sVal,
            _ => TestString_0 = sVal
        });

    [DataTestMethod()]
    [TestProperty("Author", "J.C.")]
    [TestCategory("SetData")]
    [DynamicData(nameof(TestPropData))]
    public void SetPropertyTest4(string name, int iTs, string sVal, eValidReact eReact, string sExp, object sDebExp)
        => SetPropertyTest(name, iTs, sVal, eReact, sExp, sDebExp switch { string s => s, string[] a => a[3], _ => "" }, () => iTs switch {
            2 => TestString_3 = sVal,
            3 => TestString_5 = sVal,
            4 => TestString_7 = sVal,
            _ => TestString_1 = sVal
        });
}