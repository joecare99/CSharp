using System;
using System.Collections.Generic;
using System.Linq;
using GenInterfaces.Interfaces.Genealogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using OFBCreator.Core.Models;
using OFBCreator.Core.Services;
using OFBCreator.Core.Tests.Helpers;
using OFBCreator.Projects.Models;

namespace OFBCreator.Core.Tests.Services;

[TestClass]
public sealed class OFBFamilyGroupingServiceTests
{
    [TestMethod]
    public void BuildGroups_DoesNotOfferPhoneticOnlyPairsWithoutQualifyingParentTransitions()
    {
        var result = new OFBFamilyGroupingService().BuildGroups(
            [
                CreateFamily("F1", "Müller"),
                CreateFamily("F2", "Mueller")
            ],
            "gedcom",
            new OFBGroupingPolicy(),
            []);

        Assert.AreEqual(2, result.Groups.Count);
        Assert.AreEqual(0, result.Candidates.Count);
    }

    [TestMethod]
    public void BuildGroups_OffersTransitionWhenSingleKnownParentAndFamilyNameAreDifferentGroups()
    {
        var result = new OFBFamilyGroupingService().BuildGroups(
            [
                CreateFamily("F1", "Kind", parentSurname: "Elternname"),
                CreateFamily("F2", "Elternname")
            ],
            "gedcom",
            new OFBGroupingPolicy(),
            []);

        Assert.AreEqual(1, result.Candidates.Count);
        CollectionAssert.AreEquivalent(
            new[] { "Elternname", "Kind" },
            new[] { result.Candidates[0].LeftSurname, result.Candidates[0].RightSurname });
        Assert.IsTrue(result.Candidates[0].Evidence.Any(evidence =>
            evidence.Kind == "parentFamilySurnameTransition" && evidence.ObservationCount == 1));
    }

    [TestMethod]
    public void BuildGroups_OffersEachTransitionWhenBothParentsAndChildHaveDifferentGroups()
    {
        var result = new OFBFamilyGroupingService().BuildGroups(
            [
                CreateFamilyWithParents("F1", "Kind", "ElternEins", "ElternZwei"),
                CreateFamily("F2", "ElternEins"),
                CreateFamily("F3", "ElternZwei")
            ],
            "gedcom",
            new OFBGroupingPolicy(),
            []);

        Assert.AreEqual(2, result.Candidates.Count);
        Assert.IsTrue(result.Candidates.All(candidate =>
            candidate.Evidence.Any(evidence => evidence.Kind == "parentFamilySurnameTransition")));
    }

    [TestMethod]
    public void BuildGroups_DoesNotOfferTransitionWhenParentAndFamilyShareSurnameGroup()
    {
        var result = new OFBFamilyGroupingService().BuildGroups(
            [CreateFamily("F1", "Gleich"), CreateFamily("F2", "Andere")],
            "gedcom",
            new OFBGroupingPolicy(),
            []);

        Assert.AreEqual(0, result.Candidates.Count);
    }

    [TestMethod]
    public void BuildGroups_AutoAcceptsRepeatedLineageEvidenceAtConfiguredThreshold()
    {
        var result = new OFBFamilyGroupingService().BuildGroups(
            [
                CreateFamily("F1", "Müller", parentSurname: "Meier"),
                CreateFamily("F2", "Müller", parentSurname: "Meier"),
                CreateFamily("F3", "Meier")
            ],
            "gedcom",
            new OFBGroupingPolicy { AutoAcceptThreshold = 85 },
            []);

        Assert.AreEqual(1, result.Groups.Count);
        Assert.AreEqual("autoAccepted", result.Candidates.Single().Status);
        Assert.IsTrue(result.Candidates.Single().Evidence.Any(evidence =>
            evidence.Kind == "parentFamilySurnameTransition" && evidence.ObservationCount == 2));
        Assert.AreEqual("Müller", result.Groups.Keys.Single());
    }

    [TestMethod]
    public void BuildGroups_ManualGroupNameOverridesMostFrequentSurname()
    {
        var result = new OFBFamilyGroupingService().BuildGroups(
            [
                CreateFamily("F1", "Müller"),
                CreateFamily("F2", "Müller"),
                CreateFamily("F3", "Mueller")
            ],
            "gedcom",
            new OFBGroupingPolicy(),
            [new OFBGroupingDecision
            {
                Order = 1,
                LeftFamilyTargetId = OFBExportRuleTarget.Family("gedcom", "F1"),
                RightFamilyTargetId = OFBExportRuleTarget.Family("gedcom", "F3"),
                Action = "manualMerge",
                GroupName = "Redaktionell"
            }]);

        Assert.AreEqual("Redaktionell", result.Groups.Keys.Single());
    }

