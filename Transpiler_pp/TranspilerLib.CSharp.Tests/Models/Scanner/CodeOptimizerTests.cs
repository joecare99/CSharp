using TranspilerLib.Data;
using TranspilerLib.Models.Scanner;

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
