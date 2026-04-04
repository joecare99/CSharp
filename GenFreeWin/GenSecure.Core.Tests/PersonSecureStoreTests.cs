using System;
using System.IO;
using System.Linq;
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

            return new DirectoryInfo(Options.DataDirectoryPath).GetFiles();
        }

        public FileInfo[] GetKeyFiles()
        {
            if (!Directory.Exists(Options.KeyDirectoryPath))
            {
                return Array.Empty<FileInfo>();
            }

            return new DirectoryInfo(Options.KeyDirectoryPath).GetFiles();
        }

        public void Dispose()
        {
            if (Directory.Exists(Options.GetValidatedRootDirectory()))
            {
                Directory.Delete(Options.GetValidatedRootDirectory(), recursive: true);
            }
        }
    }
}
