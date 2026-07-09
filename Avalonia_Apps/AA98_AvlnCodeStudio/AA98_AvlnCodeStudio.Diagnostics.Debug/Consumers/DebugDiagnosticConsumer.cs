using AppKomponentBaseLib.Diagnostics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using DebugOutput = System.Diagnostics.Debug;

namespace AA98_AvlnCodeStudio.Diagnostics.Debug.Consumers;

/// <summary>
/// Emits diagnostics through <see cref="DebugOutput"/>.
/// </summary>
public sealed class DebugDiagnosticConsumer : IDiagnosticConsumer
{
    /// <inheritdoc/>
    public ValueTask ConsumeAsync(IEnumerable<Diagnostic> diagnostics, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(diagnostics);

        foreach (Diagnostic diagnostic in diagnostics)
        {
            cancellationToken.ThrowIfCancellationRequested();
            DebugOutput.WriteLine(FormatDiagnostic(diagnostic));
        }

        return ValueTask.CompletedTask;
    }

    public static string FormatDiagnostic(Diagnostic diagnostic)
    {
        ArgumentNullException.ThrowIfNull(diagnostic);

        return string.Format(
            CultureInfo.InvariantCulture,
            "{0}|{1}|{2}|{3}|{4}",
            diagnostic.Severity,
            diagnostic.Code,
            diagnostic.SourcePath ?? string.Empty,
            diagnostic.LineNumber?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
            diagnostic.Message);
    }
}
