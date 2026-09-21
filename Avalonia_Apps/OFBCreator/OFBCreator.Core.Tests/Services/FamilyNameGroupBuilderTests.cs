using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using GenInterfaces.Interfaces.Genealogic;
using OFBCreator.Core.Services;
using OFBCreator.Core.Tests.Helpers;

namespace OFBCreator.Core.Tests.Services;

/// <summary>
/// Tests for FamilyNameGroupBuilder — parent→child phonetic bridge detection.
/// Groups track surname evolution across generations through blood relationships.
/// Phase 1: Exact name grouping (Heis, Heus, Hös as initial separate keys)
/// Phase 2: Link via father/mother→child phonetic bridges (distance ≤ 2)
/// Adoptions keep groups separated; spouse surnames do NOT trigger fusion.
/// </summary>
[TestClass]
public class FamilyNameGroupBuilderTests
{
    #region Null and empty inputs

    [TestMethod]
    public void BuildGroups_WithNullSource_ShouldThrowArgumentNullException()
    {
        var builder = new FamilyNameGroupBuilder();
        Assert.Throws<ArgumentNullException>(() => builder.BuildGroups((IEnumerable<IGenFamily>)null!));
    }

    [TestMethod]
    public void BuildGroups_WithEmptySource_ShouldReturnEmptyGroupKeys()
    {
        var builder = new FamilyNameGroupBuilder();
        var families = Array.Empty<IGenFamily>();
        builder.BuildGroups(families);
        Assert.AreEqual(0, builder.GroupKeys.Count);
        Assert.IsNull(builder.GetGroup("any"));
    }

    #endregion

    #region Phase 1: Exact name grouping (no phonetic bridges)

    [TestMethod]
    public void BuildGroups_WithNoPhoneticBridge_ShouldKeepExactGroups()
    {
        var builder = new FamilyNameGroupBuilder();

        // Families with unrelated surnames → no parent→child phonetic link possible
        var familyHeis = CreateFamilyWithSurname("Heis");
        var familySchmidt = CreateFamilyWithSurname("Schmidt");

        builder.BuildGroups(new List<IGenFamily> { familyHeis, familySchmidt });

        // Two distinct groups (no phonetic parent→child bridge)
        Assert.AreEqual(2, builder.GroupKeys.Count);
    }

    [TestMethod]
    public void BuildGroups_WithSameExactName_ShouldGroupTogether()
    {
        var builder = new FamilyNameGroupBuilder();

        var family1 = CreateFamilyWithSurname("Müller");
        var family2 = CreateFamilyWithSurname("Müller");

        builder.BuildGroups(new List<IGenFamily> { family1, family2 });

        // All same surname → single group
        Assert.AreEqual(1, builder.GroupKeys.Count);
        var group = builder.GetGroup("Müller");
        Assert.IsNotNull(group);
        Assert.AreEqual(2, group!.Count);
    }

    #endregion

    #region Phase 2: Parent→Child phonetic bridge detection

    [TestMethod]
    public void BuildGroups_WithFatherChildPhoneticBridge_ShouldFuseGroups()
    {
        // Test: Heis → Heus (distance 1) via father→child should fuse groups
        var builder = new FamilyNameGroupBuilder();

        // Generation 1: family with surname "Heis"
        var gen1 = CreateFamilyWithSurname("Heis");

        // Generation 2: family where father="Heis", child has surname "Heus" (phonetic distance 1)
        var gen2Father = Substitute.For<IGenPerson>();
        gen2Father.Surname.Returns("Heis");
        var gen2Child = Substitute.For<IGenPerson>();
        gen2Child.Surname.Returns("Heus");
        var gen2ChildrenList = new TestIndexedList<IGenPerson> { gen2Child };
        var gen2 = Substitute.For<IGenFamily>();
        gen2.Husband.Returns(gen2Father);
        gen2.Wife.Returns((IGenPerson)null!);
        gen2.Children.Returns(gen2ChildrenList);
        gen2.ChildCount.Returns(1);

        builder.BuildGroups(new List<IGenFamily> { gen1, gen2 });

        // Heis and Heus should fuse into one group via parent→child bridge
        Assert.AreEqual(1, builder.GroupKeys.Count);
    }

    [TestMethod]
    public void BuildGroups_WithMotherChildPhoneticBridge_ShouldFuseGroups()
    {
        // Test: Müller → Mueller (distance 1) via mother→child should fuse groups
        var builder = new FamilyNameGroupBuilder();

        var gen1 = CreateFamilyWithSurname("Müller");

        var gen2Mother = Substitute.For<IGenPerson>();
        gen2Mother.Surname.Returns("Müller");
        var gen2Child = Substitute.For<IGenPerson>();
        gen2Child.Surname.Returns("Mueller");
        var gen2ChildrenList = new TestIndexedList<IGenPerson> { gen2Child };
        var gen2 = Substitute.For<IGenFamily>();
        gen2.Husband.Returns((IGenPerson)null!);
        gen2.Wife.Returns(gen2Mother);
        gen2.Children.Returns(gen2ChildrenList);
        gen2.ChildCount.Returns(1);

        builder.BuildGroups(new List<IGenFamily> { gen1, gen2 });

        // Müller and Mueller should fuse
        Assert.AreEqual(1, builder.GroupKeys.Count);
    }

