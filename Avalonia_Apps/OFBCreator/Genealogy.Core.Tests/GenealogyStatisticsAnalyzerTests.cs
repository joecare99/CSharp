using System;
using System.Collections.Generic;
using System.Linq;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
using Genealogy.Statistics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Genealogy.Core.Tests;

[TestClass]
public sealed class GenealogyStatisticsAnalyzerTests
{
    [TestMethod]
    public void Analyze_FindsBirthAgeAndMarriageExtremes()
    {
        var early = CreatePerson("F", CreateDate(1600, EDateType.Year));
        var late = CreatePerson("M", CreateDate(1900, EDateType.Year));
        var oldestWoman = CreatePerson("F", CreateDate(1700, EDateType.Year), CreateDate(1790, EDateType.Year));
        var oldestMan = CreatePerson("M", CreateDate(1701, EDateType.Year), CreateDate(1785, EDateType.Year));
        var firstWedding = CreateFamily(CreateDate(1740, EDateType.Year));
        var lastWedding = CreateFamily(CreateDate(1940, EDateType.Year));
        SetMarriages(oldestWoman, firstWedding);
        SetMarriages(late, lastWedding);
        SetMarriages(oldestMan, firstWedding);

        var result = GenealogyStatisticsAnalyzer.Analyze(
            new[] { early, late, oldestWoman, oldestMan },
            1600,
            2000);

        Assert.AreSame(early, result.EarliestPerson!.Person);
        Assert.AreSame(late, result.LatestPerson!.Person);
        Assert.AreSame(oldestWoman, result.OldestWoman!.Person);
        Assert.AreSame(oldestMan, result.OldestMan!.Person);
        Assert.AreSame(firstWedding, result.EarliestMarriage!.Family);
        Assert.AreSame(lastWedding, result.LatestMarriage!.Family);
    }

    [TestMethod]
    public void Analyze_SplitsPossibleBirthDatesAcrossYearsAndCountsThemOnce()
    {
        var person = CreatePerson("F", CreateDate(
            new DateTime(1899, 12, 31),
            EDateType.Full,
            new DateTime(1900, 1, 1),
            EDateType.Full));

        var result = GenealogyStatisticsAnalyzer.Analyze(new[] { person }, 1899, 1900);

        Assert.AreEqual(0.5, result.Years[0].Births, 0.0001);
        Assert.AreEqual(0.5, result.Years[1].Births, 0.0001);
        Assert.AreEqual(1.0, result.Years.Sum(year => year.Births), 0.0001);
    }

    [TestMethod]
    public void Analyze_CountsKnownBirthsDeathsAndLivingPeopleAtMidYear()
    {
        var person = CreatePerson(
            "M",
            CreateDate(new DateTime(1900, 1, 1)),
            CreateDate(new DateTime(1901, 12, 31)));

        var result = GenealogyStatisticsAnalyzer.Analyze(new[] { person }, 1899, 1902);

        Assert.AreEqual(0, result.Years[0].LivingPeople, 0.0001);
        Assert.AreEqual(1, result.Years[1].Births, 0.0001);
        Assert.AreEqual(1, result.Years[1].LivingPeople, 0.0001);
        Assert.AreEqual(1, result.Years[2].Deaths, 0.0001);
        Assert.AreEqual(1, result.Years[2].LivingPeople, 0.0001);
        Assert.AreEqual(0, result.Years[3].LivingPeople, 0.0001);
    }

    [TestMethod]
    public void Analyze_UsesBaptismWhenBirthDateIsMissing()
    {
        var person = CreatePerson("F", null);
        var baptismDate = CreateDate(1800, EDateType.Year);
        person.BaptDate.Returns(baptismDate);

        var result = GenealogyStatisticsAnalyzer.Analyze(new[] { person }, 1800, 1801);

        Assert.AreSame(person, result.EarliestPerson!.Person);
        Assert.AreEqual(1800, result.EarliestPerson.PossibleBirthStart.Year);
        Assert.AreEqual(1.0, result.Years[0].Births, 0.0001);
    }

    [TestMethod]
    public void Analyze_UsesBurialWhenDeathDateIsMissing()
    {
        var person = CreatePerson("M", CreateDate(new DateTime(1800, 1, 1)));
        var burialDate = CreateDate(new DateTime(1801, 1, 1));
        person.BurialDate.Returns(burialDate);

        var result = GenealogyStatisticsAnalyzer.Analyze(new[] { person }, 1800, 1802);

        Assert.AreEqual(1.0, result.Years[1].Deaths, 0.0001);
        Assert.AreEqual(0.0, result.Years[2].LivingPeople, 0.0001);
    }

