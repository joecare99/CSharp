using Microsoft.VisualStudio.TestTools.UnitTesting;
#if !NETFRAMEWORK
using System.CommandLine;

[TestClass]
public sealed class CliOptionsTests
{
    [TestMethod]
    public void NewOptionsAreDisabledByDefault()
    {
        CliOptions? parsedOptions = null;
        var parseResult = CliOptions.CreateCommand(options =>
        {
            parsedOptions = options;
            return 0;
        }).Parse(["--stdin"]);

        Assert.AreEqual(0, parseResult.Invoke());
        Assert.IsNotNull(parsedOptions);
        Assert.IsFalse(parsedOptions.CheckEquivalence);
        Assert.IsNull(parsedOptions.CompareAgainstPath);
        Assert.IsNull(parsedOptions.EquivalenceJsonPath);
        Assert.IsFalse(parsedOptions.FailOnMismatch);
        Assert.IsFalse(parsedOptions.ReplaceVbLegacy);
        Assert.IsNull(parsedOptions.LegacyRulesPath);
    }

    [TestMethod]
    public void NewOptionsCanBeEnabledIndependently()
    {
        CliOptions? parsedOptions = null;
        var parseResult = CliOptions.CreateCommand(options =>
        {
            parsedOptions = options;
            return 0;
        }).Parse([
            "--stdin",
            "--check-equivalence",
            "--equivalence-json", "equivalence.json",
            "--fail-on-mismatch",
            "--replace-vb-legacy",
            "--legacy-rules", "legacy-rules.json"
        ]);

        Assert.AreEqual(0, parseResult.Invoke());
        Assert.IsNotNull(parsedOptions);
        Assert.IsTrue(parsedOptions.CheckEquivalence);
        Assert.AreEqual("equivalence.json", parsedOptions.EquivalenceJsonPath);
        Assert.IsTrue(parsedOptions.FailOnMismatch);
        Assert.IsTrue(parsedOptions.ReplaceVbLegacy);
        Assert.AreEqual("legacy-rules.json", parsedOptions.LegacyRulesPath);
    }

    [TestMethod]
    public void CompareAgainstOptionCanBeUsedWithoutOptimizingComparisonTarget()
    {
        CliOptions? parsedOptions = null;
        var parseResult = CliOptions.CreateCommand(options =>
        {
            parsedOptions = options;
            return 0;
        }).Parse([
            "--stdin",
            "--compare-against", "baseline.cs",
            "--fail-on-mismatch"
        ]);

        Assert.AreEqual(0, parseResult.Invoke());
        Assert.IsNotNull(parsedOptions);
        Assert.IsFalse(parsedOptions.CheckEquivalence);
        Assert.AreEqual("baseline.cs", parsedOptions.CompareAgainstPath);
        Assert.IsTrue(parsedOptions.FailOnMismatch);
    }

    [TestMethod]
    public void LegacyReplacementCanBeEnabledWithoutEquivalenceChecking()
    {
        CliOptions? parsedOptions = null;
        var parseResult = CliOptions.CreateCommand(options =>
        {
            parsedOptions = options;
            return 0;
        }).Parse(["--stdin", "--replace-vb-legacy"]);

        Assert.AreEqual(0, parseResult.Invoke());
        Assert.IsNotNull(parsedOptions);
        Assert.IsFalse(parsedOptions.CheckEquivalence);
        Assert.IsTrue(parsedOptions.ReplaceVbLegacy);
    }
}
#endif
