/// <summary>
/// OFB Creator - Ortsfamilienbuch Generator (Console Application).
/// </summary>
/// <remarks>
/// Headless CLI for batch generation of local family books from GEDCOM data.
/// Output is generated via IUserDocument abstraction for maximum format flexibility.
/// Supported indices: Person, Occupation, Property, Place Hierarchy, Place Alphabetical.
/// </remarks>
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using GenInterfaces.Interfaces.Genealogic.Drivers;
using IGenImportDriver = GenInterfaces.Interfaces.Genealogic.Drivers.IGenImportDriver<GenInterfaces.Interfaces.Genealogic.IGenealogy>;
using Document.Base.Factories;
using Document.Base.Models.Interfaces;
using OFBCreator.Core.Models;
using OFBCreator.Core.Services;

namespace OFBCreator.Console;


public static class Program
{
    /// <summary>
    /// Entry point for the OFB Creator console application.
    /// </summary>
    private static async Task<int> Main( string[] args )
    {
        var services = new ServiceCollection();

        // Register GEDCOM import driver (user will configure via DI)
        // For now, register a placeholder — user can override with their own setup
        services.AddSingleton<IGenImportDriver>( _ => null! ); // Replace with actual IGenImportDriver implementation
        services.AddSingleton<IUserDocumentFactory, UserDocumentFactoryImpl>();

        var serviceProvider = services.BuildServiceProvider();

        var rootCommand = new RootCommand( "Ortsfamilienbuch Generator" )
        {
            GenerateCommand( serviceProvider ),
            ExportCommand( serviceProvider )
        };

        return await rootCommand.InvokeAsync( args );
    }

    /// <summary>
    /// Creates the 'generate' command with all GEDCOM-to-OFB options.
    /// </summary>
    private static Command GenerateCommand( IServiceProvider serviceProvider )
    {
        var inputOption = new Option<string>(
            name: "--input",
            description: "Path to the source GEDCOM file" )
        { IsRequired = true };

        var outputOption = new Option<string>(
            name: "--output",
            description: "Path to the output OFB document (.docx/.odt)" )
        { IsRequired = true };

        var titleOption = new Option<string>(
            name: "--title",
            description: "Title of the Ortsfamilienbuch" )
        { IsRequired = true };

        var placeIdOption = new Option<string?>(
            name: "--place-id",
            description: "GedCom reference ID for the primary place filter (optional)" );

        var includeDescendants = new Option<bool>(
            name: "--include-descendants",
            description: "Include descendants of families in this place" );

        var prefaceOption = new Option<string?>(
            name: "--preface",
            description: "Preface/introduction text for the OFB" );

        var legendOption = new Option<string?>(
            name: "--legend",
            description: "Character explanation/legend text" );

        var docxOption = new Option<bool>(
            name: "--docx",
            description: "Use DOCX format (default)" );
        docxOption.AddAlias( "-d" );

        var odtOption = new Option<bool>(
            name: "--odt",
            description: "Use ODF (ODT) format instead of DOCX" );
        odtOption.AddAlias( "-o" );

        var command = new Command( "generate", "Generate OFB from GEDCOM data" )
        {
            inputOption, outputOption, titleOption, placeIdOption,
            includeDescendants, prefaceOption, legendOption, docxOption, odtOption
        };

        command.SetHandler( async context =>
        {
            var options = new OFBGenerateOptions()
            {
                InputPath = context.ParseResult.GetValueForOption( inputOption ),
                OutputPath = context.ParseResult.GetValueForOption( outputOption ),
                Title = context.ParseResult.GetValueForOption( titleOption ),
                PlaceId = context.ParseResult.GetValueForOption( placeIdOption ),
                IncludeDescendants = context.ParseResult.GetValueForOption( includeDescendants ),
                Preface = context.ParseResult.GetValueForOption( prefaceOption ),
                Legend = context.ParseResult.GetValueForOption( legendOption ),
                UseDocxFormat = context.ParseResult.GetValueForOption( docxOption ) ||
                                !context.ParseResult.GetValueForOption( odtOption ),
            };

            await RunExportAsync( options, serviceProvider );
        } );

        return command;
    }

    /// <summary>
    /// Creates the 'export' command for loading from a JSON config file.
    /// </summary>
    private static Command ExportCommand( IServiceProvider serviceProvider )
    {
        var configOption = new Option<string>(
            name: "--config",
            description: "Path to OFB configuration JSON file" )
        { IsRequired = true };

        var command = new Command( "export", "Export OFB using JSON configuration" )
        {
            configOption
        };

        command.SetHandler( async context =>
        {
            var configPath = context.ParseResult.GetValueForOption( configOption );
            var options = await LoadConfigAsync( configPath );
            if ( options is null )
            {
                System.Console.Error.WriteLine( $"Error: Failed to load config from '{configPath}'" );
                Environment.Exit( 1 );
                return;
            }

            await RunExportAsync( options, serviceProvider );
        } );

        return command;
    }

    /// <summary>
    /// Orchestrates the full OFB generation pipeline.
    /// </summary>
    private static async Task RunExportAsync( OFBGenerateOptions options, IServiceProvider serviceProvider )
    {
        try
        {
            System.Console.WriteLine( "Starting OFB Creator..." );
            System.Console.WriteLine( $"  Input:    {options.InputPath}" );
            System.Console.WriteLine( $"  Output:   {options.OutputPath}" );
            System.Console.WriteLine( $"  Title:    {options.Title}" );

            var exportService = new ConsoleExportService(
                serviceProvider.GetRequiredService<IGenImportDriver>(),
                serviceProvider.GetRequiredService<IUserDocumentFactory>() );
            await exportService.ExportAsync( options );

            System.Console.WriteLine( "OFB generation completed successfully." );
        }
        catch ( Exception ex )
        {
            System.Console.Error.WriteLine( $"\nError during OFB generation:" );
            System.Console.Error.WriteLine( ex.Message );

            if ( Environment.GetEnvironmentVariable( "OFBCREATOR_DEBUG" ) == "1" )
            {
                System.Console.Error.WriteLine( $"\nStack trace:\n{ex.StackTrace}" );
            }

            Environment.Exit( 1 );
        }
    }

    /// <summary>
    /// Loads OFB generation options from a JSON configuration file.
    /// </summary>
    private static async Task<OFBGenerateOptions?> LoadConfigAsync( string configPath )
    {
        if ( !File.Exists( configPath ) )
            return null;

        var json = await File.ReadAllTextAsync( configPath );
        return System.Text.Json.JsonSerializer.Deserialize<OFBGenerateOptions>( json );
    }
}