    [TestMethod]
    public void Analyze_UsesBaptismWhenBirthDateIsUnusable()
    {
        var unusableBirthDate = CreateDate(new DateTime(1800, 1, 1), modifier: EDateModifier.Before);
        var person = CreatePerson("F", unusableBirthDate);
        var baptismFact = CreateFact(EFactType.Baptism, CreateDate(1801, EDateType.Year));
        person.Baptism.Returns(baptismFact);

        var result = GenealogyStatisticsAnalyzer.Analyze(new[] { person }, 1800, 1802);

        Assert.AreSame(person, result.EarliestPerson!.Person);
        Assert.AreEqual(1801, result.EarliestPerson.PossibleBirthStart.Year);
        Assert.AreEqual(1.0, result.Years[1].Births, 0.0001);
    }

    [TestMethod]
    public void ChartModel_NormalizesPopulationSeparatelyFromBirthAndDeathEvents()
    {
        var person = CreatePerson(
            "M",
            CreateDate(new DateTime(1800, 1, 1)),
            CreateDate(new DateTime(1801, 1, 1)));
        var statistics = GenealogyStatisticsAnalyzer.Analyze(new[] { person }, 1800, 1801);

        var chart = GenealogyStatisticsChartModel.Create(statistics);

        Assert.AreEqual(1.0, chart.Points[0].LivingPeople, 0.0001);
        Assert.AreEqual(1.0, chart.Points[1].Deaths, 0.0001);
        Assert.AreEqual(0.0, chart.Points[1].LivingPeople, 0.0001);
        StringAssert.Contains(GenealogyStatisticsChartRenderer.RenderMonospace(chart), "1800");
        CollectionAssert.AreEqual(
            new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 },
            GenealogyStatisticsChartRenderer.RenderPng(chart).Take(8).ToArray());
    }

    [TestMethod]
    public void ChartModel_UsesPopulationSensitiveGaussianSmoothingAndConservesEvents()
    {
        var singlePerson = CreatePerson(
            "M",
            CreateDate(new DateTime(1900, 1, 1)),
            CreateDate(new DateTime(1901, 1, 1)));
        var singleStatistics = GenealogyStatisticsAnalyzer.Analyze(new[] { singlePerson }, 1870, 1930);
        var singleChart = GenealogyStatisticsChartModel.Create(singleStatistics);

        var manyPeople = Enumerable.Range(0, 300)
            .Select(_ => CreatePerson(
                "M",
                CreateDate(new DateTime(1900, 1, 1)),
                CreateDate(new DateTime(1901, 1, 1))))
            .ToArray();
        var manyStatistics = GenealogyStatisticsAnalyzer.Analyze(manyPeople, 1870, 1930);
        var manyChart = GenealogyStatisticsChartModel.Create(manyStatistics);

        var singleBirthTail = singleChart.Points.Single(point => point.Year == 1902).Births;
        var manyBirthTail = manyChart.Points.Single(point => point.Year == 1902).Births;
        Assert.IsTrue(singleBirthTail > manyBirthTail * 3);
        Assert.AreEqual(1.0, singleChart.Points.Sum(point => point.Births) * singleChart.MaximumEvents, 0.0001);
        Assert.AreEqual(300.0, manyChart.Points.Sum(point => point.Births) * manyChart.MaximumEvents, 0.001);
        Assert.AreEqual(1.0, singleChart.Points.Sum(point => point.Deaths) * singleChart.MaximumEvents, 0.0001);
        Assert.AreEqual(300.0, manyChart.Points.Sum(point => point.Deaths) * manyChart.MaximumEvents, 0.001);
        Assert.AreEqual(300.0, manyChart.MaximumLivingPeople, 0.001);
    }

    [TestMethod]
    public void AnalyzeForChart_UsesRoundedTenYearEventMarginsAndIncludesFamilyFoundings()
    {
        var person = CreatePerson(
            "M",
            CreateDate(new DateTime(1803, 1, 1)),
            CreateDate(new DateTime(1887, 1, 1)));
        var family = CreateFamily(CreateDate(new DateTime(1842, 1, 1)));
        var result = GenealogyStatisticsAnalyzer.AnalyzeForChart(new[] { person }, new[] { family });

        Assert.AreEqual(1790, result.Years[0].Year);
        Assert.AreEqual(1900, result.Years[^1].Year);
        Assert.AreEqual(1.0, result.Years.Single(year => year.Year == 1842).FamilyFoundings, 0.0001);
        var chart = GenealogyStatisticsChartModel.Create(result);
        Assert.IsTrue(chart.Points.Sum(point => point.FamilyFoundings) > 0);
        Assert.AreEqual(10, chart.TimeMajorStepYears);
        Assert.IsTrue(chart.TimeAxisTicks.Any(tick => tick.Value == 1800 && tick.IsCentury));
        Assert.IsTrue(chart.PopulationAxisTicks.Count >= 2);
        Assert.IsTrue(chart.EventAxisTicks.Count >= 2);
        StringAssert.Contains(GenealogyStatisticsChartRenderer.RenderMonospace(chart), "Familiengründungen");
    }

    [TestMethod]
    public void AnalyzeForChart_UsesExplicitRangeOverrides()
    {
        var person = CreatePerson("F", CreateDate(new DateTime(1842, 1, 1)));

        var result = GenealogyStatisticsAnalyzer.AnalyzeForChart(
            new[] { person }, Array.Empty<IGenFamily>(), 1840, 1850);

        Assert.AreEqual(1840, result.Years[0].Year);
        Assert.AreEqual(1850, result.Years[^1].Year);
    }

    [TestMethod]
    public void Analyze_UnknownDeathFadesLinearlyFromAge75To90()
    {
        var person = CreatePerson("F", CreateDate(new DateTime(1900, 7, 1)));

        var result = GenealogyStatisticsAnalyzer.Analyze(new[] { person }, 1974, 1991);

        Assert.AreEqual(1.0, result.Years.Single(year => year.Year == 1975).LivingPeople, 0.001);
        Assert.AreEqual(0.4667, result.Years.Single(year => year.Year == 1983).LivingPeople, 0.002);
        Assert.AreEqual(0.0, result.Years.Single(year => year.Year == 1990).LivingPeople, 0.001);
    }

    [TestMethod]
    public void Analyze_LaterPersonEventDelaysUnknownDeathFade()
    {
        var laterEvidence = Substitute.For<IGenFact>();
        laterEvidence.eFactType.Returns(EFactType.Occupation);
        var eventDate = CreateDate(new DateTime(1980, 7, 1));
        laterEvidence.Date.Returns(eventDate);
        var person = CreatePerson("M", CreateDate(new DateTime(1900, 7, 1)), facts: new List<IGenFact?> { laterEvidence });

        var result = GenealogyStatisticsAnalyzer.Analyze(new[] { person }, 1980, 1996);

        Assert.AreEqual(1.0, result.Years.Single(year => year.Year == 1980).LivingPeople, 0.001);
        Assert.AreEqual(0.4667, result.Years.Single(year => year.Year == 1988).LivingPeople, 0.002);
        Assert.AreEqual(0.0, result.Years.Single(year => year.Year == 1995).LivingPeople, 0.001);
    }

    [TestMethod]
    public void Analyze_IgnoresPeopleWithoutUsableBirthDatesAndUnboundedDates()
    {
        var person = CreatePerson("F", CreateDate(new DateTime(1900, 1, 1), modifier: EDateModifier.Before));

        var result = GenealogyStatisticsAnalyzer.Analyze(new[] { person }, 1800, 2000);

        Assert.IsNull(result.EarliestPerson);
        Assert.IsNull(result.LatestPerson);
        Assert.IsNull(result.OldestWoman);
        Assert.AreEqual(0, result.Years.Sum(year => year.Births), 0.0001);
        Assert.AreEqual(0, result.Years.Sum(year => year.LivingPeople), 0.0001);
    }

    [TestMethod]
    public void Analyze_CollectsRankedLabelsAveragesAndAgeDistribution()
    {
        var father = CreatePerson("M", CreateDate(1950, EDateType.Year));
        father.GivenName.Returns("Karl");
        father.Surname.Returns("Müller");
        var mother = CreatePerson("F", CreateDate(1952, EDateType.Year));
        mother.GivenName.Returns("Anna");
        mother.Surname.Returns("Müller");
        var child = CreatePerson(
            "M",
            CreateDate(1980, EDateType.Year),
            CreateDate(2020, EDateType.Year));
        child.GivenName.Returns("Karl");
        child.Surname.Returns("Müller");
        child.Father.Returns(father);
        child.Mother.Returns(mother);
        child.Occupation.Returns("Farmer");
        var occupationFact = CreateFact(EFactType.Occupation, null, "Farmer");
        child.Facts.Returns(new List<IGenFact?>
        {
            occupationFact
        });

        var result = GenealogyStatisticsAnalyzer.Analyze(new[] { father, mother, child }, 1950, 2020);

        Assert.AreEqual("Farmer", result.TopOccupations[0].Label);
        Assert.AreEqual(1, result.TopOccupations[0].Count);
        Assert.AreEqual("Karl", result.TopGivenNames[0].Label);
        Assert.AreEqual(2, result.TopGivenNames[0].Count);
        Assert.AreEqual("Müller", result.TopSurnames[0].Label);
        Assert.AreEqual(3, result.TopSurnames[0].Count);
        Assert.IsTrue(result.AverageAgeYears > 0);
        Assert.IsTrue(result.AverageDeathAgeYears > 0);
        Assert.IsTrue(result.AverageGenerationGapMaleYears > 0);
        Assert.IsTrue(result.AverageGenerationGapFemaleYears > 0);
        Assert.IsTrue(result.AverageGenerationGapGeneralYears > 0);
        Assert.IsTrue(result.AgeDistribution.Any(point => point.Age >= 40 && point.DeathAge > 0));
    }

    [TestMethod]
    public void AgeTimelineChartModel_UsesYearStatisticsAsChartInput()
    {
        var father = CreatePerson("M", CreateDate(1950, EDateType.Year));
        var mother = CreatePerson("F", CreateDate(1952, EDateType.Year));
        var child = CreatePerson("M", CreateDate(1980, EDateType.Year), CreateDate(2020, EDateType.Year));
        child.Father.Returns(father);
        child.Mother.Returns(mother);

        var result = GenealogyStatisticsAnalyzer.Analyze(new[] { father, mother, child }, 1950, 2020);
        var chart = GenealogyStatisticsAgeDistributionChartModel.Create(result);

        Assert.AreEqual(result.Years.Count, chart.Points.Count);
        Assert.AreEqual(result.Years[0].Year, chart.Points[0].Year);
        Assert.AreEqual(result.Years[^1].Year, chart.Points[^1].Year);
        Assert.IsTrue(chart.Points.Any(point => point.RawAge > 0));
        Assert.IsTrue(chart.TimeAxisTicks.All(tick => tick.IsMajor));
        Assert.IsTrue(chart.TimeAxisTicks.Count >= 2);
        Assert.IsTrue(chart.YAxisTicks.Any(tick => tick.IsMajor));
        Assert.IsTrue(chart.YAxisMaximum >= chart.MaximumValue);
    }

    [TestMethod]
    public void AgeTimelineChartModel_MarksCenturyTicks()
    {
        var person = CreatePerson("M", CreateDate(1900, EDateType.Year));

        var result = GenealogyStatisticsAnalyzer.Analyze(new[] { person }, 1890, 2020);
        var chart = GenealogyStatisticsAgeDistributionChartModel.Create(result);

        Assert.IsTrue(chart.TimeAxisTicks.Any(tick => tick.IsCentury));
        Assert.IsTrue(chart.TimeAxisTicks.Any(tick => tick.Value == 1900 && tick.IsCentury));
    }

    [TestMethod]
    public void AgeTimelineChartModel_UsesAdaptiveWindowingForSparseAndDenseData()
    {
        var sparsePeople = new[]
        {
            CreatePerson("M", CreateDate(new DateTime(1850, 1, 1)), CreateDate(new DateTime(1900, 12, 31))),
            CreatePerson("F", CreateDate(new DateTime(1800, 1, 1)), CreateDate(new DateTime(1905, 12, 31)))
        };
        var sparseStatistics = GenealogyStatisticsAnalyzer.Analyze(sparsePeople, 1890, 1910);
        var sparseChart = GenealogyStatisticsAgeDistributionChartModel.Create(sparseStatistics);

        var densePeople = Enumerable.Range(0, 300)
            .SelectMany(_ => new[]
            {
                CreatePerson("M", CreateDate(new DateTime(1850, 1, 1)), CreateDate(new DateTime(1900, 12, 31))),
                CreatePerson("F", CreateDate(new DateTime(1800, 1, 1)), CreateDate(new DateTime(1905, 12, 31)))
            })
            .ToArray();
        var denseStatistics = GenealogyStatisticsAnalyzer.Analyze(densePeople, 1890, 1910);
        var denseChart = GenealogyStatisticsAgeDistributionChartModel.Create(denseStatistics);

        var sparseMidPoint = sparseChart.Points.Single(point => point.Year == 1900).RawDeathAge;
        var denseMidPoint = denseChart.Points.Single(point => point.Year == 1900).RawDeathAge;
        var sparseLatePoint = sparseChart.Points.Single(point => point.Year == 1905).RawDeathAge;
        var denseLatePoint = denseChart.Points.Single(point => point.Year == 1905).RawDeathAge;
        var sparseEmptyWindow = sparseChart.Points.Single(point => point.Year == 1890).RawDeathAge;
        var denseEmptyWindow = denseChart.Points.Single(point => point.Year == 1890).RawDeathAge;

        Assert.IsTrue(sparseMidPoint > denseMidPoint);
        Assert.IsTrue(denseLatePoint > sparseLatePoint);
        Assert.IsTrue(sparseEmptyWindow > 0);
        Assert.AreEqual(0, denseEmptyWindow, 0.0001);
    }

    [TestMethod]
    public void Analyze_UsesSingleDeathObservationForUnknownDeathPaths()
    {
        var person = CreatePerson("M", CreateDate(new DateTime(1900, 1, 1)));

        var result = GenealogyStatisticsAnalyzer.Analyze(new[] { person }, 1900, 1990);

        Assert.AreEqual(1.0, result.Years.Sum(entry => entry.DeathAgeObservationCount), 0.0001);
        var observedYear = result.Years.Single(entry => entry.DeathAgeObservationCount > 0);
        Assert.AreEqual(1983, observedYear.Year);
        Assert.AreEqual(82.5, observedYear.AverageDeathAgeYears, 0.0001);
    }

    [TestMethod]
    public void Analyze_RejectsInvalidYearRanges()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => GenealogyStatisticsAnalyzer.Analyze(Array.Empty<IGenPerson>(), 1901, 1900));

        Assert.AreEqual("lastYear", exception.ParamName);
    }

    private static IGenPerson CreatePerson(
        string sex,
        IGenDate? birthDate,
        IGenDate? deathDate = null,
        IList<IGenFact?>? facts = null)
    {
        var person = Substitute.For<IGenPerson>();
        person.Sex.Returns(sex);
        person.BirthDate.Returns(birthDate);
        person.DeathDate.Returns(deathDate);
        person.Birth.Returns((IGenFact?)null);
        person.Death.Returns((IGenFact?)null);
        person.BaptDate.Returns((IGenDate?)null);
        person.Baptism.Returns((IGenFact?)null);
        person.BurialDate.Returns((IGenDate?)null);
        person.Burial.Returns((IGenFact?)null);
        person.Facts.Returns(facts ?? new List<IGenFact?>());
        var marriages = CreateIndexedList<IGenFamily>();
        person.Marriages.Returns(marriages);
        return person;
    }

    private static IGenFamily CreateFamily(IGenDate marriageDate)
    {
        var family = Substitute.For<IGenFamily>();
        family.UId.Returns(Guid.NewGuid());
        family.MarriageDate.Returns(marriageDate);
        family.Marriage.Returns((IGenFact?)null);
        family.Facts.Returns(new List<IGenFact?>());
        return family;
    }

    private static void SetMarriages(IGenPerson person, params IGenFamily[] families)
    {
        var marriages = CreateIndexedList(families);
        person.Marriages.Returns(marriages);
    }

    private static IIndexedList<T> CreateIndexedList<T>(params T[] values) where T : class
    {
        var indexedList = Substitute.For<IIndexedList<T>>();
        indexedList.GetEnumerator().Returns(_ => ((IEnumerable<T?>)values).GetEnumerator());
        return indexedList;
    }

    private static IGenDate CreateDate(
        int year,
        EDateType type,
        EDateModifier modifier = EDateModifier.None)
    {
        return CreateDate(new DateTime(year, 1, 1), type, modifier: modifier);
    }

    private static IGenDate CreateDate(
        DateTime firstDate,
        EDateType type = EDateType.Full,
        DateTime? secondDate = null,
        EDateType? secondType = null,
        EDateModifier modifier = EDateModifier.None)
    {
        var date = Substitute.For<IGenDate>();
        date.Date1.Returns(firstDate);
        date.eDateType1.Returns(type);
        date.Date2.Returns(secondDate);
        date.eDateType2.Returns(secondType ?? type);
        date.eDateModifier.Returns(modifier);
        return date;
    }

    private static IGenFact CreateFact(EFactType type, IGenDate? date = null, string? data = null)
    {
        var fact = Substitute.For<IGenFact>();
        fact.eFactType.Returns(type);
        if (date is not null)
        {
            fact.Date.Returns(date);
        }
        if (data is not null)
        {
            fact.Data.Returns(data);
        }
        return fact;
    }
}
