using System;
using System.Reflection;
using System.Runtime.Loader;

namespace AppWithPlugin.Model;

class PluginLoadContext : AssemblyLoadContext
{
#if NET5_0_OR_GREATER
    private AssemblyDependencyResolver _resolver;
#else
    private string? assemblyPath;
#endif

    public PluginLoadContext(string pluginPath)
    {
#if NET5_0_OR_GREATER
        _resolver = new AssemblyDependencyResolver(pluginPath);
#else
        assemblyPath = pluginPath;
#endif
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
#if NET5_0_OR_GREATER
        string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
#else
        string? assemblyPath = this.assemblyPath;
#endif

        if (assemblyPath != null)
        {

            Assembly assembly = LoadFromAssemblyPath(assemblyPath);
            if (!assembly?.IsFullyTrusted ?? false)
                return null;
            return assembly;
        }

        return null;
    }

    protected override nint LoadUnmanagedDll(string unmanagedDllName)
    {
#if NET5_0_OR_GREATER
        string? libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
#else
        string? libraryPath = this.assemblyPath;
#endif
        if (libraryPath != null)
        {
            return LoadUnmanagedDllFromPath(libraryPath);
        }

#if NET7_0_OR_GREATER
        return nint.Zero;
#else
        return IntPtr.Zero;
#endif
    }
}