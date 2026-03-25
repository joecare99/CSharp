using BaseLib.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ConsoleDisplay.View.Tests;


[TestClass()]
public class MyConsoleTests : ConsoleProxy
{
    #region Properties
    private ConsoleColor _debColorProp;
    private string DebugLog = "";
    private int _debIntProp;
    private bool _debBoolProp;
    private string _debStringProp = "";
    private int _i1;
    private int _i2;
    private string _st = "";

    public ConsoleColor DebColorProp
    {
        get
        {
            LogDeb("Get", _debColorProp);
            return _debColorProp;
        }
        set
        {
            LogDeb("Set", value);
            _debColorProp = value;
        }
    }

    public int DebIntProp
    {
        get
        {
            LogDeb("Get", _debIntProp);
            return _debIntProp;
        }
        set
        {
            LogDeb("Set", value);
            _debIntProp = value;
        }
    }

    public bool DebBoolProp
    {
        get
        {
            LogDeb("Get", _debBoolProp);
            return _debBoolProp;
        }
        set
        {
            LogDeb("Set", value);
            _debBoolProp = value;
        }
    }

    public string DebStringProp
    {
        get
        {
            LogDeb("Get", _debStringProp);
            return _debStringProp;
        }
        set
        {
            LogDeb("Set", value);
            _debStringProp = value;
        }
    }
    #endregion

    #region Methods
    private void LogDeb(string sMethod, object prop, [CallerMemberName] string sProp = "")
        => DebugLog += $"{sProp}.{sMethod}({prop}){Environment.NewLine}";

    public void DebVoidMethod()
        => LogDeb("Call", null);

    public ConsoleKeyInfo DebConsKInfoFunc()
    {
        LogDeb("Call", $"(#{_debIntProp};{(ConsoleKey)_debIntProp})");
        return new ConsoleKeyInfo((char)_debIntProp, (ConsoleKey)_debIntProp,false,false,false );
    }

    public (int, int) DebIntIntFunc()
    {
        LogDeb("Call", $"({_i1};{_i2})");
        return (_i1, _i2);
    }
    public int DebIntFunc()
    {
        LogDeb("Call", $"({_i1})");
        return _i1;
    }

    public void DebIntIntMethod(int i1, int i2)
        => LogDeb("Call", $"({_i1 = i1};{_i2 = i2})");

    public string DebStringFunc()
    {
        LogDeb("Call", _st);
        return _st;
    }

    public void DebStringMethod(string st)
        => LogDeb("Call", _st = st);

    public void DebCharMethod(char ch)
        => LogDeb("Call", ch);

