using System;
using System.Globalization;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using OFBCreator.Core.Services;

namespace OFBCreator.Core.Tests.Services;

[TestClass]
public sealed class GedcomDateFormatterTests
{
    [TestMethod]
    [DataRow("de-DE", "15.01.1890")]
    [DataRow("en-US", "1/15/1890")]
    public void Format_UsesCultureForFullDate(string cultureName, string expected)
    {
        var date = CreateDate("15 JAN 1890", EDateModifier.None, EDateType.Full, new DateTime(1890, 1, 15));

        var result = GedcomDateFormatter.Format(date, CultureInfo.GetCultureInfo(cultureName));

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [DataRow("de-DE", "Januar 1890")]
    [DataRow("en-US", "January 1890")]
    public void Format_PreservesMonthYearPrecision(string cultureName, string expected)
    {
        var date = CreateDate("JAN 1890", EDateModifier.None, EDateType.MonthYear, new DateTime(1890, 1, 1));

        var result = GedcomDateFormatter.Format(date, CultureInfo.GetCultureInfo(cultureName));

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [DataRow("ABT 1890", EDateModifier.About, "um 1890", "about 1890")]
    [DataRow("CAL 1890", EDateModifier.Calculated, "berechnet 1890", "calculated 1890")]
    [DataRow("EST 1890", EDateModifier.Estimated, "geschätzt 1890", "estimated 1890")]
    [DataRow("BEF 1890", EDateModifier.Before, "vor 1890", "before 1890")]
    [DataRow("AFT 1890", EDateModifier.After, "nach 1890", "after 1890")]
    public void Format_LocalizesSimpleModifiers(
        string rawText,
        EDateModifier modifier,
        string german,
        string english)
    {
        var date = CreateDate(rawText, modifier, EDateType.Year, default);

        Assert.AreEqual(german, GedcomDateFormatter.Format(date, CultureInfo.GetCultureInfo("de-DE")));
        Assert.AreEqual(english, GedcomDateFormatter.Format(date, CultureInfo.GetCultureInfo("en-US")));
    }

    [TestMethod]
    [DataRow("BET 1 JAN 1880 AND 31 DEC 1890", EDateModifier.Between,
        "zwischen 01.01.1880 und 31.12.1890", "between 1/1/1880 and 12/31/1890")]
    [DataRow("FROM 1 JAN 1880 TO 31 DEC 1890", EDateModifier.FromTo,
        "von 01.01.1880 bis 31.12.1890", "from 1/1/1880 to 12/31/1890")]
    public void Format_LocalizesDateRanges(
        string rawText,
        EDateModifier modifier,
        string german,
        string english)
    {
        var date = CreateDate(rawText, modifier, EDateType.Full, new DateTime(1880, 1, 1));
        date.Date2.Returns(new DateTime(1890, 12, 31));

        Assert.AreEqual(german, GedcomDateFormatter.Format(date, CultureInfo.GetCultureInfo("de-DE")));
        Assert.AreEqual(english, GedcomDateFormatter.Format(date, CultureInfo.GetCultureInfo("en-US")));
    }

    [TestMethod]
    public void Format_LeavesUnrecognizedFreeTextUnchanged()
    {
        var date = CreateDate("sometime in spring", EDateModifier.Text, EDateType.Full, default);

        Assert.AreEqual("sometime in spring",
            GedcomDateFormatter.Format(date, CultureInfo.GetCultureInfo("en-US")));
    }

    private static IGenDate CreateDate(
        string rawText,
        EDateModifier modifier,
        EDateType precision,
        DateTime value)
    {
        var date = Substitute.For<IGenDate>();
        date.DateText.Returns(rawText);
        date.eDateModifier.Returns(modifier);
        date.eDateType1.Returns(precision);
        date.Date1.Returns(value);
        date.Date2.Returns((DateTime?)null);
        date.eDateType2.Returns((EDateType?)null);
        return date;
    }
}
