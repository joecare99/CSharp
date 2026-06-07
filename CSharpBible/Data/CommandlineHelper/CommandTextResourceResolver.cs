using System.Globalization;
using System.Reflection;
using System.Resources;

namespace CommandlineHelper;

public static class CommandTextResourceResolver
{
    public static bool TryResolveString(Type resourceType, string resourceName, out string? value)
    {
        if (resourceType is null)
            throw new ArgumentNullException(nameof(resourceType));

        if (resourceName is null)
            throw new ArgumentNullException(nameof(resourceName));

        value = null;
        if (string.IsNullOrWhiteSpace(resourceName))
            return false;

        var property = resourceType.GetProperty(resourceName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        if (property is not null && property.PropertyType == typeof(string) && property.GetMethod is not null)
        {
            value = property.GetValue(null) as string;
            return value is not null;
        }

        var resourceManagerProperty = resourceType.GetProperty("ResourceManager", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        if (resourceManagerProperty?.GetValue(null) is not ResourceManager resourceManager)
            return false;

        var cultureProperty = resourceType.GetProperty("Culture", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        var culture = cultureProperty?.GetValue(null) as CultureInfo;
        value = resourceManager.GetString(resourceName, culture);
        return value is not null;
    }
}
