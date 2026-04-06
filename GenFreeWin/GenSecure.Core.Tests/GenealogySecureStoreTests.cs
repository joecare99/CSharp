using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using BaseGenClasses.Model;
using CommunityToolkit.Mvvm.Messaging;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
using GenSecure.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace GenSecure.Core.Tests;

[TestClass]
public sealed class GenealogySecureStoreTests
{
    [TestMethod]
    public void SaveAndLoad_ShouldRoundTripBaseGenClassesGenealogy()
    {
        using GenealogyStoreScope scope = new();
        Genealogy genealogy = CreateSampleGenealogy();

        scope.Store.Save("tree-roundtrip", genealogy, _ => StoreMode.Encrypted);

        IGenealogy loaded = scope.Store.Load("tree-roundtrip", new BaseGenClassesFactory());

        Assert.AreEqual(2, loaded.Entitys.Count);
        Assert.AreEqual(1, loaded.Places.Count);

        IGenPerson loadedPerson = loaded.Entitys.OfType<IGenPerson>().Single();
        IGenFamily loadedFamily = loaded.Entitys.OfType<IGenFamily>().Single();
        IGenPlace loadedPlace = loaded.Places.Single();
        IGenFact birthFact = loadedPerson.Facts.Where(genFact => genFact is not null).Select(genFact => genFact!).Single(genFact => genFact.eFactType == EFactType.Birth);

        Assert.AreEqual("London", loadedPlace.Name);
        Assert.AreEqual(3, loadedPerson.Facts.Count(genFact => genFact is not null));
        Assert.AreEqual(new DateTime(1815, 12, 10), birthFact.Date?.Date1);
        Assert.AreSame(loadedPlace, birthFact.Place);
        Assert.AreEqual(1, loadedPerson.Connects.Count(genConnect => genConnect is not null));
        Assert.AreSame(loadedFamily, loadedPerson.Connects.Single(genConnect => genConnect is not null)!.Entity);
        Assert.AreEqual(1, loadedFamily.Connects.Count(genConnect => genConnect is not null));
        Assert.AreSame(loadedPerson, loadedFamily.Connects.Single(genConnect => genConnect is not null)!.Entity);
    }

    [TestMethod]
    public void Save_ShouldCreateSplitGitFriendlyGenealogyFiles()
    {
        using GenealogyStoreScope scope = new();
        Genealogy genealogy = CreateSampleGenealogy();

        scope.Store.Save("tree-layout", genealogy, _ => StoreMode.Encrypted);

        string sGenealogyRootPath = scope.GetGenealogyRootPath("tree-layout");
        FileInfo[] arrPersonFiles = GetFiles(Path.Combine(sGenealogyRootPath, scope.Options.SecureDataDirectoryName, scope.Options.DataDirectoryName), "*.person.json");
        FileInfo[] arrKeyFiles = GetFiles(Path.Combine(sGenealogyRootPath, scope.Options.SecureDataDirectoryName, scope.Options.KeyDirectoryName), "*.key.json");
        FileInfo[] arrFamilyFiles = GetFiles(Path.Combine(sGenealogyRootPath, "families"), "*.family.json");
        FileInfo[] arrPlaceFiles = GetFiles(Path.Combine(sGenealogyRootPath, "places"), "*.place.json");

        Assert.IsTrue(scope.Store.Exists("tree-layout"));
        Assert.IsTrue(File.Exists(Path.Combine(sGenealogyRootPath, "manifest.json")));
        Assert.AreEqual(1, arrPersonFiles.Length);
        Assert.AreEqual(1, arrKeyFiles.Length);
        Assert.AreEqual(1, arrFamilyFiles.Length);
        Assert.AreEqual(1, arrPlaceFiles.Length);
        Assert.AreEqual(3, GetRelativeSegments(Path.Combine(sGenealogyRootPath, scope.Options.SecureDataDirectoryName, scope.Options.DataDirectoryName), arrPersonFiles.Single().FullName).Length,
            "Expected person payloads to use a sharded file layout.");
    }

