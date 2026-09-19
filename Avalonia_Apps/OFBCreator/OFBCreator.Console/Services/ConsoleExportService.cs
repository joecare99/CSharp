using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BaseLib.Models.Interfaces;
using Document.Base.Models.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
using IGenImportDriver = GenInterfaces.Interfaces.Genealogic.Drivers.IGenImportDriver<GenInterfaces.Interfaces.Genealogic.IGenealogy>;
using OFBCreator.Abstractions.Models;
using OFBCreator.Core.Models;
using OFBCreator.Core.Services;

/// <summary>
/// Headless export service that orchestrates the full OFB (Ortsfamilienbuch) generation pipeline.
/// </summary>
/// <remarks>
/// Responsibilities:
/// 1. Load GEDCOM via IGenImportDriver dependency injection.
/// 2. Build name groups and sort families by FamilyName + FormationDate.
/// 3. Assign global family numbers (4-5 digit with leading zeros).
/// 4. Generate all indices (Person, Occupation, Property, PlaceHierarchy, PlaceAlphabetical).
/// 5. Compose the OFB document via IUserDocument abstraction (DOCX or ODT).
/// </remarks>
public sealed class ConsoleExportService( IGenImportDriver gedcomDriver, IUserDocumentFactory documentFactory )
{
    private readonly IGenImportDriver _gedcomDriver = gedcomDriver;
    private readonly IUserDocumentFactory _documentFactory = documentFactory;

    /// <summary>
    /// Executes the full OFB generation pipeline for the given options.
    /// </summary>
    public async Task ExportAsync( OFBGenerateOptions options )
    {
        if ( string.IsNullOrWhiteSpace( options.InputPath ) )
            throw new ArgumentException( "GEDCOM input path is required.", nameof( options ) );

        if ( string.IsNullOrWhiteSpace( options.OutputPath ) )
            throw new ArgumentException( "Output path is required.", nameof( options ) );

        if ( string.IsNullOrWhiteSpace( options.Title ) )
            throw new ArgumentException( "OFB title is required.", nameof( options ) );

        // Step 1: Load GEDCOM data
        System.Console.WriteLine( "Loading GEDCOM..." );
        using var fs = new FileStream( options.InputPath, FileMode.Open, FileAccess.Read, FileShare.Read );
        var result = await _gedcomDriver.ImportAsync( fs );

        if ( result == null || !result.Success || result.Payload?.Entitys == null )
            throw new InvalidOperationException( $"No families found in '{options.InputPath}'" );

        var genealogy = result.Payload;
        var allFamilies = genealogy.Entitys.OfType<IGenFamily>().ToList();

        if ( allFamilies.Count == 0 )
            throw new InvalidOperationException( $"No families found in '{options.InputPath}'" );

        // Step 2: Build name groups and filter by place
        System.Console.WriteLine( "Building family name groups..." );
        var nameGrouping = new FamilyNameGroupBuilder();
        nameGrouping.BuildGroups( allFamilies );

        // Step 3: Convert Abstraction models to Core models, sort, and assign global numbers
        System.Console.WriteLine( "Sorting and numbering families..." );
        var sorter = new OFBFamilySorter();
        var abFamilies = sorter.SortAndNumber( allFamilies, options.PlaceId, options.IncludeDescendants );

        // Convert Abstractions.OFBFamilyModel → Core.OFBFamily
        var sortedFamilies = ConvertToCoreFamilies( abFamilies );

        // Step 4: Generate indices
        System.Console.WriteLine( "Generating Person Index..." );
        var personIndex = await GeneratePersonIndexAsync( sortedFamilies );

        System.Console.WriteLine( "Generating Occupation Index..." );
        var occIndex = await GenerateOccupationIndexAsync( sortedFamilies );

        System.Console.WriteLine( "Generating Property Index..." );
        var propIndex = await GeneratePropertyIndexAsync( sortedFamilies );

        System.Console.WriteLine( "Generating Place Hierarchy Index..." );
        var placeHierarchyIndex = await GeneratePlaceHierarchyIndexAsync( sortedFamilies );

        System.Console.WriteLine( "Generating Place Alphabetical Index..." );
        var placeAlphaIndex = await GeneratePlaceAlphabeticalIndexAsync( sortedFamilies );

        // Step 5: Create document and compose content
        System.Console.WriteLine( "Composing document..." );
        var doc = _documentFactory.CreateDocument( options.UseDocxFormat ? OFBOutputFormat.Docx : OFBOutputFormat.Odt );

        await ComposeTitlePageAsync( doc, options );
        await ComposePrefaceAsync( doc, options );
        await ComposeIndicesAsync( doc, personIndex, occIndex, propIndex, placeHierarchyIndex, placeAlphaIndex );
        await ComposeFamiliesAsync( doc, sortedFamilies );

        // Step 6: Save document
        System.Console.WriteLine( "Saving output..." );
        var success = doc.SaveTo( options.OutputPath );
        if ( !success )
            throw new InvalidOperationException( $"Failed to save OFB document to '{options.OutputPath}'" );
    }

