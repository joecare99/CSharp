using System.Text.RegularExpressions;
using XamlDecompiler.Core.Models;

namespace XamlDecompiler.Core.Services;

internal sealed class GeneratedSourceParser
{
    private static readonly string[] FallbackXmlNamespaces =
    [
        "xmlns=\"http://schemas.microsoft.com/dotnet/2021/maui\"",
        "xmlns:x=\"http://schemas.microsoft.com/winfx/2009/xaml\""
    ];

    private readonly Dictionary<string, string> _valueMap = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _bindingPathMap = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _bindingModeMap = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _bindingSourceMap = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _extensionKeyMap = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _typeHandleMap = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _relativeSourceMap = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _variableTypeMap = new(StringComparer.Ordinal);
    private readonly Dictionary<string, Dictionary<string, string>> _extensionProperties = new(StringComparer.Ordinal);
    private readonly Dictionary<string, DecompiledElement> _elements = new(StringComparer.Ordinal);
    private readonly List<string> _diagnostics = [];
    private readonly List<(string Prefix, string Namespace)> _xmlNamespaces = [];
    private readonly HashSet<string> _templateVariables = new(StringComparer.Ordinal);

    public GeneratedSourceModel Parse(string sourceText)
    {
        ResetState();

        string source = sourceText.Replace("\r\n", "\n", StringComparison.Ordinal);
        string ns = MatchValue(NamespaceRegex(), source) ?? throw new InvalidOperationException("Namespace could not be detected.");
        string xamlPath = MatchValue(XamlPathRegex(), source) ?? $"{MatchValue(ClassRegex(), source) ?? "View"}.xaml";
        Match classMatch = ClassRegex().Match(source);
        if (!classMatch.Success)
        {
            throw new InvalidOperationException("Class declaration could not be detected.");
        }

        string className = classMatch.Groups[1].Value;
        string rootTypeName = SimplifyTypeName(classMatch.Groups[2].Value);
        string initializeComponentBody = ExtractMethodBody(source, "InitializeComponent");
        string sanitizedBody = StripTemplateDelegates(initializeComponentBody);

        DecompiledElement rootElement = new(rootTypeName, ToCamelCase(className));
        _elements[rootElement.VariableName] = rootElement;
        _variableTypeMap[rootElement.VariableName] = rootTypeName;

        string? explicitRootAlias = ExtractRootAlias(sanitizedBody, className);
        if (!string.IsNullOrWhiteSpace(explicitRootAlias))
        {
            _elements[explicitRootAlias] = rootElement;
            _variableTypeMap[explicitRootAlias] = className;
        }

        ParseNamespaces(sanitizedBody);
        ParseInitializeComponentBody(sanitizedBody, rootElement);

        IReadOnlyList<(string Prefix, string Namespace)> namespaces = _xmlNamespaces.Count > 0
            ? _xmlNamespaces
            : BuildFallbackNamespaces();

        return new GeneratedSourceModel
        {
            Namespace = ns,
            ClassName = className,
            RootTypeName = rootTypeName,
            XamlFilePath = xamlPath.Replace('\\', '/'),
            UsingBlock = ExtractUsingBlock(source),
            UserMembersBlock = ExtractUserMembers(source, className),
            RootElement = rootElement,
            XmlNamespaces = namespaces,
            Diagnostics = _diagnostics.ToArray()
        };
    }

    private void ParseInitializeComponentBody(string body, DecompiledElement rootElement)
    {
        foreach (string rawLine in body.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            string line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (TryParseTypeHandle(line) ||
                TryParsePrimitiveLocalAssignment(line) ||
                TryParseVariableCreation(line) ||
                TryParseStandaloneVariableDeclaration(line, rootElement) ||
                TryParseExtensionAssignments(line) ||
                TryParseProvideValue(line) ||
                TryParseRegisterName(line) ||
                TryParseBinding(line) ||
                TryParseSetValue(line) ||
                TryParseDirectPropertyAssignment(line) ||
                TryParseEventSubscription(line) ||
                TryParseCollectionAdd(line))
            {
                continue;
            }
        }
    }

    private bool TryParseTypeHandle(string line)
    {
        Match match = TypeHandleRegex().Match(line);
        if (!match.Success)
        {
            return false;
        }

        _typeHandleMap[match.Groups[1].Value] = SimplifyTypeName(match.Groups[2].Value);
        return true;
    }

    private bool TryParsePrimitiveLocalAssignment(string line)
    {
        Match match = PrimitiveLocalAssignmentRegex().Match(line);
        if (!match.Success)
        {
            return false;
        }

        string typeName = SimplifyTypeName(match.Groups[1].Value);
        string variableName = match.Groups[2].Value;
        string expression = match.Groups[3].Value.Trim();

        _variableTypeMap[variableName] = typeName;
        _valueMap[variableName] = ResolveExpression(expression);
        return true;
    }

    private bool TryParseVariableCreation(string line)
    {
        Match match = VariableCreationRegex().Match(line);
        if (!match.Success)
        {
            return false;
        }

        string declaredType = SimplifyTypeName(match.Groups[1].Value);
        string variableName = match.Groups[2].Value;
        string constructedType = SimplifyTypeName(match.Groups[3].Value);
        string constructorArguments = match.Groups[4].Value.Trim();

        _variableTypeMap[variableName] = declaredType;
        if (ShouldRepresentAsElement(constructedType))
        {
            DecompiledElement element = new(constructedType, variableName);
            ApplyConstructorArguments(element, constructorArguments);
            _elements[variableName] = element;
        }
        else
        {
            string? primitiveValue = ConvertConstructorExpression(constructedType, constructorArguments);
            if (primitiveValue is not null)
            {
                _valueMap[variableName] = primitiveValue;
            }
        }

        return true;
    }

