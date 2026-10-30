using IDR.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IDR.Cli;

public sealed partial class PascalClassEmitter
{
    private static readonly HashSet<string> ReservedWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "absolute", "and", "array", "as", "asm", "begin", "case", "class", "const",
        "constructor", "destructor", "dispinterface", "div", "do", "downto", "else",
        "end", "except", "exports", "file", "finalization", "finally", "for", "function",
        "goto", "if", "implementation", "in", "inherited", "initialization", "inline",
        "interface", "is", "label", "library", "mod", "nil", "not", "object", "of",
        "or", "out", "packed", "procedure", "program", "property", "raise", "record",
        "repeat", "resourcestring", "set", "shl", "shr", "string", "then", "threadvar",
        "to", "try", "type", "unit", "until", "uses", "var", "while", "with", "xor"
    };

    public string Emit(AnalysisSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        AnalysisItem[] classes = session.Items.Values
            .Where(item => item.Flags.HasFlag(AnalysisFlags.Vmt) && !string.IsNullOrWhiteSpace(item.Name))
            .OrderBy(item => item.Address)
            .ToArray();
        StringBuilder output = new();
        output.AppendLine("{ Generated from bounded IDR Delphi VMT/RTTI analysis; declarations are candidates. }");
        output.AppendLine();
        output.AppendLine("type");

        HashSet<string> emittedNames = new(StringComparer.OrdinalIgnoreCase);
        Dictionary<uint, string> classNamesByAddress = [];
        foreach (AnalysisItem classItem in classes)
        {
            classNamesByAddress.Add(
                classItem.Address,
                UniqueIdentifier(
                    classItem.Name!,
                    $"TClass_{classItem.Address:X8}",
                    emittedNames));
        }

        foreach (AnalysisItem classItem in classes)
        {
            string className = classNamesByAddress[classItem.Address];
            string? parentName = string.Equals(
                    className,
                    "TObject",
                    StringComparison.OrdinalIgnoreCase)
                || classItem.ParentAddress == classItem.Address
                    ? null
                    : "TObject";
            if (classItem.ParentAddress is uint parentAddress
                && parentAddress != classItem.Address
                && classNamesByAddress.TryGetValue(parentAddress, out string? emittedParentName))
            {
                parentName = emittedParentName;
            }
            else if (classItem.ParentAddress is uint unresolvedParentAddress
                && unresolvedParentAddress != classItem.Address
                && session.Items.TryGetValue(unresolvedParentAddress, out AnalysisItem? parent)
                && !string.IsNullOrWhiteSpace(parent.Name))
            {
                parentName = ToIdentifier(parent.Name, $"TParent_{unresolvedParentAddress:X8}");
            }

            string inheritance = parentName is null ? "class" : $"class({parentName})";
            output.AppendLine($"  {className} = {inheritance}");
            output.AppendLine("  public");
            if (classItem.Fields.Count == 0)
            {
                output.AppendLine("    { No field declarations were recovered. }");
            }
            else
            {
                HashSet<string> fieldNames = new(StringComparer.OrdinalIgnoreCase);
                foreach (DelphiVmtField field in classItem.Fields.OrderBy(field => field.Offset))
                {
                    string fallback = $"Field_{field.Offset:X}";
                    string fieldName = UniqueIdentifier(field.Name, fallback, fieldNames);
                    string fieldType = ResolveFieldType(session, field.TypeInfoAddress);
                    string typeInfoEvidence = field.TypeInfoAddress is uint typeInfoAddress
                        ? $"TypeInfo RVA 0x{typeInfoAddress:X8}"
                        : "TypeInfo unavailable";
                    output.AppendLine(
                        $"    {fieldName}: {fieldType}; {{ +$ {field.Offset:X}; {typeInfoEvidence}; visibility unverified }}");
                }
            }

            HashSet<string> methodNames = new(StringComparer.OrdinalIgnoreCase);
            foreach (DelphiVmtMethod method in classItem.Methods.OrderBy(method => method.Name, StringComparer.Ordinal))
            {
                if (string.IsNullOrWhiteSpace(method.Name))
                {
                    continue;
                }

                string shortName = method.Name[(method.Name.LastIndexOf('.') + 1)..];
                string methodName = UniqueIdentifier(shortName, $"Method_{methodNames.Count}", methodNames);
                string kind = shortName.StartsWith("Create", StringComparison.OrdinalIgnoreCase)
                    ? "constructor"
                    : shortName.StartsWith("Destroy", StringComparison.OrdinalIgnoreCase)
                        ? "destructor"
                        : "procedure";
                string virtualText = method.VirtualIndex is null ? string.Empty : " virtual;";
                string address = method.CodeAddress is uint codeAddress
                    ? $"0x{codeAddress:X8}"
                    : "unresolved";
                output.AppendLine($"    {kind} {methodName};{virtualText} {{ target {address}; signature unknown }}");
            }

            foreach (DelphiVmtAutoMethod method in classItem.AutoMethods
                         .Where(method => method.IsMethod)
                         .OrderBy(method => method.DispatchId))
            {
                string methodName = UniqueIdentifier(method.Name, $"Dispatch_{method.DispatchId:X}", methodNames);
                string kind = methodName.StartsWith("Create", StringComparison.OrdinalIgnoreCase)
                    ? "constructor"
                    : methodName.StartsWith("Destroy", StringComparison.OrdinalIgnoreCase)
                        ? "destructor"
                        : "procedure";
                string virtualText = method.IsVirtual ? " virtual;" : string.Empty;
                output.AppendLine(
                    $"    {kind} {methodName};{virtualText} {{ dispatch {method.DispatchId}; target 0x{method.CodeAddress:X8}; signature unknown }}");
            }

            foreach (DelphiVmtDynamicMethod method in classItem.DynamicMethods.OrderBy(method => method.MessageId))
            {
                string target = method.CodeAddress is uint codeAddress
                    ? $"0x{codeAddress:X8}"
                    : "unresolved";
                output.AppendLine($"    {{ dynamic message {method.MessageId}; target {target}; name unknown }}");
            }

            foreach (DelphiVmtVirtualMethod method in classItem.VirtualMethods.OrderBy(method => method.SlotOffset))
            {
                output.AppendLine(
                    $"    {{ virtual slot +$ {method.SlotOffset:X}; target 0x{method.CodeAddress:X8}; name unknown }}");
            }

            output.AppendLine($"  end; {{ VMT RVA 0x{classItem.Address:X8}; instance size {classItem.ClassInstanceSizeBytes?.ToString() ?? "unknown"} }}");
            output.AppendLine();
        }

        if (classes.Length == 0)
        {
            output.AppendLine("  { No named VMT classes were detected. }");
        }

        return output.ToString();
    }

    private static string ResolveFieldType(AnalysisSession session, uint? typeInfoAddress)
    {
        if (typeInfoAddress is uint address
            && session.Items.TryGetValue(address, out AnalysisItem? typeInfo)
            && IsSafeTypeName(typeInfo.Name))
        {
            return typeInfo.Name!;
        }

        return "Pointer";
    }

    private static string UniqueIdentifier(string source, string fallback, HashSet<string> used)
    {
        string candidate = ToIdentifier(source, fallback);
        string baseName = candidate;
        int suffix = 2;
        while (!used.Add(candidate))
        {
            candidate = $"{baseName}_{suffix++}";
        }

        return candidate;
    }

    private static string ToIdentifier(string source, string fallback)
    {
        string normalized = IdentifierPattern().Replace(source, "_");
        normalized = normalized.Trim('_');
        if (normalized.Length == 0 || !IsIdentifierStart(normalized[0]))
        {
            normalized = fallback;
        }

        if (ReservedWords.Contains(normalized))
        {
            normalized = $"&{normalized}";
        }

        return normalized;
    }

    private static bool IsSafeTypeName(string? value) =>
        value is not null
        && TypeNamePattern().IsMatch(value)
        && !ReservedWords.Contains(value);

    private static bool IsIdentifierStart(char value) =>
        char.IsLetter(value) || value == '_';

    [GeneratedRegex("[^A-Za-z0-9_]", RegexOptions.CultureInvariant)]
    private static partial Regex IdentifierPattern();

    [GeneratedRegex("^[A-Za-z_][A-Za-z0-9_]*(\\.[A-Za-z_][A-Za-z0-9_]*)*$", RegexOptions.CultureInvariant)]
    private static partial Regex TypeNamePattern();
}
