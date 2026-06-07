using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Fluent;
using Avln_ControlsAndLayout.Models;
using Avln_ControlsAndLayout.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Avln_ControlsAndLayoutTests;

/// <summary>
/// Provides an Avalonia headless application for runtime AXAML parser tests.
/// </summary>
public static class TestAppBuilder
{
    private static bool _isInitialized;

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<TestApplication>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions())
            .WithInterFont();

    public static void EnsureInitialized()
    {
        if (_isInitialized)
        {
            return;
        }

        BuildAvaloniaApp().SetupWithoutStarting();
        _isInitialized = true;
    }
}

/// <summary>
/// Minimal test application with Fluent styles so controls from snippets can be loaded.
/// </summary>
public sealed class TestApplication : Application
{
    public override void Initialize()
    {
        Styles.Add(new FluentTheme());
    }
}

[TestClass]
public class ControlsAndLayoutViewModelTests
{
    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        TestAppBuilder.EnsureInitialized();
    }

    [TestMethod]
    public void Constructor_ShouldInitializeFirstSampleAndRenderPreview()
    {
        var viewModel = new ControlsAndLayoutViewModel();

        Assert.IsNotNull(viewModel.SelectedSample);
        Assert.AreEqual("Border", viewModel.SampleTitle);
        Assert.AreEqual(SampleCode.Border, viewModel.EditableAxamlText);
        Assert.AreEqual(string.Empty, viewModel.ErrorText);
        Assert.IsInstanceOfType<Control>(viewModel.CurrentPreview);
    }

    [TestMethod]
    public void SelectedSample_ShouldUpdateMetadataTextAndPreview()
    {
        var viewModel = new ControlsAndLayoutViewModel();
        var target = viewModel.SampleGroups.SelectMany(group => group.Samples).Single(sample => sample.Title == "ToggleSwitch");

        viewModel.SelectedSample = target;

        Assert.AreEqual("ToggleSwitch", viewModel.SampleTitle);
        Assert.AreEqual(target.Description, viewModel.SampleDescription);
        Assert.AreEqual(SampleCode.ToggleSwitch, viewModel.EditableAxamlText);
        Assert.AreEqual(string.Empty, viewModel.ErrorText);
        Assert.IsInstanceOfType<Control>(viewModel.CurrentPreview);
    }

    [TestMethod]
    public void EditableAxamlText_WhenValid_ShouldRenderPreviewAndClearError()
    {
        var viewModel = new ControlsAndLayoutViewModel();

        viewModel.EditableAxamlText = """
<Grid xmlns="https://github.com/avaloniaui">
    <Button Content="Runtime changed" />
</Grid>
""";

        Assert.AreEqual(string.Empty, viewModel.ErrorText);
        Assert.IsInstanceOfType<Grid>(viewModel.CurrentPreview);
    }

    [TestMethod]
    public void EditableAxamlText_WhenInvalid_ShouldSetErrorAndFallbackPreview()
    {
        var viewModel = new ControlsAndLayoutViewModel();

        viewModel.EditableAxamlText = "<Grid xmlns=\"https://github.com/avaloniaui\"><UnknownControl /></Grid>";

        Assert.AreNotEqual(string.Empty, viewModel.ErrorText);
        Assert.IsInstanceOfType<TextBlock>(viewModel.CurrentPreview);
    }

    [TestMethod]
    public void EditableAxamlText_WhenEmpty_ShouldShowEmptyInputPreviewWithoutError()
    {
        var viewModel = new ControlsAndLayoutViewModel();

        viewModel.EditableAxamlText = "";

        Assert.AreEqual(string.Empty, viewModel.ErrorText);
        Assert.IsInstanceOfType<TextBlock>(viewModel.CurrentPreview);
    }

    [TestMethod]
    public void ShowCommands_ShouldChangePreviewAndCodeRowHeights()
    {
        var viewModel = new ControlsAndLayoutViewModel();

        viewModel.ShowPreviewCommand.Execute(null);
        Assert.AreEqual(1.0, viewModel.PreviewRowHeight.Value);
        Assert.AreEqual(GridUnitType.Star, viewModel.PreviewRowHeight.GridUnitType);
        Assert.AreEqual(0.0, viewModel.CodeRowHeight.Value);
        Assert.AreEqual(GridUnitType.Pixel, viewModel.CodeRowHeight.GridUnitType);

        viewModel.ShowCodeCommand.Execute(null);
        Assert.AreEqual(0.0, viewModel.PreviewRowHeight.Value);
        Assert.AreEqual(GridUnitType.Pixel, viewModel.PreviewRowHeight.GridUnitType);
        Assert.AreEqual(1.0, viewModel.CodeRowHeight.Value);
        Assert.AreEqual(GridUnitType.Star, viewModel.CodeRowHeight.GridUnitType);

        viewModel.ShowSplitCommand.Execute(null);
        Assert.AreEqual(1.0, viewModel.PreviewRowHeight.Value);
        Assert.AreEqual(GridUnitType.Star, viewModel.PreviewRowHeight.GridUnitType);
        Assert.AreEqual(1.0, viewModel.CodeRowHeight.Value);
        Assert.AreEqual(GridUnitType.Star, viewModel.CodeRowHeight.GridUnitType);
    }

    [TestMethod]
    public void AllViewModelSamples_ShouldRenderThroughRuntimeAxamlLoader()
    {
        var viewModel = new ControlsAndLayoutViewModel();
        var samples = viewModel.SampleGroups.SelectMany(group => group.Samples).ToArray();

        Assert.IsTrue(samples.Length > 0);
        foreach (var sample in samples)
        {
            viewModel.SelectedSample = sample;

            Assert.AreEqual(string.Empty, viewModel.ErrorText, sample.Title);
            Assert.IsInstanceOfType<Control>(viewModel.CurrentPreview, sample.Title);
        }
    }

    [TestMethod]
    public void AllSampleCodeConstants_ShouldParseToControls()
    {
        foreach (var item in GetSampleCodeConstants())
        {
            var loaded = AvaloniaRuntimeXamlLoader.Parse(item.AxamlText);

            Assert.IsInstanceOfType<Control>(loaded, item.Name);
        }
    }

    private static IEnumerable<(string Name, string AxamlText)> GetSampleCodeConstants()
    {
        return typeof(SampleCode)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(field => field.FieldType == typeof(string))
            .Select(field => (field.Name, AxamlText: (string)field.GetValue(null)!));
    }
}
