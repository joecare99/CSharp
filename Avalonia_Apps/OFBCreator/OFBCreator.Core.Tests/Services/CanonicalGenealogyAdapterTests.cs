using System;
using System.Linq;
using Genealogy.Statistics;
using Genealogy.Models;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OFBCreator.Core.Services;

namespace OFBCreator.Core.Tests.Services;

[TestClass]
public sealed class CanonicalGenealogyAdapterTests
{
    [TestMethod]
    public void AdaptMapsPersonFactsAndFamilyAssociationsToOFBContracts()
    {
        var document = new GenealogyDocument();
        var father = new GenealogyPerson { GivenName = "Alex", Surname = "Example", Sex = "M" };
        father.Identifiers.Add(new GenealogyIdentifier { Provider = "gedcom", Value = "@I1@" });
        var birth = new GenealogyFact { TypeCode = "BIRT" };
        birth.Children.Add(new GenealogyFact { TypeCode = "DATE", Value = "1 JAN 1900" });
        birth.Children.Add(new GenealogyFact { TypeCode = "PLAC", Value = "Example Town" });
        father.Content.Add(birth);
        father.Content.Add(new GenealogyFact { TypeCode = "OCCU", Value = "Farmer" });

        var child = new GenealogyPerson { GivenName = "Chris", Surname = "Example", Sex = "X" };
        child.Identifiers.Add(new GenealogyIdentifier { Provider = "gedcom", Value = "@I2@" });

        var family = new GenealogyFamily();
        family.Identifiers.Add(new GenealogyIdentifier { Provider = "gedcom", Value = "@F1@" });
        family.Associations.Add(new GenealogyAssociation
        {
            SourceRecordId = family.Id,
            TargetRecordId = father.Id,
            TargetIdentifier = new GenealogyIdentifier { Provider = "gedcom", Value = "@I1@" },
            Role = "Husband"
        });
        family.Associations.Add(new GenealogyAssociation
        {
            SourceRecordId = family.Id,
            TargetRecordId = child.Id,
            TargetIdentifier = new GenealogyIdentifier { Provider = "gedcom", Value = "@I2@" },
            Role = "Child"
        });
        document.Records.Add(father);
        document.Records.Add(child);
        document.Records.Add(family);

        var adapted = new CanonicalGenealogyAdapter().Adapt(document);
        var adaptedFather = adapted.Entitys.OfType<IGenPerson>().Single(person => person.IndRefID == "I1");
        var adaptedChild = adapted.Entitys.OfType<IGenPerson>().Single(person => person.IndRefID == "I2");
        var adaptedFamily = adapted.Entitys.OfType<IGenFamily>().Single();

        Assert.AreEqual("Alex", adaptedFather.GivenName);
        Assert.AreEqual("Example", adaptedFather.Surname);
        Assert.AreEqual("Farmer", adaptedFather.Occupation);
        Assert.AreEqual("Example Town", adaptedFather.BirthPlace?.Name);
        Assert.AreEqual("1 JAN 1900", adaptedFather.BirthDate?.DateText);
        Assert.AreSame(adaptedFather, adaptedFamily.Husband);
        Assert.AreSame(adaptedChild, adaptedFamily.Children.Single());
        Assert.AreSame(adaptedFamily, adaptedChild.ParentFamily);
        Assert.IsTrue(adapted.Places.Any(place => place.Name == "Example Town"));
    }

    [TestMethod]
    public void Adapt_ConvertsFrenchRepublicanDateAndRejectsInvalidModifiedDate()
    {
        var document = new GenealogyDocument();
        var eva = new GenealogyPerson { GivenName = "Eva", Surname = "Frantz", Sex = "F" };
        eva.Content.Add(CreateDateFact("BIRT", "ABT @#DFRENCH R@ 3 BRUM 3"));
        eva.Content.Add(CreateDateFact("DEAT", "01 JUL 1870"));
        var edwin = new GenealogyPerson { GivenName = "Edwin", Surname = "Müller", Sex = "M" };
        edwin.Content.Add(CreateDateFact("BIRT", "ABT 1860"));
        edwin.Content.Add(CreateDateFact("DEAT", "01 JUL 1936"));
        var invalid = new GenealogyPerson { GivenName = "Invalid", Surname = "Date", Sex = "M" };
        invalid.Content.Add(CreateDateFact("BIRT", "ABT not-a-calendar-date"));
        invalid.Content.Add(CreateDateFact("DEAT", "01 JUL 1900"));
        document.Records.Add(eva);
        document.Records.Add(edwin);
        document.Records.Add(invalid);

        var adapted = new CanonicalGenealogyAdapter().Adapt(document);
        var people = adapted.Entitys.OfType<IGenPerson>().ToArray();
        var adaptedEva = people.Single(person => person.GivenName == "Eva");
        var adaptedEdwin = people.Single(person => person.GivenName == "Edwin");
        var adaptedInvalid = people.Single(person => person.GivenName == "Invalid");
        var statistics = GenealogyStatisticsAnalyzer.Analyze(people, 1794, 1936);

        Console.WriteLine(
            $"{adaptedEdwin.GivenName} {adaptedEdwin.Surname}: " +
            $"birth={adaptedEdwin.BirthDate?.Date1:yyyy-MM-dd}, " +
            $"age={statistics.OldestMan?.EstimatedAgeYears:0.0} years");
        Assert.AreEqual(new DateTime(1794, 10, 24), adaptedEva.BirthDate!.Date1);
        Assert.AreEqual(new DateTime(1860, 1, 1), adaptedEdwin.BirthDate!.Date1);
        Assert.AreEqual(EDateModifier.Text, adaptedInvalid.BirthDate!.eDateModifier);
        Assert.AreSame(adaptedEdwin, statistics.OldestMan!.Person);
        Assert.AreSame(adaptedEva, statistics.OldestWoman!.Person);
        Assert.AreEqual(76.5, statistics.OldestMan.EstimatedAgeYears!.Value, 0.2);
    }

    private static GenealogyFact CreateDateFact(string typeCode, string value)
    {
        var fact = new GenealogyFact { TypeCode = typeCode };
        fact.Children.Add(new GenealogyFact { TypeCode = "DATE", Value = value });
        return fact;
    }
}
