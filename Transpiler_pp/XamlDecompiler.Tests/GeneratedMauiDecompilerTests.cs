using XamlDecompiler.Core.Services;

namespace XamlDecompiler.Tests;

[TestClass]
public sealed class GeneratedMauiDecompilerTests
{
    private readonly GeneratedMauiDecompiler _decompiler = new();

    [TestMethod]
    public void Decompile_ContentPageSource_ReconstructsBasicXamlAndCodeBehind()
    {
        const string source = """
using System;
using System.CodeDom.Compiler;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Sample;

[XamlFilePath("Pages\\SamplePage.xaml")]
public class SamplePage : ContentPage
{
    [GeneratedCode("Microsoft.Maui.Controls.SourceGen", "1.0.0.0")]
    private Button ActionButton;

    public SamplePage()
    {
        InitializeComponent();
    }

    private void ActionButton_Clicked(object? sender, EventArgs e)
    {
    }

    [GeneratedCode("Microsoft.Maui.Controls.SourceGen", "1.0.0.0")]
    private void InitializeComponent()
    {
        Button button = new Button();
        StackLayout stackLayout = new StackLayout();
        SamplePage samplePage;
        NameScope nameScope = (NameScope)(NameScope.GetNameScope(samplePage = this) ?? new NameScope());
        NameScope.SetNameScope(samplePage, nameScope);
        button.transientNamescope = nameScope;
        ((INameScope)nameScope).RegisterName("ActionButton", (object)button);
        if (button.StyleId == null)
        {
            button.StyleId = "ActionButton";
        }
        button.SetValue(Button.TextProperty, "Run");
        button.Clicked += samplePage.ActionButton_Clicked;
        stackLayout.Children.Add(button);
        samplePage.SetValue(ContentPage.ContentProperty, stackLayout);
    }
}
""";

        var result = _decompiler.Decompile(source);

        StringAssert.Contains(result.XamlText, "<ContentPage");
        StringAssert.Contains(result.XamlText, "x:Class=\"Sample.SamplePage\"");
        StringAssert.Contains(result.XamlText, "<StackLayout>");
        StringAssert.Contains(result.XamlText, "<Button");
        StringAssert.Contains(result.XamlText, "x:Name=\"ActionButton\"");
        StringAssert.Contains(result.XamlText, "Text=\"Run\"");
        StringAssert.Contains(result.XamlText, "Clicked=\"ActionButton_Clicked\"");
        StringAssert.Contains(result.CodeBehindText, "public partial class SamplePage : ContentPage");
        StringAssert.Contains(result.CodeBehindText, "private void ActionButton_Clicked");
    }

    [TestMethod]
    public void Decompile_ApplicationSource_ReconstructsResourcesSection()
    {
        const string source = """
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Sample;

[XamlFilePath("App.xaml")]
public class App : Application
{
    private void InitializeComponent()
    {
        SewStyles item = new SewStyles();
        MonitorStyles item2 = new MonitorStyles();
        ResourceDictionary resourceDictionary = new ResourceDictionary();
        App app;
        NameScope value = (NameScope)(NameScope.GetNameScope(app = this) ?? new NameScope());
        NameScope.SetNameScope(app, value);
        resourceDictionary.MergedDictionaries.Add(item);
        resourceDictionary.MergedDictionaries.Add(item2);
        app.Resources = resourceDictionary;
    }
}
""";

        var result = _decompiler.Decompile(source);

        StringAssert.Contains(result.XamlText, "<Application.Resources>");
        StringAssert.Contains(result.XamlText, "<ResourceDictionary.MergedDictionaries>");
        StringAssert.Contains(result.XamlText, "<SewStyles />");
        StringAssert.Contains(result.XamlText, "<MonitorStyles />");
    }

