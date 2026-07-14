using GenSecure.Contracts;
using GenSecure.Core;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace GenSecure.Linux;

/// <summary>
/// Protects the local master key by using the ASP.NET Core data protection stack.
/// </summary>
public sealed class DataProtectionLocalKeyProtector : ILocalKeyProtector
{
    private readonly IDataProtector _dataProtector;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataProtectionLocalKeyProtector"/> class.
    /// </summary>
    /// <param name="options">The GenSecure store options.</param>
    public DataProtectionLocalKeyProtector(GenSecureStoreOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        string sKeyRingDirectoryPath = Path.Combine(options.MasterKeyDirectoryPath, "keyring");
        Directory.CreateDirectory(sKeyRingDirectoryPath);

        var serviceCollection = new ServiceCollection();
        serviceCollection
            .AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(sKeyRingDirectoryPath))
            .SetApplicationName("GenSecure.LocalMasterKey");

        _dataProtector = serviceCollection
            .BuildServiceProvider()
            .GetRequiredService<IDataProtectionProvider>()
            .CreateProtector("GenSecure.LocalMasterKey");
    }

    /// <inheritdoc />
    public byte[] Protect(byte[] arrPlaintext)
    {
        ArgumentNullException.ThrowIfNull(arrPlaintext);
        return _dataProtector.Protect(arrPlaintext);
    }

    /// <inheritdoc />
    public byte[] Unprotect(byte[] arrProtectedData)
    {
        ArgumentNullException.ThrowIfNull(arrProtectedData);
        return _dataProtector.Unprotect(arrProtectedData);
    }
}
