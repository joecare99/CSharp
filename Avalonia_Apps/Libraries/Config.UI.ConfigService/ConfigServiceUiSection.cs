using Config.Service;
using Property.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Config.UI.ConfigService;

/// <summary>
/// Adapts a Config.Service section for the product-neutral Config.UI contract.
/// </summary>
public sealed class ConfigServiceUiSection : IConfigUiSection
{
    private readonly global::Config.Service.ConfigService _configService;
    private readonly IConfigSectionProvider _provider;
    private object? _model;

    /// <summary>
    /// Initializes an adapter for a registered Config.Service section.
    /// </summary>
    public ConfigServiceUiSection(
        global::Config.Service.ConfigService configService,
        IConfigSectionProvider provider,
        bool isReadOnly = false)
    {
        _configService = configService ?? throw new ArgumentNullException(nameof(configService));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        Section = new ConfigUiSection(provider.Name, provider.DisplayName, provider.Description, provider.Order);
        IsReadOnly = isReadOnly;
    }

    /// <inheritdoc/>
    public ConfigUiSection Section { get; }

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
            _model = await LoadModelAsync(cancellationToken).ConfigureAwait(false);
            Properties = CreatePropertyItems(_model);
            State = ConfigUiSectionState.Ready;
        }
        catch (InvalidOperationException exception)
        {
            SetFailed(exception);
        }
        catch (TargetInvocationException exception)
        {
            SetFailed(exception.InnerException ?? exception);
        }
    }

    /// <inheritdoc/>
    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (IsReadOnly)
        {
            ErrorMessage = "This configuration section is read-only.";
            State = ConfigUiSectionState.Failed;
            return;
        }

        if (_model is null || Properties.Any(static property => !property.ValidationResult.IsValid))
        {
            ErrorMessage = "Configuration contains invalid values.";
            State = ConfigUiSectionState.Failed;
            return;
        }

        State = ConfigUiSectionState.Saving;
        ErrorMessage = null;
        try
        {
            await SaveModelAsync(_model, cancellationToken).ConfigureAwait(false);
            State = ConfigUiSectionState.Ready;
        }
        catch (InvalidOperationException exception)
        {
            SetFailed(exception);
        }
        catch (TargetInvocationException exception)
        {
            SetFailed(exception.InnerException ?? exception);
        }
    }

    /// <inheritdoc/>
    public async Task ResetAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (IsReadOnly)
        {
            ErrorMessage = "This configuration section is read-only.";
            State = ConfigUiSectionState.Failed;
            return;
        }

        State = ConfigUiSectionState.Resetting;
        ErrorMessage = null;
        try
        {
            await _configService.ResetAsync(_provider.Name).WaitAsync(cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            _model = _provider.CreateModel();
            Properties = CreatePropertyItems(_model);
            State = ConfigUiSectionState.Ready;
        }
        catch (InvalidOperationException exception)
        {
            SetFailed(exception);
        }
    }

    private async Task<object> LoadModelAsync(CancellationToken cancellationToken)
    {
        MethodInfo method = typeof(global::Config.Service.ConfigService)
            .GetMethod(nameof(global::Config.Service.ConfigService.LoadAsync))!
            .MakeGenericMethod(_provider.ModelType);
        Task task = (Task)method.Invoke(_configService, [_provider.Name])!;
        await task.WaitAsync(cancellationToken).ConfigureAwait(false);
        return task.GetType().GetProperty("Result")!.GetValue(task)
            ?? throw new InvalidOperationException($"Config section '{_provider.Name}' returned no model.");
    }

    private async Task SaveModelAsync(object model, CancellationToken cancellationToken)
    {
        MethodInfo method = typeof(global::Config.Service.ConfigService)
            .GetMethod(nameof(global::Config.Service.ConfigService.SaveAsync))!
            .MakeGenericMethod(_provider.ModelType);
        Task task = (Task)method.Invoke(_configService, [_provider.Name, model])!;
        await task.WaitAsync(cancellationToken).ConfigureAwait(false);
    }

    private IReadOnlyList<IPropertyItem> CreatePropertyItems(object model)
    {
        return _provider.ModelType
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(static property => property.CanRead && property.CanWrite)
            .Where(static property => property.GetCustomAttribute<ConfigIgnoreAttribute>() is null)
            .OrderBy(static property => property.Name, StringComparer.Ordinal)
            .Select(property => CreatePropertyItem(model, property))
            .Cast<IPropertyItem>()
            .ToArray();
    }

    private PropertyItem CreatePropertyItem(object model, PropertyInfo property)
    {
        Type declaredType = property.PropertyType;
        Type valueType = Nullable.GetUnderlyingType(declaredType) ?? declaredType;
        bool isNullable = !valueType.IsValueType || Nullable.GetUnderlyingType(declaredType) is not null;
        IReadOnlyList<PropertyOption>? options = valueType.IsEnum
            ? Enum.GetValues(valueType)
                .Cast<object>()
                .Select(static value => new PropertyOption(value, value.ToString()!))
                .ToArray()
            : null;

        PropertyItem propertyItem = new(
            new PropertyCategory(Section.Name, Section.DisplayName, Section.SortOrder),
            property.Name,
            property.Name,
            valueType,
            property.GetValue(model),
            isNullable,
            !IsReadOnly,
            property.GetCustomAttribute<SensitiveConfigPropertyAttribute>() is not null,
            options);
        propertyItem.ValueChanged += (_, eventArgs) => property.SetValue(model, eventArgs.NewValue);
        return propertyItem;
    }

    private void SetFailed(Exception exception)
    {
        State = ConfigUiSectionState.Failed;
        ErrorMessage = exception.Message;
    }
}
