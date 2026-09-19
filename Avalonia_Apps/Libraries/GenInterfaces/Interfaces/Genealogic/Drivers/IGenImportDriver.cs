using GenInterfaces.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GenInterfaces.Interfaces.Genealogic.Drivers;

/// <summary>
/// Defines an asynchronous genealogy import driver.
/// </summary>
/// <typeparam name="TModel">The imported model type.</typeparam>
public interface IGenImportDriver<TModel>
{
    /// <summary>
    /// Imports a model from the provided source stream.
    /// </summary>
    /// <param name="sourceStream">The source stream to read from.</param>
    /// <param name="cancellationToken">A token that can cancel the operation.</param>
    /// <returns>The import result containing diagnostics and an optional payload.</returns>
    Task<GenDriverResult<TModel>> ImportAsync(
        Stream sourceStream,
        CancellationToken cancellationToken = default);
}
