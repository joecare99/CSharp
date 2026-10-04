using System;
using Genealogy.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Genealogy.Core.Tests;

[TestClass]
public sealed class GenealogyModelTests
{
    [TestMethod]
    public void RecordsHaveStableUniqueIdentityAndProviderIdentifiers()
    {
        var first = new GenealogyRecord { Kind = "Person" };
        var second = new GenealogyRecord { Kind = "Person" };
        first.Identifiers.Add(new GenealogyIdentifier { Provider = "gedcom", Value = "@I1@" });

        Assert.AreNotEqual(Guid.Empty, first.Id);
        Assert.AreNotEqual(first.Id, second.Id);
        Assert.AreEqual("@I1@", first.Identifiers[0].Value);
    }

    [TestMethod]
    public void RecordContentPreservesOrderAndNestedUserEvents()
    {
        var record = new GenealogyRecord { Kind = "Person" };
        var birth = new GenealogyFact { TypeCode = "Birth" };
        var privateEvent = new UserDefinedEvent { TypeCode = "_CUSTOM", Context = "Birth" };
        privateEvent.Children.Add(new GenealogyFact { TypeCode = "Value", Value = "retained" });
        birth.Children.Add(privateEvent);
        record.Content.Add(birth);
        record.Content.Add(new GenealogyFact { TypeCode = "Occupation", Value = "Farmer" });

        Assert.AreEqual("Birth", record.Content[0].TypeCode);
        Assert.AreEqual("_CUSTOM", record.Content[0].Children[0].TypeCode);
        Assert.AreEqual("retained", record.Content[0].Children[0].Children[0].Value);
        Assert.AreEqual("Occupation", record.Content[1].TypeCode);
    }
}
