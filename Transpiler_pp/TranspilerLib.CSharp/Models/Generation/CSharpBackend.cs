#if NET8_0_OR_GREATER
using System;
using TranspilerLib.IEC.Models.Ast;

namespace TranspilerLib.CSharp.Models.Generation;

/// <summary>
/// Provides the first backend-facing entry point for generating C# source from the shared semantic model.
/// The backend currently delegates to the deterministic semantic emitter so the C# project can depend on
/// the common transpiler semantics layer without changing the existing scanner and optimizer workflow.
/// </summary>
public static class CSharpBackend
{
    /// <summary>
    /// Emits deterministic C# source for the supplied semantic compilation unit.
    /// </summary>
    /// <param name="compilationUnit">The semantic compilation unit to emit.</param>
    /// <param name="methodName">The generated method name.</param>
    /// <param name="options">Optional backend output-shaping options.</param>
    /// <returns>The generated C# source code.</returns>
    public static string EmitMethod(IecCompilationUnit compilationUnit, string methodName = "Execute", CSharpBackendOptions? options = null)
    {
        var result = IecCSharpEmitter.EmitMethod(compilationUnit, methodName);
        var effectiveOptions = options ?? new CSharpBackendOptions();
        result = ApplyArtifactMetadata(result, methodName, compilationUnit.ArtifactMetadata);
        if (!string.Equals(effectiveOptions.TypeName, "GeneratedIecProgram", StringComparison.Ordinal))
        {
            result = result.Replace("GeneratedIecProgram", effectiveOptions.TypeName, StringComparison.Ordinal);
        }

        if (!string.IsNullOrWhiteSpace(effectiveOptions.NamespaceName))
        {
            var namespaceName = effectiveOptions.NamespaceName.Trim();
            result = result.Replace("public ", $"namespace {namespaceName}{Environment.NewLine}{{{Environment.NewLine}public ", StringComparison.Ordinal);
            result = result.Replace("internal ", $"namespace {namespaceName}{Environment.NewLine}{{{Environment.NewLine}internal ", StringComparison.Ordinal);
            result = result.Replace("protected ", $"namespace {namespaceName}{Environment.NewLine}{{{Environment.NewLine}protected ", StringComparison.Ordinal);
            result = result.Replace("private ", $"namespace {namespaceName}{Environment.NewLine}{{{Environment.NewLine}private ", StringComparison.Ordinal);
            result += Environment.NewLine + "}";
        }

        return result;
    }

    private static string ApplyArtifactMetadata(string result, string methodName, IecArtifactMetadata artifactMetadata)
    {
        result = ApplyAccessibility(result, artifactMetadata.Accessibility);
        result = ApplyWrapperTraits(result, methodName, artifactMetadata);

        return result;
    }

    private static string ApplyAccessibility(string result, IecAccessibility accessibility)
    {
        return accessibility switch
        {
            IecAccessibility.Default or IecAccessibility.Public => result,
            IecAccessibility.Internal => result.Replace("public ", "internal ", StringComparison.Ordinal),
            IecAccessibility.Protected => result.Replace("public ", "protected ", StringComparison.Ordinal),
            IecAccessibility.Private => result.Replace("public ", "private ", StringComparison.Ordinal),
            _ => result,
        };
    }

    private static string ApplyWrapperTraits(string result, string methodName, IecArtifactMetadata artifactMetadata)
    {
        result = ApplyClassTraits(result, artifactMetadata.IsStatic, artifactMetadata.IsPartial);
        result = ApplyMethodStaticTrait(result, methodName, artifactMetadata.IsStatic);
        return result;
    }

    private static string ApplyClassTraits(string result, bool isStatic, bool isPartial)
    {
        foreach (var modifier in new[] { "public", "internal", "protected", "private" })
        {
            var originalDeclaration = $"{modifier} static class GeneratedIecProgram";
            var replacementDeclaration = BuildClassDeclaration(modifier, isStatic, isPartial);
            result = result.Replace(originalDeclaration, replacementDeclaration, StringComparison.Ordinal);
        }

        return result;
    }

    private static string BuildClassDeclaration(string accessibilityModifier, bool isStatic, bool isPartial)
    {
        var staticToken = isStatic ? " static" : " sealed";
        var partialToken = isPartial ? " partial" : string.Empty;
        return $"{accessibilityModifier}{staticToken}{partialToken} class GeneratedIecProgram";
    }

    private static string ApplyMethodStaticTrait(string result, string methodName, bool isStatic)
    {
        foreach (var modifier in new[] { "public", "internal", "protected", "private" })
        {
            foreach (var typeName in new[] { "void", "bool", "int", "double", "string", "object" })
            {
                var staticDeclaration = $"{modifier} static {typeName} {methodName}";
                var instanceDeclaration = $"{modifier} {typeName} {methodName}";
                result = isStatic
                    ? result.Replace(instanceDeclaration, staticDeclaration, StringComparison.Ordinal)
                    : result.Replace(staticDeclaration, instanceDeclaration, StringComparison.Ordinal);
            }
        }

        return result;
    }
}
#endif
