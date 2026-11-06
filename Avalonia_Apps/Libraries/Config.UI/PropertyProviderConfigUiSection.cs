using Property.Editor;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Config.UI;

/// <summary>Adapts a host-neutral property provider to the Config.UI section lifecycle.</summary>
public sealed class PropertyProviderConfigUiSection : IConfigUiSection
{
    private readonly IPropertyProvider _provider;

    public PropertyProviderConfigUiSection(
        string name,
        string displayName,
        IPropertyProvider provider,
        string? description = null,
        int sortOrder = 0)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        Section = new ConfigUiSection(name, displayName, description, sortOrder);
        State = ConfigUiSectionState.Unavailable;
    }

    /// <inheritdoc/>
    public ConfigUiSection Section { get; }

    /// <inheritdoc/>
    public PropertyProviderContext Context => _provider.Context;

    /// <inheritdoc/>
    public IReadOnlyList<IPropertyItem> Properties => _provider.Properties;

    /// <inheritdoc/>
    public bool IsReadOnly => _provider.IsReadOnly;

    /// <inheritdoc/>
    public ConfigUiSectionState State { get; private set; }

    /// <inheritdoc/>
    public string? ErrorMessage { get; private set; }

    /// <inheritdoc/>
    public Task LoadAsync(CancellationToken cancellationToken = default) =>
        RunAsync(() => _provider.LoadAsync(cancellationToken), ConfigUiSectionState.Loading);

    /// <inheritdoc/>
    public Task SaveAsync(CancellationToken cancellationToken = default) =>
        RunAsync(() => _provider.ApplyAsync(cancellationToken), ConfigUiSectionState.Saving);

    /// <inheritdoc/>
    public Task ResetDraftAsync(CancellationToken cancellationToken = default) =>
        RunAsync(() => _provider.ResetDraftAsync(cancellationToken), ConfigUiSectionState.Resetting);

    /// <inheritdoc/>
    public Task ResetAsync(CancellationToken cancellationToken = default)
    {
        ErrorMessage = "This provider supports draft reset only; persistent reset is not available.";
        State = ConfigUiSectionState.Failed;
        return Task.CompletedTask;
    }

    private async Task RunAsync(Func<Task> operation, ConfigUiSectionState operationState)
    {
        State = operationState;
        ErrorMessage = null;
        try
        {
            await operation();
            State = ConfigUiSectionState.Ready;
        }
        catch (OperationCanceledException)
        {
            State = ConfigUiSectionState.Ready;
            throw;
        }
        catch (Exception exception)
        {
            ErrorMessage = exception.Message;
            State = ConfigUiSectionState.Failed;
        }
    }
}
