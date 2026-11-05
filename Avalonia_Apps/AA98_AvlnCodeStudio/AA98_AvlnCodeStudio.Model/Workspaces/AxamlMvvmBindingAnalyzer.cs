using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

public sealed class AxamlMvvmBindingAnalyzer
{
    private static readonly XNamespace s_avaloniaNamespace = "https://github.com/avaloniaui";
    private const int MaximumMarkupLength = 1_000_000;

    public IReadOnlyList<AxamlMvvmMemberRequirement> Analyze(string markup)
    {
        ArgumentNullException.ThrowIfNull(markup);
        if (markup.Length > MaximumMarkupLength)
        {
            throw new InvalidDataException("AXAML markup exceeds the MVVM binding analysis size limit.");
        }

        XDocument document = ParseMarkup(markup);
        XElement root = document.Root
            ?? throw new InvalidDataException("AXAML must have a root control.");
        List<AxamlMvvmMemberRequirement> requirements = [];
        foreach (XElement element in root.DescendantsAndSelf())
        {
            if (element.Name.Namespace != s_avaloniaNamespace)
            {
                continue;
            }

            string controlName = element.Name.LocalName;
            if (!AxamlControlCatalog.TryGet(controlName, out AxamlControlCapability capability))
            {
                continue;
            }

            foreach (AxamlBindablePropertyCapability bindableProperty in capability.BindableProperties)
            {
                XAttribute? attribute = element.Attribute(bindableProperty.Name);
                if (attribute is null || !TryReadBindingPath(attribute.Value, out string bindingPath))
                {
                    continue;
                }

                if (bindingPath.Contains('.')
                    || bindingPath.Contains('[')
                    || bindingPath.Contains(']')
                    || bindingPath.Contains('(')
                    || bindingPath.Contains(')')
                    || bindingPath.Contains('/')
                    || bindingPath.Contains('?'))
                {
                    throw new InvalidDataException(
                        $"Binding '{bindingPath}' on {controlName}.{bindableProperty.Name} is not a simple ViewModel member and cannot be planned safely.");
                }

                requirements.Add(new AxamlMvvmMemberRequirement(
                    bindingPath,
                    bindableProperty.TypeName,
                    bindableProperty.IsCommand));
            }
        }

        return requirements;
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

    private static bool TryReadBindingPath(string value, out string path)
    {
        path = string.Empty;
        ReadOnlySpan<char> expression = value.AsSpan().Trim();
        if (expression.Length < 2 || expression[0] != '{')
        {
            return false;
        }

        ReadOnlySpan<char> body = expression[1..].TrimStart();
        if (!body.StartsWith("Binding", StringComparison.Ordinal)
            || body.Length > "Binding".Length
                && !char.IsWhiteSpace(body["Binding".Length])
                && body["Binding".Length] != ',')
        {
            return false;
        }
        if (expression[^1] != '}')
        {
            throw new InvalidDataException("The binding expression is missing its closing brace.");
        }

        expression = expression[1..^1].Trim();
        expression = expression["Binding".Length..].Trim();
        if (expression.Length == 0)
        {
            throw new InvalidDataException("A binding used by MVVM planning must specify a member path.");
        }

        int comma = expression.IndexOf(',');
        ReadOnlySpan<char> pathToken = (comma < 0 ? expression : expression[..comma]).Trim();
        if (pathToken.StartsWith("Path=", StringComparison.Ordinal))
        {
            pathToken = pathToken["Path=".Length..].Trim();
        }

        path = pathToken.ToString();
        if (!AxamlMvvmMemberName.IsValid(path))
        {
            throw new InvalidDataException("The binding path is malformed or not a simple member name.");
        }

        return true;
    }

}
