using System;
using System.Linq;
using TranspilerLib.CSharp.StatEqualCheck;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Models.Scanner;

namespace TranspilerLib.CSharp.StatEqualCheck.Tests;

[TestClass]
public sealed class StatEqualCheckTests
{
    [TestMethod]
    public void Compare_ParenthesesAndNegationPreserveConditionBehavior()
    {
        var result = StatEqualCheck.Compare(
            "if ((ready && !blocked)) { Run(); } else { Wait(); }",
            "if (ready && !(blocked)) { Run(); } else { Wait(); }");

        Assert.AreEqual(StatEqualStatus.Equivalent, result.Status);
        Assert.AreEqual(0, result.Findings.Count);
    }

    [TestMethod]
    public void Compare_LogicalAndMatchesNestedShortCircuitConditions()
    {
        var result = StatEqualCheck.Compare(
            "if (First() && Second()) { Run(); }",
            "if (First()) { if (Second()) { Run(); } }");

        Assert.AreEqual(StatEqualStatus.Equivalent, result.Status);
    }

    [TestMethod]
    public void Compare_GotoAndLabelRoutingAreStructural()
    {
        var result = StatEqualCheck.Compare(
            "goto Done; Unreachable(); Done: Finish();",
            "Finish();");

        Assert.AreEqual(StatEqualStatus.Equivalent, result.Status);
    }

    [TestMethod]
    public void Compare_SwitchAndIfWithEquivalentCasesMatch()
    {
        var result = StatEqualCheck.Compare(
            "switch (value) { case 1: Run(); break; default: Wait(); break; }",
            "if (value == 1) { Run(); } else { Wait(); }");

        Assert.AreEqual(StatEqualStatus.Equivalent, result.Status);
    }

    [TestMethod]
    [DataRow("==")]
    [DataRow("!=")]
    [DataRow("<")]
    [DataRow("<=")]
    [DataRow(">")]
    [DataRow(">=")]
    public void Compare_EqualityAndRelationalAtomsAreModeled(string operatorText)
    {
        var condition = $"if (left {operatorText} right) {{ Run(); }}";
        Assert.AreEqual(StatEqualStatus.Equivalent, StatEqualCheck.Compare(condition, condition).Status);
    }

    [TestMethod]
    [DataRow("source.Member")]
    [DataRow("source[index]")]
    [DataRow("Check()")]
    public void Compare_MemberIndexerAndCallAtomsAreModeled(string condition)
    {
        var source = $"if ({condition}) {{ Run(); }}";
        Assert.AreEqual(StatEqualStatus.Equivalent, StatEqualCheck.Compare(source, source).Status);
    }

    [TestMethod]
    public void Compare_ObservableStatementsMustMatch()
    {
        var result = StatEqualCheck.Compare("Send(1);", "Send(2);");
        Assert.AreEqual(StatEqualStatus.NotEquivalent, result.Status);
        Assert.IsTrue(result.Findings.Any(finding => finding.Code == "BEHAVIOR_MISMATCH"));
    }

    [TestMethod]
    public void Compare_LogicalOrMatchesNestedShortCircuitConditions()
    {
        var result = StatEqualCheck.Compare(
            "if (First() || Second()) { Run(); } else { Wait(); }",
            "if (First()) { Run(); } else if (Second()) { Run(); } else { Wait(); }");
        Assert.AreEqual(StatEqualStatus.Equivalent, result.Status);
    }

    [TestMethod]
    public void Compare_BitwiseOrEvaluatesBothAtomsOnEveryPath()
    {
        var result = StatEqualCheck.Compare(
            "if (First() | Second()) { Run(); }",
            "if (First() || Second()) { Run(); }");
        Assert.AreEqual(StatEqualStatus.NotEquivalent, result.Status);
    }

    [TestMethod]
    public void Compare_AtomEvaluationOrderIsSignificant()
    {
        var result = StatEqualCheck.Compare(
            "if (First() && Second()) { Run(); }",
            "if (Second() && First()) { Run(); }");
        Assert.AreEqual(StatEqualStatus.NotEquivalent, result.Status);
    }

    [TestMethod]
    public void Compare_MismatchFindingPointsToTheDifferingStatement()
    {
        var result = StatEqualCheck.Compare("First();\nLeft();", "First();\nRight();");
        var finding = result.Findings.Single(item => item.Code == "BEHAVIOR_MISMATCH");
        Assert.AreEqual(2, finding.LeftSpan?.Line);
        Assert.AreEqual(2, finding.RightSpan?.Line);
    }

    [TestMethod]
    public void Compare_UnmodeledStatementsCannotProveEquivalence()
    {
        var result = StatEqualCheck.Compare("for (;;) { Run(); }", "for (;;) { Run(); }");
        Assert.AreEqual(StatEqualStatus.Inconclusive, result.Status);
        Assert.IsTrue(result.Findings.Any(finding => finding.Severity == StatEqualFindingSeverity.Warning));
    }

