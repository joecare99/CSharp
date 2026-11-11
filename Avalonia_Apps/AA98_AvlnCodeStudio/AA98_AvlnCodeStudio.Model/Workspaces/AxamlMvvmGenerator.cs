using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>
/// Generates a CommunityToolkit.Mvvm view model and binds explicitly named AXAML controls to it.
/// </summary>
public sealed class AxamlMvvmGenerator
{
    private static readonly XNamespace s_xamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
    private static readonly XNamespace s_avaloniaNamespace = "https://github.com/avaloniaui";
    private const int MaximumMarkupLength = 1_000_000;

    /// <param name="usePartialProperties">
    /// Enable only after verifying that the target project supports Toolkit partial properties
    /// (C# 14 and CommunityToolkit.Mvvm 8.4.1 or later). Otherwise compatible fields are generated.
    /// </param>
    public IReadOnlyList<WorkspaceFileChangeRequest> Generate(
        string axamlRelativePath,
        string markup,
        bool usePartialProperties = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(axamlRelativePath);
        ArgumentNullException.ThrowIfNull(markup);
        axamlRelativePath = axamlRelativePath
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar);
        if (markup.Length > MaximumMarkupLength)
        {
            throw new InvalidDataException("AXAML markup exceeds the MVVM generator size limit.");
        }

        XDocument document = ParseMarkup(markup);
        XElement root = document.Root
            ?? throw new InvalidDataException("AXAML must have a root control.");
        XAttribute classAttribute = root.Attribute(s_xamlNamespace + "Class")
            ?? throw new InvalidDataException("AXAML root must declare x:Class before generating a view model.");
        string viewClassName = classAttribute.Value.Split('.').Last();
        string viewModelNamespace = GetNamespace(classAttribute.Value);
        ValidateQualifiedIdentifier(classAttribute.Value);
        string viewModelName = viewClassName + "ViewModel";

        XAttribute? dataType = root.Attribute(s_xamlNamespace + "DataType");
        if (dataType is not null
            && !IsGeneratedViewModelDataType(root, dataType.Value, viewModelNamespace, viewModelName))
        {
            throw new InvalidDataException("AXAML already declares a different x:DataType; it was not replaced.");
        }

        if (root.Attribute("DataContext") is not null
            || root.Elements().Any(element => element.Name.LocalName == "DataContext"))
        {
            throw new InvalidDataException(
                "AXAML already defines DataContext. The generator will not replace existing runtime wiring.");
        }

        if (AxamlBindingMarkup.ContainsBindingExpression(root))
        {
            throw new InvalidDataException(
                "The view already contains Binding expressions. Adding a generated root DataContext could redirect them; extend the existing ViewModel instead.");
        }

