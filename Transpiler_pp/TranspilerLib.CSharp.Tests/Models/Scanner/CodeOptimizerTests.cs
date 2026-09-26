using System;
using TranspilerLib.Data;
using TranspilerLib.CSharp.Data;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Models.Scanner;
using TranspilerLibTests.Properties;

#pragma warning disable IDE0130 // Der Namespace entspricht stimmt nicht der Ordnerstruktur.
namespace TranspilerLib.Models.Scanner.Tests;
#pragma warning restore IDE0130 // Der Namespace entspricht stimmt nicht der Ordnerstruktur.

[TestClass]
public class CodeOptimizerTests
{
    [TestMethod]
    [DataRow("if (gift > 0)", "while (gift > 0)")]
    [DataRow("if(gift > 0)", "while(gift > 0)")]
    public void TestItem_RewritesWhileWithoutReplacingInnerIfTokens(string conditionCode, string expectedCode)
    {
        var testClass = new CodeOptimizer();
        var root = CreateBlock(CodeBlockType.MainBlock, "root");
        var label = CreateBlock(CodeBlockType.Label, "Label:", root);
        _ = CreateBlock(CodeBlockType.Operation, "work();", root);
        var ifItem = CreateBlock(CodeBlockType.Operation, conditionCode, root);
        var gotoSource = CreateBlock(CodeBlockType.Goto, "goto Label;", ifItem);
        var helper = CreateBlock(CodeBlockType.Operation, "helper", root);
        var otherSource = CreateBlock(CodeBlockType.Goto, "goto Label;", helper);
        ConnectSource(gotoSource, label);
        ConnectSource(otherSource, label);

        testClass.TestItem(label);

        Assert.AreEqual(expectedCode, ifItem.Code);
        Assert.AreEqual(2, ifItem.SubBlocks.Count);
        Assert.AreEqual("work();", ifItem.SubBlocks[0].Code);
        Assert.IsFalse(ifItem.Code.Contains("gwhilet"));
    }

    [TestMethod]
    public void TestItem_ReplacesOnlyIdentifierTokensInIfCondition()
    {
        var testClass = new CodeOptimizer();
        var root = CreateBlock(CodeBlockType.MainBlock, "root");
        var label = CreateBlock(CodeBlockType.Label, "Label:", root);
        _ = CreateBlock(CodeBlockType.Operation, "work();", root);
        _ = CreateBlock(CodeBlockType.Operation, "x = value;", root);
        var ifItem = CreateBlock(CodeBlockType.Operation, "if (x > 0 && max > 1)", root);
        var gotoSource = CreateBlock(CodeBlockType.Goto, "goto Label;", ifItem);
        var helper = CreateBlock(CodeBlockType.Operation, "helper", root);
        var otherSource = CreateBlock(CodeBlockType.Goto, "goto Label;", helper);
        ConnectSource(gotoSource, label);
        ConnectSource(otherSource, label);

        testClass.TestItem(label);

        Assert.AreEqual("while (value > 0 && max > 1)", ifItem.Code);
        Assert.IsFalse(ifItem.Code.Contains("mavalue"));
        Assert.AreEqual("work();", ifItem.SubBlocks[0].Code);
    }

    [TestMethod]
    public void TestItem_DoesNotThrowWhenResumeNextSourceIsFirstSwitchChild()
    {
        var testClass = new CodeOptimizer() { _noWhile = true };
        var root = CreateBlock(CodeBlockType.MainBlock, "root");
        var label = CreateBlock(CodeBlockType.Label, "Label:", root);
        var switchBlock = CreateBlock(CodeBlockType.Operation, "switch (num4)", root);
        var gotoSource = CreateBlock(CodeBlockType.Goto, "goto Label;", switchBlock);
        var helper = CreateBlock(CodeBlockType.Operation, "helper", root);
        var otherSource = CreateBlock(CodeBlockType.Goto, "goto Label;", helper);
        ConnectSource(gotoSource, label);
        ConnectSource(otherSource, label);

        testClass.TestItem(label);

        Assert.AreEqual(1, switchBlock.SubBlocks.Count);
        Assert.AreSame(switchBlock, gotoSource.Parent);
        Assert.AreEqual(2, label.Sources.Count);
    }

