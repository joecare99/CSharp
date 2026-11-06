using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Config.Secrets;

/// <summary>Stores generic credentials in the current Windows user's Credential Manager.</summary>
public sealed class WindowsCredentialProvider : ICredentialProvider
{
    private const int MaximumCredentialBlobBytes = 2560;
    private const string TargetPrefix = "AvlnCodeStudio:";
    private static readonly UTF8Encoding s_utf8 = new(false, true);
    private readonly IWindowsCredentialApi _api;
    private readonly bool _requiresWindows;

    /// <summary>Creates a provider backed by the Windows Credential Manager API.</summary>
    public WindowsCredentialProvider()
        : this(new WindowsCredentialApi())
    {
    }

    internal WindowsCredentialProvider(IWindowsCredentialApi api)
    {
        _api = api ?? throw new ArgumentNullException(nameof(api));
        _requiresWindows = api is WindowsCredentialApi;
    }

    /// <inheritdoc/>
    public string ProviderId => "windows-credential-manager";

    /// <inheritdoc/>
    public ValueTask<CredentialProviderStatus> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(new CredentialProviderStatus(
            ProviderId,
            OperatingSystem.IsWindows()
                ? CredentialProviderAvailability.Available
                : CredentialProviderAvailability.Unavailable));
    }

    /// <inheritdoc/>
    public ValueTask<CredentialSecret?> GetAsync(CredentialId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        cancellationToken.ThrowIfCancellationRequested();
        EnsureSupportedPlatform();

        if (!_api.TryRead(GetTargetName(id), out byte[]? bytes) || bytes is null)
        {
            return ValueTask.FromResult<CredentialSecret?>(null);
        }

        char[] characters = [];
        try
        {
            int characterCount = s_utf8.GetCharCount(bytes);
            characters = new char[characterCount];
            s_utf8.GetChars(bytes, characters);
            return ValueTask.FromResult<CredentialSecret?>(new CredentialSecret(characters));
        }
        catch (DecoderFallbackException exception)
        {
            throw new InvalidDataException(
                "The Windows credential is not valid UTF-8. Delete and recreate this credential.",
                exception);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(bytes);
            CryptographicOperations.ZeroMemory(MemoryMarshal.AsBytes(characters.AsSpan()));
        }
    }

    /// <inheritdoc/>
    public ValueTask SetAsync(
        CredentialId id,
        CredentialSecret secret,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(secret);
        cancellationToken.ThrowIfCancellationRequested();
        EnsureSupportedPlatform();

        ReadOnlySpan<char> characters = secret.Reveal();
        int byteCount;
        try
        {
            byteCount = s_utf8.GetByteCount(characters);
        }
        catch (EncoderFallbackException exception)
        {
            throw new ArgumentException("The credential contains characters that cannot be encoded as UTF-8.", nameof(secret), exception);
        }

        if (byteCount > MaximumCredentialBlobBytes)
        {
            throw new ArgumentOutOfRangeException(
                nameof(secret),
                $"Windows generic credentials are limited to {MaximumCredentialBlobBytes} bytes.");
        }

        byte[] bytes = new byte[byteCount];
        try
        {
            s_utf8.GetBytes(characters, bytes);
            cancellationToken.ThrowIfCancellationRequested();
            _api.Write(GetTargetName(id), bytes);
            return ValueTask.CompletedTask;
        }
        finally
        {
            CryptographicOperations.ZeroMemory(bytes);
        }
    }

    /// <inheritdoc/>
    public ValueTask DeleteAsync(CredentialId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        cancellationToken.ThrowIfCancellationRequested();
        EnsureSupportedPlatform();
        _api.Delete(GetTargetName(id));
        return ValueTask.CompletedTask;
    }

    private static string GetTargetName(CredentialId id)
    {
        string identity = string.Concat(id.ServiceId, "\0", id.Name, "\0", id.ScopeId ?? string.Empty);
        byte[] identityBytes = Encoding.UTF8.GetBytes(identity);
        try
        {
            return TargetPrefix + Convert.ToHexString(SHA256.HashData(identityBytes));
        }
        finally
        {
            CryptographicOperations.ZeroMemory(identityBytes);
        }
    }

    private void EnsureSupportedPlatform()
    {
        if (_requiresWindows && !OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("Windows Credential Manager is available only on Windows.");
        }
    }
}

internal interface IWindowsCredentialApi
{
    bool TryRead(string targetName, out byte[]? secret);
    void Write(string targetName, ReadOnlySpan<byte> secret);
    void Delete(string targetName);
}

