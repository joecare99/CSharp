using System;
using System.Collections.Generic;
using RnzTrauer.Places;

namespace RnzTrauer.Persistence.MySql;

/// <summary>
/// Characterizes the optional coordinate columns used by the legacy Places UI.
/// The historical CREATE TABLE statement does not create these columns.
/// </summary>
public static class MySqlPlaceCoordinateSql
{
    public static SqlStatement BuildProbe() =>
        new(
            "SELECT `Latitude`,`Longitude` FROM `Orte` LIMIT 0",
            new Dictionary<string, object?>());

    public static SqlStatement BuildCoordinateMigration() =>
        new(
            "ALTER TABLE `Orte` ADD COLUMN `Latitude` DECIMAL(10,7) NULL, ADD COLUMN `Longitude` DECIMAL(10,7) NULL",
            new Dictionary<string, object?>());

    public static SqlStatement BuildGet(string place)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(place);
        return new SqlStatement(
            "SELECT `Ortname`,`Latitude`,`Longitude` FROM `Orte` WHERE `Ortname`=@place LIMIT 1",
            new Dictionary<string, object?> { ["@place"] = PlaceNormalizer.Normalize(place) });
    }

    public static SqlStatement BuildSave(PlaceCoordinate coordinate)
    {
        ArgumentNullException.ThrowIfNull(coordinate);
        return new SqlStatement(
            "UPDATE `Orte` SET `Latitude`=@latitude,`Longitude`=@longitude WHERE `Ortname`=@place",
            new Dictionary<string, object?>
            {
                ["@place"] = PlaceNormalizer.Normalize(coordinate.Place),
                ["@latitude"] = coordinate.Latitude,
                ["@longitude"] = coordinate.Longitude,
            });
    }
}
