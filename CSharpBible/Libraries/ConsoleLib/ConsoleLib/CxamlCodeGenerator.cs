using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ConsoleLib.Interfaces;

namespace ConsoleLib;

/// <summary>Generates a deterministic C# factory that loads embedded CXAML markup.</summary>
public sealed class CxamlCodeGenerator
{
    private readonly ICxamlValidator _validator;

    public CxamlCodeGenerator(ICxamlValidator? validator = null) =>
        _validator = validator ?? new CxamlLoader();

    public CxamlGenerationResult Generate(string markup, string className, string namespaceName)
    {
        if (markup is null)
            throw new ArgumentNullException(nameof(markup));
        if (string.IsNullOrWhiteSpace(className))
            throw new ArgumentException("A class name is required.", nameof(className));
        if (string.IsNullOrWhiteSpace(namespaceName))
            throw new ArgumentException("A namespace name is required.", nameof(namespaceName));

        var diagnostics = new List<CxamlDiagnostic>(_validator.Validate(new StringReader(markup)));
        if (diagnostics.Count == 0 && _validator is ICxamlLoader loader)
        {
            try
            {
                loader.Load(new StringReader(markup));
            }
            catch (CxamlParseException error)
            {
                diagnostics.Add(new CxamlDiagnostic(CxamlDiagnosticSeverity.Error, error.Message));
            }
        }
        if (diagnostics.Count != 0)
            return new CxamlGenerationResult(string.Empty, diagnostics);

        var builder = new StringBuilder();
        builder.AppendLine("using System.IO;");
        builder.AppendLine("using ConsoleLib.Interfaces;");
        builder.AppendLine();
        builder.Append("namespace ").Append(namespaceName).AppendLine(";");
        builder.AppendLine();
        builder.Append("public static class ").Append(className).AppendLine();
        builder.AppendLine("{");
        builder.AppendLine("    public static IControl Create()");
        builder.AppendLine("    {");
        builder.Append("        using var markup = new StringReader(\"")
            .Append(EscapeString(markup))
            .AppendLine("\");");
        builder.AppendLine("        return new ConsoleLib.CxamlLoader().Load(markup);");
        builder.AppendLine("    }");
        builder.AppendLine("}");
        return new CxamlGenerationResult(builder.ToString(), Array.Empty<CxamlDiagnostic>());
    }

    private static string EscapeString(string value) =>
        value.Replace("\\", "\\\\").Replace("\"", "\\\"")
            .Replace("\r", "\\r").Replace("\n", "\\n");
}
