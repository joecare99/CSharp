namespace AhnWin52Backup.Core.Paradox;

/// <summary>Describes one physical field in a Paradox table.</summary>
/// <param name="Name">The field name from the table header.</param>
/// <param name="TypeCode">The Paradox field type identifier.</param>
/// <param name="Length">The physical byte length of the field in a record.</param>
public sealed record ParadoxField(string Name, byte TypeCode, int Length);