    [TestMethod]
    public void Compare_StandaloneMethodDeclarationsAreParsedWithoutWrapperDiagnostics()
    {
        var result = StatEqualCheck.Compare("void Left() { Run(); }", "void Right() { Run(); }");
        Assert.AreEqual(StatEqualStatus.Equivalent, result.Status);
    }

    [TestMethod]
    public void Compare_BitwiseAndEvaluatesBothAtomsOnEveryPath()
    {
        var result = StatEqualCheck.Compare(
            "if (First() & Second()) { Run(); }",
            "if (First() && Second()) { Run(); }");
        Assert.AreEqual(StatEqualStatus.NotEquivalent, result.Status);
    }

    [TestMethod]
    public void Compare_WhileConditionParenthesesDoNotChangeTheFlowGraph()
    {
        var result = StatEqualCheck.Compare(
            "while ((ready)) { Run(); }",
            "while (ready) { Run(); }");
        Assert.AreEqual(StatEqualStatus.Equivalent, result.Status);
    }

    [TestMethod]
    public void Compare_CodeBlockTreesUsesPublicOverload()
    {
        var left = CreateRoot("Run();");
        var right = CreateRoot("Run();");
        Assert.AreEqual(StatEqualStatus.Equivalent, StatEqualCheck.Compare(left, right).Status);
    }

    [TestMethod]
    public void Compare_SuppliedBlocksBecomeReturnLikeTerminalsPerInput()
    {
        var leftResume = CreateBlock("ResumeLeft();", "LeftDispatcherDetail();");
        var rightResume = CreateBlock("ResumeRight();", "RightDispatcherDetail();");
        var left = CreateRoot("Before();", leftResume, "LeftAfterDispatcher();");
        var right = CreateRoot("Before();", rightResume, "RightAfterDispatcher();");

        Assert.AreEqual(StatEqualStatus.NotEquivalent, StatEqualCheck.Compare(left, right).Status);
        var result = StatEqualCheck.Compare(left, right, [leftResume], [rightResume]);

        Assert.AreEqual(StatEqualStatus.Equivalent, result.Status);
        Assert.AreEqual("ResumeLeft();", leftResume.Code);
        Assert.AreEqual("ResumeRight();", rightResume.Code);
        Assert.AreEqual(1, leftResume.SubBlocks.Count);
        Assert.AreEqual(1, rightResume.SubBlocks.Count);
    }

    [TestMethod]
    public void Compare_ExplicitTerminalListsAreOptionalPerInputAndDisableDownstreamTraversal()
    {
        var leftTerminal = CreateBlock("LeftResumeNext();", "LeftDispatchDetail();");
        var rightTerminal = CreateBlock("RightResumeNext();", "RightDispatchDetail();");
        var left = CreateRoot("Before();", leftTerminal, "LeftAfter();");
        var right = CreateRoot("Before();", rightTerminal, "RightAfter();");

        Assert.AreEqual(StatEqualStatus.NotEquivalent, StatEqualCheck.Compare(left, right).Status);
        Assert.AreEqual(StatEqualStatus.Equivalent,
            StatEqualCheck.Compare(left, right, [leftTerminal], [rightTerminal]).Status);
    }

    [TestMethod]
    public void Compare_UnlistedNum4SwitchAndTryCatchRemainObservable()
    {
        const string leftSwitch = "switch (num4) { case 1: Run(); break; default: Wait(); break; }";
        const string rightSwitch = "switch (num4) { case 1: Execute(); break; default: Wait(); break; }";
        Assert.AreEqual(StatEqualStatus.NotEquivalent, StatEqualCheck.Compare(leftSwitch, rightSwitch).Status);

        const string leftHandler = "try { Run(); } catch (Exception ex) { HandleLeft(ex); }";
        const string rightHandler = "try { Run(); } catch (Exception ex) { HandleRight(ex); }";
        Assert.AreEqual(StatEqualStatus.NotEquivalent, StatEqualCheck.Compare(leftHandler, rightHandler).Status);
    }

    [TestMethod]
    public void Compare_TerminalBlockMustBelongToCorrespondingTree()
    {
        var left = CreateRoot("Run();");
        var right = CreateRoot("Run();");
        var foreignBlock = CreateBlock("Foreign();");

        Assert.Throws<ArgumentException>(() => StatEqualCheck.Compare(left, right, [foreignBlock], null));
    }

    private static CodeBlock CreateRoot(params object[] children)
    {
        var root = new CodeBlock { Type = CodeBlockType.MainBlock, Name = "root" };
        foreach (var child in children)
            (child is ICodeBlock block ? block : CreateBlock((string)child)).Parent = root;
        return root;
    }

    private static CodeBlock CreateBlock(string code, params string[] children)
    {
        var block = new CodeBlock { Type = CodeBlockType.Operation, Code = code };
        foreach (var child in children)
            CreateBlock(child).Parent = block;
        return block;
    }
}