    /// <summary>
    /// Composes the title page with OFB title, place name, and date.
    /// </summary>
    private async Task ComposeTitlePageAsync( IUserDocument doc, OFBGenerateOptions options )
    {
        // Title (headline level 1)
        var titleHeadline = doc.AddHeadline( 1, "Titel" );
        titleHeadline.TextContent = options.Title;

        // Subtitle with place info
        var placeInfo = string.IsNullOrEmpty( options.PlaceId ) ? "alle Orte" : $"Ort: {options.PlaceId}";
        var subtitle = doc.AddParagraph( "Normaler Absatz" );
        subtitle.TextContent = $"{placeInfo}, erstellt {DateTime.Now:yyyy-MM-dd}";
    }

    /// <summary>
    /// Composes the preface section if provided.
    /// </summary>
    private async Task ComposePrefaceAsync( IUserDocument doc, OFBGenerateOptions options )
    {
        if ( string.IsNullOrWhiteSpace( options.Preface ) )
            return;

        var prefaceHeadline = doc.AddHeadline( 1, "Vorwort" );
        prefaceHeadline.TextContent = "Vorwort";
        var prefacePara = doc.AddParagraph( "Normaler Absatz" );
        prefacePara.TextContent = options.Preface;
    }

    /// <summary>
    /// Composes the character explanation/legend showing OFB symbols.
    /// </summary>
    private async Task ComposeLegendAsync( IUserDocument doc, string legend )
    {
        var legendHeadline = doc.AddHeadline( 1, "Zeichenerklärung" );
        legendHeadline.TextContent = "Zeichenerklärung";

        // Standard OFB character conventions (user confirmed)
        var lines = new[]
        {
            "* = Geboren",
            "~ = Getauft",
            "+ = Gestorben",
            "= = Begraben",
            "",
            "oo = Hochzeit (verheiratet)",
            "o-o = Verbindung (unverheiratet)",
            "o/o = Geschieden"
        };

        foreach ( var line in lines )
        {
            var para = doc.AddParagraph( "Normaler Absatz" );
            para.TextContent = line;
        }

        if ( !string.IsNullOrWhiteSpace( legend ) )
        {
            var legendPara = doc.AddParagraph( "Normaler Absatz" );
            legendPara.TextContent = $"\n{legend}";
        }
    }

