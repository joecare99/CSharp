using System;
using System.IO;
using Osb.Core.Tenancy;
using Osb.ModernHost.Models;

namespace Osb.ModernHost.Services;

public sealed class ModernHostStartupService
{
    private readonly string[] _arguments;

    public ModernHostStartupService(string[] arguments)
    {
        _arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
    }

    public ModernHostState CreateState()
    {
        try
        {
            var tenant = TenantPathConfiguration.FromCommandLine(_arguments);
            tenant.EnsureDirectoryExists();
            return new ModernHostState(tenant, null);
        }
        catch (Exception exception) when (exception is ArgumentException || exception is DirectoryNotFoundException)
        {
            return new ModernHostState(
                TryParseTenant(),
                exception.Message);
        }
    }

    private TenantPathConfiguration? TryParseTenant()
    {
        try
        {
            return TenantPathConfiguration.FromCommandLine(_arguments);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
}
