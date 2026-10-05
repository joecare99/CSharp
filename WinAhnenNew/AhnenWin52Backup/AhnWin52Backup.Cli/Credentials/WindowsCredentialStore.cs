using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace AhnWin52Backup.Cli.Credentials;

internal sealed class WindowsCredentialStore : IPasswordCredentialStore
{
    private const string TargetName = "AhnWin52Backup/ParadoxDatabasePassword";
    private const uint CredentialTypeGeneric = 1;
    private const uint CredentialPersistLocalMachine = 2;
    private const int MaximumCredentialBlobSize = 5 * 512;

    public void SetPassword(char[] password)
    {
        ArgumentNullException.ThrowIfNull(password);
        ReadOnlySpan<char> passwordSpan = password;
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("Windows Credential Manager is available only on Windows.");
        }

        if (passwordSpan.IsEmpty)
        {
            throw new ArgumentException("The password cannot be empty.", nameof(password));
        }

        int byteCount = Encoding.Unicode.GetByteCount(passwordSpan);
        if (byteCount > MaximumCredentialBlobSize)
        {
            throw new ArgumentException("The password exceeds the Windows Credential Manager size limit.", nameof(password));
        }

        byte[] passwordBytes = new byte[byteCount];
        IntPtr credentialBlob = IntPtr.Zero;
        try
        {
            Encoding.Unicode.GetBytes(passwordSpan, passwordBytes);
            credentialBlob = Marshal.AllocHGlobal(byteCount);
            Marshal.Copy(passwordBytes, 0, credentialBlob, byteCount);

            NativeCredential credential = new()
            {
                Type = CredentialTypeGeneric,
                TargetName = TargetName,
                CredentialBlobSize = (uint)byteCount,
                CredentialBlob = credentialBlob,
                Persist = CredentialPersistLocalMachine
            };

            if (!CredWrite(ref credential, 0))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not save the password in Windows Credential Manager.");
            }
        }
        finally
        {
            CryptographicOperations.ZeroMemory(passwordBytes);
            if (credentialBlob != IntPtr.Zero)
            {
                Marshal.Copy(new byte[byteCount], 0, credentialBlob, byteCount);
                Marshal.FreeHGlobal(credentialBlob);
            }
        }
    }

    [DllImport("Advapi32.dll", EntryPoint = "CredWriteW", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CredWrite(ref NativeCredential credential, uint flags);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct NativeCredential
    {
        public uint Flags;
        public uint Type;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string? TargetName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string? Comment;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
        public uint CredentialBlobSize;
        public IntPtr CredentialBlob;
        public uint Persist;
        public uint AttributeCount;
        public IntPtr Attributes;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string? TargetAlias;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string? UserName;
    }
}
