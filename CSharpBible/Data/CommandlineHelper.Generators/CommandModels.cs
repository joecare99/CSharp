using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace CommandlineHelper.Generators;

internal sealed class CommandModel
{
    public string CommandName { get; set; } = string.Empty;
    public string NamespaceName { get; set; } = string.Empty;
    public string OptionsTypeName { get; set; } = string.Empty;
    public string OptionsTypeQualifiedName { get; set; } = string.Empty;
    public string CompanionTypeName { get; set; } = string.Empty;
    public string? ResourceTypeQualifiedName { get; set; }
    public string? DescriptionResourceName { get; set; }
    public string? HelpTextResourceName { get; set; }
    public Location? Location { get; set; }
    public List<PropertyModel> Properties { get; set; } = new();
    public List<DiagnosticInfo> Diagnostics { get; set; } = new();
}

internal sealed class PropertyModel
{
    public string PropertyName { get; set; } = string.Empty;
    public string PropertyTypeQualifiedName { get; set; } = string.Empty;
    public string AssignmentTypeQualifiedName { get; set; } = string.Empty;
    public BindingKind BindingKind { get; set; }
    public ValueKind ValueKind { get; set; }
    public bool IsNullable { get; set; }
    public bool IsCollection { get; set; }
    public string? ElementTypeQualifiedName { get; set; }
    public CollectionKind CollectionKind { get; set; }
    public string? LongName { get; set; }
    public string? ShortName { get; set; }
    public int? Position { get; set; }
    public bool IsRequired { get; set; }
    public string? ExplicitDefaultValueCode { get; set; }
    public string? InferredDefaultValueCode { get; set; }
    public bool UsesEmptyCollectionDefault { get; set; }
    public string? ResourceTypeQualifiedName { get; set; }
    public string? DescriptionResourceName { get; set; }
    public Location? Location { get; set; }
}

internal sealed class DiagnosticInfo
{
    public DiagnosticDescriptor Descriptor { get; set; } = null!;
    public Location? Location { get; set; }
    public object[] MessageArguments { get; set; } = [];
}

internal enum BindingKind
{
    Option,
    Flag,
    Argument
}

internal enum ValueKind
{
    String,
    Int32,
    Int64,
    Boolean,
    Enum
}

internal enum CollectionKind
{
    None,
    Array,
    List,
    InterfaceList,
    InterfaceReadOnlyList
}