    private static Genealogy CreateSampleGenealogy()
    {
        IMessenger messenger = Substitute.For<IMessenger>();
        var genealogy = new Genealogy(messenger)
        {
            UId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
        };

        var place = new GenPlace("London")
        {
            UId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            ID = 10,
            Type = "City",
        };
        ((IHasOwner<IGenealogy>)place).SetOwner(genealogy);
        genealogy.Places.Add(place);

        var person = new GenPerson
        {
            UId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            ID = 20,
        };
        ((IHasOwner<IGenealogy>)person).SetOwner(genealogy);
        genealogy.Entitys.Add(person);

        person.Facts.Add(new GenFact(person, EFactType.Givenname)
        {
            UId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            ID = 21,
            Data = "Ada",
        });
        person.Facts.Add(new GenFact(person, EFactType.Surname)
        {
            UId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
            ID = 22,
            Data = "Lovelace",
        });
        person.Facts.Add(new GenFact(person, EFactType.Birth)
        {
            UId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
            ID = 23,
            Data = "Born in London",
            Place = place,
            Date = new GenDate(new DateTime(1815, 12, 10))
            {
                UId = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                ID = 24,
            },
        });

        var family = new GenFamily
        {
            UId = Guid.Parse("88888888-8888-8888-8888-888888888888"),
            ID = 30,
        };
        ((IHasOwner<IGenealogy>)family).SetOwner(genealogy);
        genealogy.Entitys.Add(family);

        family.Facts.Add(new GenFact(family, EFactType.Mariage)
        {
            UId = Guid.Parse("99999999-9999-9999-9999-999999999999"),
            ID = 31,
            Place = place,
            Date = new GenDate(new DateTime(1835, 7, 8))
            {
                UId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                ID = 32,
            },
        });

        person.Connects.Add(new GenConnect
        {
            UId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            Entity = family,
            eGenConnectionType = EGenConnectionType.ChildFamily,
        });
        family.Connects.Add(new GenConnect
        {
            UId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
            Entity = person,
            eGenConnectionType = EGenConnectionType.Child,
        });

        return genealogy;
    }

    private static FileInfo[] GetFiles(string sRootPath, string sSearchPattern)
    {
        if (!Directory.Exists(sRootPath))
        {
            return Array.Empty<FileInfo>();
        }

        return new DirectoryInfo(sRootPath).GetFiles(sSearchPattern, SearchOption.AllDirectories);
    }

    private static string[] GetRelativeSegments(string sRootPath, string sFilePath)
    {
        string sRelativePath = Path.GetRelativePath(sRootPath, sFilePath);
        return sRelativePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    private sealed class GenealogyStoreScope : IDisposable
    {
        public GenealogyStoreScope()
        {
            string sRootDirectory = Path.Combine(Path.GetTempPath(), "GenSecureGenealogyTests", Guid.NewGuid().ToString("N"));
            Options = new GenSecureStoreOptions
            {
                RootDirectory = sRootDirectory,
            };
            BackupService = new MasterKeyBackupService(Options);
            Store = new GenealogySecureStore(BackupService, Options);
        }

        public GenSecureStoreOptions Options { get; }

        public MasterKeyBackupService BackupService { get; }

        public GenealogySecureStore Store { get; }

        public string GetGenealogyRootPath(string sGenealogyId)
        {
            string sFileId = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(sGenealogyId))).ToLowerInvariant();
            return Path.Combine(Options.GenealogyDirectoryPath, sFileId);
        }

        public void Dispose()
        {
            if (Directory.Exists(Options.GetValidatedRootDirectory()))
            {
                Directory.Delete(Options.GetValidatedRootDirectory(), recursive: true);
            }
        }
    }

    private sealed class BaseGenClassesFactory : IGenealogyModelFactory
    {
        private readonly IMessenger _messenger = Substitute.For<IMessenger>();

        public IGenealogy CreateGenealogy(Guid gUid)
            => new Genealogy(_messenger)
            {
                UId = gUid,
            };

        public IGenPerson CreatePerson(Guid gUid, int iId, DateTime? dtLastChange)
            => new GenPerson
            {
                UId = gUid,
                ID = iId,
            };

        public IGenFamily CreateFamily(Guid gUid, int iId, DateTime? dtLastChange)
            => new GenFamily
            {
                UId = gUid,
                ID = iId,
            };

        public IGenPlace CreatePlace(Guid gUid, int iId, DateTime? dtLastChange)
            => new GenPlace()
            {
                UId = gUid,
                ID = iId,
            };

        public IGenFact CreateFact(IGenEntity genOwner, Guid gUid, int iId, DateTime? dtLastChange, EFactType eFactType)
            => new GenFact(genOwner, eFactType)
            {
                UId = gUid,
                ID = iId,
            };

        public IGenConnects CreateConnection(IGenEntity? genConnectedEntity, Guid gUid, EGenConnectionType eConnectionType)
            => new GenConnect
            {
                UId = gUid,
                Entity = genConnectedEntity,
                eGenConnectionType = eConnectionType,
            };

        public IGenDate CreateDate(Guid gUid, int iId, DateTime? dtLastChange)
            => new GenDate()
            {
                UId = gUid,
                ID = iId,
            };
    }
}
