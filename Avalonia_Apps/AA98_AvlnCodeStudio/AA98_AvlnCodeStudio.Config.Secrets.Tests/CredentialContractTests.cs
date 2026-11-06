using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AA98_AvlnCodeStudio.Config.Secrets.Tests;

[TestClass]
public sealed class CredentialContractTests
{
    [TestMethod]
    public void CredentialId_RequiresStableNonSecretIdentifiers()
    {
        CredentialId id = new("code-studio", "ai-api-key", "ifm");

        Assert.AreEqual("code-studio", id.ServiceId);
        Assert.AreEqual("ai-api-key", id.Name);
        Assert.AreEqual("ifm", id.ScopeId);
        Assert.ThrowsExactly<ArgumentException>(() => new CredentialId(" ", "key"));
        Assert.ThrowsExactly<ArgumentException>(() => new CredentialId("service", "key\nvalue"));
        Assert.ThrowsExactly<ArgumentException>(() => new CredentialId("service", " key "));
    }

    [TestMethod]
    public void CredentialSecret_IsRedactedAndUnavailableAfterDisposal()
    {
        CredentialSecret secret = new("sensitive-token".AsSpan());

        Assert.AreEqual("[REDACTED]", secret.ToString());
        Assert.AreEqual("sensitive-token", new string(secret.Reveal()));

        secret.Dispose();

        char[] ownedBuffer = (char[])typeof(CredentialSecret)
            .GetField("_characters", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(secret)!;
        CollectionAssert.AreEqual(new char[ownedBuffer.Length], ownedBuffer);
        Assert.ThrowsExactly<ObjectDisposedException>(() => secret.Reveal());
        Assert.AreEqual("[REDACTED]", secret.ToString());
    }

    [TestMethod]
    public void CredentialProviderCatalog_RequiresExplicitUniqueSelection()
    {
        FakeCredentialProvider windows = new("windows-credential-manager");
        FakeCredentialProvider vault = new("encrypted-vault");

        CredentialProviderCatalog catalog = new([vault, windows]);

        CollectionAssert.AreEqual(
            new[] { "encrypted-vault", "windows-credential-manager" },
            catalog.ProviderIds.ToArray());
        Assert.AreSame(vault, catalog.GetRequired("encrypted-vault"));
        Assert.ThrowsExactly<KeyNotFoundException>(() => catalog.GetRequired("unavailable-provider"));
        Assert.ThrowsExactly<ArgumentException>(() =>
            new CredentialProviderCatalog([windows, new FakeCredentialProvider(windows.ProviderId)]));
        Assert.ThrowsExactly<ArgumentException>(() =>
            new CredentialProviderCatalog([new FakeCredentialProvider("invalid ")]));
    }

    [TestMethod]
    public async Task FakeCredentialProvider_UsesDisposableSecretsForStoreReadAndDelete()
    {
        FakeCredentialProvider provider = new("test-provider");
        CredentialId id = new("code-studio", "ai-api-key");
        using CredentialSecret input = new("test-secret".AsSpan());

        await provider.SetAsync(id, input);
        using CredentialSecret? retrieved = await provider.GetAsync(id);

        Assert.IsNotNull(retrieved);
        Assert.AreEqual("test-secret", new string(retrieved.Reveal()));
        Assert.AreEqual(CredentialProviderAvailability.Available,
            (await provider.GetStatusAsync()).Availability);

        await provider.DeleteAsync(id);

        Assert.IsNull(await provider.GetAsync(id));
        Assert.AreEqual("[REDACTED]", input.ToString());
    }

    [TestMethod]
    public async Task WindowsCredentialProvider_EncodesSecretAndDoesNotRetainCallerBuffer()
    {
        FakeWindowsCredentialApi api = new();
        WindowsCredentialProvider provider = new(api);
        CredentialId id = new("code-studio", "ai-api-key", "ifm");
        using CredentialSecret input = new("token-ö".AsSpan());

        await provider.SetAsync(id, input);

        Assert.IsNotNull(api.TargetName);
        Assert.IsTrue(api.TargetName.StartsWith("AvlnCodeStudio:", StringComparison.Ordinal));
        Assert.IsTrue(api.StoredSecret!.All(static value => value != 0));
        using CredentialSecret? retrieved = await provider.GetAsync(id);
        Assert.IsNotNull(retrieved);
        Assert.AreEqual("token-ö", new string(retrieved.Reveal()));

        await provider.DeleteAsync(id);
        Assert.IsNull(await provider.GetAsync(id));
    }

    [TestMethod]
    public async Task WindowsCredentialProvider_RejectsOversizedSecretBeforeWriting()
    {
        FakeWindowsCredentialApi api = new();
        WindowsCredentialProvider provider = new(api);
        using CredentialSecret oversized = new(new string('x', 2561).AsSpan());

        await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(
            async () => await provider.SetAsync(new CredentialId("service", "key"), oversized));
        Assert.IsNull(api.StoredSecret);
    }

    [TestMethod]
    public async Task WindowsCredentialProvider_RealCredentialManagerRoundTripWhenExplicitlyEnabled()
    {
        if (!OperatingSystem.IsWindows()
            || !string.Equals(
                Environment.GetEnvironmentVariable("AVLN_RUN_CREDENTIAL_MANAGER_SMOKE"),
                "1",
                StringComparison.Ordinal))
        {
            Assert.Inconclusive("Set AVLN_RUN_CREDENTIAL_MANAGER_SMOKE=1 on Windows to opt in to a real Credential Manager write/delete smoke test.");
        }

        CredentialId id = new("code-studio-smoke", "temporary-" + Convert.ToHexString(RandomNumberGenerator.GetBytes(8)));
        WindowsCredentialProvider provider = new();
        using CredentialSecret secret = new("temporary-smoke-secret".AsSpan());
        try
        {
            await provider.SetAsync(id, secret);
            using CredentialSecret? retrieved = await provider.GetAsync(id);

            Assert.IsNotNull(retrieved);
            Assert.AreEqual("temporary-smoke-secret", new string(retrieved.Reveal()));
        }
        finally
        {
            await provider.DeleteAsync(id);
        }
    }

    [TestMethod]
    public async Task LinuxSecretServiceProvider_RealRoundTripWhenExplicitlyEnabled()
    {
        if (!OperatingSystem.IsLinux()
            || !string.Equals(
                Environment.GetEnvironmentVariable("AVLN_RUN_SECRET_SERVICE_SMOKE"),
                "1",
                StringComparison.Ordinal))
        {
            Assert.Inconclusive("Set AVLN_RUN_SECRET_SERVICE_SMOKE=1 on Linux to opt in to a real Secret Service write/read/delete smoke test.");
        }

        LinuxSecretServiceProvider provider = new();
        CredentialProviderStatus status = await provider.GetStatusAsync();
        if (status.Availability != CredentialProviderAvailability.Available)
        {
            Assert.Inconclusive($"Linux Secret Service is {status.Availability}; a real keyring is required for this smoke test.");
        }

        CredentialId id = new("code-studio-smoke", "temporary-" + Convert.ToHexString(RandomNumberGenerator.GetBytes(8)));
        using CredentialSecret secret = new("temporary-smoke-secret".AsSpan());
        try
        {
            await provider.SetAsync(id, secret);
            using CredentialSecret? retrieved = await provider.GetAsync(id);

            Assert.IsNotNull(retrieved);
            Assert.AreEqual("temporary-smoke-secret", new string(retrieved.Reveal()));
        }
        finally
        {
            await provider.DeleteAsync(id);
        }
    }

    [TestMethod]
    public async Task LinuxSecretServiceProvider_ReportsUnavailableOnOtherPlatforms()
    {
        if (OperatingSystem.IsLinux())
        {
            Assert.Inconclusive("This platform-specific status test runs only on non-Linux systems.");
        }

        CredentialProviderStatus status = await new LinuxSecretServiceProvider().GetStatusAsync();

        Assert.AreEqual(CredentialProviderAvailability.Unavailable, status.Availability);
    }

    [TestMethod]
    public async Task LinuxSecretServiceProvider_UsesOpaqueKeysAndClearsTemporaryBuffers()
    {
        FakeLinuxSecretServiceBackend backend = new();
        LinuxSecretServiceProvider provider = new(backend, requiresLinux: false);
        CredentialId id = new("code-studio", "ai-api-key", "ifm");
        using CredentialSecret input = new("token-ö".AsSpan());

        await provider.SetAsync(id, input);

        Assert.IsNotNull(backend.LastKey);
        Assert.AreEqual(64, backend.LastKey.Length);
        Assert.IsFalse(backend.LastKey.Contains(id.ServiceId, StringComparison.Ordinal));
        Assert.IsFalse(backend.LastKey.Contains(id.Name, StringComparison.Ordinal));
        CollectionAssert.AreEqual(new byte[backend.LastSetBuffer!.Length], backend.LastSetBuffer);

        using CredentialSecret? retrieved = await provider.GetAsync(id);

        Assert.IsNotNull(retrieved);
        Assert.AreEqual("token-ö", new string(retrieved.Reveal()));
        CollectionAssert.AreEqual(new byte[backend.LastReadBuffer!.Length], backend.LastReadBuffer);
        await provider.DeleteAsync(id);
        Assert.IsNull(await provider.GetAsync(id));
    }

    [TestMethod]
    public async Task LinuxSecretServiceProvider_ReportsLockedAndRefusesOperations()
    {
        FakeLinuxSecretServiceBackend backend = new()
        {
            Availability = CredentialProviderAvailability.Locked,
        };
        LinuxSecretServiceProvider provider = new(backend, requiresLinux: false);
        using CredentialSecret secret = new("secret".AsSpan());

        Assert.AreEqual(CredentialProviderAvailability.Locked, (await provider.GetStatusAsync()).Availability);
        await Assert.ThrowsExactlyAsync<CredentialProviderLockedException>(
            async () => await provider.GetAsync(new CredentialId("service", "key")));
        await Assert.ThrowsExactlyAsync<CredentialProviderLockedException>(
            async () => await provider.SetAsync(new CredentialId("service", "key"), secret));
        await Assert.ThrowsExactlyAsync<CredentialProviderLockedException>(
            async () => await provider.DeleteAsync(new CredentialId("service", "key")));
    }

    [TestMethod]
    public async Task LinuxSecretServiceProvider_RejectsInvalidUtf8AndClearsReadBuffer()
    {
        FakeLinuxSecretServiceBackend backend = new();
        LinuxSecretServiceProvider provider = new(backend, requiresLinux: false);
        backend.NextReadValue = [0xC3, 0x28];

        await Assert.ThrowsExactlyAsync<InvalidDataException>(
            async () => await provider.GetAsync(new CredentialId("service", "key")));
        CollectionAssert.AreEqual(new byte[backend.LastReadBuffer!.Length], backend.LastReadBuffer);
    }

    [TestMethod]
    public async Task LinuxSecretServiceProvider_HonorsCancellationBeforeBackendCalls()
    {
        FakeLinuxSecretServiceBackend backend = new();
        LinuxSecretServiceProvider provider = new(backend, requiresLinux: false);
        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();

        await Assert.ThrowsExactlyAsync<TaskCanceledException>(
            async () => await provider.GetStatusAsync(cancellation.Token));
        Assert.AreEqual(0, backend.CallCount);
    }

    private sealed class FakeCredentialProvider(string providerId) : ICredentialProvider
    {
        private readonly Dictionary<CredentialId, char[]> _values = [];

        public string ProviderId { get; } = providerId;

        public ValueTask<CredentialProviderStatus> GetStatusAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult(new CredentialProviderStatus(
                ProviderId,
                CredentialProviderAvailability.Available));
        }

        public ValueTask<CredentialSecret?> GetAsync(CredentialId id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CredentialSecret? value = _values.TryGetValue(id, out char[]? characters)
                ? new CredentialSecret(characters)
                : null;
            return ValueTask.FromResult(value);
        }

        public ValueTask SetAsync(
            CredentialId id,
            CredentialSecret secret,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (_values.Remove(id, out char[]? oldValue))
            {
                Array.Clear(oldValue);
            }

            _values.Add(id, secret.Reveal().ToArray());
            return ValueTask.CompletedTask;
        }

        public ValueTask DeleteAsync(CredentialId id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (_values.Remove(id, out char[]? value))
            {
                Array.Clear(value);
            }

            return ValueTask.CompletedTask;
        }
    }

