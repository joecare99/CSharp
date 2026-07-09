using System;
using GenSecure.Contracts;

namespace GenSecure.Core;

internal static class PlatformServiceResolver
{
    private const string WindowsLocalKeyProtectorTypeName = "GenSecure.Windows.DpapiLocalKeyProtector, GenSecure.Windows";
    private const string WindowsPrincipalProviderTypeName = "GenSecure.Windows.WindowsSidPrincipalProvider, GenSecure.Windows";
    private const string LinuxLocalKeyProtectorTypeName = "GenSecure.Linux.DataProtectionLocalKeyProtector, GenSecure.Linux";
    private const string LinuxPrincipalProviderTypeName = "GenSecure.Linux.EnvironmentPrincipalProvider, GenSecure.Linux";

    public static ILocalKeyProtector CreateLocalKeyProtector(GenSecureStoreOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        string sTypeName = OperatingSystem.IsWindows()
            ? WindowsLocalKeyProtectorTypeName
            : LinuxLocalKeyProtectorTypeName;

        Type? type = Type.GetType(sTypeName, throwOnError: false);
        if (type is null)
        {
            throw CreateMissingPlatformAssemblyException(sTypeName);
        }

        object? instance = OperatingSystem.IsWindows()
            ? Activator.CreateInstance(type)
            : Activator.CreateInstance(type, options);

        return instance as ILocalKeyProtector
            ?? throw new InvalidOperationException($"The platform local key protector '{sTypeName}' does not implement {nameof(ILocalKeyProtector)}.");
    }

    public static ICurrentPrincipalProvider CreateCurrentPrincipalProvider()
    {
        string sTypeName = OperatingSystem.IsWindows()
            ? WindowsPrincipalProviderTypeName
            : LinuxPrincipalProviderTypeName;

        Type? type = Type.GetType(sTypeName, throwOnError: false);
        if (type is null)
        {
            throw CreateMissingPlatformAssemblyException(sTypeName);
        }

        object? instance = Activator.CreateInstance(type);
        return instance as ICurrentPrincipalProvider
            ?? throw new InvalidOperationException($"The platform principal provider '{sTypeName}' does not implement {nameof(ICurrentPrincipalProvider)}.");
    }

    private static InvalidOperationException CreateMissingPlatformAssemblyException(string sTypeName)
    {
        string sAssemblyName = OperatingSystem.IsWindows() ? "GenSecure.Windows" : "GenSecure.Linux";
        return new InvalidOperationException(
            $"The platform adapter assembly '{sAssemblyName}' is required but was not found. " +
            $"Reference the appropriate GenSecure platform project and register its DI extension before using GenSecure.Core. Missing type: '{sTypeName}'.");
    }
}
