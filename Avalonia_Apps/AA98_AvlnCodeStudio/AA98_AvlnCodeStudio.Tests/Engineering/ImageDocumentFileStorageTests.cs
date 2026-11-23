using AA98_AvlnCodeStudio.UI.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class ImageDocumentFileStorageTests
{
    [TestMethod]
    public async Task NewDestinationNeverOverwritesAnExistingFile()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_ImageStore_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string path = Path.Combine(root, "image.png");
            await File.WriteAllBytesAsync(path, [1, 2, 3]);
            await Assert.ThrowsExactlyAsync<IOException>(() =>
                ImageDocumentFileStorage.WriteAsync(path, [4, 5], null, CancellationToken.None));

            CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, await File.ReadAllBytesAsync(path));
            Assert.AreEqual(1, Directory.GetFiles(root).Length);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task ExistingOutputIsReplacedOnlyWhenItsSavedBytesStillMatch()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_ImageStore_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string path = Path.Combine(root, "image.png");
            byte[] first = [1, 2, 3];
            byte[] second = [4, 5, 6];
            await ImageDocumentFileStorage.WriteAsync(path, first, null, CancellationToken.None);
            await ImageDocumentFileStorage.WriteAsync(path, second, SHA256.HashData(first), CancellationToken.None);
            CollectionAssert.AreEqual(second, await File.ReadAllBytesAsync(path));

            byte[] external = [7, 8, 9];
            await File.WriteAllBytesAsync(path, external);
            await Assert.ThrowsExactlyAsync<IOException>(() =>
                ImageDocumentFileStorage.WriteAsync(path, first, SHA256.HashData(second), CancellationToken.None));
            CollectionAssert.AreEqual(external, await File.ReadAllBytesAsync(path));
            Assert.AreEqual(1, Directory.GetFiles(root).Length);

            File.Delete(path);
            await Assert.ThrowsExactlyAsync<FileNotFoundException>(() =>
                ImageDocumentFileStorage.WriteAsync(path, first, SHA256.HashData(second), CancellationToken.None));
            Assert.IsFalse(File.Exists(path));
            Assert.AreEqual(0, Directory.GetFiles(root).Length);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task OversizedOrCanceledOperationsDoNotCreateOutput()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_ImageStore_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string source = Path.Combine(root, "large.png");
            using (FileStream stream = File.Create(source))
            {
                stream.SetLength(ImageDocumentFileStorage.MaxEncodedBytes + 1L);
            }

            await Assert.ThrowsExactlyAsync<InvalidDataException>(() =>
                ImageDocumentFileStorage.ReadAsync(source, CancellationToken.None));
            string output = Path.Combine(root, "output.png");
            await Assert.ThrowsExactlyAsync<InvalidDataException>(() =>
                ImageDocumentFileStorage.WriteAsync(output,
                    new byte[ImageDocumentFileStorage.MaxEncodedBytes + 1], null, CancellationToken.None));
            using CancellationTokenSource cancellation = new();
            cancellation.Cancel();
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                ImageDocumentFileStorage.WriteAsync(output, [1, 2], null, cancellation.Token));
            Assert.IsFalse(File.Exists(output));

            byte[] existing = [3, 4, 5];
            await File.WriteAllBytesAsync(output, existing);
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                ImageDocumentFileStorage.WriteAsync(output, [1, 2], SHA256.HashData(existing), cancellation.Token));
            CollectionAssert.AreEqual(existing, await File.ReadAllBytesAsync(output));
            File.Delete(output);
            Assert.AreEqual(1, Directory.GetFiles(root).Length);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