    private sealed class FakeLinuxSecretServiceBackend : ILinuxSecretServiceBackend
    {
        private readonly Dictionary<string, byte[]> _values = new(StringComparer.Ordinal);

        public CredentialProviderAvailability Availability { get; set; } = CredentialProviderAvailability.Available;
        public byte[]? LastSetBuffer { get; private set; }
        public byte[]? LastReadBuffer { get; private set; }
        public string? LastKey { get; private set; }
        public byte[]? NextReadValue { get; set; }
        public int CallCount { get; private set; }

        public ValueTask<CredentialProviderAvailability> GetAvailabilityAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CallCount++;
            return ValueTask.FromResult(Availability);
        }

        public ValueTask<byte[]?> GetAsync(string credentialKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CallCount++;
            EnsureAvailable();
            LastKey = credentialKey;
            byte[]? value = NextReadValue ?? (_values.TryGetValue(credentialKey, out byte[]? stored)
                ? stored.ToArray()
                : null);
            NextReadValue = null;
            LastReadBuffer = value;
            return ValueTask.FromResult(value);
        }

        public ValueTask SetAsync(
            string credentialKey,
            ReadOnlyMemory<byte> secret,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CallCount++;
            EnsureAvailable();
            LastKey = credentialKey;
            if (_values.Remove(credentialKey, out byte[]? oldValue))
            {
                CryptographicOperations.ZeroMemory(oldValue);
            }

            LastSetBuffer = MemoryMarshal.TryGetArray(secret, out ArraySegment<byte> segment)
                ? segment.Array
                : secret.ToArray();
            _values.Add(credentialKey, secret.ToArray());
            return ValueTask.CompletedTask;
        }

