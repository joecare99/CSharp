using Config.Service;
using Property.Editor;
using System;

namespace Config.UI.ConfigService;

/// <summary>
/// Maps resolved configuration provenance to the shared property-editor context.
/// </summary>
public static class ConfigLayerPropertyContext
{
    /// <summary>
    /// Creates UI metadata for a resolved value while the user is editing one target layer.
    /// </summary>
    public static PropertyValueContext Create(
        ConfigValueResolution resolution,
        ConfigLayer targetLayer,
        string? origin = null)
    {
        ArgumentNullException.ThrowIfNull(resolution);
        if (!Enum.IsDefined(targetLayer))
        {
            throw new ArgumentOutOfRangeException(nameof(targetLayer));
        }

        PropertyScope sourceScope = resolution.Source switch
        {
            ConfigValueSource.ProductDefault => PropertyScope.Application,
            ConfigValueSource.System => PropertyScope.System,
            ConfigValueSource.Group => PropertyScope.Group,
            ConfigValueSource.User => PropertyScope.User,
            _ => throw new ArgumentOutOfRangeException(nameof(resolution), "The resolved configuration source is unknown."),
        };
        bool isInherited = resolution.Source switch
        {
            ConfigValueSource.ProductDefault => true,
            ConfigValueSource.System => targetLayer != ConfigLayer.System,
            ConfigValueSource.Group => targetLayer != ConfigLayer.Group,
            ConfigValueSource.User => targetLayer != ConfigLayer.User,
            _ => true,
        };
        string? readOnlyReason = resolution.IsEnforced
            ? "This value is locked by an enforced system policy."
            : null;

        return new PropertyValueContext(
            sourceScope,
            origin: origin ?? resolution.Source switch
            {
                ConfigValueSource.ProductDefault => "Product default",
                ConfigValueSource.System when resolution.IsEnforced => "Enforced system policy",
                ConfigValueSource.System => "System configuration",
                ConfigValueSource.Group => "Group configuration",
                ConfigValueSource.User => "User configuration",
                _ => throw new ArgumentOutOfRangeException(nameof(resolution), "The resolved configuration source is unknown."),
            },
            isInherited: isInherited,
            isPolicyLocked: resolution.IsEnforced,
            readOnlyReason: readOnlyReason);
    }
}
