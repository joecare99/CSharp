using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.AxamlDesigner.UI.Controls;
using AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class AxamlPreviewResourcesTests
{
    [TestMethod]
    public void TextAlignment_PreviewIgnoresAmbiguousSources()
    {
        AxamlPreviewService service = new();
        foreach (string type in new[] { "TextBox", "TextBlock" })
        {
            foreach ((string attributes, TextAlignment expected) in new[]
            {
                ("TextAlignment='Right'", TextAlignment.Right),
                ("TextAlignment='2'", TextAlignment.Left),
                ("TextAlignment='{Binding Alignment}'", TextAlignment.Left),
                ("xmlns:e='urn:foreign' TextAlignment='Right' e:TextAlignment='Center'", TextAlignment.Left),
            })
            {
                string markup = $"<{type} {attributes} />";
                AxamlPreviewNode node = service.Parse(markup);
                AxamlPreviewControl preview = new() { PreviewNode = node };
                Grid wrapper = (Grid)preview.GetLogicalDescendants().OfType<Border>()
                    .Single(border => ReferenceEquals(border.Tag, node)).Child!;
                TextAlignment alignment = wrapper.Children[0] is TextBox textBox
                    ? textBox.TextAlignment
                    : ((TextBlock)wrapper.Children[0]).TextAlignment;
                Assert.AreEqual(expected, alignment, markup);
            }
        }
    }

    [TestMethod]
    public void TextBlockWrapping_PreviewUsesNativeDefaultAndSafeLiterals()
    {
        AxamlPreviewService service = new();
        foreach ((string markup, TextWrapping expected) in new[]
        {
            ("<TextBlock Text='example' />", new TextBlock().TextWrapping),
            ("<TextBlock Text='example' TextWrapping='Wrap' />", TextWrapping.Wrap),
            ("<TextBlock Text='example' TextWrapping='{Binding Broken' />", TextWrapping.NoWrap),
            ("<TextBlock xmlns:e='urn:foreign' TextWrapping='Wrap' e:TextWrapping='NoWrap' />", TextWrapping.NoWrap),
        })
        {
            AxamlPreviewNode node = service.Parse(markup);
            AxamlPreviewControl preview = new() { PreviewNode = node };
            Grid wrapper = (Grid)preview.GetLogicalDescendants().OfType<Border>()
                .Single(border => ReferenceEquals(border.Tag, node)).Child!;
            Assert.AreEqual(expected, ((TextBlock)wrapper.Children[0]).TextWrapping, markup);
        }
    }

    [TestMethod]
    public void TextBoxWrapping_PreviewUsesNativeDefaultAndSafeLiterals()
    {
        AxamlPreviewService service = new();
        foreach ((string markup, TextWrapping expected) in new[]
        {
            ("<TextBox />", new TextBox().TextWrapping),
            ("<TextBox TextWrapping='Wrap' />", TextWrapping.Wrap),
            ("<TextBox TextWrapping='2' />", TextWrapping.NoWrap),
            ("<TextBox TextWrapping='{Binding Wrapping}' />", TextWrapping.NoWrap),
            ("<TextBox xmlns:e='urn:foreign' TextWrapping='Wrap' e:TextWrapping='NoWrap' />", TextWrapping.NoWrap),
            ("<TextBox TextWrapping='Wrap'><TextBox.TextWrapping>Wrap</TextBox.TextWrapping></TextBox>", TextWrapping.NoWrap),
        })
        {
            AxamlPreviewNode node = service.Parse(markup);
            AxamlPreviewControl preview = new() { PreviewNode = node };
            Grid wrapper = (Grid)preview.GetLogicalDescendants().OfType<Border>()
                .Single(border => ReferenceEquals(border.Tag, node)).Child!;
            Assert.AreEqual(expected, ((TextBox)wrapper.Children[0]).TextWrapping, markup);
        }
    }

    [TestMethod]
    public void BorderGradient_UsesOnlyUnambiguousBoundedLiteralResources()
    {
        const string markup = """
            <StackPanel xmlns='https://github.com/avaloniaui' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
              <StackPanel.Resources>
                <LinearGradientBrush x:Key='Good' StartPoint='0,0' EndPoint='220,0'>
                  <LinearGradientBrush.GradientStops><GradientStop Offset='0' Color='#FF000040'/><GradientStop Offset='1' Color='#FF000000'/></LinearGradientBrush.GradientStops>
                </LinearGradientBrush>
                <LinearGradientBrush x:Key='Duplicate' StartPoint='0,0' EndPoint='1,0'/><x:String x:Key='Duplicate'>bad</x:String>
                <LinearGradientBrush x:Key='BadPoint' StartPoint='NaN,0' EndPoint='1,0'><LinearGradientBrush.GradientStops><GradientStop Offset='0' Color='#FF000000'/><GradientStop Offset='1' Color='#FFFFFFFF'/></LinearGradientBrush.GradientStops></LinearGradientBrush>
                <LinearGradientBrush x:Key='BadStop' StartPoint='0,0' EndPoint='1,0'><LinearGradientBrush.GradientStops><GradientStop Offset='0' Color='#FF000000'/><GradientStop Offset='2' Color='#FFFFFFFF'/></LinearGradientBrush.GradientStops></LinearGradientBrush>
                <LinearGradientBrush x:Key='Extra' StartPoint='0,0' EndPoint='1,0' Opacity='0.5'><LinearGradientBrush.GradientStops><GradientStop Offset='0' Color='#FF000000'/><GradientStop Offset='1' Color='#FFFFFFFF'/></LinearGradientBrush.GradientStops></LinearGradientBrush>
                <x:String x:Key='Alias'>{StaticResource Good}</x:String>
              </StackPanel.Resources>
              <Border Background='{DynamicResource ResourceKey=Good}'/>
              <Border Background='{StaticResource Duplicate}'/><Border Background='{StaticResource BadPoint}'/>
              <Border Background='{StaticResource BadStop}'/><Border Background='{StaticResource Extra}'/>
              <Border Background='{StaticResource Alias}'/><Border Background='{Binding Dangerous}'/>
              <Border Background='#FF123456'/>
              <StackPanel><StackPanel.Resources><x:String x:Key='Good'>shadow</x:String></StackPanel.Resources><Border Background='{StaticResource Good}'/></StackPanel>
              <StackPanel><StackPanel.Resources><ResourceDictionary/></StackPanel.Resources><Border Background='{StaticResource Good}'/></StackPanel>
            </StackPanel>
            """;
        AxamlPreviewNode root = new AxamlPreviewService().Parse(markup);
        AxamlPreviewControl preview = new() { PreviewNode = root };
        Border Rendered(AxamlPreviewNode node) => (Border)((Grid)preview.GetLogicalDescendants()
            .OfType<Border>().Single(border => ReferenceEquals(border.Tag, node)).Child!).Children[0];
        LinearGradientBrush brush = (LinearGradientBrush)Rendered(root.Children[0]).Background!;
        Assert.AreEqual(new RelativePoint(220, 0, RelativeUnit.Absolute), brush.EndPoint);
        Assert.AreEqual(2, brush.GradientStops.Count);
        Assert.AreEqual(Color.Parse("#FF000040"), brush.GradientStops[0].Color);
        Assert.AreEqual(1d, brush.GradientStops[1].Offset);
        Assert.AreEqual("{DynamicResource ResourceKey=Good}", root.Children[0].Properties["Background"]);
        foreach (AxamlPreviewNode child in root.Children.Skip(1).Take(6))
        {
            Assert.IsNull(Rendered(child).Background);
        }

        Assert.AreEqual(Color.Parse("#FF123456"), ((ISolidColorBrush)Rendered(root.Children[7]).Background!).Color);
        Assert.IsNull(Rendered(root.Children[8].Children[0]).Background);
        Assert.IsNull(Rendered(root.Children[9].Children[0]).Background);
    }

    [TestMethod]
    public void ReportedCalculatorShell_RendersApplicationSidebarGradient()
    {
        string root = Environment.GetEnvironmentVariable("AA98_CALCULATOR_SAMPLE_ROOT")
            ?? @"C:\Projekte\CSharp\Avalonia_Apps\AA05_CommandParCalc\AA05_CommandParCalc";
        string view = Path.Combine(root, "Views", "CommandParCalcView.axaml");
        if (!File.Exists(view))
        {
            Assert.Inconclusive("The external read-only calculator sample is not present.");
        }

        AxamlPreviewNode node = new AxamlPreviewService().Parse(File.ReadAllText(view), view, root);
        AxamlPreviewNode sidebar = Descendants(node).Single(n => n.TypeName == "Border");
        Assert.AreEqual("{DynamicResource ResourceKey=SideBarBackground}", sidebar.Properties["Background"]);
        AxamlPreviewControl preview = new() { PreviewNode = node };
        Grid wrapper = (Grid)preview.GetLogicalDescendants().OfType<Border>()
            .Single(border => ReferenceEquals(border.Tag, sidebar)).Child!;
        LinearGradientBrush brush = (LinearGradientBrush)((Border)wrapper.Children[0]).Background!;
        Assert.AreEqual(new RelativePoint(220, 0, RelativeUnit.Absolute), brush.EndPoint);
        Assert.AreEqual(4, brush.GradientStops.Count);
        Assert.AreEqual(Color.Parse("#FF000040"), brush.GradientStops[0].Color);
        Assert.AreEqual(0.7d, brush.GradientStops[2].Offset);
        AxamlPreviewNode home = Descendants(node).Single(n => n.StableName == "btnHome");
        Button menu = preview.GetLogicalDescendants().OfType<Button>()
            .Single(button => Equals(button.Content, home.Text));
        Assert.IsInstanceOfType<LinearGradientBrush>(menu.Background);
        LinearGradientBrush menuBrush = (LinearGradientBrush)menu.Background!;
        Assert.AreEqual(new RelativePoint(220, 0, RelativeUnit.Absolute), menuBrush.EndPoint);
        Assert.AreEqual(4, menuBrush.GradientStops.Count);
        Assert.AreEqual(Color.Parse("#FF000040"), menuBrush.GradientStops[0].Color);
    }

    [TestMethod]
    public void ButtonStyleGradient_RejectsConflictsAndRespectsOverridesAndRefresh()
    {
        string root = Path.Combine(Environment.CurrentDirectory, "preview-button-gradient-" + Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(Path.Combine(root, "Styles"));
            Directory.CreateDirectory(Path.Combine(root, "Views"));
            File.WriteAllText(Path.Combine(root, "Sample.csproj"), "<Project />");
            string document = Path.Combine(root, "Views", "View.axaml");
            File.WriteAllText(document, "");
            string app = Path.Combine(root, "App.axaml");
            const string appMarkup = """
                <Application xmlns='https://github.com/avaloniaui' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                  <Application.Resources>
                    <LinearGradientBrush x:Key='Gradient' StartPoint='0,0' EndPoint='220,0'>
                      <LinearGradientBrush.GradientStops><GradientStop Offset='0' Color='#FF000040'/><GradientStop Offset='1' Color='#FF000000'/></LinearGradientBrush.GradientStops>
                    </LinearGradientBrush>
                  </Application.Resources>
                  <Application.Styles><StyleInclude Source='Styles/Default.axaml'/></Application.Styles>
                </Application>
                """;
            File.WriteAllText(app, appMarkup);
            File.WriteAllText(Path.Combine(root, "Styles", "Default.axaml"), """
                <Styles xmlns='https://github.com/avaloniaui'>
                  <Style Selector='Button.Menu'><Setter Property='Background' Value='{DynamicResource Gradient}'/></Style>
                  <Style Selector='Button.Later'><Setter Property='Background' Value='#FF123456'/></Style>
                  <Style Selector='Button.Unknown'><Setter Property='Background' Value='{Binding Unsafe}'/></Style>
                </Styles>
                """);
            const string markup = """
                <UserControl xmlns='https://github.com/avaloniaui' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                  <StackPanel>
                    <Button Classes='Menu' Content='Good'/>
                    <Button Classes='Menu Later' Content='Later'/>
                    <Button Classes='Menu Unknown' Content='Unknown'/>
                    <Button Classes='Menu' Content='Literal' Background='#FFABCDEF'/>
                    <Button Classes='Menu' Content='Binding' Background='{Binding Unsafe}'/>
                    <Button Classes='Menu' Content='Property'><Button.Background><SolidColorBrush Color='Red'/></Button.Background></Button>
                    <StackPanel><StackPanel.Resources><x:String x:Key='Gradient'>shadow</x:String></StackPanel.Resources><Button Classes='Menu' Content='Shadow'/></StackPanel>
                    <StackPanel><StackPanel.Resources><ResourceDictionary/></StackPanel.Resources><Button Classes='Menu' Content='Dictionary'/></StackPanel>
                    <StackPanel><StackPanel.Styles><Style Selector='Button'><Setter Property='Background' Value='Blue'/></Style></StackPanel.Styles><Button Classes='Menu' Content='Local'/></StackPanel>
                  </StackPanel>
                </UserControl>
                """;
            AxamlPreviewNode node = new AxamlPreviewService().Parse(markup, document, root);
            AxamlPreviewNode[] buttons = Descendants(node).Where(n => n.TypeName == "Button").ToArray();
            Assert.IsNotNull(buttons[0].ButtonBackgroundGradient);
            Assert.IsFalse(buttons[0].Properties.ContainsKey("Background"));
            foreach (AxamlPreviewNode button in buttons.Skip(1))
            {
                Assert.IsNull(button.ButtonBackgroundGradient, button.Text);
            }

            Assert.AreEqual("#FF123456", buttons[1].RenderProperties["Background"]);
            Assert.AreEqual("#FFABCDEF", buttons[3].RenderProperties["Background"]);
            Assert.AreEqual("{Binding Unsafe}", buttons[4].Properties["Background"]);
            Assert.AreEqual("Blue", buttons[8].RenderProperties["Background"]);

            string manyResources = string.Concat(Enumerable.Range(0, 513)
                .Select(i => $"<x:String x:Key='K{i}'>value</x:String>"));
            string oversizedScope = markup.Replace("<StackPanel>",
                "<StackPanel><StackPanel.Resources>" + manyResources + "</StackPanel.Resources>");
            node = new AxamlPreviewService().Parse(oversizedScope, document, root);
            Assert.IsNull(Descendants(node).First(n => n.TypeName == "Button").ButtonBackgroundGradient);

            File.WriteAllText(app, appMarkup.Replace("</Application.Resources>",
                "<x:String x:Key='Gradient'>duplicate</x:String></Application.Resources>"));
            node = new AxamlPreviewService().Parse(markup, document, root);
            Assert.IsNull(Descendants(node).First(n => n.TypeName == "Button").ButtonBackgroundGradient);
            File.WriteAllText(app, appMarkup.Replace("EndPoint='220,0'", "EndPoint='NaN,0'"));
            node = new AxamlPreviewService().Parse(markup, document, root);
            Assert.IsNull(Descendants(node).First(n => n.TypeName == "Button").ButtonBackgroundGradient);
            File.WriteAllText(app, appMarkup.Replace("<LinearGradientBrush x:Key='Gradient' StartPoint='0,0' EndPoint='220,0'>",
                "<LinearGradientBrush x:Key='Gradient' StartPoint='0,0' EndPoint='220,0' Opacity='0.5'>"));
            node = new AxamlPreviewService().Parse(markup, document, root);
            Assert.IsNull(Descendants(node).First(n => n.TypeName == "Button").ButtonBackgroundGradient);
            File.WriteAllText(app, appMarkup.Replace("<LinearGradientBrush x:Key='Gradient' StartPoint='0,0' EndPoint='220,0'>",
                "<LinearGradientBrush x:Key='Other' StartPoint='0,0' EndPoint='220,0'>")
                .Replace("</Application.Resources>",
                    "<x:String x:Key='Gradient'>{StaticResource Other}</x:String></Application.Resources>"));
            node = new AxamlPreviewService().Parse(markup, document, root);
            Assert.IsNull(Descendants(node).First(n => n.TypeName == "Button").ButtonBackgroundGradient);
            File.WriteAllText(app, appMarkup.Replace("EndPoint='220,0'", "EndPoint='120,0'"));
            node = new AxamlPreviewService().Parse(markup, document, root);
            Assert.AreEqual(120d, Descendants(node).First(n => n.TypeName == "Button").ButtonBackgroundGradient!.EndX);
        }
        finally
        {
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    [TestMethod]
    public void ReportedCalculatorShell_LocalizesMenuAlongsideGradient()
    {
        string root = Environment.GetEnvironmentVariable("AA98_CALCULATOR_SAMPLE_ROOT")
            ?? @"C:\Projekte\CSharp\Avalonia_Apps\AA05_CommandParCalc\AA05_CommandParCalc";
        string view = Path.Combine(root, "Views", "CommandParCalcView.axaml");
        if (!File.Exists(view))
        {
            Assert.Inconclusive("The external read-only calculator sample is not present.");
        }

        CultureInfo previous = CultureInfo.CurrentUICulture;
        try
        {
            foreach ((string culture, string expected) in new[] { ("de-CH", "Anfang"), ("en", "Home"), ("", "Home") })
            {
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
                AxamlPreviewNode node = new AxamlPreviewService().Parse(File.ReadAllText(view), view, root);
                AxamlPreviewNode home = Descendants(node).Single(n => n.StableName == "btnHome");
                Assert.AreEqual(expected, home.Text);
                Assert.AreEqual("{x:Static p:Resources.btnHome}", home.Properties["Content"]);
                Assert.IsNotNull(home.ButtonBackgroundGradient);
            }
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }

    [TestMethod]
    public void GridBackground_RendersSafeResourcesAndIgnoresUnresolvedValues()
    {
        const string markup = """
            <StackPanel xmlns='https://github.com/avaloniaui' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
              <StackPanel.Resources><SolidColorBrush x:Key='Color' Color='#FF204020'/></StackPanel.Resources>
              <StackPanel.Styles><Style Selector='Grid'><Setter Property='Background' Value='{DynamicResource Color}'/></Style></StackPanel.Styles>
              <Grid/><Grid Background='#FF123456'/><Grid Background='{Binding Dangerous}'/>
              <Grid Background='{StaticResource Missing}'/><Grid Background='not-a-color'/>
            </StackPanel>
            """;
        AxamlPreviewNode root = new AxamlPreviewService().Parse(markup);
        AxamlPreviewControl preview = new() { PreviewNode = root };
        Grid[] grids = root.Children.Select(node => preview.GetLogicalDescendants().OfType<Border>()
            .Single(border => ReferenceEquals(border.Tag, node)).Child)
            .Cast<Grid>().Select(wrapper => (Grid)wrapper.Children[0]).ToArray();
        Assert.AreEqual(5, grids.Length);
        Assert.AreEqual(Color.Parse("#FF204020"), ((ISolidColorBrush)grids[0].Background!).Color);
        Assert.AreEqual(Color.Parse("#FF123456"), ((ISolidColorBrush)grids[1].Background!).Color);
        Assert.IsNull(grids[2].Background);
        Assert.IsNull(grids[3].Background);
        Assert.IsNull(grids[4].Background);
        Assert.AreEqual("{Binding Dangerous}", root.Children[2].Properties["Background"]);
    }

    [TestMethod]
    public void ToggleButtonPreview_DisplaysLiteralStateWithoutExecutingBindings()
    {
        AxamlPreviewNode root = new AxamlPreviewService().Parse(
            "<StackPanel><ToggleButton Content='Pin' IsChecked='True' IsThreeState='True' Command='{Binding PinCommand}' /></StackPanel>");
        AxamlPreviewControl preview = new() { PreviewNode = root };
        ToggleButton button = preview.GetLogicalDescendants().OfType<ToggleButton>().Single();
        Assert.AreEqual("Pin", button.Content);
        Assert.AreEqual(true, button.IsChecked);
        Assert.IsTrue(button.IsThreeState);
        Assert.IsFalse(button.IsEnabled);
        Assert.IsNull(button.Command);

        root = new AxamlPreviewService().Parse("<StackPanel><ToggleButton /></StackPanel>");
        preview = new AxamlPreviewControl { PreviewNode = root };
        button = preview.GetLogicalDescendants().OfType<ToggleButton>().Single();
        Assert.AreEqual(false, button.IsChecked);
    }

    [TestMethod]
    public void LocalStylesAndResources_AreInertScopedAndDoNotChangeSourceProperties()
    {
        const string markup = """
            <UserControl xmlns='https://github.com/avaloniaui' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:evil='clr-namespace:Evil'>
              <UserControl.Resources>
                <x:String x:Key='Caption'>Hello</x:String>
                <x:String x:Key='LoopA'>{StaticResource LoopB}</x:String>
                <x:String x:Key='LoopB'>{StaticResource LoopA}</x:String>
                <ResourceDictionary Source='https://example.invalid/resources.axaml' />
                <evil:Converter x:Key='Danger' />
              </UserControl.Resources>
              <UserControl.Styles><Style Selector='Button'><Setter Property='Margin' Value='-5'/><Setter Property='Padding' Value='10'/></Style>
                <Style Selector='Button.hot'><Setter Property='Margin' Value='2'/></Style>
                <Style Selector='Button:pointerover'><Setter Property='Padding' Value='99'/></Style>
                <StyleInclude Source='https://example.invalid/styles.axaml'/></UserControl.Styles>
              <UserControl.DataContext><evil:ViewModel/></UserControl.DataContext>
              <StackPanel><Button Classes='hot' Content='{StaticResource Caption}' Padding='3'/>
                <Button Content='{StaticResource LoopA}'/><Button Content='{StaticResource Danger}'/></StackPanel>
            </UserControl>
            """;
        AxamlPreviewNode root = new AxamlPreviewService().Parse(markup);
        Assert.AreEqual("StackPanel", root.TypeName);
        AxamlPreviewNode button = root.Children[0];
        Assert.AreEqual("Hello", button.Text);
        Assert.AreEqual("{StaticResource Caption}", button.Properties["Content"]);
        Assert.AreEqual("2", button.RenderProperties["Margin"]);
        Assert.AreEqual("3", button.RenderProperties["Padding"]);
        Assert.AreEqual("10", root.Children[1].RenderProperties["Padding"]);
        StringAssert.Contains(root.Children[1].Text!, "StaticResource LoopA");
        StringAssert.Contains(root.Children[2].Text!, "StaticResource Danger");
    }

    [TestMethod]
    public void ApplicationIncludes_UseOnlyBoundedLocalStylesAndLiteralResources()
    {
        string root = Path.Combine(Environment.CurrentDirectory, "preview-app-" + Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(Path.Combine(root, "Styles"));
            Directory.CreateDirectory(Path.Combine(root, "Views"));
            File.WriteAllText(Path.Combine(root, "Sample.csproj"), "<Project />");
            string document = Path.Combine(root, "Views", "View.axaml");
            File.WriteAllText(document, "");
            string app = Path.Combine(root, "App.axaml");
            File.WriteAllText(app, """
                <Application xmlns='https://github.com/avaloniaui' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                  <Application.Resources>
                    <SolidColorBrush x:Key='Color' Color='#FF204020'/>
                    <x:String x:Key='Alias'>{DynamicResource Color}</x:String>
                    <x:String x:Key='LoopA'>{StaticResource LoopB}</x:String>
                    <x:String x:Key='LoopB'>{StaticResource LoopA}</x:String>
                    <x:String x:Key='Duplicate'>A</x:String><x:String x:Key='Duplicate'>B</x:String>
                  </Application.Resources>
                  <Application.Styles>
                    <StyleInclude Source='Styles/Default.axaml'/>
                    <StyleInclude Source='Styles/Later.axaml'/>
                    <StyleInclude Source='Styles/Duplicate.axaml'/><StyleInclude Source='Styles/Duplicate.axaml'/>
                    <StyleInclude Source='Styles/../outside.axaml'/>
                    <StyleInclude Source='Styles/Dtd.axaml'/>
                    <StyleInclude Source='Styles/Extra.axaml' Other='ignored'/>
                    <StyleInclude Source='../outside.axaml'/><StyleInclude Source='https://example.invalid/a.axaml'/>
                  </Application.Styles>
                </Application>
                """);
            File.WriteAllText(Path.Combine(root, "Styles", "Default.axaml"), """
                <Styles xmlns='https://github.com/avaloniaui' xmlns:evil='clr-namespace:Evil'>
                  <Style Selector='Button.Number'><Setter Property='Background' Value='{DynamicResource Alias}'/>
                    <Setter Property='FontSize' Value='35'/><Setter Property='Margin' Value='5'/></Style>
                  <Style Selector='Button:pointerover'><Setter Property='FontSize' Value='99'/></Style>
                  <evil:Style Selector='Button.Number'><Setter Property='FontSize' Value='98'/></evil:Style>
                  <StyleInclude Source='Nested.axaml'/>
                </Styles>
                """);
            File.WriteAllText(Path.Combine(root, "Styles", "Duplicate.axaml"),
                "<Styles><Style Selector='Button.Number'><Setter Property='FontSize' Value='90'/></Style></Styles>");
            File.WriteAllText(Path.Combine(root, "Styles", "Later.axaml"),
                "<Styles><Style Selector='Button.Number'><Setter Property='FontSize' Value='36'/></Style></Styles>");
            File.WriteAllText(Path.Combine(root, "outside.axaml"),
                "<Styles><Style Selector='Button.Number'><Setter Property='FontSize' Value='91'/></Style></Styles>");
            File.WriteAllText(Path.Combine(root, "Styles", "Extra.axaml"),
                "<Styles><Style Selector='Button.Number'><Setter Property='FontSize' Value='92'/></Style></Styles>");
            File.WriteAllText(Path.Combine(root, "Styles", "Dtd.axaml"),
                "<!DOCTYPE Styles [<!ENTITY x '93'>]><Styles><Style Selector='Button.Number'><Setter Property='FontSize' Value='&x;'/></Style></Styles>");
            const string markup = """
                <UserControl xmlns='https://github.com/avaloniaui' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                  <UserControl.Styles><Style Selector='Button'><Setter Property='Margin' Value='-5'/></Style></UserControl.Styles>
                  <StackPanel><Button Classes='Number' Content='{StaticResource Color}'/>
                    <Button Classes='Number' FontSize='40' Background='{Binding Dangerous}' Content='{StaticResource LoopA}'/>
                    <Button Content='{StaticResource Duplicate}'/><Button Content='{StaticResource Missing}'/>
                    <StackPanel><StackPanel.Resources><x:String x:Key='Color'>local</x:String></StackPanel.Resources>
                      <Button Classes='Number' Content='{StaticResource Color}'/></StackPanel>
                    <StackPanel><StackPanel.Resources><ResourceDictionary><x:String x:Key='Color'>dictionary</x:String></ResourceDictionary></StackPanel.Resources>
                      <Button Classes='Number'/></StackPanel>
                  </StackPanel>
                </UserControl>
                """;
            AxamlPreviewNode preview = new AxamlPreviewService().Parse(markup, document, root);
            Assert.AreEqual("#FF204020", preview.Children[0].Text);
            Assert.AreEqual("#FF204020", preview.Children[0].RenderProperties["Background"]);
            Assert.AreEqual("36", preview.Children[0].RenderProperties["FontSize"]);
            Assert.AreEqual("-5", preview.Children[0].RenderProperties["Margin"]);
            Assert.AreEqual("40", preview.Children[1].RenderProperties["FontSize"]);
            Assert.AreEqual("{Binding Dangerous}", preview.Children[1].RenderProperties["Background"]);
            Assert.AreEqual("[StaticResource LoopA]", preview.Children[1].Text);
            Assert.AreEqual("[StaticResource Duplicate]", preview.Children[2].Text);
            Assert.AreEqual("[StaticResource Missing]", preview.Children[3].Text);
            Assert.AreEqual("local", preview.Children[4].Children[0].Text);
            Assert.IsFalse(preview.Children[4].Children[0].RenderProperties.ContainsKey("Background"));
            Assert.AreEqual("36", preview.Children[4].Children[0].RenderProperties["FontSize"]);
            Assert.IsFalse(preview.Children[5].Children[0].RenderProperties.ContainsKey("Background"));
            File.WriteAllText(app, "<Application xmlns='clr-namespace:Evil'><Application.Resources /></Application>");
            preview = new AxamlPreviewService().Parse(markup, document, root);
            Assert.AreEqual("[StaticResource Color]", preview.Children[0].Text);
            Assert.IsFalse(preview.Children[0].RenderProperties.ContainsKey("FontSize"));
            File.WriteAllText(app, "<!DOCTYPE Application [<!ENTITY x SYSTEM 'https://example.invalid/x'>]><Application>&x;</Application>");
            preview = new AxamlPreviewService().Parse(markup, document, root);
            Assert.AreEqual("[StaticResource Color]", preview.Children[0].Text);
        }
        finally
        {
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    [TestMethod]
    public void Resx_UsesCultureFallbackAndIgnoresTypedDataExternalAndDtd()
    {
        string root = Path.Combine(Environment.CurrentDirectory, "preview-resources-" + Guid.NewGuid().ToString("N"));
        CultureInfo previous = CultureInfo.CurrentUICulture;
        try
        {
            Directory.CreateDirectory(Path.Combine(root, "Properties"));
            Directory.CreateDirectory(Path.Combine(root, "Views"));
            File.WriteAllText(Path.Combine(root, "Sample.csproj"), "<Project />");
            string document = Path.Combine(root, "Views", "View.axaml");
            File.WriteAllText(document, "");
            File.WriteAllText(Path.Combine(root, "Properties", "Resources.resx"),
                "<root><data name='Neutral'><value>neutral</value></data><data name='Typed' type='System.String'><value>unsafe</value></data><data name='Duplicate'><value>A</value></data><data name='Duplicate' type='System.String'><value>B</value></data></root>");
            File.WriteAllText(Path.Combine(root, "Properties", "Resources.de.resx"),
                "<root><data name='Greeting'><value>Hallo</value></data></root>");
            File.WriteAllText(Path.Combine(root, "Properties", "Resources.de-CH.resx"),
                "<!DOCTYPE root [<!ENTITY x SYSTEM 'https://example.invalid/evil'>]><root><data name='Greeting'><value>&x;</value></data></root>");
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("de-CH");
            const string markup = """
                <StackPanel xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:p='clr-namespace:Sample.Properties'>
                  <Button Content='{x:Static p:Resources.Greeting}'/><Button Content='{x:Static p:Resources.Neutral}'/>
                  <Button Content='{x:Static p:Resources.Typed}'/><Button Content='{x:Static p:Resources.Duplicate}'/>
                  <Button Content='{x:Static p:Resources.Missing}'/>
                </StackPanel>
                """;
            AxamlPreviewNode preview = new AxamlPreviewService().Parse(markup, document, root);
            Assert.AreEqual("Hallo", preview.Children[0].Text);
            Assert.AreEqual("neutral", preview.Children[1].Text);
            Assert.AreEqual("{x:Static p:Resources.Typed}", preview.Children[2].Text);
            Assert.AreEqual("{x:Static p:Resources.Duplicate}", preview.Children[3].Text);
            Assert.AreEqual("{x:Static p:Resources.Missing}", preview.Children[4].Text);
            AxamlDesignerSession session = new();
            session.Load(markup);
            session.SetResourceContext(document, root);
            Assert.AreEqual("Hallo", session.Root!.Children[0].Text);
            session.SelectNode(session.Root.Children[0]);
            session.SelectProperty("Margin");
            Assert.IsTrue(session.ApplySelectedValue("4"));
            Assert.AreEqual("Hallo", session.Root!.Children[0].Text);
            AxamlPreviewNode noContext = new AxamlPreviewService().Parse(markup, document, Path.Combine(root, "Views"));
            Assert.AreEqual("{x:Static p:Resources.Greeting}", noContext.Children[0].Text);
            AxamlPreviewNode external = new AxamlPreviewService().Parse(markup.Replace("clr-namespace:Sample.Properties", "clr-namespace:Other.Properties"), document, root);
            Assert.AreEqual("{x:Static p:Resources.Greeting}", external.Children[0].Text);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    [TestMethod]
    public void DesignerContextChange_RefreshesPreviewAndPreservesEditHistory()
    {
        string root = Path.Combine(Environment.CurrentDirectory, "preview-context-" + Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(Path.Combine(root, "Properties"));
            Directory.CreateDirectory(Path.Combine(root, "Views"));
            File.WriteAllText(Path.Combine(root, "Sample.csproj"), "<Project />");
            string document = Path.Combine(root, "Views", "View.axaml");
            File.WriteAllText(document, "");
            File.WriteAllText(Path.Combine(root, "Properties", "Resources.resx"),
                "<root><data name='Caption'><value>Localized</value></data></root>");
            string markup = "<StackPanel xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' "
                + "xmlns:p='clr-namespace:Sample.Properties'>"
                + "<Button Width='100' Content='{x:Static p:Resources.Caption}'/></StackPanel>";
            AxamlDesignerViewModel designer = new();
            designer.MarkupChanged += (_, args) => markup = args.Markup;
            designer.LoadMarkup(markup);
            designer.SelectedNode = designer.PreviewRoot!.Children[0];
            designer.SelectedProperty = "Width";
            designer.SelectedValue = "120";
            designer.ApplySelectedValueCommand.Execute(null);
            Assert.IsTrue(designer.CanUndo);

            designer.SetResourceContext(document, root);
            Assert.AreEqual("Localized", designer.PreviewRoot!.Children[0].Text);
            Assert.AreEqual("120", designer.SelectedValue);
            Assert.IsTrue(designer.CanUndo);
            designer.UndoCommand.Execute(null);
            Assert.AreEqual("100", designer.SelectedValue);
            Assert.AreEqual("Localized", designer.PreviewRoot!.Children[0].Text);
            Assert.IsTrue(designer.CanRedo);
            designer.RedoCommand.Execute(null);
            Assert.AreEqual("120", designer.SelectedValue);

            designer.SetResourceContext(null, null);
            Assert.AreEqual("{x:Static p:Resources.Caption}", designer.PreviewRoot!.Children[0].Text);
            Assert.IsTrue(designer.CanUndo);
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, true);
            }
        }
    }

    [TestMethod]
    public void ReportedCalculator_ResolvesRealNeutralResxAndLocalStyle()
    {
        string root = Environment.GetEnvironmentVariable("AA98_CALCULATOR_SAMPLE_ROOT")
            ?? @"C:\Projekte\CSharp\Avalonia_Apps\AA05_CommandParCalc\AA05_CommandParCalc";
        string view = Path.Combine(root, "Views", "CommandParCalculatorView.axaml");
        if (!File.Exists(view))
        {
            Assert.Inconclusive("The external read-only calculator sample is not present.");
        }

        CultureInfo previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
            AxamlPreviewNode preview = new AxamlPreviewService().Parse(File.ReadAllText(view), view, root);
            AxamlPreviewNode button = Descendants(preview).Single(node => node.StableName == "btn7");
            Assert.AreEqual("7", button.Text);
            Assert.AreEqual("-5", button.RenderProperties["Margin"]);
            Assert.AreEqual("10", button.RenderProperties["Padding"]);
            Assert.AreEqual("35", button.RenderProperties["FontSize"]);
            Assert.AreEqual("#FF204020", button.RenderProperties["Background"]);
            Assert.AreEqual("#FF000020", preview.RenderProperties["Background"]);
            AxamlPreviewControl control = new() { PreviewNode = preview };
            Grid renderedGrid = control.GetLogicalDescendants().OfType<Grid>()
                .Single(grid => grid.RowDefinitions.Count == 6 && grid.ColumnDefinitions.Count == 10);
            Assert.AreEqual(Color.Parse("#FF000020"), ((ISolidColorBrush)renderedGrid.Background!).Color);
            Button renderedButton = control.GetLogicalDescendants().OfType<Button>()
                .Single(item => Equals(item.Content, "7"));
            Assert.AreEqual(new Thickness(-5), renderedButton.Margin);
            Assert.AreEqual(new Thickness(10), renderedButton.Padding);
            Assert.AreEqual("C/CE", Descendants(preview).Single(node => node.StableName == "btnC_CE").Text);
            Assert.AreEqual(".", Descendants(preview).Single(node => node.StableName == "btnDecp").Text);
            Assert.IsFalse(Descendants(preview).Any(node => node.TypeName is "DoubleValueConverter" or "CommandParCalculatorViewModel"));
            Assert.AreEqual(6, preview.RowDefinitions.Count);
            Assert.AreEqual(10, preview.ColumnDefinitions.Count);
            Assert.IsTrue(preview.RowDefinitions.All(value => value == "*"));
            Assert.IsTrue(preview.ColumnDefinitions.All(value => value == "*"));
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }

    private static System.Collections.Generic.IEnumerable<AxamlPreviewNode> Descendants(AxamlPreviewNode node) =>
        new[] { node }.Concat(node.Children.SelectMany(Descendants));
}
