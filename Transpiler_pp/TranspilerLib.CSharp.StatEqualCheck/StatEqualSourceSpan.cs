namespace TranspilerLib.CSharp.StatEqualCheck;

/// <summary>A zero-based source location suitable for reporting parser and comparison findings.</summary>
/// <param name="Start">The zero-based character offset.</param>
/// <param name="Length">The source fragment length in characters.</param>
/// <param name="Line">The one-based source line.</param>
/// <param name="Column">The one-based source column.</param>
public readonly record struct StatEqualSourceSpan(int Start, int Length, int Line, int Column);