    [TestMethod]
    public void TestItem_RemovesGotoWhenNextInstructionTargetsSameLabel()
    {
        var testClass = new CodeOptimizer() { _noWhile = true };
        var root = CreateBlock(CodeBlockType.MainBlock, "root");
        var firstGoto = CreateBlock(CodeBlockType.Goto, "goto Label;", root);
        _ = CreateBlock(CodeBlockType.Comment, "// comment", root);
        var nextGoto = CreateBlock(CodeBlockType.Goto, "goto Label;", root);
        var label = CreateBlock(CodeBlockType.Label, "Label:", root);
        ConnectSource(firstGoto, label);
        ConnectSource(nextGoto, label);

        testClass.TestItem(label);

        Assert.IsNull(firstGoto.Parent);
        Assert.AreEqual(3, root.SubBlocks.Count);
        Assert.AreSame(nextGoto, root.SubBlocks[1]);
        Assert.AreEqual(1, label.Sources.Count);
    }

    [TestMethod]
    public void TestItem_KeepsGotoWhenNextInstructionTargetsDifferentLabel()
    {
        var testClass = new CodeOptimizer() { _noWhile = true };
        var root = CreateBlock(CodeBlockType.MainBlock, "root");
        var firstGoto = CreateBlock(CodeBlockType.Goto, "goto First;", root);
        _ = CreateBlock(CodeBlockType.Comment, "// comment", root);
        var nextGoto = CreateBlock(CodeBlockType.Goto, "goto Second;", root);
        var firstLabel = CreateBlock(CodeBlockType.Label, "First:", root);
        var secondLabel = CreateBlock(CodeBlockType.Label, "Second:", root);
        var additionalSource = CreateBlock(CodeBlockType.Goto, "goto First;", root);
        ConnectSource(firstGoto, firstLabel);
        ConnectSource(nextGoto, secondLabel);
        ConnectSource(additionalSource, firstLabel);

        testClass.TestItem(firstLabel);

        Assert.AreSame(root, firstGoto.Parent);
        Assert.AreEqual(6, root.SubBlocks.Count);
        Assert.AreEqual(2, firstLabel.Sources.Count);
    }

    [TestMethod]
    [DataRow("Test15Dat_cs", 2, DisplayName = "Reduces goto across nested if blocks")]
    [DataRow("Test16Dat_cs", 2, DisplayName = "Keeps goto across while boundary")]
    public void RemoveSingleSourceLabels1_HandlesGotoAcrossControlFlowBoundaries(string resourceName, int expectedGotoCount)
    {
        var source = Resources.ResourceManager.GetString(resourceName);
        Assert.IsNotNull(source);

        var root = ParseAndOptimize(source);

        Assert.AreEqual(expectedGotoCount, CountGotos(root));
    }

    [TestMethod]
    public void Parse_SeparatesElseAndFollowingIfBranches()
    {
        const string source = @"private void Test20Dat(bool b1, bool b2)
{
    if (b1)
    {
        // some code 1
    }
    else if (b2)
    {
        // some code 2
    }
}";

        var code = new CSCode(new CSTokenHandler()
        {
            stringEndChars = CSCode.stringEndChars,
            reservedWords = CSCode.ReservedWords
        }, new CSCodeBuilder(), new CodeOptimizer())
        {
            OriginalCode = source
        };

        var root = code.Parse() ?? throw new InvalidOperationException("The parser should produce a root block.");
        var parsed = root.ToString() ?? string.Empty;

        Assert.IsTrue(parsed.Contains("else", StringComparison.Ordinal));
        Assert.IsTrue(parsed.Contains("if (b2)", StringComparison.Ordinal));
        Assert.IsFalse(parsed.Contains("else if", StringComparison.Ordinal));
        Assert.IsFalse(parsed.Contains("elseif", StringComparison.OrdinalIgnoreCase));
    }

