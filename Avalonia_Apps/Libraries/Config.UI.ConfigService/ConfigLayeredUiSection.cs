using Config.Service;
using Property.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Config.UI.ConfigService;

/// <summary>
/// Adapts one registered configuration section to an explicitly selected persistence layer.
/// </summary>
public sealed class ConfigLayeredUiSection : IConfigUiSection, IConfigUiDraftDiscard
{
    private readonly ConfigLayeredSettingsResolver _resolver;
    private readonly IConfigSectionProvider _provider;
    private readonly ConfigLayerDraftSession _draftSession;
    private readonly Func<IConfigSectionProvider, PropertyInfo, IReadOnlyList<PropertyOption>?>? _optionsProvider;
    private PropertyGeneration? _propertyGeneration;

    private sealed class PropertyGeneration
    {
        public volatile bool IsCurrent = true;
        public long ReloadVersion { get; init; }
    }

    /// <summary>
    /// Initializes a provider that edits only <paramref name="targetLayer"/>.
    /// The resolver must include a store for that layer.
    /// </summary>
    public ConfigLayeredUiSection(
        ConfigLayeredSettingsResolver resolver,
        IConfigSectionProvider provider,
        IConfigLayerStore targetStore,
        ConfigLayer targetLayer,
        bool isReadOnly = false,
        string? storageLocation = null)
        : this(resolver, provider, targetStore, targetLayer, new ConfigLayerDraftSession(targetStore), isReadOnly, storageLocation)
    {
    }

    /// <summary>
    /// Initializes a section with host-supplied property options. A null result uses
    /// the default options. A saved value outside the supplied options remains visible
    /// as unavailable so it can be replaced without silently changing it.
    /// </summary>
    public ConfigLayeredUiSection(
        ConfigLayeredSettingsResolver resolver,
        IConfigSectionProvider provider,
        IConfigLayerStore targetStore,
        ConfigLayer targetLayer,
        Func<IConfigSectionProvider, PropertyInfo, IReadOnlyList<PropertyOption>?> optionsProvider,
        bool isReadOnly = false,
        string? storageLocation = null)
        : this(
            resolver,
            provider,
            targetStore,
            targetLayer,
            new ConfigLayerDraftSession(targetStore),
            isReadOnly,
            storageLocation,
            optionsProvider ?? throw new ArgumentNullException(nameof(optionsProvider)))
    {
    }

    internal ConfigLayeredUiSection(
        ConfigLayeredSettingsResolver resolver,
        IConfigSectionProvider provider,
        IConfigLayerStore targetStore,
        ConfigLayer targetLayer,
        ConfigLayerDraftSession draftSession,
        bool isReadOnly = false,
        string? storageLocation = null,
        Func<IConfigSectionProvider, PropertyInfo, IReadOnlyList<PropertyOption>?>? optionsProvider = null)
    {
        _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        ArgumentNullException.ThrowIfNull(targetStore);
        _draftSession = draftSession ?? throw new ArgumentNullException(nameof(draftSession));
        _optionsProvider = optionsProvider;
        if (!Enum.IsDefined(targetLayer))
        {
            throw new ArgumentOutOfRangeException(nameof(targetLayer));
        }

        if (targetStore.Layer != targetLayer)
        {
            throw new ArgumentException("The target store must match the selected configuration layer.", nameof(targetStore));
        }

        TargetLayer = targetLayer;
        Section = new ConfigUiSection(provider.Name, provider.DisplayName, provider.Description, provider.Order);
        IsReadOnly = isReadOnly;
        string? readOnlyReason = isReadOnly ? "This configuration layer is read-only." : null;
        Context = new PropertyProviderContext(
            GetScope(targetLayer),
            provider.DisplayName,
            GetTargetDisplayName(targetLayer),
            storageLocation,
            readOnlyReason);
    }

    /// <summary>Gets the layer this provider edits.</summary>
    public ConfigLayer TargetLayer { get; }

    /// <inheritdoc/>
    public object DraftIdentity => _draftSession;

    /// <inheritdoc/>
    public ConfigUiSection Section { get; }

    /// <inheritdoc/>
    public PropertyProviderContext Context { get; }

    /// <inheritdoc/>
    public IReadOnlyList<IPropertyItem> Properties { get; private set; } = [];

    /// <inheritdoc/>
    public bool IsReadOnly { get; }

    /// <inheritdoc/>
    public ConfigUiSectionState State { get; private set; } = ConfigUiSectionState.Unavailable;

    /// <inheritdoc/>
    public string? ErrorMessage { get; private set; }

