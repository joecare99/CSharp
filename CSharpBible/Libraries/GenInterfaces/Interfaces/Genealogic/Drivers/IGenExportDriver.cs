using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GenInterfaces.Data;

namespace GenInterfaces.Interfaces.Genealogic.Drivers;

/// <summary>
/// Defines an asynchronous genealogy export driver.
/// </summary>
/// <typeparam name="TModel">The exported model type.</typeparam>
public interface IGenExportDriver<TModel>
{
    /// <summary>
    /// Exports the provided model to the target stream.
    /// </summary>
    /// <param name="model">The model to export.</param>
    /// <param name="targetStream">The target stream to write to.</param>
    /// <param name="cancellationToken">A token that can cancel the operation.</param>
    /// <returns>The export result containing diagnostics.</returns>
    Task<GenDriverResult> ExportAsync(
        TModel model,
        Stream targetStream,
        CancellationToken cancellationToken = default);
}
