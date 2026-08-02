using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RnzTrauer.Core.Domain;
using RnzTrauer.Core.Export;

namespace RnzTrauer.Core.Tests;

[TestClass]
public sealed class NoticeExportServiceTests
{
    [TestMethod]
    public async Task ExportCsvAsync_WritesUtf8BomTsvAndSkipsNonGenealogyCategories()
    {
        var fileName = Path.Combine(Path.GetTempPath(), $"rnz-{Guid.NewGuid():N}.tsv");
        try
        {
            var notices = new List<DeathNotice>
            {
                new()
                {
                    Id = 7,
                    OrderNumber = "A-7",
                    FamilyName = "Müller",
                    GivenName = "Anna",
                    Category = AdvertisementCategory.DeathNotice,
                    BirthDate = new DateTime(1940, 4, 3),
                    Text = "Text\twith\nseparators",
                },
                new()
                {
                    Id = 8,
                    OrderNumber = "A-8",
                    Category = AdvertisementCategory.Advertisement,
                },
            };

            await new NoticeExportService().ExportCsvAsync(fileName, notices);

            var bytes = await File.ReadAllBytesAsync(fileName);
            var content = Encoding.UTF8.GetString(bytes);
            Assert.IsTrue(bytes.AsSpan(0, 3).SequenceEqual(new byte[] { 0xEF, 0xBB, 0xBF }));
            StringAssert.Contains(content, "idAnzeige\tAuftrag\tNachname");
            StringAssert.Contains(content, "7\tA-7\tMüller\tAnna");
            Assert.IsFalse(content.Contains("A-8", StringComparison.Ordinal));
        }
        finally
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
        }
    }

    [TestMethod]
    public async Task ExportGedcomAsync_WritesHeaderPersonEventsAndNote()
    {
        var fileName = Path.Combine(Path.GetTempPath(), $"rnz-{Guid.NewGuid():N}.ged");
        try
        {
            var notice = new DeathNotice
            {
                Id = 12,
                OrderNumber = "A-12",
                FamilyName = "Müller",
                GivenName = "Anna",
                Sex = "F",
                Category = AdvertisementCategory.DeathNotice,
                BirthDate = new DateTime(1940, 4, 3),
                DeathDate = new DateTime(2024, 5, 12),
                BurialDate = new DateTime(2024, 5, 20),
                Place = "Heidelberg",
                Text = "Abschied",
            };

            await new NoticeExportService().ExportGedcomAsync(fileName, [notice]);

            var content = await File.ReadAllTextAsync(fileName, Encoding.UTF8);
            StringAssert.Contains(content, "0 HEAD");
            StringAssert.Contains(content, "0 @I12@ INDI");
            StringAssert.Contains(content, "1 NAME Anna /Müller/");
            StringAssert.Contains(content, "1 SEX F");
            StringAssert.Contains(content, "1 BIRT");
            StringAssert.Contains(content, "2 DATE 03 Apr 1940");
            StringAssert.Contains(content, "2 PLAC Heidelberg");
            StringAssert.Contains(content, "1 NOTE Abschied");
            StringAssert.Contains(content, "0 TRLR");
        }
        finally
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
        }
    }
}
