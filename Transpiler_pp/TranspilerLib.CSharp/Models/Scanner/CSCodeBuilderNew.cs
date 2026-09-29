using TranspilerLib.Interfaces.Code;

namespace TranspilerLib.Models.Scanner;

/// <summary>
/// Provides a dedicated builder entry point for the future granular C# code-block pipeline.
/// </summary>
/// <remarks>
/// The current implementation intentionally reuses the legacy <see cref="CSCodeBuilder"/> behavior so
/// the new builder shape can be introduced without changing the established output contract.
/// </remarks>
public class CSCodeBuilderNew : CSCodeBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CSCodeBuilderNew"/> class.
    /// </summary>
    public CSCodeBuilderNew()
    {
    }
}
