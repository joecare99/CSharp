using Avalonia.Media;
using Avln_CommonDialogs.Avalonia.ViewModels;
using Avln_CommonDialogs.Base.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AA20_SysdialogsTests.Models;

[TestClass]
public class FontPickerViewModelTests
{
    [TestMethod]
    public void Constructor_UsesDefaultPreviewText_WhenNoPreviewTextProvided()
    {
        var fontFamilies = new[] { new FontFamily("Arial") };
        var selection = new FontDialogSelection { FamilyName = "Arial", Size = 16 };

        var viewModel = new FontPickerViewModel(fontFamilies, selection, null);

        Assert.IsFalse(string.IsNullOrWhiteSpace(viewModel.PreviewText));
        Assert.AreEqual("Arial", viewModel.SelectedFontFamily?.Name);
        Assert.AreEqual(16m, viewModel.FontSize);
    }

    [TestMethod]
    public void CreateSelection_ReturnsUpdatedUiAgnosticModel()
    {
        var fontFamilies = new[] { new FontFamily("Arial"), new FontFamily("Consolas") };
        var selection = new FontDialogSelection { FamilyName = "Arial", Size = 12 };
        var viewModel = new FontPickerViewModel(fontFamilies, selection, "Sample");

        viewModel.SelectedFontFamily = fontFamilies[1];
        viewModel.FontSize = 22m;
        viewModel.IsBold = true;
        viewModel.IsItalic = true;
        viewModel.IsUnderline = true;
        viewModel.SelectedColor = Color.FromUInt32(0xFF445566);

        var result = viewModel.CreateSelection();

        Assert.AreEqual("Consolas", result.FamilyName);
        Assert.AreEqual(22d, result.Size);
        Assert.IsTrue(result.IsBold);
        Assert.IsTrue(result.IsItalic);
        Assert.IsTrue(result.IsUnderline);
        Assert.AreEqual(0xFF445566u, result.ArgbColor);
    }
}