    /// <inheritdoc/>
    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        State = ConfigUiSectionState.Loading;
        ErrorMessage = null;
        try
        {
            await _draftSession.LoadAsync(cancellationToken).ConfigureAwait(false);
            await RefreshPropertiesAsync(cancellationToken).ConfigureAwait(false);
            State = ConfigUiSectionState.Ready;
        }
        catch (Exception exception) when (IsExpectedProviderFailure(exception))
        {
            SetFailed(Unwrap(exception));
        }
    }

    /// <inheritdoc/>
    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (IsReadOnly)
        {
            SetFailed(new InvalidOperationException("This configuration layer is read-only."));
            return;
        }

        if (Properties.Any(static property => !property.ValidationResult.IsValid))
        {
            SetFailed(new InvalidOperationException("Configuration contains invalid values."));
            return;
        }

        State = ConfigUiSectionState.Saving;
        ErrorMessage = null;
        bool saved = false;
        try
        {
            await _draftSession.SaveAsync(cancellationToken).ConfigureAwait(false);
            saved = true;
            await RefreshPropertiesAsync(cancellationToken).ConfigureAwait(false);
            State = ConfigUiSectionState.Ready;
        }
        catch (Exception exception) when (IsExpectedProviderFailure(exception))
        {
            SetFailed(Unwrap(exception), saved);
        }
    }

    /// <inheritdoc/>
    public async Task ResetAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (IsReadOnly)
        {
            SetFailed(new InvalidOperationException("This configuration layer is read-only."));
            return;
        }

        State = ConfigUiSectionState.Resetting;
        ErrorMessage = null;
        bool saved = false;
        try
        {
            await _draftSession.LoadAsync(cancellationToken).ConfigureAwait(false);
            await RemoveEditableSectionOverridesAsync(cancellationToken).ConfigureAwait(false);
            await _draftSession.SaveAsync(cancellationToken).ConfigureAwait(false);
            saved = true;
            await RefreshPropertiesAsync(cancellationToken).ConfigureAwait(false);
            State = ConfigUiSectionState.Ready;
        }
        catch (Exception exception) when (IsExpectedProviderFailure(exception))
        {
            SetFailed(Unwrap(exception), saved);
        }
    }

    /// <inheritdoc/>
    public async Task ResetDraftAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (IsReadOnly)
        {
            SetFailed(new InvalidOperationException("This configuration layer is read-only."));
            return;
        }

        State = ConfigUiSectionState.Resetting;
        ErrorMessage = null;
        try
        {
            await _draftSession.LoadAsync(cancellationToken).ConfigureAwait(false);
            await RemoveEditableSectionOverridesAsync(cancellationToken).ConfigureAwait(false);
            await RefreshPropertiesAsync(cancellationToken).ConfigureAwait(false);
            State = ConfigUiSectionState.Ready;
        }
        catch (Exception exception) when (IsExpectedProviderFailure(exception))
        {
            SetFailed(Unwrap(exception));
        }
    }

    Task IPropertyProvider.DiscardAsync(CancellationToken cancellationToken) =>
        ReloadDiscardingDraftAsync(cancellationToken);

    public Task DiscardDraftAsync(CancellationToken cancellationToken = default) =>
        ReloadDiscardingDraftAsync(cancellationToken);

    private async Task ReloadDiscardingDraftAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        State = ConfigUiSectionState.Loading;
        ErrorMessage = null;
        try
        {
            await _draftSession.ReloadAsync(cancellationToken).ConfigureAwait(false);
            await RefreshPropertiesAsync(cancellationToken).ConfigureAwait(false);
            State = ConfigUiSectionState.Ready;
        }
        catch (Exception exception) when (IsExpectedProviderFailure(exception))
        {
            SetFailed(Unwrap(exception));
        }
    }

    private async Task RefreshPropertiesAsync(CancellationToken cancellationToken)
    {
        long reloadVersion = _draftSession.GetReloadVersion();
        IReadOnlyDictionary<string, ConfigValueResolution> resolutions =
            await _resolver.ResolveSectionAsync(Section.Name, _draftSession.GetDraft(), cancellationToken).ConfigureAwait(false);

        PropertyGeneration generation = new() { ReloadVersion = reloadVersion };
        IPropertyItem[] properties = _provider.ModelType
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(static property => property.CanRead && property.CanWrite)
            .Where(static property => property.GetIndexParameters().Length == 0)
            .Where(static property => property.GetCustomAttribute<ConfigIgnoreAttribute>() is null)
            .Where(static property => property.GetCustomAttribute<ConfigUiIgnoreAttribute>() is null)
            .OrderBy(static property => property.Name, StringComparer.Ordinal)
            .Select(property => CreatePropertyItem(property, resolutions[property.Name], generation))
            .Cast<IPropertyItem>()
            .ToArray();
        if (_propertyGeneration is not null)
        {
            _propertyGeneration.IsCurrent = false;
        }
        _propertyGeneration = generation;
        Properties = properties;
    }

    private PropertyItem CreatePropertyItem(PropertyInfo property, ConfigValueResolution resolution, PropertyGeneration generation)
    {
        Type declaredType = property.PropertyType;
        Type valueType = Nullable.GetUnderlyingType(declaredType) ?? declaredType;
        bool isNullable = !valueType.IsValueType || Nullable.GetUnderlyingType(declaredType) is not null;
        IReadOnlyList<PropertyOption>? options = _optionsProvider?.Invoke(_provider, property)
            ?? (valueType.IsEnum
            ? Enum.GetValues(valueType)
                .Cast<object>()
                .Select(static value => new PropertyOption(value, value.ToString()!))
                .ToArray()
            : null);
        PropertyValueContext valueContext = ConfigLayerPropertyContext.Create(
            resolution,
            TargetLayer);
        string? readOnlyReason = valueContext.ReadOnlyReason ?? Context.ReadOnlyReason;
        bool isEditable = !IsReadOnly && !resolution.IsEnforced;
        if (isEditable && options is not null && resolution.Value is not null
            && !options.Any(option => Equals(option.Value, resolution.Value)))
        {
            options = options
                .Append(new PropertyOption(
                    resolution.Value,
                    $"Unavailable on this platform: {resolution.Value}"))
                .ToArray();
        }

        PropertyItem item = new(
            new PropertyCategory(Section.Name, Section.DisplayName, Section.SortOrder),
            property.Name,
            property.Name,
            valueType,
            resolution.Value,
            isNullable,
            isEditable: isEditable,
            isSensitive: property.GetCustomAttribute<SensitiveConfigPropertyAttribute>() is not null,
            options,
            validator: _ => generation.IsCurrent && _draftSession.IsReloadVersionCurrent(generation.ReloadVersion)
                ? PropertyValidationResult.Valid
                : PropertyValidationResult.Invalid("This property has been replaced. Use the current settings field."),
            valueContext: valueContext,
            readOnlyReason: readOnlyReason);
        item.ValueChanged += (_, eventArgs) =>
        {
            ConfigPropertyKey key = new(Section.Name, property.Name);
            _draftSession.SetValue(key, new ConfigLayerValue(eventArgs.NewValue));
        };
        return item;
    }

    private async Task RemoveEditableSectionOverridesAsync(CancellationToken cancellationToken)
    {
        ConfigLayerSnapshot draft = _draftSession.GetDraft();
        IReadOnlyDictionary<string, ConfigValueResolution> resolutions =
            await _resolver.ResolveSectionAsync(Section.Name, draft, cancellationToken).ConfigureAwait(false);
        foreach (PropertyInfo property in _provider.ModelType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!property.CanRead
                || !property.CanWrite
                || property.GetIndexParameters().Length != 0
                || property.GetCustomAttribute<ConfigIgnoreAttribute>() is not null
                || property.GetCustomAttribute<ConfigUiIgnoreAttribute>() is not null)
            {
                continue;
            }

            if (IsReadOnly || resolutions[property.Name].IsEnforced)
            {
                continue;
            }

            ConfigPropertyKey key = new(Section.Name, property.Name);
            if (_draftSession.GetDraft().TryGetValue(key, out _))
            {
                _draftSession.RemoveValue(key);
            }
        }
    }

    private void SetFailed(Exception exception, bool saved = false)
    {
        State = ConfigUiSectionState.Failed;
        ErrorMessage = saved
            ? $"Settings were saved, but the section could not be refreshed: {exception.Message}"
            : exception.Message;
    }

    private static PropertyScope GetScope(ConfigLayer layer) => layer switch
    {
        ConfigLayer.System => PropertyScope.System,
        ConfigLayer.Group => PropertyScope.Group,
        ConfigLayer.User => PropertyScope.User,
        _ => throw new ArgumentOutOfRangeException(nameof(layer)),
    };

    private static string GetTargetDisplayName(ConfigLayer layer) => layer switch
    {
        ConfigLayer.System => "System configuration",
        ConfigLayer.Group => "Group configuration",
        ConfigLayer.User => "User configuration",
        _ => throw new ArgumentOutOfRangeException(nameof(layer)),
    };

    private static bool IsExpectedProviderFailure(Exception exception) =>
        exception is IOException
            or UnauthorizedAccessException
            or InvalidDataException
            or ArgumentException
            or InvalidOperationException
            or TargetInvocationException;

    private static Exception Unwrap(Exception exception)
    {
        return exception is TargetInvocationException invocation
            && invocation.InnerException is Exception innerException
            ? innerException
            : exception;
    }
}
