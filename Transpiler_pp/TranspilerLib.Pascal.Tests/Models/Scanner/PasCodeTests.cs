using Microsoft.VisualStudio.TestTools.UnitTesting;
using TranspilerLib.Pascal.Models.Scanner;
using TranspilerLib.Data;
using System.Collections.Generic;
using static TranspilerLib.Helper.TestHelper;

#pragma warning disable IDE0130
namespace TranspilerLib.Pascal.Models.Scanner.Tests;
#pragma warning restore IDE0130

[TestClass]
public class PasCodeTests : TranspilerLib.Models.Tests.TestBase
{
    private PasCode _testClass = null!;

    [TestInitialize]
    public void Init() => _testClass = new PasCode();

    [TestMethod]
    public void Tokenize_Empty_NoTokens()
    {
        _testClass.OriginalCode = string.Empty;
        var list = new List<TokenData>();
        _testClass.Tokenize(t => list.Add(t));
        Assert.AreEqual(0, list.Count);
    }

    [TestMethod]
    public void Tokenize_SimpleBeginEnd()
    {
        _testClass.OriginalCode = "begin end;";
        var list = new List<TokenData>();
        _testClass.Tokenize(t => list.Add(t));
        Assert.IsTrue(list.Exists(t => t.Code.ToLower()=="begin"));
        Assert.IsTrue(list.Exists(t => t.Code.ToLower().StartsWith("end")));
    }

    [TestMethod]
    public void Parse_SimpleBeginEnd()
    {
        _testClass.OriginalCode = "begin end;";
        var root = _testClass.Parse();
        var code = root.ToCode();
        Assert.IsTrue(code.ToLower().Contains("begin"));
        Assert.IsTrue(code.ToLower().Contains("end"));
    }

    [TestMethod]
    public void Tokenize_Strings_Concatenate()
    {
        _testClass.OriginalCode = "begin 'a''b'; end;";
        var list = new List<TokenData>();
        _testClass.Tokenize(t => list.Add(t));
        Assert.IsTrue(list.Exists(t => t.type==CodeBlockType.String));
    }

    [TestMethod]
    public void Comments_BlockAndLine()
    {
        _testClass.OriginalCode = "begin //x\n(* abc *) {def} end;";
        var list = new List<TokenData>();
        _testClass.Tokenize(t => list.Add(t));
        Assert.IsTrue(list.Exists(t => t.type==CodeBlockType.LComment));
        Assert.IsTrue(list.Exists(t => t.type==CodeBlockType.Comment));
    }
}