    [TestMethod]
    public void Decompile_BindingSource_ReconstructsBindingMarkup()
    {
        const string source = """
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Sample;

[XamlFilePath("Pages\\BindingPage.xaml")]
public class BindingPage : ContentPage
{
    private void InitializeComponent()
    {
        BindingExtension bindingExtension = new BindingExtension();
        Label label = new Label();
        BindingPage bindingPage;
        NameScope nameScope = (NameScope)(NameScope.GetNameScope(bindingPage = this) ?? new NameScope());
        NameScope.SetNameScope(bindingPage, nameScope);
        bindingExtension.Path = "Title";
        BindingBase binding = ((IMarkupExtension<BindingBase>)bindingExtension).ProvideValue((IServiceProvider)null);
        label.SetBinding(Label.TextProperty, binding);
        bindingPage.SetValue(ContentPage.ContentProperty, label);
    }
}
""";

        var result = _decompiler.Decompile(source);

        StringAssert.Contains(result.XamlText, "Text=\"{Binding Title}\"");
        StringAssert.Contains(result.XamlText, "<Label");
    }

    [TestMethod]
    public void Decompile_MainPageInlineRowDefinitions_ReconstructsDefinitionPropertyElements()
    {
        const string source = """
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Sample;

[XamlFilePath("Pages\\MainPage.xaml")]
public class MainPage : ContentPage
{
    private void InitializeComponent()
    {
        Grid grid9 = new Grid();
        Grid grid5 = new Grid();
        MainPage mainPage;
        NameScope nameScope = (NameScope)(NameScope.GetNameScope(mainPage = this) ?? new NameScope());
        NameScope.SetNameScope(mainPage, nameScope);
        grid9.SetValue(Grid.RowDefinitionsProperty, new RowDefinitionCollection(new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Star)));
        grid5.SetValue(Grid.RowDefinitionsProperty, new RowDefinitionCollection(new RowDefinition(new GridLength(32.0)), new RowDefinition(GridLength.Auto), new RowDefinition(new GridLength(32.0)), new RowDefinition(GridLength.Auto)));
        grid9.Children.Add(grid5);
        mainPage.SetValue(ContentPage.ContentProperty, grid9);
    }
}
""";

        var result = _decompiler.Decompile(source);

        StringAssert.Contains(result.XamlText, "<Grid.RowDefinitions>");
        StringAssert.Contains(result.XamlText, "<RowDefinition");
        StringAssert.Contains(result.XamlText, "Height=\"Auto\"");
        StringAssert.Contains(result.XamlText, "Height=\"*\"");
        StringAssert.Contains(result.XamlText, "Height=\"32\"");
        Assert.IsFalse(result.XamlText.Contains("RowDefinitionCollection", StringComparison.Ordinal), result.XamlText);
    }

    [TestMethod]
    public void Decompile_MainPageInlineColumnDefinitions_ReconstructsMixedGridLengths()
    {
        const string source = """
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Sample;

[XamlFilePath("Pages\\MainPage.xaml")]
public class MainPage : ContentPage
{
    private void InitializeComponent()
    {
        Grid grid = new Grid();
        MainPage mainPage;
        NameScope nameScope = (NameScope)(NameScope.GetNameScope(mainPage = this) ?? new NameScope());
        NameScope.SetNameScope(mainPage, nameScope);
        grid.SetValue(Grid.ColumnDefinitionsProperty, new ColumnDefinitionCollection(new ColumnDefinition(GridLength.Star), new ColumnDefinition(new GridLength(30.0)), new ColumnDefinition(GridLength.Auto), new ColumnDefinition(new GridLength(2.0, GridUnitType.Star))));
        mainPage.SetValue(ContentPage.ContentProperty, grid);
    }
}
""";

        var result = _decompiler.Decompile(source);

        StringAssert.Contains(result.XamlText, "<Grid.ColumnDefinitions>");
        StringAssert.Contains(result.XamlText, "Width=\"*\"");
        StringAssert.Contains(result.XamlText, "Width=\"30\"");
        StringAssert.Contains(result.XamlText, "Width=\"Auto\"");
        StringAssert.Contains(result.XamlText, "Width=\"2*\"");
        Assert.IsFalse(result.XamlText.Contains("ColumnDefinitionCollection", StringComparison.Ordinal), result.XamlText);
    }

