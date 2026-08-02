using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RnzTrauer.Core.Domain;
using RnzTrauer.Core.Services;

namespace RnzTrauer.Core.Tests;

[TestClass]
public sealed class NoticeTextParserTests
{
    [TestMethod]
    public void Parse_ExtractsGermanDatesMaidenNamePlaceAndAge()
    {
        var notice = new DeathNotice
        {
            FamilyName = "Müller",
            Category = AdvertisementCategory.DeathNotice,
        };
        var parser = new NoticeTextParser();

        var facts = parser.Parse(
            notice,
            "Maria Müller, geborene Schmidt, geboren am 03.04.1940, verst. am 12.05.2024. " +
            "Die Beisetzung findet in Heidelberg statt. Im Alter von 84 Jahren.",
            ["Heidelberg", "Baden"]);

        Assert.AreEqual(new DateTime(1940, 4, 3), facts.BirthDate);
        Assert.AreEqual(new DateTime(2024, 5, 12), facts.DeathDate);
        Assert.AreEqual("Schmidt", facts.MaidenName);
        Assert.AreEqual("Heidelberg", facts.Place);
        Assert.AreEqual(84, facts.Age);
    }

    [TestMethod]
    public void Parse_RecognizesQuietBurialAsDeathNoticeWithoutBurial()
    {
        var notice = new DeathNotice
        {
            Category = AdvertisementCategory.DeathNotice,
        };
        var parser = new NoticeTextParser();

        var facts = parser.Parse(notice, "Wir nehmen in aller Stille Abschied. Gest. am 2. März 2024.", []);

        Assert.AreEqual(new DateTime(2024, 3, 2), facts.DeathDate);
        Assert.IsNull(facts.BurialDate);
        Assert.AreEqual(AdvertisementCategory.DeathNoticeWithoutBurial, facts.AdjustedCategory);
    }

    [TestMethod]
    public void Parse_MatchesPascalAnzText1GoldenFacts()
    {
        var fixtureDirectory = Path.Combine(AppContext.BaseDirectory, "Fixtures", "Pascal");
        var text = File.ReadAllText(Path.Combine(fixtureDirectory, "AnzText1.txt"));
        var expected = File.ReadAllLines(Path.Combine(fixtureDirectory, "AnzText1_Erg.txt"));
        var notice = new DeathNotice { Category = AdvertisementCategory.DeathNotice };

        var facts = new NoticeTextParser().Parse(
            notice,
            text,
            new List<string> { "Sinsheim-Rohrbach", "Sinsheim", "Neckarsteinach", "Heidelberg" });

        Assert.AreEqual("dkBirth=23. 3. 1932", expected[0]);
        Assert.AreEqual("dkDeath=27. 7. 2018", expected[1]);
        Assert.AreEqual("dkPlace=Neckarsteinach", expected[2]);
        Assert.AreEqual("dkBurial=31. 07. 2018", expected[3]);
        Assert.AreEqual(new DateTime(1932, 3, 23), facts.BirthDate);
        Assert.AreEqual(new DateTime(2018, 7, 27), facts.DeathDate);
        Assert.AreEqual(new DateTime(2018, 7, 31), facts.BurialDate);
        Assert.AreEqual("Neckarsteinach", facts.Place);
    }

    [TestMethod]
    public void Parse_MatchesPascalAnzText2GoldenFacts()
    {
        var fixtureDirectory = Path.Combine(AppContext.BaseDirectory, "Fixtures", "Pascal");
        var text = File.ReadAllText(Path.Combine(fixtureDirectory, "AnzText2.txt"));
        var expected = File.ReadAllLines(Path.Combine(fixtureDirectory, "AnzText2_Erg.txt"));
        var facts = new NoticeTextParser().Parse(
            new DeathNotice { Category = AdvertisementCategory.DeathNotice },
            text,
            new List<string> { "Sinsheim-Rohrbach", "Sinsheim" });

        Assert.AreEqual(new DateTime(1932, 11, 29), facts.BirthDate);
        Assert.AreEqual(new DateTime(2018, 7, 28), facts.DeathDate);
        Assert.AreEqual(new DateTime(2018, 8, 2), facts.BurialDate);
        Assert.AreEqual(expected[2]["dkPlace=".Length..], facts.Place);
        Assert.AreEqual(expected[5]["dkMaidenname=".Length..], facts.MaidenName);
    }