    private bool TryParseStandaloneVariableDeclaration(string line, DecompiledElement rootElement)
    {
        Match match = StandaloneVariableRegex().Match(line);
        if (!match.Success)
        {
            return false;
        }

        string typeName = SimplifyTypeName(match.Groups[1].Value);
        string variableName = match.Groups[2].Value;
        _variableTypeMap[variableName] = typeName;

        if (string.Equals(typeName, rootElement.TypeName, StringComparison.Ordinal) ||
            string.Equals(typeName, rootElement.VariableName, StringComparison.OrdinalIgnoreCase))
        {
            _elements[variableName] = rootElement;
        }

        return true;
    }

    private bool TryParseExtensionAssignments(string line)
    {
        Match match = DirectAssignmentRegex().Match(line);
        if (!match.Success)
        {
            return false;
        }

        string variableName = match.Groups[1].Value;
        string propertyName = match.Groups[2].Value;
        string expression = match.Groups[3].Value.Trim();

        if (propertyName.Equals("Key", StringComparison.Ordinal))
        {
            _extensionKeyMap[variableName] = StripQuotes(expression);
            return true;
        }

        if (propertyName.Equals("Path", StringComparison.Ordinal))
        {
            _bindingPathMap[variableName] = StripQuotes(expression);
            return true;
        }

        if (propertyName.Equals("Mode", StringComparison.Ordinal))
        {
            _bindingModeMap[variableName] = SimplifyEnumValue(expression);
            return true;
        }

        if (propertyName.Equals("Source", StringComparison.Ordinal))
        {
            _bindingSourceMap[variableName] = ResolveExpression(expression);
            return true;
        }

        if (propertyName.Equals("AncestorType", StringComparison.Ordinal))
        {
            _relativeSourceMap[variableName] = ResolveExpression(expression);
            return true;
        }

        if (IsExtensionVariable(variableName))
        {
            Dictionary<string, string> values = GetOrCreateExtensionValues(variableName);
            values[propertyName] = ResolveExpression(expression);
            return true;
        }

        return false;
    }

    private bool TryParseProvideValue(string line)
    {
        Match match = ProvideValueRegex().Match(line);
        if (!match.Success)
        {
            return false;
        }

        string valueVariable = match.Groups[1].Value;
        string extensionVariable = match.Groups[2].Value;
        string extensionType = _variableTypeMap.GetValueOrDefault(extensionVariable, string.Empty);
        string resolvedValue = extensionType switch
        {
            var type when type.Contains("StaticResourceExtension", StringComparison.Ordinal) => $"{{StaticResource {_extensionKeyMap.GetValueOrDefault(extensionVariable, valueVariable)}}}",
            var type when type.Contains("AppThemeResourceExtension", StringComparison.Ordinal) => $"{{AppThemeResource {_extensionKeyMap.GetValueOrDefault(extensionVariable, valueVariable)}}}",
            var type when type.Contains("OnPlatformExtension", StringComparison.Ordinal) => BuildOnPlatformValue(extensionVariable),
            var type when type.Contains("AppThemeBindingExtension", StringComparison.Ordinal) => BuildAppThemeBindingValue(extensionVariable),
            var type when type.Contains("BindingExtension", StringComparison.Ordinal) => BuildBindingValue(extensionVariable),
            var type when type.Contains("RelativeSourceExtension", StringComparison.Ordinal) => _relativeSourceMap.GetValueOrDefault(extensionVariable, "RelativeSource"),
            _ => ResolveExpression(extensionVariable)
        };

        _valueMap[valueVariable] = resolvedValue;
        return true;
    }

    private bool TryParseRegisterName(string line)
    {
        Match match = RegisterNameRegex().Match(line);
        if (!match.Success)
        {
            return false;
        }

        string xamlName = match.Groups[1].Value;
        string variableName = match.Groups[2].Value;
        if (_elements.TryGetValue(variableName, out DecompiledElement? element))
        {
            element.XamlName = xamlName;
        }

        return true;
    }

    private bool TryParseBinding(string line)
    {
        Match match = SetBindingRegex().Match(line);
        if (!match.Success)
        {
            return false;
        }

        string targetVariable = match.Groups[1].Value;
        string propertyToken = match.Groups[2].Value.Trim();
        string bindingVariable = match.Groups[3].Value;

        if (!_elements.TryGetValue(targetVariable, out DecompiledElement? target))
        {
            return false;
        }

        string propertyName = ConvertPropertyToken(propertyToken, target.TypeName);
        string bindingValue = _valueMap.GetValueOrDefault(bindingVariable, BuildBindingValue(bindingVariable));
        if (!string.IsNullOrWhiteSpace(bindingValue))
        {
            target.Attributes[propertyName] = bindingValue;
        }

        return true;
    }

