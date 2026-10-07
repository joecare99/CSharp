using System;
using System.Collections.Generic;
using System.Linq;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using OFBCreator.Core.Tests.Helpers;
using OFBCreator.Core.Services;
using OFBCreator.Projects.Models;

namespace OFBCreator.Core.Tests.Services;

[TestClass]
public sealed class OFBExportOverlayServiceTests
{
    [TestMethod]
    public void Apply_ReplacesNamesAndGeneralizesDatesWithoutMutatingSource()
    {
        var (genealogy, sourcePerson, sourceFamily) = CreateGenealogy();
        var sourceDate = Substitute.For<IGenDate>();
        sourceDate.Date1.Returns(new DateTime(1876, 5, 9));
        sourceDate.DateText.Returns("9 MAY 1876");
        sourcePerson.BirthDate.Returns(sourceDate);
        var rules = new[]
        {
            new OFBExportRule
            {
                Order = 1,
                TargetKind = "person",
                TargetId = OFBExportRuleTarget.Person("gedcom", "I1"),
                Action = "replace",
                Field = "surname",
                Value = "Pseudonym"
            },
            new OFBExportRule
            {
                Order = 2,
                TargetKind = "person",
                TargetId = OFBExportRuleTarget.Person("gedcom", "I1"),
                Action = "generalize",
                Field = "birthDate",
                Value = "decade"
            }
        };

        var result = new OFBExportOverlayService().Apply(genealogy, "gedcom", rules);
        var exportedPerson = result.Genealogy.Entitys.OfType<IGenPerson>().Single();
        var exportedFamily = result.Genealogy.Entitys.OfType<IGenFamily>().Single();

        Assert.AreNotSame(sourcePerson, exportedPerson);
        Assert.AreNotSame(sourceFamily, exportedFamily);
        Assert.AreEqual("Smith", sourcePerson.Surname);
        Assert.AreEqual(new DateTime(1876, 5, 9), sourceDate.Date1);
        Assert.AreEqual("Pseudonym", exportedPerson.Surname);
        Assert.AreEqual("1870s", exportedPerson.BirthDate?.DateText);
        Assert.AreEqual(new DateTime(1870, 1, 1), exportedPerson.BirthDate?.Date1);
        Assert.AreSame(exportedPerson, exportedFamily.Husband);
        Assert.AreEqual(0, result.Diagnostics.Count);
        var preview = result.PersonPreviews.Single();
        Assert.AreEqual(OFBExportRuleTarget.Person("gedcom", "I1"), preview.TargetId);
        Assert.IsTrue(preview.HasChanges);
        CollectionAssert.AreEquivalent(new[] { "surname", "birthDate" }, preview.Changes.Select(change => change.Field).ToArray());
    }

    [TestMethod]
    public void Apply_ExcludesPersonFromDetachedFamilyAndReportsMissingTargets()
    {
        var (genealogy, sourcePerson, _) = CreateGenealogy();
        var rules = new[]
        {
            new OFBExportRule
            {
                Order = 1,
                TargetKind = "person",
                TargetId = OFBExportRuleTarget.Person("gedcom", "I1"),
                Action = "exclude"
            },
            new OFBExportRule
            {
                Order = 2,
                TargetKind = "person",
                TargetId = OFBExportRuleTarget.Person("gedcom", "I999"),
                Action = "exclude"
            }
        };

        var result = new OFBExportOverlayService().Apply(genealogy, "gedcom", rules);

        Assert.AreEqual(0, result.Genealogy.Entitys.OfType<IGenFamily>().Count());
        Assert.AreEqual(0, result.Genealogy.Entitys.OfType<IGenPerson>().Count());
        Assert.AreEqual(0, result.PersonPreviews.Count);
        Assert.AreEqual("Smith", sourcePerson.Surname);
        Assert.AreEqual("STALE_RULE_TARGET", result.Diagnostics.Single().Code);
        Assert.IsFalse(result.Diagnostics.Single().IsError);
    }

    [TestMethod]
    public void Apply_IncludeRuleMakesTheScopeAnAllowlist()
    {
        var (genealogy, _, _) = CreateGenealogy();
        var rule = new OFBExportRule
        {
            Order = 1,
            TargetKind = "person",
            TargetId = OFBExportRuleTarget.Person("gedcom", "I999"),
            Action = "include"
        };

        var result = new OFBExportOverlayService().Apply(genealogy, "gedcom", [rule]);

        Assert.AreEqual(0, result.Genealogy.Entitys.OfType<IGenPerson>().Count());
        Assert.AreEqual(0, result.Genealogy.Entitys.OfType<IGenFamily>().Count());
        Assert.AreEqual("STALE_RULE_TARGET", result.Diagnostics.Single().Code);
        Assert.AreEqual(0, result.PersonPreviews.Count);
    }

