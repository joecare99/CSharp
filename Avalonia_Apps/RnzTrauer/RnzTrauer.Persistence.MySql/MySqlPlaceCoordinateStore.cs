using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Db.Core.Abstractions.Sql.Interfaaces;
using RnzTrauer.Places;

namespace RnzTrauer.Persistence.MySql;

/// <summary>
/// Persists place coordinates in the optional legacy <c>Orte</c> columns.
/// Provider errors, including missing columns, are deliberately propagated.
/// </summary>
public sealed class MySqlPlaceCoordinateStore : IPlaceCoordinateStore, ICoordinateSchemaProbe
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IDBSettings _settings;

    public MySqlPlaceCoordinateStore(
        IDbConnectionFactory connectionFactory,
        IDBSettings settings)
    {
        _connectionFactory = connectionFactory;
        _settings = settings;
    }

    public async Task<PlaceCoordinate?> GetAsync(
        string place,
        CancellationToken cancellationToken = default)
    {
        var statement = MySqlPlaceCoordinateSql.BuildGet(place);
        using var connection = _connectionFactory.CreateConnection(_settings);
        await OpenAsync(connection, cancellationToken).ConfigureAwait(false);
        using var command = connection.CreateCommand();
        command.CommandText = statement.CommandText;
        AddParameters(command, statement.Parameters);
        using var reader = await AsDbCommand(command)
            .ExecuteReaderAsync(cancellationToken)
            .ConfigureAwait(false);

        if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            return null;

        var latitude = Decimal(reader, "Latitude");
        var longitude = Decimal(reader, "Longitude");
        if (latitude is null || longitude is null)
            return null;

        return new PlaceCoordinate(
            reader["Ortname"]?.ToString() ?? place,
            (double)latitude.Value,
            (double)longitude.Value,
            "mysql",
            false);
    }

    public async Task SaveAsync(
        PlaceCoordinate coordinate,
        CancellationToken cancellationToken = default)
    {
        var statement = MySqlPlaceCoordinateSql.BuildSave(coordinate);
        using var connection = _connectionFactory.CreateConnection(_settings);
        await OpenAsync(connection, cancellationToken).ConfigureAwait(false);
        using var command = connection.CreateCommand();
        command.CommandText = statement.CommandText;
        AddParameters(command, statement.Parameters);
        await AsDbCommand(command).ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<CoordinateSchemaReport> ProbeAsync(
        CancellationToken cancellationToken = default)
    {
        var statement = MySqlPlaceCoordinateSql.BuildProbe();
        using var connection = _connectionFactory.CreateConnection(_settings);
        try
        {
            await OpenAsync(connection, cancellationToken).ConfigureAwait(false);
            using var command = connection.CreateCommand();
            command.CommandText = statement.CommandText;
            AddParameters(command, statement.Parameters);
            using var reader = await AsDbCommand(command)
                .ExecuteReaderAsync(cancellationToken)
                .ConfigureAwait(false);
            return CoordinateSchemaReport.Create(CoordinateSchemaStatus.Available);
        }
        catch (DbException exception)
        {
            if (exception.ErrorCode == 1054)
            {
                return new CoordinateSchemaReport(
                    CoordinateSchemaStatus.Missing,
                    ["Orte.Latitude", "Orte.Longitude"],
                    "MySQL reports an unknown coordinate column; persistence is disabled.",
                    "schema.missing_columns");
            }

            return CoordinateSchemaReport.CreateUnverified(
                $"Schema probe failed with {exception.GetType().Name}; persistence remains unverified.");
        }
    }

    private static DbCommand AsDbCommand(IDbCommand command) =>
        command as DbCommand
        ?? throw new NotSupportedException("The configured provider must expose DbCommand async operations.");

    private static Task OpenAsync(
        IDbConnection connection,
        CancellationToken cancellationToken) =>
        connection is DbConnection db
            ? db.OpenAsync(cancellationToken)
            : Task.Run(connection.Open, cancellationToken);

    private static void AddParameters(
        IDbCommand command,
        System.Collections.Generic.IReadOnlyDictionary<string, object?> values)
    {
        foreach (var pair in values)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = pair.Key;
            parameter.Value = pair.Value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }
    }

    private static decimal? Decimal(IDataRecord record, string name)
    {
        var ordinal = record.GetOrdinal(name);
        if (record.IsDBNull(ordinal))
            return null;
        return Convert.ToDecimal(record.GetValue(ordinal), CultureInfo.InvariantCulture);
    }
}
