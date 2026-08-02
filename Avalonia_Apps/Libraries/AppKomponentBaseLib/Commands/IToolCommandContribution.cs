using AppKomponentBaseLib.Context;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppKomponentBaseLib.Commands;

/// <summary>
/// Contributes a tool-capable command that can be executed by a host or workflow engine.
/// </summary>
public interface IToolCommandContribution
{
    /// <summary>
    /// Gets the descriptor that describes the tool-facing contract.
    /// </summary>
    ToolCommandDescriptor Descriptor { get; }

    /// <summary>
    /// Determines whether the contribution can execute for the current context.
    /// </summary>
    bool CanExecute(IAppContext context);

    /// <summary>
    /// Executes the tool contribution with the supplied parameters.
    /// </summary>
    Task<object?> ExecuteAsync(IAppContext context, IReadOnlyDictionary<string, object?> parameters, CancellationToken cancellationToken = default);
}
