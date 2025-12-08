#if !NET7_0_OR_GREATER
// Kompatibilitätsattribute für 'required' auf älteren TFMs (.NET 5, .NET Framework usw.)
using System;

namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class RequiredMemberAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class CompilerFeatureRequiredAttribute : Attribute
{
    public CompilerFeatureRequiredAttribute(string featureName) => FeatureName = featureName;
    public string FeatureName { get; }
    public bool IsOptional { get; set; }
    public string? Language { get; set; }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public sealed class SetsRequiredMembersAttribute : Attribute { }
#endif