using System;
using System.Linq;
using TranspilerLib.CSharp.VBLegacyReplace.Models;

namespace TranspilerLib.CSharp.VBLegacyReplace.Tests.Services;

[TestClass]
public sealed class LegacyReplacementEngineTests
{

    [TestMethod]
    public void DefaultRules_TrimCompleteNestedExpression_AndPreservesCapturedText()
    {
        var engine = LegacyReplacementEngine.LoadDefaultRules();
        const string source = "var result = Strings.Trim( Call( new[] { \")\", \"x\" }[index], nested(a, b) ) );";

        ReplacementResult result = engine.Apply(source);

        Assert.AreEqual("var result = ((Call( new[] { \")\", \"x\" }[index], nested(a, b) )) ?? string.Empty).Trim(' ');", result.Source);
        Assert.AreEqual("VB.Strings.Trim", result.Diagnostics.Single(d => d.Kind == RuleDiagnosticKind.Applied).RuleId);
        Assert.AreEqual(0, result.RequiredUsings.Count);
    }

    [TestMethod]
    public void DefaultRules_TrimVariantsPreserveVisualBasicSpaceAndNullBehavior()
    {
        var engine = LegacyReplacementEngine.LoadDefaultRules();

        ReplacementResult result = engine.Apply(
            "var all = Strings.Trim(value); var left = Strings.LTrim(null); var right = Strings.RTrim(source);");

        Assert.AreEqual(
            "var all = ((value) ?? string.Empty).Trim(' '); var left = ((null) ?? string.Empty).TrimStart(' '); var right = ((source) ?? string.Empty).TrimEnd(' ');",
            result.Source);
        CollectionAssert.AreEquivalent(
            new[] { "VB.Strings.Trim", "VB.Strings.LTrim", "VB.Strings.RTrim" },
            result.Diagnostics.Where(d => d.Kind == RuleDiagnosticKind.Applied).Select(d => d.RuleId).ToArray());
    }

    [TestMethod]
    public void DefaultRules_ReplacesNestedLegacyCallsBeforeTheirContainingCall()
    {
        var engine = LegacyReplacementEngine.LoadDefaultRules();

        ReplacementResult result = engine.Apply("var text = Strings.Trim(Strings.LTrim(value));");

        Assert.AreEqual(
            "var text = ((((value) ?? string.Empty).TrimStart(' ')) ?? string.Empty).Trim(' ');",
            result.Source);
        CollectionAssert.AreEqual(
            new[] { "VB.Strings.LTrim", "VB.Strings.Trim" },
            result.Diagnostics.Where(d => d.Kind == RuleDiagnosticKind.Applied).Select(d => d.RuleId).ToArray());
        Assert.AreEqual(24, result.Diagnostics.First(d => d.RuleId == "VB.Strings.LTrim").Position);
    }

    [TestMethod]
    public void NestedReplacementDepthLimitProducesDiagnosticInsteadOfUnboundedRecursion()
    {
        var engine = LegacyReplacementEngine.LoadDefaultRules();
        string source = string.Concat(Enumerable.Repeat("Strings.Trim(", 130)) + "value" + new string(')', 130);

        ReplacementResult result = engine.Apply(source);

        Assert.IsTrue(result.Diagnostics.Any(d =>
            d.Kind == RuleDiagnosticKind.Skipped &&
            d.Severity == RuleDiagnosticSeverity.Warning &&
            d.Message.IndexOf("Nested replacement depth exceeded", StringComparison.Ordinal) >= 0));
        StringAssert.Contains(result.Source, "Strings.Trim(");
    }

    [TestMethod]
    public void DefaultRules_TrimPreservesCharacterLiteralAndRejectsMultipleArguments()
    {
        var engine = LegacyReplacementEngine.LoadDefaultRules();

        ReplacementResult character = engine.Apply("Strings.Trim(GetChar(')', values[index]))");
        ReplacementResult multipleArguments = engine.Apply("Strings.Trim(value, other)");

        Assert.AreEqual("((GetChar(')', values[index])) ?? string.Empty).Trim(' ')", character.Source);
        Assert.AreEqual("Strings.Trim(value, other)", multipleArguments.Source);
        Assert.IsTrue(multipleArguments.Diagnostics.Any(d => d.Kind == RuleDiagnosticKind.Skipped));
    }

