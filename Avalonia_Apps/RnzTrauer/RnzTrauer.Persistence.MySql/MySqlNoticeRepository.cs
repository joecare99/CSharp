using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Db.Core.Abstractions.Sql.Interfaaces;
using RnzTrauer.Core.Domain;
using RnzTrauer.Core.Services;

namespace RnzTrauer.Persistence.MySql;

/// <summary>
/// MySQL repository using the supplied provider-neutral <see cref="IDbConnectionFactory"/>.
/// Unlike the Pascal data module, all values are parameterized and each operation owns its connection.
/// </summary>
public sealed class MySqlNoticeRepository : INoticeRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IDBSettings _settings;

    public MySqlNoticeRepository(IDbConnectionFactory connectionFactory, IDBSettings settings)
    {
        _connectionFactory = connectionFactory;
        _settings = settings;
    }

    public async Task<IReadOnlyList<DeathNotice>> FindAsync(NoticeFilter filter, CancellationToken cancellationToken = default)
    {
        var where = new List<string>();
        var values = new Dictionary<string, object?>();
        if (!string.IsNullOrWhiteSpace(filter.OrderNumberPrefix)) { where.Add("`Auftrag` LIKE @order"); values["@order"] = filter.OrderNumberPrefix + "%"; }
        if (!string.IsNullOrWhiteSpace(filter.KeywordContains)) { where.Add("`Stichwort` LIKE @keyword"); values["@keyword"] = "%" + filter.KeywordContains + "%"; }
        AddReviewWhere(filter, where, values);
        var sql = "SELECT `idAnzeige`,`Auftrag`,`Stichwort`,`Nachname`,`Vorname`,`Geburtsname`,`Titel`,`Geschlecht`,`Erscheinungsdatum`,`Geb`,`GebModif`,`Gest`,`GestModif`,`Begr`,`Ort`,`Rubrik`,`Text`,`Pfad`,`PDF`,`PNG`,`LinkID`,`ProfileImg`,`ProfImgCount`,`TimeStamp` FROM `Anzeigen`" + (where.Count == 0 ? string.Empty : " WHERE " + string.Join(" AND ", where)) + " ORDER BY `Auftrag` DESC LIMIT 2000";
        return await QueryAsync(sql, values, cancellationToken).ConfigureAwait(false);
    }

    public async Task SaveAsync(DeathNotice notice, CancellationToken cancellationToken = default)
    {
        const string sql = "UPDATE `Anzeigen` SET `Nachname`=@family,`Vorname`=@given,`Geburtsname`=@maiden,`Titel`=@title,`Geschlecht`=@sex,`Geb`=@birth,`GebModif`=@birthModif,`Gest`=@death,`GestModif`=@deathModif,`Begr`=@burial,`Ort`=@place,`Rubrik`=@category,`Text`=@text,`LinkID`=@link,`ProfileImg`=@profile WHERE `idAnzeige`=@id";
        await ExecuteAsync(sql, ToParameters(notice, includeId: true), cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> UpsertImportedAsync(DeathNotice notice, CancellationToken cancellationToken = default)
    {
        const string sql = "INSERT INTO `Anzeigen` (`Auftrag`,`Pfad`,`Stichwort`,`Nachname`,`Vorname`,`Geschlecht`,`Hoehe`,`Breite`,`Erscheinungsdatum`,`Rubrik`,`PDF`,`PNG`) VALUES (@order,@path,@keyword,@family,@given,@sex,0,0,@published,@category,@pdf,@png) ON DUPLICATE KEY UPDATE `Pfad`=VALUES(`Pfad`),`PDF`=VALUES(`PDF`),`PNG`=VALUES(`PNG`)";
        var parameters = ToParameters(notice, false);
        parameters["@order"] = notice.OrderNumber;
        await ExecuteAsync(sql, parameters, cancellationToken).ConfigureAwait(false);
        return true;
    }

    public async Task<IReadOnlyList<string>> GetPlaceNamesAsync(CancellationToken cancellationToken = default)
    {
        var result = new List<string>();
        using var connection = _connectionFactory.CreateConnection(_settings);
        await OpenAsync(connection, cancellationToken).ConfigureAwait(false);
        using var command = connection.CreateCommand(); command.CommandText = "SELECT `Ortname` FROM `Orte` ORDER BY LENGTH(`Ortname`) DESC";
        using var reader = await AsDbCommand(command).ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false)) result.Add(reader.GetString(0));
        return result;
    }

    public Task<IReadOnlyList<DeathNotice>> GetLinkCandidatesAsync(long noticeId, CancellationToken cancellationToken = default) => QueryAsync("SELECT `idAnzeige`,`Auftrag`,`Stichwort`,`Nachname`,`Vorname`,`Geburtsname`,`Titel`,`Geschlecht`,`Erscheinungsdatum`,`Geb`,`GebModif`,`Gest`,`GestModif`,`Begr`,`Ort`,`Rubrik`,`Text`,`Pfad`,`PDF`,`PNG`,`LinkID`,`ProfileImg`,`ProfImgCount`,`TimeStamp` FROM `Anzeigen` WHERE `idAnzeige` IN (SELECT `LinkID` FROM `vPossibleLink1` WHERE `idAnzeige`=@id) LIMIT 20", new Dictionary<string, object?> { ["@id"] = noticeId }, cancellationToken);

    private async Task<IReadOnlyList<DeathNotice>> QueryAsync(string sql, IReadOnlyDictionary<string, object?> parameters, CancellationToken cancellationToken)
    {
        var result = new List<DeathNotice>();
        using var connection = _connectionFactory.CreateConnection(_settings); await OpenAsync(connection, cancellationToken).ConfigureAwait(false);
        using var command = connection.CreateCommand(); command.CommandText = sql; AddParameters(command, parameters);
        using var reader = await AsDbCommand(command).ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false)) result.Add(Read(reader));
        return result;
    }

    private async Task ExecuteAsync(string sql, IReadOnlyDictionary<string, object?> parameters, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection(_settings); await OpenAsync(connection, cancellationToken).ConfigureAwait(false);
        using var command = connection.CreateCommand(); command.CommandText = sql; AddParameters(command, parameters); await AsDbCommand(command).ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    private static DbCommand AsDbCommand(IDbCommand command) => command as DbCommand ?? throw new NotSupportedException("The configured provider must expose DbCommand async operations.");
    private static Task OpenAsync(IDbConnection connection, CancellationToken cancellationToken) => connection is DbConnection db ? db.OpenAsync(cancellationToken) : Task.Run(connection.Open, cancellationToken);
    private static void AddParameters(IDbCommand command, IReadOnlyDictionary<string, object?> values) { foreach (var pair in values) { var parameter = command.CreateParameter(); parameter.ParameterName = pair.Key; parameter.Value = pair.Value ?? DBNull.Value; command.Parameters.Add(parameter); } }
    private static Dictionary<string, object?> ToParameters(DeathNotice n, bool includeId) { var p = new Dictionary<string, object?> { ["@path"] = n.Path, ["@keyword"] = n.Keyword, ["@family"] = n.FamilyName, ["@given"] = n.GivenName, ["@maiden"] = n.MaidenName, ["@title"] = n.Title, ["@sex"] = n.Sex, ["@published"] = n.PublishedOn, ["@birth"] = n.BirthDate, ["@birthModif"] = Qualification(n.BirthQualification), ["@death"] = n.DeathDate, ["@deathModif"] = Qualification(n.DeathQualification), ["@burial"] = n.BurialDate, ["@place"] = n.Place, ["@category"] = (int)n.Category, ["@text"] = n.Text, ["@pdf"] = n.PdfFile, ["@png"] = n.PngFile, ["@link"] = n.LinkedNoticeId, ["@profile"] = n.ProfileImage }; if (includeId) p["@id"] = n.Id; return p; }
    private static string Qualification(DateQualification qualification) => qualification switch { DateQualification.Before => "bef.", DateQualification.After => "aft.", DateQualification.Calculated => "cal.", DateQualification.Estimated => "est.", _ => string.Empty };
    private static object? Value(IDataRecord r, string name) { var ordinal = r.GetOrdinal(name); return r.IsDBNull(ordinal) ? null : r.GetValue(ordinal); }
    private static string? String(IDataRecord r, string name) => Value(r, name)?.ToString();
    private static DateTime? Date(IDataRecord r, string name) => Value(r, name) is DateTime value ? value : null;
    private static long? Long(IDataRecord r, string name) => Value(r, name) is null ? null : Convert.ToInt64(Value(r, name), CultureInfo.InvariantCulture);
    private static DeathNotice Read(IDataRecord r) => new() { Id = Long(r,"idAnzeige") ?? 0, OrderNumber = String(r,"Auftrag") ?? string.Empty, Keyword = String(r,"Stichwort"), FamilyName = String(r,"Nachname"), GivenName = String(r,"Vorname"), MaidenName = String(r,"Geburtsname"), Title = String(r,"Titel"), Sex = String(r,"Geschlecht"), PublishedOn = Date(r,"Erscheinungsdatum"), BirthDate = Date(r,"Geb"), BirthQualification = ParseQualification(String(r,"GebModif")), DeathDate = Date(r,"Gest"), DeathQualification = ParseQualification(String(r,"GestModif")), BurialDate = Date(r,"Begr"), Place = String(r,"Ort"), Category = (AdvertisementCategory)(Long(r,"Rubrik") ?? 8050), Text = String(r,"Text"), Path = String(r,"Pfad"), PdfFile = String(r,"PDF"), PngFile = String(r,"PNG"), LinkedNoticeId = Long(r,"LinkID"), ProfileImage = String(r,"ProfileImg"), ProfileImageCount = (int)(Long(r,"ProfImgCount") ?? 0), TimeStamp = Date(r,"TimeStamp") };
    private static DateQualification ParseQualification(string? value) => value switch { "bef." => DateQualification.Before, "aft." => DateQualification.After, "cal." => DateQualification.Calculated, "est." => DateQualification.Estimated, _ => DateQualification.Exact };
    private static void AddReviewWhere(NoticeFilter f, List<string> where, IDictionary<string, object?> values) { switch (f.Kind) { case NoticeFilterKind.MissingText: where.Add("`Text` IS NULL"); break; case NoticeFilterKind.DeathNoticeWithoutPlace: where.Add("`Rubrik`=8050 AND `Ort` IS NULL"); break; case NoticeFilterKind.MissingSex: where.Add("(`Geschlecht` IS NULL OR `Geschlecht` NOT IN ('M','F')) AND `Vorname` <> ''"); break; case NoticeFilterKind.MissingLink: where.Add("`LinkID` IS NULL AND `Rubrik` IN (8055,8060,8070,8080)"); break; case NoticeFilterKind.MaleWithMaidenName: where.Add("`Geschlecht`='M' AND `Geburtsname` IS NOT NULL AND `Geburtsname`<>''"); break; case NoticeFilterKind.RecentMissingProfileImage: where.Add("`TimeStamp` > NOW() - INTERVAL 14 DAY AND `ProfileImg` IS NULL AND `ProfImgCount` > 0"); break; case NoticeFilterKind.ImplausibleDates: where.Add("`idAnzeige` IN (SELECT `idAnzeige` FROM `vWrongDate`)"); break; } if (f.ChangedSince is not null) { where.Add("`TimeStamp` > @changedSince"); values["@changedSince"] = f.ChangedSince; } }
}