    [TestMethod]
    public void RemoveSingleSourceLabels1_PreservesChainedConditionalGotoStructure()
    {
        const string source = @"private void Test21Dat(bool b1, bool b2, bool b3)
{
    if (b1)
    {
        // some code 1
        goto End;
    }
    if (b2)
    {
        // some code 2
        goto End;
    }
    if (b3)
    {
        // some code 3
    }
    goto End;
End:
    // some code 4
    return;
}";

        var code = new CSCode(new CSTokenHandler()
        {
            stringEndChars = CSCode.stringEndChars,
            reservedWords = CSCode.ReservedWords
        }, new CSCodeBuilder(), new CodeOptimizer())
        {
            OriginalCode = source
        };

        var root = code.Parse() ?? throw new InvalidOperationException("The parser should produce a root block.");
        code.RemoveSingleSourceLabels1(root);
        var parsed = root.ToString() ?? string.Empty;

        var output = root.ToCode();
        Assert.IsTrue(parsed.Contains("else", StringComparison.Ordinal));
        Assert.IsTrue(parsed.Contains("if (b2)", StringComparison.Ordinal));
        Assert.IsTrue(parsed.Contains("if (b3)", StringComparison.Ordinal));
        Assert.AreEqual(1, CountGotos(root), output);
        Assert.IsTrue(output.Contains("else if (b2)", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("else if (b3)", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("goto End;", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("End:", StringComparison.Ordinal));
    }

    [TestMethod]
    public void RemoveSingleSourceLabels1_PreservesSwitchGotoWhenCodePrecedesOuterGoto()
    {
        const string source = @"private void Test19aDat(int state)
{
    switch (state)
    {
        case 1:
            goto end;
        default:
            goto end;
    }
    AfterSwitch();
    goto end;
end:
    return;
}";

        var root = ParseAndOptimize(source);
        var output = root.ToCode();

        Assert.AreEqual(3, CountGotos(root));
        Assert.IsTrue(output.Contains("case 1:", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("goto end;", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("AfterSwitch();", StringComparison.Ordinal));
    }

    [TestMethod]
    public void RemoveSingleSourceLabels1_ReducesTest22ControlFlow()
    {
        var root = ParseAndOptimize(Resources.Test22Dat_cs);
        var output = root.ToCode();

        Assert.IsFalse(output.Contains("IL_023d:", StringComparison.Ordinal));
        Assert.IsFalse(output.Contains("goto IL_023d;", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("else if ((left == \"V\") || (left == \"v\"))", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("else if (aus[46] != \"1\")", StringComparison.Ordinal));
        Assert.IsTrue(System.Text.RegularExpressions.Regex.IsMatch(
            output,
            @"else\r?\n\s*\{\r?\n\s*Datu = Datu;\r?\n\s*if \(aus\[46\] != ""1""\)"));
        Assert.IsFalse(output.Contains("Datu = \"um \" + Datu;\r\n                            goto end_IL_0000_2;", StringComparison.Ordinal));
    }

    [TestMethod]
    public void RemoveSingleSourceLabels1_ReducesTest23InnerGotos()
    {
        var root = ParseAndOptimize(Resources.Test23Dat_cs);
        var output = root.ToCode();

        Assert.AreEqual(2, output.Split("goto IL_05f8;", StringSplitOptions.None).Length - 1, output);
        Assert.IsFalse(output.Contains("goto IL_0539;", StringComparison.Ordinal));
        Assert.IsFalse(output.Contains("IL_0539:", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("IL_05f8:", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("else if (aus[75] == \"1\")", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("OrtTable.Fields[\"Zusatz\"]", StringComparison.Ordinal));
    }

    private static ICodeBlock ParseAndOptimize(string source)
    {
        var optimizer = new CodeOptimizer() { _noWhile = true };
        var code = new CSCode(new CSTokenHandler()
        {
            stringEndChars = CSCode.stringEndChars,
            reservedWords = CSCode.ReservedWords
        }, new CSCodeBuilder(), optimizer)
        {
            OriginalCode = source
        };
        var root = code.Parse();
        code.RemoveSingleSourceLabels1(root);
        return root;
    }

    private static int CountGotos(ICodeBlock root)
    {
        var count = 0;
        foreach (var block in root.SubBlocks)
        {
            if (block.Type == CodeBlockType.Goto)
                count++;
            count += CountGotos(block);
        }

        return count;
    }

    private static CodeBlock CreateBlock(CodeBlockType type, string code, CodeBlock? parent = null)
    {
        var result = new CodeBlock()
        {
            Type = type,
            Name = code,
            Code = code,
        };
        if (parent != null)
            result.Parent = parent;
        return result;
    }

    private static void ConnectSource(CodeBlock source, CodeBlock target)
    {
        source.Destination = new(target);
        target.Sources.Add(new(source));
    }
}
