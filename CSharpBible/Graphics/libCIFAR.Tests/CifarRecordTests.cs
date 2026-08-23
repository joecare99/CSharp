using libCIFAR.Data;

namespace libCIFAR.Tests;

[TestClass]
public sealed class CifarRecordTests
{
    [TestMethod]
    public void Cifar10_ReadFrom_MapsLabelAndPlanarChannelsToRgb()
    {
        byte[] buffer = CreateRecordBuffer(1);
        buffer[0] = (byte)ECifar10Category.Automobile;
        buffer[1] = 10;
        buffer[1 + 1024] = 20;
        buffer[1 + 2048] = 30;

        var record = new Cifar10Record();
        record.ReadFrom(buffer, 0);

        Assert.AreEqual(ECifar10Category.Automobile, record.Label);
        CollectionAssert.AreEqual(new byte[] { 10, 20, 30 }, record.GetImageAsRgbArray()[..3]);
    }

    [TestMethod]
    public void Cifar100_ReadFrom_MapsFineLabelAndSubCategory()
    {
        byte[] buffer = CreateRecordBuffer(2);
        buffer[0] = (byte)ECifar100Category.vehicles_2;
        buffer[1] = 7;

        var record = new Cifar100Record();
        record.ReadFrom(buffer, 0);

        Assert.AreEqual(ECifar100Category.vehicles_2, record.Label);
        Assert.AreEqual(7, record.SubCategory);
    }

    [TestMethod]
    public void Cifar10_ReadFromStream_ReadsTheRecordHeaderAndPayload()
    {
        byte[] buffer = CreateRecordBuffer(1);
        buffer[0] = (byte)ECifar10Category.Truck;
        buffer[1] = 10;
        buffer[1 + 2047] = 30;
        buffer[1 + 2048] = 30;
        using var stream = new MemoryStream(buffer);
        var record = new Cifar10Record();

        record.ReadFromStream(stream);

        Assert.AreEqual(ECifar10Category.Truck, record.Label);
        Assert.AreEqual(10, record.ImageData[0]);
        Assert.AreEqual(30, record.ImageData[2047]);
    }

    private static byte[] CreateRecordBuffer(int headerLength)
    {
        byte[] buffer = new byte[headerLength + 3072];
        for (int index = 0; index < 3072; index++)
        {
            buffer[headerLength + index] = (byte)(index % 251);
        }

        return buffer;
    }
}