    private bool TryParseSetValue(string line)
    {
        Match match = SetValueRegex().Match(line);
        if (!match.Success)
        {
            return false;
        }

        string targetVariable = match.Groups[1].Value;
        string propertyToken = match.Groups[2].Value.Trim();
        string expression = match.Groups[3].Value.Trim();

        if (!_elements.TryGetValue(targetVariable, out DecompiledElement? target))
        {
            return false;
        }

        string propertyName = ConvertPropertyToken(propertyToken, target.TypeName);
        if (TryAssignInlineDefinitionCollection(target, propertyName, expression))
        {
            return true;
        }

        if (_elements.TryGetValue(UnwrapExpression(expression), out DecompiledElement? childElement))
        {
            AssignChildProperty(target, propertyName, childElement);
            return true;
        }

        string resolvedValue = ResolveExpression(expression);
        if (!string.IsNullOrWhiteSpace(resolvedValue))
        {
            target.Attributes[propertyName] = resolvedValue;
        }

        return true;
    }

    private bool TryAssignInlineDefinitionCollection(DecompiledElement target, string propertyName, string expression)
    {
        string unwrapped = UnwrapExpression(expression);
        if (unwrapped.StartsWith("new RowDefinitionCollection(", StringComparison.Ordinal) && unwrapped.EndsWith(')'))
        {
            AddInlineDefinitions(target, propertyName, "RowDefinition", "Height", unwrapped[28..^1]);
            return true;
        }

        if (unwrapped.StartsWith("new ColumnDefinitionCollection(", StringComparison.Ordinal) && unwrapped.EndsWith(')'))
        {
            AddInlineDefinitions(target, propertyName, "ColumnDefinition", "Width", unwrapped[31..^1]);
            return true;
        }

        return false;
    }

    private void AddInlineDefinitions(DecompiledElement target, string propertyName, string definitionTypeName, string valuePropertyName, string constructorArguments)
    {
        foreach (string definitionExpression in SplitTopLevelArguments(constructorArguments))
        {
            string unwrappedDefinition = UnwrapExpression(definitionExpression);
            string prefix = $"new {definitionTypeName}(";
            if (!unwrappedDefinition.StartsWith(prefix, StringComparison.Ordinal) || !unwrappedDefinition.EndsWith(')'))
            {
                _diagnostics.Add($"Inline {definitionTypeName} expression '{definitionExpression}' could not be reconstructed exactly.");
                continue;
            }

            string valueExpression = unwrappedDefinition[prefix.Length..^1];
            int definitionIndex = target.PropertyElements.TryGetValue(propertyName, out IList<DecompiledElement>? existingDefinitions)
                ? existingDefinitions.Count
                : 0;
            DecompiledElement definitionElement = new(definitionTypeName, $"{definitionTypeName.ToLowerInvariant()}{definitionIndex}");
            definitionElement.Attributes[valuePropertyName] = ResolveExpression(valueExpression);
            target.AddPropertyElement(propertyName, definitionElement);
        }
    }

    private bool TryParseDirectPropertyAssignment(string line)
    {
        Match match = DirectAssignmentRegex().Match(line);
        if (!match.Success)
        {
            return false;
        }

        string targetVariable = match.Groups[1].Value;
        string propertyName = match.Groups[2].Value;
        string expression = match.Groups[3].Value.Trim();

        if (propertyName.Equals("LoadTemplate", StringComparison.Ordinal))
        {
            if (_elements.TryGetValue(targetVariable, out DecompiledElement? templateElement))
            {
                templateElement.Comments.Add("Delegate-based DataTemplate reconstruction is not implemented yet.");
            }

            return true;
        }

        if (propertyName.Equals("transientNamescope", StringComparison.Ordinal) ||
            propertyName.Equals("StyleId", StringComparison.Ordinal))
        {
            return true;
        }

        if (!_elements.TryGetValue(targetVariable, out DecompiledElement? target))
        {
            return false;
        }

        if (_elements.TryGetValue(UnwrapExpression(expression), out DecompiledElement? childElement))
        {
            AssignChildProperty(target, propertyName, childElement);
            return true;
        }

        string resolvedValue = ResolveExpression(expression);
        if (!string.IsNullOrWhiteSpace(resolvedValue))
        {
            target.Attributes[propertyName] = resolvedValue;
        }

        return true;
    }

    private bool TryParseEventSubscription(string line)
    {
        Match match = EventSubscriptionRegex().Match(line);
        if (!match.Success)
        {
            return false;
        }

        string targetVariable = match.Groups[1].Value;
        string eventName = match.Groups[2].Value;
        string handlerName = match.Groups[3].Value;

        if (_elements.TryGetValue(targetVariable, out DecompiledElement? target))
        {
            target.Events[eventName] = handlerName;
        }

        return true;
    }

