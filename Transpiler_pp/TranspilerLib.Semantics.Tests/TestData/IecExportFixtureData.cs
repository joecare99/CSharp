using System;
using System.IO;
using System.Xml;

namespace TranspilerLib.IEC.TestData;

/// <summary>
/// Provides access to IEC export fixtures that are stored as XML-based resource files.
/// The helper extracts the declaration and implementation structured-text blobs so tests
/// can operate on realistic exported source content without duplicating the fixture text.
/// </summary>
public static class IecExportFixtureData
{
    /// <summary>
    /// Loads the bundled fixture for the <c>MF_ComputeAlignmentRot</c> method export.
    /// </summary>
    /// <returns>A tuple containing declaration text and implementation text.</returns>
    public static (string DeclarationText, string ImplementationText) LoadMfComputeAlignmentRot()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Resources", "MF_ComputeAlignmentRot.export");
        return LoadFromPath(path);
    }

    /// <summary>
    /// Loads an IEC export fixture from the specified file path.
    /// </summary>
    /// <param name="path">Absolute path to the export fixture file.</param>
    /// <returns>A tuple containing declaration text and implementation text.</returns>
    public static (string DeclarationText, string ImplementationText) LoadFromPath(string path)
    {
        var document = new XmlDocument();
        document.Load(path);

        var declarationNode = document.SelectSingleNode("//Single[@Name='Interface']//Single[@Name='TextBlobForSerialisation']");
        var implementationNode = document.SelectSingleNode("//Single[@Name='Implementation']//Single[@Name='TextBlobForSerialisation']");

        if (declarationNode == null || implementationNode == null)
        {
            throw new InvalidDataException($"The IEC export fixture '{path}' does not contain the expected declaration and implementation text blobs.");
        }

        return (NormalizeLineEndings(declarationNode.InnerText), NormalizeLineEndings(implementationNode.InnerText));
    }

    /// <summary>
    /// Normalizes line endings so tests remain stable across different runtime environments.
    /// </summary>
    /// <param name="text">The text to normalize.</param>
    /// <returns>The normalized text with <see cref="Environment.NewLine"/> separators.</returns>
    private static string NormalizeLineEndings(string text)
    {
        return text
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace("\r", "\n", StringComparison.Ordinal)
            .Replace("\n", Environment.NewLine, StringComparison.Ordinal);
    }
}