    [TestMethod]
    public void Decompile_MainPageLocalDoubleResources_ReconstructsResourceValues()
    {
        const string source = """
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Sample;

[XamlFilePath("Pages\\MainPage.xaml")]
public class MainPage : ContentPage
{
    private void InitializeComponent()
    {
        double num = 100.0;
        double num2 = 110.0;
        double num3 = 40.0;
        ResourceDictionary resourceDictionary = new ResourceDictionary();
        Grid grid9 = new Grid();
        MainPage mainPage;
        NameScope nameScope = (NameScope)(NameScope.GetNameScope(mainPage = this) ?? new NameScope());
        NameScope.SetNameScope(mainPage, nameScope);
        resourceDictionary.Add("WidthPdRangeGrid", num);
        resourceDictionary.Add("WidthNavButtonsGrid", num2);
        resourceDictionary.Add("WidthHandIcon", num3);
        mainPage.Resources.Add(resourceDictionary);
        mainPage.SetValue(ContentPage.ContentProperty, grid9);
    }
}
""";

        var result = _decompiler.Decompile(source);

        StringAssert.Contains(result.XamlText, "x:Key=\"WidthPdRangeGrid\"");
        StringAssert.Contains(result.XamlText, "100");
        StringAssert.Contains(result.XamlText, "x:Key=\"WidthNavButtonsGrid\"");
        StringAssert.Contains(result.XamlText, "110");
        StringAssert.Contains(result.XamlText, "x:Key=\"WidthHandIcon\"");
        StringAssert.Contains(result.XamlText, "40");
        Assert.IsFalse(result.Diagnostics.Any(diagnostic => diagnostic.Contains("WidthPdRangeGrid", StringComparison.Ordinal)), string.Join("\n", result.Diagnostics));
    }

    [TestMethod]
    public void Decompile_AppShellSource_ReconstructsShellContentChild()
    {
        const string source = """
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Sample;

[XamlFilePath("AppShell.xaml")]
public class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        base.Title = $"MOVIRUN® DiagnosticMonitoring - {AppInfo.Current.Version}";
        Routing.RegisterRoute("MonitorPage", typeof(MonitorPage));
        Routing.RegisterRoute("AboutPage", typeof(AboutPage));
    }

    [GeneratedCode("Microsoft.Maui.Controls.SourceGen", "1.0.0.0")]
    private void InitializeComponent()
    {
        DataTemplate value = new DataTemplate(typeof(MainPage));
        ShellContent shellContent = new ShellContent();
        AppShell appShell;
        NameScope value2 = (NameScope)(NameScope.GetNameScope(appShell = this) ?? new NameScope());
        NameScope.SetNameScope(appShell, value2);
        shellContent.transientNamescope = value2;
        appShell.SetValue(Shell.FlyoutBehaviorProperty, FlyoutBehavior.Disabled);
        shellContent.SetValue(Shell.NavBarIsVisibleProperty, true);
        shellContent.SetValue(BaseShellItem.TitleProperty, "Home");
        shellContent.SetValue(ShellContent.ContentTemplateProperty, value);
        shellContent.Route = "MainPage";
        ((ICollection<ShellItem>)appShell.GetValue(Shell.ItemsProperty)).Add((ShellItem)shellContent);
    }
}
""";

        var result = _decompiler.Decompile(source);

        StringAssert.Contains(result.XamlText, "<Shell");
        StringAssert.Contains(result.XamlText, "<ShellContent");
        StringAssert.Contains(result.XamlText, "Route=\"MainPage\"");
        StringAssert.Contains(result.XamlText, "Title=\"Home\"");
        StringAssert.Contains(result.XamlText, "ContentTemplate");
        Assert.IsFalse(result.Diagnostics.Any(diagnostic => diagnostic.Contains("ShellContent", StringComparison.Ordinal)), string.Join("\n", result.Diagnostics));
    }

