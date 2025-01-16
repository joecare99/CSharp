// ***********************************************************************
// Assembly         : MVVM_BaseLibTests
// Author           : Mir
// Created          : 05-22-2023
//
// Last Modified By : Mir
// Last Modified On : 05-23-2023
// ***********************************************************************
// <copyright file="NotificationObjectCTTests.cs" company="JC-Soft">
//     Copyright � JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.Linq;

/// <summary>
/// The ViewModels namespace.
/// </summary>
/// <autogeneratedoc />
namespace Avalonia.ViewModels.Tests;

/// <summary>
/// Defines test class PropertyTests.
/// </summary>
[TestClass()]
public class NotificationObjectCTTests : NotificationObjectCT
{
    /// <summary>
    /// Enum eValidReact
    /// </summary>
    /// <autogeneratedoc />
    public enum eValidReact
    {
        /// <summary>
        /// The ok
        /// </summary>
        /// <autogeneratedoc />
        OK = 0,
        /// <summary>
        /// The nio
        /// </summary>
        /// <autogeneratedoc />
        NIO,
        /// <summary>
        /// The general exception
        /// </summary>
        /// <autogeneratedoc />
        GeneralException,
        /// <summary>
        /// The argument exception
        /// </summary>
        /// <autogeneratedoc />
        ArgumentException,
    }

    /// <summary>
    /// The test string
    /// </summary>
    /// <autogeneratedoc />
    private string _testString="";
    /// <summary>
    /// The test int
    /// </summary>
    /// <autogeneratedoc />
    private int _testInt;
    /// <summary>
    /// The test float
    /// </summary>
    /// <autogeneratedoc />
    private float _testFloat;
    /// <summary>
    /// The test double
    /// </summary>
    /// <autogeneratedoc />
    private double _testDouble;

    /// <summary>
    /// The value react
    /// </summary>
    /// <autogeneratedoc />
    private eValidReact valReact=eValidReact.OK;

    /// <summary>
    /// The debug result
    /// </summary>
    /// <autogeneratedoc />
    private string DebugResult ="";

    /// <summary>
    /// Gets or sets the test string.
    /// </summary>
    /// <value>The test string.</value>
    /// <autogeneratedoc />
    public string TestString { get => _testString;set=>SetProperty(ref _testString,value); }

    /// <summary>
    /// Gets or sets the test string0a.
    /// </summary>
    /// <value>The test string0a.</value>
    /// <autogeneratedoc />
    public string TestString0a { get => _testString; set => ExecPropSetter((v) => _testString = v, _testString, value); }
    /// <summary>
    /// Gets or sets the test string2a.
    /// </summary>
    /// <value>The test string2a.</value>
    /// <autogeneratedoc />
    public string TestString2a { get => _testString; set => ExecPropSetter((v) => _testString = v, _testString, value, StringAct); }
    /// <summary>
    /// Gets or sets the test string6a.
    /// </summary>
    /// <value>The test string6a.</value>
    /// <autogeneratedoc />
    public string TestString6a { get => _testString; set
            => ExecPropSetter((v)=>_testString=v, _testString, value, new string[] { nameof(TestString), nameof(TestString) },null, StringAct);}
    /// <summary>
    /// Gets or sets the test string7a.
    /// </summary>
    /// <value>The test string7a.</value>
    /// <autogeneratedoc />
    public string TestString7a { get => _testString; set
            => ExecPropSetter((v)=>_testString=v, _testString, value, new string[] { nameof(TestString), nameof(TestString) }, ValidateString, StringAct);}

    /// <summary>
    /// Gets or sets the test int.
    /// </summary>
    /// <value>The test int.</value>
    /// <autogeneratedoc />
    public int TestInt { get => _testInt; set => SetProperty(ref _testInt, value); }
    /// <summary>
    /// Gets or sets the test float.
    /// </summary>
    /// <value>The test float.</value>
    /// <autogeneratedoc />
    public float TestFloat { get => _testFloat; set => SetProperty(ref _testFloat, value); }
    /// <summary>
    /// Gets or sets the test double.
    /// </summary>
    /// <value>The test double.</value>
    /// <autogeneratedoc />
    public double TestDouble { get => _testDouble; set => SetProperty(ref _testDouble, value); }

    /// <summary>
    /// Strings the act.
    /// </summary>
    /// <param name="arg1">The arg1.</param>
    /// <param name="arg2">The arg2.</param>
    /// <exception cref="Exception">A general exception occurred</exception>
    /// <exception cref="ArgumentException">Argument ({arg2}) not valid!</exception>
    /// <autogeneratedoc />
    private void StringAct(string arg1, string arg2)
    {
        DebugResult += $"StrAct: {arg1}; {arg2}{Environment.NewLine}";
        var _=valReact switch
        {
            eValidReact.GeneralException => throw new Exception("A general exception occurred"),
            eValidReact.ArgumentException => throw new ArgumentException($"Argument ({arg2}) not valid!"),
            _ => (object?)null,
        };
    }