    [TestMethod]
    public void Parse_MatchesPascalAnzText3GoldenFacts()
    {
        var fixtureDirectory = Path.Combine(AppContext.BaseDirectory, "Fixtures", "Pascal");
        var text = File.ReadAllText(Path.Combine(fixtureDirectory, "AnzText3.txt"));
        var expected = File.ReadAllLines(Path.Combine(fixtureDirectory, "AnzText3_Erg.txt"));
        var facts = new NoticeTextParser().Parse(
            new DeathNotice { Category = AdvertisementCategory.Thanks },
            text,
            new List<string> { "Sinsheim", "Heidelberg" });

        Assert.AreEqual(expected[0]["dkPlace=".Length..], facts.Place);
        Assert.AreEqual(expected[2]["dkMaidenname=".Length..], facts.MaidenName);
    }

    [TestMethod]
    public void Parse_MatchesPascalAnzText4GoldenFacts()
    {
        var fixtureDirectory = Path.Combine(AppContext.BaseDirectory, "Fixtures", "Pascal");
        var text = File.ReadAllText(Path.Combine(fixtureDirectory, "AnzText4.txt"));
        var expected = File.ReadAllLines(Path.Combine(fixtureDirectory, "AnzText4_Erg.txt"));
        var facts = new NoticeTextParser().Parse(
            new DeathNotice { Category = AdvertisementCategory.CorporateObituary },
            text,
            new List<string> { "Handschuhsheim", "Heidelberg" });

        Assert.AreEqual(new DateTime(2017, 1, 15), facts.DeathDate);
        Assert.AreEqual(73, facts.Age);
        Assert.AreEqual(expected[2]["dkPlace=".Length..], facts.Place);
    }

    [TestMethod]
    public void Parse_MatchesPascalAnzText5GoldenFacts()
    {
        var fixtureDirectory = Path.Combine(AppContext.BaseDirectory, "Fixtures", "Pascal");
        var text = File.ReadAllText(Path.Combine(fixtureDirectory, "AnzText5.txt"));
        var expected = File.ReadAllLines(Path.Combine(fixtureDirectory, "AnzText5_Erg.txt"));
        var facts = new NoticeTextParser().Parse(
            new DeathNotice { Category = AdvertisementCategory.CorporateObituary },
            text,
            new List<string> { "Heidelberg", "Sinsheim" });

        Assert.AreEqual(new DateTime(2017, 1, 15), facts.DeathDate);
        Assert.AreEqual(75, facts.Age);
        Assert.IsNull(facts.Place);
        Assert.IsTrue(expected[2].StartsWith("dkSex=", StringComparison.Ordinal));
    }

    [TestMethod]
    public void Parse_MatchesPascalAnzText6GoldenFacts()
    {
        var fixtureDirectory = Path.Combine(AppContext.BaseDirectory, "Fixtures", "Pascal");
        var text = File.ReadAllText(Path.Combine(fixtureDirectory, "AnzText6.txt"));
        var expected = File.ReadAllLines(Path.Combine(fixtureDirectory, "AnzText6_Erg.txt"));
        var facts = new NoticeTextParser().Parse(
            new DeathNotice { Category = AdvertisementCategory.DeathNotice },
            text,
            new List<string> { "Heidelberg", "Bammental" });

        Assert.AreEqual(new DateTime(1947, 3, 5), facts.BirthDate);
        Assert.AreEqual(new DateTime(2018, 12, 12), facts.DeathDate);
        Assert.AreEqual(new DateTime(2019, 1, 11), facts.BurialDate);
        Assert.AreEqual(expected[2]["dkPlace=".Length..], facts.Place);
        Assert.AreEqual(
            expected[5]["dkMaidenname=".Length..].ToUpperInvariant(),
            facts.MaidenName?.ToUpperInvariant());
    }