    [TestMethod]
    public void BuildGroups_AppliesPersistedRejectAndManualMergeDecisions()
    {
        var service = new OFBFamilyGroupingService();
        var families = new[]
        {
            CreateFamily("F1", "Müller", parentSurname: "Meier"),
            CreateFamily("F2", "Meier")
        };
        var decision = new OFBGroupingDecision
        {
            Order = 1,
            LeftFamilyTargetId = OFBExportRuleTarget.Family("gedcom", "F1"),
            RightFamilyTargetId = OFBExportRuleTarget.Family("gedcom", "F2"),
            Action = "rejectMerge"
        };
        var reject = decision;
        var rejected = service.BuildGroups(families, "gedcom", new OFBGroupingPolicy(), [reject]);

        Assert.AreEqual(2, rejected.Groups.Count);
        Assert.AreEqual("rejected", rejected.Candidates.Single().Status);
        Assert.AreEqual(0, rejected.Diagnostics.Count);

        var manual = new OFBGroupingDecision
        {
            Order = 1,
            LeftFamilyTargetId = OFBExportRuleTarget.Family("gedcom", "F1"),
            RightFamilyTargetId = OFBExportRuleTarget.Family("gedcom", "F2"),
            Action = "manualMerge"
        };
        manual.GroupName = "Redaktionelle Gruppe";
        var manuallyGrouped = service.BuildGroups(families, "gedcom", new OFBGroupingPolicy(), [manual]);

        Assert.AreEqual(1, manuallyGrouped.Groups.Count);
        Assert.IsTrue(manuallyGrouped.Groups.ContainsKey("Redaktionelle Gruppe"));
        Assert.AreEqual("manualMerge", manuallyGrouped.Candidates.Single().Status);
    }

    [TestMethod]
    public void BuildGroups_AllowsManualMergeWithoutAutomaticEvidence()
    {
        var families = new[] { CreateFamily("F10", "Nguyen"), CreateFamily("F11", "Olsen") };
        var decision = new OFBGroupingDecision
        {
            Order = 1,
            LeftFamilyTargetId = OFBExportRuleTarget.Family("gedcom", "F10"),
            RightFamilyTargetId = OFBExportRuleTarget.Family("gedcom", "F11"),
            Action = "manualMerge",
            GroupName = "Editorial"
        };

        var result = new OFBFamilyGroupingService().BuildGroups(
            families, "gedcom", new OFBGroupingPolicy(), [decision]);

        Assert.AreEqual(1, result.Groups.Count);
        Assert.AreEqual("manualMerge", result.Candidates.Single().Status);
        Assert.AreEqual(0, result.Candidates.Single().Score);
        Assert.AreEqual(0, result.Candidates.Single().Evidence.Count);
    }

    [TestMethod]
    public void BuildGroups_ReportsStaleDecisionsAndUnstableAutomaticTargets()
    {
        var stale = new OFBGroupingDecision
        {
            Order = 1,
            LeftFamilyTargetId = "gedcom:family:Missing1",
            RightFamilyTargetId = "gedcom:family:Missing2",
            Action = "acceptMerge"
        };
        var staleResult = new OFBFamilyGroupingService().BuildGroups(
            [CreateFamily("F1", "Müller"), CreateFamily("F2", "Mueller")],
            "gedcom",
            new OFBGroupingPolicy(),
            [stale]);
        Assert.AreEqual("GROUPING_DECISION_STALE", staleResult.Diagnostics.Single().Code);

        var unstableResult = new OFBFamilyGroupingService().BuildGroups(
            [CreateFamily(null, "Mueller", parentSurname: "Müller"), CreateFamily(null, "Müller")],
            "gedcom",
            new OFBGroupingPolicy { AutoAcceptThreshold = 75 },
            []);
        Assert.AreEqual(2, unstableResult.Groups.Count);
        Assert.AreEqual("requiresStableTargets", unstableResult.Candidates.Single().Status);
        Assert.AreEqual("GROUPING_TARGET_UNSTABLE", unstableResult.Diagnostics.Single().Code);
    }

    private static OFBGroupingDecision CreateDecision(OFBGroupingCandidate candidate, string action) => new()
    {
        Order = 1,
        LeftFamilyTargetId = candidate.LeftFamilyTargetId!,
        RightFamilyTargetId = candidate.RightFamilyTargetId!,
        Action = action
    };

    private static IGenFamily CreateFamily(string? familyId, string surname, string? parentSurname = null)
    {
        var family = Substitute.For<IGenFamily>();
        family.FamilyRefID.Returns(familyId);
        var husband = CreatePerson(parentSurname ?? surname);
        family.Husband.Returns(husband);
        family.Wife.Returns((IGenPerson?)null);
        var children = new TestIndexedList<IGenPerson>();
        if (parentSurname is not null)
            children.Add(CreatePerson(surname));
        family.Children.Returns(children);
        return family;
    }

    private static IGenFamily CreateFamilyWithParents(
        string familyId,
        string childSurname,
        string husbandSurname,
        string? wifeSurname)
    {
        var family = Substitute.For<IGenFamily>();
        family.FamilyRefID.Returns(familyId);
        var wife = wifeSurname is null ? null : CreatePerson(wifeSurname);
        var husband = CreatePerson(husbandSurname);
        family.Husband.Returns(husband);
        family.Wife.Returns(wife);
        var children = new TestIndexedList<IGenPerson>();
        children.Add(CreatePerson(childSurname));
        family.Children.Returns(children);
        return family;
    }

    private static IGenPerson CreatePerson(string surname)
    {
        var person = Substitute.For<IGenPerson>();
        person.Surname.Returns(surname);
        return person;
    }
}