    /// <summary>
    /// Validates the string.
    /// </summary>
    /// <param name="arg1">The arg1.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    /// <exception cref="Exception">A general exception occurred</exception>
    /// <exception cref="ArgumentException">Argument ({arg1}) not valid!</exception>
    /// <autogeneratedoc />
    private bool ValidateString(string arg1)
    {
        DebugResult += $"Validate: {arg1}, React:{valReact}{Environment.NewLine}";
        return valReact switch
        {
            eValidReact.OK => true,
            eValidReact.NIO => false,
            eValidReact.GeneralException => throw new Exception("A general exception occurred"),
            eValidReact.ArgumentException => throw new ArgumentException($"Argument ({arg1}) not valid!"),
            _ => false,
        };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationObjectCTTests"/> class.
    /// </summary>
    /// <autogeneratedoc />
    public NotificationObjectCTTests()
    {
    }

    /// <summary>
    /// Clears this instance.
    /// </summary>
    /// <autogeneratedoc />
    private void Clear()
    {
        _testString = String.Empty;
        _testInt = 0;
        _testFloat = 0f;
        _testDouble = 0d;
        DebugResult = "";
        PropertyChanged -= OnPropertyChanged;
    }

    /// <summary>
    /// Handles the <see cref="E:PropertyChanged" /> event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
    /// <autogeneratedoc />
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        => DebugResult += $"OnPropChanged: o:{sender.GetType().Name}, p:{e.PropertyName}:{sender?.GetType().GetProperty(e.PropertyName!)?.GetValue(sender)}{Environment.NewLine}";

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <autogeneratedoc />
    [TestInitialize]
    public void Init()
    {
        Clear();
        PropertyChanged += OnPropertyChanged;
    }

    /// <summary>
    /// Defines the test method TestRaise.
    /// </summary>
    /// <autogeneratedoc />
    [TestMethod()]
    public void TestRaise()
    {
        PropertyChanged -= OnPropertyChanged;
        OnPropertyChanged("Test");
        OnPropertyChanged("Test2");
        Assert.AreEqual("", DebugResult);
    }

    /// <summary>
    /// Tests the string property.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="iTs">The i ts.</param>
    /// <param name="sVal">The s value.</param>
    /// <param name="eReact">The e react.</param>
    /// <param name="sExp">The s exp.</param>
    /// <param name="sDebExp">The s deb exp.</param>
    /// <autogeneratedoc />
    [DataTestMethod]
    [TestProperty("Author","J.C.")]
    [DataRow("00 Empty",0,"",eValidReact.OK,"","")]
    [DataRow("01-Test", 0, "Test", eValidReact.OK, "Test", "OnPropChanged: o:NotificationObjectCTTests, p:TestString:Test\r\n")]
           public void TestStringProp(string name,int iTs,string sVal, eValidReact eReact, string sExp,string sDebExp)
    {
        valReact = eReact;
        bool xCh = sVal != _testString;
        bool eRIsEx = eReact == eValidReact.GeneralException || eReact == eValidReact.ArgumentException;
        bool xITsHasVl = new[]{ 1,3,5,7}.Contains(iTs);
        Action<string> Setter = iTs switch
        {            
            _ => (s) => TestString = s
        };
        if (xCh && xITsHasVl && eReact == eValidReact.GeneralException)
            Assert.ThrowsException<Exception>(() => Setter(sVal), $"{name}.T{iTs}");
        else if (xCh && xITsHasVl && eReact == eValidReact.ArgumentException)
            Assert.ThrowsException<ArgumentException>(() => Setter(sVal), $"{name}.T{iTs}");
        else
            Setter(sVal);

        Assert.AreEqual(sExp, _testString, $"{name}.Result");
        Assert.AreEqual(sDebExp, DebugResult, $"{name}.DebRes");    
    }

    /// <summary>
    /// Tests the string2 property.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="iTs">The i ts.</param>
    /// <param name="sVal">The s value.</param>
    /// <param name="eReact">The e react.</param>
    /// <param name="sExp">The s exp.</param>
    /// <param name="sDebExp">The s deb exp.</param>
    /// <autogeneratedoc />
    [DataTestMethod]
    [TestProperty("Author", "J.C.")]
    [DataRow("00 Empty", 0, "", eValidReact.OK, "", new string[] { "" })]
    [DataRow("01-Test", 0, "Test", eValidReact.OK, "Test", new string[] { "OnPropChanged: o:NotificationObjectCTTests, p:TestString0a:Test\r\n" })]
    [DataRow("20-1 Empty", 2, "", eValidReact.OK, "", new string[] { "" })]
    [DataRow("20-2 Test", 2, "", eValidReact.NIO, "", new string[] { "" })]
    [DataRow("21-1 Test", 2, "Test", eValidReact.OK, "Test", new string[] { "OnPropChanged: o:NotificationObjectCTTests, p:TestString2a:Test\r\nStrAct: ; Test\r\n" })]
    [DataRow("21-2 Test2", 2, "Test2", eValidReact.NIO, "Test2", new string[] { "OnPropChanged: o:NotificationObjectCTTests, p:TestString2a:Test2\r\nStrAct: ; Test2\r\n" })]
    [DataRow("21-3 GEx", 2, "Test", eValidReact.GeneralException, "Test", new string[] { "OnPropChanged: o:NotificationObjectCTTests, p:TestString2a:Test\r\nStrAct: ; Test\r\n" })]
    [DataRow("21-4 AEx", 2, "Test2", eValidReact.ArgumentException, "Test2", new string[] { "OnPropChanged: o:NotificationObjectCTTests, p:TestString2a:Test2\r\nStrAct: ; Test2\r\n" })]
    [DataRow("60-1 Empty", 6, "", eValidReact.OK, "", new string[] { "" })]
    [DataRow("60-2 Test ", 6, "", eValidReact.NIO, "", new string[] { "" })]
    [DataRow("61-1 Test ", 6, "Test", eValidReact.OK, "Test", new string[] { "OnPropChanged: o:NotificationObjectCTTests, p:TestString6a:Test\r\nOnPropChanged: o:NotificationObjectCTTests, p:TestString:Test\r\nOnPropChanged: o:NotificationObjectCTTests, p:TestString:Test\r\nStrAct: ; Test\r\n" })]
    [DataRow("61-2 Test2", 6, "Test2", eValidReact.NIO, "Test2", new string[] { "OnPropChanged: o:NotificationObjectCTTests, p:TestString6a:Test2\r\nOnPropChanged: o:NotificationObjectCTTests, p:TestString:Test2\r\nOnPropChanged: o:NotificationObjectCTTests, p:TestString:Test2\r\nStrAct: ; Test2\r\n" })]
    [DataRow("61-3 GEx  ", 6, "Test", eValidReact.GeneralException, "Test", new string[] { "OnPropChanged: o:NotificationObjectCTTests, p:TestString6a:Test\r\nOnPropChanged: o:NotificationObjectCTTests, p:TestString:Test\r\nOnPropChanged: o:NotificationObjectCTTests, p:TestString:Test\r\nStrAct: ; Test\r\n" })]
    [DataRow("70-1 Empty", 7, "", eValidReact.OK, "", new string[] { "" })]
    [DataRow("70-2 Test ", 7, "", eValidReact.NIO, "", new string[] { "" })]
    [DataRow("70-2 Test ", 7, "", eValidReact.GeneralException, "", new string[] { "" })]
    [DataRow("71-1 Empty", 7, "Test", eValidReact.OK, "Test", new string[] { "Validate: Test, React:OK\r\nOnPropChanged: o:NotificationObjectCTTests, p:TestString7a:Test\r\nOnPropChanged: o:NotificationObjectCTTests, p:TestString:Test\r\nOnPropChanged: o:NotificationObjectCTTests, p:TestString:Test\r\nStrAct: ; Test\r\n" })]
    [DataRow("71-2 Test ", 7, "Test", eValidReact.NIO, "", new string[] { "Validate: Test, React:NIO\r\n" })]
    [DataRow("71-3 GEx  ", 7, "Test", eValidReact.GeneralException, "", new string[] { "Validate: Test, React:GeneralException\r\n" })]
    [DataRow("71-4 AEx  ", 7, "Test", eValidReact.ArgumentException, "",new string[] { "Validate: Test, React:ArgumentException\r\n" })]
    public void TestString2Prop(string name, int iTs, string sVal, eValidReact eReact, string sExp, string[] sDebExp)
    {
        valReact = eReact;
        bool xCh = sVal != _testString;
        bool eRIsEx = eReact == eValidReact.GeneralException || eReact == eValidReact.ArgumentException;
        bool xITsHasVl = new[] { 1, 3, 5, 7 }.Contains(iTs);
        Action<string> Setter = iTs switch
        {
            2 => (s) => TestString2a = s,
            6 => (s) => TestString6a = s,
            7 => (s) => TestString7a = s,
            _ => (s) => TestString0a = s
        };

        if (xCh && xITsHasVl && eReact == eValidReact.GeneralException)
            Assert.ThrowsException<Exception>(() => Setter(sVal), $"{name}.T{iTs}");
        else if (xCh && xITsHasVl && eReact == eValidReact.ArgumentException) 
            Assert.ThrowsException<ArgumentException>(() => Setter(sVal), $"{name}.T{iTs}");
        else
            Setter(sVal);

        Assert.AreEqual(sExp, _testString, $"{name}.Result");
        Assert.AreEqual(sDebExp[0], DebugResult, $"{name}.DebRes");
    }

}
