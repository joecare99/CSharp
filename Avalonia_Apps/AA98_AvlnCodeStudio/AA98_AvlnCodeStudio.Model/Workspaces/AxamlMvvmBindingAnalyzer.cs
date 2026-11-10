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
            if (element != root && HasBindingScopeBoundaryAncestor(element, root))
            {
                continue;
            }

            if (element != root && IsBindingScopeBoundary(element))
            {
                if (AxamlBindingMarkup.ContainsBindingExpression(element))
                {
                    throw new InvalidDataException(
                        $"Bindings inside '{element.Name.LocalName}' or its nested DataContext use a different or unresolved source and cannot be planned safely.");
                }

                continue;
            }

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

                if (AxamlBindingMarkup.HasExplicitSource(attribute.Value))
                {
                    throw new InvalidDataException(
                        $"Binding on {controlName}.{bindableProperty.Name} has an explicit source and cannot be planned as a ViewModel member.");
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

    private static bool HasBindingScopeBoundaryAncestor(XElement element, XElement root)
    {
        return element.Ancestors()
            .TakeWhile(ancestor => ancestor != root)
            .Any(IsBindingScopeBoundary);
    }

    private static bool IsBindingScopeBoundary(XElement element)
    {
        if (element.Name.LocalName is "DataTemplate"
            or "TreeDataTemplate"
            or "HierarchicalDataTemplate"
            or "ItemsPanelTemplate"
            or "ControlTemplate")
        {
            return true;
        }

        return element.Name.LocalName.EndsWith(".DataContext", StringComparison.Ordinal)
            || element.Attributes().Any(static attribute =>
                attribute.Name.LocalName == "DataContext"
                || attribute.Name.LocalName.EndsWith(".DataContext", StringComparison.Ordinal));
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

internal static class AxamlBindingMarkup
{
    public static bool ContainsBindingExpression(XElement element)
    {
        return element.DescendantsAndSelf()
            .SelectMany(static descendant => descendant.Attributes())
            .Any(static attribute => !attribute.IsNamespaceDeclaration && IsBindingExpression(attribute.Value))
            || element.DescendantNodes().OfType<XText>()
                .Any(static text => IsBindingExpression(text.Value));
    }

    public static bool IsBindingExpression(string value)
    {
        ReadOnlySpan<char> expression = value.AsSpan().Trim();
        if (expression.Length < 2 || expression[0] != '{')
        {
            return false;
        }

        ReadOnlySpan<char> body = expression[1..].TrimStart();
        return body.StartsWith("Binding", StringComparison.Ordinal)
            && (body.Length == "Binding".Length
                || char.IsWhiteSpace(body["Binding".Length])
                || body["Binding".Length] is ',' or '}');
    }

    public static bool HasExplicitSource(string value)
    {
        if (!IsBindingExpression(value))
        {
            return false;
        }

        string expression = value.Trim();
        int end = expression.EndsWith('}') ? expression.Length - 1 : expression.Length;
        string body = expression[1..end].Trim();
        body = body["Binding".Length..].Trim();
        string[] options = body.Split(',');
        return options.Skip(1).Any(static option =>
        {
            int equals = option.IndexOf('=');
            if (equals < 0)
            {
                return false;
            }

            string name = option[..equals].Trim();
            return string.Equals(name, "Source", StringComparison.OrdinalIgnoreCase)
                || string.Equals(name, "ElementName", StringComparison.OrdinalIgnoreCase)
                || string.Equals(name, "RelativeSource", StringComparison.OrdinalIgnoreCase);
        });
    }
}
