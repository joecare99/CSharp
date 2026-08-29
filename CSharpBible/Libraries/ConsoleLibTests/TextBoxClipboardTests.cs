using System.Threading;
using System.Threading.Tasks;
using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.Tests;

[TestClass]
public class TextBoxClipboardTests
{
    [TestMethod]
    public async Task TextBox_UsesInjectedClipboardService()
    {
        var clipboard = new FakeClipboard();
        var textBox = new TextBox { Text = "hello", ClipboardService = clipboard };

        Assert.IsTrue(await textBox.CopyAsync());
        Assert.AreEqual("hello", clipboard.Value);
        clipboard.Value = " world";
        Assert.IsTrue(await textBox.PasteAsync());
        Assert.AreEqual("hello world", textBox.Text);
    }

    private sealed class FakeClipboard : IClipboardService
    {
        public string? Value { get; set; }
        public Task<bool> CopyAsync(string text, CancellationToken cancellationToken = default)
        {
            Value = text;
            return Task.FromResult(true);
        }
        public Task<string?> PasteAsync(CancellationToken cancellationToken = default) => Task.FromResult(Value);
    }
}