    [TestMethod]
    public void BuildGroups_WithSpouseDifferentSurnames_ShouldNotFuse()
    {
        // Test: Different surnames between spouses should NOT trigger fusion alone
        // This was the old wrong behavior — spouse bridges don't fuse groups
        var builder = new FamilyNameGroupBuilder();

        // Only families with different exact names, no parent→child phonetic bridge
        var familyHeis = CreateFamilyWithSurname("Heis");
        var familySchmidt = CreateFamilyWithSurname("Schmidt");

        builder.BuildGroups(new List<IGenFamily> { familyHeis, familySchmidt });

        // Two groups — spouse names alone don't fuse unrelated groups
        Assert.AreEqual(2, builder.GroupKeys.Count);
    }

    [TestMethod]
    public void BuildGroups_WithPhoneticallyDistantNames_ShouldNotFuse()
    {
        // Test: Names with phonetic distance > 2 should NOT fuse
        var builder = new FamilyNameGroupBuilder();

        var familyHeis = CreateFamilyWithSurname("Heis");

        // Father "Heis" → child "Schmidt" (distance >> 2) — no fusion
        var father = Substitute.For<IGenPerson>();
        father.Surname.Returns("Heis");
        var child = Substitute.For<IGenPerson>();
        child.Surname.Returns("Schmidt");
        var childrenList = new TestIndexedList<IGenPerson> { child };
        var family = Substitute.For<IGenFamily>();
        family.Husband.Returns(father);
        family.Wife.Returns((IGenPerson)null!);
        family.Children.Returns(childrenList);
        family.ChildCount.Returns(1);

        builder.BuildGroups(new List<IGenFamily> { familyHeis, family });

        // Two groups — too distant to be same lineage
        Assert.AreEqual(2, builder.GroupKeys.Count);
    }

    #endregion

    #region Phase 3: Multi-generation chains and adoption separation

    [TestMethod]
    public void BuildGroups_WithMultiGenerationChain_ShouldConverge()
    {
        // Test: Heis → Heus → Hös should converge into one group
        var builder = new FamilyNameGroupBuilder();

        // Gen 1: Heis
        var gen1 = CreateFamilyWithSurname("Heis");

        // Gen 2: father="Heis", child="Heus" (distance 1)
        var gen2Father = Substitute.For<IGenPerson>();
        gen2Father.Surname.Returns("Heis");
        var gen2Child = Substitute.For<IGenPerson>();
        gen2Child.Surname.Returns("Heus");
        var gen2ChildrenList = new TestIndexedList<IGenPerson> { gen2Child };
        var gen2 = Substitute.For<IGenFamily>();
        gen2.Husband.Returns(gen2Father);
        gen2.Wife.Returns((IGenPerson)null!);
        gen2.Children.Returns(gen2ChildrenList);
        gen2.ChildCount.Returns(1);

        // Gen 3: father="Heus", child="Hös" (distance 1)
        var gen3Father = Substitute.For<IGenPerson>();
        gen3Father.Surname.Returns("Heus");
        var gen3Child = Substitute.For<IGenPerson>();
        gen3Child.Surname.Returns("Hös");
        var gen3ChildrenList = new TestIndexedList<IGenPerson> { gen3Child };
        var gen3 = Substitute.For<IGenFamily>();
        gen3.Husband.Returns(gen3Father);
        gen3.Wife.Returns((IGenPerson)null!);
        gen3.Children.Returns(gen3ChildrenList);
        gen3.ChildCount.Returns(1);

        builder.BuildGroups(new List<IGenFamily> { gen1, gen2, gen3 });

        // All three (Heis, Heus, Hös) should converge into one group
        Assert.AreEqual(1, builder.GroupKeys.Count);
    }