        List<GeneratedProperty> properties = [];
        List<GeneratedCommand> commands = [];
        HashSet<string> memberNames = new(StringComparer.Ordinal);
        foreach (XElement element in root.Descendants())
        {
            if (element.Name.Namespace != XNamespace.None
                && element.Name.Namespace != s_avaloniaNamespace)
            {
                continue;
            }

            string? controlName = (string?)element.Attribute(s_xamlNamespace + "Name");
            if (string.IsNullOrWhiteSpace(controlName))
            {
                continue;
            }

            if (!AxamlControlCatalog.TryGet(element.Name.LocalName, out AxamlControlCapability capability)
                || !capability.SupportsMvvmGeneration)
            {
                continue;
            }

            if (element.Name.LocalName == "AutoCompleteBox")
            {
                string? initialValue = (string?)element.Attribute("Text");
                if (initialValue?.StartsWith('{') == true)
                {
                    continue;
                }

                string propertyName = NormalizeIdentifier(TrimSuffix(controlName, "AutoCompleteBox"), "SearchText");
                if (!memberNames.Add(propertyName))
                {
                    throw new InvalidDataException($"More than one named control maps to '{propertyName}'.");
                }

                element.SetAttributeValue("Text", $"{{Binding {propertyName}, Mode=TwoWay}}");
                properties.Add(new GeneratedProperty(
                    propertyName,
                    "string?",
                    initialValue is null ? "null" : ToCSharpStringLiteral(initialValue)));
            }
            else if (element.Name.LocalName == "TextBox")
            {
                if (element.Attribute("Text") is not null)
                {
                    if (TryGetBindingPath((string?)element.Attribute("Text"), out _))
                    {
                        continue;
                    }

                    if (((string?)element.Attribute("Text"))?.StartsWith('{') == true)
                    {
                        continue;
                    }
                }

                string propertyName = NormalizeIdentifier(TrimSuffix(controlName, "TextBox"), "Input");
                if (!memberNames.Add(propertyName))
                {
                    throw new InvalidDataException($"More than one named control maps to '{propertyName}'.");
                }

                string initialValue = (string?)element.Attribute("Text") ?? string.Empty;
                element.SetAttributeValue(
                    "Text",
                    $"{{Binding {propertyName}, Mode=TwoWay}}");
                properties.Add(new GeneratedProperty(
                    propertyName,
                    "string",
                    ToCSharpStringLiteral(initialValue)));
            }
            else if (element.Name.LocalName == "TextBlock")
            {
                string? text = (string?)element.Attribute("Text");
                if (text is null || text.StartsWith('{'))
                {
                    continue;
                }

                string propertyName = NormalizeIdentifier(TrimSuffix(controlName, "TextBlock"), "Text");
                if (!memberNames.Add(propertyName))
                {
                    throw new InvalidDataException($"More than one named control maps to '{propertyName}'.");
                }

                element.SetAttributeValue("Text", $"{{Binding {propertyName}, Mode=OneWay}}");
                properties.Add(new GeneratedProperty(
                    propertyName,
                    "string",
                    ToCSharpStringLiteral(text)));
            }
            else if (element.Name.LocalName is "CheckBox" or "ToggleSwitch" or "RadioButton")
            {
                XAttribute? isCheckedAttribute = element.Attribute("IsChecked");
                string? initialValue = isCheckedAttribute?.Value;
                if (initialValue?.StartsWith('{') == true)
                {
                    continue;
                }

                if (initialValue is not null && !bool.TryParse(initialValue, out _))
                {
                    throw new InvalidDataException(
                        $"Named {element.Name.LocalName} '{controlName}' has an IsChecked value that is not a Boolean literal.");
                }

                string propertyName = NormalizeIdentifier(
                    TrimSuffix(controlName, element.Name.LocalName),
                    "Option");
                if (!memberNames.Add(propertyName))
                {
                    throw new InvalidDataException($"More than one named control maps to '{propertyName}'.");
                }

                element.SetAttributeValue(
                    "IsChecked",
                    $"{{Binding {propertyName}, Mode=TwoWay}}");
                properties.Add(new GeneratedProperty(
                    propertyName,
                    "bool?",
                    initialValue is null ? "null" : initialValue.ToLowerInvariant()));
            }
            else if (element.Name.LocalName == "NumericUpDown")
            {
                string? initialValue = (string?)element.Attribute("Value");
                if (initialValue?.StartsWith('{') == true)
                {
                    continue;
                }

                string initialLiteral = "null";
                if (initialValue is not null)
                {
                    if (!decimal.TryParse(
                        initialValue,
                        NumberStyles.AllowLeadingSign
                            | NumberStyles.AllowDecimalPoint
                            | NumberStyles.AllowLeadingWhite
                            | NumberStyles.AllowTrailingWhite,
                        CultureInfo.InvariantCulture,
                        out decimal number))
                    {
                        throw new InvalidDataException(
                            $"Named NumericUpDown '{controlName}' has a Value that is not an invariant decimal literal.");
                    }

                    initialLiteral = number.ToString(CultureInfo.InvariantCulture) + "m";
                }

                string propertyName = NormalizeIdentifier(TrimSuffix(controlName, "NumericUpDown"), "Value");
                if (!memberNames.Add(propertyName))
                {
                    throw new InvalidDataException($"More than one named control maps to '{propertyName}'.");
                }

                element.SetAttributeValue("Value", $"{{Binding {propertyName}, Mode=TwoWay}}");
                properties.Add(new GeneratedProperty(propertyName, "decimal?", initialLiteral));
            }
            else if (element.Name.LocalName == "Slider")
            {
                string? initialValue = (string?)element.Attribute("Value");
                if (initialValue?.StartsWith('{') == true)
                {
                    continue;
                }

                string initialLiteral = "0d";
                if (initialValue is not null)
                {
                    if (!double.TryParse(
                        initialValue,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out double number)
                        || !double.IsFinite(number))
                    {
                        throw new InvalidDataException(
                            $"Named Slider '{controlName}' has a Value that is not a finite invariant number.");
                    }

                    initialLiteral = number.ToString("R", CultureInfo.InvariantCulture) + "d";
                }

                string propertyName = NormalizeIdentifier(TrimSuffix(controlName, "Slider"), "Value");
                if (!memberNames.Add(propertyName))
                {
                    throw new InvalidDataException($"More than one named control maps to '{propertyName}'.");
                }

                element.SetAttributeValue("Value", $"{{Binding {propertyName}, Mode=TwoWay}}");
                properties.Add(new GeneratedProperty(propertyName, "double", initialLiteral));
            }
            else if (element.Name.LocalName is "ComboBox" or "TabControl")
            {
                string? initialValue = (string?)element.Attribute("SelectedIndex");
                if (initialValue?.StartsWith('{') == true)
                {
                    continue;
                }

                string initialLiteral = "-1";
                if (initialValue is not null)
                {
                    if (!int.TryParse(
                        initialValue,
                        NumberStyles.AllowLeadingSign
                            | NumberStyles.AllowLeadingWhite
                            | NumberStyles.AllowTrailingWhite,
                        CultureInfo.InvariantCulture,
                        out int selectedIndex)
                        || AxamlPropertyValueValidator.Validate(
                            element.Name.LocalName,
                            "SelectedIndex",
                            initialValue) is not null)
                    {
                        throw new InvalidDataException(
                            $"Named {element.Name.LocalName} '{controlName}' has an invalid SelectedIndex.");
                    }

                    initialLiteral = selectedIndex.ToString(CultureInfo.InvariantCulture);
                }

                string propertyName = NormalizeIdentifier(
                    TrimSuffix(controlName, element.Name.LocalName),
                    "Selection");
                if (!memberNames.Add(propertyName))
                {
                    throw new InvalidDataException($"More than one named control maps to '{propertyName}'.");
                }

                element.SetAttributeValue("SelectedIndex", $"{{Binding {propertyName}, Mode=TwoWay}}");
                properties.Add(new GeneratedProperty(propertyName, "int", initialLiteral));
            }
            else if (element.Name.LocalName is "DatePicker" or "CalendarDatePicker")
            {
                string controlType = element.Name.LocalName;
                string? initialValue = (string?)element.Attribute("SelectedDate");
                if (initialValue?.StartsWith('{') == true)
                {
                    continue;
                }

                string initialLiteral = "null";
                if (initialValue is not null)
                {
                    if (AxamlPropertyValueValidator.Validate(controlType, "SelectedDate", initialValue) is not null)
                    {
                        throw new InvalidDataException(
                            $"Named {controlType} '{controlName}' has a SelectedDate that is not a valid yyyy-MM-dd date.");
                    }

                    string dateType = controlType == "DatePicker" ? "DateTimeOffset" : "DateTime";
                    initialLiteral =
                        $"global::System.{dateType}.ParseExact({ToCSharpStringLiteral(initialValue)}, \"yyyy-MM-dd\", global::System.Globalization.CultureInfo.InvariantCulture, global::System.Globalization.DateTimeStyles.None)";
                }

                string propertyName = NormalizeIdentifier(TrimSuffix(controlName, controlType), "SelectedDate");
                if (!memberNames.Add(propertyName))
                {
                    throw new InvalidDataException($"More than one named control maps to '{propertyName}'.");
                }

                element.SetAttributeValue("SelectedDate", $"{{Binding {propertyName}, Mode=TwoWay}}");
                properties.Add(new GeneratedProperty(
                    propertyName,
                    controlType == "DatePicker" ? "global::System.DateTimeOffset?" : "global::System.DateTime?",
                    initialLiteral));
            }
            else if (element.Name.LocalName == "TimePicker")
            {
                string? initialValue = (string?)element.Attribute("SelectedTime");
                if (initialValue?.StartsWith('{') == true)
                {
                    continue;
                }

                string initialLiteral = "null";
                if (initialValue is not null)
                {
                    if (AxamlPropertyValueValidator.Validate("TimePicker", "SelectedTime", initialValue) is not null)
                    {
                        throw new InvalidDataException(
                            $"Named TimePicker '{controlName}' has a SelectedTime that is not a valid HH:mm or HH:mm:ss time.");
                    }

                    initialLiteral =
                        $"global::System.TimeSpan.Parse({ToCSharpStringLiteral(initialValue)}, global::System.Globalization.CultureInfo.InvariantCulture)";
                }

                string propertyName = NormalizeIdentifier(TrimSuffix(controlName, "TimePicker"), "SelectedTime");
                if (!memberNames.Add(propertyName))
                {
                    throw new InvalidDataException($"More than one named control maps to '{propertyName}'.");
                }

                element.SetAttributeValue("SelectedTime", $"{{Binding {propertyName}, Mode=TwoWay}}");
                properties.Add(new GeneratedProperty(propertyName, "global::System.TimeSpan?", initialLiteral));
            }
            else if (element.Name.LocalName == "Calendar")
            {
                string? initialValue = (string?)element.Attribute("SelectedDate");
                if (initialValue?.StartsWith('{') == true)
                {
                    continue;
                }

                string initialLiteral = "null";
                if (initialValue is not null)
                {
                    if (AxamlPropertyValueValidator.Validate("Calendar", "SelectedDate", initialValue) is not null)
                    {
                        throw new InvalidDataException(
                            $"Named Calendar '{controlName}' has a SelectedDate that is not a valid yyyy-MM-dd date.");
                    }

                    initialLiteral =
                        $"global::System.DateTime.ParseExact({ToCSharpStringLiteral(initialValue)}, \"yyyy-MM-dd\", global::System.Globalization.CultureInfo.InvariantCulture, global::System.Globalization.DateTimeStyles.None)";
                }

                string propertyName = NormalizeIdentifier(TrimSuffix(controlName, "Calendar"), "SelectedDate");
                if (!memberNames.Add(propertyName))
                {
                    throw new InvalidDataException($"More than one named control maps to '{propertyName}'.");
                }

                element.SetAttributeValue("SelectedDate", $"{{Binding {propertyName}, Mode=TwoWay}}");
                properties.Add(new GeneratedProperty(propertyName, "global::System.DateTime?", initialLiteral));
            }
            else if (element.Name.LocalName == "ColorPicker")
            {
                string? initialValue = (string?)element.Attribute("Color");
                if (initialValue?.StartsWith('{') == true)
                {
                    continue;
                }

                if (initialValue is not null
                    && AxamlPropertyValueValidator.Validate("ColorPicker", "Color", initialValue) is not null)
                {
                    throw new InvalidDataException(
                        $"Named ColorPicker '{controlName}' has a Color that is not a valid color literal.");
                }

                string initialLiteral =
                    $"global::Avalonia.Media.Color.Parse({ToCSharpStringLiteral(initialValue ?? "White")})";
                string propertyName = NormalizeIdentifier(TrimSuffix(controlName, "ColorPicker"), "Color");
                if (!memberNames.Add(propertyName))
                {
                    throw new InvalidDataException($"Generated color '{propertyName}' conflicts with another ViewModel member.");
                }

                element.SetAttributeValue("Color", $"{{Binding {propertyName}, Mode=TwoWay}}");
                properties.Add(new GeneratedProperty(propertyName, "global::Avalonia.Media.Color", initialLiteral));
            }

            if (capability.BindableProperties.Any(
                    static property => property.Name == "Command" && property.IsCommand)
                && element.Attribute("Command") is null)
            {
                string? commandParameter = (string?)element.Attribute("CommandParameter");
                if (commandParameter is not null
                    && AxamlPropertyValueValidator.Validate(
                        element.Name.LocalName,
                        "CommandParameter",
                        commandParameter) is not null)
                {
                    continue;
                }

                string baseCommandName = NormalizeIdentifier(
                    TrimSuffix(controlName, element.Name.LocalName),
                    "Action");
                bool isToggleControl = element.Name.LocalName is "CheckBox" or "RadioButton" or "ToggleSwitch";
                string commandName = isToggleControl && memberNames.Contains(baseCommandName)
                    ? NormalizeIdentifier(controlName, "Action")
                    : baseCommandName;
                string commandPropertyName = commandName + "Command";
                if (!memberNames.Add(commandName) || !memberNames.Add(commandPropertyName))
                {
                    throw new InvalidDataException($"Generated command '{commandName}' conflicts with another ViewModel member.");
                }

                element.SetAttributeValue("Command", $"{{Binding {commandPropertyName}}}");
                commands.Add(new GeneratedCommand(commandName, commandParameter is not null));
            }
        }

