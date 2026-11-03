namespace AA98_AvlnCodeStudio.Model.Workspaces;

public sealed record AxamlMvvmSemanticMember(
    string Name,
    string TypeName,
    bool IsCommand,
    bool IsWritable);
