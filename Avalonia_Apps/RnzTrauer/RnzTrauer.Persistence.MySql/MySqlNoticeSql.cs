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
        var sql = "SELECT " + NoticeColumns + " FROM `Anzeigen`"
            + (where.Count == 0 ? string.Empty : " WHERE " + string.Join(" AND ", where))
            + " ORDER BY `Auftrag` DESC LIMIT 2000";
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
                where.Add("(`Geschlecht` IS NULL OR `Geschlecht` NOT IN ('M','F')) AND `Vorname` <> ''");
                break;
            case NoticeFilterKind.MissingLink:
                where.Add("`LinkID` IS NULL AND `Rubrik` IN (8055,8060,8070,8080)");
                break;
            case NoticeFilterKind.DuplicateCandidates:
                where.Add("`idAnzeige` IN (SELECT `idAnzeige` FROM `vNonSingletonName`)");
                break;
            case NoticeFilterKind.MaleWithMaidenName:
                where.Add("`Geschlecht`='M' AND `Geburtsname` IS NOT NULL AND `Geburtsname`<>''");
                break;
            case NoticeFilterKind.RecentMissingProfileImage:
                where.Add("`TimeStamp` > NOW() - INTERVAL 14 DAY AND `ProfileImg` IS NULL AND `ProfImgCount` > 0");
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
