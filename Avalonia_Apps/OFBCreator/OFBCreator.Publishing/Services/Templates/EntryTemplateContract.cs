using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OFBCreator.Publishing.Models;

namespace OFBCreator.Publishing.Services.Templates;

internal static class EntryTemplateContract
{
    internal static class EntryRoot
    {
        public const string Family = "Family";
        public const string Individual = "Individual";
    }

    internal static class PathOperator
    {
        public const string Any = "any";
        public const string One = "one";
        public const char Negation = '!';
    }

    internal static class Block
    {
        public const string Section = "section";
        public const string Paragraph = "paragraph";
        public const string Text = "text";
        public const string Field = "field";
        public const string Link = "link";
        public const string If = "if";
        public const string ForEach = "forEach";
        public const string Include = "include";
    }

    internal static class Model
    {
        public const string Family = "family";
        public const string Person = "person";
        public const string PersonEvent = "personEvent";
        public const string FamilyReference = "familyReference";
        public const string FamilyReferenceToken = "familyReferenceToken";
        public const string Occupation = "occupation";
        public const string Property = "property";
    }

    internal static class Variable
    {
        public const string Family = "family";
        public const string Person = "person";
        public const string Child = "child";
        public const string Individual = "individual";
        public const string Occupation = "occupation";
        public const string Property = "property";
        public const string FamilyReference = "familyReference";
        public const string FamilyReferenceToken = "familyReferenceToken";
        public const string PersonEvent = "personEvent";
    }

    internal static class Role
    {
        public const string SectionHeading = "section-heading";
        public const string FamilyHeader = "family-header";
        public const string FamilyData = "family-data";
        public const string Adult = "adult";
        public const string ChildrenHeader = "children-header";
        public const string Child = "child";
        public const string Note = "note";
        public const string FamilyEntry = "FamilyEntry";
        public const string ParentEntry = "ParentEntry";
        public const string ParentLifeEvents = "ParentLifeEvents";
        public const string ChildHeader = "ChildHeader";
        public const string ChildEntry = "ChildEntry";
        public const string ChildLifeEvents = "ChildLifeEvents";
        public const string IndexEntry = "IndexEntry";
    }

    internal static class Formatter
    {
        public const string ChildCount = "childCount";
        public const string DateSuffix = "dateSuffix";
        public const string GedcomDatePrefix = "gedcomDatePrefix";
        public const string Ordinal = "ordinal";
        public const string VitalEvents = "vitalEvents";
    }

    private static readonly IReadOnlyDictionary<string, Type> ModelTypes =
        new Dictionary<string, Type>(StringComparer.Ordinal)
        {
            [Model.Family] = typeof(FamilyEntryTemplateModel),
            [Model.Person] = typeof(PersonEntryTemplateModel),
            [Model.PersonEvent] = typeof(PersonEventEntryTemplateModel),
            [Model.FamilyReference] = typeof(FamilyReferenceEntryTemplateModel),
            [Model.FamilyReferenceToken] = typeof(FamilyReferenceTokenModel),
            [Model.Occupation] = typeof(OccupationEntryTemplateModel),
            [Model.Property] = typeof(PropertyEntryTemplateModel)
        };

    private static readonly IReadOnlyDictionary<(Type Model, string Alias), string> PropertyAliases =
        new Dictionary<(Type Model, string Alias), string>
        {
            [(typeof(PersonEntryTemplateModel), "vitalEventsGc")] = nameof(PersonEntryTemplateModel.VitalEvents),
            [(typeof(PersonEntryTemplateModel), "vitalEventsAk")] = nameof(PersonEntryTemplateModel.VitalEvents),
            [(typeof(PersonEventEntryTemplateModel), "relatedPerson")] = nameof(PersonEventEntryTemplateModel.HasRelatedPerson),
            [(typeof(PersonEventEntryTemplateModel), "relatedFamily")] = nameof(PersonEventEntryTemplateModel.HasRelatedFamily)
        };

    public static string ToCamelCase(string propertyName) =>
        string.IsNullOrEmpty(propertyName)
            ? propertyName
            : char.ToLowerInvariant(propertyName[0]) + propertyName[1..];

    public static Type? GetModelType(string modelName) =>
        ModelTypes.TryGetValue(modelName, out var modelType) ? modelType : null;

    public static string? GetModelName(Type modelType) =>
        ModelTypes.FirstOrDefault(pair => pair.Value == modelType).Key;

    public static PropertyInfo? GetProperty(Type modelType, string templateName)
    {
        if (!ModelTypes.Values.Contains(modelType))
            return null;

        var propertyName = PropertyAliases.TryGetValue((modelType, templateName), out var alias)
            ? alias
            : null;
        return modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(property => property.CanRead
                && (propertyName is not null
                    ? property.Name == propertyName
                    : string.Equals(ToCamelCase(property.Name), templateName, StringComparison.Ordinal)));
    }

    public static bool TryGetCollectionElementType(Type collectionType, out Type? elementType)
    {
        if (collectionType == typeof(string))
        {
            elementType = null;
            return false;
        }

        if (collectionType.IsArray)
        {
            elementType = collectionType.GetElementType();
            return elementType is not null;
        }

        var enumerableType = collectionType.GetInterfaces()
            .Append(collectionType)
            .FirstOrDefault(type => type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        if (enumerableType is not null)
        {
            elementType = enumerableType.GetGenericArguments()[0];
            return true;
        }

        elementType = null;
        return false;
    }

    public static bool IsScalar(Type valueType)
    {
        var type = Nullable.GetUnderlyingType(valueType) ?? valueType;
        return type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal);
    }
}
