using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace CommandlineHelper.Generators;

[Generator(LanguageNames.CSharp)]
public sealed class CommandlineHelperGenerator : IIncrementalGenerator
{
    private const string CommandDescriptorAttributeName = "CommandlineHelper.CommandDescriptorAttribute";
    private const string CommandOptionAttributeName = "CommandlineHelper.CommandOptionAttribute";
    private const string CommandFlagAttributeName = "CommandlineHelper.CommandFlagAttribute";
    private const string CommandArgumentAttributeName = "CommandlineHelper.CommandArgumentAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var commands = context.SyntaxProvider
            .CreateSyntaxProvider(static (node, _) => node is ClassDeclarationSyntax classDeclaration && classDeclaration.AttributeLists.Count > 0,
                static (ctx, ct) => CreateCommandModel(ctx, ct))
            .Where(static model => model is not null)
            .Select(static (model, _) => model!);

        context.RegisterSourceOutput(commands, static (spc, model) => Execute(spc, model));
    }

    private static void Execute(SourceProductionContext context, CommandModel model)
    {
        var hasErrors = false;
        foreach (var diagnostic in model.Diagnostics)
        {
            context.ReportDiagnostic(Diagnostic.Create(diagnostic.Descriptor, diagnostic.Location, diagnostic.MessageArguments));
            hasErrors = true;
        }

        if (hasErrors)
            return;

        var source = EmitCommand(model);
        context.AddSource($"{model.OptionsTypeName}.CommandlineHelper.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    private static CommandModel? CreateCommandModel(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        if (context.Node is not ClassDeclarationSyntax classDeclaration)
            return null;

        if (context.SemanticModel.GetDeclaredSymbol(classDeclaration, cancellationToken) is not INamedTypeSymbol typeSymbol)
            return null;

        var descriptorAttribute = GetAttribute(typeSymbol, CommandDescriptorAttributeName);
        if (descriptorAttribute is null)
            return null;

        var diagnostics = new List<DiagnosticInfo>();
        var properties = new List<PropertyModel>();
        var commandName = descriptorAttribute.ConstructorArguments.Length == 1
            ? descriptorAttribute.ConstructorArguments[0].Value as string
            : null;

        if (string.IsNullOrWhiteSpace(commandName))
        {
            diagnostics.Add(new DiagnosticInfo
            {
                Descriptor = DiagnosticDescriptors.InvalidCommandDeclaration,
                Location = typeSymbol.Locations.FirstOrDefault(),
                MessageArguments = new object[] { typeSymbol.Name, "missing command name" }
            });
            commandName = typeSymbol.Name;
        }

        var resourceType = GetNamedTypeArgument(descriptorAttribute, "ResourceType");
        var descriptionResourceName = GetNamedStringArgument(descriptorAttribute, "DescriptionResourceName");
        var helpTextResourceName = GetNamedStringArgument(descriptorAttribute, "HelpTextResourceName");

        ValidateResourceReference(resourceType, descriptionResourceName, typeSymbol.Locations.FirstOrDefault(), diagnostics);
        ValidateResourceReference(resourceType, helpTextResourceName, typeSymbol.Locations.FirstOrDefault(), diagnostics);

        foreach (var propertySymbol in typeSymbol.GetMembers().OfType<IPropertySymbol>())
        {
            var bindingAttributes = propertySymbol.GetAttributes()
                .Where(static attribute =>
                {
                    var name = attribute.AttributeClass?.ToDisplayString();
                    return name == CommandOptionAttributeName
                        || name == CommandFlagAttributeName
                        || name == CommandArgumentAttributeName;
                })
                .ToArray();

            if (bindingAttributes.Length == 0)
                continue;

            if (bindingAttributes.Length > 1)
            {
                diagnostics.Add(new DiagnosticInfo
                {
                    Descriptor = DiagnosticDescriptors.MultipleBindingAttributes,
                    Location = propertySymbol.Locations.FirstOrDefault(),
                    MessageArguments = new object[] { propertySymbol.Name }
                });
                continue;
            }

            var propertyDeclaration = propertySymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax(cancellationToken) as PropertyDeclarationSyntax;
            var propertyModel = TryCreatePropertyModel(propertySymbol, propertyDeclaration, bindingAttributes[0], resourceType, diagnostics);
            if (propertyModel is not null)
                properties.Add(propertyModel);
        }

        ValidateDuplicates(commandName!, properties, diagnostics);

        return new CommandModel
        {
            CommandName = commandName!,
            NamespaceName = typeSymbol.ContainingNamespace.IsGlobalNamespace ? string.Empty : typeSymbol.ContainingNamespace.ToDisplayString(),
            OptionsTypeName = typeSymbol.Name,
            OptionsTypeQualifiedName = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            CompanionTypeName = typeSymbol.Name + "Command",
            ResourceTypeQualifiedName = resourceType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            DescriptionResourceName = descriptionResourceName,
            HelpTextResourceName = helpTextResourceName,
            Location = typeSymbol.Locations.FirstOrDefault(),
            Properties = properties,
            Diagnostics = diagnostics
        };
    }

    private static PropertyModel? TryCreatePropertyModel(
        IPropertySymbol propertySymbol,
        PropertyDeclarationSyntax? propertyDeclaration,
        AttributeData attribute,
        INamedTypeSymbol? commandResourceType,
        List<DiagnosticInfo> diagnostics)
    {
        var bindingKind = GetBindingKind(attribute);
        if (bindingKind is null)
            return null;

        var propertyType = propertySymbol.Type;
        var isNullable = propertyType.NullableAnnotation == NullableAnnotation.Annotated;
        var initializerText = propertyDeclaration?.Initializer?.Value.ToString();
        var resourceType = GetNamedTypeArgument(attribute, "ResourceType") ?? commandResourceType;
        var descriptionResourceName = GetNamedStringArgument(attribute, "DescriptionResourceName");
        ValidateResourceReference(resourceType, descriptionResourceName, propertySymbol.Locations.FirstOrDefault(), diagnostics);

        var collectionInfo = TryGetCollectionInfo(propertyType);
        var targetType = collectionInfo?.ElementType ?? UnwrapNullable(propertyType);
        if (!TryGetValueKind(targetType, out var valueKind))
        {
            diagnostics.Add(new DiagnosticInfo
            {
                Descriptor = collectionInfo is null ? DiagnosticDescriptors.UnsupportedPropertyType : DiagnosticDescriptors.UnsupportedCollectionTarget,
                Location = propertySymbol.Locations.FirstOrDefault(),
                MessageArguments = new object[] { propertySymbol.Name, propertyType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) }
            });
            return null;
        }

        if (bindingKind == BindingKind.Flag && (collectionInfo is not null || valueKind != ValueKind.Boolean || propertyType.NullableAnnotation == NullableAnnotation.Annotated))
        {
            diagnostics.Add(new DiagnosticInfo
            {
                Descriptor = DiagnosticDescriptors.InvalidFlagDeclaration,
                Location = propertySymbol.Locations.FirstOrDefault(),
                MessageArguments = new object[] { propertySymbol.Name }
            });
            return null;
        }

        if (bindingKind == BindingKind.Argument)
        {
            var position = attribute.ConstructorArguments.Length == 1 && attribute.ConstructorArguments[0].Value is int declaredPosition ? declaredPosition : -1;
            if (position != 0)
            {
                diagnostics.Add(new DiagnosticInfo
                {
                    Descriptor = DiagnosticDescriptors.InvalidArgumentPosition,
                    Location = propertySymbol.Locations.FirstOrDefault(),
                    MessageArguments = new object[] { propertySymbol.Name, position }
                });
                return null;
            }
        }

        var explicitDefaultValueCode = TryGetExplicitDefaultValueCode(attribute, propertySymbol, valueKind, collectionInfo, diagnostics);
        var inferredDefaultValueCode = TryGetInferredDefaultValueCode(propertySymbol, propertyDeclaration, collectionInfo, valueKind, diagnostics);
        var requiredOverride = GetNamedBooleanArgument(attribute, "Required");
        var isRequired = requiredOverride ?? InferRequired(propertySymbol, bindingKind.Value, collectionInfo is not null, initializerText is not null, explicitDefaultValueCode is not null);

        return new PropertyModel
        {
            PropertyName = propertySymbol.Name,
            PropertyTypeQualifiedName = propertyType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            AssignmentTypeQualifiedName = targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            BindingKind = bindingKind.Value,
            ValueKind = valueKind,
            IsNullable = isNullable,
            IsCollection = collectionInfo is not null,
            ElementTypeQualifiedName = collectionInfo?.ElementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            CollectionKind = collectionInfo?.Kind ?? CollectionKind.None,
            LongName = GetConstructorStringArgument(attribute),
            ShortName = GetNamedStringArgument(attribute, "ShortName"),
            Position = bindingKind == BindingKind.Argument ? 0 : null,
            IsRequired = isRequired,
            ExplicitDefaultValueCode = explicitDefaultValueCode,
            InferredDefaultValueCode = inferredDefaultValueCode,
            UsesEmptyCollectionDefault = collectionInfo is not null,
            ResourceTypeQualifiedName = resourceType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            DescriptionResourceName = descriptionResourceName,
            Location = propertySymbol.Locations.FirstOrDefault()
        };
    }

    private static bool InferRequired(IPropertySymbol propertySymbol, BindingKind bindingKind, bool isCollection, bool hasInitializer, bool hasExplicitDefault)
    {
        if (bindingKind == BindingKind.Flag)
            return false;

        if (isCollection)
            return false;

        if (hasExplicitDefault || hasInitializer)
            return false;

        if (propertySymbol.IsRequired)
            return true;

        if (propertySymbol.Type.IsReferenceType && propertySymbol.NullableAnnotation != NullableAnnotation.Annotated)
            return true;

        return false;
    }

    private static void ValidateDuplicates(string commandName, IReadOnlyList<PropertyModel> properties, List<DiagnosticInfo> diagnostics)
    {
        foreach (var duplicate in properties.Where(static property => !string.IsNullOrWhiteSpace(property.LongName)).GroupBy(static property => property.LongName!, StringComparer.OrdinalIgnoreCase).Where(static group => group.Count() > 1))
        {
            diagnostics.Add(new DiagnosticInfo
            {
                Descriptor = DiagnosticDescriptors.DuplicateBinding,
                Location = duplicate.First().Location,
                MessageArguments = new object[] { commandName, duplicate.Key }
            });
        }

        foreach (var duplicate in properties.Where(static property => !string.IsNullOrWhiteSpace(property.ShortName)).GroupBy(static property => property.ShortName!, StringComparer.OrdinalIgnoreCase).Where(static group => group.Count() > 1))
        {
            diagnostics.Add(new DiagnosticInfo
            {
                Descriptor = DiagnosticDescriptors.DuplicateBinding,
                Location = duplicate.First().Location,
                MessageArguments = new object[] { commandName, duplicate.Key }
            });
        }

        foreach (var duplicate in properties.Where(static property => property.Position.HasValue).GroupBy(static property => property.Position!.Value).Where(static group => group.Count() > 1))
        {
            diagnostics.Add(new DiagnosticInfo
            {
                Descriptor = DiagnosticDescriptors.DuplicateBinding,
                Location = duplicate.First().Location,
                MessageArguments = new object[] { commandName, $"position {duplicate.Key}" }
            });
        }
    }

    private static string EmitCommand(CommandModel model)
    {
        var builder = new StringBuilder();
        builder.AppendLine("using System;");
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine("using System.Globalization;");
        builder.AppendLine("using System.IO;");
        builder.AppendLine("#nullable enable");
        builder.AppendLine();
        if (!string.IsNullOrWhiteSpace(model.NamespaceName))
        {
            builder.Append("namespace ").Append(model.NamespaceName).AppendLine(";");
            builder.AppendLine();
        }

        builder.Append("internal static class ").Append(model.CompanionTypeName).AppendLine();
        builder.AppendLine("{");
        EmitParseMethod(builder, model);
        EmitWriteUsageMethod(builder, model);
        EmitWriteHelpMethod(builder, model);
        EmitSupportMethods(builder, model);
        builder.AppendLine("}");
        return builder.ToString();
    }

    private static void EmitParseMethod(StringBuilder builder, CommandModel model)
    {
        builder.Append("    public static global::CommandlineHelper.CommandParseResult<").Append(model.OptionsTypeQualifiedName).AppendLine("> Parse(string[] args)");
        builder.AppendLine("    {");
        builder.AppendLine("        if (args == null)");
        builder.AppendLine("            throw new global::System.ArgumentNullException(nameof(args));");
        builder.AppendLine();

        foreach (var property in model.Properties)
        {
            if (property.IsCollection)
            {
                builder.Append("        var ").Append(ToLocalName(property.PropertyName)).Append("Values = new global::System.Collections.Generic.List<").Append(property.AssignmentTypeQualifiedName).AppendLine(">();");
            }
            else
            {
                builder.Append("        var ").Append(ToLocalName(property.PropertyName)).Append(" = ").Append(GetInitialVariableValue(property)).AppendLine(";");
            }

            if (property.BindingKind != BindingKind.Flag && !property.IsCollection && property.BindingKind != BindingKind.Argument)
            {
                builder.Append("        var has").Append(property.PropertyName).AppendLine(" = false;");
            }
        }

        var positionalProperty = model.Properties.FirstOrDefault(static property => property.BindingKind == BindingKind.Argument);
        if (positionalProperty is not null && !positionalProperty.IsCollection)
        {
            builder.Append("        var has").Append(positionalProperty.PropertyName).AppendLine(" = false;");
        }

        builder.AppendLine();
        builder.AppendLine("        for (var i = 0; i < args.Length; i++)");
        builder.AppendLine("        {");
        builder.AppendLine("            var arg = args[i];");
        builder.AppendLine("            if (string.Equals(arg, \"--help\", StringComparison.OrdinalIgnoreCase) || string.Equals(arg, \"-h\", StringComparison.OrdinalIgnoreCase))");
        builder.AppendLine("                return global::CommandlineHelper.CommandParseResult<" + model.OptionsTypeQualifiedName + ">.FromHelpRequest();");
        builder.AppendLine();
        builder.AppendLine("            switch (arg)");
        builder.AppendLine("            {");

        foreach (var property in model.Properties.Where(static property => property.BindingKind != BindingKind.Argument))
        {
            var labels = new List<string>();
            if (!string.IsNullOrWhiteSpace(property.LongName))
                labels.Add(property.LongName!);
            if (!string.IsNullOrWhiteSpace(property.ShortName))
                labels.Add(property.ShortName!);

            foreach (var label in labels)
            {
                builder.Append("                case \"").Append(EscapeString(label)).AppendLine("\":");
            }

            builder.AppendLine("                {");

            if (property.BindingKind == BindingKind.Flag)
            {
                builder.Append("                    ").Append(ToLocalName(property.PropertyName)).AppendLine(" = true;");
                builder.AppendLine("                    break;");
                builder.AppendLine("                }");
                continue;
            }

            builder.AppendLine("                    if (i + 1 >= args.Length)");
            builder.Append("                        return global::CommandlineHelper.CommandParseResult<").Append(model.OptionsTypeQualifiedName).Append(">.FromError(\"Missing value for '").Append(EscapeString(property.LongName ?? property.ShortName ?? property.PropertyName)).AppendLine("'.\");");
            builder.AppendLine();
            builder.AppendLine("                    i++;" );
            EmitConversion(builder, property, "args[i]", "convertedValue", model.OptionsTypeQualifiedName, indent: 20);
            if (property.IsCollection)
            {
                builder.Append("                    ").Append(ToLocalName(property.PropertyName)).AppendLine("Values.Add(convertedValue);");
            }
            else
            {
                builder.Append("                    ").Append(ToLocalName(property.PropertyName)).AppendLine(" = convertedValue;");
                builder.Append("                    has").Append(property.PropertyName).AppendLine(" = true;");
            }
            builder.AppendLine("                    break;");
            builder.AppendLine("                }");
        }

        builder.AppendLine("                default:");
        if (positionalProperty is not null)
        {
            builder.Append("                    if (!has").Append(positionalProperty.PropertyName).AppendLine(")");
            builder.AppendLine("                    {");
            EmitConversion(builder, positionalProperty, "arg", "convertedValue", model.OptionsTypeQualifiedName, indent: 24);
            builder.Append("                        ").Append(ToLocalName(positionalProperty.PropertyName)).AppendLine(" = convertedValue;");
            builder.Append("                        has").Append(positionalProperty.PropertyName).AppendLine(" = true;");
            builder.AppendLine("                        break;");
            builder.AppendLine("                    }");
        }
        builder.Append("                    return global::CommandlineHelper.CommandParseResult<").Append(model.OptionsTypeQualifiedName).AppendLine(">.FromError($\"Unknown argument '{arg}'.\");");
        builder.AppendLine("            }");
        builder.AppendLine("        }");
        builder.AppendLine();

        foreach (var property in model.Properties)
        {
            if (!property.IsRequired)
                continue;

            if (property.IsCollection)
            {
                builder.Append("        if (").Append(ToLocalName(property.PropertyName)).AppendLine("Values.Count == 0)");
                builder.Append("            return global::CommandlineHelper.CommandParseResult<").Append(model.OptionsTypeQualifiedName).Append(">.FromError(\"Missing required argument '").Append(EscapeString(property.LongName ?? property.PropertyName)).AppendLine("'.\");");
            }
            else if (property.BindingKind == BindingKind.Flag)
            {
                continue;
            }
            else
            {
                builder.Append("        if (!has").Append(property.PropertyName).AppendLine(")");
                builder.Append("            return global::CommandlineHelper.CommandParseResult<").Append(model.OptionsTypeQualifiedName).Append(">.FromError(\"Missing required argument '").Append(EscapeString(property.LongName ?? property.PropertyName)).AppendLine("'.\");");
            }
        }

        builder.AppendLine();
        builder.Append("        var options = new ").Append(model.OptionsTypeQualifiedName).AppendLine();
        builder.AppendLine("        {");
        for (var i = 0; i < model.Properties.Count; i++)
        {
            var property = model.Properties[i];
            builder.Append("            ").Append(property.PropertyName).Append(" = ");
            if (property.IsCollection)
            {
                builder.Append(GetCollectionAssignmentExpression(property, ToLocalName(property.PropertyName) + "Values"));
            }
            else if (property.BindingKind == BindingKind.Flag)
            {
                builder.Append(ToLocalName(property.PropertyName));
            }
            else
            {
                builder.Append("has").Append(property.PropertyName).Append(" ? ").Append(ToLocalName(property.PropertyName)).Append(" : ").Append(GetEffectiveDefaultExpression(property));
            }

            builder.AppendLine(i == model.Properties.Count - 1 ? string.Empty : ",");
        }
        builder.AppendLine("        };" );
        builder.AppendLine();
        builder.Append("        return global::CommandlineHelper.CommandParseResult<").Append(model.OptionsTypeQualifiedName).AppendLine(">.FromSuccess(options);");
        builder.AppendLine("    }");
        builder.AppendLine();
    }

    private static void EmitWriteUsageMethod(StringBuilder builder, CommandModel model)
    {
        var usage = BuildUsageSynopsis(model);
        builder.AppendLine("    public static void WriteUsage(TextWriter writer)");
        builder.AppendLine("    {");
        builder.AppendLine("        if (writer == null)");
        builder.AppendLine("            throw new global::System.ArgumentNullException(nameof(writer));");
        builder.AppendLine();
        builder.AppendLine("        writer.WriteLine(\"Usage:\");");
        builder.Append("        writer.WriteLine(\"").Append(EscapeString(usage)).AppendLine("\");");
        builder.AppendLine("    }");
        builder.AppendLine();
    }

    private static void EmitWriteHelpMethod(StringBuilder builder, CommandModel model)
    {
        builder.AppendLine("    public static void WriteHelp(TextWriter writer)");
        builder.AppendLine("    {");
        builder.AppendLine("        if (writer == null)");
        builder.AppendLine("            throw new global::System.ArgumentNullException(nameof(writer));");
        builder.AppendLine();
        builder.AppendLine("        WriteUsage(writer);");
        builder.AppendLine();

        EmitResourceWrite(builder, model.ResourceTypeQualifiedName, model.DescriptionResourceName, 8);
        builder.AppendLine("        writer.WriteLine();");
        builder.AppendLine("        writer.WriteLine(\"Options:\");");
        foreach (var property in model.Properties)
        {
            var signature = BuildOptionSignature(property);
            var propertyTextVariableName = "optionText_" + property.PropertyName;
            builder.Append("        writer.Write(\"  ").Append(EscapeString(signature)).AppendLine("\");");
            if (!string.IsNullOrWhiteSpace(property.ResourceTypeQualifiedName ?? model.ResourceTypeQualifiedName)
                && !string.IsNullOrWhiteSpace(property.DescriptionResourceName))
            {
                builder.Append("        var ").Append(propertyTextVariableName).Append(" = ResolveText(typeof(")
                    .Append(property.ResourceTypeQualifiedName ?? model.ResourceTypeQualifiedName)
                    .Append("), \"")
                    .Append(EscapeString(property.DescriptionResourceName!))
                    .AppendLine("\");");
                builder.Append("        if (!string.IsNullOrWhiteSpace(").Append(propertyTextVariableName).AppendLine("))");
                builder.AppendLine("        {");
                builder.AppendLine("            writer.Write(\" - \");");
                builder.Append("            writer.Write(").Append(propertyTextVariableName).AppendLine(");");
                builder.AppendLine("        }");
            }
            builder.AppendLine("        writer.WriteLine();");
        }
        builder.AppendLine();
        EmitResourceWrite(builder, model.ResourceTypeQualifiedName, model.HelpTextResourceName, 8);
        builder.AppendLine("    }");
        builder.AppendLine();
    }

    private static void EmitSupportMethods(StringBuilder builder, CommandModel model)
    {
        builder.AppendLine("    private static string? ResolveText(Type? resourceType, string? resourceName)");
        builder.AppendLine("    {");
        builder.AppendLine("        if (resourceType == null || string.IsNullOrWhiteSpace(resourceName))");
        builder.AppendLine("            return null;");
        builder.AppendLine();
        builder.AppendLine("        return global::CommandlineHelper.CommandTextResourceResolver.TryResolveString(resourceType, resourceName, out var value) ? value : null;");
        builder.AppendLine("    }");
        builder.AppendLine();
        builder.AppendLine("    private static string FormatEnumParseError(string argumentName, string rawValue)");
        builder.AppendLine("        => $\"Value '{rawValue}' is not valid for '{argumentName}'.\";");
        builder.AppendLine();
        builder.AppendLine("    private static string FormatValueParseError(string argumentName, string rawValue)");
        builder.AppendLine("        => $\"Value '{rawValue}' is not valid for '{argumentName}'.\";");
    }

    private static void EmitResourceWrite(StringBuilder builder, string? resourceTypeQualifiedName, string? resourceName, int indent, string? inlinePrefix = null)
    {
        if (string.IsNullOrWhiteSpace(resourceTypeQualifiedName) || string.IsNullOrWhiteSpace(resourceName))
            return;

        var padding = new string(' ', indent);
        var textVariableName = "text_" + Math.Abs((resourceTypeQualifiedName + "_" + resourceName).GetHashCode()).ToString(CultureInfo.InvariantCulture);
        builder.Append(padding).Append("var ").Append(textVariableName).Append(" = ResolveText(typeof(").Append(resourceTypeQualifiedName).Append("), \"").Append(EscapeString(resourceName!)).AppendLine("\");");
        builder.Append(padding).Append("if (!string.IsNullOrWhiteSpace(").Append(textVariableName).AppendLine("))");
        builder.Append(padding).AppendLine("{");
        builder.Append(padding).Append("    ").Append(inlinePrefix ?? "writer.WriteLine").Append("(").Append(textVariableName).AppendLine(");");
        builder.Append(padding).AppendLine("}");
    }

    private static void EmitConversion(StringBuilder builder, PropertyModel property, string rawExpression, string localName, string optionsTypeQualifiedName, int indent)
    {
        var padding = new string(' ', indent);
        switch (property.ValueKind)
        {
            case ValueKind.String:
                builder.Append(padding).Append("var ").Append(localName).Append(" = ").Append(rawExpression).AppendLine(";");
                break;
            case ValueKind.Int32:
                builder.Append(padding).Append("if (!int.TryParse(").Append(rawExpression).Append(", NumberStyles.Integer, CultureInfo.InvariantCulture, out var ").Append(localName).AppendLine("))");
                builder.Append(padding).Append("    return global::CommandlineHelper.CommandParseResult<").Append(optionsTypeQualifiedName).Append(">.FromError(FormatValueParseError(\"").Append(EscapeString(property.LongName ?? property.PropertyName)).Append("\", ").Append(rawExpression).AppendLine("));");
                break;
            case ValueKind.Int64:
                builder.Append(padding).Append("if (!long.TryParse(").Append(rawExpression).Append(", NumberStyles.Integer, CultureInfo.InvariantCulture, out var ").Append(localName).AppendLine("))");
                builder.Append(padding).Append("    return global::CommandlineHelper.CommandParseResult<").Append(optionsTypeQualifiedName).Append(">.FromError(FormatValueParseError(\"").Append(EscapeString(property.LongName ?? property.PropertyName)).Append("\", ").Append(rawExpression).AppendLine("));");
                break;
            case ValueKind.Boolean:
                builder.Append(padding).Append("if (!bool.TryParse(").Append(rawExpression).Append(", out var ").Append(localName).AppendLine("))");
                builder.Append(padding).Append("    return global::CommandlineHelper.CommandParseResult<").Append(optionsTypeQualifiedName).Append(">.FromError(FormatValueParseError(\"").Append(EscapeString(property.LongName ?? property.PropertyName)).Append("\", ").Append(rawExpression).AppendLine("));");
                break;
            case ValueKind.Enum:
                builder.Append(padding).Append("if (!Enum.TryParse<").Append(property.AssignmentTypeQualifiedName).Append(">( ").Append(rawExpression).Append(", true, out var ").Append(localName).AppendLine("))");
                builder.Append(padding).Append("    return global::CommandlineHelper.CommandParseResult<").Append(optionsTypeQualifiedName).Append(">.FromError(FormatEnumParseError(\"").Append(EscapeString(property.LongName ?? property.PropertyName)).Append("\", ").Append(rawExpression).AppendLine("));");
                break;
        }
    }

    private static string BuildUsageSynopsis(CommandModel model)
    {
        var builder = new StringBuilder();
        builder.Append("  ");
        builder.Append(model.CommandName);
        foreach (var property in model.Properties.Where(static property => property.BindingKind == BindingKind.Argument))
        {
            builder.Append(' ');
            builder.Append(property.IsRequired ? '<' : '[');
            builder.Append(property.PropertyName.ToLowerInvariant());
            builder.Append(property.IsRequired ? '>' : ']');
        }

        foreach (var property in model.Properties.Where(static property => property.BindingKind != BindingKind.Argument))
        {
            builder.Append(' ');
            if (!property.IsRequired)
                builder.Append('[');
            builder.Append(property.LongName ?? property.ShortName ?? property.PropertyName);
            if (property.BindingKind != BindingKind.Flag)
            {
                builder.Append(" <value>");
                if (property.IsCollection)
                    builder.Append("...");
            }
            if (!property.IsRequired)
                builder.Append(']');
        }

        return builder.ToString();
    }

    private static string BuildOptionSignature(PropertyModel property)
    {
        if (property.BindingKind == BindingKind.Argument)
            return property.IsRequired ? $"<{property.PropertyName.ToLowerInvariant()}>" : $"[{property.PropertyName.ToLowerInvariant()}]";

        var builder = new StringBuilder();
        builder.Append(property.LongName);
        if (!string.IsNullOrWhiteSpace(property.ShortName))
            builder.Append(", ").Append(property.ShortName);
        if (property.BindingKind != BindingKind.Flag)
        {
            builder.Append(" <value>");
            if (property.IsCollection)
                builder.Append(" repeated");
        }
        if (property.IsRequired)
            builder.Append(" required");
        return builder.ToString();
    }

    private static string GetInitialVariableValue(PropertyModel property)
    {
        if (property.BindingKind == BindingKind.Flag)
            return property.ExplicitDefaultValueCode ?? "false";

        return GetTypeDefaultExpression(property.PropertyTypeQualifiedName);
    }

    private static string GetEffectiveDefaultExpression(PropertyModel property)
    {
        if (!string.IsNullOrWhiteSpace(property.ExplicitDefaultValueCode))
            return property.ExplicitDefaultValueCode!;

        if (!string.IsNullOrWhiteSpace(property.InferredDefaultValueCode))
            return property.InferredDefaultValueCode!;

        if (property.UsesEmptyCollectionDefault)
            return GetCollectionAssignmentExpression(property, "new global::System.Collections.Generic.List<" + property.AssignmentTypeQualifiedName + ">()");

        return GetTypeDefaultExpression(property.PropertyTypeQualifiedName);
    }

    private static string GetCollectionAssignmentExpression(PropertyModel property, string sourceExpression)
    {
        return property.CollectionKind switch
        {
            CollectionKind.Array => sourceExpression + ".ToArray()",
            CollectionKind.List => sourceExpression,
            CollectionKind.InterfaceList => sourceExpression,
            CollectionKind.InterfaceReadOnlyList => sourceExpression + ".ToArray()",
            _ => sourceExpression
        };
    }

    private static string GetTypeDefaultExpression(string typeName)
        => "default(" + typeName + ")";

    private static CollectionInfo? TryGetCollectionInfo(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is IArrayTypeSymbol arrayType)
            return new CollectionInfo(arrayType.ElementType, CollectionKind.Array);

        if (typeSymbol is not INamedTypeSymbol namedType || !namedType.IsGenericType || namedType.TypeArguments.Length != 1)
            return null;

        var metadataName = namedType.ConstructedFrom.ToDisplayString();
        return metadataName switch
        {
            "System.Collections.Generic.List<T>" => new CollectionInfo(namedType.TypeArguments[0], CollectionKind.List),
            "System.Collections.Generic.IList<T>" => new CollectionInfo(namedType.TypeArguments[0], CollectionKind.InterfaceList),
            "System.Collections.Generic.IReadOnlyList<T>" => new CollectionInfo(namedType.TypeArguments[0], CollectionKind.InterfaceReadOnlyList),
            _ => null
        };
    }

    private static bool TryGetValueKind(ITypeSymbol typeSymbol, out ValueKind valueKind)
    {
        switch (typeSymbol.SpecialType)
        {
            case SpecialType.System_String:
                valueKind = ValueKind.String;
                return true;
            case SpecialType.System_Int32:
                valueKind = ValueKind.Int32;
                return true;
            case SpecialType.System_Int64:
                valueKind = ValueKind.Int64;
                return true;
            case SpecialType.System_Boolean:
                valueKind = ValueKind.Boolean;
                return true;
        }

        if (typeSymbol.TypeKind == TypeKind.Enum)
        {
            valueKind = ValueKind.Enum;
            return true;
        }

        valueKind = default;
        return false;
    }

    private static ITypeSymbol UnwrapNullable(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is INamedTypeSymbol namedType
            && namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T
            && namedType.TypeArguments.Length == 1)
        {
            return namedType.TypeArguments[0];
        }

        return typeSymbol;
    }

    private static AttributeData? GetAttribute(ISymbol symbol, string fullName)
        => symbol.GetAttributes().FirstOrDefault(attribute => attribute.AttributeClass?.ToDisplayString() == fullName);

    private static BindingKind? GetBindingKind(AttributeData attribute)
    {
        var name = attribute.AttributeClass?.ToDisplayString();
        if (name == CommandOptionAttributeName)
            return BindingKind.Option;
        if (name == CommandFlagAttributeName)
            return BindingKind.Flag;
        if (name == CommandArgumentAttributeName)
            return BindingKind.Argument;
        return null;
    }

    private static string? GetConstructorStringArgument(AttributeData attribute)
        => attribute.ConstructorArguments.Length == 1 ? attribute.ConstructorArguments[0].Value as string : null;

    private static string? GetNamedStringArgument(AttributeData attribute, string name)
        => attribute.NamedArguments.FirstOrDefault(pair => pair.Key == name).Value.Value as string;

    private static bool? GetNamedBooleanArgument(AttributeData attribute, string name)
        => attribute.NamedArguments.FirstOrDefault(pair => pair.Key == name).Value.Value as bool?;

    private static INamedTypeSymbol? GetNamedTypeArgument(AttributeData attribute, string name)
        => attribute.NamedArguments.FirstOrDefault(pair => pair.Key == name).Value.Value as INamedTypeSymbol;

    private static string? TryGetExplicitDefaultValueCode(AttributeData attribute, IPropertySymbol propertySymbol, ValueKind valueKind, CollectionInfo? collectionInfo, List<DiagnosticInfo> diagnostics)
    {
        var namedArgument = attribute.NamedArguments.FirstOrDefault(pair => pair.Key == "DefaultValue");
        if (namedArgument.Key is null)
            return null;

        if (collectionInfo is not null)
        {
            diagnostics.Add(new DiagnosticInfo
            {
                Descriptor = DiagnosticDescriptors.InvalidDefaultValue,
                Location = propertySymbol.Locations.FirstOrDefault(),
                MessageArguments = new object[] { propertySymbol.Name }
            });
            return null;
        }

        var value = namedArgument.Value;
        if (value.IsNull)
            return "null";

        try
        {
            return valueKind switch
            {
                ValueKind.String => "\"" + EscapeString((string?)value.Value ?? string.Empty) + "\"",
                ValueKind.Int32 => Convert.ToString(value.Value, CultureInfo.InvariantCulture),
                ValueKind.Int64 => Convert.ToString(value.Value, CultureInfo.InvariantCulture) + "L",
                ValueKind.Boolean => ((bool)value.Value!).ToString().ToLowerInvariant(),
                ValueKind.Enum => value.Value is null ? null : propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) + "." + value.Value,
                _ => null
            };
        }
        catch
        {
            diagnostics.Add(new DiagnosticInfo
            {
                Descriptor = DiagnosticDescriptors.InvalidDefaultValue,
                Location = propertySymbol.Locations.FirstOrDefault(),
                MessageArguments = new object[] { propertySymbol.Name }
            });
            return null;
        }
    }

    private static string? TryGetInferredDefaultValueCode(IPropertySymbol propertySymbol, PropertyDeclarationSyntax? propertyDeclaration, CollectionInfo? collectionInfo, ValueKind valueKind, List<DiagnosticInfo> diagnostics)
    {
        if (collectionInfo is not null)
            return null;

        var initializer = propertyDeclaration?.Initializer?.Value;
        if (initializer is null)
            return null;

        switch (initializer)
        {
            case LiteralExpressionSyntax:
            case PrefixUnaryExpressionSyntax:
            case MemberAccessExpressionSyntax:
            case IdentifierNameSyntax:
            case InvocationExpressionSyntax:
                return initializer.ToString();
            default:
                diagnostics.Add(new DiagnosticInfo
                {
                    Descriptor = DiagnosticDescriptors.InvalidDefaultValue,
                    Location = propertySymbol.Locations.FirstOrDefault(),
                    MessageArguments = new object[] { propertySymbol.Name }
                });
                return null;
        }
    }

    private static void ValidateResourceReference(INamedTypeSymbol? resourceType, string? resourceName, Location? location, List<DiagnosticInfo> diagnostics)
    {
        if (resourceType is null || string.IsNullOrWhiteSpace(resourceName))
            return;

        var hasMatchingProperty = resourceType.GetMembers(resourceName).OfType<IPropertySymbol>().Any(static property => property.IsStatic && property.Type.SpecialType == SpecialType.System_String);
        if (hasMatchingProperty)
            return;

        diagnostics.Add(new DiagnosticInfo
        {
            Descriptor = DiagnosticDescriptors.UnresolvedResourceMember,
            Location = location,
            MessageArguments = new object[] { resourceName!, resourceType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) }
        });
    }

    private static string ToLocalName(string propertyName)
        => char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1);

    private static string EscapeString(string value)
        => value.Replace("\\", "\\\\").Replace("\"", "\\\"");

    private sealed class CollectionInfo
    {
        public CollectionInfo(ITypeSymbol elementType, CollectionKind kind)
        {
            ElementType = elementType;
            Kind = kind;
        }

        public ITypeSymbol ElementType { get; }

        public CollectionKind Kind { get; }
    }
}