    [TestMethod]
    public void Decompile_AboutHeaderContentView_ReconstructsNamedFontSizeValue()
    {
        const string source = """
using System.CodeDom.Compiler;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Sample;

[XamlFilePath("Pages\\AboutHeaderContentView.xaml")]
public class AboutHeaderContentView : ContentView
{
    [GeneratedCode("Microsoft.Maui.Controls.SourceGen", "1.0.0.0")]
    private Label HeaderLabel;

    [GeneratedCode("Microsoft.Maui.Controls.SourceGen", "1.0.0.0")]
    private void InitializeComponent()
    {
        Label label = new Label();
        Grid grid = new Grid();
        AboutHeaderContentView aboutHeaderContentView;
        NameScope nameScope = (NameScope)(NameScope.GetNameScope(aboutHeaderContentView = this) ?? new NameScope());
        NameScope.SetNameScope(aboutHeaderContentView, nameScope);
        label.transientNamescope = nameScope;
        ((INameScope)nameScope).RegisterName("HeaderLabel", (object)label);
        if (label.StyleId == null)
        {
            label.StyleId = "HeaderLabel";
        }

        HeaderLabel = label;
        label.SetValue(Label.FontSizeProperty, Device.GetNamedSize(NamedSize.Title, label.GetType()));
        grid.Children.Add(label);
        aboutHeaderContentView.SetValue(ContentView.ContentProperty, grid);
    }
}
""";

        var result = _decompiler.Decompile(source);

        StringAssert.Contains(result.XamlText, "x:Name=\"HeaderLabel\"");
        StringAssert.Contains(result.XamlText, "FontSize=\"Title\"");
        Assert.IsFalse(result.Diagnostics.Any(diagnostic => diagnostic.Contains("FontSize", StringComparison.Ordinal)), string.Join("\n", result.Diagnostics));
    }

    [TestMethod]
    public void Decompile_AboutPageStaticResourceStyle_ReconstructsGeneralResourceReference()
    {
        const string source = """
using System.CodeDom.Compiler;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls.Xaml.Internals;

namespace Sample;

[XamlFilePath("Pages\\AboutPage.xaml")]
public class AboutPage : ContentPage
{
    [GeneratedCode("Microsoft.Maui.Controls.SourceGen", "1.0.0.0")]
    private void InitializeComponent()
    {
        StaticResourceExtension staticResourceExtension = new StaticResourceExtension();
        Button button = new Button();
        StackLayout stackLayout = new StackLayout();
        AboutPage aboutPage;
        NameScope nameScope = (NameScope)(NameScope.GetNameScope(aboutPage = this) ?? new NameScope());
        NameScope.SetNameScope(aboutPage, nameScope);
        staticResourceExtension.Key = "LightweightButton";
        XamlServiceProvider xamlServiceProvider = new XamlServiceProvider();
        object obj = ((IMarkupExtension)staticResourceExtension).ProvideValue((IServiceProvider)xamlServiceProvider);
        button.SetValue(VisualElement.StyleProperty, (obj == null || !typeof(BindingBase).IsAssignableFrom(obj.GetType())) ? obj : obj);
        button.SetValue(Button.TextProperty, "SEW License");
        stackLayout.Children.Add(button);
        aboutPage.SetValue(ContentPage.ContentProperty, stackLayout);
    }
}
""";

        var result = _decompiler.Decompile(source);

        StringAssert.Contains(result.XamlText, "Style=\"{StaticResource LightweightButton}\"");
        StringAssert.Contains(result.XamlText, "Text=\"SEW License\"");
        Assert.IsFalse(result.Diagnostics.Any(diagnostic => diagnostic.Contains("StaticResource", StringComparison.Ordinal)), string.Join("\n", result.Diagnostics));
    }
}