    [TestMethod]
    public void DefaultRules_MatchesWithDifferentWhitespaceAndComments_ButNotInsideLiterals()
    {
        var engine = LegacyReplacementEngine.LoadDefaultRules();

        ReplacementResult result = engine.Apply("var text = Strings /* legacy */ . Trim ( value /* captured */ + other ); var literal = \"Strings.Trim(no)\";");

        Assert.AreEqual("var text = ((value /* captured */ + other) ?? string.Empty).Trim(' '); var literal = \"Strings.Trim(no)\";", result.Source);
        Assert.AreEqual(1, result.Diagnostics.Count(d => d.Kind == RuleDiagnosticKind.Applied));
    }

    [TestMethod]
    public void DefaultRules_ReplacesDBNullAndDatePropertiesWithBclEquivalents()
    {
        var engine = LegacyReplacementEngine.LoadDefaultRules();

        ReplacementResult result = engine.Apply(
            "if (Information.IsDBNull(value)) return DateAndTime.Now; var today = DateAndTime.Today;");

        Assert.AreEqual(
            "if (Convert.IsDBNull(value)) return DateTime.Now; var today = DateTime.Today;",
            result.Source);
        CollectionAssert.AreEquivalent(
            new[] { "VB.Information.IsDBNull", "VB.DateAndTime.Now", "VB.DateAndTime.Today" },
            result.Diagnostics.Where(d => d.Kind == RuleDiagnosticKind.Applied).Select(d => d.RuleId).ToArray());
    }

    [TestMethod]
    [DataRow("true")]
    [DataRow("!ready")]
    [DataRow("left == right")]
    [DataRow("ready && left < right")]
    [DataRow("(ready || !done)")]
    public void BooleanPlaceholder_AcceptsSyntacticallyBooleanExpressions(string expression)
    {
        var engine = LegacyReplacementEngine.LoadJson(RuleJson("Check(<Param1:bool>)", "Use(<Param1>)"));

        ReplacementResult result = engine.Apply($"Check({expression})");

        Assert.AreEqual($"Use({expression})", result.Source);
        Assert.AreEqual(1, result.Diagnostics.Count(d => d.Kind == RuleDiagnosticKind.Applied));
        Assert.AreEqual(0, result.Diagnostics.Count(d => d.Kind == RuleDiagnosticKind.Skipped && d.Severity == RuleDiagnosticSeverity.Warning));
    }

    [TestMethod]
    public void PlaceholderSyntaxInsideStringLiteralsRemainsLiteralText()
    {
        LegacyReplacementEngine engine = LegacyReplacementEngine.LoadJson(RuleJson("Keep(\"<Param1>\")", "Saved(\"<Param1>\")"));

        ReplacementResult result = engine.Apply("Keep(\"<Param1>\")");

        Assert.AreEqual("Saved(\"<Param1>\")", result.Source);
        Assert.AreEqual(0, result.Diagnostics.Count(d => d.Kind == RuleDiagnosticKind.Malformed));
    }

    [TestMethod]
    public void BooleanPlaceholder_UnknownExpressionIsSkippedWithWarning()
    {
        var engine = LegacyReplacementEngine.LoadJson(RuleJson("Check(<Param1:bool>)", "Use(<Param1>)"));

        ReplacementResult result = engine.Apply("Check(maybe)");

        Assert.AreEqual("Check(maybe)", result.Source);
        Assert.IsTrue(result.Diagnostics.Any(d => d.Kind == RuleDiagnosticKind.Skipped && d.Severity == RuleDiagnosticSeverity.Warning && d.RuleId == "Test.Rule"));
    }

    [TestMethod]
    public void Rules_SelectPriorityAndNeverOverlap()
    {
        string json = """
        {
          "schemaVersion": 1,
          "rules": [
            { "id": "generic", "sourceTemplate": "Foo(<Param1>)", "replacementTemplate": "low(<Param1>)", "priority": 1 },
            { "id": "specific", "sourceTemplate": "Foo(1)", "replacementTemplate": "high", "priority": 10 }
          ]
        }
        """;

        ReplacementResult result = LegacyReplacementEngine.LoadJson(json).Apply("Foo(1) + Foo(2)");

        Assert.AreEqual("high + low(2)", result.Source);
        CollectionAssert.AreEqual(new[] { "specific", "generic" }, result.Diagnostics
            .Where(d => d.Kind == RuleDiagnosticKind.Applied).Select(d => d.RuleId).ToArray());
    }

