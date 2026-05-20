using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Extracts typed IEC declarations from declaration text blocks.
/// The first implementation slice targets exported method/interface text and keeps the
/// parsing rules intentionally small and deterministic for the current test fixtures.
/// </summary>
public static class IecDeclarationExtractor
{
    private static readonly Regex _headerPattern = new(
        @"^(?<kind>METHOD|FUNCTION|FUNCTION_BLOCK|PROGRAM)\s+(?:(?<accessibility>PUBLIC|PROTECTED|PRIVATE|INTERNAL)\s+)?(?<name>[A-Za-z_][A-Za-z0-9_]*)\s*(?::\s*(?<returnType>[A-Za-z_][A-Za-z0-9_]*))?\s*;?$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    /// <summary>
    /// Extracts typed variable declarations from IEC declaration text.
    /// </summary>
    /// <param name="declarationText">The raw declaration text.</param>
    /// <returns>The extracted declarations.</returns>
    public static IReadOnlyList<IecVariableDeclaration> ExtractDeclarations(string declarationText)
    {
        var declarations = new List<IecVariableDeclaration>();
        var lines = declarationText
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace("\r", "\n", StringComparison.Ordinal)
            .Split('\n');

        var currentSection = IecDeclarationSection.Unknown;
        for (var i = 0; i < lines.Length; i++)
        {
            var trimmedLine = RemoveInlineComment(lines[i]).Trim();
            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                continue;
            }

            currentSection = trimmedLine.ToUpperInvariant() switch
            {
                "VAR_INPUT" => IecDeclarationSection.Input,
                "VAR_OUTPUT" => IecDeclarationSection.Output,
                "VAR_IN_OUT" => IecDeclarationSection.InOut,
                "VAR_INST" => IecDeclarationSection.Instance,
                "VAR" => IecDeclarationSection.Local,
                "END_VAR" => IecDeclarationSection.Unknown,
                _ => currentSection,
            };

            if (currentSection == IecDeclarationSection.Unknown || !trimmedLine.Contains(':', StringComparison.Ordinal))
            {
                continue;
            }

            var colonIndex = trimmedLine.IndexOf(':', StringComparison.Ordinal);
            var identifier = trimmedLine[..colonIndex].Trim();
            if (string.IsNullOrWhiteSpace(identifier))
            {
                continue;
            }

            var remainder = trimmedLine[(colonIndex + 1)..].Trim().TrimEnd(';').Trim();
            string? initializerText = null;
            var initializerIndex = remainder.IndexOf(":=", StringComparison.Ordinal);
            if (initializerIndex >= 0)
            {
                initializerText = remainder[(initializerIndex + 2)..].Trim();
                remainder = remainder[..initializerIndex].Trim();
            }

            var typeName = string.IsNullOrWhiteSpace(remainder) ? null : remainder;
            declarations.Add(new IecVariableDeclaration(identifier, typeName, currentSection, initializerText, i));
        }

        return declarations;
    }

    /// <summary>
    /// Extracts shared artifact metadata and exported declaration header information from IEC declaration text.
    /// </summary>
    /// <param name="declarationText">The raw declaration text.</param>
    /// <returns>The extracted declaration header information.</returns>
    public static IecDeclarationHeader ExtractDeclarationHeader(string declarationText)
    {
        var lines = declarationText
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace("\r", "\n", StringComparison.Ordinal)
            .Split('\n');

        for (var i = 0; i < lines.Length; i++)
        {
            var trimmedLine = RemoveInlineComment(lines[i]).Trim();
            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                continue;
            }

            var match = _headerPattern.Match(trimmedLine);
            if (!match.Success)
            {
                continue;
            }

            var artifactKind = ParseArtifactKind(match.Groups["kind"].Value);
            var accessibility = ParseAccessibility(match.Groups["accessibility"].Value);
            var artifactName = match.Groups["name"].Value;
            var returnTypeName = match.Groups["returnType"].Success ? match.Groups["returnType"].Value : null;
            return new IecDeclarationHeader(artifactName, returnTypeName, new IecArtifactMetadata(artifactKind, accessibility), i);
        }

        return new IecDeclarationHeader(null, null, new IecArtifactMetadata());
    }

    /// <summary>
    /// Creates a lightweight compilation unit from declaration and implementation text.
    /// The current implementation extracts declarations and leaves executable statements
    /// to the existing statement parsing pipeline.
    /// </summary>
    /// <param name="declarationText">The raw declaration text.</param>
    /// <param name="implementationText">The raw implementation text.</param>
    /// <returns>The typed IEC compilation unit.</returns>
    public static IecCompilationUnit CreateCompilationUnit(string declarationText, string implementationText)
    {
        _ = implementationText;
        var declarations = ExtractDeclarations(declarationText);
        var header = ExtractDeclarationHeader(declarationText);
        return new IecCompilationUnit(declarations, Array.Empty<IecStatement>(), header.ArtifactMetadata, header.SourcePos);
    }

    private static IecArtifactKind ParseArtifactKind(string value)
    {
        return value.ToUpperInvariant() switch
        {
            "FUNCTION" or "METHOD" => IecArtifactKind.Function,
            "FUNCTION_BLOCK" => IecArtifactKind.FunctionBlock,
            "PROGRAM" => IecArtifactKind.Program,
            _ => IecArtifactKind.Function,
        };
    }

    private static IecAccessibility ParseAccessibility(string value)
    {
        return value.ToUpperInvariant() switch
        {
            "PUBLIC" => IecAccessibility.Public,
            "PROTECTED" => IecAccessibility.Protected,
            "PRIVATE" => IecAccessibility.Private,
            "INTERNAL" => IecAccessibility.Internal,
            _ => IecAccessibility.Public,
        };
    }

    private static string RemoveInlineComment(string line)
    {
        var commentIndex = line.IndexOf("//", StringComparison.Ordinal);
        return commentIndex >= 0 ? line[..commentIndex] : line;
    }
}