    /// <summary>
    /// Composes all index sections (Person, Occupation, Property, Place).
    /// </summary>
    private async Task ComposeIndicesAsync( IUserDocument doc, OFBIndexEntry[] personIndex,
        OFBIndexEntry[] occIndex, OFBIndexEntry[] propIndex,
        OFBPlaceHierarchyNode[] placeHierarchy, OFBIndexEntry[] placeAlpha )
    {
        // Person Index (alphabetical)
        var personTitle = doc.AddHeadline( 1, "Personenindex" );
        personTitle.TextContent = "Personenindex";
        foreach ( var entry in personIndex )
        {
            var para = doc.AddParagraph( "Normaler Absatz" );
            para.TextContent = $"{entry.SortKey} — Fam. {entry.Ref}";
        }

        // Occupation Index (alphabetical)
        var occTitle = doc.AddHeadline( 1, "Berufsindex" );
        occTitle.TextContent = "Berufsindex";
        foreach ( var entry in occIndex )
        {
            var para = doc.AddParagraph( "Normaler Absatz" );
            para.TextContent = $"{entry.SortKey} — {entry.Ref}";
        }

        // Property Index (alphabetical, decision 1A)
        var propTitle = doc.AddHeadline( 1, "Besitzindex" );
        propTitle.TextContent = "Eigentums-/Besitz-Index";
        foreach ( var entry in propIndex )
        {
            var para = doc.AddParagraph( "Normaler Absatz" );
            para.TextContent = $"{entry.SortKey} — {entry.Ref}";
        }

        // Place Hierarchy Index (tree structure)
        var phTitle = doc.AddHeadline( 1, "Ortsindex (Hierarchisch)" );
        phTitle.TextContent = "Ortsindex";
        await WritePlaceHierarchyAsync( doc, placeHierarchy, indent: 0 );

        // Place Alphabetical Index (A-Z flat list)
        var paTitle = doc.AddHeadline( 1, "Ortsindex (Alphabetisch)" );
        paTitle.TextContent = "Ortsindex Alphabetisch";
        foreach ( var entry in placeAlpha )
        {
            var para = doc.AddParagraph( "Normaler Absatz" );
            para.TextContent = $"{entry.SortKey} — {entry.Ref}";
        }
    }

    /// <summary>
    /// Recursively writes place hierarchy nodes as indented paragraphs.
    /// </summary>
    private async Task WritePlaceHierarchyAsync( IUserDocument doc, OFBPlaceHierarchyNode[] nodes, int indent )
    {
        foreach ( var node in nodes )
        {
            var prefix = new string( ' ', indent * 2 );
            var para = doc.AddParagraph( "Normaler Absatz" );
            para.TextContent = $"{prefix}{node.Name} ({node.PlaceId})";

            if ( node.Children != null && node.Children.Length > 0 )
                await WritePlaceHierarchyAsync( doc, node.Children, indent + 1 );
        }
    }

    /// <summary>
    /// Composes the main body: all families with full details.
    /// </summary>
    private async Task ComposeFamiliesAsync( IUserDocument doc, IReadOnlyList<OFBFamily> sortedFamilies )
    {
        foreach ( var family in sortedFamilies )
        {
            // Family header with global number and name group
            var header = $"Familie {family.GlobalNumber}: {family.FamilyName}";
            var headline = doc.AddHeadline( 1, header );
            headline.TextContent = header;

            // Husband
            if ( family.Husband != null )
                AppendPersonDetails( doc, family.Husband );

            // Wife
            if ( family.Wife != null )
                AppendPersonDetails( doc, family.Wife );

            // Marriage details
            if ( family.MarriageDate != null )
            {
                var marrPara = doc.AddParagraph( "Normaler Absatz" );
                marrPara.TextContent = $"Hochzeit: {family.MarriageDate}";
            }

            // Children
            foreach ( var child in family.Children )
            {
                var birth = !string.IsNullOrEmpty( child.BirthDate?.ToString() )
                    ? child.BirthDate.ToString()
                    : "~ n.v.";
                var childPara = doc.AddParagraph( "Normaler Absatz" );
                childPara.TextContent = $"* {child.Name} ({birth})";
            }

            // Blank line separator between families
            var blankPara = doc.AddParagraph( "Normaler Absatz" );
            blankPara.TextContent = "";
        }
    }

    /// <summary>
    /// Appends person details (vital events) to the document.
    /// </summary>
    private void AppendPersonDetails( IUserDocument doc, IGenPerson person )
    {
        var parts = new List<string>() { person.Name };

        if ( !string.IsNullOrEmpty( person.BirthDate?.ToString() ) )
            parts.Add( $"* {person.BirthDate}" );
        if ( !string.IsNullOrEmpty( person.DeathDate?.ToString() ) )
            parts.Add( $"+ {person.DeathDate}" );

        var detailLine = string.Join( "  ", parts );
        var para = doc.AddParagraph( "Normaler Absatz" );
        para.TextContent = detailLine;
    }