    [TestMethod()]
    [DataRow(nameof(_ForegroundColor))]
    [DataRow(nameof(_BackgroundColor))]
    [DataRow(nameof(_KeyAvailable))]
    [DataRow(nameof(_WindowHeight))]
    [DataRow(nameof(_WindowWidth))]
    [DataRow(nameof(_LargestWindowHeight))]
    [DataRow(nameof(_Title))]
    [DataRow(nameof(_clear))]
    [DataRow(nameof(_ReadKey))]
    [DataRow(nameof(_writeCh))]
    [DataRow(nameof(_writeLineS))]
    [DataRow(nameof(_setCursorPosition))]
    [DataRow(nameof(_beep))]
#if NET6_0_OR_GREATER
    [DataRow(nameof(GetCursorPosition))]
#endif
    public void PropertyTest(string name)
    {
        var pi = GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).First((p) => p.Name == name)?.GetValue(this);
        Assert.IsNotNull(pi);
        Assert.IsInstanceOfType(pi, typeof(MemberInfo));
    }

    public bool SetDebugProp()
    {
        instance = this;
        _ForegroundColor = GetType().GetProperty(nameof(DebColorProp))!;
        _BackgroundColor = GetType().GetProperty(nameof(DebColorProp))!;
        _WindowHeight = GetType().GetProperty(nameof(DebIntProp))!;
        _WindowWidth = GetType().GetProperty(nameof(DebIntProp))!;
        _KeyAvailable = GetType().GetProperty(nameof(DebBoolProp))!;
        _LargestWindowHeight = GetType().GetProperty(nameof(DebIntProp))!;
        _Title = GetType().GetProperty(nameof(DebStringProp))!;

        _clear = GetType().GetMethod(nameof(DebVoidMethod))!;
        _writeCh = GetType().GetMethod(nameof(DebCharMethod))!;
        _writeS = GetType().GetMethod(nameof(DebStringMethod))!;
        _Readline = GetType().GetMethod(nameof(DebStringFunc))!;
        _setCursorPosition = GetType().GetMethod(nameof(DebIntIntMethod))!;
      #if NET5_0_OR_GREATER
    _GetCursorPosition = GetType().GetMethod(nameof(DebIntIntFunc))!;
#else
    _CursorLeft = GetType().GetMethod(nameof(DebIntFunc))!;
    _CursorTop = GetType().GetMethod(nameof(DebIntFunc))!;
#endif
        _ReadKey = GetType().GetMethod(nameof(DebConsKInfoFunc))!;
        _beep = GetType().GetMethod(nameof(DebIntIntMethod))!;
        return true;
    }

    public bool SetDebugProp2()
    {
        instance = this;
        _ForegroundColor =
        _BackgroundColor =
        _WindowHeight =
        _WindowWidth =
        _KeyAvailable =
        _LargestWindowHeight =
        _Title = null!;

        _clear =
        _writeCh =
        _writeS =
        _Readline =
        _setCursorPosition =
#if NET5_0_OR_GREATER
       _GetCursorPosition =
#else
         _CursorLeft =
         _CursorTop =
#endif
        _ReadKey =
        _beep = null!;
        return true;
    }


    [TestMethod()]
    [DataRow("Normal Green",0,ConsoleColor.Green,new string[] { "Green", "DebColorProp.Set(Green)\r\nDebColorProp.Get(Green)\r\n" })]
    [DataRow("None Green", 1, ConsoleColor.Green, new string[] {"Gray", "" })]
    [DataRow("Normal Green", 0, ConsoleColor.Red, new string[] { "Red", "DebColorProp.Set(Red)\r\nDebColorProp.Get(Red)\r\n" })]
    [DataRow("None Green", 1, ConsoleColor.Red, new string[] { "Gray", "" })]
    public void ForegroundColorTest(string name,int iInit,object oData, object[] aExp)
    {
        _=iInit switch { 0 => SetDebugProp(), 1 => SetDebugProp2(), _ => true };
        ForegroundColor = (ConsoleColor)oData;
        Assert.AreEqual(aExp[0], ForegroundColor.ToString());
        Assert.AreEqual(aExp[1], DebugLog);
    }

    [TestMethod()]
    [DataRow("Normal Green", 0, ConsoleColor.Green, new string[] { "Green", "DebColorProp.Set(Green)\r\nDebColorProp.Get(Green)\r\n" })]
    [DataRow("None Green", 1, ConsoleColor.Green, new string[] { "Gray", "" })]
    [DataRow("Normal Green", 0, ConsoleColor.Red, new string[] { "Red", "DebColorProp.Set(Red)\r\nDebColorProp.Get(Red)\r\n" })]
    [DataRow("None Green", 1, ConsoleColor.Red, new string[] { "Gray", "" })]
    public void BackgroundColorTest(string name, int iInit, object oData, object[] aExp)
    {
        _ = iInit switch { 0 => SetDebugProp(), 1 => SetDebugProp2(), _ => true };
        BackgroundColor = (ConsoleColor)oData;
        Assert.AreEqual(aExp[0], BackgroundColor.ToString());
        Assert.AreEqual(aExp[1], DebugLog);
    }

    [TestMethod()]
    [DataRow("Normal 30", 0, 30, new string[] { "30", "DebIntProp.Set(30)\r\nDebIntProp.Get(30)\r\n" })]
    [DataRow("None 30", 1, 30, new string[] { "0", "" })]
    [DataRow("Normal 60", 0, 60, new string[] { "60", "DebIntProp.Set(60)\r\nDebIntProp.Get(60)\r\n" })]
    [DataRow("None 60", 1, 60, new string[] { "0", "" })]
    public void WindowHeightTest(string name, int iInit, object oData, object[] aExp)
    {
        _ = iInit switch { 0 => SetDebugProp(), 1 => SetDebugProp2(), _ => true };
        WindowHeight = (int)oData;
        Assert.AreEqual(aExp[0], WindowHeight.ToString());
        Assert.AreEqual(aExp[1], DebugLog);
    }

    [TestMethod()]
    [DataRow("Normal 30", 0, 123, new string[] { "123", "DebIntProp.Set(123)\r\nDebIntProp.Get(123)\r\n" })]
    [DataRow("None 30", 1, 123, new string[] { "0", "" })]
    [DataRow("Normal 60", 0, 80, new string[] { "80", "DebIntProp.Set(80)\r\nDebIntProp.Get(80)\r\n" })]
    [DataRow("None 60", 1, 80, new string[] { "0", "" })]
    public void WindowWidthTest(string name, int iInit, object oData, object[] aExp)
    {
        _ = iInit switch { 0 => SetDebugProp(), 1 => SetDebugProp2(), _ => true };
        WindowWidth = (int)oData;
        Assert.AreEqual(aExp[0], WindowWidth.ToString());
        Assert.AreEqual(aExp[1], DebugLog);
    }

    [TestMethod()]
    [DataRow("Normal 30", 0, 30, new string[] { "30", "DebIntProp.Get(30)\r\n" })]
    [DataRow("None 30", 1, 30, new string[] { "0", "" })]
    [DataRow("Normal 60", 0, 60, new string[] { "60", "DebIntProp.Get(60)\r\n" })]
    [DataRow("None 60", 1, 60, new string[] { "0", "" })]
    public void LargestWindowHeightTest(string name, int iInit, object oData, object[] aExp)
    {
        _ = iInit switch { 0 => SetDebugProp(), 1 => SetDebugProp2(), _ => true };
        _debIntProp = (int)oData;
        Assert.AreEqual(aExp[0], LargestWindowHeight.ToString());
        Assert.AreEqual(aExp[1], DebugLog);
    }

    [TestMethod()]
    [DataRow("Normal T", 0, true, new string[] { "True", "DebBoolProp.Get(True)\r\n" })]
    [DataRow("None T", 1, true, new string[] { "False", "" })]
    [DataRow("Normal F", 0, false, new string[] { "False", "DebBoolProp.Get(False)\r\n" })]
    [DataRow("None F", 1, false, new string[] { "False", "" })]
    public void KeyAvailableTest(string name, int iInit, object oData, object[] aExp)
    {
        _ = iInit switch { 0 => SetDebugProp(), 1 => SetDebugProp2(), _ => true };
        _debBoolProp = (bool)oData;
        Assert.AreEqual(aExp[0], KeyAvailable.ToString());
        Assert.AreEqual(aExp[1], DebugLog);
    }

    [TestMethod()]
    [DataRow("Normal T", 0, true, new string[] { "True", "DebVoidMethod.Call()\r\n" })]
    [DataRow("None T", 1, true, new string[] { "False", "" })]
    [DataRow("Normal F", 0, false, new string[] { "False", "DebVoidMethod.Call()\r\n" })]
    [DataRow("None F", 1, false, new string[] { "False", "" })]
    public void ClearTest(string name, int iInit, object oData, object[] aExp)
    {
        _ = iInit switch { 0 => SetDebugProp(), 1 => SetDebugProp2(), _ => true };
        Clear();
        Assert.AreEqual(aExp[1], DebugLog);
    }

    [TestMethod()]
    [DataRow("Normal X", 0, 'X', new string[] { "X", "DebCharMethod.Call(X)\r\n" })]
    [DataRow("None X", 1, 'X', new string[] { "False", "" })]
    [DataRow("Normal Y", 0, 'Y', new string[] { "Y", "DebCharMethod.Call(Y)\r\n" })]
    [DataRow("None Y", 1, 'Y', new string[] { "False", "" })]
    public void WriteTest(string name, int iInit, object oData, object[] aExp)
    {
        _ = iInit switch { 0 => SetDebugProp(), 1 => SetDebugProp2(), _ => true };
        Write((char)oData);
        Assert.AreEqual(aExp[1], DebugLog);
    }

    [TestMethod()]
    [DataRow("Normal X", 0, "Hello World", new string[] { "Hello World", "DebStringMethod.Call(Hello World)\r\n" })]
    [DataRow("None X", 1, "Hello World", new string[] { "", "" })]
    [DataRow("Normal Y", 0, "To be, or not to be ...", new string[] { "To be, or not to be ...", "DebStringMethod.Call(To be, or not to be ...)\r\n" })]
    [DataRow("None Y", 1, "To be, or not to be ...", new string[] { "", "" })]
    public void WriteTest1(string name, int iInit, object oData, object[] aExp)
    {
        _ = iInit switch { 0 => SetDebugProp(), 1 => SetDebugProp2(), _ => true };
        Write((string)oData);
        Assert.AreEqual(aExp[0], _st);
        Assert.AreEqual(aExp[1], DebugLog);
    }

    [TestMethod()]
    [DataRow("Normal X", 0, "Hello World", new string[] { "Hello World\r\n", "DebStringMethod.Call(Hello World\r\n)\r\n" })]
    [DataRow("None X", 1, "Hello World", new string[] { "", "" })]
    [DataRow("Normal Y", 0, "To be, or not to be ...", new string[] { "To be, or not to be ...\r\n", "DebStringMethod.Call(To be, or not to be ...\r\n)\r\n" })]
    [DataRow("None Y", 1, "To be, or not to be ...", new string[] { "", "" })]
    public void WriteLineTest(string name, int iInit, object oData, object[] aExp)
    {
        _ = iInit switch { 0 => SetDebugProp(), 1 => SetDebugProp2(), _ => true };
        WriteLine((string)oData);
        Assert.AreEqual(aExp[0], _st);
        Assert.AreEqual(aExp[1], DebugLog);
    }

    [TestMethod()]
    [DataRow("Normal X", 0, "Hello World", new string[] { "Hello World", "DebStringFunc.Call(Hello World)\r\n" })]
    [DataRow("None X", 1, "Hello World", new string[] { "", "" })]
    [DataRow("Normal Y", 0, "To be, or not to be ...", new string[] { "To be, or not to be ...", "DebStringFunc.Call(To be, or not to be ...)\r\n" })]
    [DataRow("None Y", 1, "To be, or not to be ...", new string[] { "", "" })]
    [DataRow("Normal", 0, "In this nice country ...", new string[] { "In this nice country ...", "DebStringFunc.Call(In this nice country ...)\r\n" })]
    [DataRow("None", 1, "In this nice country ...", new string[] { "", "" })]
    public void ReadLineTest(string name, int iInit, object oData, object[] aExp)
    {
        _ = iInit switch { 0 => SetDebugProp(), 1 => SetDebugProp2(), _ => true };
        _st = (string)oData;
        Assert.AreEqual(aExp[0], ReadLine());
        Assert.AreEqual(aExp[1], DebugLog);
    }

    [TestMethod()]
    [DataRow("Normal Hello ...", 0, "Hello World", new string[] { "Hello World", "DebStringProp.Set(Hello World)\r\nDebStringProp.Get(Hello World)\r\n" })]
    [DataRow("None X", 1, "Hello World", new string[] { "", "" })]
    [DataRow("Normal To be ...", 0, "To be, or not to be ...", new string[] { "To be, or not to be ...", "DebStringProp.Set(To be, or not to be ...)\r\nDebStringProp.Get(To be, or not to be ...)\r\n" })]
    [DataRow("None Y", 1, "To be, or not to be ...", new string[] { "", "" })]
    public void TitleTest(string name, int iInit, object oData, object[] aExp)
    {
        _ = iInit switch { 0 => SetDebugProp(), 1 => SetDebugProp2(), _ => true };
        Title=(string)oData;
        Assert.AreEqual(aExp[0], Title);
        Assert.AreEqual(aExp[1], DebugLog);
    }

    [TestMethod()]
    [DataRow("Normal 50,15", 0, 50,15, new string[] { "(50, 15)", "DebIntIntMethod.Call((50;15))\r\n" })]
    [DataRow("None 50,15", 1, 50, 15, new string[] { "(0, 0)", "" })]
    [DataRow("Normal 80, 25", 0, 80, 25, new string[] { "(80, 25)", "DebIntIntMethod.Call((80;25))\r\n" })]
    [DataRow("None 80, 25", 1, 80, 25, new string[] { "(0, 0)", "" })]
    public void SetCursorPositionTest(string name, int iInit, int oData1,int oData2 , object[] aExp)
    {
        _ = iInit switch { 0 => SetDebugProp(), 1 => SetDebugProp2(), _ => true };
        SetCursorPosition(oData1,oData2);
        Assert.AreEqual(aExp[0], (_i1,_i2).ToString() );
        Assert.AreEqual(aExp[1], DebugLog);
    }

    [TestMethod()]
    [DataRow("Normal Hello ...", 0, 65, new string[] { "A,A", "DebConsKInfoFunc.Call((#65;A))\r\n" })]
    [DataRow("None X", 1, 65, new string[] { ",", "" })]
    [DataRow("Normal To be ...", 0, 127, new string[] { "F16,\x7f", "DebConsKInfoFunc.Call((#127;F16))\r\n" })]
    [DataRow("None Y", 1, 127, new string[] { ",", "" })]
    [DataRow("Normal To be ...", 0, ConsoleKey.Backspace, new string[] { "Backspace,\x8", "DebConsKInfoFunc.Call((#8;Backspace))\r\n" })]
    [DataRow("None Y", 1, ConsoleKey.Backspace, new string[] { ",", "" })]
    public void ReadKeyTest(string name, int iInit, object oData, object[] aExp)
    {
        _ = iInit switch { 0 => SetDebugProp(), 1 => SetDebugProp2(), _ => true };
        _debIntProp = (int)oData;
        var cki = ReadKey();
        Assert.AreEqual(aExp[0], $"{cki?.Key},{cki?.KeyChar}");
        Assert.AreEqual(aExp[1], DebugLog);
    }

    [TestMethod()]
    public void GetCursorPositionTest()
    {
        SetDebugProp();
        var c = GetCursorPosition();
        Assert.AreEqual(0, c.Left);
        Assert.AreEqual(0, c.Top);
        (_i1, _i2) = (43, 21);
        c = GetCursorPosition();
        Assert.AreEqual(43, c.Left);
        Assert.AreEqual(21, c.Top);
        Assert.AreEqual("DebIntIntFunc.Call((0;0))\r\nDebIntIntFunc.Call((43;21))\r\n", DebugLog);
    }

    [TestMethod()]
    public void BeepTest()
    {
        SetDebugProp();
        Beep(220, 200);
    }
#endregion
}