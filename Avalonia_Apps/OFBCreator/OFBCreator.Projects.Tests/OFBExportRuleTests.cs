using Microsoft.VisualStudio.TestTools.UnitTesting;
using OFBCreator.Projects.Models;
using OFBCreator.Projects.Services;
using System;
using System.IO;
using System.Linq;

namespace OFBCreator.Projects.Tests;

[TestClass]
public sealed class OFBExportRuleTests
{
    [TestMethod]
    public void ProjectStore_RoundTripsOrderedExportRules()
    {
        using var testDirectory = new TestDirectory();
        var store = new OFBProjectStore(testDirectory.Path);
        var project = new OFBProject
        {
            Name = "Privacy",
            Title = "Privacy",
            ExportRules =
            [
                new OFBExportRule
                {
                    Order = 10,
                    TargetKind = "person",
                    TargetId = OFBExportRuleTarget.Person("gedcom", "I7"),
                    Action = "replace",
                    Field = "surname",
                    Value = "Example"
                }
            ],
            GroupingPolicy = new OFBGroupingPolicy { AutoAcceptThreshold = 92 },
            GroupingDecisions =
            [
                new OFBGroupingDecision
                {
                    Order = 1,
                    LeftFamilyTargetId = OFBExportRuleTarget.Family("gedcom", "F1"),
                    RightFamilyTargetId = OFBExportRuleTarget.Family("gedcom", "F2"),
                    Action = "manualMerge",
                    GroupName = "Muster"
                }
            ]
        };
        var path = Path.Combine(testDirectory.Path, "Privacy.ofbproject");

        store.Save(project, path);
        var reopened = store.Open(path).Project;

        Assert.AreEqual(1, reopened.ExportRules.Count);
        Assert.AreEqual(10, reopened.ExportRules[0].Order);
        Assert.AreEqual("gedcom:person:I7", reopened.ExportRules[0].TargetId);
        Assert.AreEqual("Example", reopened.ExportRules[0].Value);
        Assert.AreEqual(92, reopened.GroupingPolicy.AutoAcceptThreshold);
        Assert.AreEqual("Muster", reopened.GroupingDecisions.Single().GroupName);
    }

    [TestMethod]
    public void Validator_RejectsAmbiguousOrderAndUnsupportedFields()
    {
        var first = CreatePersonReplacement(order: 1);
        var duplicateOrder = CreatePersonReplacement(order: 1);
        var unsupportedField = CreatePersonReplacement(order: 2);
        unsupportedField.Field = "arbitraryExpression";

        Assert.ThrowsExactly<InvalidDataException>(() =>
            OFBExportRuleValidator.Validate([first, duplicateOrder]));
        Assert.ThrowsExactly<InvalidDataException>(() =>
            OFBExportRuleValidator.Validate([unsupportedField]));
    }

    [TestMethod]
    public void Validator_AcceptsDisabledRuleWithoutExecutingIt()
    {
        var disabled = new OFBExportRule
        {
            Order = 1,
            Enabled = false,
            TargetKind = "person",
            TargetId = OFBExportRuleTarget.Person("gedcom", "I7"),
            Action = "replace",
            Field = "surname",
            Value = "Example"
        };

        OFBExportRuleValidator.Validate([disabled]);
    }

    [TestMethod]
    public void GroupingValidator_RejectsOutOfRangeThresholdAndDuplicateFamilyPair()
    {
        var firstDecision = new OFBGroupingDecision
        {
            Order = 1,
            LeftFamilyTargetId = OFBExportRuleTarget.Family("gedcom", "F1"),
            RightFamilyTargetId = OFBExportRuleTarget.Family("gedcom", "F2"),
            Action = "acceptMerge"
        };
        var duplicateDecision = new OFBGroupingDecision
        {
            Order = 2,
            LeftFamilyTargetId = OFBExportRuleTarget.Family("GEDCOM", "F2"),
            RightFamilyTargetId = OFBExportRuleTarget.Family("gedcom", "F1"),
            Action = "rejectMerge"
        };

        Assert.ThrowsExactly<InvalidDataException>(() =>
            OFBGroupingPolicyValidator.Validate(new OFBGroupingPolicy { AutoAcceptThreshold = 101 }, []));
        Assert.ThrowsExactly<InvalidDataException>(() =>
            OFBGroupingPolicyValidator.Validate(new OFBGroupingPolicy(), [firstDecision, duplicateDecision]));
    }

    private static OFBExportRule CreatePersonReplacement(int order) => new()
    {
    Order = order,
    TargetKind = "person",
    TargetId = OFBExportRuleTarget.Person("gedcom", $"I{order}"),
    Action = "replace",
    Field = "surname",
    Value = "Example"
    };

    private sealed class TestDirectory : IDisposable
    {
        public TestDirectory()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"ofb-rules-{Guid.NewGuid():N}");
            Directory.CreateDirectory(Path);
        }

        public string Path { get; }

        public void Dispose()
        {
            if (Directory.Exists(Path))
                Directory.Delete(Path, recursive: true);
        }
    }
}