    private bool TryParseCollectionAdd(string line)
    {
        Match dictionaryMatch = DictionaryAddRegex().Match(line);
        if (dictionaryMatch.Success)
        {
            string dictionaryVariable = dictionaryMatch.Groups[1].Value;
            string resourceKey = dictionaryMatch.Groups[2].Value;
            string resourceVariable = dictionaryMatch.Groups[3].Value;

            if (_elements.TryGetValue(dictionaryVariable, out DecompiledElement? dictionaryElement) && _elements.TryGetValue(resourceVariable, out DecompiledElement? resourceElement))
            {
                resourceElement.ResourceKey = resourceKey;
                dictionaryElement.AddChild(resourceElement);
            }
            else if (_elements.TryGetValue(dictionaryVariable, out dictionaryElement) && _valueMap.TryGetValue(resourceVariable, out string? resourceValue))
            {
                string resourceTypeName = ToXamlPrimitiveType(_variableTypeMap.GetValueOrDefault(resourceVariable, string.Empty));
                DecompiledElement primitiveResource = new(resourceTypeName, resourceVariable)
                {
                    ResourceKey = resourceKey,
                    TextValue = resourceValue
                };
                dictionaryElement.AddChild(primitiveResource);
            }
            else
            {
                _diagnostics.Add($"Resource key '{resourceKey}' was detected but could not be reconstructed exactly.");
            }

            return true;
        }

        Match castCollectionMatch = CastCollectionAddRegex().Match(line);
        if (castCollectionMatch.Success)
        {
            string castTargetVariable = castCollectionMatch.Groups[1].Success
                ? castCollectionMatch.Groups[1].Value
                : castCollectionMatch.Groups[3].Value;
            string castPropertyName = castCollectionMatch.Groups[2].Success
                ? castCollectionMatch.Groups[2].Value
                : castCollectionMatch.Groups[4].Value;
            string castChildVariable = UnwrapExpression(castCollectionMatch.Groups[5].Value);

            if (!_elements.TryGetValue(castTargetVariable, out DecompiledElement? castTarget) || !_elements.TryGetValue(castChildVariable, out DecompiledElement? castChild))
            {
                return false;
            }

            AddCollectionChild(castTarget, castPropertyName, castChild);
            return true;
        }

        Match match = CollectionAddRegex().Match(line);
        if (!match.Success)
        {
            return false;
        }

        string targetVariable = match.Groups[1].Value;
        string propertyName = match.Groups[2].Value;
        string childVariable = match.Groups[3].Value;

        if (!_elements.TryGetValue(targetVariable, out DecompiledElement? target) || !_elements.TryGetValue(childVariable, out DecompiledElement? child))
        {
            return false;
        }

        AddCollectionChild(target, propertyName, child);

        return true;
    }

    private static void AddCollectionChild(DecompiledElement target, string propertyName, DecompiledElement child)
    {
        if (propertyName.Equals("Children", StringComparison.Ordinal) ||
            propertyName.Equals("Items", StringComparison.Ordinal))
        {
            target.AddChild(child);
        }
        else
        {
            target.AddPropertyElement(propertyName, child);
        }
    }

    private void AssignChildProperty(DecompiledElement target, string propertyName, DecompiledElement childElement)
    {
        if (propertyName.Equals("Content", StringComparison.Ordinal))
        {
            target.Content = childElement;
            return;
        }

        target.AddPropertyElement(propertyName, childElement);
    }

    private void ApplyConstructorArguments(DecompiledElement element, string constructorArguments)
    {
        if (string.IsNullOrWhiteSpace(constructorArguments))
        {
            return;
        }

        if (element.TypeName.Equals("Style", StringComparison.Ordinal))
        {
            element.Attributes["TargetType"] = $"{{x:Type {ResolveExpression(constructorArguments)}}}";
        }
        else if (element.TypeName.Equals("DataTrigger", StringComparison.Ordinal))
        {
            element.Attributes["TargetType"] = $"{{x:Type {ResolveExpression(constructorArguments)}}}";
        }
        else if (element.TypeName.Equals("RowDefinition", StringComparison.Ordinal))
        {
            element.Attributes["Height"] = ResolveExpression(constructorArguments);
        }
        else if (element.TypeName.Equals("ColumnDefinition", StringComparison.Ordinal))
        {
            element.Attributes["Width"] = ResolveExpression(constructorArguments);
        }
        else if (element.TypeName.Equals("DataTemplate", StringComparison.Ordinal))
        {
            string resolvedType = ResolveExpression(constructorArguments);
            if (!string.IsNullOrWhiteSpace(resolvedType))
            {
                element.Attributes["DataType"] = $"{{x:Type {resolvedType}}}";
            }
        }
    }

    private string ResolveExpression(string expression)
    {
        string unwrapped = UnwrapExpression(expression);
        string passthroughExpression = TryUnwrapPassthroughExpression(unwrapped);
        if (!ReferenceEquals(passthroughExpression, unwrapped))
        {
            return ResolveExpression(passthroughExpression);
        }

        if (_valueMap.TryGetValue(unwrapped, out string? mappedValue))
        {
            return mappedValue;
        }

        if (_typeHandleMap.TryGetValue(unwrapped, out string? mappedTypeValue))
        {
            return mappedTypeValue;
        }

        if (unwrapped.StartsWith("typeof(", StringComparison.Ordinal) && unwrapped.EndsWith(')'))
        {
            return SimplifyTypeName(unwrapped[7..^1]);
        }

        if (unwrapped.StartsWith("ImageSource.FromFile(", StringComparison.Ordinal))
        {
            int quoteIndex = unwrapped.IndexOf('"');
            int endQuoteIndex = unwrapped.LastIndexOf('"');
            if (quoteIndex >= 0 && endQuoteIndex > quoteIndex)
            {
                return unwrapped[(quoteIndex + 1)..endQuoteIndex];
            }
        }

        if (unwrapped.StartsWith("new Thickness(", StringComparison.Ordinal) && unwrapped.EndsWith(')'))
        {
            return NormalizeNumericList(unwrapped[14..^1]);
        }

        if (unwrapped.StartsWith("new GridLength(", StringComparison.Ordinal) && unwrapped.EndsWith(')'))
        {
            return ConvertGridLength(unwrapped[15..^1]);
        }

        if (unwrapped.StartsWith("new ItemsLayoutTypeConverter().ConvertFromInvariantString(", StringComparison.Ordinal))
        {
            return StripQuotes(unwrapped[(unwrapped.IndexOf('(') + 1)..^1]);
        }

        string? queriedTypeValue = TryResolveTypeValueQuery(unwrapped);
        if (!string.IsNullOrWhiteSpace(queriedTypeValue))
        {
            return queriedTypeValue;
        }

        if (Regex.IsMatch(unwrapped, "^\".*\"$", RegexOptions.Singleline))
        {
            return StripQuotes(unwrapped);
        }

        if (bool.TryParse(unwrapped, out bool booleanValue))
        {
            return booleanValue ? "True" : "False";
        }

        if (double.TryParse(unwrapped, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double numericValue))
        {
            return numericValue.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture);
        }

