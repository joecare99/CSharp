namespace IDR.Core.Models;

public sealed record StackLocalVariable(int EbpOffset, int SizeBytes, string TypeName);