    [TestMethod]
    public void Parse_MatchesPascalAnzText7GoldenFacts()
    {
        var fixtureDirectory = Path.Combine(AppContext.BaseDirectory, "Fixtures", "Pascal");
        var text = File.ReadAllText(Path.Combine(fixtureDirectory, "AnzText7.txt"));
        var expected = File.ReadAllLines(Path.Combine(fixtureDirectory, "AnzText7_Erg.txt"));
        var facts = new NoticeTextParser().Parse(
            new DeathNotice { Category = AdvertisementCategory.DeathNotice },
            text,
            new List<string> { "Heidelberg", "Sinsheim" });

        Assert.AreEqual(new DateTime(1923, 3, 24), facts.BirthDate);
        Assert.AreEqual(new DateTime(2018, 12, 21), facts.DeathDate);
        Assert.AreEqual(new DateTime(2019, 1, 4), facts.BurialDate);
        Assert.AreEqual(expected[2]["dkPlace=".Length..], facts.Place);
        Assert.AreEqual(expected[5]["dkMaidenname=".Length..], facts.MaidenName);
    }

    [TestMethod]
    public void Parse_MatchesPascalAnzText8GoldenFacts()
    {
        var fixtureDirectory = Path.Combine(AppContext.BaseDirectory, "Fixtures", "Pascal");
        var text = File.ReadAllText(Path.Combine(fixtureDirectory, "AnzText8.txt"));
        var expected = File.ReadAllLines(Path.Combine(fixtureDirectory, "AnzText8_Erg.txt"));
        var facts = new NoticeTextParser().Parse(
            new DeathNotice { Category = AdvertisementCategory.DeathNotice },
            text,
            new List<string> { "Sinsheim-Rohrbach", "Sinsheim" });

        Assert.AreEqual(new DateTime(1941, 11, 29), facts.BirthDate);
        Assert.AreEqual(new DateTime(2018, 12, 19), facts.DeathDate);
        Assert.AreEqual(new DateTime(2019, 1, 4), facts.BurialDate);
        Assert.AreEqual(expected[2]["dkPlace=".Length..], facts.Place);
    }

    [TestMethod]
    public void Parse_MatchesPascalAnzText9GoldenFacts()
    {
        var fixtureDirectory = Path.Combine(AppContext.BaseDirectory, "Fixtures", "Pascal");
        var text = File.ReadAllText(Path.Combine(fixtureDirectory, "AnzText9.txt"));
        var expected = File.ReadAllLines(Path.Combine(fixtureDirectory, "AnzText9_Erg.txt"));
        var facts = new NoticeTextParser().Parse(
            new DeathNotice { Category = AdvertisementCategory.DeathNotice },
            text,
            new List<string> { "Sinsheim", "Heidelberg" });

        Assert.AreEqual(new DateTime(1932, 6, 20), facts.BirthDate);
        Assert.AreEqual(new DateTime(2018, 12, 24), facts.DeathDate);
        Assert.AreEqual(new DateTime(2019, 1, 4), facts.BurialDate);
        Assert.AreEqual(expected[2]["dkPlace=".Length..], facts.Place);
    }

    [TestMethod]
    public void Parse_MatchesPascalAnzText10PartialFactsWithoutInventingYear()
    {
        var fixtureDirectory = Path.Combine(AppContext.BaseDirectory, "Fixtures", "Pascal");
        var text = File.ReadAllText(Path.Combine(fixtureDirectory, "AnzText10.txt"));
        var expected = File.ReadAllLines(Path.Combine(fixtureDirectory, "AnzText10_Erg.txt"));
        var facts = new NoticeTextParser().Parse(
            new DeathNotice { Category = AdvertisementCategory.CorporateObituary },
            text,
            new List<string> { "Sinsheim-Rohrbach", "Sinsheim" });

        Assert.IsNull(facts.BirthDate);
        Assert.IsNull(facts.DeathDate);
        Assert.IsNull(facts.BurialDate);
        Assert.AreEqual(75, facts.Age);
        Assert.AreEqual(expected[2]["dkPlace=".Length..], facts.Place);
    }

