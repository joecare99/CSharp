using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using Newtonsoft.Json;
using OFBCreator.Core.Models;

namespace OFBCreator.Console.Config;

/// <summary>
/// Loader for JSON-based OFB configuration files.
/// </summary>
/// <remarks>
/// Supports deserializing options to/from a JSON file for batch/CI-CD workflows.
/// Uses camelCase property naming convention matching typical CLI tool conventions.
/// </remarks>
public static class FileConfigLoader
{
    private const string InputFileKey = "input";
    private const string OutputFileKey = "output";
    private const string TitleKey = "title";
    private const string PlaceIdKey = "placeId";
    private const string IncludeDescendantsKey = "includeDescendants";
    private const string PrefaceKey = "preface";
    private const string LegendKey = "legend";
    private const string FormatKey = "format"; // "docx" or "odt"

    /// <summary>
    /// Loads OFB generation options from a JSON configuration file.
    /// </summary>
    /// <param name="configPath">Absolute or relative path to the JSON file.</param>
    /// <returns>The deserialized options, or null if loading fails.</returns>
    public static async Task<OFBGenerateOptions?> LoadAsync( string configPath )
    {
        var json = await File.ReadAllTextAsync( configPath );

        // Use case-insensitive property matching for flexible config files
        var settings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            Culture = System.Globalization.CultureInfo.InvariantCulture
        };

        try
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, object?>>( json, settings );
            if ( dict == null )
                return null;

            return new OFBGenerateOptions()
            {
                InputPath = GetString( dict, InputFileKey ),
                OutputPath = GetString( dict, OutputFileKey ),
                Title = GetString( dict, TitleKey ),
                PlaceId = TryGetValue( dict, PlaceIdKey )?.ToString(),
                IncludeDescendants = TryGetValueAsBool( dict, IncludeDescendantsKey, false ),
                Preface = TryGetValue( dict, PrefaceKey )?.ToString(),
                Legend = TryGetValue( dict, LegendKey )?.ToString(),
                UseDocxFormat = TryGetValue( dict, FormatKey )?.ToString()?.Equals( "odt", StringComparison.OrdinalIgnoreCase ) != true,
            };
        }
        catch ( Exception ex )
        {
            System.Console.Error.WriteLine( $"Error deserializing config '{configPath}': {ex.Message}" );
            return null;
        }
    }

    /// <summary>
    /// Saves current options to a JSON configuration file.
    /// </summary>
    public static async Task<bool> SaveAsync( string configPath, OFBGenerateOptions options )
    {
        try
        {
            var dict = new Dictionary<string, object?>
            {
                [InputFileKey] = options.InputPath,
                [OutputFileKey] = options.OutputPath,
                [TitleKey] = options.Title,
                [PlaceIdKey] = string.IsNullOrEmpty( options.PlaceId ) ? null : options.PlaceId,
                [IncludeDescendantsKey] = options.IncludeDescendants,
                [PrefaceKey] = string.IsNullOrEmpty( options.Preface ) ? null : options.Preface,
                [LegendKey] = string.IsNullOrEmpty( options.Legend ) ? null : options.Legend,
                [FormatKey] = options.UseDocxFormat ? "docx" : "odt"
            };

            var json = JsonConvert.SerializeObject( dict, Formatting.Indented );
            await File.WriteAllTextAsync( configPath, json );
            return true;
        }
        catch ( Exception ex )
        {
            System.Console.Error.WriteLine( $"Error saving config to '{configPath}': {ex.Message}" );
            return false;
        }
    }

    private static string? GetString( Dictionary<string, object?> dict, string key )
    {
        var val = TryGetValue( dict, key );
        return val?.ToString();
    }

    private static object? TryGetValue( Dictionary<string, object?> dict, string key )
    {
        dict.TryGetValue( key, out var value );
        return value;
    }

    private static bool TryGetValueAsBool( Dictionary<string, object?> dict, string key, bool defaultValue )
    {
        var val = TryGetValue( dict, key );
        if ( val == null ) return defaultValue;

        if ( bool.TryParse( val.ToString(), out var result ) )
            return result;

        return defaultValue;
    }
}