    [TestMethod]
    public void Apply_ExcludesSelectedFactOccurrenceWithoutChangingSourceFacts()
    {
        var (genealogy, sourcePerson, _) = CreateGenealogy();
        sourcePerson.Occupation.Returns("Farmer");
        var firstOccupation = CreateFact(EFactType.Occupation, "Farmer");
        var secondOccupation = CreateFact(EFactType.Occupation, "Teacher");
        sourcePerson.Facts.Returns(new List<IGenFact?> { firstOccupation, secondOccupation });
        var rule = new OFBExportRule
        {
            Order = 1,
            TargetKind = "fact",
            TargetId = OFBExportRuleTarget.FactOwner("gedcom", "person", "I1"),
            Action = "exclude",
            Field = EFactType.Occupation.ToString(),
            Occurrence = 1
        };

        var result = new OFBExportOverlayService().Apply(genealogy, "gedcom", [rule]);
        var exportedPerson = result.Genealogy.Entitys.OfType<IGenPerson>().Single();

        Assert.AreEqual(2, sourcePerson.Facts.Count);
        Assert.AreEqual("Farmer", sourcePerson.Occupation);
        Assert.AreEqual(1, exportedPerson.Facts.Count);
        Assert.AreEqual("Teacher", exportedPerson.Facts.Single()?.Data);
        Assert.AreEqual("Teacher", exportedPerson.Occupation);
        Assert.AreEqual(0, result.Diagnostics.Count);
    }

    [TestMethod]
    public void Apply_WithoutRules_ReturnsUnchangedPersonPreviewWithoutUsingUnfilteredNameFallback()
    {
        var (genealogy, _, _) = CreateGenealogy();

        var result = new OFBExportOverlayService().Apply(genealogy, "gedcom", []);

        var preview = result.PersonPreviews.Single();
        Assert.AreEqual(OFBExportRuleTarget.Person("gedcom", "I1"), preview.TargetId);
        Assert.AreEqual("Alice Smith", preview.DisplayName);
        Assert.IsFalse(preview.HasChanges);
    }

    [TestMethod]
    public void Apply_ReplacesPlaceOnExportCopyAndRemovesSourceCoordinates()
    {
        var (genealogy, sourcePerson, _) = CreateGenealogy();
        var sourcePlace = Substitute.For<IGenPlace>();
        sourcePlace.UId.Returns(Guid.NewGuid());
        sourcePlace.Name.Returns("Private Village");
        sourcePlace.GOV_ID.Returns("Private Village");
        sourcePlace.Latitude.Returns(49.5);
        sourcePlace.Longitude.Returns(8.6);
        sourcePerson.BirthPlace.Returns(sourcePlace);
        var rule = new OFBExportRule
        {
            Order = 1,
            TargetKind = "person",
            TargetId = OFBExportRuleTarget.Person("gedcom", "I1"),
            Action = "replace",
            Field = "birthPlace",
            Value = "Region"
        };

        var result = new OFBExportOverlayService().Apply(genealogy, "gedcom", [rule]);
        var exportedPerson = result.Genealogy.Entitys.OfType<IGenPerson>().Single();

        Assert.AreNotSame(sourcePlace, exportedPerson.BirthPlace);
        Assert.AreEqual("Private Village", sourcePlace.Name);
        Assert.AreEqual("Private Village", sourcePlace.GOV_ID);
        Assert.AreEqual("Region", exportedPerson.BirthPlace?.Name);
        Assert.IsNull(exportedPerson.BirthPlace?.GOV_ID);
        Assert.AreEqual(0, exportedPerson.BirthPlace?.Latitude);
        Assert.AreEqual(0, exportedPerson.BirthPlace?.Longitude);
        Assert.AreEqual(0, result.Diagnostics.Count);
    }

    private static (IGenealogy Genealogy, IGenPerson Person, IGenFamily Family) CreateGenealogy()
    {
        var person = Substitute.For<IGenPerson>();
        person.UId.Returns(Guid.NewGuid());
        person.ID.Returns(1);
        person.Name.Returns("Alice /Smith/");
        person.GivenName.Returns("Alice");
        person.Surname.Returns("Smith");
        person.Sex.Returns("F");
        person.IndRefID.Returns("I1");
        person.Facts.Returns(new List<IGenFact?>());
        person.Birth.Returns((IGenFact?)null);
        person.Baptism.Returns((IGenFact?)null);
        person.Death.Returns((IGenFact?)null);
        person.Burial.Returns((IGenFact?)null);

        var family = Substitute.For<IGenFamily>();
        family.UId.Returns(Guid.NewGuid());
        family.ID.Returns(2);
        family.FamilyRefID.Returns("F1");
        family.Husband.Returns(person);
        family.Wife.Returns((IGenPerson?)null);
        family.Marriage.Returns((IGenFact?)null);
        family.Facts.Returns(new List<IGenFact?>());
        family.Children.Returns(new TestIndexedList<IGenPerson>());

        var genealogy = Substitute.For<IGenealogy>();
        genealogy.Entitys.Returns(new List<IGenEntity> { family, person });
        return (genealogy, person, family);
    }

    private static IGenFact CreateFact(EFactType type, string data)
    {
        var fact = Substitute.For<IGenFact>();
        fact.UId.Returns(Guid.NewGuid());
        fact.ID.Returns(3);
        fact.eFactType.Returns(type);
        fact.Data.Returns(data);
        fact.Sources.Returns(new List<IGenSource?>());
        fact.Medias.Returns(new List<IGenMedia?>());
        return fact;
    }
}
