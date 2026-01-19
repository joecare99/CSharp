using System;
using System.Collections.Generic;
using System.Reflection;

namespace Treppen.Print.Services;

public static class PrintExporterRegistry
{
    private static readonly Dictionary<string, Type> _byExtension = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, Type> _byName = new(StringComparer.OrdinalIgnoreCase);
    private static bool _initialized;

    public static void EnsureInitialized(params Assembly[] assemblies)
    {
        if (_initialized) return;
        var scan = assemblies != null && assemblies.Length > 0 ? assemblies : new[] { Assembly.GetExecutingAssembly() };

        foreach (var asm in scan)
        {
            foreach (var t in asm.GetTypes())
            {
                var attr = t.GetCustomAttribute<PrintExporterAttribute>();
                if (attr == null) continue;
                _byName[attr.Name] = t;
                foreach (var ext in attr.Extensions)
                {
                    _byExtension[ext.StartsWith('.') ? ext : "." + ext] = t;
                }
            }
        }
        _initialized = true;
    }

    public static Type? GetByExtension(string extension)
    {
        EnsureInitialized();
        return _byExtension.TryGetValue(extension, out var t) ? t : null;
    }

    public static Type? GetByName(string name)
    {
        EnsureInitialized();
        return _byName.TryGetValue(name, out var t) ? t : null;
    }
}