        public ValueTask DeleteAsync(string credentialKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CallCount++;
            EnsureAvailable();
            LastKey = credentialKey;
            if (_values.Remove(credentialKey, out byte[]? oldValue))
            {
                CryptographicOperations.ZeroMemory(oldValue);
            }

            return ValueTask.CompletedTask;
        }

        private void EnsureAvailable()
        {
            if (Availability == CredentialProviderAvailability.Locked)
            {
                throw new CredentialProviderLockedException("linux-secret-service");
            }
        }
    }

    private sealed class FakeWindowsCredentialApi : IWindowsCredentialApi
    {
        public string? TargetName { get; private set; }
        public byte[]? StoredSecret { get; private set; }

        public bool TryRead(string targetName, out byte[]? secret)
        {
            if (string.Equals(TargetName, targetName, StringComparison.Ordinal) && StoredSecret is not null)
            {
                secret = StoredSecret.ToArray();
                return true;
            }

            secret = null;
            return false;
        }

        public void Write(string targetName, ReadOnlySpan<byte> secret)
        {
            if (StoredSecret is not null)
            {
                Array.Clear(StoredSecret);
            }

            TargetName = targetName;
            StoredSecret = secret.ToArray();
        }

        public void Delete(string targetName)
        {
            if (string.Equals(TargetName, targetName, StringComparison.Ordinal))
            {
                if (StoredSecret is not null)
                {
                    Array.Clear(StoredSecret);
                }

                StoredSecret = null;
            }
        }
    }
}
