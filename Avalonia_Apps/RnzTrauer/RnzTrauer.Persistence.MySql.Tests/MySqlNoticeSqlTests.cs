using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RnzTrauer.Core.Domain;
using RnzTrauer.Places;

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
    [DataRow(NoticeFilterKind.MissingText, "LIMIT 300")]
    [DataRow(NoticeFilterKind.DeathNoticeWithoutPlace, "ORDER BY `Auftrag` DESC LIMIT 100")]
    [DataRow(NoticeFilterKind.MissingSex, "LIMIT 100")]
    [DataRow(NoticeFilterKind.MissingLink, "LIMIT 300")]
    [DataRow(NoticeFilterKind.DuplicateCandidates, "ORDER BY `Auftrag` DESC LIMIT 300")]
    [DataRow(NoticeFilterKind.MaleWithMaidenName, "ORDER BY `Auftrag` DESC LIMIT 300")]
    [DataRow(NoticeFilterKind.RecentMissingProfileImage, "ORDER BY `Auftrag` DESC LIMIT 2000")]
    [DataRow(NoticeFilterKind.ImplausibleDates, "ORDER BY `Auftrag` DESC LIMIT 300")]
    public void BuildFind_PreservesPascalQueueLimitAndOrdering(
        NoticeFilterKind kind,
        string expectedFragment)
    {
        var statement = MySqlNoticeSql.BuildFind(new NoticeFilter(Kind: kind));

        StringAssert.Contains(statement.CommandText, expectedFragment);
    }

    [TestMethod]
    public void BuildFind_UsesPascalDefaultQueuePredicates()
    {
        var missingSex = MySqlNoticeSql.BuildFind(
            new NoticeFilter(Kind: NoticeFilterKind.MissingSex));
        var recentProfile = MySqlNoticeSql.BuildFind(
            new NoticeFilter(Kind: NoticeFilterKind.RecentMissingProfileImage));
        var duplicates = MySqlNoticeSql.BuildFind(
            new NoticeFilter(Kind: NoticeFilterKind.DuplicateCandidates));

        StringAssert.Contains(missingSex.CommandText, "Rubrik` IN (8050,8051,8052,8055,8060,8070,8080)");
        StringAssert.Contains(recentProfile.CommandText, "Rubrik` IN (8050,8051,8052,8056,8061,8071,8081)");
        StringAssert.Contains(duplicates.CommandText, "idAnzeige_min` FROM `vNonSingletonName` UNION SELECT `idAnzeige_max`");
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

    [TestMethod]
    public void BuildPlaceCoordinateGet_UsesNormalizedPlaceAndLegacyCoordinateColumns()
    {
        var statement = MySqlPlaceCoordinateSql.BuildGet("  Heidelberg ");

        StringAssert.Contains(statement.CommandText, "`Ortname`,`Latitude`,`Longitude`");
        StringAssert.Contains(statement.CommandText, "WHERE `Ortname`=@place");
        Assert.AreEqual("Heidelberg", statement.Parameters["@place"]);
    }

    [TestMethod]
    public void BuildPlaceCoordinateSave_BindsCoordinatesAndPlace()
    {
        var statement = MySqlPlaceCoordinateSql.BuildSave(
            new PlaceCoordinate(" Heidelberg ", 49.3988, 8.6724, "fixture", false));

        StringAssert.Contains(statement.CommandText, "UPDATE `Orte`");
        Assert.AreEqual("Heidelberg", statement.Parameters["@place"]);
        Assert.AreEqual(49.3988, statement.Parameters["@latitude"]);
        Assert.AreEqual(8.6724, statement.Parameters["@longitude"]);
    }

    [TestMethod]
    public void BuildPlaceCoordinateProbe_OnlyChecksOptionalCoordinateColumns()
    {
        var statement = MySqlPlaceCoordinateSql.BuildProbe();

        StringAssert.Contains(
            statement.CommandText,
            "SELECT `Latitude`,`Longitude` FROM `Orte` LIMIT 0");
        Assert.AreEqual(0, statement.Parameters.Count);
    }

    [TestMethod]
    public void BuildCoordinateMigration_AddsOptionalCoordinateColumnsWithoutValues()
    {
        var statement = MySqlPlaceCoordinateSql.BuildCoordinateMigration();

        StringAssert.Contains(statement.CommandText, "ALTER TABLE `Orte`");
        StringAssert.Contains(statement.CommandText, "DECIMAL(10,7) NULL");
        Assert.AreEqual(0, statement.Parameters.Count);
    }

    [TestMethod]
    public void CoordinateSchemaStatus_MissingDisablesPersistence()
    {
        var report = new CoordinateSchemaReport(
            CoordinateSchemaStatus.Missing,
            ["Orte.Latitude", "Orte.Longitude"],
            "unknown column",
            "schema.missing_columns");

        Assert.IsFalse(report.CanPersist);
        Assert.AreEqual(CoordinateSchemaStatus.Missing, report.Status);
    }
}