        if (unwrapped.Contains('.', StringComparison.Ordinal))
        {
            return SimplifyEnumValue(unwrapped);
        }

        return unwrapped;
    }

    private static string TryUnwrapPassthroughExpression(string expression)
    {
        int questionMarkIndex = FindTopLevelCharacter(expression, '?');
        if (questionMarkIndex < 0)
        {
            return expression;
        }

        int colonIndex = FindTopLevelCharacter(expression, ':', questionMarkIndex + 1);
        if (colonIndex < 0)
        {
            return expression;
        }

        string whenTrue = UnwrapExpression(expression[(questionMarkIndex + 1)..colonIndex]);
        string whenFalse = UnwrapExpression(expression[(colonIndex + 1)..]);
        return string.Equals(whenTrue, whenFalse, StringComparison.Ordinal)
            ? whenTrue
            : expression;
    }

    private static int FindTopLevelCharacter(string expression, char targetCharacter, int startIndex = 0)
    {
        int parenthesisDepth = 0;
        for (int index = startIndex; index < expression.Length; index++)
        {
            char current = expression[index];
            if (current == '(')
            {
                parenthesisDepth++;
                continue;
            }

            if (current == ')')
            {
                parenthesisDepth--;
                continue;
            }

            if (parenthesisDepth == 0 && current == targetCharacter)
            {
                return index;
            }
        }

        return -1;
    }

    private string? TryResolveTypeValueQuery(string expression)
    {
        Match match = StaticMethodCallRegex().Match(expression);
        if (!match.Success)
        {
            return null;
        }

        string declaringType = SimplifyTypeName(match.Groups[1].Value);
        string methodName = match.Groups[2].Value;
        IReadOnlyList<string> arguments = SplitTopLevelArguments(match.Groups[3].Value);

        return declaringType switch
        {
            "Device" when methodName.Equals("GetNamedSize", StringComparison.Ordinal) => ResolveNamedSizeQuery(arguments),
            _ => null
        };
    }

    private string? ResolveNamedSizeQuery(IReadOnlyList<string> arguments)
    {
        if (arguments.Count == 0)
        {
            return null;
        }

        string namedSize = SimplifyEnumValue(arguments[0]);
        return string.IsNullOrWhiteSpace(namedSize) ? null : namedSize;
    }

    private static string ConvertGridLength(string expression)
    {
        string[] parts = expression.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1)
        {
            return ResolveNumeric(parts[0]);
        }

        if (parts.Length == 2 && parts[1].EndsWith("Star", StringComparison.Ordinal))
        {
            string factor = ResolveNumeric(parts[0]);
            return factor == "1" ? "*" : $"{factor}*";
        }

        return expression;
    }

    private static string ResolveNumeric(string value)
    {
        return double.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double number)
            ? number.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture)
            : value;
    }

    private string BuildBindingValue(string bindingVariable)
    {
        string path = _bindingPathMap.GetValueOrDefault(bindingVariable, string.Empty);
        _bindingSourceMap.TryGetValue(bindingVariable, out string? source);
        _bindingModeMap.TryGetValue(bindingVariable, out string? mode);

        if (string.IsNullOrWhiteSpace(path) && string.IsNullOrWhiteSpace(source) && string.IsNullOrWhiteSpace(mode))
        {
            return string.Empty;
        }

        List<string> parts = [];
        if (!string.IsNullOrWhiteSpace(path))
        {
            parts.Add(path == "." ? "." : path);
        }

        if (!string.IsNullOrWhiteSpace(mode))
        {
            parts.Add($"Mode={mode}");
        }

        if (!string.IsNullOrWhiteSpace(source) && !source.Equals("RelativeSource", StringComparison.Ordinal))
        {
            parts.Add($"Source={source}");
        }

        return $"{{Binding {string.Join(", ", parts)}}}";
    }

    private string BuildOnPlatformValue(string extensionVariable)
    {
        Dictionary<string, string> values = GetOrCreateExtensionValues(extensionVariable);
        string body = string.Join(", ", values.OrderBy(static pair => pair.Key, StringComparer.Ordinal).Select(static pair => $"{pair.Key}={pair.Value}"));
        return $"{{OnPlatform {body}}}";
    }

    private string BuildAppThemeBindingValue(string extensionVariable)
    {
        Dictionary<string, string> values = GetOrCreateExtensionValues(extensionVariable);
        string light = values.GetValueOrDefault("Light", string.Empty);
        string dark = values.GetValueOrDefault("Dark", string.Empty);
        List<string> parts = [];
        if (!string.IsNullOrWhiteSpace(light))
        {
            parts.Add($"Light={light}");
        }

        if (!string.IsNullOrWhiteSpace(dark))
        {
            parts.Add($"Dark={dark}");
        }

        return parts.Count == 0 ? string.Empty : $"{{AppThemeBinding {string.Join(", ", parts)}}}";
    }

    private static string ConvertPropertyToken(string propertyToken, string targetTypeName)
    {
        string token = propertyToken.Trim();
        if (!token.EndsWith("Property", StringComparison.Ordinal))
        {
            return token;
        }

        string[] segments = token.Split('.');
        if (segments.Length < 2)
        {
            return token[..^8];
        }

        string owner = SimplifyTypeName(segments[^2]);
        string propertyName = segments[^1][..^8];
        if (owner.Equals("Grid", StringComparison.Ordinal) && GridInstancePropertyNames.Contains(propertyName))
        {
            return propertyName;
        }

        if (AttachedPropertyOwners.Contains(owner))
        {
            return $"{owner}.{propertyName}";
        }

        return owner.Equals(targetTypeName, StringComparison.Ordinal) ? propertyName : propertyName;
    }

    private static readonly HashSet<string> AttachedPropertyOwners = new(StringComparer.Ordinal)
    {
        "Grid",
        "Shell",
        "SemanticProperties",
        "ToolTipProperties",
        "LeakMonitorBehavior"
    };

    private static readonly HashSet<string> GridInstancePropertyNames = new(StringComparer.Ordinal)
    {
        "ColumnDefinitions",
        "RowDefinitions"
    };

    private static bool ShouldRepresentAsElement(string typeName)
    {
        return !IgnoredConstructedTypes.Contains(typeName);
    }

    private static string ToXamlPrimitiveType(string typeName)
    {
        return typeName switch
        {
            "double" or "Double" => "x:Double",
            "float" or "Single" => "x:Single",
            "decimal" or "Decimal" => "x:Decimal",
            "int" or "Int32" => "x:Int32",
            "uint" or "UInt32" => "x:UInt32",
            "long" or "Int64" => "x:Int64",
            "ulong" or "UInt64" => "x:UInt64",
            "short" or "Int16" => "x:Int16",
            "ushort" or "UInt16" => "x:UInt16",
            "byte" or "Byte" => "x:Byte",
            "sbyte" or "SByte" => "x:SByte",
            "bool" or "Boolean" => "x:Boolean",
            "string" or "String" => "x:String",
            _ => "x:String"
        };
    }

    private static readonly HashSet<string> IgnoredConstructedTypes = new(StringComparer.Ordinal)
    {
        "StaticResourceExtension",
        "AppThemeBindingExtension",
        "AppThemeResourceExtension",
        "BindingExtension",
        "RelativeSourceExtension",
        "OnPlatformExtension",
        "XamlServiceProvider",
        "SimpleValueTargetProvider",
        "XmlNamespaceResolver",
        "TypedBinding",
        "BindingBase",
        "RelativeBindingSource",
        "XamlTypeResolver",
        "XmlLineInfoProvider",
        "XmlLineInfo",
        "NameScope",
        "Tuple",
        "Type",
        "object"
    };

    private static string ConvertConstructorExpression(string constructedType, string constructorArguments)
    {
        if (constructedType.Equals("Thickness", StringComparison.Ordinal))
        {
            return NormalizeNumericList(constructorArguments);
        }

        return string.Empty;
    }

    private static string NormalizeNumericList(string values)
    {
        string[] parts = values.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        return string.Join(",", parts.Select(ResolveNumeric));
    }

    private static IReadOnlyList<string> SplitTopLevelArguments(string expression)
    {
        List<string> arguments = [];
        int startIndex = 0;
        int parenthesisDepth = 0;

        for (int index = 0; index < expression.Length; index++)
        {
            char current = expression[index];
            if (current == '(')
            {
                parenthesisDepth++;
            }
            else if (current == ')')
            {
                parenthesisDepth--;
            }
            else if (current == ',' && parenthesisDepth == 0)
            {
                arguments.Add(expression[startIndex..index].Trim());
                startIndex = index + 1;
            }
        }

        string lastArgument = expression[startIndex..].Trim();
        if (!string.IsNullOrWhiteSpace(lastArgument))
        {
            arguments.Add(lastArgument);
        }

        return arguments;
    }

    private static string SimplifyEnumValue(string value)
    {
        string normalized = UnwrapExpression(value);
        if (normalized.Equals("GridLength.Auto", StringComparison.Ordinal))
        {
            return "Auto";
        }

        if (normalized.Equals("GridLength.Star", StringComparison.Ordinal))
        {
            return "*";
        }

        int lastDot = normalized.LastIndexOf('.');
        return lastDot >= 0 ? normalized[(lastDot + 1)..] : normalized;
    }

    private static string ExtractUsingBlock(string source)
    {
        MatchCollection matches = UsingRegex().Matches(source);
        return string.Join(Environment.NewLine, matches.Select(static match => match.Value.TrimEnd()));
    }

    private static string ExtractUserMembers(string source, string className)
    {
        int constructorIndex = source.IndexOf($"public {className}(", StringComparison.Ordinal);
        int initializeComponentIndex = source.IndexOf("private void InitializeComponent()", StringComparison.Ordinal);
        if (constructorIndex < 0 || initializeComponentIndex <= constructorIndex)
        {
            return string.Empty;
        }

        return source[constructorIndex..initializeComponentIndex].Trim();
    }

    private static string ExtractMethodBody(string source, string methodName)
    {
        int methodIndex = source.IndexOf($"private void {methodName}()", StringComparison.Ordinal);
        if (methodIndex < 0)
        {
            methodIndex = source.IndexOf($"void {methodName}()", StringComparison.Ordinal);
        }

        if (methodIndex < 0)
        {
            throw new InvalidOperationException($"Method '{methodName}' could not be located.");
        }

        int startBraceIndex = source.IndexOf('{', methodIndex);
        if (startBraceIndex < 0)
        {
            throw new InvalidOperationException($"Method '{methodName}' does not contain a body.");
        }

        int depth = 0;
        for (int index = startBraceIndex; index < source.Length; index++)
        {
            char current = source[index];
            if (current == '{')
            {
                depth++;
            }
            else if (current == '}')
            {
                depth--;
                if (depth == 0)
                {
                    return source[(startBraceIndex + 1)..index];
                }
            }
        }

        throw new InvalidOperationException($"Method '{methodName}' body could not be parsed.");
    }

    private string StripTemplateDelegates(string body)
    {
        string workingBody = body;
        Match match = TemplateDelegateStartRegex().Match(workingBody);
        while (match.Success)
        {
            string variableName = match.Groups[1].Value;
            _templateVariables.Add(variableName);

            int openBraceIndex = workingBody.IndexOf('{', match.Index);
            if (openBraceIndex < 0)
            {
                break;
            }

            int depth = 0;
            int endIndex = openBraceIndex;
            for (; endIndex < workingBody.Length; endIndex++)
            {
                char current = workingBody[endIndex];
                if (current == '{')
                {
                    depth++;
                }
                else if (current == '}')
                {
                    depth--;
                    if (depth == 0)
                    {
                        break;
                    }
                }
            }

            if (endIndex >= workingBody.Length)
            {
                break;
            }

            int semicolonIndex = workingBody.IndexOf(';', endIndex);
            if (semicolonIndex < 0)
            {
                break;
            }

            workingBody = workingBody.Remove(match.Index, (semicolonIndex - match.Index) + 1)
                .Insert(match.Index, $"{variableName}.LoadTemplate = null;\n");
            match = TemplateDelegateStartRegex().Match(workingBody, match.Index + 1);
        }

        return workingBody;
    }

    private string? ExtractRootAlias(string body, string className)
    {
        MatchCollection matches = StandaloneVariableRegex().Matches(body);
        foreach (Match match in matches)
        {
            if (SimplifyTypeName(match.Groups[1].Value).Equals(className, StringComparison.Ordinal))
            {
                return match.Groups[2].Value;
            }
        }

        return null;
    }

    private void ParseNamespaces(string body)
    {
        foreach (Match match in XmlNamespaceRegex().Matches(body))
        {
            string prefix = match.Groups[1].Value;
            string namespaceValue = match.Groups[2].Value;
            if (_xmlNamespaces.Any(existing => existing.Prefix == prefix && existing.Namespace == namespaceValue))
            {
                continue;
            }

            _xmlNamespaces.Add((prefix, namespaceValue));
        }
    }

    private IReadOnlyList<(string Prefix, string Namespace)> BuildFallbackNamespaces()
    {
        List<(string Prefix, string Namespace)> namespaces = [];
        foreach (string declaration in FallbackXmlNamespaces)
        {
            int separatorIndex = declaration.IndexOf('=');
            string prefix = declaration[5..separatorIndex].Trim(':');
            string namespaceValue = declaration[(separatorIndex + 2)..^1];
            namespaces.Add((prefix, namespaceValue));
        }

        return namespaces;
    }

    private static string MatchValue(Regex regex, string source)
    {
        Match match = regex.Match(source);
        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    private static string StripQuotes(string expression)
    {
        string trimmed = expression.Trim();
        return trimmed.Length >= 2 && trimmed[0] == '"' && trimmed[^1] == '"'
            ? trimmed[1..^1]
            : trimmed;
    }

    private static string ToCamelCase(string value)
    {
        return string.IsNullOrEmpty(value)
            ? value
            : char.ToLowerInvariant(value[0]) + value[1..];
    }

    private static string SimplifyTypeName(string value)
    {
        string trimmed = UnwrapExpression(value).Trim();
        int genericIndex = trimmed.IndexOf('<');
        string genericSuffix = genericIndex >= 0 ? trimmed[genericIndex..] : string.Empty;
        string simple = genericIndex >= 0 ? trimmed[..genericIndex] : trimmed;
        int lastDot = simple.LastIndexOf('.');
        return (lastDot >= 0 ? simple[(lastDot + 1)..] : simple) + genericSuffix;
    }

    private static string UnwrapExpression(string expression)
    {
        string value = expression.Trim();
        while (value.StartsWith('(') && value.EndsWith(')'))
        {
            value = value[1..^1].Trim();
        }

        value = CastRegex().Replace(value, "$1").Trim();
        return value;
    }

    private static bool IsExtensionVariable(string variableName)
    {
        return variableName.EndsWith("Extension", StringComparison.Ordinal) || variableName.Contains("bindingExtension", StringComparison.OrdinalIgnoreCase);
    }

    private Dictionary<string, string> GetOrCreateExtensionValues(string variableName)
    {
        if (!_extensionProperties.TryGetValue(variableName, out Dictionary<string, string>? values))
        {
            values = new Dictionary<string, string>(StringComparer.Ordinal);
            _extensionProperties[variableName] = values;
        }

        return values;
    }

    private void ResetState()
    {
        _valueMap.Clear();
        _bindingPathMap.Clear();
        _bindingModeMap.Clear();
        _bindingSourceMap.Clear();
        _extensionKeyMap.Clear();
        _typeHandleMap.Clear();
        _relativeSourceMap.Clear();
        _variableTypeMap.Clear();
        _extensionProperties.Clear();
        _elements.Clear();
        _diagnostics.Clear();
        _xmlNamespaces.Clear();
        _templateVariables.Clear();
    }

    private static Regex NamespaceRegex() => new("^namespace\\s+([\\w\\.]+);", RegexOptions.Multiline | RegexOptions.Compiled);

    private static Regex XamlPathRegex() => new("\\[XamlFilePath\\(\"([^\"]+)\"\\)\\]", RegexOptions.Compiled);

    private static Regex ClassRegex() => new("public\\s+(?:partial\\s+)?class\\s+(\\w+)\\s*:\\s*([^\\r\\n{]+)", RegexOptions.Compiled);

    private static Regex UsingRegex() => new("^using\\s+[^;]+;", RegexOptions.Multiline | RegexOptions.Compiled);

    private static Regex TypeHandleRegex() => new("^Type\\?*\\s+(\\w+)\\s*=\\s*typeof\\(([^)]+)\\);$", RegexOptions.Compiled);

    private static Regex PrimitiveLocalAssignmentRegex() => new("^(bool|byte|decimal|double|float|int|long|sbyte|short|string|uint|ulong|ushort|Boolean|Byte|Decimal|Double|Single|Int32|Int64|SByte|Int16|String|UInt32|UInt64|UInt16)\\s+(\\w+)\\s*=\\s*(.+);$", RegexOptions.Compiled);

    private static Regex VariableCreationRegex() => new("^([\\w\\.<>,?]+)\\s+(\\w+)\\s*=\\s*new\\s+([\\w\\.<>,?]+)\\((.*)\\);$", RegexOptions.Compiled);

    private static Regex StandaloneVariableRegex() => new("^([\\w\\.<>,?]+)\\s+(\\w+)\\s*;$", RegexOptions.Compiled);

    private static Regex ProvideValueRegex() => new("^(?:object|BindingBase|RelativeBindingSource)\\s+(\\w+)\\s*=\\s*\\(\\(IMarkupExtension(?:<[^>]+>)?\\)(\\w+)\\)\\.ProvideValue", RegexOptions.Compiled);

    private static Regex RegisterNameRegex() => new("^\\(\\(INameScope\\)\\w+\\)\\.RegisterName\\(\"([^\"]+)\",\\s*\\(object\\)(\\w+)\\);$", RegexOptions.Compiled);

    private static Regex SetBindingRegex() => new("^(\\w+)\\.SetBinding\\(([^,]+),\\s*(\\w+)\\);$", RegexOptions.Compiled);

    private static Regex SetValueRegex() => new("^(\\w+)\\.SetValue\\(([^,]+),\\s*(.+)\\);$", RegexOptions.Compiled);

    private static Regex DirectAssignmentRegex() => new("^(\\w+)\\.(\\w+)\\s*=\\s*(.+);$", RegexOptions.Compiled);

    private static Regex EventSubscriptionRegex() => new("^(\\w+)\\.(\\w+)\\s*\\+=\\s*\\w+\\.(\\w+);$", RegexOptions.Compiled);

    private static Regex CollectionAddRegex() => new("^(\\w+)\\.([A-Za-z]+)\\.Add\\((\\w+)\\);$", RegexOptions.Compiled);

    private static Regex CastCollectionAddRegex() => new("^\\(\\(ICollection<[^>]+>\\)(?:(\\w+)\\.([A-Za-z]+)|(\\w+)\\.GetValue\\((?:[A-Za-z_][\\w]*\\.)?([A-Za-z]+)Property\\))\\)\\.Add\\((.+)\\);$", RegexOptions.Compiled);

    private static Regex DictionaryAddRegex() => new("^(\\w+)\\.Add\\(\"([^\"]+)\",\\s*(\\w+)\\);$", RegexOptions.Compiled);

    private static Regex TemplateDelegateStartRegex() => new("(\\w+)\\.LoadTemplate\\s*=\\s*delegate", RegexOptions.Compiled);

    private static Regex XmlNamespaceRegex() => new("xmlNamespaceResolver\\d*\\.Add\\(\"([^\"]*)\",\\s*\"([^\"]+)\"\\);", RegexOptions.Compiled);

    private static Regex StaticMethodCallRegex() => new("^([\\w\\.]+)\\.(\\w+)\\((.*)\\)$", RegexOptions.Compiled);

    private static Regex CastRegex() => new("^\\s*(?:\\((?:[\\w\\.<>,\\?]+)\\)\\s*)+(.+)$", RegexOptions.Compiled);
}
