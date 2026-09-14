using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml;
using ConsoleLib.Interfaces;
using ConsoleLib.CommonControls;
using ConsoleLib.Data;

namespace ConsoleLib;

/// <summary>Minimal reflection-based CXAML runtime loader for ConsoleLib controls.</summary>
public sealed class CxamlLoader : ICxamlLoader, ICxamlValidator
{
    private readonly ICxamlComponentRegistry? _components;

    public CxamlLoader(ICxamlComponentRegistry? components = null)
        => _components = components;

    public IReadOnlyList<CxamlDiagnostic> Validate(TextReader markup)
    {
        if (markup is null)
            throw new ArgumentNullException(nameof(markup));

        var diagnostics = new List<CxamlDiagnostic>();
        try
        {
            using var reader = XmlReader.Create(markup, new XmlReaderSettings { IgnoreComments = true, IgnoreWhitespace = true });
            if (reader.MoveToContent() == XmlNodeType.None)
            {
                diagnostics.Add(new CxamlDiagnostic(CxamlDiagnosticSeverity.Error, "CXAML markup does not contain a root control."));
                return diagnostics;
            }

            ValidateControl(reader, diagnostics, new Dictionary<string, string>(StringComparer.Ordinal));
        }
        catch (XmlException error)
        {
            diagnostics.Add(new CxamlDiagnostic(CxamlDiagnosticSeverity.Error, error.Message));
        }
        return diagnostics;
    }

    public IControl Load(TextReader markup)
        => LoadCore(markup, null, new Dictionary<string, IControl>(StringComparer.Ordinal));

