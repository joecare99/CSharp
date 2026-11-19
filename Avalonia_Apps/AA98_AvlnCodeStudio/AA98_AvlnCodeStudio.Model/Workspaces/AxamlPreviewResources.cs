using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>Reads only bounded literal data. It never loads a project assembly or XAML runtime.</summary>
internal sealed class AxamlPreviewResources(string? documentPath, string? projectRoot)
{
    private static readonly XNamespace s_avalonia = "https://github.com/avaloniaui";
    private static readonly XNamespace s_xaml = "http://schemas.microsoft.com/winfx/2006/xaml";
    private static readonly Regex s_identifier = new(@"^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.CultureInvariant);
    private static readonly HashSet<string> s_styleProperties = new(StringComparer.Ordinal)
    {
        "Margin", "Padding", "FontSize", "Foreground", "Background", "BorderBrush",
        "BorderThickness", "CornerRadius", "HorizontalAlignment", "VerticalAlignment",
        "HorizontalContentAlignment", "VerticalContentAlignment", "Width", "Height",
    };
    private readonly Dictionary<string, Dictionary<string, string>> _resxCache = new(StringComparer.OrdinalIgnoreCase);
    private int _fileReads;
    private bool _applicationLoaded;
    private XElement? _application;
    private readonly List<XElement> _applicationStyles = [];

    public Dictionary<string, string> GetPreviewProperties(XElement element, Dictionary<string, string> source,
        out AxamlPreviewLinearGradient? buttonBackgroundGradient)
    {
        Dictionary<string, string> result = new(StringComparer.Ordinal);
        buttonBackgroundGradient = null;
        LoadApplication();
        foreach (XElement style in _applicationStyles)
        {
            ApplyStyle(element, style, result, applicationStyle: true, ref buttonBackgroundGradient);
        }

        foreach (XElement owner in element.AncestorsAndSelf().Reverse())
        {
            foreach (XElement styles in owner.Elements().Where(e => IsAvalonia(e, owner.Name.LocalName + ".Styles")))
            {
                foreach (XElement style in styles.Elements().Take(256).Where(e => IsAvalonia(e, "Style")))
                {
                    ApplyStyle(element, style, result, applicationStyle: false, ref buttonBackgroundGradient);
                }
            }
        }

        foreach ((string name, string value) in source)
        {
            if (name == "Background")
            {
                buttonBackgroundGradient = null;
            }

            // An unresolved local value must still override a style.
            result[name] = TryLiteral(element, value, out string? literal) ? literal! : value;
        }

        if (element.Elements().Any(e => IsAvalonia(e, element.Name.LocalName + ".Background")))
        {
            buttonBackgroundGradient = null;
        }

        return result;
    }

    private void ApplyStyle(XElement element, XElement style, Dictionary<string, string> result, bool applicationStyle,
        ref AxamlPreviewLinearGradient? buttonBackgroundGradient)
    {
        if (!MatchesSelector(element, (string?)style.Attribute("Selector")))
        {
            return;
        }

        foreach (XElement setter in style.Elements().Take(256).Where(e => IsAvalonia(e, "Setter")))
        {
            string? name = (string?)setter.Attribute("Property");
            string? value = (string?)setter.Attribute("Value");
            if (name == "Background")
            {
                buttonBackgroundGradient = null;
            }

            if (name is null || value is null || !s_styleProperties.Contains(name)
                || setter.HasElements || setter.Attributes().Any(a => !a.IsNamespaceDeclaration
                    && a.Name != "Property" && a.Name != "Value"))
            {
                continue;
            }

            if (applicationStyle && element.Name.LocalName == "Button" && name == "Background"
                && _application is not null && TryResourceReference(value, out string? gradientKey)
                && !HasGradientResourceConflict(element, gradientKey!))
            {
                AxamlPreviewLinearGradient? gradient = TryBorderGradient(_application, value);
                if (gradient is not null)
                {
                    result.Remove(name);
                    buttonBackgroundGradient = gradient;
                    continue;
                }
            }

            string? literal = null;
            bool resolved = applicationStyle && value.TrimStart().StartsWith('{')
                ? TryResourceReference(value, out string? key) && _application is not null
                    && TryLocalResource(_application, key!, new HashSet<string>(StringComparer.Ordinal), out literal, element)
                : TryLiteral(element, value, out literal);
            if (resolved
                && AxamlPropertyValueValidator.Validate(element.Name.LocalName, name, literal!) is null)
            {
                result[name] = literal!;
            }

        }
    }

    private static bool HasGradientResourceConflict(XElement element, string key) =>
        element.AncestorsAndSelf().Any(owner =>
        {
            XElement[] entries = owner.Elements()
                .Where(e => IsAvalonia(e, owner.Name.LocalName + ".Resources"))
                .SelectMany(e => e.Elements().Take(513)).Take(513).ToArray();
            return entries.Length > 512 || entries.Any(e => IsAvalonia(e, "ResourceDictionary")
                || (string?)e.Attribute(s_xaml + "Key") == key);
        });

    public bool TryResolve(XElement element, string value, out string? result)
    {
        result = null;
        if (TryResourceReference(value, out string? key))
        {
            return TryLocalResource(element, key!, new HashSet<string>(StringComparer.Ordinal), out result);
        }

        Match match = Regex.Match(value, @"^\{x:Static\s+([A-Za-z_][A-Za-z0-9_]*):([A-Za-z_][A-Za-z0-9_]*)\.([A-Za-z_][A-Za-z0-9_]*)\}$",
            RegexOptions.CultureInvariant);
        if (!match.Success || element.GetNamespaceOfPrefix("x") != s_xaml)
        {
            return false;
        }

        XNamespace? ns = element.GetNamespaceOfPrefix(match.Groups[1].Value);
        string nsText = ns?.NamespaceName ?? string.Empty;
        string prefix = nsText.StartsWith("clr-namespace:", StringComparison.Ordinal) ? "clr-namespace:"
            : nsText.StartsWith("using:", StringComparison.Ordinal) ? "using:" : string.Empty;
        if (prefix.Length == 0)
        {
            return false;
        }

        return TryResx(nsText[prefix.Length..], match.Groups[2].Value, match.Groups[3].Value, out result);
    }

    public AxamlPreviewLinearGradient? TryBorderGradient(XElement element, string value)
    {
        if (!TryResourceReference(value, out string? key))
        {
            return null;
        }

        LoadApplication();
        IEnumerable<XElement> owners = element.AncestorsAndSelf();
        if (_application is not null && !owners.Contains(_application))
        {
            owners = owners.Append(_application);
        }

        foreach (XElement owner in owners)
        {
            XElement[] entries = owner.Elements()
                .Where(e => IsAvalonia(e, owner.Name.LocalName + ".Resources"))
                .SelectMany(e => e.Elements().Take(513)).ToArray();
            if (entries.Length > 512 || entries.Any(e => IsAvalonia(e, "ResourceDictionary")))
            {
                return null;
            }

            XElement[] matches = entries.Where(e => (string?)e.Attribute(s_xaml + "Key") == key).Take(2).ToArray();
            if (matches.Length != 0)
            {
                return matches.Length == 1 ? ParseGradient(matches[0]) : null;
            }
        }

        return null;
    }

    private static AxamlPreviewLinearGradient? ParseGradient(XElement brush)
    {
        if (!IsAvalonia(brush, "LinearGradientBrush")
            || !OnlyAttributes(brush, s_xaml + "Key", "StartPoint", "EndPoint")
            || !TryPoint((string?)brush.Attribute("StartPoint"), out double startX, out double startY)
            || !TryPoint((string?)brush.Attribute("EndPoint"), out double endX, out double endY)
            || HasUnsupportedNodes(brush))
        {
            return null;
        }

        XElement[] containers = brush.Elements().ToArray();
        if (containers.Length != 1 || !IsAvalonia(containers[0], "LinearGradientBrush.GradientStops")
            || !OnlyAttributes(containers[0]) || HasUnsupportedNodes(containers[0]))
        {
            return null;
        }

        XElement[] stops = containers[0].Elements().Take(33).ToArray();
        if (stops.Length is < 2 or > 32)
        {
            return null;
        }

        List<AxamlPreviewGradientStop> result = [];
        foreach (XElement stop in stops)
        {
            string? color = (string?)stop.Attribute("Color");
            if (!IsAvalonia(stop, "GradientStop") || !OnlyAttributes(stop, "Offset", "Color")
                || stop.HasElements || HasUnsupportedNodes(stop)
                || !double.TryParse((string?)stop.Attribute("Offset"), NumberStyles.Float, CultureInfo.InvariantCulture, out double offset)
                || !double.IsFinite(offset) || offset is < 0 or > 1
                || color is null || !Regex.IsMatch(color, @"^#(?:[0-9a-fA-F]{6}|[0-9a-fA-F]{8})$", RegexOptions.CultureInvariant))
            {
                return null;
            }

            result.Add(new AxamlPreviewGradientStop(offset, color));
        }

        return new AxamlPreviewLinearGradient(startX, startY, endX, endY, result);
    }

    private static bool OnlyAttributes(XElement element, params XName[] names) =>
        element.Attributes().All(a => a.IsNamespaceDeclaration || names.Contains(a.Name));

    private static bool HasUnsupportedNodes(XElement element) =>
        element.Nodes().Any(node => node is not XElement
            && (node is not XText text || !string.IsNullOrWhiteSpace(text.Value)));

    private static bool TryPoint(string? value, out double x, out double y)
    {
        x = y = 0;
        string[] parts = value?.Split(',', StringSplitOptions.TrimEntries) ?? [];
        return value is { Length: <= 64 } && parts.Length == 2
            && double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out x)
            && double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out y)
            && double.IsFinite(x) && double.IsFinite(y)
            && x is >= -10000 and <= 10000 && y is >= -10000 and <= 10000;
    }

    private bool TryLiteral(XElement element, string value, out string? result)
    {
        if (value.TrimStart().StartsWith('{'))
        {
            return TryResolve(element, value, out result);
        }

        result = value.Length <= 4096 ? value : null;
        return result is not null;
    }

    private bool TryLocalResource(XElement element, string key, HashSet<string> visited, out string? result, XElement? shadowGuard = null)
    {
        result = null;
        if (visited.Count >= 32 || !visited.Add(key))
        {
            return false;
        }

        if (shadowGuard is not null && HasLocalResourceConflict(shadowGuard, key))
        {
            return false;
        }

        LoadApplication();
        IEnumerable<XElement> owners = element.AncestorsAndSelf();
        if (_application is not null && !element.AncestorsAndSelf().Contains(_application))
        {
            owners = owners.Append(_application);
        }

        foreach (XElement owner in owners)
        {
            XElement[] matches = owner.Elements()
                .Where(e => IsAvalonia(e, owner.Name.LocalName + ".Resources"))
                .SelectMany(e => e.Elements().Take(512))
                .Where(e => (string?)e.Attribute(s_xaml + "Key") == key).Take(2).ToArray();
            if (matches.Length == 0)
            {
                continue;
            }

            if (matches.Length != 1)
            {
                return false;
            }

            XElement resource = matches[0];
            string? value = resource.Name == s_xaml + "String" && !resource.HasElements
                ? resource.Value
                : IsAvalonia(resource, "SolidColorBrush") && !resource.HasElements
                    ? (string?)resource.Attribute("Color")
                    : null;
            if (value is null || value.Length > 4096)
            {
                return false;
            }

            if (TryResourceReference(value, out string? next))
            {
                return TryLocalResource(resource, next!, visited, out result, shadowGuard);
            }

            if (value.TrimStart().StartsWith('{'))
            {
                return false;
            }

            result = value;
            return true;
        }

        return false;
    }

    private static bool HasLocalResourceConflict(XElement element, string key) =>
        element.AncestorsAndSelf().Any(owner => owner.Elements()
            .Where(e => IsAvalonia(e, owner.Name.LocalName + ".Resources"))
            .SelectMany(e => e.Elements().Take(512))
            .Any(e => (string?)e.Attribute(s_xaml + "Key") == key || IsAvalonia(e, "ResourceDictionary")));

    private void LoadApplication()
    {
        if (_applicationLoaded)
        {
            return;
        }

        _applicationLoaded = true;
        try
        {
            if (documentPath is null || projectRoot is null || !Path.IsPathFullyQualified(documentPath)
                || !Path.IsPathFullyQualified(projectRoot))
            {
                return;
            }

            string root = Path.GetFullPath(projectRoot).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string document = Path.GetFullPath(documentPath);
            if (IsNetworkPath(root) || !IsUnderRoot(root, document) || HasReparsePoint(root, document))
            {
                return;
            }

            string[] projects = Directory.EnumerateFiles(root, "*.csproj", SearchOption.TopDirectoryOnly).Take(2).ToArray();
            if (projects.Length != 1 || HasReparsePoint(root, projects[0]))
            {
                return;
            }

            string appPath = Path.Combine(root, "App.axaml");
            if (HasReparsePoint(root, appPath) || ReadSafeAxaml(appPath) is not XElement app
                || !IsAvalonia(app, "Application"))
            {
                return;
            }

            _application = app;
            XElement[] includes = app.Elements().Where(e => IsAvalonia(e, "Application.Styles"))
                .SelectMany(e => e.Elements().Take(256)).Where(e => IsAvalonia(e, "StyleInclude"))
                .Take(17).ToArray();
            if (includes.Length > 16)
            {
                return;
            }

            StringComparer pathComparer = OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
            var sources = includes.Select(e => (string?)e.Attribute("Source")).ToArray();
            foreach (XElement include in includes)
            {
                string? source = (string?)include.Attribute("Source");
                if (source is null || source.Length > 256 || sources.Count(s => pathComparer.Equals(s, source)) != 1
                    || include.HasElements || include.Attributes().Any(a => !a.IsNamespaceDeclaration && a.Name != "Source")
                    || !Regex.IsMatch(source, @"^[A-Za-z0-9_-]+(?:/[A-Za-z0-9_-]+)*\.axaml$", RegexOptions.CultureInvariant))
                {
                    continue;
                }

                string path = Path.GetFullPath(Path.Combine(root, source.Replace('/', Path.DirectorySeparatorChar)));
                if (!IsUnderRoot(root, path) || HasReparsePoint(root, path)
                    || ReadSafeAxaml(path) is not XElement styles || !IsAvalonia(styles, "Styles"))
                {
                    continue;
                }

                _applicationStyles.AddRange(styles.Elements().Take(256).Where(e => IsAvalonia(e, "Style"))
                    .Take(256 - _applicationStyles.Count));
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
            _application = null;
            _applicationStyles.Clear();
        }
    }

    private static XElement? ReadSafeAxaml(string path)
    {
        if (!File.Exists(path) || new FileInfo(path).Length > 1_000_000)
        {
            return null;
        }

        try
        {
            XmlReaderSettings settings = new() { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null, MaxCharactersInDocument = 1_000_000 };
            using XmlReader reader = XmlReader.Create(path, settings);
            return XDocument.Load(reader).Root;
        }
        catch (XmlException)
        {
            return null;
        }
    }

    private static bool TryResourceReference(string value, out string? key)
    {
        Match match = Regex.Match(value, @"^\{(?:StaticResource|DynamicResource)\s+(?:ResourceKey=)?([A-Za-z_][A-Za-z0-9_.-]*)\}$",
            RegexOptions.CultureInvariant);
        key = match.Success ? match.Groups[1].Value : null;
        return match.Success;
    }

    private static bool MatchesSelector(XElement element, string? selector)
    {
        if (selector is null || element.Name.Namespace != s_avalonia && element.Name.Namespace != XNamespace.None)
        {
            return false;
        }

        string[] parts = selector.Split('.');
        if (parts.Length is < 1 or > 2 || parts.Any(p => !s_identifier.IsMatch(p))
            || parts[0] != element.Name.LocalName)
        {
            return false;
        }

        return parts.Length == 1 || ((string?)element.Attribute("Classes") ?? string.Empty)
            .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Contains(parts[1], StringComparer.Ordinal);
    }

    private bool TryResx(string resourceNamespace, string type, string key, out string? result)
    {
        result = null;
        try
        {
            if (documentPath is null || projectRoot is null || !Path.IsPathFullyQualified(documentPath)
                || !Path.IsPathFullyQualified(projectRoot))
            {
                return false;
            }

            string root = Path.GetFullPath(projectRoot).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string document = Path.GetFullPath(documentPath);
            if (IsNetworkPath(root)
                || !IsUnderRoot(root, document) || HasReparsePoint(root, document))
            {
                return false;
            }

            string[] projects = Directory.EnumerateFiles(root, "*.csproj", SearchOption.TopDirectoryOnly).Take(2).ToArray();
            if (projects.Length != 1 || HasReparsePoint(root, projects[0]))
            {
                return false;
            }

            string rootNamespace = Path.GetFileNameWithoutExtension(projects[0]);
            if (resourceNamespace != rootNamespace && !resourceNamespace.StartsWith(rootNamespace + ".", StringComparison.Ordinal))
            {
                return false;
            }

            string suffix = resourceNamespace == rootNamespace ? string.Empty : resourceNamespace[(rootNamespace.Length + 1)..];
            string[] parts = suffix.Length == 0 ? [] : suffix.Split('.');
            if (!s_identifier.IsMatch(rootNamespace) || !s_identifier.IsMatch(type)
                || parts.Any(p => !s_identifier.IsMatch(p)))
            {
                return false;
            }

            string directory = parts.Aggregate(root, Path.Combine);
            for (CultureInfo culture = CultureInfo.CurrentUICulture; ; culture = culture.Parent)
            {
                string file = Path.Combine(directory, type + (culture.Name.Length == 0 ? "" : "." + culture.Name) + ".resx");
                if (IsUnderRoot(root, file) && !HasReparsePoint(root, file)
                    && ReadResx(file).TryGetValue(key, out result))
                {
                    return true;
                }

                if (culture.Name.Length == 0)
                {
                    return false;
                }
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
            return false;
        }
    }

    private Dictionary<string, string> ReadResx(string file)
    {
        if (_resxCache.TryGetValue(file, out Dictionary<string, string>? cached))
        {
            return cached;
        }

        Dictionary<string, string> values = new(StringComparer.Ordinal);
        _resxCache[file] = values;
        if (++_fileReads > 64 || !File.Exists(file) || new FileInfo(file).Length > 1_000_000)
        {
            return values;
        }

        try
        {
            XmlReaderSettings settings = new() { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null, MaxCharactersInDocument = 1_000_000 };
            using XmlReader reader = XmlReader.Create(file, settings);
            XDocument doc = XDocument.Load(reader);
            HashSet<string> seen = new(StringComparer.Ordinal);
            HashSet<string> duplicates = new(StringComparer.Ordinal);
            foreach (XElement data in doc.Root?.Elements("data").Take(10000) ?? [])
            {
                string? name = (string?)data.Attribute("name");
                if (name is null || name.Length > 256)
                {
                    continue;
                }

                if (!seen.Add(name))
                {
                    duplicates.Add(name);
                }

                XElement[] elements = data.Elements("value").Take(2).ToArray();
                if (data.Attribute("type") is not null
                    || data.Attribute("mimetype") is not null || elements.Length != 1 || elements[0].HasElements
                    || elements[0].Value.Length > 4096)
                {
                    continue;
                }

                values.TryAdd(name, elements[0].Value);
            }

            foreach (string duplicate in duplicates)
            {
                values.Remove(duplicate);
            }
        }
        catch (Exception ex) when (ex is XmlException or IOException or UnauthorizedAccessException)
        {
            values.Clear();
        }

        return values;
    }

    private static bool IsAvalonia(XElement element, string name) =>
        element.Name.LocalName == name && (element.Name.Namespace == s_avalonia || element.Name.Namespace == XNamespace.None);

    private static bool IsUnderRoot(string root, string path) =>
        path.StartsWith(root + Path.DirectorySeparatorChar, OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

    private static bool HasReparsePoint(string root, string path)
    {
        DirectoryInfo directory = new(root);
        if ((directory.Attributes & FileAttributes.ReparsePoint) != 0)
        {
            return true;
        }

        for (DirectoryInfo? ancestor = directory.Parent; ancestor is not null; ancestor = ancestor.Parent)
        {
            if ((ancestor.Attributes & FileAttributes.ReparsePoint) != 0)
            {
                FileSystemInfo? target = ancestor.ResolveLinkTarget(returnFinalTarget: true);
                if (target is null || IsNetworkPath(target.FullName))
                {
                    return true;
                }
            }
        }

        string current = root;
        foreach (string part in Path.GetRelativePath(root, path).Split(Path.DirectorySeparatorChar))
        {
            current = Path.Combine(current, part);
            if ((File.Exists(current) || Directory.Exists(current))
                && (File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsNetworkPath(string path) =>
        path.StartsWith(@"\\", StringComparison.Ordinal)
        || OperatingSystem.IsWindows()
            && new DriveInfo(Path.GetPathRoot(path)!).DriveType == DriveType.Network;
}
