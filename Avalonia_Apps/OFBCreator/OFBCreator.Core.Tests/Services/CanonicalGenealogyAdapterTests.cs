using System;
using System.Linq;
using Genealogy.Models;
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
}
