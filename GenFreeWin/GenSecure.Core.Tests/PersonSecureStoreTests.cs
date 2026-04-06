using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using GenSecure.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenSecure.Core.Tests;

[TestClass]
public sealed class PersonSecureStoreTests
{
    [TestMethod]
    public void SaveAndLoad_ShouldRoundTripEncryptedPersonRecord()
    {
        using TestStoreScope scope = new();

        scope.Store.Save("person-1", new TestPerson("Ada", "Lovelace"));

        TestPerson person = scope.Store.Load<TestPerson>("person-1");

        Assert.AreEqual("Ada", person.FirstName);
        Assert.AreEqual("Lovelace", person.LastName);
        Assert.AreEqual(1, scope.GetDataFiles().Length);
        Assert.AreEqual(1, scope.GetKeyFiles().Length);
    }

    [TestMethod]
    public void SecureDelete_ShouldKeepCiphertextButRemovePersonKey()
    {
        using TestStoreScope scope = new();

        scope.Store.Save("person-2", new TestPerson("Grace", "Hopper"));
        string sDataFilePath = scope.GetDataFiles().Single().FullName;
        string sKeyFilePath = scope.GetKeyFiles().Single().FullName;

        scope.Store.Delete("person-2", DeleteMode.SecureDelete);

        Assert.IsTrue(File.Exists(sDataFilePath));
        Assert.IsFalse(File.Exists(sKeyFilePath));
        Assert.ThrowsExactly<InvalidOperationException>(() => scope.Store.Load<TestPerson>("person-2"));
    }

    [TestMethod]
    public void SoftDelete_ShouldRemoveCiphertextAndPersonKey()
    {
        using TestStoreScope scope = new();

        scope.Store.Save("person-3", new TestPerson("Katherine", "Johnson"));

        scope.Store.Delete("person-3", DeleteMode.SoftDelete);

        Assert.AreEqual(0, scope.GetDataFiles().Length);
        Assert.AreEqual(0, scope.GetKeyFiles().Length);
        Assert.IsFalse(scope.Store.Exists("person-3"));
    }

    [TestMethod]
    public void RestoreLocalMasterKey_ShouldRecoverAccessFromRecoveryBackup()
    {
        using TestStoreScope scope = new();

        scope.Store.Save("person-4", new TestPerson("Marie", "Curie"));
        scope.BackupService.CreateRecoveryKeyBackup("Recovery!Passphrase42", xOverwrite: true);
        File.Delete(scope.Options.LocalMasterKeyFilePath);

        bool xRestored = scope.BackupService.TryRestoreLocalMasterKey("Recovery!Passphrase42");
        TestPerson person = scope.Store.Load<TestPerson>("person-4");

        Assert.IsTrue(xRestored);
        Assert.AreEqual("Marie", person.FirstName);
        Assert.AreEqual("Curie", person.LastName);
    }

    [TestMethod]
    public void SaveAndLoad_Plaintext_ShouldRoundTripWithoutKeyFile()
    {
        using TestStoreScope scope = new();

        scope.Store.Save("plain-1", new TestPerson("Johann Wolfgang", "von Goethe"), StoreMode.Plaintext);

        // No key file must be created for plaintext records
        Assert.AreEqual(1, scope.GetDataFiles().Length);
        Assert.AreEqual(0, scope.GetKeyFiles().Length);

        TestPerson person = scope.Store.Load<TestPerson>("plain-1");

        Assert.AreEqual("Johann Wolfgang", person.FirstName);
        Assert.AreEqual("von Goethe", person.LastName);
    }

    [TestMethod]
    public void SoftDelete_Plaintext_ShouldRemoveDataFile()
    {
        using TestStoreScope scope = new();

        scope.Store.Save("plain-2", new TestPerson("Friedrich", "Schiller"), StoreMode.Plaintext);

        scope.Store.Delete("plain-2", DeleteMode.SoftDelete);

        Assert.AreEqual(0, scope.GetDataFiles().Length);
        Assert.IsFalse(scope.Store.Exists("plain-2"));
    }

    [TestMethod]
    public void SecureDelete_Plaintext_ShouldRemoveDataFile()
    {
        using TestStoreScope scope = new();

        scope.Store.Save("plain-3", new TestPerson("Immanuel", "Kant"), StoreMode.Plaintext);
        string sDataFilePath = scope.GetDataFiles().Single().FullName;

        scope.Store.Delete("plain-3", DeleteMode.SecureDelete);

        // No key file exists for plaintext; SecureDelete removes the data file as fallback
        Assert.IsFalse(File.Exists(sDataFilePath));
    }