    /// <summary>
    /// Generates alphabetical person index entries for all individuals.
    /// </summary>
    private async Task<OFBIndexEntry[]> GeneratePersonIndexAsync( IReadOnlyList<OFBFamily> sortedFamilies )
    {
        // Collect unique persons from all families (use OFBFamily which has GlobalNumber)
        var seen = new HashSet<string>();
        var entries = new List<OFBIndexEntry>();

        foreach ( var family in sortedFamilies )
        {
            if ( family.Husband != null )
            {
                var key = family.Husband.IndRefID ?? family.Husband.Name;
                if ( seen.Add( key ) )
                    entries.Add( OFBIndexEntry.FromCombined( family.Husband.Surname, family.Husband.GivenName, $"Fam.{family.GlobalNumber}" ) );
            }

            if ( family.Wife != null )
            {
                var key = family.Wife.IndRefID ?? family.Wife.Name;
                if ( seen.Add( key ) )
                    entries.Add( OFBIndexEntry.FromCombined( family.Wife.Surname, family.Wife.GivenName, $"Fam.{family.GlobalNumber}" ) );
            }

            foreach ( var child in family.Children )
            {
                var key = child.IndRefID ?? child.Name;
                if ( seen.Add( key ) )
                    entries.Add( OFBIndexEntry.FromCombined( child.Surname, child.GivenName, $"Fam.{family.GlobalNumber}K" ) );
            }
        }

        // Sort alphabetically by full name key
        entries.Sort( ( a, b ) => string.Compare( a.SortKey, b.SortKey, StringComparison.Ordinal ) );
        return [.. entries];
    }

    /// <summary>
    /// Generates alphabetical occupation index with person/family references.
    /// </summary>
    private async Task<OFBIndexEntry[]> GenerateOccupationIndexAsync( IReadOnlyList<OFBFamily> sortedFamilies )
    {
        var entries = new List<OFBIndexEntry>();
        var seen = new HashSet<string>( StringComparer.Ordinal );

        foreach ( var family in sortedFamilies )
        {
            foreach ( var person in new[] { family.Husband, family.Wife }.Where( p => p != null ) )
                if ( !string.IsNullOrEmpty( person.Occupation ) && seen.Add( person.Occupation ) )
                    entries.Add( OFBIndexEntry.FromSingle( person.Occupation, $"Fam.{family.GlobalNumber}" ) );

            foreach ( var child in family.Children )
                if ( !string.IsNullOrEmpty( child.Occupation ) && seen.Add( child.Occupation ) )
                    entries.Add( OFBIndexEntry.FromSingle( child.Occupation, $"Fam.{family.GlobalNumber}K" ) );
        }

        entries.Sort( ( a, b ) => string.Compare( a.SortKey, b.SortKey, StringComparison.Ordinal ) );
        return [.. entries];
    }

    /// <summary>
    /// Generates property/ownership index from family data.
    /// Implements decision 1A: Property Index included.
    /// </summary>
    private async Task<OFBIndexEntry[]> GeneratePropertyIndexAsync( IReadOnlyList<OFBFamily> sortedFamilies )
    {
        var entries = new List<OFBIndexEntry>();

        foreach ( var family in sortedFamilies )
        {
            // Extract property info from family facts/narratives
            var properties = ExtractPropertiesFromFamily( family );
            foreach ( var prop in properties )
                entries.Add( OFBIndexEntry.FromSingle( prop, $"Fam.{family.GlobalNumber}" ) );
        }

        entries.Sort( ( a, b ) => string.Compare( a.SortKey, b.SortKey, StringComparison.Ordinal ) );
        return [.. entries];
    }

    /// <summary>
    /// Extracts property names from family facts.
    /// Placeholder — actual extraction depends on GEDCOM NARR/NOTE parsing.
    /// </summary>
    private IEnumerable<string> ExtractPropertiesFromFamily( OFBFamily family )
    {
        // TODO: Parse family notes/facts for property references (e.g., "Besitz", "Eigentum")
        yield break;
    }

