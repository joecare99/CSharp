using System.IO;
using System.Linq;
using AhnWin52Backup.Core.Abstractions;
using AhnWin52Backup.Core.Hej;
using AhnWin52Backup.Core.Workflows;
using NSubstitute;

namespace AhnWin52Backup.Tests.Workflows;

[TestClass]
public sealed class HejInspectionServiceTests
{
    [TestMethod]
    public void Inspect_UsesReaderAndReportsRecordCounts()
    {
        IHejReader reader = Substitute.For<IHejReader>();
        HejDocument document = new(
            [CreateRecord(HejSection.Individuals)],
            [],
            [CreateRecord(HejSection.Adoptions)],
            [],
            []);
        reader.Read(Arg.Any<Stream>()).Returns(document);
        HejInspectionService service = new(reader);
        using MemoryStream stream = new();

        HejInspection result = service.Inspect(stream);

        Assert.AreSame(document, result.Document);
        Assert.AreEqual(1, result.RecordCounts[HejSection.Individuals]);
        Assert.AreEqual(1, result.RecordCounts[HejSection.Adoptions]);
        Assert.AreEqual(0, result.RecordCounts[HejSection.Sources]);
        reader.Received(1).Read(stream);
    }

    private static HejRecord CreateRecord(HejSection section) =>
        new(Enumerable.Repeat(string.Empty, HejSchema.GetFieldCount(section)));
}
