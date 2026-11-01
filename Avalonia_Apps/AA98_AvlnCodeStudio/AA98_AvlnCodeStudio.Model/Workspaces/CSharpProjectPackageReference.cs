namespace AA98_AvlnCodeStudio.Model.Workspaces;

public sealed record CSharpProjectPackageReference(
    string Id,
    string RequestedVersion,
    string ResolvedVersion,
    string TargetFramework);
