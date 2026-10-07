namespace AhnWin52Backup.Core.Workflows;

/// <summary>Describes a physical field in a structure-template table or index.</summary>
public sealed record StructureTemplateField(string Name, byte TypeCode, int Length);
