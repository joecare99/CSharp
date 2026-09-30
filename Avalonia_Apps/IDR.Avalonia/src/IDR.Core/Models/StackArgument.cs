namespace IDR.Core.Models;

public sealed record StackArgument(uint Offset, int SizeBytes, string? TypeName = null);