    [TestMethod]
    public void Parse_MatchesPascalAnzText12GoldenFacts()
    {
        var fixtureDirectory = Path.Combine(AppContext.BaseDirectory, "Fixtures", "Pascal");
        var text = File.ReadAllText(Path.Combine(fixtureDirectory, "AnzText12.txt"));
        var expected = File.ReadAllLines(Path.Combine(fixtureDirectory, "AnzText12_Erg.txt"));
        var facts = new NoticeTextParser().Parse(
            new DeathNotice { Category = AdvertisementCategory.DeathNotice },
            text,
            new List<string> { "Neckarsteinach", "Heidelberg" });

        Assert.AreEqual(new DateTime(1945, 3, 12), facts.BirthDate);
        Assert.AreEqual(new DateTime(2019, 9, 17), facts.DeathDate);
        Assert.AreEqual(new DateTime(2019, 9, 30), facts.BurialDate);
        Assert.AreEqual(expected[2]["dkPlace=".Length..], facts.Place);
    }

    [TestMethod]
    public void Parse_MatchesPascalAnzText13NegativeFacts()
    {
        var fixtureDirectory = Path.Combine(AppContext.BaseDirectory, "Fixtures", "Pascal");
        var text = File.ReadAllText(Path.Combine(fixtureDirectory, "AnzText13.txt"));
        var expected = File.ReadAllLines(Path.Combine(fixtureDirectory, "AnzText13_Erg.txt"));
        var facts = new NoticeTextParser().Parse(
            new DeathNotice { FamilyName = "Musterfrau", Category = AdvertisementCategory.Thanks },
            text,
            new List<string> { "Sinsheim", "Heidelberg" });

        Assert.AreEqual("dkSex=M", expected[0]);
        Assert.IsNull(facts.BirthDate);
        Assert.IsNull(facts.DeathDate);
        Assert.IsNull(facts.BurialDate);
        Assert.IsNull(facts.Place);
        Assert.IsNull(facts.MaidenName);
        Assert.IsNull(facts.Age);
    }

    [TestMethod]
    public void Parse_MatchesPascalAnzText14UnmarkedDatePair()
    {
        var fixtureDirectory = Path.Combine(AppContext.BaseDirectory, "Fixtures", "Pascal");
        var text = File.ReadAllText(Path.Combine(fixtureDirectory, "AnzText14.txt"));
        var facts = new NoticeTextParser().Parse(new DeathNotice { Category = AdvertisementCategory.DeathNotice }, text, []);

        Assert.AreEqual(new DateTime(1940, 3, 29), facts.BirthDate);
        Assert.AreEqual(new DateTime(2020, 12, 13), facts.DeathDate);
    }

    [TestMethod]
    public void Parse_MatchesPascalAnzText15UnmarkedDatesAndBurial()
    {
        var fixtureDirectory = Path.Combine(AppContext.BaseDirectory, "Fixtures", "Pascal");
        var text = File.ReadAllText(Path.Combine(fixtureDirectory, "AnzText15.txt"));
        var facts = new NoticeTextParser().Parse(new DeathNotice { Category = AdvertisementCategory.DeathNotice }, text, []);

        Assert.AreEqual(new DateTime(1941, 9, 17), facts.BirthDate);
        Assert.AreEqual(new DateTime(2021, 3, 30), facts.DeathDate);
        Assert.AreEqual(new DateTime(2021, 4, 8), facts.BurialDate);
        Assert.AreEqual("Buchloh", facts.MaidenName);
    }

    [TestMethod]
    public void Parse_MatchesPascalAnzText16CategoryAndAge()
    {
        var fixtureDirectory = Path.Combine(AppContext.BaseDirectory, "Fixtures", "Pascal");
        var text = File.ReadAllText(Path.Combine(fixtureDirectory, "AnzText16.txt"));
        var facts = new NoticeTextParser().Parse(new DeathNotice { Category = AdvertisementCategory.DeathNotice }, text, []);

        Assert.AreEqual(new DateTime(2021, 4, 5), facts.DeathDate);
        Assert.AreEqual(81, facts.Age);
        Assert.AreEqual(AdvertisementCategory.DeathNoticeWithoutBurial, facts.AdjustedCategory);
    }
}
