using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Decompilation;

/// <summary>
/// Inspects managed assembly metadata and decompiles selected types without executing the assembly.
/// </summary>
public interface IAssemblyDecompilationService
{
    /// <summary>Reads assembly and type/member metadata without creating files.</summary>
    Task<AssemblyInspection> InspectAsync(string assemblyPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Decompiles one type and writes the generated source only into the configured artifact root.
    /// </summary>
    Task<DecompiledTypeArtifact> DecompileTypeAsync(
        string assemblyPath,
        string typeFullName,
        CancellationToken cancellationToken = default);
}