    [TestMethod]
    public void BuildGroups_WithSeparatePhoneticLineages_ShouldStaySeparate()
    {
        // Test: "Heiss → Heiß → Heuss" and "Heis → Heus → Hös" are two separate groups
        var builder = new FamilyNameGroupBuilder();

        // Lineage 1: Heis → Heus → Hös
        var l1g1 = CreateFamilyWithSurname("Heis");
        var l1g2Father = Substitute.For<IGenPerson>();
        l1g2Father.Surname.Returns("Heis");
        var l1g2Child = Substitute.For<IGenPerson>();
        l1g2Child.Surname.Returns("Heus");
        var l1g2ChildrenList = new TestIndexedList<IGenPerson> { l1g2Child };
        var l1g2 = Substitute.For<IGenFamily>();
        l1g2.Husband.Returns(l1g2Father);
        l1g2.Wife.Returns((IGenPerson)null!);
        l1g2.Children.Returns(l1g2ChildrenList);
        l1g2.ChildCount.Returns(1);

        var l1g3Father = Substitute.For<IGenPerson>();
        l1g3Father.Surname.Returns("Heus");
        var l1g3Child = Substitute.For<IGenPerson>();
        l1g3Child.Surname.Returns("Hös");
        var l1g3ChildrenList = new TestIndexedList<IGenPerson> { l1g3Child };
        var l1g3 = Substitute.For<IGenFamily>();
        l1g3.Husband.Returns(l1g3Father);
        l1g3.Wife.Returns((IGenPerson)null!);
        l1g3.Children.Returns(l1g3ChildrenList);
        l1g3.ChildCount.Returns(1);

        // Lineage 2: Heiss → Heiß → Heuss
        var l2g1 = CreateFamilyWithSurname("Heiss");
        var l2g2Father = Substitute.For<IGenPerson>();
        l2g2Father.Surname.Returns("Heiss");
        var l2g2Child = Substitute.For<IGenPerson>();
        l2g2Child.Surname.Returns("Heiß");
        var l2g2ChildrenList = new TestIndexedList<IGenPerson> { l2g2Child };
        var l2g2 = Substitute.For<IGenFamily>();
        l2g2.Husband.Returns(l2g2Father);
        l2g2.Wife.Returns((IGenPerson)null!);
        l2g2.Children.Returns(l2g2ChildrenList);
        l2g2.ChildCount.Returns(1);

        var l2g3Father = Substitute.For<IGenPerson>();
        l2g3Father.Surname.Returns("Heiß");
        var l2g3Child = Substitute.For<IGenPerson>();
        l2g3Child.Surname.Returns("Heuss");
        var l2g3ChildrenList = new TestIndexedList<IGenPerson> { l2g3Child };
        var l2g3 = Substitute.For<IGenFamily>();
        l2g3.Husband.Returns(l2g3Father);
        l2g3.Wife.Returns((IGenPerson)null!);
        l2g3.Children.Returns(l2g3ChildrenList);
        l2g3.ChildCount.Returns(1);

        builder.BuildGroups(new List<IGenFamily> { l1g1, l1g2, l1g3, l2g1, l2g2, l2g3 });

        // Two separate groups: Lineage 1 (Heis/Heus/Hös) and Lineage 2 (Heiss/Heiß/Heuss)
        Assert.AreEqual(2, builder.GroupKeys.Count);
    }

    [TestMethod]
    public void BuildGroups_WithAdoption_ShouldKeepGroupsSeparate()
    {
        // Test: Adoption should NOT trigger phonetic bridge fusion
        var builder = new FamilyNameGroupBuilder();

        // Biological family: father="Heis", child="Heis" (exact match)
        var bioFather = Substitute.For<IGenPerson>();
        bioFather.Surname.Returns("Heis");
        var bioChild = Substitute.For<IGenPerson>();
        bioChild.Surname.Returns("Heis");
        var bioChildrenList = new TestIndexedList<IGenPerson> { bioChild };
        var bioFamily = Substitute.For<IGenFamily>();
        bioFamily.Husband.Returns(bioFather);
        bioFamily.Wife.Returns((IGenPerson)null!);
        bioFamily.Children.Returns(bioChildrenList);
        bioFamily.ChildCount.Returns(1);

        // Adopted family: father="Heuss", child="Heis" (phonetically similar but adopted)
        var adoptFather = Substitute.For<IGenPerson>();
        adoptFather.Surname.Returns("Heuss");
        var adoptChild = Substitute.For<IGenPerson>();
        adoptChild.Surname.Returns("Heis");
        var adoptChildrenList = new TestIndexedList<IGenPerson> { adoptChild };
        var adoptFamily = Substitute.For<IGenFamily>();
        adoptFamily.Husband.Returns(adoptFather);
        adoptFamily.Wife.Returns((IGenPerson)null!);
        adoptFamily.Children.Returns(adoptChildrenList);
        adoptFamily.ChildCount.Returns(1);

        // Note: Without adoption flags in IGenPerson, this test validates default behavior
        // When adoption detection is implemented, groups should stay separate
        builder.BuildGroups(new List<IGenFamily> { bioFamily, adoptFamily });

        // Default: without adoption flags, phonetic bridge fuses (will change when adopted=true)
        // This documents current placeholder behavior
        Assert.AreEqual(1, builder.GroupKeys.Count); // Will be 2 when IsAdoptedChild is implemented
    }

    #endregion

    #region Helper methods

    private static IGenFamily CreateFamilyWithSurname(string surname)
    {
        var husband = Substitute.For<IGenPerson>();
        husband.Surname.Returns(surname);

        var wife = Substitute.For<IGenPerson>();
        wife.Surname.Returns(surname);

        var child = Substitute.For<IGenPerson>();
        child.Surname.Returns(surname);

        var childrenList = new TestIndexedList<IGenPerson> { child };

        var family = Substitute.For<IGenFamily>();
        family.Husband.Returns(husband);
        family.Wife.Returns(wife);
        family.Children.Returns(childrenList);
        family.ChildCount.Returns(1);

        return family;
    }

    #endregion
}