        if (properties.Count == 0 && commands.Count == 0)
        {
            throw new InvalidDataException(
                "No unbound, explicitly named supported controls were found to generate.");
        }

        string prefix = GetOrCreateViewModelPrefix(root, viewModelNamespace);
        if (dataType is null)
        {
            root.SetAttributeValue(s_xamlNamespace + "DataType", $"{prefix}:{viewModelName}");
        }

        AddDataContext(root, viewModelName, viewModelNamespace);
        string generatedMarkup = document.ToString(SaveOptions.DisableFormatting);
        string fileName = Path.GetFileNameWithoutExtension(axamlRelativePath) + "ViewModel.cs";
        string directory = Path.GetDirectoryName(axamlRelativePath) ?? string.Empty;
        string viewModelPath = string.IsNullOrEmpty(directory)
            ? fileName
            : Path.Combine(directory, fileName);

        return
        [
            WorkspaceFileChangeRequest.ReplaceExisting(axamlRelativePath, markup, generatedMarkup),
            WorkspaceFileChangeRequest.CreateNew(
                viewModelPath,
                GenerateViewModel(viewModelNamespace, viewModelName, properties, commands, usePartialProperties)),
        ];
    }

    private static XDocument ParseMarkup(string markup)
    {
        XmlReaderSettings settings = new()
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            MaxCharactersInDocument = MaximumMarkupLength,
        };
        using StringReader input = new(markup);
        using XmlReader reader = XmlReader.Create(input, settings);
        return XDocument.Load(reader, LoadOptions.PreserveWhitespace);
    }

    private static void AddDataContext(XElement root, string viewModelName, string viewModelNamespace)
    {
        XNamespace typeNamespace = $"using:{viewModelNamespace}";
        XElement dataContext = new(
            root.Name.Namespace + $"{root.Name.LocalName}.DataContext",
            new XElement(typeNamespace + viewModelName));
        XElement? firstElement = root.Elements().FirstOrDefault();
        if (firstElement is null)
        {
            root.Add(dataContext);
        }
        else
        {
            XNode? precedingWhitespace = firstElement.PreviousNode;
            firstElement.AddBeforeSelf(dataContext);
            if (precedingWhitespace is XText whitespace)
            {
                dataContext.AddAfterSelf(new XText(whitespace.Value));
            }
        }
    }

    private static bool IsGeneratedViewModelDataType(
        XElement root,
        string value,
        string viewModelNamespace,
        string viewModelName)
    {
        int separator = value.IndexOf(':');
        if (separator <= 0
            || separator == value.Length - 1
            || value.IndexOf(':', separator + 1) >= 0
            || !string.Equals(value[(separator + 1)..], viewModelName, StringComparison.Ordinal))
        {
            return false;
        }

        string prefix = value[..separator];
        return string.Equals(
            root.GetNamespaceOfPrefix(prefix)?.NamespaceName,
            $"using:{viewModelNamespace}",
            StringComparison.Ordinal);
    }

    private static string GetOrCreateViewModelPrefix(XElement root, string viewModelNamespace)
    {
        XNamespace targetNamespace = $"using:{viewModelNamespace}";
        foreach (XAttribute attribute in root.Attributes().Where(static attribute => attribute.IsNamespaceDeclaration))
        {
            if (attribute.Value == targetNamespace.NamespaceName)
            {
                return attribute.Name.LocalName == "xmlns" ? string.Empty : attribute.Name.LocalName;
            }
        }

        string prefix = "vm";
        int suffix = 1;
        while (root.GetNamespaceOfPrefix(prefix) is not null)
        {
            prefix = $"vm{suffix++}";
        }

        root.SetAttributeValue(XNamespace.Xmlns + prefix, targetNamespace.NamespaceName);
        return prefix;
    }

    private static string GenerateViewModel(
        string viewModelNamespace,
        string viewModelName,
        IReadOnlyList<GeneratedProperty> properties,
        IReadOnlyList<GeneratedCommand> commands,
        bool usePartialProperties)
    {
        StringBuilder source = new();
        source.AppendLine("using CommunityToolkit.Mvvm.ComponentModel;");
        source.AppendLine("using CommunityToolkit.Mvvm.Input;");
        source.AppendLine();
        source.Append("namespace ").Append(viewModelNamespace).AppendLine(";");
        source.AppendLine();
        source.Append("public partial class ").Append(viewModelName).AppendLine(" : ObservableObject");
        source.AppendLine("{");
        foreach (GeneratedProperty property in properties)
        {
            source.AppendLine("    [ObservableProperty]");
            if (usePartialProperties)
            {
                source.Append("    public partial ").Append(property.Type).Append(' ').Append(property.Name)
                    .Append(" { get; set; } = ").Append(property.InitialValue).AppendLine(";");
            }
            else
            {
                string fieldName = "_" + char.ToLowerInvariant(property.Name[0]) + property.Name[1..];
                source.Append("    private ").Append(property.Type).Append(' ').Append(fieldName).Append(" = ")
                    .Append(property.InitialValue).AppendLine(";");
            }
            source.AppendLine();
        }

        foreach (GeneratedCommand command in commands)
        {
            source.AppendLine("    [RelayCommand]");
            source.Append("    private void ").Append(command.Name);
            source.AppendLine(command.HasStringParameter ? "(string? commandParameter)" : "()");
            source.AppendLine("    {");
            source.Append("        // TODO: Implement the ").Append(command.Name).AppendLine(" action.");
            source.AppendLine("    }");
            source.AppendLine();
        }

        source.AppendLine("}");
        return source.ToString();
    }

    private static bool TryGetBindingPath(string? value, out string path)
    {
        path = string.Empty;
        if (value is null || !AxamlBindingMarkup.IsBindingExpression(value))
        {
            return false;
        }

        string expression = value.Trim();
        int end = expression.EndsWith('}') ? expression.Length - 1 : expression.Length;
        string body = expression[1..end].Trim()["Binding".Length..].Trim();
        int separator = body.IndexOf(',');
        path = (separator < 0 ? body : body[..separator]).Trim();
        if (path.StartsWith("Path=", StringComparison.Ordinal))
        {
            path = path["Path=".Length..].Trim();
        }

        return path.Length > 0;
    }

    private static string GetNamespace(string className)
    {
        int separator = className.LastIndexOf('.');
        return separator < 0 ? "Generated" : className[..separator];
    }

    private static void ValidateQualifiedIdentifier(string value)
    {
        string[] segments = value.Split('.');
        if (segments.Any(static segment => segment.Length == 0
            || !(char.IsLetter(segment[0]) || segment[0] == '_')
            || segment.Skip(1).Any(static character => !char.IsLetterOrDigit(character) && character != '_')))
        {
            throw new InvalidDataException($"AXAML x:Class '{value}' is not a valid C# type name.");
        }
    }

    private static string TrimSuffix(string name, string suffix)
    {
        return name.EndsWith(suffix, StringComparison.Ordinal)
            ? name[..^suffix.Length]
            : name;
    }

    private static string NormalizeIdentifier(string value, string fallback)
    {
        StringBuilder identifier = new(value.Length + 1);
        foreach (char character in value)
        {
            identifier.Append(char.IsLetterOrDigit(character) || character == '_' ? character : '_');
        }

        if (identifier.Length == 0)
        {
            identifier.Append(fallback);
        }

        if (char.IsDigit(identifier[0]))
        {
            identifier.Insert(0, '_');
        }

        if (IsCSharpKeyword(identifier.ToString()))
        {
            identifier.Insert(0, '_');
        }

        identifier[0] = char.ToUpperInvariant(identifier[0]);
        return identifier.ToString();
    }

    private static bool IsCSharpKeyword(string value)
    {
        return value is "abstract" or "as" or "base" or "bool" or "break" or "byte" or "case"
            or "catch" or "char" or "checked" or "class" or "const" or "continue" or "decimal"
            or "default" or "delegate" or "do" or "double" or "else" or "enum" or "event"
            or "explicit" or "extern" or "false" or "finally" or "fixed" or "float" or "for"
            or "foreach" or "goto" or "if" or "implicit" or "in" or "int" or "interface"
            or "internal" or "is" or "lock" or "long" or "namespace" or "new" or "null"
            or "object" or "operator" or "out" or "override" or "params" or "private"
            or "protected" or "public" or "readonly" or "ref" or "return" or "sbyte" or "sealed"
            or "short" or "sizeof" or "stackalloc" or "static" or "string" or "struct" or "switch"
            or "this" or "throw" or "true" or "try" or "typeof" or "uint" or "ulong" or "unchecked"
            or "unsafe" or "ushort" or "using" or "virtual" or "void" or "volatile" or "while";
    }

    private static string ToCSharpStringLiteral(string value)
    {
        return System.Text.Json.JsonSerializer.Serialize(value);
    }

    private sealed record GeneratedProperty(string Name, string Type, string InitialValue);

    private sealed record GeneratedCommand(string Name, bool HasStringParameter);
}
