using System;
using System.Collections.Generic;
using RnzTrauer.Core.Domain;

namespace RnzTrauer.Persistence.MySql;

/// <summary>
/// Builds the adapter's parameterized SQL independently of any database
/// connection, enabling queue and statement characterization tests.
/// </summary>
public static class MySqlNoticeSql
{
    private const string NoticeColumns =
        "`idAnzeige`,`Auftrag`,`Stichwort`,`Nachname`,`Vorname`,`Geburtsname`,`Titel`,`Geschlecht`,`Erscheinungsdatum`,`Geb`,`GebModif`,`Gest`,`GestModif`,`Begr`,`Ort`,`Rubrik`,`Text`,`Pfad`,`PDF`,`PNG`,`LinkID`,`ProfileImg`,`ProfImgCount`,`TimeStamp`";

    /// <summary>Builds the principal review/list query.</summary>
    public static SqlStatement BuildFind(NoticeFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        var where = new List<string>();
        var values = new Dictionary<string, object?>();

        if (!string.IsNullOrWhiteSpace(filter.OrderNumberPrefix))
        {
            where.Add("`Auftrag` LIKE @order");
            values["@order"] = filter.OrderNumberPrefix + "%";
        }
        if (!string.IsNullOrWhiteSpace(filter.KeywordContains))
        {
            where.Add("`Stichwort` LIKE @keyword");
            values["@keyword"] = "%" + filter.KeywordContains + "%";
        }

        AddReviewWhere(filter, where, values);
        var limit = filter.Kind switch
        {
            NoticeFilterKind.MissingText => 300,
            NoticeFilterKind.DeathNoticeWithoutPlace => 100,
            NoticeFilterKind.MissingSex => 100,
            NoticeFilterKind.MissingLink => 300,
            NoticeFilterKind.DuplicateCandidates => 300,
            NoticeFilterKind.MaleWithMaidenName => 300,
            NoticeFilterKind.RecentMissingProfileImage => 2000,
            NoticeFilterKind.ImplausibleDates => 300,
            _ => 300,
        };
        var orderBy = filter.Kind switch
        {
            NoticeFilterKind.DeathNoticeWithoutPlace
                or NoticeFilterKind.DuplicateCandidates
                or NoticeFilterKind.MaleWithMaidenName
                or NoticeFilterKind.RecentMissingProfileImage
                or NoticeFilterKind.ImplausibleDates => " ORDER BY `Auftrag` DESC",
            _ => string.Empty,
        };
        var sql = "SELECT " + NoticeColumns + " FROM `Anzeigen`"
            + (where.Count == 0 ? string.Empty : " WHERE " + string.Join(" AND ", where))
            + orderBy
            + " LIMIT " + limit;
        return new SqlStatement(sql, values);
    }

    /// <summary>Builds the place-name query used by parser normalization.</summary>
    public static SqlStatement BuildPlaceNames()
    {
        return new SqlStatement(
            "SELECT `Ortname` FROM `Orte` ORDER BY LENGTH(`Ortname`) DESC",
            new Dictionary<string, object?>());
    }

    /// <summary>Builds the link-candidate query for one notice.</summary>
    public static SqlStatement BuildLinkCandidates(long noticeId)
    {
        return new SqlStatement(
            "SELECT " + NoticeColumns
            + " FROM `Anzeigen` WHERE `idAnzeige` IN "
            + "(SELECT `LinkID` FROM `vPossibleLink1` WHERE `idAnzeige`=@id) LIMIT 20",
            new Dictionary<string, object?> { ["@id"] = noticeId });
    }

    private static void AddReviewWhere(
        NoticeFilter filter,
        ICollection<string> where,
        IDictionary<string, object?> values)
    {
        switch (filter.Kind)
        {
            case NoticeFilterKind.MissingText:
                where.Add("`Text` IS NULL");
                break;
            case NoticeFilterKind.DeathNoticeWithoutPlace:
                where.Add("`Rubrik`=8050 AND `Ort` IS NULL");
                break;
            case NoticeFilterKind.MissingSex:
                where.Add("(`Geschlecht` IS NULL OR `Geschlecht` NOT IN ('M','F')) AND `Vorname` IS NOT NULL AND `Vorname` <> '' AND `Rubrik` IN (8050,8051,8052,8055,8060,8070,8080)");
                break;
            case NoticeFilterKind.MissingLink:
                where.Add("`LinkID` IS NULL");
                break;
            case NoticeFilterKind.DuplicateCandidates:
                where.Add("`idAnzeige` IN (SELECT `idAnzeige_min` FROM `vNonSingletonName` UNION SELECT `idAnzeige_max` FROM `vNonSingletonName`)");
                break;
            case NoticeFilterKind.MaleWithMaidenName:
                where.Add("`Geschlecht`='M' AND `Geburtsname` IS NOT NULL AND `Geburtsname`<>''");
                break;
            case NoticeFilterKind.RecentMissingProfileImage:
                where.Add("`TimeStamp` > NOW() - INTERVAL 14 DAY AND `ProfileImg` IS NULL AND `ProfImgCount` > 0 AND `Rubrik` IN (8050,8051,8052,8056,8061,8071,8081)");
                break;
            case NoticeFilterKind.ImplausibleDates:
                where.Add("`idAnzeige` IN (SELECT `idAnzeige` FROM `vWrongDate`)");
                break;
        }

        if (filter.ChangedSince is not null)
        {
            where.Add("`TimeStamp` > @changedSince");
            values["@changedSince"] = filter.ChangedSince;
        }
    }
}
