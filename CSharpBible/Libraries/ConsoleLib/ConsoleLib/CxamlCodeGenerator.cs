using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using ConsoleLib.Interfaces;

namespace ConsoleLib;

/// <summary>Generates deterministic C# control construction from valid CXAML.</summary>
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

        try
        {
            var root = Parse(markup);
            var builder = new StringBuilder();
            builder.AppendLine("using System;");
            builder.AppendLine("using System.Drawing;");
            builder.AppendLine("using ConsoleLib.Interfaces;");
            builder.AppendLine();
            builder.Append("namespace ").Append(namespaceName).AppendLine(";");
            builder.AppendLine();
            builder.Append("public static class ").Append(className).AppendLine();
            builder.AppendLine("{");
            builder.AppendLine("    public static IControl Create()");
            builder.AppendLine("    {");
            var counter = 0;
            var rootVariable = EmitNode(builder, root, 2, ref counter);
            builder.Append("        return ").Append(rootVariable).AppendLine(";");
            builder.AppendLine("    }");
            builder.AppendLine("}");
            return new CxamlGenerationResult(builder.ToString(), Array.Empty<CxamlDiagnostic>());
        }
        catch (XmlException error)
        {
            return new CxamlGenerationResult(string.Empty, new[]
            {
                new CxamlDiagnostic(CxamlDiagnosticSeverity.Error, "Invalid CXAML markup: " + error.Message)
            });
        }
        catch (CxamlParseException error)
        {
            return new CxamlGenerationResult(string.Empty, new[]
            {
                new CxamlDiagnostic(CxamlDiagnosticSeverity.Error, error.Message)
            });
        }
    }

    private static Node Parse(string markup)
    {
        using var reader = XmlReader.Create(new StringReader(markup),
            new XmlReaderSettings { IgnoreComments = true, IgnoreWhitespace = true });
        if (reader.MoveToContent() == XmlNodeType.None)
            throw new CxamlParseException("CXAML markup does not contain a root control.");
        var root = ReadNode(reader);
        if (reader.Read() && reader.MoveToContent() == XmlNodeType.Element)
            throw new CxamlParseException("CXAML markup contains more than one root control.");
        return root;
    }

    private static Node ReadNode(XmlReader reader)
    {
        var node = new Node(reader.Name);
        if (reader.HasAttributes)
        {
            while (reader.MoveToNextAttribute())
            {
                if (reader.Prefix == "xmlns" || reader.Name == "xmlns")
                    continue;
                node.Attributes.Add((reader.Name, reader.Value));
            }
            reader.MoveToElement();
        }

        if (reader.IsEmptyElement)
            return node;

        var depth = reader.Depth;
        reader.Read();
        while (!(reader.NodeType == XmlNodeType.EndElement && reader.Depth == depth))
        {
            if (reader.NodeType == XmlNodeType.Element)
                node.Children.Add(ReadNode(reader));
            else if (reader.NodeType == XmlNodeType.Text && !string.IsNullOrWhiteSpace(reader.Value))
                node.Text = reader.Value;
            reader.Read();
        }
        return node;
    }

    private static string EmitNode(StringBuilder builder, Node node, int indent, ref int counter)
    {
        var variable = "control" + counter++;
        var typeName = node.Name.Contains(':', StringComparison.Ordinal)
            ? node.Name.Replace(":", ".", StringComparison.Ordinal)
            : "ConsoleLib.CommonControls." + node.Name;
        AppendIndent(builder, indent);
        builder.Append("var ").Append(variable).Append(" = new ").Append(typeName).AppendLine("();");

        foreach (var attribute in node.Attributes)
            EmitAttribute(builder, variable, node.Name, attribute.Name, attribute.Value, indent);
        if (node.Text is not null)
            EmitAssignment(builder, variable + ".Text", Quote(node.Text), indent);

        foreach (var child in node.Children)
        {
            var childVariable = EmitNode(builder, child, indent, ref counter);
            AppendIndent(builder, indent);
            builder.Append(variable).Append(".Add(").Append(childVariable).AppendLine(");");
        }
        return variable;
    }

    private static void EmitAttribute(StringBuilder builder, string variable, string controlName, string name, string value, int indent)
    {
        if (name == "Name")
            return;
        if (value.StartsWith("{Binding ", StringComparison.Ordinal))
            throw new CxamlParseException("Direct C# generation does not support bindings yet: " + name);

        var expression = name switch
        {
            "Text" or "Tag" => Quote(value),
            "Accelerator" => Quote(value),
            "Visible" or "Enabled" or "Shadow" or "IsChecked" => Bool(value, name),
            "BackColor" or "ForeColor" or "BorderColor" or "HLBackColor" => "ConsoleColor." + value,
            "BorderStyle" => "ConsoleLib.CommonControls.BorderStyle." + value,
            "Width" or "Height" or "X" or "Y" or "Grid.Row" or "Grid.Column" or "Grid.RowSpan" or "Grid.ColumnSpan"
                => Int(value, name),
            _ => throw new CxamlParseException("Direct C# generation does not support attribute '" + name + "'.")
        };

        if (name.StartsWith("Grid.", StringComparison.Ordinal))
        {
            var method = name switch
            {
                "Grid.Row" => "SetRow",
                "Grid.Column" => "SetColumn",
                "Grid.RowSpan" => "SetRowSpan",
                _ => "SetColumnSpan"
            };
            EmitAssignment(builder, "ConsoleLib.CommonControls.Grid." + method + "(" + variable + ", " + expression + ")", null, indent);
            return;
        }
        if (name is "Width" or "Height" or "X" or "Y")
        {
            var dimension = name switch
            {
                "Width" => "new Size(" + expression + ", " + variable + ".size.Height)",
                "Height" => "new Size(" + variable + ".size.Width, " + expression + ")",
                "X" => "new Point(" + expression + ", " + variable + ".Position.Y)",
                _ => "new Point(" + variable + ".Position.X, " + expression + ")"
            };
            EmitAssignment(builder, variable + (name is "Width" or "Height" ? ".size" : ".Position"), dimension, indent);
            return;
        }
        EmitAssignment(builder, variable + "." + name, expression, indent);
    }

    private static void EmitAssignment(StringBuilder builder, string target, string? expression, int indent)
    {
        AppendIndent(builder, indent);
        builder.Append(target);
        if (expression is not null)
            builder.Append(" = ").Append(expression);
        builder.AppendLine(";");
    }

    private static string Quote(string value) => "\"" + value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n") + "\"";
    private static string Int(string value, string name) => int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)
        ? result.ToString(CultureInfo.InvariantCulture)
        : throw new CxamlParseException("Invalid integer value for " + name + ": " + value);
    private static string Bool(string value, string name) => bool.TryParse(value, out var result)
        ? result.ToString().ToLowerInvariant()
        : throw new CxamlParseException("Invalid boolean value for " + name + ": " + value);
    private static void AppendIndent(StringBuilder builder, int indent) => builder.Append(' ', indent * 4);

    private sealed class Node
    {
        public Node(string name) => Name = name;
        public string Name { get; }
        public string? Text { get; set; }
        public List<(string Name, string Value)> Attributes { get; } = new();
        public List<Node> Children { get; } = new();
    }
}
