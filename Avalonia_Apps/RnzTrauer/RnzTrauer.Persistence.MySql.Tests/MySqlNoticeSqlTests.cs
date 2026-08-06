using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RnzTrauer.Core.Domain;

namespace RnzTrauer.Persistence.MySql.Tests;

[TestClass]
public sealed class MySqlNoticeSqlTests
{
    [TestMethod]
    [DataRow(NoticeFilterKind.MissingText, "`Text` IS NULL")]
    [DataRow(NoticeFilterKind.DeathNoticeWithoutPlace, "`Rubrik`=8050 AND `Ort` IS NULL")]
    [DataRow(NoticeFilterKind.MissingSex, "`Geschlecht` IS NULL")]
    [DataRow(NoticeFilterKind.MissingLink, "`LinkID` IS NULL")]
    [DataRow(NoticeFilterKind.DuplicateCandidates, "vNonSingletonName")]
    [DataRow(NoticeFilterKind.MaleWithMaidenName, "`Geschlecht`='M'")]
    [DataRow(NoticeFilterKind.RecentMissingProfileImage, "INTERVAL 14 DAY")]
    [DataRow(NoticeFilterKind.ImplausibleDates, "vWrongDate")]
    public void BuildFind_ContainsExpectedReviewQueue(
        NoticeFilterKind kind,
        string expectedFragment)
    {
        var statement = MySqlNoticeSql.BuildFind(new NoticeFilter(Kind: kind));

        StringAssert.Contains(statement.CommandText, expectedFragment);
    }

    [TestMethod]
    public void BuildFind_ParameterizesUserFiltersAndChangedSince()
    {
        var changedSince = new DateTime(2026, 8, 1);
        var statement = MySqlNoticeSql.BuildFind(new NoticeFilter(
            OrderNumberPrefix: "A-",
            KeywordContains: "Müller",
            ChangedSince: changedSince));

        StringAssert.Contains(statement.CommandText, "`Auftrag` LIKE @order");
        StringAssert.Contains(statement.CommandText, "`Stichwort` LIKE @keyword");
        Assert.AreEqual("A-%", statement.Parameters["@order"]);
        Assert.AreEqual("%Müller%", statement.Parameters["@keyword"]);
        Assert.AreEqual(changedSince, statement.Parameters["@changedSince"]);
    }

    [TestMethod]
    public void BuildLinkCandidates_UsesParameterizedNoticeId()
    {
        var statement = MySqlNoticeSql.BuildLinkCandidates(42);

        StringAssert.Contains(statement.CommandText, "vPossibleLink1");
        Assert.AreEqual(42L, statement.Parameters["@id"]);
    }
}