    public CxamlLoadResult Load(TextReader markup, CxamlLoadContext context)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        var namedControls = new Dictionary<string, IControl>(StringComparer.Ordinal);
        return new CxamlLoadResult(LoadCore(markup, context, namedControls), namedControls);
    }

    /// <summary>Loads markup whose root must be a non-floating page.</summary>
    public CxamlLoadResult LoadPage(TextReader markup, CxamlLoadContext context)
        => LoadExpectedRoot(markup, context, static root => root is Page,
            "CXAML page roots must use <Page>.");

    /// <summary>Loads markup whose root must be a reusable user control.</summary>
    public CxamlLoadResult LoadUserControl(TextReader markup, CxamlLoadContext context)
        => LoadExpectedRoot(markup, context, static root => root is UserControl,
            "CXAML user-control roots must use <UserControl>.");

    /// <summary>Loads markup whose root must be a floating dialog.</summary>
    public CxamlLoadResult LoadDialog(TextReader markup, CxamlLoadContext context)
        => LoadExpectedRoot(markup, context, static root => root is Dialog,
            "CXAML dialog roots must use <Dialog>.");

    private CxamlLoadResult LoadExpectedRoot(
        TextReader markup,
        CxamlLoadContext context,
        Func<IControl, bool> predicate,
        string message)
    {
        var result = Load(markup, context);
        if (!predicate(result.Root))
            throw new CxamlParseException(message);
        return result;
    }

    private IControl LoadCore(TextReader markup, CxamlLoadContext? context, IDictionary<string, IControl>? namedControls)
    {
        if (markup is null)
            throw new ArgumentNullException(nameof(markup));

        try
        {
            using var reader = XmlReader.Create(markup, new XmlReaderSettings { IgnoreComments = true, IgnoreWhitespace = true });
            if (reader.MoveToContent() == XmlNodeType.None)
                throw new CxamlParseException("CXAML markup does not contain a root control.");
            var control = ReadControl(reader, context, namedControls);
            if (reader.Read() && reader.MoveToContent() == XmlNodeType.Element)
                throw new CxamlParseException("CXAML markup contains more than one root control.");
            return control;
        }
        catch (XmlException error)
        {
            throw new CxamlParseException("Invalid CXAML markup: " + error.Message, error);
        }
    }

    private void ValidateControl(XmlReader reader, ICollection<CxamlDiagnostic> diagnostics, IDictionary<string, string> inheritedXmlns)
    {
        var controlName = reader.LocalName;
        var type = Type.GetType("ConsoleLib.CommonControls." + controlName + ", ConsoleLib", throwOnError: false);
        if (type is null || !typeof(IControl).IsAssignableFrom(type))
            diagnostics.Add(new CxamlDiagnostic(CxamlDiagnosticSeverity.Error, "Unsupported CXAML control: " + controlName));

        var xmlns = new Dictionary<string, string>(inheritedXmlns, StringComparer.Ordinal);
        if (reader.HasAttributes)
        {
            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == "xmlns" || reader.Prefix == "xmlns")
                {
                    if (reader.Name == "xmlns")
                        xmlns[string.Empty] = reader.Value;
                    else
                        xmlns[reader.LocalName] = reader.Value;
                    continue;
                }

                if (!CxamlAttributeApplicator.IsSupported(reader.Name, type))
                    diagnostics.Add(new CxamlDiagnostic(CxamlDiagnosticSeverity.Error,
                        "Unsupported CXAML attribute '" + reader.LocalName + "' on " + controlName));
            }
            reader.MoveToElement();
        }

        ValidateNamespace(reader, controlName, xmlns, diagnostics);

        var isRoot = reader.Depth == 0;
        var framePageCount = 0;
        if (!reader.IsEmptyElement)
        {
            var depth = reader.Depth;
            reader.Read();
            while (!(reader.NodeType == XmlNodeType.EndElement && reader.Depth == depth))
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (isRoot && controlName == "Frame" && reader.LocalName == "Page")
                        framePageCount++;

                    if (type == typeof(Grid) && IsGridDefinitionElement(reader.Name))
                        ValidateGridDefinitions(reader, diagnostics);
                    else
                        ValidateControl(reader, diagnostics, xmlns);
                }
                reader.Read();
            }
        }

        if (isRoot && controlName == "Frame" && framePageCount != 1)
            diagnostics.Add(new CxamlDiagnostic(CxamlDiagnosticSeverity.Warning,
                "A Frame root should host exactly one Page."));
    }

    private static void ValidateNamespace(XmlReader reader, string controlName, IReadOnlyDictionary<string, string> xmlns, ICollection<CxamlDiagnostic> diagnostics)
    {
        var prefix = reader.Prefix;
        if (string.IsNullOrEmpty(prefix) || prefix == "xmlns")
            return;

        if (!xmlns.TryGetValue(prefix, out var uri))
        {
            diagnostics.Add(new CxamlDiagnostic(CxamlDiagnosticSeverity.Error,
                "Unbound XML namespace prefix '" + prefix + "' on " + controlName + "."));
        }
        else if (!CxamlNamespaces.Recognized.Contains(uri))
        {
            diagnostics.Add(new CxamlDiagnostic(CxamlDiagnosticSeverity.Warning,
                "Unrecognized CXAML namespace '" + uri + "' for prefix '" + prefix + "'."));
        }
    }

    private static void ValidateGridDefinitions(XmlReader reader, ICollection<CxamlDiagnostic> diagnostics)
    {
        var isRows = reader.Name == "Grid.RowDefinitions";
        if (reader.IsEmptyElement)
            return;

        var depth = reader.Depth;
        reader.Read();
        while (!(reader.NodeType == XmlNodeType.EndElement && reader.Depth == depth))
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                var expected = isRows ? "RowDefinition" : "ColumnDefinition";
                if (reader.LocalName != expected)
                    diagnostics.Add(new CxamlDiagnostic(CxamlDiagnosticSeverity.Error,
                        "Unsupported Grid definition element: " + reader.Name));
                else
                {
                    var attribute = isRows ? "Height" : "Width";
                    if (reader.HasAttributes)
                    {
                        while (reader.MoveToNextAttribute())
                            if (reader.LocalName != attribute)
                                diagnostics.Add(new CxamlDiagnostic(CxamlDiagnosticSeverity.Error,
                                    "Unsupported Grid definition attribute '" + reader.Name + "'."));
                        reader.MoveToElement();
                    }
                }
            }
            reader.Read();
        }
    }

    private IControl ReadControl(XmlReader reader, CxamlLoadContext? context, IDictionary<string, IControl>? namedControls)
    {
        var control = CreateControl(reader.Name, reader.LocalName, context);
        int? width = null;
        int? height = null;
        int? x = null;
        int? y = null;
        if (reader.HasAttributes)
        {
            while (reader.MoveToNextAttribute())
            {
                if (reader.Prefix == "xmlns" || reader.Name == "xmlns")
                    continue;
                switch (reader.LocalName)
                {
                    case "Width":
                        width = CxamlAttributeApplicator.ParseInt(reader.Value, reader.LocalName);
                        break;
                    case "Height":
                        height = CxamlAttributeApplicator.ParseInt(reader.Value, reader.LocalName);
                        break;
                    case "X":
                        x = CxamlAttributeApplicator.ParseInt(reader.Value, reader.LocalName);
                        break;
                    case "Y":
                        y = CxamlAttributeApplicator.ParseInt(reader.Value, reader.LocalName);
                        break;
                    default:
                        CxamlAttributeApplicator.Apply(control, reader.Name, reader.Value, context, namedControls);
                        break;
                }
            }
            reader.MoveToElement();
        }
        if (width.HasValue || height.HasValue || x.HasValue || y.HasValue)
        {
            control.Dimension = new Rectangle(
                x ?? control.Position.X,
                y ?? control.Position.Y,
                width ?? control.size.Width,
                height ?? control.size.Height);
        }

        if (reader.IsEmptyElement)
            return control;

        var depth = reader.Depth;
        reader.Read();
        while (!(reader.NodeType == XmlNodeType.EndElement && reader.Depth == depth))
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                if (control is Grid grid && IsGridDefinitionElement(reader.Name))
                    ReadGridDefinitions(reader, grid);
                else
                    control.Add(ReadControl(reader, context, namedControls));
            }
            else if (reader.NodeType == XmlNodeType.Text && !string.IsNullOrWhiteSpace(reader.Value))
            {
                control.Text = reader.Value;
            }
            reader.Read();
        }
        return control;
    }

    private IControl CreateControl(string xmlName, string localName, CxamlLoadContext? context)
    {
        if (_components is not null && context is not null && _components.TryCreate(xmlName, context, out var component))
            return component;

        var type = Type.GetType("ConsoleLib.CommonControls." + localName + ", ConsoleLib", throwOnError: false)
            ?? AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType("ConsoleLib.Showcase.Desktop.Controls." + localName, throwOnError: false))
                .FirstOrDefault(candidate => candidate is not null);
        if (type is null || !typeof(IControl).IsAssignableFrom(type))
            throw new CxamlParseException("Unsupported CXAML control: " + xmlName);
        try
        {
            return (IControl)Activator.CreateInstance(type)!;
        }
        catch (MissingMethodException error)
        {
            throw new CxamlParseException("Unable to create CXAML control: " + xmlName, error);
        }
        catch (MemberAccessException error)
        {
            throw new CxamlParseException("Unable to create CXAML control: " + xmlName, error);
        }
        catch (InvalidCastException error)
        {
            throw new CxamlParseException("Unable to create CXAML control: " + xmlName, error);
        }
        catch (System.Reflection.TargetInvocationException error)
        {
            throw new CxamlParseException("Unable to create CXAML control: " + xmlName, error);
        }
    }

    private static bool IsGridDefinitionElement(string name) =>
        name is "Grid.RowDefinitions" or "Grid.ColumnDefinitions";

    private static void ReadGridDefinitions(XmlReader reader, Grid grid)
    {
        var isRows = reader.Name == "Grid.RowDefinitions";
        if (reader.IsEmptyElement)
            return;

        var depth = reader.Depth;
        reader.Read();
        while (!(reader.NodeType == XmlNodeType.EndElement && reader.Depth == depth))
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                var definitionName = reader.LocalName;
                if (isRows && definitionName == "RowDefinition")
                    grid.RowDefinitions.Add(new RowDefinition { Height = ReadGridLength(reader, "Height") });
                else if (!isRows && definitionName == "ColumnDefinition")
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = ReadGridLength(reader, "Width") });
                else
                    throw new CxamlParseException("Unsupported Grid definition element: " + reader.Name);
            }
            reader.Read();
        }
    }

    private static GridLength ReadGridLength(XmlReader reader, string attributeName)
    {
        var value = reader.GetAttribute(attributeName);
        if (string.IsNullOrWhiteSpace(value))
            return GridLength.Star;
        return CxamlAttributeApplicator.ParseGridLength(value);
    }


    }