    /// <summary>
    /// Converts abstraction-layer OFBFamilyModel instances to Core-layer OFBFamily instances.
    /// Both models share identical properties; this bridges the layer boundary.
    /// </summary>
    private static IReadOnlyList<OFBFamily> ConvertToCoreFamilies( IReadOnlyList<OFBFamilyModel> models )
    {
        var result = new OFBFamily[models.Count];
        for ( int i = 0; i < models.Count; i++ )
        {
            var m = models[i];
            result[i] = new OFBFamily
            {
                GlobalNumber   = m.GlobalNumber,
                FamilyName     = m.FamilyName,
                Husband        = m.Husband,
                Wife           = m.Wife,
                Children       = m.Children,
                MarriageDate   = m.MarriageDate,
                MarriagePlace  = m.MarriagePlace,
                FormationDate  = m.FormationDate,
                SourceRefId    = m.SourceRefId
            };
        }
        return result;
    }

    /// <summary>
    /// Generates hierarchical place index from all families in the output set.
    /// </summary>
    private async Task<OFBPlaceHierarchyNode[]> GeneratePlaceHierarchyIndexAsync( IReadOnlyList<OFBFamily> families )
    {
        // Collect unique places and build tree structure
        var placeMap = new Dictionary<string, OFBPlaceHierarchyNode>( StringComparer.Ordinal );

        foreach ( var family in families )
        {
            if ( !string.IsNullOrEmpty( family.MarriagePlace?.GOV_ID ) && !placeMap.ContainsKey( family.MarriagePlace.GOV_ID ) )
                placeMap[family.MarriagePlace.GOV_ID] = new OFBPlaceHierarchyNode( family.MarriagePlace.Name, family.MarriagePlace.GOV_ID );

            foreach ( var child in family.Children )
            {
                if ( child.BirthPlace != null && !string.IsNullOrEmpty( child.BirthPlace.GOV_ID ) )
                    placeMap[child.BirthPlace.GOV_ID] = new OFBPlaceHierarchyNode( child.BirthPlace.Name, child.BirthPlace.GOV_ID );

                // Add parents' birth places too
                if ( family.Husband?.BirthPlace != null && !string.IsNullOrEmpty( family.Husband.BirthPlace.GOV_ID ) )
                    placeMap[family.Husband.BirthPlace.GOV_ID] = new OFBPlaceHierarchyNode( family.Husband.BirthPlace.Name, family.Husband.BirthPlace.GOV_ID );

                if ( family.Wife?.BirthPlace != null && !string.IsNullOrEmpty( family.Wife.BirthPlace.GOV_ID ) )
                    placeMap[family.Wife.BirthPlace.GOV_ID] = new OFBPlaceHierarchyNode( family.Wife.BirthPlace.Name, family.Wife.BirthPlace.GOV_ID );
            }
        }

        // TODO: Build parent-child relationships from GEDCOM place hierarchy data
        return [.. placeMap.Values];
    }

    /// <summary>
    /// Generates flat alphabetical place index (A-Z) for cross-reference.
    /// Implements decision 2A: Place Alphabetical Index included.
    /// </summary>
    private async Task<OFBIndexEntry[]> GeneratePlaceAlphabeticalIndexAsync( IReadOnlyList<OFBFamily> families )
    {
        var seen = new HashSet<string>( StringComparer.Ordinal );
        var entries = new List<OFBIndexEntry>();

        foreach ( var family in families )
        {
            if ( !string.IsNullOrEmpty( family.MarriagePlace?.Name ) && seen.Add( family.MarriagePlace.Name ) )
                entries.Add( OFBIndexEntry.FromSingle( family.MarriagePlace.Name, $"Fam.{family.SourceRefId}" ) );

            foreach ( var child in family.Children )
            {
                if ( child.BirthPlace != null && !string.IsNullOrEmpty( child.BirthPlace.Name ) && seen.Add( child.BirthPlace.Name ) )
                    entries.Add( OFBIndexEntry.FromSingle( child.BirthPlace.Name, $"Fam.{family.SourceRefId}K" ) );
            }
        }

        entries.Sort( ( a, b ) => string.Compare( a.SortKey, b.SortKey, StringComparison.Ordinal ) );
        return [.. entries];
    }
}