    [TestMethod]
    public void Rules_CollectRequiredUsingsOnlyForAppliedReplacements()
    {
        string json = RuleJson("Legacy.Wrap(<Param1>)", "Modern.Wrap(<Param1>)", "Example.Namespace", "System.Collections.Generic");

        ReplacementResult result = LegacyReplacementEngine.LoadJson(json).Apply("Legacy.Wrap(value)");

        CollectionAssert.AreEqual(new[] { "Example.Namespace", "System.Collections.Generic" }, result.RequiredUsings.ToArray());
        Assert.AreEqual("Modern.Wrap(value)", result.Source);
    }

    [TestMethod]
    public void MalformedJsonAndUnsupportedSchemaProduceMalformedDiagnostics()
    {
        LegacyReplacementEngine invalidJson = LegacyReplacementEngine.LoadJson("{");
        LegacyReplacementEngine unsupported = LegacyReplacementEngine.LoadJson("""{"schemaVersion":77,"rules":[]}""");
        LegacyReplacementEngine missingVersion = LegacyReplacementEngine.LoadJson("""{"rules":[]}""");
        LegacyReplacementEngine missingTemplate = LegacyReplacementEngine.LoadJson("""{"schemaVersion":1,"rules":[{"id":"missing-template","sourceTemplate":"Old"}]}""");

        Assert.AreEqual(RuleDiagnosticKind.Malformed, invalidJson.ConfigurationDiagnostics.Single().Kind);
        Assert.AreEqual(RuleDiagnosticKind.Malformed, unsupported.ConfigurationDiagnostics.Single().Kind);
        Assert.IsTrue(missingVersion.ConfigurationDiagnostics.Any(d => d.Kind == RuleDiagnosticKind.Malformed));
        Assert.IsTrue(missingTemplate.ConfigurationDiagnostics.Any(d => d.Kind == RuleDiagnosticKind.Malformed));
        Assert.AreEqual("source", unsupported.Apply("source").Source);
    }

    [TestMethod]
    public void InvalidPlaceholderAndDisabledRuleAreReported()
    {
        string json = """
        {
          "schemaVersion": 1,
          "rules": [
            { "id": "bad", "sourceTemplate": "Old(<Param1:string>)", "replacementTemplate": "New(<Param1>)" },
            { "id": "bad-unclosed", "sourceTemplate": "Open(<Param1", "replacementTemplate": "Closed" },
            { "id": "off", "sourceTemplate": "Off(<Param1>)", "replacementTemplate": "On(<Param1>)", "enabled": false }
          ]
        }
        """;

        LegacyReplacementEngine engine = LegacyReplacementEngine.LoadJson(json);

        Assert.IsTrue(engine.ConfigurationDiagnostics.Any(d => d.Kind == RuleDiagnosticKind.Malformed && d.RuleId == "bad"));
        Assert.IsTrue(engine.ConfigurationDiagnostics.Any(d => d.Kind == RuleDiagnosticKind.Malformed && d.RuleId == "bad-unclosed"));
        Assert.IsTrue(engine.ConfigurationDiagnostics.Any(d => d.Kind == RuleDiagnosticKind.Skipped && d.RuleId == "off"));
    }

    private static string RuleJson(string sourceTemplate, string replacementTemplate, params string[] usings)
    {
        string serializedUsings = string.Join(",", usings.Select(value => System.Text.Json.JsonSerializer.Serialize(value)));
        return $$"""
        {
          "schemaVersion": 1,
          "rules": [
            {
              "id": "Test.Rule",
              "sourceTemplate": {{System.Text.Json.JsonSerializer.Serialize(sourceTemplate)}},
              "replacementTemplate": {{System.Text.Json.JsonSerializer.Serialize(replacementTemplate)}},
              "requiredUsings": [{{serializedUsings}}]
            }
          ]
        }
        """;
    }
}