internal sealed class WindowsCredentialApi : IWindowsCredentialApi
{
    private const uint CredentialTypeGeneric = 1;
    private const uint CredentialPersistLocalMachine = 2;
    private const uint MaximumCredentialBlobBytes = 2560;
    private const int ErrorNotFound = 1168;

    public bool TryRead(string targetName, out byte[]? secret)
    {
        if (!NativeMethods.CredRead(targetName, CredentialTypeGeneric, 0, out IntPtr credentialPointer))
        {
            int error = Marshal.GetLastPInvokeError();
            if (error == ErrorNotFound)
            {
                secret = null;
                return false;
            }

            throw new Win32Exception(error, "Windows Credential Manager could not read the requested credential.");
        }

        try
        {
            NativeCredential credential = Marshal.PtrToStructure<NativeCredential>(credentialPointer);
            if (credential.CredentialBlobSize > MaximumCredentialBlobBytes)
            {
                throw new InvalidDataException("The stored Windows credential exceeds the supported size limit.");
            }

            secret = new byte[credential.CredentialBlobSize];
            if (secret.Length > 0)
            {
                Marshal.Copy(credential.CredentialBlob, secret, 0, secret.Length);
            }

            return true;
        }
        finally
        {
            NativeCredential credential = Marshal.PtrToStructure<NativeCredential>(credentialPointer);
            if (credential.CredentialBlob != IntPtr.Zero
                && credential.CredentialBlobSize > 0
                && credential.CredentialBlobSize <= MaximumCredentialBlobBytes)
            {
                byte[] zeroBuffer = new byte[credential.CredentialBlobSize];
                Marshal.Copy(zeroBuffer, 0, credential.CredentialBlob, zeroBuffer.Length);
            }

            NativeMethods.CredFree(credentialPointer);
        }
    }

    public void Write(string targetName, ReadOnlySpan<byte> secret)
    {
        byte[] bytes = secret.ToArray();
        GCHandle pinnedBlob = default;
        IntPtr credentialPointer = IntPtr.Zero;
        try
        {
            pinnedBlob = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            NativeCredential credential = new()
            {
                Type = CredentialTypeGeneric,
                TargetName = targetName,
                CredentialBlobSize = checked((uint)bytes.Length),
                CredentialBlob = bytes.Length == 0 ? IntPtr.Zero : pinnedBlob.AddrOfPinnedObject(),
                Persist = CredentialPersistLocalMachine,
            };
            credentialPointer = Marshal.AllocHGlobal(Marshal.SizeOf<NativeCredential>());
            Marshal.StructureToPtr(credential, credentialPointer, fDeleteOld: false);
            if (!NativeMethods.CredWrite(credentialPointer, 0))
            {
                throw new Win32Exception(
                    Marshal.GetLastPInvokeError(),
                    "Windows Credential Manager could not store the credential.");
            }
        }
        finally
        {
            if (credentialPointer != IntPtr.Zero)
            {
                Marshal.DestroyStructure<NativeCredential>(credentialPointer);
                Marshal.FreeHGlobal(credentialPointer);
            }

            if (pinnedBlob.IsAllocated)
            {
                pinnedBlob.Free();
            }

            CryptographicOperations.ZeroMemory(bytes);
        }
    }

    public void Delete(string targetName)
    {
        if (!NativeMethods.CredDelete(targetName, CredentialTypeGeneric, 0))
        {
            int error = Marshal.GetLastPInvokeError();
            if (error != ErrorNotFound)
            {
                throw new Win32Exception(error, "Windows Credential Manager could not delete the credential.");
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct NativeCredential
    {
        public uint Flags;
        public uint Type;
        [MarshalAs(UnmanagedType.LPWStr)] public string? TargetName;
        [MarshalAs(UnmanagedType.LPWStr)] public string? Comment;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
        public uint CredentialBlobSize;
        public IntPtr CredentialBlob;
        public uint Persist;
        public uint AttributeCount;
        public IntPtr Attributes;
        [MarshalAs(UnmanagedType.LPWStr)] public string? TargetAlias;
        [MarshalAs(UnmanagedType.LPWStr)] public string? UserName;
    }

    private static class NativeMethods
    {
        [DllImport("advapi32.dll", EntryPoint = "CredReadW", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CredRead(string targetName, uint type, uint flags, out IntPtr credential);

        [DllImport("advapi32.dll", EntryPoint = "CredWriteW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CredWrite(IntPtr credential, uint flags);

        [DllImport("advapi32.dll", EntryPoint = "CredDeleteW", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CredDelete(string targetName, uint type, uint flags);

        [DllImport("advapi32.dll", EntryPoint = "CredFree")]
        internal static extern void CredFree(IntPtr buffer);
    }
}
