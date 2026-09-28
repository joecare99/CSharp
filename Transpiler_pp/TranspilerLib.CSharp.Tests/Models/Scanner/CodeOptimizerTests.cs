using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public TestContext? TestContext { get; set; }

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
    [DataRow("Test24Dat_cs",
        new string[] { "if ((b1) && (b2))", "branch note", "SomeFunction();", "NoElseFunction();" },
        new string[] { "if (b2)" },
        new string[] { "SomeFunction();", "NoElseFunction();" },
        DisplayName = "Test24: Merges nested ifs with && and preserves the branch comment")]
    [DataRow("Test24aDat_cs",
        new string[] { "if (b1)", "if (b2)", "SomeOtherFunction();" },
        new string[] { "&&" },
        new string[] { "SomeOtherFunction();" },
        DisplayName = "Test24a: Keeps nested ifs separate when the outer if has an else")]
    [DataRow("Test24bDat_cs",
        new string[] { "if (b1)", "if (b2)", "BeforeNestedIf();" },
        new string[] { "&&" },
        new string[] { "BeforeNestedIf();", "SomeFunction();" },
        DisplayName = "Test24b: Keeps nested ifs separate when the outer branch has another statement")]
    [DataRow("Test24cDat_cs",
        new string[] { "if ((b1) && (b2))", "goto End;", "End:", "SomeFunction();" },
        new string[] { "if (b2)" },
        new string[] { "goto End;" },
        DisplayName = "Test24c: Merges nested ifs while preserving the goto to its shared label")]
    [DataRow("Test25Dat_cs",
        new string[] { "if ((b1) || (b2))", "first equivalent branch", "second equivalent branch" },
        new string[] { "else if (b2)" },
        new string[] { "SomeFunction();", "first equivalent branch", "second equivalent branch" },
        DisplayName = "Test25: Merges equivalent else-if bodies and preserves both comments")]
    [DataRow("Test25aDat_cs",
        new string[] { "if (b1)", "else", "if (b2)", "SomeFunction();", "SomeOtherFunction();" },
        new string[] { "||" },
        new string[] { "SomeFunction();", "SomeOtherFunction();" },
        DisplayName = "Test25a: Rejects else-if branches with different bodies")]
    [DataRow("Test25bDat_cs",
        new string[] { "if ((b1) || (b2))", "value = 1;" },
        new string[] { "else if (b2)" },
        new string[] { "value = 1;" },
        DisplayName = "Test25b: Merges equivalent assignment branches and executes the assignment once")]
    [DataRow("Test25cDat_cs",
        new string[] { "if (b1)", "BeforeNestedIf();", "if (b2)", "SomeFunction();" },
        new string[] { "||" },
        null,
        DisplayName = "Test25c: Rejects an else block that has an executable statement")]
    [DataRow("Test26Dat_cs",
        new string[] { "if ((b1) || (b2))", "if (b3)", "SomeFunction();", "SomeOtherFunction();" },
        new string[] { "|| (b3)" },
        new string[] { "SomeFunction();", "SomeOtherFunction();" },
        DisplayName = "Test26: Merges the first else-if pair and retains its distinct fallback")]
    [DataRow("Test26aDat_cs",
        new string[] { "if (b1)", "else", "if (b2)", "SomeFunction();", "SomeOtherFunction();" },
        new string[] { "||" },
        new string[] { "SomeFunction();", "SomeOtherFunction();" },
        DisplayName = "Test26a: Does not merge a longer chain when its first pair differs")]
    [DataRow("Test26bDat_cs",
        new string[] { "if ((b1) || (b2 || b3))", "SomeFunction();" },
        new string[] { "else if (b2 || b3)" },
        new string[] { "SomeFunction();" },
        DisplayName = "Test26b: Merges an alternative whose condition is already an OR expression")]
    [DataRow("Test26cDat_cs",
        new string[] { "if (((b1) || (b2)) || (b3))", "SomeFunction();" },
        new string[] { "else if (b3)" },
        new string[] { "SomeFunction();" },
        DisplayName = "Test26c: Merges three equivalent alternatives while preserving left-to-right short-circuiting")]
    [DataRow("Test27Dat_cs",
        new string[] { "if (b1)", "else", "if (b2)", "SomeFunction();", "SomeOtherFunction();" },
        new string[] { "||" },
        new string[] { "SomeFunction();", "SomeOtherFunction();" },
        DisplayName = "Test27: Preserves the main negative case with different bodies")]
    [DataRow("Test27aDat_cs",
        new string[] { "if (b1)", "else", "if (b2)", "FirstFunction();", "SecondFunction();" },
        new string[] { "||" },
        new string[] { "SecondFunction();" },
        DisplayName = "Test27a: Rejects otherwise matching branches with different statement counts")]
    [DataRow("Test27bDat_cs",
        new string[] { "if (b1)", "else", "if ((b2) && (b3))" },
        new string[] { "||" },
        null,
        DisplayName = "Test27b: Does not OR-merge nested if bodies, though safe inner AND remains allowed")]
    [DataRow("Test27cDat_cs",
        new string[] { "if ((b1) || (b2))", "Comment differences", "Both comments", "Additional note" },
        new string[] { "else if (b2)" },
        new string[] { "SomeFunction();", "Comment differences", "Both comments", "Additional note" },
        DisplayName = "Test27c: Ignores comment differences but retains comments from both branches")]
    public void TestIfElseOptimize(
        string Code,
        string[] Contains,
        string[] NotContains,
        string[]? ContainsOnce = null)
    {
        var source = Resources.ResourceManager.GetString(Code)
            ?? throw new InvalidOperationException($"Code resource '{Code}' was not found.");
        var root = ParseWithoutLabelReduction(source);
        new CodeOptimizer().OptimizeIfChains(root);
        var generatedCode = root.ToCode();

        Debug.WriteLine($"Generated code for {TestContext?.TestName ?? nameof(TestIfElseOptimize)} [{Code}]:{Environment.NewLine}{generatedCode}");

        foreach (var expectedText in Contains)
            Assert.IsTrue(generatedCode.Contains(expectedText, StringComparison.Ordinal),
                $"Expected generated code to contain '{expectedText}'.{Environment.NewLine}{generatedCode}");

        foreach (var unexpectedText in NotContains)
            Assert.IsFalse(generatedCode.Contains(unexpectedText, StringComparison.Ordinal),
                $"Expected generated code not to contain '{unexpectedText}'.{Environment.NewLine}{generatedCode}");

        if (ContainsOnce is null)
            return;

        foreach (var expectedOnce in ContainsOnce)
            Assert.AreEqual(1, CountOccurrences(generatedCode, expectedOnce),
                $"Expected '{expectedOnce}' exactly once.{Environment.NewLine}{generatedCode}");
    }

    [TestMethod]
    [DataRow("Test28Dat_cs", 4,
        new string[] { "else if (b2)", "// Keep this line comment", "D();", "E();", "F();" },
        new string[] { },
        DisplayName = "Flattens an else wrapper containing an if-else and keeps its line comment before else-if")]
    [DataRow("Test28aDat_cs", 1,
        new string[] { "// Preserve this line comment", "if ((b1) && (b2))", "E();" },
        new string[] { "if (b1)", "if (b2)" },
        DisplayName = "Removes nested singleton blocks after merging nested if conditions")]
    [DataRow("Test28dDat_cs", 2,
        new string[] { "if (b1)", "A();", "B();" },
        new string[] { },
        DisplayName = "Removes only the redundant outer block around a grouped multi-statement body")]
    [DataRow("Test28bDat_cs", 3,
        new string[] { "BeforeNestedIf();", "if (b2)" },
        new string[] { "else if (b2)" },
        DisplayName = "Retains a wrapper with another executable statement")]
    [DataRow("Test28cDat_cs", 4,
        new string[] { "/* Keep block comments unchanged. */", "if (b2)" },
        new string[] { "else if (b2)" },
        DisplayName = "Retains a wrapper whose leading comment is not a line comment")]
    public void TestUnnecessaryBlockRemoval(
        string Code,
        int ExpectedBlockCount,
        string[] Contains,
        string[] NotContains)
    {
        var source = Resources.ResourceManager.GetString(Code)
            ?? throw new InvalidOperationException($"Code resource '{Code}' was not found.");
        var root = ParseWithoutLabelReduction(source);
        new CodeOptimizer().OptimizeIfChains(root);
        var generatedCode = root.ToCode();

        Debug.WriteLine($"Generated code for {TestContext?.TestName ?? nameof(TestUnnecessaryBlockRemoval)} [{Code}]:{Environment.NewLine}{generatedCode}");

        foreach (var expectedText in Contains)
            Assert.IsTrue(generatedCode.Contains(expectedText, StringComparison.Ordinal),
                $"Expected generated code to contain '{expectedText}'.{Environment.NewLine}{generatedCode}");

        foreach (var unexpectedText in NotContains)
            Assert.IsFalse(generatedCode.Contains(unexpectedText, StringComparison.Ordinal),
                $"Expected generated code not to contain '{unexpectedText}'.{Environment.NewLine}{generatedCode}");

        Assert.AreEqual(ExpectedBlockCount, CountOccurrences(generatedCode, "{"),
            $"Unexpected number of opening block delimiters.{Environment.NewLine}{generatedCode}");
        Assert.AreEqual(ExpectedBlockCount, CountOccurrences(generatedCode, "}"),
            $"Unexpected number of closing block delimiters.{Environment.NewLine}{generatedCode}");
    }

    [TestMethod]
    public void TestUnnecessaryBlockRemoval_MovesLineCommentAheadOfElseWithoutChangingItsType()
    {
        var source = Resources.ResourceManager.GetString("Test28Dat_cs")
            ?? throw new InvalidOperationException("Code resource 'Test28Dat_cs' was not found.");
        var root = ParseWithoutLabelReduction(source);
        new CodeOptimizer().OptimizeIfChains(root);
        var lineComment = FindBlock(root, block => block.Code.Contains("Keep this line comment", StringComparison.Ordinal));
        var elseIf = FindBlock(root, block => block.Code == "else");
        var generatedCode = root.ToCode();

        Assert.IsNotNull(lineComment, generatedCode);
        Assert.IsNotNull(elseIf, generatedCode);
        Assert.AreEqual(CodeBlockType.LComment, lineComment!.Type, generatedCode);
        Assert.AreSame(elseIf!.Parent, lineComment.Parent, generatedCode);
        Assert.AreSame(elseIf, lineComment.Next, generatedCode);
        Assert.IsTrue(generatedCode.IndexOf("// Keep this line comment", StringComparison.Ordinal)
            < generatedCode.IndexOf("else if (b2)", StringComparison.Ordinal), generatedCode);
    }

    [TestMethod]
    public void TestUnnecessaryBlockRemoval_LeavesMalformedNestedBlockBoundariesUnchanged()
    {
        var root = CreateBlock(CodeBlockType.MainBlock, "root");
        var conditional = CreateBlock(CodeBlockType.Operation, "if (b1)", root);
        var outerOpen = CreateBlock(CodeBlockType.Block, "{", conditional);
        var nestedOpen = CreateBlock(CodeBlockType.Block, "{", outerOpen);
        var nestedClose = CreateBlock(CodeBlockType.Block, "}", outerOpen);
        var secondNestedOpen = CreateBlock(CodeBlockType.Block, "{", outerOpen);
        var secondNestedClose = CreateBlock(CodeBlockType.Block, "}", outerOpen);
        var outerClose = CreateBlock(CodeBlockType.Block, "}", conditional);

        new CodeOptimizer().OptimizeIfChains(root);

        Assert.AreEqual("if (b1)", conditional.Code);
        Assert.AreSame(conditional, outerOpen.Parent);
        Assert.AreSame(outerOpen, nestedOpen.Parent);
        Assert.AreSame(outerOpen, nestedClose.Parent);
        Assert.AreSame(outerOpen, secondNestedOpen.Parent);
        Assert.AreSame(outerOpen, secondNestedClose.Parent);
        Assert.AreSame(conditional, outerClose.Parent);
    }

    [TestMethod]
    public void TestUnnecessaryBlockRemoval_MovesLineCommentOutOfNestedWrapper()
    {
        var root = CreateBlock(CodeBlockType.MainBlock, "private void Test(bool b1, bool b2)");
        var conditional = CreateBlock(CodeBlockType.Operation, "if (b1)", root);
        var outerOpen = CreateBlock(CodeBlockType.Block, "{", conditional);
        var lineComment = CreateBlock(CodeBlockType.LComment, "// nested note", outerOpen);
        _ = CreateBlock(CodeBlockType.Block, "{", outerOpen);
        var innerIf = CreateBlock(CodeBlockType.Operation, "if (b2)", outerOpen);
        _ = CreateBlock(CodeBlockType.Operation, "SomeFunction();", innerIf);
        _ = CreateBlock(CodeBlockType.Block, "}", outerOpen);
        _ = CreateBlock(CodeBlockType.Block, "}", conditional);

        new CodeOptimizer().OptimizeIfChains(root);
        var generatedCode = root.ToCode();

        Assert.AreEqual(CodeBlockType.LComment, lineComment.Type, generatedCode);
        Assert.AreSame(root, lineComment.Parent, generatedCode);
        Assert.AreSame(conditional, lineComment.Next, generatedCode);
        Assert.AreEqual("if ((b1) && (b2))", conditional.Code);
        Assert.IsTrue(generatedCode.IndexOf("// nested note", StringComparison.Ordinal)
            < generatedCode.IndexOf("if ((b1) && (b2))", StringComparison.Ordinal), generatedCode);
    }

    [TestMethod]
    public void Test24c_MergesNestedIfWithoutChangingGotoLabelSources()
    {
        var source = Resources.ResourceManager.GetString("Test24cDat_cs")
            ?? throw new InvalidOperationException("Code resource 'Test24cDat_cs' was not found.");
        var root = ParseWithoutLabelReduction(source);
        var gotoBlock = FindBlock(root, block => block.Type == CodeBlockType.Goto);
        Assert.IsNotNull(gotoBlock?.Destination);
        Assert.IsTrue(gotoBlock!.Destination!.TryGetTarget(out var label));
        var sourceCount = label.Sources.Count;

        new CodeOptimizer().OptimizeIfChains(root);

        Assert.AreEqual("if ((b1) && (b2))", gotoBlock.Parent?.Code);
        Assert.IsTrue(gotoBlock.Destination!.TryGetTarget(out var currentLabel));
        Assert.AreSame(label, currentLabel);
        Assert.AreEqual(sourceCount, currentLabel.Sources.Count);
        Assert.AreEqual("goto End;", gotoBlock.Code);
    }

    [TestMethod]
    public void OptimizeIfChains_DoesNotMergeNestedIfContainingLabel()
    {
        var root = CreateBlock(CodeBlockType.MainBlock, "private void Test(bool b1, bool b2)");
        var outerIf = CreateBlock(CodeBlockType.Operation, "if (b1)", root);
        var innerIf = CreateBlock(CodeBlockType.Operation, "if (b2)", outerIf);
        var label = CreateBlock(CodeBlockType.Label, "End:", innerIf);

        new CodeOptimizer().OptimizeIfChains(root);

        Assert.AreEqual("if (b1)", outerIf.Code);
        Assert.AreSame(outerIf, innerIf.Parent);
        Assert.AreSame(innerIf, label.Parent);
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
    public void RemoveSingleSourceLabels1_MergesNestedIfWithoutReorderingConditionEvaluation()
    {
        const string source = @"private void NestedAndCalls()
{
    if (FirstCondition() == true)
    {
        if (SecondCondition() == true)
        {
            SomeFunction();
        }
    }
}";

        var output = ParseAndOptimize(source).ToCode();

        Assert.IsTrue(output.Contains("if ((FirstCondition() == true) && (SecondCondition() == true))", StringComparison.Ordinal), output);
        Assert.IsTrue(output.IndexOf("FirstCondition()", StringComparison.Ordinal)
            < output.IndexOf("SecondCondition()", StringComparison.Ordinal), output);
        Assert.AreEqual(1, CountOccurrences(output, "FirstCondition()"));
        Assert.AreEqual(1, CountOccurrences(output, "SecondCondition()"));
    }

    [TestMethod]
    [DataRow(false, false)]
    [DataRow(false, true)]
    [DataRow(true, false)]
    [DataRow(true, true)]
    public void OptimizeIfChains_PreservesSideEffectAndShortCircuitTrace(bool firstResult, bool secondResult)
    {
        const string source = @"private void Test()
{
    if (probe.First)
    {
        if (probe.Second)
        {
            probe.Action();
        }
    }
}";
        var root = ParseWithoutLabelReduction(source);
        new CodeOptimizer().OptimizeIfChains(root);
        var optimizedCode = root.ToCode();
        Assert.IsTrue(optimizedCode.Contains("if ((probe.First) && (probe.Second))", StringComparison.Ordinal), optimizedCode);

        var nestedProbe = new BooleanProbe(firstResult, secondResult);
        if (nestedProbe.First)
        {
            if (nestedProbe.Second)
                nestedProbe.Action();
        }

        var mergedProbe = new BooleanProbe(firstResult, secondResult);
        if (mergedProbe.First && mergedProbe.Second)
            mergedProbe.Action();

        CollectionAssert.AreEqual(nestedProbe.Trace, mergedProbe.Trace);
        Assert.AreEqual(1, nestedProbe.FirstReads);
        Assert.AreEqual(1, mergedProbe.FirstReads);
        Assert.AreEqual(firstResult ? 1 : 0, nestedProbe.SecondReads);
        Assert.AreEqual(firstResult ? 1 : 0, mergedProbe.SecondReads);
        Assert.AreEqual(firstResult && secondResult ? 1 : 0, nestedProbe.ActionCalls);
        Assert.AreEqual(firstResult && secondResult ? 1 : 0, mergedProbe.ActionCalls);
    }

    [TestMethod]
    public void OptimizeIfChains_MergesNestedIfWithoutBlockDelimiters()
    {
        var root = CreateBlock(CodeBlockType.MainBlock, "private void Test(bool b1, bool b2)");
        var outerIf = CreateBlock(CodeBlockType.Operation, "if (b1)", root);
        var innerIf = CreateBlock(CodeBlockType.Operation, "if (b2)", outerIf);
        _ = CreateBlock(CodeBlockType.Operation, "SomeFunction();", innerIf);

        new CodeOptimizer().OptimizeIfChains(root);

        Assert.AreEqual("if ((b1) && (b2))", outerIf.Code);
        Assert.AreEqual(1, outerIf.SubBlocks.Count);
        Assert.AreEqual("SomeFunction();", outerIf.SubBlocks[0].Code);
    }

    [TestMethod]
    public void OptimizeIfChains_MergesNestedIfAndRemovesRedundantOuterBody()
    {
        var root = CreateBlock(CodeBlockType.MainBlock, "private void Test(bool b1, bool b2)");
        var outerIf = CreateBlock(CodeBlockType.Operation, "if (b1)", root);
        var outerOpen = CreateBlock(CodeBlockType.Block, "{", outerIf);
        _ = CreateBlock(CodeBlockType.Comment, "/* keep scope */", outerIf);
        var innerIf = CreateBlock(CodeBlockType.Operation, "if (b2)", outerIf);
        var innerOpen = CreateBlock(CodeBlockType.Block, "{", innerIf);
        var firstAction = CreateBlock(CodeBlockType.Operation, "First();", innerIf);
        var secondAction = CreateBlock(CodeBlockType.Operation, "Second();", innerIf);
        var innerClose = CreateBlock(CodeBlockType.Block, "}", innerIf);
        var outerClose = CreateBlock(CodeBlockType.Block, "}", outerIf);

        new CodeOptimizer().OptimizeIfChains(root);
        var output = root.ToCode();

        Assert.AreEqual("if ((b1) && (b2))", outerIf.Code);
        Assert.IsNull(innerIf.Parent);
        Assert.IsNull(outerOpen.Parent, output);
        Assert.IsNull(outerClose.Parent, output);
        Assert.AreSame(outerIf, innerOpen.Parent, output);
        Assert.AreSame(outerIf, innerClose.Parent, output);
        Assert.AreSame(outerIf, firstAction.Parent, output);
        Assert.AreSame(outerIf, secondAction.Parent, output);
    }

    [TestMethod]
    public void OptimizeIfChains_MergesLongNestedIfChainLeftAssociatively()
    {
        var root = CreateBlock(CodeBlockType.MainBlock, "private void Test(bool b1, bool b2, bool b3)");
        var firstIf = CreateBlock(CodeBlockType.Operation, "if (b1)", root);
        var secondIf = CreateBlock(CodeBlockType.Operation, "if (b2)", firstIf);
        _ = CreateBlock(CodeBlockType.Operation, "if (b3)", secondIf);

        new CodeOptimizer().OptimizeIfChains(root);

        Assert.AreEqual("if (((b1) && (b2)) && (b3))", firstIf.Code);
        Assert.IsNull(secondIf.Parent);
    }

    [TestMethod]
    public void OptimizeIfChains_MergesThreeParsedNestedIfStatementsLeftAssociatively()
    {
        const string source = @"private void NestedAndThree(bool b1, bool b2, bool b3)
{
    if (b1)
    {
        if (b2)
        {
            if (b3)
            {
                E();
            }
        }
    }
}";

        var root = ParseWithoutLabelReduction(source);
        new CodeOptimizer().OptimizeIfChains(root);
        var output = root.ToCode();

        Assert.IsTrue(output.Contains("if (((b1) && (b2)) && (b3))", StringComparison.Ordinal), output);
        Assert.AreEqual(1, CountOccurrences(output, "(b1)"));
        Assert.AreEqual(1, CountOccurrences(output, "(b2)"));
        Assert.AreEqual(1, CountOccurrences(output, "(b3)"));
        Assert.AreEqual(1, CountOccurrences(output, "E();"));
    }

    [TestMethod]
    [DataRow("b1", "b2", "if ((b1) && (b2))", DisplayName = "Merges boolean parameters")]
    [DataRow("GetFirstCondition() != false", "GetSecondCondition() == true",
        "if ((GetFirstCondition() != false) && (GetSecondCondition() == true))",
        DisplayName = "Merges negated equality conditions in source order")]
    [DataRow("GetFirstCondition() == (true == false)", "(GetSecondCondition() == false) == false",
        "if ((GetFirstCondition() == (true == false)) && ((GetSecondCondition() == false) == false))",
        DisplayName = "Merges equality conditions with nested operands")]
    [DataRow("GetFirstCharacter() == 'A'", "GetSecondCharacter() == 'B'",
        "if ((GetFirstCharacter() == 'A') && (GetSecondCharacter() == 'B'))",
        DisplayName = "Merges character equality conditions")]
    [DataRow("true", "false", "if ((true) && (false))", DisplayName = "Merges boolean literals")]
    [DataRow("COND.FamNrTable.NoMatch", "!COND.FamNrTable.NoMatch",
        "if ((COND.FamNrTable.NoMatch) && (!COND.FamNrTable.NoMatch))",
        DisplayName = "Merges qualified boolean member and its negation")]
    [DataRow("num17 > 0", "(double)num17 < Conversion.Val(COND.aus[83])",
        "if ((num17 > 0) && ((double)num17 < Conversion.Val(COND.aus[83])))",
        DisplayName = "Merges greater-than and less-than comparisons")]
    [DataRow("value >= lowerBound", "value <= upperBound",
        "if ((value >= lowerBound) && (value <= upperBound))",
        DisplayName = "Merges inclusive relational comparisons")]
    [DataRow("!Information.IsDBNull(RuntimeHelpers.GetObjectValue(field))",
        "Operators.ConditionalCompareObjectGreater(field, 0, TextCompare: false)",
        "if ((!Information.IsDBNull(RuntimeHelpers.GetObjectValue(field))) && (Operators.ConditionalCompareObjectGreater(field, 0, TextCompare: false)))",
        DisplayName = "Merges whitelisted Visual Basic runtime boolean predicates")]
    [DataRow("Conversions.ToBoolean(flag)", "Operators.ConditionalCompareObjectEqual(value, 1, TextCompare: false)",
        "if ((Conversions.ToBoolean(flag)) && (Operators.ConditionalCompareObjectEqual(value, 1, TextCompare: false)))",
        DisplayName = "Merges Visual Basic boolean conversion and equality predicates")]
    [DataRow("Operators.ConditionalCompareObjectNotEqual(value, 0, TextCompare: false)",
        "Operators.ConditionalCompareObjectLess(value, limit, TextCompare: false)",
        "if ((Operators.ConditionalCompareObjectNotEqual(value, 0, TextCompare: false)) && (Operators.ConditionalCompareObjectLess(value, limit, TextCompare: false)))",
        DisplayName = "Merges inequality and less-than comparison helpers")]
    [DataRow("Operators.ConditionalCompareObjectGreaterOrEqual(value, lower, TextCompare: false)",
        "Operators.ConditionalCompareObjectLessOrEqual(value, upper, TextCompare: false)",
        "if ((Operators.ConditionalCompareObjectGreaterOrEqual(value, lower, TextCompare: false)) && (Operators.ConditionalCompareObjectLessOrEqual(value, upper, TextCompare: false)))",
        DisplayName = "Merges inclusive Visual Basic comparison helpers")]
    [DataRow("(b1) & (b2)", "b3", "if (((b1) & (b2)) && (b3))",
        DisplayName = "Merges validated boolean bitwise operands")]
    [DataRow("(b1) | (b2)", "b3", "if (((b1) | (b2)) && (b3))",
        DisplayName = "Merges validated boolean bitwise alternatives")]
    [DataRow("flags[index]", "!flags[otherIndex]",
        "if ((flags[index]) && (!flags[otherIndex]))",
        DisplayName = "Merges boolean indexer paths and negation")]
    [DataRow("probe . Ready", "probe.OtherReady",
        "if ((probe . Ready) && (probe.OtherReady))",
        DisplayName = "Merges member paths with whitespace around the separator")]
    [DataRow("flags[indices[index]]", "flags[\"active\"]",
        "if ((flags[indices[index]]) && (flags[\"active\"]))",
        DisplayName = "Merges nested indexers while ignoring quoted operator characters")]
    [DataRow("_probe.Ready", "!_probe.OtherReady",
        "if ((_probe.Ready) && (!_probe.OtherReady))",
        DisplayName = "Merges underscore-prefixed qualified boolean members")]
    [DataRow("GetText(\"a\\\"> b\") == expected", "otherLabel != \">\"",
        "if ((GetText(\"a\\\"> b\") == expected) && (otherLabel != \">\"))",
        DisplayName = "Skips escaped quotes while scanning a string before comparisons")]
    [DataRow("label == @\"a > b\"", "otherLabel != \">\"",
        "if ((label == @\"a > b\") && (otherLabel != \">\"))",
        DisplayName = "Ignores relational characters inside quoted literals")]
    [DataRow("label == @\"a\"\" > b\"", "otherLabel != \">\"",
        "if ((label == @\"a\"\" > b\") && (otherLabel != \">\"))",
        DisplayName = "Skips doubled quotes inside verbatim string literals")]
    public void OptimizeIfChains_MergesSupportedConditions(
        string firstCondition,
        string secondCondition,
        string expectedCondition)
    {
        var root = CreateBlock(CodeBlockType.MainBlock, "private void Test(bool b1, bool b2, bool b3)");
        var firstIf = CreateBlock(CodeBlockType.Operation, $"if ({firstCondition})", root);
        var secondIf = CreateBlock(CodeBlockType.Operation, $"if ({secondCondition})", firstIf);
        _ = CreateBlock(CodeBlockType.Operation, "SomeFunction();", secondIf);

        new CodeOptimizer().OptimizeIfChains(root);

        Assert.AreEqual(expectedCondition, firstIf.Code);
    }

    [TestMethod]
    [DataRow("root", "if (b1)", "if (b2)", DisplayName = "Rejects unresolved boolean identifiers")]
    [DataRow("private void Test(bool b1, bool b2)", "if (b1 = b2)", "if (b2)",
        DisplayName = "Rejects a single assignment operator")]
    [DataRow("private void Test(bool b1)", "if (GetUnknownCondition())", "if (b1)",
        DisplayName = "Rejects an unknown method return type")]
    [DataRow("private void Test(bool b1)", "if (GetCondition(\"a > b\"))", "if (b1)",
        DisplayName = "Does not treat comparison text inside a string as boolean proof")]
    [DataRow("private void Test(bool b1)", "if (value == \"unterminated)", "if (b1)",
        DisplayName = "Rejects an unterminated string literal before comparison proof")]
    [DataRow("private void Test(bool b1)", "if (flags[])", "if (b1)",
        DisplayName = "Rejects an empty indexer")]
    [DataRow("private void Test(bool b1)", "if (flags[unfinished)", "if (b1)",
        DisplayName = "Rejects an unterminated indexer")]
    [DataRow("private void Test(bool b1)", "if (probe.)", "if (b1)",
        DisplayName = "Rejects an incomplete member path")]
    [DataRow("private void Test(bool b1)", "if (Information.IsDBNull(value) extra)", "if (b1)",
        DisplayName = "Rejects a whitelisted invocation with trailing expression text")]
    [DataRow("private void Test(bool b1)", "if (Information.IsDBNull(value)", "if (b1)",
        DisplayName = "Rejects an unterminated whitelisted invocation")]
    [DataRow("private void Test(bool b1)", "if (!)", "if (b1)",
        DisplayName = "Rejects an empty negated expression")]
    [DataRow("private void Test(bool b1)", "if (b1 + 1)", "if (b1)",
        DisplayName = "Rejects an expression without a boolean operator")]
    [DataRow("private void Test(bool b1)", "if (b1 ==)", "if (b1)",
        DisplayName = "Rejects a comparison with a missing right operand")]
    [DataRow("private void Test(bool b1)", "if (b1 =)", "if (b1)",
        DisplayName = "Rejects a comparison character at the end of the expression")]
    [DataRow("private void Test(bool b1)", "if (> b1)", "if (b1)",
        DisplayName = "Rejects a comparison with a missing left operand")]
    [DataRow("private void Test(bool b1)", "if (b1 >)", "if (b1)",
        DisplayName = "Rejects a relational comparison with a missing right operand")]
    [DataRow("private void Test(bool b1)", "if (left ! right)", "if (b1)",
        DisplayName = "Rejects a standalone exclamation mark instead of equality")]
    [DataRow("private void Test(bool b1)", "if (left >> 1)", "if (b1)",
        DisplayName = "Rejects shift operators instead of treating them as relational comparisons")]
    [DataRow("private void Test(bool b1)", "if (Information.IsDBNull.)", "if (b1)",
        DisplayName = "Rejects an incomplete qualified whitelisted method name")]
    [DataRow("private void Test(bool b1)", "if (Information.IsDBNull extra)", "if (b1)",
        DisplayName = "Rejects a whitelisted method name followed by non-invocation text")]
    [DataRow("private void Test(bool b1)", "if (b1)", "if (b2",
        DisplayName = "Rejects a malformed inner condition")]
    public void OptimizeIfChains_RejectsUnprovenOrMalformedConditions(
        string rootCode,
        string firstCondition,
        string secondCondition)
    {
        var root = CreateBlock(CodeBlockType.MainBlock, rootCode);
        var outerIf = CreateBlock(CodeBlockType.Operation, firstCondition, root);
        var innerIf = CreateBlock(CodeBlockType.Operation, secondCondition, outerIf);
        _ = CreateBlock(CodeBlockType.Operation, "SomeFunction();", innerIf);

        new CodeOptimizer().OptimizeIfChains(root);

        Assert.AreEqual(firstCondition, outerIf.Code);
        Assert.AreEqual(secondCondition, innerIf.Code);
    }

    [TestMethod]
    public void RemoveSingleSourceLabels1_LeavesUnbracedNestedIfUnchangedAndIsIdempotent()
    {
        const string source = @"private void UnbracedAnd(bool b1, bool b2)
{
    if (b1)
        if (b2)
            SomeFunction();
}";

        var root = ParseWithoutLabelReduction(source);
        var optimizer = new CodeOptimizer();
        optimizer.OptimizeIfChains(root);
        var firstOutput = root.ToCode();
        optimizer.OptimizeIfChains(root);

        Assert.IsTrue(firstOutput.Contains("if (b1) if (b2)", StringComparison.Ordinal), firstOutput);
        Assert.AreEqual(firstOutput, root.ToCode());
    }

    [TestMethod]
    public void RemoveSingleSourceLabels1_MergesElseIfWithoutReorderingConditionEvaluation()
    {
        const string source = @"private void ElseIfCalls()
{
    if (FirstCondition() == true)
    {
        SomeFunction();
    }
    else if (SecondCondition() == true)
    {
        SomeFunction();
    }
}";

        var output = ParseAndOptimize(source).ToCode();

        Assert.IsTrue(output.Contains("if ((FirstCondition() == true) || (SecondCondition() == true))", StringComparison.Ordinal), output);
        Assert.IsTrue(output.IndexOf("FirstCondition()", StringComparison.Ordinal)
            < output.IndexOf("SecondCondition()", StringComparison.Ordinal), output);
        Assert.AreEqual(1, CountOccurrences(output, "FirstCondition()"));
        Assert.AreEqual(1, CountOccurrences(output, "SecondCondition()"));
        Assert.AreEqual(1, CountOccurrences(output, "SomeFunction()"));
    }

    [TestMethod]
    public void RemoveSingleSourceLabels1_MergesNestedIfWithCompoundCondition()
    {
        const string source = @"private void NestedAndCompound(bool b1, bool b2, bool b3)
{
    if (b1)
    {
        if (b2 && b3)
        {
            SomeCode();
        }
    }
}";

        var output = ParseAndOptimize(source).ToCode();

        Assert.IsTrue(output.Contains("if ((b1) && (b2 && b3))", StringComparison.Ordinal), output);
        Assert.AreEqual(1, CountOccurrences(output, "SomeCode()"));
    }

    [TestMethod]
    public void RemoveSingleSourceLabels1_DoesNotMergeUnprovenMethodCallConditions()
    {
        const string source = @"private void UnknownConditionTypes()
{
    if (FirstCondition())
    {
        if (SecondCondition())
        {
            SomeFunction();
        }
    }
}";

        var output = ParseAndOptimize(source).ToCode();

        Assert.IsTrue(output.Contains("if (FirstCondition())", StringComparison.Ordinal), output);
        Assert.IsTrue(output.Contains("if (SecondCondition())", StringComparison.Ordinal), output);
        Assert.IsFalse(output.Contains("&&", StringComparison.Ordinal), output);
    }

    [TestMethod]
    public void OptimizeIfChains_DoesNotTreatEqualityTextInsideStringAsBooleanProof()
    {
        const string source = @"private void UnknownStringCallCondition()
{
    if (GetCondition(""==""))
    {
        if (GetOtherCondition())
        {
            SomeFunction();
        }
    }
}";

        var output = ParseAndOptimize(source).ToCode();

        Assert.IsTrue(output.Contains("if (GetCondition(\"==\"))", StringComparison.Ordinal), output);
        Assert.IsTrue(output.Contains("if (GetOtherCondition())", StringComparison.Ordinal), output);
        Assert.IsFalse(output.Contains("&&", StringComparison.Ordinal), output);
    }

    [TestMethod]
    public void RemoveSingleSourceLabels1_DoesNotMergeNestedIfWithInnerElse()
    {
        const string source = @"private void NestedAndInnerElse(bool b1, bool b2)
{
    if (b1)
    {
        if (b2)
            SomeCode();
        else
            SomeOtherCode();
    }
}";

        var output = ParseAndOptimize(source).ToCode();

        Assert.IsTrue(output.Contains("if (b1)", StringComparison.Ordinal), output);
        Assert.IsTrue(output.Contains("if (b2)", StringComparison.Ordinal), output);
        Assert.IsTrue(output.Contains("SomeOtherCode();", StringComparison.Ordinal), output);
        Assert.IsFalse(output.Contains("&&", StringComparison.Ordinal), output);
    }

    [TestMethod]
    [DataRow("else", DisplayName = "Rejects a nested else")]
    [DataRow("switch (b3)", DisplayName = "Rejects a nested switch")]
    [DataRow("while (b3)", DisplayName = "Rejects a nested while loop")]
    [DataRow("for (;; )", DisplayName = "Rejects a nested for loop")]
    [DataRow("do", DisplayName = "Rejects a nested do loop")]
    [DataRow("return;", DisplayName = "Rejects return")]
    [DataRow("throw new Exception();", DisplayName = "Rejects throw")]
    [DataRow("break;", DisplayName = "Rejects break")]
    [DataRow("continue;", DisplayName = "Rejects continue")]
    public void OptimizeIfChains_RejectsNestedControlFlow(string statement)
    {
        var root = CreateBlock(CodeBlockType.MainBlock, "root");
        var outerIf = CreateBlock(CodeBlockType.Operation, "if (b1)", root);
        var innerIf = CreateBlock(CodeBlockType.Operation, "if (b2)", outerIf);
        _ = CreateBlock(CodeBlockType.Operation, statement, innerIf);

        new CodeOptimizer().OptimizeIfChains(root);

        Assert.AreEqual("if (b1)", outerIf.Code);
        Assert.AreEqual("if (b2)", innerIf.Code);
    }

    [TestMethod]
    public void OptimizeIfChains_IgnoresNestedCommentsWhenComparingBodiesAndMergesThem()
    {
        var tree = CreateElseIfTree();
        var firstCall = tree.FirstIf.SubBlocks[0];
        var secondCall = tree.SecondIf.SubBlocks[0];
        _ = CreateBlock(CodeBlockType.Block, "{", firstCall);
        var firstNestedCall = CreateBlock(CodeBlockType.Operation, "NestedCall();", firstCall);
        _ = CreateBlock(CodeBlockType.Block, "}", firstCall);
        _ = CreateBlock(CodeBlockType.Block, "{", secondCall);
        var secondNestedCall = CreateBlock(CodeBlockType.Operation, "NestedCall();", secondCall);
        _ = CreateBlock(CodeBlockType.Block, "}", secondCall);
        _ = CreateBlock(CodeBlockType.Comment, "// first nested note", firstNestedCall);
        _ = CreateBlock(CodeBlockType.Comment, "// second nested note", secondNestedCall);

        new CodeOptimizer().OptimizeIfChains(tree.Root);
        var output = tree.Root.ToCode();

        Assert.AreEqual("if ((b1) || (b2))", tree.FirstIf.Code);
        Assert.IsTrue(output.Contains("first nested note", StringComparison.Ordinal), output);
        Assert.IsTrue(output.Contains("second nested note", StringComparison.Ordinal), output);
        Assert.AreEqual(1, CountOccurrences(output, "SomeFunction()"));
        Assert.AreEqual(1, CountOccurrences(output, "NestedCall()"));
    }

    [TestMethod]
    public void OptimizeIfChains_MergesTrailingCommentWithoutBlockEndExactlyOnce()
    {
        var root = CreateBlock(CodeBlockType.MainBlock, "private void Test(bool b1, bool b2)");
        var firstIf = CreateBlock(CodeBlockType.Operation, "if (b1)", root);
        _ = CreateBlock(CodeBlockType.Operation, "SomeFunction();", firstIf);
        _ = CreateBlock(CodeBlockType.Operation, "else", root);
        var secondIf = CreateBlock(CodeBlockType.Operation, "if (b2)", root);
        _ = CreateBlock(CodeBlockType.Operation, "SomeFunction();", secondIf);
        _ = CreateBlock(CodeBlockType.FLComment, "// trailing note", secondIf);

        new CodeOptimizer().OptimizeIfChains(root);
        var output = root.ToCode();

        Assert.AreEqual("if ((b1) || (b2))", firstIf.Code);
        Assert.AreEqual(1, CountOccurrences(output, "trailing note"));
        Assert.AreEqual(1, CountOccurrences(output, "SomeFunction()"));
    }

    [TestMethod]
    public void OptimizeIfChains_MergesEquivalentNestedCallBodies()
    {
        var root = CreateBlock(CodeBlockType.MainBlock, "private void Test(bool b1, bool b2)");
        var firstIf = CreateBlock(CodeBlockType.Operation, "if (b1)", root);
        var firstCall = CreateBlock(CodeBlockType.Operation, "SomeFunction();", firstIf);
        _ = CreateBlock(CodeBlockType.Operation, "NestedCall();", firstCall);
        _ = CreateBlock(CodeBlockType.Operation, "else", root);
        var secondIf = CreateBlock(CodeBlockType.Operation, "if (b2)", root);
        var secondCall = CreateBlock(CodeBlockType.Operation, "SomeFunction();", secondIf);
        _ = CreateBlock(CodeBlockType.Operation, "NestedCall();", secondCall);

        new CodeOptimizer().OptimizeIfChains(root);

        Assert.AreEqual("if ((b1) || (b2))", firstIf.Code);
        Assert.IsNull(secondIf.Parent);
    }

    [TestMethod]
    public void OptimizeIfChains_DoesNotMergeDifferentNestedCallBodies()
    {
        var root = CreateBlock(CodeBlockType.MainBlock, "private void Test(bool b1, bool b2)");
        var firstIf = CreateBlock(CodeBlockType.Operation, "if (b1)", root);
        var firstCall = CreateBlock(CodeBlockType.Operation, "SomeFunction();", firstIf);
        _ = CreateBlock(CodeBlockType.Operation, "NestedCall();", firstCall);
        _ = CreateBlock(CodeBlockType.Operation, "else", root);
        var secondIf = CreateBlock(CodeBlockType.Operation, "if (b2)", root);
        var secondCall = CreateBlock(CodeBlockType.Operation, "SomeFunction();", secondIf);
        _ = CreateBlock(CodeBlockType.Operation, "OtherNestedCall();", secondCall);

        new CodeOptimizer().OptimizeIfChains(root);

        Assert.AreEqual("if (b1)", firstIf.Code);
        Assert.AreSame(root, secondIf.Parent);
    }

    [TestMethod]
    public void OptimizeIfChains_DoesNotMergeEquivalentBodiesWithDifferentBlockTypes()
    {
        var tree = CreateElseIfTree(
            firstBodyType: CodeBlockType.Operation,
            secondBodyType: CodeBlockType.Variable);

        new CodeOptimizer().OptimizeIfChains(tree.Root);

        Assert.AreEqual("if (b1)", tree.FirstIf.Code);
        Assert.AreSame(tree.Root, tree.SecondIf.Parent);
    }

    [TestMethod]
    public void OptimizeIfChains_DoesNotMergeWhenFirstBodyIsNotAnOperation()
    {
        var tree = CreateElseIfTree(firstBodyType: CodeBlockType.Variable);

        new CodeOptimizer().OptimizeIfChains(tree.Root);

        Assert.AreEqual("if (b1)", tree.FirstIf.Code);
        Assert.AreSame(tree.Root, tree.SecondIf.Parent);
    }

    [TestMethod]
    public void OptimizeIfChains_DoesNotMergeEquivalentBodiesWithDifferentChildCounts()
    {
        var tree = CreateElseIfTree(firstNestedCode: "NestedCall();");

        new CodeOptimizer().OptimizeIfChains(tree.Root);

        Assert.AreEqual("if (b1)", tree.FirstIf.Code);
        Assert.AreSame(tree.Root, tree.SecondIf.Parent);
    }

    [TestMethod]
    public void OptimizeIfChains_DoesNotMergeEmptyBodies()
    {
        var tree = CreateElseIfTree(firstBodyCode: null, secondBodyCode: null);

        new CodeOptimizer().OptimizeIfChains(tree.Root);

        Assert.AreEqual("if (b1)", tree.FirstIf.Code);
        Assert.AreSame(tree.Root, tree.SecondIf.Parent);
    }

    [TestMethod]
    public void OptimizeIfChains_DoesNotMergeMalformedNonInvocationStatements()
    {
        var tree = CreateElseIfTree(firstBodyCode: "SomeFunction);", secondBodyCode: "SomeFunction);");

        new CodeOptimizer().OptimizeIfChains(tree.Root);

        Assert.AreEqual("if (b1)", tree.FirstIf.Code);
    }

    [TestMethod]
    public void OptimizeIfChains_DoesNotMergeMalformedAssignmentLikeStatements()
    {
        var tree = CreateElseIfTree(
            firstBodyCode: "SomeFunction() = value);",
            secondBodyCode: "SomeFunction() = value);");

        new CodeOptimizer().OptimizeIfChains(tree.Root);

        Assert.AreEqual("if (b1)", tree.FirstIf.Code);
    }

    [TestMethod]
    public void OptimizeIfChains_DoesNotMergeMalformedSecondElseIfCondition()
    {
        var tree = CreateElseIfTree(secondCondition: "if (b2");

        new CodeOptimizer().OptimizeIfChains(tree.Root);

        Assert.AreEqual("if (b1)", tree.FirstIf.Code);
        Assert.AreSame(tree.Root, tree.SecondIf.Parent);
    }

    [TestMethod]
    public void OptimizeIfChains_MergesElseIfWithLogicalCondition()
    {
        var tree = CreateElseIfTree(secondCondition: "if (b2 || b3)");

        new CodeOptimizer().OptimizeIfChains(tree.Root);

        Assert.AreEqual("if ((b1) || (b2 || b3))", tree.FirstIf.Code);
        Assert.IsNull(tree.SecondIf.Parent);
    }

    [TestMethod]
    [DataRow(CodeBlockType.Goto, "goto End;", DisplayName = "Rejects a typed goto in equivalent OR bodies")]
    [DataRow(CodeBlockType.Label, "End:", DisplayName = "Rejects a label in equivalent OR bodies")]
    [DataRow(CodeBlockType.Operation, "goto End;", DisplayName = "Rejects a goto operation in equivalent OR bodies")]
    public void OptimizeIfChains_RejectsGotoAndLabelsInOrBodies(CodeBlockType bodyType, string bodyCode)
    {
        var tree = CreateElseIfTree(firstBodyType: bodyType, secondBodyType: bodyType,
            firstBodyCode: bodyCode, secondBodyCode: bodyCode);

        new CodeOptimizer().OptimizeIfChains(tree.Root);

        Assert.AreEqual("if (b1)", tree.FirstIf.Code);
        Assert.AreSame(tree.Root, tree.SecondIf.Parent);
    }

    [TestMethod]
    public void OptimizeIfChains_IgnoresFirstBranchCommentWhenComparingBodies()
    {
        var tree = CreateElseIfTree();
        _ = CreateBlock(CodeBlockType.Comment, "// first-only", tree.FirstIf);
        var secondBody = tree.SecondIf.SubBlocks[0];
        var secondComment = CreateBlock(CodeBlockType.Comment, "// second-only", tree.SecondIf);
        _ = tree.SecondIf.SubBlocks.Remove(secondComment);
        tree.SecondIf.SubBlocks.Insert(secondBody.Index, secondComment);

        new CodeOptimizer().OptimizeIfChains(tree.Root);

        Assert.AreEqual("if ((b1) || (b2))", tree.FirstIf.Code);
        Assert.IsNull(tree.SecondIf.Parent);
        Assert.IsTrue(tree.FirstIf.ToCode().Contains("first-only", StringComparison.Ordinal));
        Assert.AreEqual(1, CountOccurrences(tree.FirstIf.ToCode(), "second-only"));
    }

    [TestMethod]
    public void OptimizeIfChains_LeavesNonDelimiterBlockInNestedBranchUnchanged()
    {
        var root = CreateBlock(CodeBlockType.MainBlock, "private void Test(bool b1, bool b2)");
        var outerIf = CreateBlock(CodeBlockType.Operation, "if (b1)", root);
        _ = CreateBlock(CodeBlockType.Block, "scope", outerIf);
        var innerIf = CreateBlock(CodeBlockType.Operation, "if (b2)", outerIf);

        new CodeOptimizer().OptimizeIfChains(root);

        Assert.AreEqual("if (b1)", outerIf.Code);
        Assert.AreSame(outerIf, innerIf.Parent);
    }

    [TestMethod]
    public void OptimizeIfChains_RejectsElseNodeWithExecutableChildren()
    {
        var root = CreateBlock(CodeBlockType.MainBlock, "private void Test(bool b1, bool b2)");
        var firstIf = CreateBlock(CodeBlockType.Operation, "if (b1)", root);
        _ = CreateBlock(CodeBlockType.Operation, "SomeFunction();", firstIf);
        var elseBlock = CreateBlock(CodeBlockType.Operation, "else", root);
        _ = CreateBlock(CodeBlockType.Operation, "Unexpected();", elseBlock);
        var secondIf = CreateBlock(CodeBlockType.Operation, "if (b2)", root);
        _ = CreateBlock(CodeBlockType.Operation, "SomeFunction();", secondIf);

        new CodeOptimizer().OptimizeIfChains(root);

        Assert.AreEqual("if (b1)", firstIf.Code);
        Assert.AreSame(root, elseBlock.Parent);
        Assert.AreSame(root, secondIf.Parent);
    }

    [TestMethod]
    [DataRow("if (b3) { SomeFunction(); }")]
    [DataRow("else")]
    [DataRow("switch (b3) { case 1: SomeFunction(); break; }")]
    [DataRow("while (b3) { SomeFunction(); }")]
    [DataRow("for (;; ) { SomeFunction(); }")]
    [DataRow("do { SomeFunction(); } while (b3);")]
    [DataRow("return;")]
    [DataRow("return value;")]
    [DataRow("throw new Exception();")]
    [DataRow("break;")]
    [DataRow("continue;")]
    public void OptimizeIfChains_DoesNotMergeElseIfWithControlFlowBody(string statement)
    {
        var tree = CreateElseIfTree(firstBodyCode: statement, secondBodyCode: statement);

        new CodeOptimizer().OptimizeIfChains(tree.Root);

        Assert.AreEqual("if (b1)", tree.FirstIf.Code);
        Assert.AreSame(tree.Root, tree.SecondIf.Parent);
    }

    [TestMethod]
    [DataRow("if (", DisplayName = "Rejects a missing condition")]
    [DataRow("if ()", DisplayName = "Rejects an empty condition")]
    [DataRow("if (b1", DisplayName = "Rejects an unclosed condition")]
    [DataRow("if b1", DisplayName = "Rejects a missing condition delimiter")]
    [DataRow("if (b1 && )", DisplayName = "Rejects a compound condition with an empty operand")]
    [DataRow("if (missing && b2)", DisplayName = "Rejects a compound condition with an undeclared operand")]
    public void OptimizeIfChains_LeavesMalformedIfConditionsUnchanged(string condition)
    {
        var root = CreateBlock(CodeBlockType.MainBlock, "private void Test(bool b1, bool b2)");
        var outerIf = CreateBlock(CodeBlockType.Operation, condition, root);
        _ = CreateBlock(CodeBlockType.Operation, "if (b2)", outerIf);

        new CodeOptimizer().OptimizeIfChains(root);

        Assert.AreEqual(condition, outerIf.Code);
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

    private sealed class BooleanProbe(bool firstResult, bool secondResult)
    {
        public List<string> Trace { get; } = new();
        public int FirstReads { get; private set; }
        public int SecondReads { get; private set; }
        public int ActionCalls { get; private set; }

        public bool First
        {
            get
            {
                FirstReads++;
                Trace.Add("first");
                return firstResult;
            }
        }

        public bool Second
        {
            get
            {
                SecondReads++;
                Trace.Add("second");
                return secondResult;
            }
        }

        public void Action()
        {
            ActionCalls++;
            Trace.Add("action");
        }
    }

    private static ICodeBlock ParseAndOptimize(string source)
    {
        var code = CreateCode(source);
        var root = code.Parse()
            ?? throw new InvalidOperationException("The parser should produce a root block.");
        code.RemoveSingleSourceLabels1(root);
        return root;
    }

    private static ICodeBlock ParseWithoutLabelReduction(string source)
    {
        return CreateCode(source).Parse()
            ?? throw new InvalidOperationException("The parser should produce a root block.");
    }

    private static CSCode CreateCode(string source)
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
        return code;
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

    private static int CountOccurrences(string value, string search)
    {
        var count = 0;
        var index = 0;
        while ((index = value.IndexOf(search, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += search.Length;
        }

        return count;
    }

    private static ICodeBlock? FindBlock(ICodeBlock root, Func<ICodeBlock, bool> predicate)
    {
        if (predicate(root))
            return root;

        foreach (var child in root.SubBlocks)
        {
            var result = FindBlock(child, predicate);
            if (result is not null)
                return result;
        }

        return null;
    }

    private static (CodeBlock Root, CodeBlock FirstIf, CodeBlock ElseBlock, CodeBlock SecondIf) CreateElseIfTree(
        string firstCondition = "if (b1)",
        string secondCondition = "if (b2)",
        CodeBlockType firstBodyType = CodeBlockType.Operation,
        CodeBlockType secondBodyType = CodeBlockType.Operation,
        string? firstBodyCode = "SomeFunction();",
        string? secondBodyCode = "SomeFunction();",
        string? firstNestedCode = null,
        string? secondNestedCode = null)
    {
        var root = CreateBlock(CodeBlockType.MainBlock, "private void Test(bool b1, bool b2, bool b3)");
        var firstIf = CreateBlock(CodeBlockType.Operation, firstCondition, root);
        if (firstBodyCode is not null)
        {
            var firstBody = CreateBlock(firstBodyType, firstBodyCode, firstIf);
            if (firstNestedCode is not null)
                _ = CreateBlock(CodeBlockType.Operation, firstNestedCode, firstBody);
        }

        var elseBlock = CreateBlock(CodeBlockType.Operation, "else", root);
        var secondIf = CreateBlock(CodeBlockType.Operation, secondCondition, root);
        if (secondBodyCode is not null)
        {
            var secondBody = CreateBlock(secondBodyType, secondBodyCode, secondIf);
            if (secondNestedCode is not null)
                _ = CreateBlock(CodeBlockType.Operation, secondNestedCode, secondBody);
        }

        return (root, firstIf, elseBlock, secondIf);
    }

    private static CodeBlock CreateBlock(CodeBlockType type, string code, ICodeBlock? parent = null)
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
