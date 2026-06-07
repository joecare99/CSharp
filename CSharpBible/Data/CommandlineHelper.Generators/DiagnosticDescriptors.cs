using Microsoft.CodeAnalysis;

namespace CommandlineHelper.Generators;

internal static class DiagnosticDescriptors
{
    private const string Category = "CommandlineHelper";

    public static readonly DiagnosticDescriptor InvalidCommandDeclaration = new(
        "CH001",
        "Invalid command declaration",
        "Command declaration on '{0}' is invalid: {1}",
        Category,
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor UnsupportedPropertyType = new(
        "CH002",
        "Unsupported property type",
        "Property '{0}' uses unsupported command binding type '{1}'.",
        Category,
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor InvalidFlagDeclaration = new(
        "CH003",
        "Invalid flag declaration",
        "Property '{0}' uses CommandFlagAttribute but is not a supported boolean property.",
        Category,
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor InvalidArgumentPosition = new(
        "CH004",
        "Invalid argument position",
        "Property '{0}' uses positional argument position '{1}', but only position 0 is supported in the first slice.",
        Category,
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor DuplicateBinding = new(
        "CH005",
        "Duplicate command binding",
        "Command '{0}' contains duplicate binding '{1}'.",
        Category,
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor UnresolvedResourceMember = new(
        "CH006",
        "Unresolved resource member",
        "Resource member '{0}' could not be resolved on '{1}'.",
        Category,
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor InvalidDefaultValue = new(
        "CH007",
        "Invalid default value",
        "Property '{0}' declares an unsupported or incompatible default value.",
        Category,
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor UnsupportedCollectionTarget = new(
        "CH008",
        "Unsupported collection target",
        "Property '{0}' uses unsupported collection target type '{1}'.",
        Category,
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor MultipleBindingAttributes = new(
        "CH009",
        "Multiple command binding attributes",
        "Property '{0}' declares multiple command binding attributes. Exactly one binding attribute is allowed.",
        Category,
        DiagnosticSeverity.Error,
        true);
}