    [TestMethod]
    public void GetAllowedWindowsSidHashes_ShouldReturnHmacSha256Hashes_NotRawSids()
    {
        using TestStoreScope scope = new();

        scope.Store.Save("person-5", new TestPerson("Emmy", "Noether"));

        IReadOnlyCollection<string> lstHashes = scope.Store.GetAllowedWindowsSidHashes("person-5");

        Assert.AreEqual(1, lstHashes.Count);

        string sSidHash = lstHashes.Single();

        // An HMAC-SHA256 output is 32 bytes = exactly 64 lowercase hex characters
        Assert.AreEqual(64, sSidHash.Length);
        Assert.IsTrue(sSidHash.All(c => (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f')),
            "The returned value must be a lowercase hex string, not a raw Windows SID.");

        // Must not look like a Windows SID
        Assert.IsFalse(sSidHash.StartsWith("S-", StringComparison.OrdinalIgnoreCase));

        // Must not be the plain SHA-256 of the SID — proves the HMAC pepper is in use
        string sCurrentSid = System.Security.Principal.WindowsIdentity.GetCurrent().User?.Value ?? string.Empty;
        string sPlainSha256 = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(sCurrentSid))).ToLowerInvariant();
        Assert.AreNotEqual(sPlainSha256, sSidHash,
            "The stored hash must be HMAC-SHA256(SID, pepper), not plain SHA-256(SID).");
    }

    [TestMethod]
    public void GrantAccess_ShouldStoreHashAndBeVerifiable()
    {
        using TestStoreScope scope = new();

        const string sFakeSid = "S-1-5-21-0000000000-1111111111-2222222222-500";
        scope.Store.Save("person-6", new TestPerson("Lise", "Meitner"));
        scope.Store.GrantAccess("person-6", sFakeSid);

        IReadOnlyCollection<string> lstHashes = scope.Store.GetAllowedWindowsSidHashes("person-6");

        // Owner hash (current user) + granted hash = 2 entries
        Assert.AreEqual(2, lstHashes.Count);

        // All entries must be lowercase hex (HMAC-SHA256 output — 64 chars)
        foreach (string sHash in lstHashes)
        {
            Assert.AreEqual(64, sHash.Length);
            Assert.IsTrue(sHash.All(c => (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f')),
                "Every stored hash must be a lowercase hex string, not a raw SID.");
        }

        // Raw SID must not appear in the list
        Assert.IsFalse(lstHashes.Contains(sFakeSid, StringComparer.OrdinalIgnoreCase),
            "The raw Windows SID must never be stored.");

        // Plain SHA-256(SID) must not appear — proves the pepper (HMAC) is in use
        string sPlainSha256 = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(sFakeSid))).ToLowerInvariant();
        Assert.IsFalse(lstHashes.Contains(sPlainSha256, StringComparer.OrdinalIgnoreCase),
            "The stored hash must be HMAC-SHA256(SID, pepper), not plain SHA-256(SID).");
    }

    [TestMethod]
    public void Save_ShouldUseShardedFilesystemLayout()
    {
        using TestStoreScope scope = new();

        scope.Store.Save("person-layout", new TestPerson("Rosalind", "Franklin"));

        FileInfo dataFile = scope.GetDataFiles().Single();
        FileInfo keyFile = scope.GetKeyFiles().Single();

        Assert.AreEqual(3, GetRelativeSegments(scope.Options.DataDirectoryPath, dataFile.FullName).Length,
            "Expected `<hash-prefix>/<hash-prefix>/<file>` under the data directory.");
        Assert.AreEqual(3, GetRelativeSegments(scope.Options.KeyDirectoryPath, keyFile.FullName).Length,
            "Expected `<hash-prefix>/<hash-prefix>/<file>` under the key directory.");
    }

    private sealed record TestPerson(string FirstName, string LastName);

    private sealed class TestStoreScope : IDisposable
    {
        public TestStoreScope()
        {
            string sRootDirectory = Path.Combine(Path.GetTempPath(), "GenSecureTests", Guid.NewGuid().ToString("N"));
            Options = new GenSecureStoreOptions
            {
                RootDirectory = sRootDirectory,
            };
            BackupService = new MasterKeyBackupService(Options);
            Store = new PersonSecureStore(BackupService, Options);
        }

        public GenSecureStoreOptions Options { get; }

        public MasterKeyBackupService BackupService { get; }

        public PersonSecureStore Store { get; }

        public FileInfo[] GetDataFiles()
        {
            if (!Directory.Exists(Options.DataDirectoryPath))
            {
                return Array.Empty<FileInfo>();
            }

            return new DirectoryInfo(Options.DataDirectoryPath).GetFiles("*", SearchOption.AllDirectories);
        }

        public FileInfo[] GetKeyFiles()
        {
            if (!Directory.Exists(Options.KeyDirectoryPath))
            {
                return Array.Empty<FileInfo>();
            }

            return new DirectoryInfo(Options.KeyDirectoryPath).GetFiles("*", SearchOption.AllDirectories);
        }

        public void Dispose()
        {
            if (Directory.Exists(Options.GetValidatedRootDirectory()))
            {
                Directory.Delete(Options.GetValidatedRootDirectory(), recursive: true);
            }
        }

    }

    private static string[] GetRelativeSegments(string sRootPath, string sFilePath)
    {
        string sRelativePath = Path.GetRelativePath(sRootPath, sFilePath);
        return sRelativePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }
}
