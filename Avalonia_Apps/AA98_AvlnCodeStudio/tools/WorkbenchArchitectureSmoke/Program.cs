using AA98_AvlnCodeStudio.AxamlDesigner.UI.Controls;
using AA98_AvlnCodeStudio.Base.Debugging.Models;
using AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;
using AA98_AvlnCodeStudio.AxamlDesigner.UI.Views;
using AA98_AvlnCodeStudio.Host.Mcp;
using AA98_AvlnCodeStudio.Model.Documents;
using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.UI;
using AA98_AvlnCodeStudio.UI.Controls;
using AA98_AvlnCodeStudio.UI.Services;
using AA98_AvlnCodeStudio.UI.ViewModels;
using AA98_AxamlDesigner.Host.Commands;
using AA98_AxamlDesigner.Host.ViewModels;
using AA98_AxamlDesigner.Host.Views;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Code.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Project.Explorer.Avalonia.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ellipse = Avalonia.Controls.Shapes.Ellipse;
using LineShape = Avalonia.Controls.Shapes.Line;
using PolygonShape = Avalonia.Controls.Shapes.Polygon;
using PolylineShape = Avalonia.Controls.Shapes.Polyline;
using Rectangle = Avalonia.Controls.Shapes.Rectangle;

internal static class Program
{
    private static string s_root = null!;

    [STAThread]
    private static int Main(string[] args)
    {
        s_root = Directory.CreateDirectory(Path.Combine(
            Environment.CurrentDirectory, "AvlnWorkbenchArchitecture-" + Guid.NewGuid().ToString("N"))).FullName;
        bool resourcesOnly = args.Contains("--resources-only", StringComparer.Ordinal);
        bool quickInfoOnly = args.Contains("--quick-info-only", StringComparer.Ordinal);
        bool xunitTestMethodOnly = args.Contains("--xunit-test-method-only", StringComparer.Ordinal);
        bool testMethodOnly = xunitTestMethodOnly || args.Contains("--test-method-only", StringComparer.Ordinal);
        try
        {
            Environment.SetEnvironmentVariable("CONFIG_ROOT", Path.Combine(s_root, "config"));
            Environment.SetEnvironmentVariable("AVLN_CONFIG_GROUP_ROOT", null);
            Environment.SetEnvironmentVariable("AVLN_CONFIG_GROUP_ID", null);
            Environment.SetEnvironmentVariable("MSBUILDDISABLENODEREUSE", "1");
            Environment.SetEnvironmentVariable("DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER", "1");
            string fixture = Path.Combine(s_root, "fixture");
            Directory.CreateDirectory(fixture);
            File.WriteAllText(Path.Combine(fixture, "Sample.csproj"),
                "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>");
            if (quickInfoOnly || testMethodOnly)
            {
                if (quickInfoOnly)
                {
                    File.WriteAllText(Path.Combine(fixture, "Quick.cs"), "public class Sample { }");
                    File.WriteAllText(Path.Combine(fixture, "Other.cs"),
                        "public class Other { public int Value => 1; }");
                }
                else
                {
                    File.WriteAllText(Path.Combine(fixture, "Sample.csproj"), xunitTestMethodOnly
                        ? "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net10.0</TargetFramework>"
                        + "<OutputType>Exe</OutputType><IsTestProject>true</IsTestProject></PropertyGroup><ItemGroup>"
                        + "<PackageReference Include=\"Microsoft.NET.Test.Sdk\" Version=\"18.11.0\" />"
                        + "<PackageReference Include=\"xunit.v3.mtp-off\" Version=\"3.2.2\" />"
                        + "<PackageReference Include=\"xunit.runner.visualstudio\" Version=\"4.0.1\" /></ItemGroup></Project>"
                        :
                        "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net10.0</TargetFramework>"
                        + "<IsTestProject>true</IsTestProject></PropertyGroup><ItemGroup>"
                        + "<PackageReference Include=\"Microsoft.NET.Test.Sdk\" Version=\"18.11.0\" />"
                        + "<PackageReference Include=\"MSTest\" Version=\"4.5.1\" /></ItemGroup></Project>");
                    File.WriteAllText(Path.Combine(fixture, "MethodTests.cs"), xunitTestMethodOnly
                        ? """
                        using Xunit;
                        namespace Sample;
                        public class MethodTests
                        {
                            [Fact] public void Passing()
                            {
                                int value = 1;
                                Assert.Equal(1, value);
                            }
                            [Fact] public void PassingExtra() { }
                        }
                        """
                        :
                        """
                        using Microsoft.VisualStudio.TestTools.UnitTesting;
                        namespace Sample;
                        [TestClass] public class MethodTests
                        {
                            [TestMethod] public void Passing()
                            {
                                int value = 1;
                                Assert.AreEqual(1, value);
                            }
                            [TestMethod] public void PassingExtra() { }
                        }
                        """);
                }
                ProcessStartInfo restore = new("dotnet")
                {
                    WorkingDirectory = fixture,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };
                restore.ArgumentList.Add("restore");
                restore.ArgumentList.Add(Path.Combine(fixture, "Sample.csproj"));
                restore.ArgumentList.Add("--ignore-failed-sources");
                restore.Environment.Remove("MSBuildSDKsPath");
                restore.Environment.Remove("MSBuildExtensionsPath");
                using Process process = Process.Start(restore) ?? throw new InvalidOperationException("Could not restore the quick info fixture.");
                Task<string> output = process.StandardOutput.ReadToEndAsync();
                Task<string> error = process.StandardError.ReadToEndAsync();
                process.WaitForExit();
                Check(process.ExitCode == 0, $"Quick info fixture restore failed: {output.Result} {error.Result}");
            }
            File.WriteAllText(Path.Combine(fixture, "First.axaml"),
                "<StackPanel><TextBlock Text=\"First\" /></StackPanel>");
            File.WriteAllText(Path.Combine(fixture, "Second.axaml"),
                "<StackPanel xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"><TextBox x:Name=\"Input\" Width=\"100\" Text=\"Before\" /><TextBlock x:Name=\"Label\" Text=\"Label\" /></StackPanel>");
            File.WriteAllText(Path.Combine(fixture, "Host.axaml"),
                "<StackPanel><TextBox Width=\"100\" Text=\"Host\" /><Button Content=\"Other\" /></StackPanel>");
            File.WriteAllText(Path.Combine(fixture, "Grid.axaml"),
                "<av:Grid xmlns:av=\"https://github.com/avaloniaui\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" "
                + "RowDefinitions=\"*,*,*\" ColumnDefinitions=\"*,*,*,*\">"
                + "<av:Button x:Name=\"Span\" av:Grid.RowSpan=\"2\" av:Grid.ColumnSpan=\"2\" />"
                + "<av:TextBlock x:Name=\"Blocker\" av:Grid.Column=\"3\" Width=\"80\" /></av:Grid>");
            File.WriteAllText(Path.Combine(fixture, "Content.axaml"),
                "<Border xmlns=\"https://github.com/avaloniaui\"><Button /></Border>");
            File.WriteAllText(Path.Combine(fixture, "ControlPreviews.axaml"),
                "<StackPanel xmlns=\"https://github.com/avaloniaui\">"
                + "<ComboBox PlaceholderText=\"Choose an option\" PlaceholderForeground=\"DarkOrange\" IsDropDownOpen=\"True\" HorizontalContentAlignment=\"Right\" VerticalContentAlignment=\"Bottom\" Background=\"White\" BorderBrush=\"Blue\" BorderThickness=\"2\" CornerRadius=\"4\" FontSize=\"14\" FontStyle=\"Italic\" FontWeight=\"Medium\" Foreground=\"Black\" Padding=\"3\" />"
                + "<ComboBox PlaceholderText=\"Selector selection\" SelectedIndex=\"2\"><ComboBoxItem Content=\"First choice\" /><ComboBoxItem>Second choice</ComboBoxItem><ComboBoxItem><ComboBoxItem.Content><TextBlock Text=\"Property content\" /></ComboBoxItem.Content></ComboBoxItem></ComboBox>"
                + "<ComboBox PlaceholderText=\"Selector invalid\" SelectedIndex=\"-2\"><ComboBoxItem>First choice</ComboBoxItem></ComboBox>"
                + "<ComboBox PlaceholderText=\"Selector bound\" SelectedIndex=\"{Binding SelectedIndex}\"><ComboBoxItem Content=\"First choice\" /><ComboBoxItem Content=\"Second choice\" /></ComboBox>"
                + "<ComboBox PlaceholderText=\"Selector out of range\" SelectedIndex=\"5\"><ComboBoxItem Content=\"First choice\" /></ComboBox>"
                + "<ComboBox PlaceholderText=\"Selector markup values\" SelectedIndex=\"2\"><ComboBoxItem Content=\"{Binding ItemText}\" /><ComboBoxItem Content=\"{DynamicResource ItemText}\" /><ComboBoxItem Content=\"{StaticResource ItemText}\" /></ComboBox>"
                + "<ComboBox PlaceholderText=\"Selector empty item\" SelectedIndex=\"0\"><ComboBoxItem Content=\"\" /></ComboBox>"
                + "<ComboBox PlaceholderText=\"Selector complex content\" SelectedIndex=\"0\"><ComboBoxItem><ComboBoxItem.Content><StackPanel><TextBlock Text=\"Complex first\" /><TextBlock Text=\"Complex second\" /></StackPanel></ComboBoxItem.Content></ComboBoxItem></ComboBox>"
                + "<DatePicker SelectedDate=\"2028-02-29\" DayVisible=\"False\" MonthVisible=\"True\" YearVisible=\"False\" DayFormat=\"dd\" MonthFormat=\"MMM\" YearFormat=\"yy\" MinYear=\"2020-01-01\" MaxYear=\"2030-12-31\" Width=\"101\" />"
                + "<DatePicker SelectedDate=\"2028-02-30\" DayVisible=\"sometimes\" DayFormat=\"%\" MonthFormat=\"%\" YearFormat=\"%\" MinYear=\"2030-12-31\" MaxYear=\"2020-01-01\" Width=\"102\" />"
                + "<DatePicker SelectedDate=\"{Binding StartDate}\" DayVisible=\"{Binding ShowDay}\" DayFormat=\"{Binding DayFormat}\" MonthFormat=\"{Binding MonthFormat}\" YearFormat=\"{Binding YearFormat}\" MinYear=\"{Binding MinYear}\" MaxYear=\"{Binding MaxYear}\" Width=\"103\" />"
                + "<DatePicker SelectedDate=\"2040-01-01\" MinYear=\"2020-01-01\" MaxYear=\"2030-12-31\" Width=\"104\" />"
                + "<CalendarDatePicker SelectedDate=\"2028-02-29\" PlaceholderText=\"Choose a day\" PlaceholderForeground=\"DarkGray\" UseFloatingPlaceholder=\"True\" IsTodayHighlighted=\"False\" IsDropDownOpen=\"True\" Width=\"118\" />"
                + "<CalendarDatePicker PlaceholderText=\"{Binding Hint}\" PlaceholderForeground=\"Gradient\" UseFloatingPlaceholder=\"sometimes\" Width=\"119\" />"
                + "<CalendarDatePicker Width=\"120\" />"
                + "<Calendar SelectedDate=\"2028-02-29\" DisplayDate=\"2028-02-01\" DisplayDateStart=\"2028-01-01\" DisplayDateEnd=\"2028-12-31\" DisplayMode=\"Year\" SelectionMode=\"SingleDate\" FirstDayOfWeek=\"Sunday\" IsTodayHighlighted=\"False\" Width=\"108\" />"
                + "<Calendar SelectedDate=\"2028-02-30\" DisplayDateStart=\"2030-01-01\" DisplayDateEnd=\"2020-12-31\" DisplayMode=\"Century\" SelectionMode=\"Unsupported\" FirstDayOfWeek=\"Funday\" IsTodayHighlighted=\"sometimes\" Width=\"109\" />"
                + "<Calendar SelectedDate=\"{Binding Date}\" DisplayDate=\"{Binding DisplayDate}\" DisplayDateStart=\"{Binding Start}\" DisplayDateEnd=\"{Binding End}\" SelectionMode=\"{Binding Mode}\" FirstDayOfWeek=\"{Binding FirstDay}\" IsTodayHighlighted=\"{Binding Highlight}\" Width=\"110\" />"
                + "<Calendar SelectedDate=\"2040-01-01\" DisplayDateStart=\"2020-01-01\" DisplayDateEnd=\"2030-12-31\" Width=\"111\" />"
                + "<Calendar DisplayMode=\"Decade\" SelectionMode=\"MultipleRange\" Width=\"112\" />"
                + "<AutoCompleteBox Text=\"Par\" PlaceholderText=\"Find a city\" PlaceholderForeground=\"DarkGray\" FilterMode=\"Contains\" MinimumPrefixLength=\"2\" IsTextCompletionEnabled=\"True\" IsDropDownOpen=\"True\" Width=\"115\" />"
                + "<AutoCompleteBox Text=\"Grace\" FilterMode=\"Custom\" MinimumPrefixLength=\"-1\" IsTextCompletionEnabled=\"maybe\" PlaceholderForeground=\"Gradient\" Width=\"116\" />"
                + "<AutoCompleteBox Text=\"{Binding SearchText}\" PlaceholderText=\"{Binding SearchHint}\" FilterMode=\"{Binding Filter}\" MinimumPrefixLength=\"{Binding PrefixLength}\" IsTextCompletionEnabled=\"{Binding Complete}\" PlaceholderForeground=\"{Binding PlaceholderBrush}\" Width=\"117\" />"
                + "<AutoCompleteBox Width=\"121\" />"
                + "<ColorPicker Color=\"#FF123456\" ColorModel=\"Hsva\" IsAlphaEnabled=\"False\" IsColorSpectrumVisible=\"False\" Width=\"112\" />"
                + "<ColorPicker Color=\"NotAColor\" ColorModel=\"Unknown\" IsAlphaEnabled=\"maybe\" Width=\"113\" />"
                + "<ColorPicker Color=\"{Binding Accent}\" ColorModel=\"{Binding Model}\" IsAlphaEnabled=\"{Binding AlphaEnabled}\" Width=\"114\" />"
                + "<Image Source=\"https://example.invalid/never-load.png\" Stretch=\"UniformToFill\" Width=\"129\" Height=\"60\" />"
                + "<Image Source=\"{Binding Logo}\" Width=\"130\" Height=\"60\" />"
                + "<TabControl SelectedIndex=\"1\" Width=\"118\"><TabItem Header=\"General\"><TextBlock Text=\"Profile\" /></TabItem><TabItem Header=\"Advanced\" /></TabControl>"
                + "<TabControl SelectedIndex=\"-2\" Width=\"119\"><TabItem Header=\"Invalid\" /></TabControl>"
                + "<TabControl SelectedIndex=\"{Binding ActiveTab}\" Width=\"120\"><TabItem Header=\"Bound\" /></TabControl>"
                + "<Menu Width=\"121\"><MenuItem Header=\"File\" Icon=\"F\" ToggleType=\"CheckBox\" IsChecked=\"True\" IsSubMenuOpen=\"True\" StaysOpenOnClick=\"True\"><MenuItem Header=\"Open\" InputGesture=\"Ctrl+O\" HotKey=\"Ctrl+H\" ToggleType=\"Radio\" GroupName=\"EditorMode\" IsChecked=\"True\" /><Separator /><MenuItem Header=\"Exit\" HotKey=\"Ctrl+Q\" Command=\"{Binding ExitCommand}\" CommandParameter=\"File\" /></MenuItem></Menu>"
                + "<Menu Width=\"122\"><MenuItem Header=\"Invalid\" Icon=\"{Binding FileIcon}\" InputGesture=\"NotAKey\" HotKey=\"{Binding SaveHotKey}\" ToggleType=\"Unknown\" GroupName=\"{Binding MenuGroup}\" IsChecked=\"sometimes\" IsSubMenuOpen=\"{Binding Open}\" StaysOpenOnClick=\"{Binding StayOpen}\" Command=\"{Binding InvalidCommand}\" CommandParameter=\"{Binding FileName}\" /></Menu>"
                + "<Button Content=\"Context trigger\" Width=\"123\"><Button.ContextMenu><ContextMenu><MenuItem Header=\"Copy\" InputGesture=\"Ctrl+C\" /><Separator /></ContextMenu></Button.ContextMenu></Button>"
                + "<TextBox Text=\"Context target\" Width=\"124\"><TextBox.ContextMenu><ContextMenu><MenuItem Header=\"Paste\" /></ContextMenu></TextBox.ContextMenu></TextBox>"
                + "<Button Content=\"Tooltip target\" IsEnabled=\"True\" Width=\"125\"><Button.ToolTip><ToolTip><Border Padding=\"4\" BorderBrush=\"Blue\" CornerRadius=\"2\"><StackPanel Spacing=\"4\"><TextBlock Text=\"Helpful text\" /><TextBlock Text=\"More information\" /></StackPanel></Border></ToolTip></Button.ToolTip></Button>"
                + "<TreeView Width=\"126\" CornerRadius=\"4,2,4,2\" FontFamily=\"DejaVu Sans\" FontSize=\"13\" FontStyle=\"Italic\" FontWeight=\"SemiBold\" Foreground=\"DarkGreen\"><TreeViewItem Header=\"Project\" CornerRadius=\"3\" FontFamily=\"Arial\" FontSize=\"14\" FontStyle=\"Oblique\" FontWeight=\"Bold\" Foreground=\"Blue\" IsExpanded=\"True\"><TreeViewItem Header=\"src\"><TreeViewItem Header=\"Main.cs\" /></TreeViewItem></TreeViewItem></TreeView>"
                + "<TimePicker SelectedTime=\"13:45:30\" UseSeconds=\"True\" ClockIdentifier=\"12HourClock\" MinuteIncrement=\"5\" SecondIncrement=\"10\" Width=\"105\" />"
                + "<TimePicker SelectedTime=\"25:99\" UseSeconds=\"sometimes\" ClockIdentifier=\"Invalid\" MinuteIncrement=\"0\" SecondIncrement=\"60\" Width=\"106\" />"
                + "<TimePicker SelectedTime=\"{Binding StartTime}\" UseSeconds=\"{Binding ShowSeconds}\" ClockIdentifier=\"{Binding Clock}\" MinuteIncrement=\"{Binding Minute}\" SecondIncrement=\"{Binding Second}\" Width=\"107\" />"
                + "<NumericUpDown Value=\"12.5\" FormatString=\"N2\" Increment=\"0.25\" Minimum=\"0.5\" Maximum=\"40\" AllowSpin=\"False\" ShowButtonSpinner=\"False\" ClipValueToMinMax=\"True\" ButtonSpinnerLocation=\"Left\" TextAlignment=\"Center\" HorizontalContentAlignment=\"Center\" VerticalContentAlignment=\"Top\" ParsingNumberStyle=\"Integer\" Margin=\"3,4\" HorizontalAlignment=\"Right\" VerticalAlignment=\"Center\" Width=\"120\" Height=\"60\" MinWidth=\"40\" MaxWidth=\"160\" MinHeight=\"30\" MaxHeight=\"90\" Background=\"#FFF0F0F0\" BorderBrush=\"Green\" BorderThickness=\"2\" CornerRadius=\"3\" FontFamily=\"DejaVu Sans\" FontSize=\"15\" FontStyle=\"Italic\" FontWeight=\"Bold\" Foreground=\"DarkGreen\" PlaceholderForeground=\"LimeGreen\" Padding=\"2\" />"
                + "<NumericUpDown Value=\"0\" />"
                + "<NumericUpDown Value=\"1\" FormatString=\"Q\" ParsingNumberStyle=\"AllowHexSpecifier\" HorizontalAlignment=\"999\" VerticalAlignment=\"999\" HorizontalContentAlignment=\"999\" VerticalContentAlignment=\"999\" />"
                + "<DockPanel HorizontalSpacing=\"4\" VerticalSpacing=\"6\" LastChildFill=\"False\"><TextBlock Text=\"Docked preview\" /><Button Content=\"Docked button\" DockPanel.Dock=\"Bottom\" /></DockPanel>"
                + "<DockPanel HorizontalSpacing=\"-1\" VerticalSpacing=\"NaN\" LastChildFill=\"False\"><Button Content=\"Invalid dock\" DockPanel.Dock=\"99\" /></DockPanel>"
                + "<UniformGrid Rows=\"2\" Columns=\"3\" RowSpacing=\"3\" ColumnSpacing=\"5\"><Button Content=\"Uniform one\" /><Button Content=\"Uniform two\" /></UniformGrid>"
                + "<UniformGrid Columns=\"4\" RowSpacing=\"NaN\" ColumnSpacing=\"-1\" />"
                + "<Grid RowSpacing=\"2\" ColumnSpacing=\"4\" ShowGridLines=\"True\" Width=\"99\" />"
                + "<Grid RowSpacing=\"NaN\" ColumnSpacing=\"-1\" ShowGridLines=\"sometimes\" Width=\"98\" />"
                + "<Canvas Width=\"140\" Height=\"100\" Background=\"White\"><Button Content=\"Positioned\" Canvas.Left=\"15.5\" Canvas.Top=\"21\" /><TextBlock Text=\"Default position\" /></Canvas>"
                + "<Grid Width=\"141\" RowDefinitions=\"Auto,*,*\" ColumnDefinitions=\"*,*\"><Button Content=\"Grid cell\" Grid.Row=\"0\" Grid.Column=\"0\" Grid.RowSpan=\"2\" /><TextBlock Text=\"Grid neighbor\" Grid.Row=\"2\" Grid.Column=\"1\" /></Grid>"
                + "<ScrollViewer HorizontalScrollBarVisibility=\"Hidden\" VerticalScrollBarVisibility=\"Disabled\" Width=\"97\"><TextBlock Text=\"Scroll content\" /></ScrollViewer>"
                + "<ScrollViewer HorizontalScrollBarVisibility=\"Always\" VerticalScrollBarVisibility=\"None\" Width=\"96\" />"
                + "<Viewbox StretchDirection=\"UpOnly\" Width=\"93\"><TextBlock Text=\"Scale direction\" /></Viewbox>"
                + "<Viewbox StretchDirection=\"Sideways\" Width=\"92\"><TextBlock Text=\"Fallback direction\" /></Viewbox>"
                + "<WrapPanel Width=\"125\" ItemSpacing=\"5\" LineSpacing=\"7\" ItemWidth=\"40\" ItemHeight=\"20\" ItemsAlignment=\"Center\"><Button Content=\"First item\" /><Button Content=\"Second item\" /></WrapPanel>"
                + "<WrapPanel Width=\"126\" />"
                + "<WrapPanel Width=\"127\" Orientation=\"Vertical\" ItemSpacing=\"-1\" LineSpacing=\"NaN\" ItemWidth=\"-1\" ItemHeight=\"Infinity\" ItemsAlignment=\"999\" />"
                + "<ProgressBar Minimum=\"10\" Maximum=\"90\" Value=\"42.5\" IsIndeterminate=\"False\" ShowProgressText=\"True\" ProgressTextFormat=\"{}{1:0.0}%\" Background=\"White\" BorderBrush=\"Purple\" BorderThickness=\"1\" CornerRadius=\"2\" FontSize=\"13\" FontStyle=\"Normal\" FontWeight=\"Normal\" Foreground=\"Blue\" Padding=\"1\" />"
                + "<ProgressBar Value=\"35\" IsIndeterminate=\"True\" />"
                + "<ProgressBar Value=\"25\" ProgressTextFormat=\"{}{2:0}%\" />"
                + "<Slider Minimum=\"-10\" Maximum=\"60\" Value=\"35.5\" TickFrequency=\"5\" IsSnapToTickEnabled=\"True\" IsDirectionReversed=\"True\" TickPlacement=\"Outside\" Orientation=\"Vertical\" Background=\"#FFF0F0F0\" BorderBrush=\"Black\" BorderThickness=\"3\" CornerRadius=\"5\" FontSize=\"16\" FontStyle=\"Oblique\" FontWeight=\"SemiBold\" Foreground=\"Red\" Padding=\"4\" />"
                + "<CheckBox IsChecked=\"True\" IsThreeState=\"True\" Command=\"{Binding RememberMeCommand}\" CommandParameter=\"RememberMe\" HorizontalContentAlignment=\"Left\" VerticalContentAlignment=\"Center\" Background=\"#FFF0F0F0\" BorderBrush=\"Blue\" BorderThickness=\"2\" CornerRadius=\"1,2,3,4\" FontSize=\"16\" FontStyle=\"Italic\" FontWeight=\"Medium\" Foreground=\"White\" Padding=\"4\" />"
                + "<RadioButton IsChecked=\"False\" GroupName=\"Choices\" Command=\"{Binding SelectPersonalCommand}\" CommandParameter=\"Personal\" HorizontalContentAlignment=\"Center\" VerticalContentAlignment=\"Top\" Background=\"White\" BorderBrush=\"Purple\" BorderThickness=\"1\" CornerRadius=\"4\" FontSize=\"17\" FontStyle=\"Oblique\" FontWeight=\"Bold\" Foreground=\"Black\" Padding=\"3\" />"
                + "<Button Content=\"Keyboard action preview\" IsDefault=\"True\" IsCancel=\"True\" Command=\"{Binding SaveCommand}\" CommandParameter=\"DocumentId\" HorizontalContentAlignment=\"Left\" VerticalContentAlignment=\"Top\" ClipToBounds=\"True\" Opacity=\"0.8\" MinWidth=\"100\" MaxWidth=\"200\" MinHeight=\"30\" MaxHeight=\"60\" Background=\"#FFF0F0F0\" BorderBrush=\"Blue\" BorderThickness=\"2\" CornerRadius=\"6,4,2,8\" FontSize=\"18\" FontStyle=\"Italic\" FontWeight=\"Medium\" Foreground=\"White\" Padding=\"8,4\" />"
                + "<Button Content=\"Bound command parameter\" Command=\"{Binding LoadCommand}\" CommandParameter=\"{Binding DocumentId}\" Width=\"128\" />"
                + "<Button Content=\"Clip fallback\" ClipToBounds=\"maybe\" />"
                + "<Button Content=\"Hidden preview\" IsVisible=\"False\" Width=\"90\" />"
                + "<Button Content=\"Visibility fallback\" IsVisible=\"maybe\" Width=\"89\" />"
                + "<Border BorderBrush=\"Purple\" CornerRadius=\"12,4,8,2\" BorderThickness=\"2\" Background=\"#FFF0F0F0\" Padding=\"6\"><TextBlock Text=\"Border appearance\" /></Border>"
                + "<TextBlock Text=\"Alignment preview\" TextAlignment=\"Center\" FontStyle=\"Italic\" FontWeight=\"SemiBold\" LineHeight=\"24\" MaxLines=\"2\" Opacity=\"0.5\" TextTrimming=\"CharacterEllipsis\" TextWrapping=\"NoWrap\" Foreground=\"Red\" />"
                + "<TextBox Text=\"Input preview\" PlaceholderText=\"Enter a value\" PlaceholderForeground=\"DarkGray\" IsReadOnly=\"False\" MaxLength=\"120\" MinLines=\"2\" MaxLines=\"5\" LineHeight=\"22\" UseFloatingPlaceholder=\"True\" AcceptsReturn=\"True\" AcceptsTab=\"True\" HorizontalContentAlignment=\"Right\" VerticalContentAlignment=\"Bottom\" Background=\"#FFF0F0F0\" BorderBrush=\"Green\" BorderThickness=\"2\" CornerRadius=\"3,6,9,12\" FontSize=\"18\" FontStyle=\"Italic\" FontWeight=\"Medium\" HorizontalAlignment=\"Center\" Opacity=\"0.75\" Padding=\"8,4\" TextAlignment=\"Right\" TextWrapping=\"WrapWithOverflow\" VerticalAlignment=\"Bottom\" Foreground=\"Blue\" />"
                + "<TextBox Text=\"Bound placeholder preview\" PlaceholderText=\"{Binding Hint}\" />"
                + "<TextBox Text=\"Default line count preview\" />"
                + "<TextBox Text=\"Invalid line count preview\" MinLines=\"-1\" MaxLines=\"many\" LineHeight=\"NaN\" UseFloatingPlaceholder=\"sometimes\" PlaceholderForeground=\"Gradient\" />"
                + "<ToggleSwitch Content=\"Notifications\" IsChecked=\"True\" IsThreeState=\"True\" Command=\"{Binding ToggleNotificationsCommand}\" CommandParameter=\"Notifications\" OnContent=\"Enabled\" OffContent=\"Disabled\" HorizontalContentAlignment=\"Right\" VerticalContentAlignment=\"Center\" Background=\"PaleGreen\" BorderBrush=\"DarkGreen\" BorderThickness=\"3\" CornerRadius=\"5\" FontSize=\"15\" FontStyle=\"Italic\" FontWeight=\"SemiBold\" Foreground=\"DarkGreen\" Padding=\"2\" />"
                + "<Border FlowDirection=\"RightToLeft\"><TextBlock Text=\"Inherited RTL direction\" UseLayoutRounding=\"True\" /></Border>"
                + "<TextBlock Text=\"Invalid flow fallback\" FlowDirection=\"Sideways\" UseLayoutRounding=\"sometimes\" />"
                + "<TextBlock Text=\"Bound flow fallback\" FlowDirection=\"{Binding Direction}\" UseLayoutRounding=\"{Binding LayoutRounding}\" />"
                + "<ContentControl HorizontalContentAlignment=\"Stretch\" VerticalContentAlignment=\"Bottom\" Background=\"PaleGreen\" BorderBrush=\"Blue\" BorderThickness=\"2\" CornerRadius=\"3\" FontSize=\"16\" FontStyle=\"Italic\" FontWeight=\"Medium\" Foreground=\"Navy\" Padding=\"4\"><TextBlock Text=\"ContentControl alignment\" /></ContentControl>"
                + "<Expander Header=\"Expander alignment\" IsExpanded=\"True\" HorizontalContentAlignment=\"Right\" VerticalContentAlignment=\"Top\" Background=\"White\" BorderBrush=\"Green\" BorderThickness=\"1\" CornerRadius=\"4\" FontSize=\"17\" FontStyle=\"Oblique\" FontWeight=\"Bold\" Foreground=\"Black\" Padding=\"2\"><TextBlock Text=\"Expander content\" /></Expander>"
                + "<GroupBox Header=\"GroupBox alignment\" HorizontalContentAlignment=\"Left\" VerticalContentAlignment=\"Bottom\" Background=\"AliceBlue\" BorderBrush=\"Purple\" BorderThickness=\"3\" CornerRadius=\"5\" FontSize=\"15\" FontStyle=\"Italic\" FontWeight=\"SemiBold\" Foreground=\"DarkGreen\" Padding=\"6\"><TextBlock Text=\"GroupBox content\" /></GroupBox>"
                + "<ListBox SelectedIndex=\"1\" Background=\"Lavender\" BorderBrush=\"Orange\" BorderThickness=\"2\" CornerRadius=\"6\" FontSize=\"14\" FontStyle=\"Italic\" FontWeight=\"Medium\" Foreground=\"DarkBlue\" Padding=\"5\"><ListBoxItem Content=\"First item\" /><ListBoxItem>Second item</ListBoxItem></ListBox>"
                + "<ListBox Background=\"LightYellow\" SelectedIndex=\"-2\"><ListBoxItem>Invalid selection</ListBoxItem></ListBox>"
                + "<ListBox Background=\"LightCyan\" SelectedIndex=\"5\"><ListBoxItem Content=\"Out of range\" /></ListBox>"
                + "<ListBox Background=\"LightPink\" SelectedIndex=\"{Binding SelectedIndex}\"><TextBlock Text=\"Bound selection\" /></ListBox>"
                + "<ItemsControl Width=\"127\"><TextBlock Text=\"First static item\" /><Label Content=\"Second static item\" /></ItemsControl>"
                + "<Rectangle Width=\"90\" Height=\"36\" Fill=\"CornflowerBlue\" Stroke=\"DarkBlue\" StrokeThickness=\"2\" IsEnabled=\"True\" />"
                + "<Ellipse Width=\"36\" Height=\"36\" Fill=\"Orange\" />"
                + "<Line StartPoint=\"5,10\" EndPoint=\"80,40\" Stroke=\"DarkGreen\" StrokeThickness=\"3\" />"
                + "<Line StartPoint=\"{Binding StartPoint}\" EndPoint=\"10001,0\" Stroke=\"Purple\" />"
                + "<Polygon Points=\"40,0 80,60 0,60\" Fill=\"LightBlue\" Stroke=\"Navy\" StrokeThickness=\"2\" />"
                + "<Polygon Points=\"{Binding Points}\" Fill=\"Purple\" />"
                + "<Polyline Points=\"5,5 30,30 60,5\" Stroke=\"Blue\" StrokeThickness=\"2\" />"
                + "<Polyline Points=\"0,0\" Stroke=\"Orange\" />"
                + "<Label Content=\"Label alignment\" FontFamily=\"DejaVu Sans\" FontSize=\"18\" FontStyle=\"Italic\" FontWeight=\"SemiBold\" Foreground=\"Blue\" Background=\"#FFF0F0F0\" BorderBrush=\"Red\" BorderThickness=\"2\" CornerRadius=\"7\" Padding=\"8\" LineHeight=\"22\" MaxLines=\"2\" TextAlignment=\"Right\" TextTrimming=\"WordEllipsis\" TextWrapping=\"NoWrap\" HorizontalContentAlignment=\"Center\" VerticalContentAlignment=\"Top\" />"
                + "<Label Content=\"Invalid label layout\" LineHeight=\"0\" MaxLines=\"-1\" />"
                + "<Label Content=\"Bound label layout\" LineHeight=\"{Binding LineHeight}\" MaxLines=\"{Binding MaxLines}\" />"
                + "</StackPanel>");
            File.WriteAllText(Path.Combine(fixture, "Sample.cs"), "namespace Sample; public class Example { }");
            File.WriteAllBytes(Path.Combine(fixture, "Image.png"), [0x89, 0x50, 0x4e, 0x47, 0x00]);
            if (!testMethodOnly)
            {
                File.WriteAllBytes(Path.Combine(fixture, "Binary.cs"), [0x41, 0x00, 0x42]);
            }
            File.WriteAllBytes(Path.Combine(fixture, "Fixture.bin"), [0x00, 0xff, 0x42]);

            return AppBuilder.Configure<SmokeApplication>()
                .UsePlatformDetect()
                .AfterSetup(_ => Dispatcher.UIThread.Post(async () => await RunAsync(fixture, resourcesOnly, quickInfoOnly, testMethodOnly)))
                .StartWithClassicDesktopLifetime(resourcesOnly || quickInfoOnly || testMethodOnly ? [] : args);
        }
        finally
        {
            SmokeApplication.Services?.Dispose();
            for (int attempt = 0; ; attempt++)
            {
                try
                {
                    if (Directory.Exists(s_root))
                    {
                        Directory.Delete(s_root, recursive: true);
                    }
                    break;
                }
                catch (IOException) when (attempt < 50)
                {
                    Thread.Sleep(100);
                }
            }
        }
    }

    private static async Task RunAsync(string fixture, bool resourcesOnly, bool quickInfoOnly, bool testMethodOnly)
    {
        IClassicDesktopStyleApplicationLifetime desktop =
            (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
        int exitCode = 0;
        try
        {
            if (quickInfoOnly)
            {
                await VerifyQuickInfoAsync(fixture);
            }
            else if (testMethodOnly)
            {
                await VerifyExactTestMethodAsync(desktop, fixture);
            }
            else if (!resourcesOnly)
            {
                await VerifyWorkbenchAsync(desktop, fixture);
                await VerifyStandaloneAsync(desktop, fixture);
            }
            if (!quickInfoOnly && !testMethodOnly)
            {
                await VerifyReportedResourcePreviewAsync(desktop);
            }
            EditorDocumentWorkspace workspace = SmokeApplication.Services!.GetRequiredService<EditorDocumentWorkspace>();
            desktop.MainWindow!.Close();
            Check(workspace.Tabs.Count == 0, "Closing the workbench did not dispose its document tabs.");
            if (!resourcesOnly && !quickInfoOnly && !testMethodOnly)
            {
                Console.WriteLine("Architecture: navigation, catalog control previews, structure and Grid editing, designer contributions, binary editor, drafts, safe files, standalone review/undo, MCP dispatch and screenshot verified.");
            }
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception);
            exitCode = 1;
        }
        finally
        {
            foreach (Window window in desktop.Windows.ToArray())
            {
                window.DataContext = null;
                window.Close();
            }
        }

        desktop.Shutdown(exitCode);
    }

    private static async Task VerifyExactTestMethodAsync(IClassicDesktopStyleApplicationLifetime desktop, string fixture)
    {
        MainWindowViewModel workbench = SmokeApplication.Services!.GetRequiredService<MainWindowViewModel>();
        workbench.Workspace = new CSharpWorkspaceLoader().Load(Path.Combine(fixture, "Sample.csproj"));
        Window window = desktop.MainWindow!;
        TabItem projectTab = window.GetVisualDescendants().OfType<TabItem>()
            .First(item => string.Equals(item.Header?.ToString(), workbench.ProjectWorkflowTabText, StringComparison.Ordinal));
        projectTab.GetVisualAncestors().OfType<TabControl>().First().SelectedItem = projectTab;
        await WaitUntilAsync(() => window.GetVisualDescendants().OfType<TextBox>()
            .Any(control => AutomationProperties.GetName(control) == workbench.TestMethodNameText), "test method input");
        ComboBox projects = window.GetVisualDescendants().OfType<ComboBox>()
            .Single(control => AutomationProperties.GetName(control) == workbench.ProjectWorkflowProjectText);
        projects.SelectedItem = workbench.Workspace.Projects[0];
        await workbench.InspectSelectedProjectCommand.ExecuteAsync(null);
        Check(workbench.ProjectInspection?.TargetFramework == "net10.0", "Test project inspection failed.");
        Button discover = FindButton(window, workbench.TestCandidatesCommandText);
        await WaitUntilAsync(() => ReferenceEquals(discover.Command, workbench.FindTestCandidatesCommand) && discover.IsEnabled,
            "bound source test discovery");
        discover.Command!.Execute(discover.CommandParameter);
        Task discovery = workbench.FindTestCandidatesCommand.ExecutionTask
            ?? throw new InvalidOperationException("The source discovery command did not start.");
        await discovery.WaitAsync(TimeSpan.FromSeconds(90));
        Check(workbench.TestMethodCandidates.Count == 2,
            $"Expected two source candidates, got {workbench.TestMethodCandidates.Count}: {workbench.ProjectWorkflowStatus}");
        Button adapterDiscovery = FindButton(window, workbench.DiscoverAdapterTestsCommandText);
        await WaitUntilAsync(() => ReferenceEquals(adapterDiscovery.Command, workbench.DiscoverAdapterTestsCommand) && adapterDiscovery.IsEnabled,
            "bound adapter discovery");
        adapterDiscovery.Command!.Execute(adapterDiscovery.CommandParameter);
        Task adapterTask = workbench.DiscoverAdapterTestsCommand.ExecutionTask
            ?? throw new InvalidOperationException("The adapter discovery command did not start.");
        await adapterTask.WaitAsync(TimeSpan.FromSeconds(90));
        Check(workbench.DiscoveredTestCases.Count == 2,
            $"Expected two adapter-discovered cases: {workbench.ProjectWorkflowStatus}");
        ListBox discoveredCases = window.GetVisualDescendants().OfType<ListBox>()
            .Single(control => AutomationProperties.GetName(control) == workbench.DiscoveredTestCasesLabelText);
        Check(ReferenceEquals(discoveredCases.ItemsSource, workbench.DiscoveredTestCases),
            "Adapter test-case list is not bound.");
        discoveredCases.SelectedItem = workbench.DiscoveredTestCases.Single(
            testCase => testCase.FullyQualifiedName == "Sample.MethodTests.Passing");
        Button runCase = FindButton(window, workbench.RunDiscoveredTestCaseCommandText);
        await WaitUntilAsync(() => ReferenceEquals(runCase.Command, workbench.RunDiscoveredTestCaseCommand) && runCase.IsEnabled,
            "bound adapter case execution");
        runCase.Command!.Execute(runCase.CommandParameter);
        Task caseTask = workbench.RunDiscoveredTestCaseCommand.ExecutionTask
            ?? throw new InvalidOperationException("The adapter case command did not start.");
        await caseTask.WaitAsync(TimeSpan.FromSeconds(90));
        Check(workbench.ProjectTestResults.Count == 1
            && workbench.ProjectTestResults[0].Children.Count == 1
            && workbench.ProjectTestResults[0].Children[0].Label.Contains("Passed", StringComparison.Ordinal),
            $"Expected exactly one successful adapter case: {workbench.ProjectWorkflowStatus}");
        string? debugAdapter = Environment.GetEnvironmentVariable("AVLN_NETCOREDBG_PATH");
        if (!string.IsNullOrWhiteSpace(debugAdapter) && File.Exists(debugAdapter))
        {
            string sourcePath = Path.Combine(fixture, "MethodTests.cs");
            await workbench.EditorWorkspace.OpenOrActivateAsync(sourcePath);
            Check(workbench.EditorWorkspace.ActiveTab.TextView is EditorTextArea, "Test source editor was not opened.");
            EditorTextArea editor = (EditorTextArea)workbench.EditorWorkspace.ActiveTab.TextView!;
            int offset = (editor.Text ?? string.Empty).IndexOf("int value = 1;", StringComparison.Ordinal);
            Check(offset >= 0, "Test breakpoint source was not found.");
            editor.SetCaretOffset(offset);
            Button debug = FindButton(window, workbench.DebugDiscoveredTestCaseCommandText);
            await WaitUntilAsync(() => ReferenceEquals(debug.Command, workbench.DebugDiscoveredTestCaseCommand) && debug.IsEnabled,
                "bound adapter case debug action");
            string originalSource = workbench.Editor.Text;
            workbench.Editor.Text = originalSource + "\n// unsaved";
            await workbench.DebugDiscoveredTestCaseCommand.ExecuteAsync(null);
            Check(!workbench.HasDebugSession && workbench.ProjectWorkflowStatus.Contains("Save the active source file", StringComparison.Ordinal),
                "Test debugging did not reject an unsaved breakpoint source.");
            workbench.Editor.Text = originalSource;
            editor.SetCaretOffset(offset);
            debug.Command!.Execute(debug.CommandParameter);
            Task debugTask = workbench.DebugDiscoveredTestCaseCommand.ExecutionTask
                ?? throw new InvalidOperationException("The adapter case debug command did not start.");
            await WaitUntilAsync(() => workbench.ActiveDebugSessionState == DebugSessionState.Paused
                && workbench.DebugStackFrames.Any(frame => frame.SourcePath == sourcePath), "debugged adapter case breakpoint");
            await workbench.ContinueDebugSessionCommand.ExecuteAsync(null);
            await debugTask.WaitAsync(TimeSpan.FromSeconds(90));
            Check(!workbench.HasDebugSession && workbench.ProjectTestResults.Count == 1
                && workbench.ProjectTestResults[0].Children[0].Label.Contains("Passed", StringComparison.Ordinal),
                $"Debugged adapter case did not finish cleanly: {workbench.ProjectWorkflowStatus}");

            debug.Command!.Execute(debug.CommandParameter);
            Task cancelTask = workbench.DebugDiscoveredTestCaseCommand.ExecutionTask
                ?? throw new InvalidOperationException("The second adapter case debug command did not start.");
            await WaitUntilAsync(() => workbench.ActiveDebugSessionState == DebugSessionState.Paused,
                "debugged adapter case before cancellation");
            await workbench.StopDebugSessionCommand.ExecuteAsync(null);
            await cancelTask.WaitAsync(TimeSpan.FromSeconds(90));
            Check(!workbench.HasDebugSession, "Canceling test debugging left a debug session active.");

            debug.Command!.Execute(debug.CommandParameter);
            Task changedSelectionTask = workbench.DebugDiscoveredTestCaseCommand.ExecutionTask
                ?? throw new InvalidOperationException("The third adapter case debug command did not start.");
            await WaitUntilAsync(() => workbench.ActiveDebugSessionState == DebugSessionState.Paused,
                "debugged adapter case before selection change");
            discoveredCases.SelectedItem = null;
            await changedSelectionTask.WaitAsync(TimeSpan.FromSeconds(90));
            Check(!workbench.HasDebugSession && workbench.ProjectTestResults.Count == 0,
                "Changing the test selection left a session or stale result.");
            Console.WriteLine("Adapter case debugging: dirty source protection, caret breakpoint, stack, continue, stop and selection change verified.");
        }
        ListBox candidates = window.GetVisualDescendants().OfType<ListBox>()
            .Single(control => AutomationProperties.GetName(control) == workbench.TestCandidatesLabelText);
        candidates.SelectedItem = workbench.TestMethodCandidates.Single(
            candidate => candidate.FullyQualifiedName == "Sample.MethodTests.Passing");
        TextBox method = window.GetVisualDescendants().OfType<TextBox>()
            .Single(control => AutomationProperties.GetName(control) == workbench.TestMethodNameText);
        await WaitUntilAsync(() => method.Text == "Sample.MethodTests.Passing", "selected candidate in method input");
        await WaitUntilAsync(() => workbench.TestSelectedMethodCommand.CanExecute(null), "enabled exact test action");
        Button run = FindButton(window, workbench.TestMethodCommandText);
        await WaitUntilAsync(() => ReferenceEquals(run.Command, workbench.TestSelectedMethodCommand) && run.IsEnabled,
            "bound and enabled exact test button");
        run.Command!.Execute(run.CommandParameter);
        Task runTask = workbench.TestSelectedMethodCommand.ExecutionTask
            ?? throw new InvalidOperationException("The exact test button did not start its command.");
        await runTask.WaitAsync(TimeSpan.FromSeconds(90));
        Check(workbench.ProjectWorkflowStatus.Contains("(1 total)", StringComparison.Ordinal)
            && workbench.Editor.NotificationText.Contains("1 passed", StringComparison.Ordinal),
            $"Expected exactly one successful test: {workbench.ProjectWorkflowStatus} {workbench.Editor.NotificationText}");
        TreeView results = window.GetVisualDescendants().OfType<TreeView>()
            .Single(control => AutomationProperties.GetName(control) == workbench.ProjectTestResultsLabelText);
        Check(ReferenceEquals(results.ItemsSource, workbench.ProjectTestResults)
            && workbench.ProjectTestResults.Count == 1
            && workbench.ProjectTestResults[0].Children.Count == 1
            && workbench.ProjectTestResults[0].Children[0].Label.Contains("Passed", StringComparison.Ordinal),
            "Expected one bound passed test result in the project result tree.");
        Console.WriteLine("Source test candidates: discovery, selection and one matching test verified.");
    }

    private static async Task VerifyQuickInfoAsync(string fixture)
    {
        MainWindowViewModel workbench = SmokeApplication.Services!.GetRequiredService<MainWindowViewModel>();
        workbench.Workspace = new CSharpWorkspaceLoader().Load(Path.Combine(fixture, "Sample.csproj"));
        string path = Path.Combine(fixture, "Quick.cs");
        await workbench.EditorWorkspace.OpenOrActivateAsync(path);
        EditorTabViewModel tab = workbench.ActiveEditorTab;
        const string edited = "public class Sample { public int Count => 1; public int Read() => Count; }";
        tab.Editor.Text = edited;
        EditorTextArea editor = (EditorTextArea)tab.TextView!;
        editor.SetCaretOffset(edited.LastIndexOf("Count", StringComparison.Ordinal));
        AvaloniaEdit.TextEditor textEditor = editor.FindControl<AvaloniaEdit.TextEditor>("PART_Editor")!;
        textEditor.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Key = Key.F1,
            KeyModifiers = KeyModifiers.Control | KeyModifiers.Shift,
        });
        await WaitUntilAsync(() => workbench.QuickInfoText.Contains("Sample.Count", StringComparison.Ordinal), "quick info at caret");
        Check(desktopTextExists(workbench.QuickInfoText), "Quick info text is not visible in the workbench.");
        Check(File.ReadAllText(path) == "public class Sample { }", "Quick info or diagnostics saved an unsaved draft.");

        string otherPath = Path.Combine(fixture, "Other.cs");
        await workbench.EditorWorkspace.OpenOrActivateAsync(otherPath);
        EditorTabViewModel otherTab = workbench.ActiveEditorTab;
        const string otherDraft = """
            public class Other
            {
                public string Value => "";
            }
            """;
        otherTab.Editor.Text = otherDraft;
        workbench.EditorWorkspace.Activate(tab);
        const string crossFileSource = "public class Sample { public string Read(Other other) => other.Value; }";
        tab.Editor.Text = crossFileSource;
        editor.SetCaretOffset(crossFileSource.LastIndexOf("Value", StringComparison.Ordinal));
        await workbench.RequestQuickInfoCommand.ExecuteAsync(null);
        Check(workbench.QuickInfoText.Contains("string Other.Value", StringComparison.Ordinal),
            "Quick info did not use the other open file's unsaved source.");
        Check(File.ReadAllText(otherPath) == "public class Other { public int Value => 1; }",
            "Cross-file quick info saved an unsaved sibling buffer.");
        const string signatureSource =
            "public class Sample { void Run() { System.Console.WriteLine(1); } }";
        tab.Editor.Text = signatureSource;
        int signaturePosition = signatureSource.IndexOf("WriteLine(", StringComparison.Ordinal)
            + "WriteLine(".Length;
        editor.SetCaretOffset(signaturePosition);
        textEditor.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Key = Key.Space,
            KeyModifiers = KeyModifiers.Control | KeyModifiers.Shift,
        });
        await WaitUntilAsync(() => workbench.QuickInfoText.Contains("WriteLine(", StringComparison.Ordinal),
            "C# signature help");
        Check(workbench.QuickInfoText.Contains("Parameter 1:", StringComparison.Ordinal),
            "Signature help did not identify the active parameter.");

        tab.Editor.Text = edited;
        editor.SetCaretOffset(edited.LastIndexOf("Count", StringComparison.Ordinal));

        editor.SetCaretOffset(0);
        Check(!workbench.HasQuickInfo, "Moving the caret did not dismiss quick info.");
        editor.SetCaretOffset(edited.LastIndexOf("Count", StringComparison.Ordinal));
        Task staleRequest = workbench.RequestQuickInfoCommand.ExecuteAsync(null);
        editor.MoveCaretWithoutFocus(0);
        await staleRequest;
        Check(!workbench.HasQuickInfo, "An outdated quick info request was displayed after moving the caret.");
        await workbench.RequestQuickInfoCommand.ExecuteAsync(null);
        Check(workbench.HasQuickInfo, "The quick info command did not show a result.");
        workbench.CloseQuickInfoCommand.Execute(null);
        Check(!workbench.HasQuickInfo, "Close quick info did not hide the result.");
        editor.SetCaretOffset(edited.IndexOf("Read", StringComparison.Ordinal));
        int caretBeforeHover = editor.CaretOffset;
        int hoverOffset = edited.LastIndexOf("Count", StringComparison.Ordinal);
        AvaloniaEdit.Rendering.TextView textView = textEditor.TextArea.TextView;
        await WaitUntilAsync(() => textView.VisualLinesValid && textView.Bounds.Width > 0, "C# editor text layout");
        Window window = ((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!).MainWindow!;
        Pointer pointer = new(25, PointerType.Mouse, true);
        Point GetWindowPoint(int offset)
        {
            AvaloniaEdit.TextViewPosition position = new(textEditor.Document.GetLocation(offset));
            Point point = textView.GetVisualPosition(position, AvaloniaEdit.Rendering.VisualYPosition.TextMiddle)
                - textView.ScrollOffset + new Vector(1, 0);
            return textView.TranslatePoint(point, window)
                ?? throw new InvalidOperationException("The C# editor has no window coordinates.");
        }
        void RaiseHover(int offset, bool stopped = false)
        {
            if (stopped)
            {
                textEditor.RaiseEvent(new PointerEventArgs(
                    InputElement.PointerExitedEvent, textEditor, pointer, window, GetWindowPoint(offset), 0,
                    new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.Other), KeyModifiers.None));
            }
            else
            {
                textView.RaiseEvent(new PointerEventArgs(
                    InputElement.PointerMovedEvent, textView, pointer, window, GetWindowPoint(offset), 0,
                    new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.Other), KeyModifiers.None));
            }
        }

        RaiseHover(hoverOffset);
        await WaitUntilAsync(() => ToolTip.GetIsOpen(textEditor)
            && ToolTip.GetTip(textEditor) is TextBlock tip
            && tip.Text?.Contains("Sample.Count", StringComparison.Ordinal) == true, "C# symbol hover tooltip");
        Check(editor.CaretOffset == caretBeforeHover && !workbench.HasQuickInfo,
            "Hover changed the caret or displayed the keyboard quick-info panel.");
        Check(File.ReadAllText(path) == "public class Sample { }", "Hover saved an unsaved draft.");
        RaiseHover(hoverOffset, stopped: true);
        Check(!ToolTip.GetIsOpen(textEditor) && editor.HoverOffset is null, "Hover tooltip did not close on pointer movement.");
        RaiseHover(hoverOffset);
        RaiseHover(hoverOffset, stopped: true);
        await Task.Delay(750);
        Check(!ToolTip.GetIsOpen(textEditor) && editor.HoverOffset is null, "A stale hover request opened a tooltip.");

        CSharpSyntaxDiagnosticService languageService = SmokeApplication.Services!.GetRequiredService<CSharpSyntaxDiagnosticService>();
        SemaphoreSlim gate = (SemaphoreSlim)(typeof(CSharpSyntaxDiagnosticService)
            .GetField("_workspaceGate", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
            .GetValue(languageService) ?? throw new InvalidOperationException("The language service gate is unavailable."));
        bool hoverRequested = false;
        CancellationToken hoverToken = default;
        void CaptureHoverRequest(int _)
        {
            CancellationTokenSource cancellation = (CancellationTokenSource)(typeof(MainWindowViewModel)
                .GetField("_hoverQuickInfoCancellation", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
                .GetValue(workbench) ?? throw new InvalidOperationException("The hover request has no cancellation source."));
            hoverToken = cancellation.Token;
            hoverRequested = true;
        }

        await gate.WaitAsync();
        editor.HoverQuickInfoRequested += CaptureHoverRequest;
        try
        {
            RaiseHover(hoverOffset);
            await WaitUntilAsync(() => hoverRequested, "blocked hover request");
            Check(!ToolTip.GetIsOpen(textEditor), "A blocked hover request showed a tooltip.");
            RaiseHover(hoverOffset, stopped: true);
            Check(hoverToken.IsCancellationRequested, "Leaving the editor did not cancel the blocked hover request.");
        }
        finally
        {
            editor.HoverQuickInfoRequested -= CaptureHoverRequest;
            gate.Release();
        }
        await Task.Delay(250);
        Check(!ToolTip.GetIsOpen(textEditor) && editor.HoverOffset is null, "A cancelled hover request opened a tooltip.");

        RaiseHover(edited.IndexOf(" Count", StringComparison.Ordinal));
        Check(editor.HoverOffset is null && !ToolTip.GetIsOpen(textEditor), "Hover over whitespace opened a tooltip.");
        RaiseHover(hoverOffset);
        await WaitUntilAsync(() => ToolTip.GetIsOpen(textEditor), "hover before editing");
        tab.Editor.Text = edited + " ";
        Check(!ToolTip.GetIsOpen(textEditor) && editor.HoverOffset is null, "Editing did not close the hover tooltip.");

        const string completionSource = "public class Sample { void Run() { System.Console.WriteLien(); } }";
        tab.Editor.Text = completionSource;
        int completionOffset = completionSource.IndexOf("WriteLien", StringComparison.Ordinal) + "WriteL".Length;
        editor.SetCaretOffset(completionOffset);
        textEditor.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Key = Key.Space,
            KeyModifiers = KeyModifiers.Control,
        });
        Task completionTask = workbench.RequestCompletionsCommand.ExecutionTask
            ?? throw new InvalidOperationException("The completion command did not start.");
        await completionTask;
        await WaitUntilAsync(() => workbench.CompletionSuggestions.Any(item => item.DisplayText == "WriteLine"),
            "filtered completion suggestions");
        ListBox completionList = window.GetVisualDescendants().OfType<ListBox>()
            .Single(list => ReferenceEquals(list.ItemsSource, workbench.CompletionSuggestions));
        completionList.SelectedItem = workbench.CompletionSuggestions.First(item => item.DisplayText == "WriteLine");
        await workbench.ApplySelectedCompletionCommand.ExecuteAsync(null);
        Check(tab.Editor.Text.Contains("System.Console.WriteLine();", StringComparison.Ordinal)
            && editor.CaretOffset == completionSource.IndexOf("WriteLien", StringComparison.Ordinal) + "WriteLine".Length,
            "Completion did not apply the provider edit and caret position.");
        textEditor.Undo();
        Check(tab.Editor.Text == completionSource, "Completion was not a single undoable editor change.");
        editor.SetCaretOffset(completionOffset);
        await workbench.RequestCompletionsCommand.ExecuteAsync(null);
        Check(workbench.HasCompletionSuggestions, "Completion suggestions were not available after undo.");
        editor.MoveCaretWithoutFocus(0);
        Check(!workbench.HasCompletionSuggestions, "Moving the caret retained stale completion suggestions.");
        await workbench.ApplySelectedCompletionCommand.ExecuteAsync(null);
        Check(tab.Editor.Text == completionSource, "A stale completion changed the document.");
        editor.SetCaretOffset(completionOffset);
        Task staleCompletion = workbench.RequestCompletionsCommand.ExecuteAsync(null);
        editor.MoveCaretWithoutFocus(0);
        await staleCompletion;
        Check(!workbench.HasCompletionSuggestions, "An outdated completion request displayed suggestions.");
        Check(workbench.EditorWorkspace.TryClose(tab, discardProtected: true), "Could not discard the quick info fixture.");
        Check(workbench.EditorWorkspace.TryClose(otherTab, discardProtected: true), "Could not discard the sibling C# fixture.");
        Console.WriteLine("C# language: quick info, hover, provider completion, undo, stale state and draft preservation verified.");

        static bool desktopTextExists(string text) => Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime
            && lifetime.MainWindow!.GetVisualDescendants().OfType<TextBlock>()
                .Any(block => block.IsEffectivelyVisible && block.Text == text);
    }

    private static async Task VerifyReportedResourcePreviewAsync(IClassicDesktopStyleApplicationLifetime desktop)
    {
        const string root = @"C:\Projekte\CSharp\Avalonia_Apps\AA05_CommandParCalc\AA05_CommandParCalc";
        string path = Path.Combine(root, "Views", "CommandParCalculatorView.axaml");
        if (!File.Exists(path))
        {
            Console.WriteLine("Calculator preview smoke skipped: read-only sample is not present.");
            return;
        }

        MainWindowViewModel workbench = SmokeApplication.Services!.GetRequiredService<MainWindowViewModel>();
        workbench.Workspace = new CSharpWorkspaceLoader().Load(Path.Combine(root, "AA05_CommandParCalc.csproj"));
        await workbench.EditorWorkspace.OpenOrActivateAsync(path);
        EditorTabViewModel tab = workbench.ActiveEditorTab;
        await WaitUntilAsync(() => tab.View.GetVisualDescendants().OfType<AxamlPreviewControl>()
            .SelectMany(preview => preview.GetVisualDescendants().OfType<Button>())
            .Any(button => Equals(button.Content, "7")), "calculator Workbench caption");
        Button workbenchButton = tab.View.GetVisualDescendants().OfType<AxamlPreviewControl>()
            .SelectMany(preview => preview.GetVisualDescendants().OfType<Button>())
            .Single(button => Equals(button.Content, "7"));
        Check(workbenchButton.Margin == new Thickness(-5) && workbenchButton.Padding == new Thickness(10),
            "The Workbench calculator Button style was not rendered.");
        EditorTextArea editor = (EditorTextArea)tab.TextView!;
        int offset = tab.Editor.Text.IndexOf("x:Name=\"btn7\"", StringComparison.Ordinal);
        editor.SetCaretOffset(offset);
        await WaitUntilAsync(() => tab.Designer!.SelectedNode?.StableName == "btn7", "calculator Workbench source focus");
        AxamlPropertyRowViewModel widthDraft = tab.Designer!.PropertyRows.Single(row => row.Name == "Width");
        Check(!widthDraft.TrySetValue("-2"), "The calculator draft fixture was not rejected.");
        Check(editor.FindControl<AvaloniaEdit.TextEditor>("PART_Editor")!.TextArea.Focus(), "Workbench editor could not obtain focus.");
        Check(editor.IsKeyboardFocusWithin, "Workbench editor did not obtain focus before context change.");
        workbench.Workspace = null;
        Check(editor.IsKeyboardFocusWithin, "Workbench resource context change lost editor focus.");
        Check(editor.CaretOffset == offset, "Workbench resource context change lost caret position.");
        Check(tab.Designer.SelectedNode?.StableName == "btn7", "Workbench resource context change lost selection.");
        Check(tab.Designer.PropertyRows.Single(row => row.Name == "Width").Value == "-2",
            "Workbench resource context change lost draft.");
        workbench.Workspace = new CSharpWorkspaceLoader().Load(Path.Combine(root, "AA05_CommandParCalc.csproj"));
        Check(editor.IsKeyboardFocusWithin && tab.Designer.PropertyRows.Single(row => row.Name == "Width").Value == "-2",
            "Restoring the Workbench resource context lost focus or draft.");
        tab.Designer.DiscardAllPropertyDrafts();
        Check(!tab.IsDirty, "Calculator Workbench preview modified the source.");

        AxamlDesignerHostViewModel host = new();
        AA98_AxamlDesigner.Host.Views.MainWindow window = new() { DataContext = host };
        window.Show();
        try
        {
            Check(host.SetWorkspaceEntry(Path.Combine(root, "AA05_CommandParCalc.csproj")) && host.OpenDocument(path),
                "The standalone calculator could not be opened.");
            await WaitUntilAsync(() => window.GetVisualDescendants().OfType<AxamlPreviewControl>()
                .SelectMany(preview => preview.GetVisualDescendants().OfType<Button>())
                .Any(button => Equals(button.Content, "7")), "calculator standalone caption");
            Button standaloneButton = window.GetVisualDescendants().OfType<AxamlPreviewControl>()
                .SelectMany(preview => preview.GetVisualDescendants().OfType<Button>())
                .Single(button => Equals(button.Content, "7"));
            Check(standaloneButton.Margin == new Thickness(-5) && standaloneButton.Padding == new Thickness(10),
                "The standalone calculator Button style was not rendered.");
            EditorTextArea standaloneEditor = window.GetVisualDescendants().OfType<EditorTextArea>().Single();
            int standaloneOffset = host.Markup.IndexOf("x:Name=\"btn7\"", StringComparison.Ordinal);
            standaloneEditor.SetCaretOffset(standaloneOffset);
            await WaitUntilAsync(() => host.Designer.SelectedNode?.StableName == "btn7", "calculator standalone source focus");
            Check(standaloneEditor.FindControl<AvaloniaEdit.TextEditor>("PART_Editor")!.TextArea.Focus(), "Standalone editor could not obtain focus.");
            Check(standaloneEditor.IsKeyboardFocusWithin, "Standalone editor did not obtain focus before context change.");
            host.ClearWorkspaceContext();
            Check(standaloneEditor.IsKeyboardFocusWithin, "Standalone resource context change lost editor focus.");
            Check(standaloneEditor.CaretOffset == standaloneOffset, "Standalone resource context change lost caret position.");
            Check(host.Designer.SelectedNode?.StableName == "btn7", "Standalone resource context change lost selection.");
            Check(!host.IsDirty, "Calculator standalone preview modified the source.");
        }
        finally
        {
            host.ClearWorkspaceContext();
            window.DataContext = null;
            window.Close();
            workbench.Workspace = null;
        }
        Console.WriteLine("Calculator: Workbench/standalone caption, local style and source focus verified.");
    }

    private static async Task VerifyWorkbenchAsync(IClassicDesktopStyleApplicationLifetime desktop, string fixture)
    {
        MainWindowViewModel workbench = SmokeApplication.Services!.GetRequiredService<MainWindowViewModel>();
        Window window = desktop.MainWindow!;
        VerifyWorkspaceSidebarVisibility(window, workbench);
        await workbench.OpenWorkspaceFileAsync(Path.Combine(fixture, "Sample.csproj"));
        await VerifyProjectWorkflowAsync(window, workbench);
        ProjectExplorerViewModel explorer = workbench.ProjectExplorer!;
        ProjectExplorerItemViewModel first = FindItem(explorer.RootItems, Path.Combine(fixture, "First.axaml"));
        ProjectExplorerItemViewModel second = FindItem(explorer.RootItems, Path.Combine(fixture, "Second.axaml"));
        ProjectExplorerItemViewModel source = FindItem(explorer.RootItems, Path.Combine(fixture, "Sample.cs"));
        ProjectExplorerItemViewModel image = FindItem(explorer.RootItems, Path.Combine(fixture, "Image.png"));
        ProjectExplorerItemViewModel binary = FindItem(explorer.RootItems, Path.Combine(fixture, "Binary.cs"));
        ProjectExplorerItemViewModel fixtureBinary = FindItem(explorer.RootItems, Path.Combine(fixture, "Fixture.bin"));

        explorer.SelectedItem = first;
        Check(workbench.PropertySelectionCaption.StartsWith("Navigation - First.axaml", StringComparison.Ordinal),
            "Navigation properties did not update immediately.");
        await WaitForActiveAsync(workbench, first.Item.Path);
        Check(workbench.ActiveEditorTab.IsPreview, "Explorer selection did not create a preview.");

        explorer.SelectedItem = second;
        await WaitForActiveAsync(workbench, second.Item.Path);
        EditorTabViewModel secondTab = workbench.ActiveEditorTab;
        Check(workbench.EditorWorkspace.FindByPath(first.Item.Path) is null && secondTab.IsPreview,
            "The clean preview was not replaced.");
        await WaitUntilAsync(() => secondTab.View.GetVisualDescendants().OfType<AxamlPreviewControl>().Any(),
            "the document's designer surface");
        Check(secondTab.TextView is EditorTextArea, "The AXAML editor does not contain its code editor.");
        ((EditorTextArea)secondTab.TextView).FocusAndMoveCaret(new CodeLocation(second.Item.Path, 1, 1));
        Check(secondTab.IsPreview, "Programmatic caret navigation pinned the preview.");

        TabItem paletteTab = window.GetVisualDescendants().OfType<TabItem>()
            .First(item => string.Equals(item.Header?.ToString(), "Palette", StringComparison.Ordinal));
        TabControl leftTabs = paletteTab.GetVisualAncestors().OfType<TabControl>().First();
        leftTabs.SelectedItem = paletteTab;
        await WaitUntilAsync(() => window.GetVisualDescendants().Contains(secondTab.PaletteView),
            "the externally hosted palette");
        Check(!secondTab.View.GetVisualDescendants().Contains(secondTab.PaletteView),
            "The palette is still inside the document editor.");

        AxamlDesignerViewModel designer = secondTab.Designer!;
        Control palette = secondTab.PaletteView!;
        foreach (string groupHeader in new[] { "Daten und Auswahl", "Statische Items" })
        {
            Expander group = palette.GetVisualDescendants().OfType<Expander>()
                .Single(expander => string.Equals(expander.Header?.ToString(), groupHeader, StringComparison.Ordinal));
            group.IsExpanded = true;
        }

        static async Task VerifyProjectWorkflowAsync(Window window, MainWindowViewModel workbench)
        {
            TabItem projectTab = window.GetVisualDescendants().OfType<TabItem>()
                .First(item => string.Equals(item.Header?.ToString(), workbench.ProjectWorkflowTabText, StringComparison.Ordinal));
            TabControl tabs = projectTab.GetVisualAncestors().OfType<TabControl>().First();
            object? previousTab = tabs.SelectedItem;
            tabs.SelectedItem = projectTab;
            await WaitUntilAsync(() => window.GetVisualDescendants().OfType<ComboBox>()
                .Any(control => AutomationProperties.GetName(control) == workbench.ProjectWorkflowProjectText),
                "project workflow controls");
            ComboBox projectPicker = window.GetVisualDescendants().OfType<ComboBox>()
                .Single(control => AutomationProperties.GetName(control) == workbench.ProjectWorkflowProjectText);
            TextBox configuration = window.GetVisualDescendants().OfType<TextBox>()
                .Single(control => AutomationProperties.GetName(control) == workbench.ProjectWorkflowConfigurationText);
            Button inspect = window.GetVisualDescendants().OfType<Button>()
                .Single(control => string.Equals(control.Content?.ToString(), workbench.InspectProjectCommandText, StringComparison.Ordinal));
            Button debug = window.GetVisualDescendants().OfType<Button>()
                .Single(control => string.Equals(control.Content?.ToString(), workbench.DebugSelectedProjectCommandText, StringComparison.Ordinal));

            await WaitUntilAsync(() => ReferenceEquals(inspect.Command, workbench.InspectSelectedProjectCommand)
                && ReferenceEquals(debug.Command, workbench.DebugSelectedProjectCommand), "bound project workflow commands");
            Check(workbench.SelectedWorkflowProject is null && !inspect.Command!.CanExecute(null),
                $"A project was selected or inspected without an explicit user action: "
                + $"selected={workbench.SelectedWorkflowProject?.ProjectPath ?? "<null>"}, "
                + $"index={projectPicker.SelectedIndex}, inspectCanExecute={inspect.Command!.CanExecute(null)}.");
            projectPicker.SelectedItem = workbench.Workspace!.Projects[0];
            await WaitUntilAsync(() => inspect.Command!.CanExecute(null), "enabled project inspection");
            Check(ReferenceEquals(projectPicker.SelectedItem, workbench.SelectedWorkflowProject)
                && ReferenceEquals(inspect.Command, workbench.InspectSelectedProjectCommand),
                "Project selection and inspection command were not bound to the workbench.");
            await workbench.InspectSelectedProjectCommand.ExecuteAsync(null);
            Check(workbench.SelectedWorkflowFramework == "net10.0"
                && workbench.ProjectInspection is { CanDebug: false }
                && workbench.ProjectWorkflowStatus.Contains("not executable", StringComparison.Ordinal)
                && !debug.Command!.CanExecute(null),
                $"A library was not inspected safely: {workbench.ProjectWorkflowStatus}");
            configuration.Text = "Release";
            await WaitUntilAsync(() => workbench.ProjectInspection is null, "project context invalidation");
            Check(workbench.WorkflowConfiguration == "Release" && !debug.Command!.CanExecute(null),
                "Changing the project configuration did not invalidate the inspected target.");
            tabs.SelectedItem = previousTab;
        }

        static void VerifyWorkspaceSidebarVisibility(Window window, MainWindowViewModel workbench)
        {
            Grid root = (Grid)window.Content!;
            Grid layout = root.Children.OfType<Grid>().Single(child => Grid.GetRow(child) == 0);
            ColumnDefinition workspaceColumn = layout.ColumnDefinitions[0];
            ColumnDefinition splitterColumn = layout.ColumnDefinitions[1];
            Border workspaceSidebar = layout.Children.OfType<Border>()
                .Single(child => Grid.GetRow(child) == 1 && Grid.GetColumn(child) == 0);
            GridSplitter workspaceSplitter = layout.Children.OfType<GridSplitter>()
                .Single(child => Grid.GetRow(child) == 1 && Grid.GetColumn(child) == 1);
            Menu menu = window.GetVisualDescendants().OfType<Menu>().Single();
            MenuItem viewMenu = menu.Items.OfType<MenuItem>().Single(item => item.Header is "_View");
            MenuItem toggle = viewMenu.Items.OfType<MenuItem>()
                .Single(item => string.Equals(
                    item.Header?.ToString(),
                    workbench.WorkspaceSidebarMenuText,
                    StringComparison.Ordinal));

            Check(toggle.IsChecked
                && workspaceSidebar.IsVisible
                && workspaceSplitter.IsVisible
                && workspaceColumn.Width == new GridLength(280)
                && splitterColumn.Width == new GridLength(8),
                "The workspace sidebar did not start visible with its default width.");

            workspaceColumn.Width = new GridLength(360);
            Dispatcher.UIThread.RunJobs();
            Check(workbench.WorkspacePaneWidth == new GridLength(360),
                "Resizing the workspace pane did not update its remembered width.");

            workbench.IsWorkspacePaneVisible = false;
            Dispatcher.UIThread.RunJobs();
            Check(!toggle.IsChecked
                && !workspaceSidebar.IsVisible
                && !workspaceSplitter.IsVisible
                && workspaceColumn.Width == new GridLength(0)
                && splitterColumn.Width == new GridLength(0),
                "Hiding the workspace sidebar did not collapse its pane and splitter.");

            workbench.IsWorkspacePaneVisible = true;
            Dispatcher.UIThread.RunJobs();
            Check(toggle.IsChecked
                && workspaceSidebar.IsVisible
                && workspaceSplitter.IsVisible
                && workspaceColumn.Width == new GridLength(360)
                && splitterColumn.Width == new GridLength(8),
                "Showing the workspace sidebar did not restore its last width.");

            workspaceColumn.Width = new GridLength(280);
            Dispatcher.UIThread.RunJobs();
        }

        string[] paletteControlNames =
        [
            "AutoCompleteBox", "Calendar", "CalendarDatePicker", "ColorPicker", "DatePicker", "Image", "ItemsControl", "ListBox",
            "TimePicker", "TreeView", "ComboBoxItem", "ListBoxItem", "TreeViewItem",
            "Ellipse", "Line", "Polygon", "Polyline", "Rectangle",
        ];
        await WaitUntilAsync(() => paletteControlNames.All(controlName =>
            palette.GetVisualDescendants().OfType<Button>().Any(button =>
                string.Equals(button.Content?.ToString(), controlName, StringComparison.Ordinal)
                && button.Command is not null)), "catalogued controls in the external palette");

        TreeView structure = palette.GetVisualDescendants().OfType<TreeView>()
            .Single(tree => tree.Name == "StructureTree");
        structure.SelectedItem = designer.PreviewRoot!.Children[1];
        await WaitUntilAsync(() => designer.SelectedNode?.StableName == "Label", "programmatic structure selection");
        Check(secondTab.IsPreview
            && workbench.PropertySelectionCaption.StartsWith("Navigation - Second.axaml", StringComparison.Ordinal),
            "Programmatic structure selection incorrectly activated the editor.");
        await WaitUntilAsync(() => structure.GetVisualDescendants().OfType<TreeViewItem>()
            .Any(item => ReferenceEquals(item.DataContext, designer.SelectedNode)), "the selected structure item");
        TreeViewItem selectedStructureItem = structure.GetVisualDescendants().OfType<TreeViewItem>()
            .First(item => ReferenceEquals(item.DataContext, designer.SelectedNode));
        Check(selectedStructureItem.Focus(NavigationMethod.Tab), "The structure item could not receive keyboard focus.");
        await WaitUntilAsync(() => !secondTab.IsPreview
            && workbench.PropertySelectionCaption.Contains("Editor - ", StringComparison.Ordinal),
            "keyboard activation of the structure tree");
        Check(workbench.PropertySelectionCaption.Contains("TextBlock", StringComparison.Ordinal),
            "Structure activation did not expose the selected control's properties.");
        Invoke(secondTab.PaletteView!, "Up");
        await WaitUntilAsync(() => designer.PreviewRoot!.Children[0].StableName == "Label"
            && designer.SelectedNode?.StableName == "Label", "structure reorder");
        Check(secondTab.Editor.IsDirty && secondTab.Editor.Text.IndexOf("Label", StringComparison.Ordinal)
            < secondTab.Editor.Text.IndexOf("Input", StringComparison.Ordinal),
            "Structure reorder did not update the shared editor buffer.");
        designer.UndoCommand.Execute(null);
        await WaitUntilAsync(() => designer.PreviewRoot!.Children[1].StableName == "Label"
            && designer.SelectedNode?.StableName == "Label", "structure undo");
        Check(!secondTab.Editor.IsDirty && ReferenceEquals(structure.SelectedItem, designer.SelectedNode),
            $"Structure undo did not restore the clean buffer and tree selection (dirty={secondTab.Editor.IsDirty}, tree={((AxamlPreviewNode?)structure.SelectedItem)?.StableName ?? "<none>"}, designer={designer.SelectedNode?.StableName ?? "<none>"}).");

        designer.SelectedNode = designer.PreviewRoot!.Children[0];
        await WaitUntilAsync(() => workbench.SelectedEditorPropertyView is { } view
            && window.GetVisualDescendants().Contains(view), "the external property contribution");
        EditorTextArea sourceEditor = (EditorTextArea)secondTab.TextView;
        Check(sourceEditor.CaretOffset == designer.SelectedNode!.SourceStartOffset,
            "Tree selection did not synchronize the source caret.");
        string unchangedMarkup = secondTab.Editor.Text;
        sourceEditor.MoveCaretWithoutFocus(unchangedMarkup.IndexOf("Label", StringComparison.Ordinal));
        await WaitUntilAsync(() => designer.SelectedNode?.StableName == "Label"
            && ReferenceEquals(structure.SelectedItem, designer.SelectedNode),
            "source caret selection");
        Check(workbench.PropertySelectionCaption.Contains("TextBlock", StringComparison.Ordinal)
            && secondTab.Editor.Text == unchangedMarkup,
            "Source selection did not update the inspector or changed markup.");
        sourceEditor.MoveCaretWithoutFocus(unchangedMarkup.IndexOf("Input", StringComparison.Ordinal));
        await WaitUntilAsync(() => designer.SelectedNode?.StableName == "Input", "source caret return selection");
        designer.SelectedNode = designer.PreviewRoot!.Children[1];
        AxamlPreviewControl preview = secondTab.View.GetVisualDescendants().OfType<AxamlPreviewControl>().Single();
        await WaitUntilAsync(() => preview.GetVisualDescendants().OfType<Border>()
            .Any(border => ReferenceEquals(border.Tag, designer.PreviewRoot!.Children[0])
                && border.Bounds.Width > 0 && border.Bounds.Height > 0),
            "preview selection layout");
        Border inputWrapper = preview.GetVisualDescendants().OfType<Border>()
            .First(border => ReferenceEquals(border.Tag, designer.PreviewRoot!.Children[0]));
        Point inputPoint = inputWrapper.TranslatePoint(
            new Point(inputWrapper.Bounds.Width / 2, inputWrapper.Bounds.Height / 2), preview)
            ?? throw new InvalidOperationException("The preview input has no visible coordinates.");
        AxamlPreviewNode? hitNode = preview.FindNodeAtPoint(inputPoint);
        Check(ReferenceEquals(hitNode, designer.PreviewRoot!.Children[0]),
            $"The inert nested TextBox could not be hit-tested: got {hitNode?.TypeName ?? "none"} at {inputPoint}, wrapper {inputWrapper.Bounds}, preview {preview.Bounds}.");
        PointerPressedEventArgs press = new(
            inputWrapper,
            new Pointer(1, PointerType.Mouse, true),
            window,
            preview.TranslatePoint(inputPoint, window)
                ?? throw new InvalidOperationException("The preview has no window coordinates."),
            0,
            new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.LeftButtonPressed),
            KeyModifiers.None,
            1)
        {
            RoutedEvent = InputElement.PointerPressedEvent,
        };
        inputWrapper.RaiseEvent(press);
        Check(press.Handled,
            $"Preview pointer was not handled (selected={designer.SelectedNode?.StableName}, pressed={press.GetCurrentPoint(preview).Properties.IsLeftButtonPressed}, point={press.GetPosition(preview)}, expected={inputPoint}, routed={press.RoutedEvent}).");
        await WaitUntilAsync(() => designer.SelectedNode?.StableName == "Input"
            && ReferenceEquals(structure.SelectedItem, designer.SelectedNode), "preview pointer selection");
        Check(press.Handled && sourceEditor.CaretOffset == designer.SelectedNode!.SourceStartOffset
            && secondTab.Editor.Text == unchangedMarkup,
            "Preview selection did not synchronize the caret or changed the source.");
        AxamlPreviewNode itemsRoot = new AxamlPreviewService().Parse(
            "<StackPanel xmlns='https://github.com/avaloniaui'>"
            + "<ListBox Height='80'><ListBoxItem Content='One'/><ListBoxItem Content='Two'/></ListBox>"
            + "<TabControl Height='80'><TabItem Header='First'/><TabItem Header='Second'/></TabControl>"
            + "<TreeView Height='80'><TreeViewItem Header='Parent' IsExpanded='True'>"
            + "<TreeViewItem Header='Child'/></TreeViewItem></TreeView></StackPanel>");
        AxamlPreviewControl itemsPreview = new() { PreviewNode = itemsRoot };
        Window itemsWindow = new() { Width = 400, Height = 350, Content = itemsPreview };
        itemsWindow.Show();
        try
        {
            await WaitUntilAsync(() => itemsPreview.GetVisualDescendants().OfType<ListBox>()
                .Any(list => list.ContainerFromIndex(1) is { Bounds.Width: > 0, Bounds.Height: > 0 })
                && itemsPreview.GetVisualDescendants().OfType<TabControl>()
                    .Any(tabs => tabs.ContainerFromIndex(1) is { Bounds.Width: > 0, Bounds.Height: > 0 })
                && itemsPreview.GetVisualDescendants().OfType<TreeViewItem>()
                    .Any(item => ReferenceEquals(item.Tag, itemsRoot.Children[2].Children[0].Children[0])
                        && item.Bounds.Width > 0 && item.Bounds.Height > 0),
                "nested inert item layout");
            Control listItem = itemsPreview.GetVisualDescendants().OfType<ListBox>().Single().ContainerFromIndex(1)!;
            Control tabItem = itemsPreview.GetVisualDescendants().OfType<TabControl>().Single().ContainerFromIndex(1)!;
            Control treeItem = itemsPreview.GetVisualDescendants().OfType<TreeViewItem>()
                .Single(item => ReferenceEquals(item.Tag, itemsRoot.Children[2].Children[0].Children[0]));
            foreach ((Control item, AxamlPreviewNode expectedNode) in new[]
            {
                (listItem, itemsRoot.Children[0].Children[1]),
                (tabItem, itemsRoot.Children[1].Children[1]),
                (treeItem, itemsRoot.Children[2].Children[0].Children[0]),
            })
            {
                Point point = item.TranslatePoint(new Point(item.Bounds.Width / 2, item.Bounds.Height / 2), itemsPreview)
                    ?? throw new InvalidOperationException("The item has no preview coordinates.");
                Check(ReferenceEquals(itemsPreview.FindNodeAtPoint(point), expectedNode),
                    $"Inert {expectedNode.TypeName} was not selected at its visible item.");
            }
        }
        finally
        {
            itemsWindow.Close();
        }
        Check(!secondTab.IsPreview && workbench.PropertySelectionCaption.Contains("Editor - ", StringComparison.Ordinal),
            "Editor activation did not pin the tab and select its properties.");
        Check(!secondTab.View.GetVisualDescendants().Contains(workbench.SelectedEditorPropertyView),
            "The properties are still inside the document editor.");

        explorer.SelectedItem = source;
        secondTab.ActivateByUser(designerSelection: true);
        await Task.Delay(350);
        Check(ReferenceEquals(workbench.ActiveEditorTab, secondTab)
            && workbench.EditorWorkspace.FindByPath(source.Item.Path) is null,
            "A delayed explorer preview overrode a newer editor activation.");

        designer.PropertyEditor.Items.Single(item => item.PropertyItem.Name == "Width").ScalarValue = "-2";
        Check(designer.HasUnappliedPropertyDrafts, "Invalid property input was not retained.");
        Button closeTab = window.GetVisualDescendants().OfType<Button>()
            .Single(button => ReferenceEquals(button.DataContext, secondTab)
                && string.Equals(button.Content?.ToString(), "x", StringComparison.Ordinal));
        closeTab.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        await WaitUntilAsync(() => desktop.Windows.OfType<AA98_AvlnCodeStudio.UI.Views.DiscardWorkbenchChangesDialog>().Any(),
            "the workbench tab close prompt");
        Click(desktop.Windows.OfType<AA98_AvlnCodeStudio.UI.Views.DiscardWorkbenchChangesDialog>().Single(), "Cancel");
        await WaitUntilAsync(() => !desktop.Windows.OfType<AA98_AvlnCodeStudio.UI.Views.DiscardWorkbenchChangesDialog>().Any(),
            "the cancelled tab close prompt");
        Check(ReferenceEquals(workbench.EditorWorkspace.FindByPath(second.Item.Path), secondTab)
            && designer.HasUnappliedPropertyDrafts, "Cancelling tab close lost the property draft.");

        window.Close();
        await WaitUntilAsync(() => desktop.Windows.OfType<AA98_AvlnCodeStudio.UI.Views.DiscardWorkbenchChangesDialog>().Any(),
            "the workbench window close prompt");
        Click(desktop.Windows.OfType<AA98_AvlnCodeStudio.UI.Views.DiscardWorkbenchChangesDialog>().Single(), "Cancel");
        await WaitUntilAsync(() => !desktop.Windows.OfType<AA98_AvlnCodeStudio.UI.Views.DiscardWorkbenchChangesDialog>().Any(),
            "the cancelled workbench close prompt");
        Check(window.IsVisible && designer.HasUnappliedPropertyDrafts, "Cancelling workbench close lost the property draft.");

        designer.SelectedNode = designer.PreviewRoot.Children[1];
        Check(designer.UnappliedDrafts.Single().Value == "-2", "The draft was lost on selection change.");
        designer.SelectedNode = designer.PreviewRoot.Children[0];
        Check(designer.PropertyRows.Single(row => row.Name == "Width").Value == "-2",
            "The unchanged target did not recover its draft.");
        designer.SelectedNode = designer.PreviewRoot.Children[1];
        secondTab.Editor.Text = secondTab.Editor.Text.Replace("Width=\"100\"", "Width=\"120\"", StringComparison.Ordinal);
        designer.SelectedNode = designer.PreviewRoot!.Children[0];
        AxamlPropertyDraft retained = designer.UnappliedDrafts.Single();
        Check(retained.CanRestore && designer.RestoreDraft(retained), "Changed-source draft recovery was not explicit.");
        AxamlPropertyRowViewModel width = designer.PropertyRows.Single(row => row.Name == "Width");
        Check(width.Value == "-2", "Draft recovery changed the retained value.");
        designer.DiscardPropertyDraft(width);
        await secondTab.Editor.SaveCommand.ExecuteAsync(null);
        Check(!secondTab.Editor.IsDirty && File.ReadAllText(second.Item.Path).Contains("Width=\"120\"", StringComparison.Ordinal),
            "Saving did not preserve the shared code/designer buffer.");

        explorer.SelectedItem = first;
        explorer.SelectedItem = source;
        await WaitForActiveAsync(workbench, source.Item.Path);
        Check(workbench.PropertySelectionCaption.StartsWith("Navigation - Sample.cs", StringComparison.Ordinal),
            "Navigation did not retake property ownership.");
        Check(workbench.ActiveEditorTab.IsPreview && workbench.ActiveEditorTab.Designer is null,
            "A C# file did not use the text editor preview.");
        explorer.SelectedItem = first;
        await WaitForActiveAsync(workbench, first.Item.Path);
        Check(workbench.EditorWorkspace.FindByPath(source.Item.Path) is null
            && ReferenceEquals(secondTab, workbench.EditorWorkspace.FindByPath(second.Item.Path)),
            "Preview replacement discarded a pinned document or kept the previous preview.");
        await explorer.OpenAsync(first);
        Check(!workbench.ActiveEditorTab.IsPreview, "Explicit explorer activation did not pin the document.");
        Check(ReferenceEquals(leftTabs.SelectedItem, paletteTab), "Changing editors changed the user's palette selection.");

        byte[] imageBytes = File.ReadAllBytes(image.Item.Path);
        explorer.SelectedItem = image;
        await WaitForActiveAsync(workbench, image.Item.Path);
        Check(workbench.ActiveEditorTab.IsDiagnostic && !workbench.CanEditActiveDocument,
            "An unsupported image did not show a read-only diagnostic.");
        Check(File.ReadAllBytes(image.Item.Path).SequenceEqual(imageBytes), "The unsupported file was modified.");
        byte[] binaryBytes = File.ReadAllBytes(binary.Item.Path);
        explorer.SelectedItem = binary;
        await WaitForActiveAsync(workbench, binary.Item.Path);
        Check(workbench.ActiveEditorTab.IsDiagnostic && File.ReadAllBytes(binary.Item.Path).SequenceEqual(binaryBytes),
            "A binary file was opened or modified as text.");

        explorer.SelectedItem = fixtureBinary;
        await WaitForActiveAsync(workbench, fixtureBinary.Item.Path);
        EditorTabViewModel binaryTab = workbench.ActiveEditorTab;
        BinarySmokeSession session = ((BinarySmokeFactory)SmokeApplication.Services!
            .GetRequiredService<IDocumentEditorFactory<EditorTabViewModel>>()).LastSession!;
        Check(binaryTab.IsPreview && !binaryTab.HasTextEditor && session.Content.SequenceEqual(new byte[] { 0x00, 0xff, 0x42 }),
            "The registered binary editor did not open its own document state.");
        await WaitUntilAsync(() => window.GetVisualDescendants().Contains(binaryTab.View), "the registered binary editor view");
        session.ReplaceContent([0xfe, 0x00, 0x24]);
        Check(!binaryTab.IsPreview && binaryTab.IsDirty, "The binary editor did not pin before mutation.");
        Button saveButton = window.GetVisualDescendants().OfType<Button>()
            .Single(button => ReferenceEquals(button.Command, binaryTab.SaveCommand));
        Check(saveButton.IsEnabled && saveButton.Command!.CanExecute(null),
            "The binary editor Save button is disabled.");
        IInvokeProvider saveProvider = ControlAutomationPeer.CreatePeerForElement(saveButton)
            .GetProvider<IInvokeProvider>() ?? throw new InvalidOperationException("The Save button has no invoke provider.");
        saveProvider.Invoke();
        await WaitUntilAsync(
            () => !binaryTab.IsDirty && File.ReadAllBytes(fixtureBinary.Item.Path).SequenceEqual(new byte[] { 0xfe, 0x00, 0x24 }),
            "saving the registered binary editor");
        Button closeBinaryTab = window.GetVisualDescendants().OfType<Button>()
            .Single(button => ReferenceEquals(button.DataContext, binaryTab)
                && string.Equals(button.Content?.ToString(), "x", StringComparison.Ordinal));
        closeBinaryTab.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        await WaitUntilAsync(() => workbench.EditorWorkspace.FindByPath(fixtureBinary.Item.Path) is null,
            "closing the binary editor tab");
        Check(session.IsDisposed, "Closing the tab did not dispose the binary editor session.");

        ProjectExplorerItemViewModel gridItem = FindItem(explorer.RootItems, Path.Combine(fixture, "Grid.axaml"));
        explorer.SelectedItem = gridItem;
        await WaitForActiveAsync(workbench, gridItem.Item.Path);
        EditorTabViewModel gridTab = workbench.ActiveEditorTab;
        await WaitUntilAsync(() => window.GetVisualDescendants().Contains(gridTab.PaletteView)
            && gridTab.View.GetVisualDescendants().OfType<AxamlPreviewControl>().Any(), "the Grid designer");
        await VerifyGridEditingAsync(gridTab.Designer!, gridTab.PaletteView!, gridTab.View,
            () => gridTab.Editor.Text, () => gridTab.Editor.IsDirty);
        Check(!gridTab.IsPreview, "Grid editing did not protect the workbench preview tab.");

        ProjectExplorerItemViewModel contentItem = FindItem(explorer.RootItems, Path.Combine(fixture, "Content.axaml"));
        explorer.SelectedItem = contentItem;
        await WaitForActiveAsync(workbench, contentItem.Item.Path);
        EditorTabViewModel contentTab = workbench.ActiveEditorTab;
        await WaitUntilAsync(() => window.GetVisualDescendants().Contains(contentTab.PaletteView),
            "the single-content designer");
        await WaitUntilAsync(() => contentTab.Designer?.PreviewRoot is
            { TypeName: "Border", Children.Count: 1 },
            "the single-content preview");
        await VerifySingleContentRemovalAsync(contentTab.Designer!, contentTab.PaletteView!,
            () => contentTab.Editor.Text, () => contentTab.Editor.IsDirty);

        ProjectExplorerItemViewModel controlPreviewsItem = FindItem(
            explorer.RootItems,
            Path.Combine(fixture, "ControlPreviews.axaml"));
        explorer.SelectedItem = controlPreviewsItem;
        await WaitForActiveAsync(workbench, controlPreviewsItem.Item.Path);
        await VerifyCatalogControlPreviewsAsync(workbench.ActiveEditorTab.View);
    }

    private static async Task VerifyStandaloneAsync(IClassicDesktopStyleApplicationLifetime desktop, string fixture)
    {
        string path = Path.Combine(fixture, "Host.axaml");
        AxamlDesignerHostViewModel host = new();
        AA98_AxamlDesigner.Host.Views.MainWindow window = new() { DataContext = host };
        window.Show();
        Check(host.OpenDocument(path) && host.SetWorkspaceEntry(Path.Combine(fixture, "Sample.csproj")),
            "The standalone designer could not open the fixture.");
        await WaitUntilAsync(() => window.GetVisualDescendants().OfType<AxamlDesignerSurfaceView>()
            .Any(surface => surface.Bounds.Width > 0 && surface.Bounds.Height > 0), "the standalone designer surface");
        Check(window.GetVisualDescendants().OfType<AxamlDesignerPaletteView>().Any()
            && window.GetVisualDescendants().OfType<AxamlDesignerPropertiesView>().Any()
            && window.GetVisualDescendants().OfType<EditorTextArea>().Any(),
            "The standalone host did not compose the shared designer components and code editor.");
        AxamlDesignerPaletteView palette = window.GetVisualDescendants().OfType<AxamlDesignerPaletteView>().Single();
        TreeView structure = palette.GetVisualDescendants().OfType<TreeView>()
            .Single(tree => tree.Name == "StructureTree");
        string originalMarkup = host.Markup;
        structure.SelectedItem = host.Designer.PreviewRoot!.Children[1];
        await WaitUntilAsync(() => host.Designer.SelectedNode?.TypeName == "Button", "standalone structure selection");
        EditorTextArea standaloneEditor = window.GetVisualDescendants().OfType<EditorTextArea>().Single();
        Check(standaloneEditor.CaretOffset == host.Designer.SelectedNode!.SourceStartOffset,
            "Standalone tree selection did not synchronize the source caret.");
        standaloneEditor.MoveCaretWithoutFocus(originalMarkup.IndexOf("<TextBox", StringComparison.Ordinal));
        await WaitUntilAsync(() => host.Designer.SelectedNode?.TypeName == "TextBox"
            && ReferenceEquals(structure.SelectedItem, host.Designer.SelectedNode),
            "standalone source caret selection");
        Check(host.Markup == originalMarkup && !host.IsDirty,
            "Standalone caret selection changed the source.");
        standaloneEditor.MoveCaretWithoutFocus(originalMarkup.IndexOf("<Button", StringComparison.Ordinal));
        await WaitUntilAsync(() => host.Designer.SelectedNode?.TypeName == "Button", "standalone source return selection");
        Invoke(palette, "Up");
        await WaitUntilAsync(() => host.Designer.PreviewRoot!.Children[0].TypeName == "Button"
            && host.Designer.SelectedNode?.TypeName == "Button", "standalone structure reorder");
        Check(host.IsDirty && host.Markup.IndexOf("<Button", StringComparison.Ordinal)
            < host.Markup.IndexOf("<TextBox", StringComparison.Ordinal),
            "Standalone structure editing did not update the code buffer.");
        host.Designer.UndoCommand.Execute(null);
        Check(host.Markup == originalMarkup && !host.IsDirty, "Standalone structure undo did not restore the buffer.");
        host.Designer.RedoCommand.Execute(null);
        Check(host.IsDirty && host.Designer.SelectedNode?.TypeName == "Button", "Standalone structure redo failed.");
        host.Designer.UndoCommand.Execute(null);
        Check(host.Markup == originalMarkup && !host.IsDirty, "Standalone structure history did not restore the document.");
        byte[] png = await new AxamlDesignerScreenshotSource(host).CapturePngAsync(CancellationToken.None);
        Check(png.Length > 8 && png.AsSpan(0, 8).SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }),
            "The standalone MCP screenshot did not produce PNG data.");

        host.Designer.SelectedNode = host.Designer.PreviewRoot!.Children[0];
        AxamlPropertyRowViewModel width = host.Designer.PropertyRows.Single(row => row.Name == "Width");
        Check(!width.TrySetValue("-2") && host.HasUnsavedChanges, "The standalone draft was not protected.");
        Check(!host.OpenDocument(Path.Combine(fixture, "First.axaml")), "Opening silently discarded the standalone draft.");
        window.Close();
        await WaitUntilAsync(() => desktop.Windows.OfType<AxamlUnsavedChangesDialog>().Any(), "the standalone close prompt");
        AxamlUnsavedChangesDialog prompt = desktop.Windows.OfType<AxamlUnsavedChangesDialog>().Single();
        Check(!FindButton(prompt, "Save").IsEnabled, "An unapplied property draft was incorrectly offered as saved.");
        Click(prompt, "Cancel");
        await WaitUntilAsync(() => !desktop.Windows.OfType<AxamlUnsavedChangesDialog>().Any(), "the cancelled close prompt");
        Check(window.IsVisible && width.Value == "-2", "Cancelling close lost the standalone draft.");
        host.Designer.DiscardPropertyDraft(width);

        host.Markup = host.Markup.Replace("Text=\"Host\"", "Text=\"Saved\"", StringComparison.Ordinal);
        Check(host.SaveDocument() && !host.HasUnsavedChanges, "The standalone code editor did not save its buffer.");
        string saved = host.Markup;
        string proposed = saved.Replace("Text=\"Saved\"", "Text=\"Reviewed\"", StringComparison.Ordinal);
        WorkspaceFileChangeProposal proposal = host.PrepareWorkspaceChanges(
        [
            WorkspaceFileChangeRequest.ReplaceExisting("Host.axaml", saved, proposed, expectedOpenDocumentContent: saved),
        ]);
        Task<bool> reviewing = host.ReviewWorkspaceFileChangesAsync!(proposal, CancellationToken.None);
        await WaitUntilAsync(() => desktop.Windows.OfType<WorkspaceFileChangeReviewDialog>().Any(), "standalone review");
        Click(desktop.Windows.OfType<WorkspaceFileChangeReviewDialog>().Single(), "Cancel");
        Check(!await reviewing && host.Markup == saved && File.ReadAllText(path) == saved,
            "Cancelling the standalone review changed the document.");

        Task<bool> staleReviewing = host.ReviewWorkspaceFileChangesAsync(proposal, CancellationToken.None);
        await WaitUntilAsync(() => desktop.Windows.OfType<WorkspaceFileChangeReviewDialog>().Any(), "stale standalone review");
        File.WriteAllText(path, "External edit");
        WorkspaceFileChangeReviewDialog staleReview = desktop.Windows.OfType<WorkspaceFileChangeReviewDialog>().Single();
        Click(staleReview, "Apply 1 change(s)");
        await WaitUntilAsync(() => staleReview.GetVisualDescendants().OfType<TextBlock>().Any(text =>
            text.Text?.Contains("not fully applied", StringComparison.Ordinal) == true), "the standalone review conflict");
        Click(staleReview, "Cancel");
        Check(!await staleReviewing && host.Markup == saved && File.ReadAllText(path) == "External edit",
            "The standalone conflict review overwrote the external edit.");

        File.WriteAllText(path, saved);
        proposal = host.PrepareWorkspaceChanges(
        [
            WorkspaceFileChangeRequest.ReplaceExisting("Host.axaml", saved, proposed, expectedOpenDocumentContent: saved),
        ]);
        reviewing = host.ReviewWorkspaceFileChangesAsync(proposal, CancellationToken.None);
        await WaitUntilAsync(() => desktop.Windows.OfType<WorkspaceFileChangeReviewDialog>().Any(), "repeated standalone review");
        Click(desktop.Windows.OfType<WorkspaceFileChangeReviewDialog>().Single(), "Apply 1 change(s)");
        Check(await reviewing && host.Markup == proposed && File.ReadAllText(path) == proposed,
            "The repeated standalone review did not synchronize the open document.");
        Check(host.WorkspaceFileChangeService.Undo() && host.Markup == saved, "Standalone workspace undo failed.");
        Check(host.WorkspaceFileChangeService.Redo() && host.Markup == proposed, "Standalone workspace redo failed.");

        string mcpPath = Path.Combine(fixture, "Mcp.axaml");
        File.WriteAllText(mcpPath, "<StackPanel><TextBlock Text=\"Before\" /></StackPanel>");
        ServiceCollection services = new();
        services.AddSingleton(host);
        services.AddSingleton<ComponentMcpCommandCatalog>();
        services.AddAxamlDesignerMcpExports();
        using ServiceProvider provider = services.BuildServiceProvider();
        ComponentMcpCommandCatalog catalog = provider.GetRequiredService<ComponentMcpCommandCatalog>();
        using JsonDocument openArguments = JsonDocument.Parse(JsonSerializer.Serialize(new { path = mcpPath }));
        using JsonDocument setArguments = JsonDocument.Parse("""{"nodePath":"0","propertyName":"Text","value":"After"}""");
        using JsonDocument canceledArguments = JsonDocument.Parse("""{"nodePath":"0","propertyName":"Text","value":"Canceled"}""");
        using JsonDocument emptyArguments = JsonDocument.Parse("{}");
        int markupChanges = 0;
        bool allChangesOnUiThread = true;
        PropertyChangedEventHandler observeMarkup = (_, eventArgs) =>
        {
            if (eventArgs.PropertyName == nameof(AxamlDesignerHostViewModel.Markup))
            {
                markupChanges++;
                allChangesOnUiThread &= Dispatcher.UIThread.CheckAccess();
            }
        };
        host.PropertyChanged += observeMarkup;
        try
        {
            await Task.Run(() => catalog.ExecuteAsync("designer.open", openArguments.RootElement, CancellationToken.None))
                .WaitAsync(TimeSpan.FromSeconds(10));
            await Task.Run(() => catalog.ExecuteAsync("designer.set_property", setArguments.RootElement, CancellationToken.None))
                .WaitAsync(TimeSpan.FromSeconds(10));
            await Task.Run(() => catalog.ExecuteAsync("designer.save", emptyArguments.RootElement, CancellationToken.None))
                .WaitAsync(TimeSpan.FromSeconds(10));
            Check(markupChanges >= 2 && allChangesOnUiThread
                && File.ReadAllText(mcpPath).Contains("Text=\"After\"", StringComparison.Ordinal),
                "The standalone MCP commands did not execute on the UI thread and save the document.");

            using CancellationTokenSource cancellation = new();
            Task<string> canceledRequest = Task.Factory.StartNew(() =>
            {
                Task<string> request = catalog.ExecuteAsync(
                    "designer.set_property", canceledArguments.RootElement, cancellation.Token);
                Check(!request.IsCompleted, "The MCP request was not queued on the UI dispatcher.");
                cancellation.Cancel();
                return request;
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default)
                .WaitAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
            try
            {
                await canceledRequest.WaitAsync(TimeSpan.FromSeconds(5));
                throw new InvalidOperationException("A canceled queued MCP request completed.");
            }
            catch (OperationCanceledException)
            {
            }
            TaskCompletionSource<bool> dispatchBarrier = new(TaskCreationOptions.RunContinuationsAsynchronously);
            Dispatcher.UIThread.Post(() => dispatchBarrier.TrySetResult(true), DispatcherPriority.Background);
            await dispatchBarrier.Task.WaitAsync(TimeSpan.FromSeconds(5));
            Check(host.Markup.Contains("Text=\"After\"", StringComparison.Ordinal),
                "A canceled queued MCP request changed the document.");
        }
        finally
        {
            host.PropertyChanged -= observeMarkup;
        }

        Check(host.OpenDocument(Path.Combine(fixture, "Grid.axaml")), "The standalone Grid fixture did not open.");
        await VerifyGridEditingAsync(host.Designer, palette, window, () => host.Markup, () => host.IsDirty);
        Check(host.OpenDocument(Path.Combine(fixture, "Content.axaml")),
            "The standalone single-content fixture did not open.");
        await WaitUntilAsync(() => host.Designer.PreviewRoot?.TypeName == "Border",
            "the standalone single-content fixture");
        await VerifySingleContentRemovalAsync(host.Designer, palette, () => host.Markup, () => host.IsDirty);
        Check(host.OpenDocument(Path.Combine(fixture, "ControlPreviews.axaml")),
            "The standalone catalog-control preview fixture did not open.");
        await WaitUntilAsync(() => host.Designer.PreviewRoot?.TypeName == "StackPanel",
            "the standalone catalog-control preview fixture");
        await VerifyCatalogControlPreviewsAsync(window);
        window.Close();
    }

    private static async Task VerifyCatalogControlPreviewsAsync(Control root)
    {
        await WaitUntilAsync(() => root.GetVisualDescendants().OfType<ComboBox>().Any()
            && root.GetVisualDescendants().OfType<CalendarDatePicker>().Any()
            && root.GetVisualDescendants().OfType<Avalonia.Controls.Calendar>().Any()
            && root.GetVisualDescendants().OfType<AutoCompleteBox>().Any()
            && root.GetVisualDescendants().OfType<ColorPicker>().Any()
            && root.GetVisualDescendants().OfType<Border>().Any(
                static candidate => candidate.Tag is AxamlPreviewNode { TypeName: "Image" })
            && root.GetVisualDescendants().OfType<ItemsControl>().Any(
                static candidate => candidate.GetType() == typeof(ItemsControl) && candidate.Width == 127)
            && root.GetVisualDescendants().OfType<TabControl>().Any()
            && root.GetVisualDescendants().OfType<TimePicker>().Any()
            && root.GetVisualDescendants().OfType<NumericUpDown>().Any()
            && root.GetVisualDescendants().OfType<ProgressBar>().Any()
            && root.GetVisualDescendants().OfType<Slider>().Any()
            && root.GetVisualDescendants().OfType<Canvas>().Any(
                static canvas => canvas.Width == 140 && canvas.Height == 100)
            && root.GetVisualDescendants().OfType<WrapPanel>().Count() >= 3
            && root.GetVisualDescendants().OfType<CheckBox>().Any()
            && root.GetVisualDescendants().OfType<RadioButton>().Any()
            && root.GetVisualDescendants().OfType<ToggleSwitch>().Any(),
            "catalogued numeric, progress, and toggle control previews");
        Canvas canvasPreview = root.GetVisualDescendants().OfType<Canvas>()
            .Single(static canvas => canvas.Width == 140 && canvas.Height == 100);
        Check(canvasPreview.Children.Count == 2
            && Canvas.GetLeft(canvasPreview.Children[0]) == 15.5
            && Canvas.GetTop(canvasPreview.Children[0]) == 21
            && double.IsNaN(Canvas.GetLeft(canvasPreview.Children[1]))
            && double.IsNaN(Canvas.GetTop(canvasPreview.Children[1])),
            $"Canvas children render at valid literal coordinates and preserve unset framework positions "
            + $"(count={canvasPreview.Children.Count}, coordinates={string.Join(';', canvasPreview.Children.Select(child => $"{Canvas.GetLeft(child)},{Canvas.GetTop(child)}"))})");
        Border literalImagePreview = root.GetVisualDescendants().OfType<Border>().Single(
            static candidate => candidate.Tag is AxamlPreviewNode node
                && node.TypeName == "Image"
                && node.Properties.GetValueOrDefault("Width") == "129");
        Border boundImagePreview = root.GetVisualDescendants().OfType<Border>().Single(
            static candidate => candidate.Tag is AxamlPreviewNode node
                && node.TypeName == "Image"
                && node.Properties.GetValueOrDefault("Width") == "130");
        Check(!literalImagePreview.GetVisualDescendants().OfType<Avalonia.Controls.Image>().Any()
            && literalImagePreview.GetVisualDescendants().OfType<TextBlock>().Any(
                static text => text.Text?.Contains("not loaded", StringComparison.OrdinalIgnoreCase) == true)
            && literalImagePreview.GetVisualDescendants().OfType<TextBlock>().Any(
                static text => text.Text?.Contains("https://example.invalid/never-load.png", StringComparison.Ordinal) == true)
            && !boundImagePreview.GetVisualDescendants().OfType<Avalonia.Controls.Image>().Any()
            && !boundImagePreview.GetVisualDescendants().OfType<TextBlock>().Any(
                static text => text.Text?.Contains("{Binding Logo}", StringComparison.Ordinal) == true),
            "Image previews loaded an external source or exposed an unresolved binding.");
        ComboBox comboBox = root.GetVisualDescendants().OfType<ComboBox>()
            .Single(candidate => candidate.CornerRadius == new CornerRadius(4));
        ComboBox selectedComboBox = root.GetVisualDescendants().OfType<ComboBox>()
            .Single(candidate => candidate.PlaceholderText == "Selector selection");
        ComboBox invalidSelectedComboBox = root.GetVisualDescendants().OfType<ComboBox>()
            .Single(candidate => candidate.PlaceholderText == "Selector invalid");
        ComboBox boundSelectedComboBox = root.GetVisualDescendants().OfType<ComboBox>()
            .Single(candidate => candidate.PlaceholderText == "Selector bound");
        ComboBox outOfRangeSelectedComboBox = root.GetVisualDescendants().OfType<ComboBox>()
            .Single(candidate => candidate.PlaceholderText == "Selector out of range");
        ComboBox markupValueComboBox = root.GetVisualDescendants().OfType<ComboBox>()
            .Single(candidate => candidate.PlaceholderText == "Selector markup values");
        ComboBox emptyItemComboBox = root.GetVisualDescendants().OfType<ComboBox>()
            .Single(candidate => candidate.PlaceholderText == "Selector empty item");
        ComboBox complexContentComboBox = root.GetVisualDescendants().OfType<ComboBox>()
            .Single(candidate => candidate.PlaceholderText == "Selector complex content");
        DatePicker selectedDatePicker = root.GetVisualDescendants().OfType<DatePicker>()
            .Single(candidate => candidate.Width == 101);
        DatePicker invalidDatePicker = root.GetVisualDescendants().OfType<DatePicker>()
            .Single(candidate => candidate.Width == 102);
        DatePicker boundDatePicker = root.GetVisualDescendants().OfType<DatePicker>()
            .Single(candidate => candidate.Width == 103);
        DatePicker outOfRangeDatePicker = root.GetVisualDescendants().OfType<DatePicker>()
            .Single(candidate => candidate.Width == 104);
        CalendarDatePicker calendarDatePicker = root.GetVisualDescendants().OfType<CalendarDatePicker>()
            .Single(candidate => candidate.Width == 118);
        CalendarDatePicker invalidCalendarDatePicker = root.GetVisualDescendants().OfType<CalendarDatePicker>()
            .Single(candidate => candidate.Width == 119);
        CalendarDatePicker defaultCalendarDatePicker = root.GetVisualDescendants().OfType<CalendarDatePicker>()
            .Single(candidate => candidate.Width == 120);
        Avalonia.Controls.Calendar selectedCalendar = root.GetVisualDescendants().OfType<Avalonia.Controls.Calendar>()
            .Single(candidate => candidate.Width == 108);
        Avalonia.Controls.Calendar invalidCalendar = root.GetVisualDescendants().OfType<Avalonia.Controls.Calendar>()
            .Single(candidate => candidate.Width == 109);
        Avalonia.Controls.Calendar boundCalendar = root.GetVisualDescendants().OfType<Avalonia.Controls.Calendar>()
            .Single(candidate => candidate.Width == 110);
        Avalonia.Controls.Calendar outOfRangeCalendar = root.GetVisualDescendants().OfType<Avalonia.Controls.Calendar>()
            .Single(candidate => candidate.Width == 111);
        Avalonia.Controls.Calendar multipleRangeCalendar = root.GetVisualDescendants().OfType<Avalonia.Controls.Calendar>()
            .Single(candidate => candidate.Width == 112);
        AutoCompleteBox configuredAutoCompleteBox = root.GetVisualDescendants().OfType<AutoCompleteBox>()
            .Single(candidate => candidate.Width == 115);
        AutoCompleteBox invalidAutoCompleteBox = root.GetVisualDescendants().OfType<AutoCompleteBox>()
            .Single(candidate => candidate.Width == 116);
        AutoCompleteBox boundAutoCompleteBox = root.GetVisualDescendants().OfType<AutoCompleteBox>()
            .Single(candidate => candidate.Width == 117);
        AutoCompleteBox defaultAutoCompleteBox = root.GetVisualDescendants().OfType<AutoCompleteBox>()
            .Single(candidate => candidate.Width == 121);
        ColorPicker selectedColorPicker = root.GetVisualDescendants().OfType<ColorPicker>()
            .Single(candidate => candidate.Width == 112);
        ColorPicker invalidColorPicker = root.GetVisualDescendants().OfType<ColorPicker>()
            .Single(candidate => candidate.Width == 113);
        ColorPicker boundColorPicker = root.GetVisualDescendants().OfType<ColorPicker>()
            .Single(candidate => candidate.Width == 114);
        TabControl selectedTabControl = root.GetVisualDescendants().OfType<TabControl>()
            .Single(candidate => candidate.Width == 118);
        TabControl invalidTabControl = root.GetVisualDescendants().OfType<TabControl>()
            .Single(candidate => candidate.Width == 119);
        TabControl boundTabControl = root.GetVisualDescendants().OfType<TabControl>()
            .Single(candidate => candidate.Width == 120);
        Menu previewMenu = root.GetVisualDescendants().OfType<Menu>()
            .Single(candidate => candidate.Width == 121);
        Menu invalidGestureMenu = root.GetVisualDescendants().OfType<Menu>()
            .Single(candidate => candidate.Width == 122);
        Button contextMenuButton = root.GetVisualDescendants().OfType<Button>()
            .Single(candidate => candidate.Content is "Context trigger");
        TextBox[] previewTextBoxes = root.GetVisualDescendants().OfType<TextBox>().ToArray();
        Check(previewTextBoxes.Any(candidate => candidate.Text == "Context target"),
            $"The TextBox ContextMenu host was not found in preview. TextBoxes: {string.Join(", ", previewTextBoxes.Select(static candidate => $"{candidate.Text} ({candidate.Width})"))}.");
        TextBox contextMenuTextBox = previewTextBoxes
            .Single(candidate => candidate.Text == "Context target");
        Border[] contextMenuPreviews = root.GetVisualDescendants().OfType<Border>()
            .Where(candidate => candidate.Tag is AxamlPreviewNode { TypeName: "ContextMenu" })
            .ToArray();
        Border buttonContextMenuPreview = contextMenuPreviews.Single(candidate =>
            candidate.GetVisualDescendants().OfType<MenuItem>().Any(item => item.Header is "Copy"));
        Border textBoxContextMenuPreview = contextMenuPreviews.Single(candidate =>
            candidate.GetVisualDescendants().OfType<MenuItem>().Any(item => item.Header is "Paste"));
        Button tooltipButton = root.GetVisualDescendants().OfType<Button>()
            .Single(candidate => candidate.Content is "Tooltip target");
        Border toolTipPreview = root.GetVisualDescendants().OfType<Border>()
            .Single(candidate => candidate.Tag is AxamlPreviewNode { TypeName: "ToolTip" });
        TreeView staticTree = root.GetVisualDescendants().OfType<TreeView>()
            .Single(candidate => candidate.Width == 126);
        TreeViewItem projectTreeItem = staticTree.Items.OfType<TreeViewItem>().Single();
        TreeViewItem sourceTreeItem = projectTreeItem.Items.OfType<TreeViewItem>().Single();
        TreeViewItem sourceFileTreeItem = sourceTreeItem.Items.OfType<TreeViewItem>().Single();
        TimePicker selectedTimePicker = root.GetVisualDescendants().OfType<TimePicker>()
            .Single(candidate => candidate.Width == 105);
        TimePicker invalidTimePicker = root.GetVisualDescendants().OfType<TimePicker>()
            .Single(candidate => candidate.Width == 106);
        TimePicker boundTimePicker = root.GetVisualDescendants().OfType<TimePicker>()
            .Single(candidate => candidate.Width == 107);
        NumericUpDown numericUpDown = root.GetVisualDescendants().OfType<NumericUpDown>()
            .Single(candidate => candidate.Value == 12.5m);
        NumericUpDown defaultNumericUpDown = root.GetVisualDescendants().OfType<NumericUpDown>()
            .Single(candidate => candidate.Value == 0m);
        NumericUpDown invalidFormatNumericUpDown = root.GetVisualDescendants().OfType<NumericUpDown>()
            .Single(candidate => candidate.Value == 1m);
        ProgressBar progressBar = root.GetVisualDescendants().OfType<ProgressBar>()
            .Single(candidate => candidate.Maximum == 90);
        ProgressBar indeterminateProgressBar = root.GetVisualDescendants().OfType<ProgressBar>()
            .Single(candidate => candidate.IsIndeterminate);
        ProgressBar invalidFormatProgressBar = root.GetVisualDescendants().OfType<ProgressBar>()
            .Single(candidate => candidate.Value == 25);
        Slider slider = root.GetVisualDescendants().OfType<Slider>().Single();
        DockPanel dockPanel = root.GetVisualDescendants().OfType<DockPanel>()
            .Single(candidate => candidate.HorizontalSpacing == 4);
        DockPanel invalidDockPanel = root.GetVisualDescendants().OfType<DockPanel>()
            .Single(candidate => candidate.LastChildFill == false && candidate.HorizontalSpacing == 0);
        Avalonia.Controls.Primitives.UniformGrid uniformGrid = root.GetVisualDescendants()
            .OfType<Avalonia.Controls.Primitives.UniformGrid>()
            .Single(candidate => candidate.ColumnSpacing == 5);
        Avalonia.Controls.Primitives.UniformGrid invalidUniformGrid = root.GetVisualDescendants()
            .OfType<Avalonia.Controls.Primitives.UniformGrid>()
            .Single(candidate => candidate.Columns == 4);
        Grid grid = root.GetVisualDescendants().OfType<Grid>()
            .Single(candidate => candidate.Width == 99);
        Grid placedGrid = root.GetVisualDescendants().OfType<Grid>()
            .Single(candidate => candidate.Width == 141);
        Grid invalidGridSpacing = root.GetVisualDescendants().OfType<Grid>()
            .Single(candidate => candidate.Width == 98);
        ScrollViewer scrollViewer = root.GetVisualDescendants().OfType<ScrollViewer>()
            .Single(candidate => candidate.Width == 97);
        ScrollViewer invalidScrollViewer = root.GetVisualDescendants().OfType<ScrollViewer>()
            .Single(candidate => candidate.Width == 96);
        Viewbox viewbox = root.GetVisualDescendants().OfType<Viewbox>()
            .Single(candidate => candidate.Width == 93);
        Viewbox invalidViewbox = root.GetVisualDescendants().OfType<Viewbox>()
            .Single(candidate => candidate.Width == 92);
        WrapPanel wrapPanel = root.GetVisualDescendants().OfType<WrapPanel>()
            .Single(candidate => candidate.Width == 125);
        WrapPanel defaultWrapPanel = root.GetVisualDescendants().OfType<WrapPanel>()
            .Single(candidate => candidate.Width == 126);
        WrapPanel invalidWrapPanel = root.GetVisualDescendants().OfType<WrapPanel>()
            .Single(candidate => candidate.Width == 127);
        CheckBox checkBox = root.GetVisualDescendants().OfType<CheckBox>().Single();
        RadioButton radioButton = root.GetVisualDescendants().OfType<RadioButton>().Single();
        Button keyboardActionButton = root.GetVisualDescendants().OfType<Button>()
            .Single(button => button.Content?.ToString() == "Keyboard action preview");
        Button boundCommandParameterButton = root.GetVisualDescendants().OfType<Button>()
            .Single(button => button.Content?.ToString() == "Bound command parameter");
        Button invalidClipButton = root.GetVisualDescendants().OfType<Button>()
            .Single(button => button.Content?.ToString() == "Clip fallback");
        Button hiddenPreviewButton = root.GetVisualDescendants().OfType<Button>()
            .Single(button => button.Width == 90);
        Button invalidVisibilityButton = root.GetVisualDescendants().OfType<Button>()
            .Single(button => button.Width == 89);
        Border styledBorder = root.GetVisualDescendants().OfType<Border>()
            .Single(border => border.CornerRadius == new CornerRadius(12, 4, 8, 2));
        TextBox inputTextBox = root.GetVisualDescendants().OfType<TextBox>().Single(textBox => textBox.MaxLength == 120);
        TextBox boundPlaceholderTextBox = root.GetVisualDescendants().OfType<TextBox>()
            .Single(textBox => textBox.Text == "Bound placeholder preview");
        TextBox defaultLineCountTextBox = root.GetVisualDescendants().OfType<TextBox>()
            .Single(textBox => textBox.Text == "Default line count preview");
        TextBox invalidLineCountTextBox = root.GetVisualDescendants().OfType<TextBox>()
            .Single(textBox => textBox.Text == "Invalid line count preview");
        TextBlock alignedTextBlock = root.GetVisualDescendants().OfType<TextBlock>().Single(textBlock => textBlock.Text == "Alignment preview");
        TextBlock inheritedFlowTextBlock = root.GetVisualDescendants().OfType<TextBlock>().Single(textBlock => textBlock.Text == "Inherited RTL direction");
        TextBlock invalidFlowTextBlock = root.GetVisualDescendants().OfType<TextBlock>().Single(textBlock => textBlock.Text == "Invalid flow fallback");
        TextBlock boundFlowTextBlock = root.GetVisualDescendants().OfType<TextBlock>().Single(textBlock => textBlock.Text == "Bound flow fallback");
        ContentControl alignedContentControl = root.GetVisualDescendants().OfType<ContentControl>()
            .Single(control => control.GetType() == typeof(ContentControl)
                && control.Content is Control content
                && content.GetVisualDescendants().OfType<TextBlock>()
                    .Any(textBlock => textBlock.Text == "ContentControl alignment"));
        Expander alignedExpander = root.GetVisualDescendants().OfType<Expander>()
            .Single(expander => expander.Header?.ToString() == "Expander alignment");
        GroupBox alignedGroupBox = root.GetVisualDescendants().OfType<GroupBox>()
            .Single(groupBox => groupBox.Header?.ToString() == "GroupBox alignment");
        ListBox styledListBox = root.GetVisualDescendants().OfType<ListBox>()
            .Single(listBox => listBox.Background is ISolidColorBrush brush && brush.Color == Colors.Lavender);
        ListBox invalidSelectionListBox = root.GetVisualDescendants().OfType<ListBox>()
            .Single(listBox => listBox.Background is ISolidColorBrush brush && brush.Color == Colors.LightYellow);
        ListBox outOfRangeSelectionListBox = root.GetVisualDescendants().OfType<ListBox>()
            .Single(listBox => listBox.Background is ISolidColorBrush brush && brush.Color == Colors.LightCyan);
        ListBox boundSelectionListBox = root.GetVisualDescendants().OfType<ListBox>()
            .Single(listBox => listBox.Background is ISolidColorBrush brush && brush.Color == Colors.LightPink);
        ItemsControl staticItemsControl = root.GetVisualDescendants().OfType<ItemsControl>()
            .Single(candidate => candidate.GetType() == typeof(ItemsControl) && candidate.Width == 127);
        Rectangle rectangle = root.GetVisualDescendants().OfType<Rectangle>()
            .Single(candidate => candidate.Width == 90);
        Ellipse ellipse = root.GetVisualDescendants().OfType<Ellipse>()
            .Single(candidate => candidate.Width == 36);
        LineShape line = root.GetVisualDescendants().OfType<LineShape>()
            .Single(candidate => candidate.Stroke is ISolidColorBrush brush && brush.Color == Colors.DarkGreen);
        LineShape invalidLine = root.GetVisualDescendants().OfType<LineShape>()
            .Single(candidate => candidate.Stroke is ISolidColorBrush brush && brush.Color == Colors.Purple);
        PolygonShape polygon = root.GetVisualDescendants().OfType<PolygonShape>()
            .Single(candidate => candidate.Stroke is ISolidColorBrush brush && brush.Color == Colors.Navy);
        PolygonShape invalidPolygon = root.GetVisualDescendants().OfType<PolygonShape>()
            .Single(candidate => candidate.Fill is ISolidColorBrush brush && brush.Color == Colors.Purple);
        PolylineShape polyline = root.GetVisualDescendants().OfType<PolylineShape>()
            .Single(candidate => candidate.Stroke is ISolidColorBrush brush && brush.Color == Colors.Blue);
        PolylineShape invalidPolyline = root.GetVisualDescendants().OfType<PolylineShape>()
            .Single(candidate => candidate.Stroke is ISolidColorBrush brush && brush.Color == Colors.Orange);
        Label alignedLabel = root.GetVisualDescendants().OfType<Label>()
            .Single(label => label.Content is TextBlock labelText && labelText.Text == "Label alignment");
        Label invalidLabelLayout = root.GetVisualDescendants().OfType<Label>()
            .Single(label => label.Content is TextBlock labelText && labelText.Text == "Invalid label layout");
        Label boundLabelLayout = root.GetVisualDescendants().OfType<Label>()
            .Single(label => label.Content is TextBlock labelText && labelText.Text == "Bound label layout");
        ToggleSwitch toggleSwitch = root.GetVisualDescendants().OfType<ToggleSwitch>().Single();
        Check(comboBox.PlaceholderText == "Choose an option"
            && comboBox.PlaceholderForeground is ISolidColorBrush comboPlaceholderForeground
            && comboPlaceholderForeground.Color == Colors.DarkOrange
            && !comboBox.IsEnabled && !comboBox.IsDropDownOpen
            && comboBox.HorizontalContentAlignment == HorizontalAlignment.Right
            && comboBox.VerticalContentAlignment == VerticalAlignment.Bottom
            && comboBox.Background is ISolidColorBrush comboBackground
            && comboBackground.Color == Colors.White
            && comboBox.BorderBrush is ISolidColorBrush comboBorderBrush
            && comboBorderBrush.Color == Colors.Blue
            && comboBox.BorderThickness == new Thickness(2)
            && comboBox.CornerRadius == new CornerRadius(4)
            && comboBox.FontSize == 14
            && comboBox.FontStyle == FontStyle.Italic
            && comboBox.FontWeight == FontWeight.Medium
            && comboBox.Foreground is ISolidColorBrush comboForeground
            && comboForeground.Color == Colors.Black
            && comboBox.Padding == new Thickness(3),
            $"The ComboBox preview did not preserve its appearance or remained interactive: placeholder={comboBox.PlaceholderText}, enabled={comboBox.IsEnabled}, background={comboBox.Background}, border={comboBox.BorderBrush}, borderThickness={comboBox.BorderThickness}, cornerRadius={comboBox.CornerRadius}, fontSize={comboBox.FontSize}, fontStyle={comboBox.FontStyle}, fontWeight={comboBox.FontWeight}, foreground={comboBox.Foreground}, padding={comboBox.Padding}.");
        Check(selectedComboBox.SelectedIndex == 2 && Equals(selectedComboBox.SelectedItem, "Property content")
            && !selectedComboBox.IsEnabled
            && invalidSelectedComboBox.SelectedIndex == -1
            && boundSelectedComboBox.SelectedIndex == -1
            && outOfRangeSelectedComboBox.SelectedIndex == -1,
            $"ComboBox selection was not safely initialized: selected={selectedComboBox.SelectedIndex}/{selectedComboBox.SelectedItem}, invalid={invalidSelectedComboBox.SelectedIndex}, bound={boundSelectedComboBox.SelectedIndex}, outOfRange={outOfRangeSelectedComboBox.SelectedIndex}.");
        Check(selectedComboBox.ItemsSource is IEnumerable<string> comboBoxItems
            && comboBoxItems.SequenceEqual(["First choice", "Second choice", "Property content"]),
            "ComboBoxItem attribute, inline, or property-element content was not preserved as selector options.");
        Check(markupValueComboBox.ItemsSource is IEnumerable<string> markupItems
            && markupItems.SequenceEqual(["[Binding ItemText]", "[DynamicResource ItemText]", "[StaticResource ItemText]"])
            && markupValueComboBox.SelectedIndex == 2
            && Equals(markupValueComboBox.SelectedItem, "[StaticResource ItemText]")
            && !markupValueComboBox.IsEnabled,
            "Selector markup extensions were evaluated instead of displayed as inert placeholders.");
        Check(emptyItemComboBox.SelectedIndex == 0
            && Equals(emptyItemComboBox.SelectedItem, string.Empty)
            && emptyItemComboBox.ItemsSource is IEnumerable<string> emptyItems
            && emptyItems.SequenceEqual([string.Empty])
            && !emptyItemComboBox.IsEnabled,
            "An explicitly empty selector item did not remain a valid inert item.");
        Check(complexContentComboBox.SelectedIndex == 0
            && Equals(complexContentComboBox.SelectedItem, "ComboBoxItem")
            && complexContentComboBox.ItemsSource is IEnumerable<string> complexItems
            && complexItems.SequenceEqual(["ComboBoxItem"])
            && !complexContentComboBox.IsEnabled,
            "Complex selector item content was interpreted instead of using a safe fallback label.");
        Check(selectedDatePicker.SelectedDate?.Date == new DateTime(2028, 2, 29)
            && !selectedDatePicker.DayVisible
            && selectedDatePicker.MonthVisible
            && !selectedDatePicker.YearVisible
            && selectedDatePicker.DayFormat == "dd"
            && selectedDatePicker.MonthFormat == "MMM"
            && selectedDatePicker.YearFormat == "yy"
            && selectedDatePicker.MinYear.Date == new DateTime(2020, 1, 1)
            && selectedDatePicker.MaxYear.Date == new DateTime(2030, 12, 31)
            && selectedDatePicker.SelectedDate?.Year == 2028
            && !selectedDatePicker.IsEnabled
            && invalidDatePicker.SelectedDate is null
            && invalidDatePicker.DayVisible
            && invalidDatePicker.MonthVisible
            && invalidDatePicker.YearVisible
            && invalidDatePicker.DayFormat == boundDatePicker.DayFormat
            && invalidDatePicker.MonthFormat == boundDatePicker.MonthFormat
            && invalidDatePicker.YearFormat == boundDatePicker.YearFormat
            && invalidDatePicker.MinYear == boundDatePicker.MinYear
            && invalidDatePicker.MaxYear == boundDatePicker.MaxYear
            && !invalidDatePicker.IsEnabled
            && boundDatePicker.SelectedDate is null
            && boundDatePicker.DayVisible
            && boundDatePicker.MonthVisible
            && boundDatePicker.YearVisible
            && !boundDatePicker.IsEnabled
            && outOfRangeDatePicker.SelectedDate is null
            && outOfRangeDatePicker.MinYear.Date == new DateTime(2020, 1, 1)
            && outOfRangeDatePicker.MaxYear.Date == new DateTime(2030, 12, 31)
            && !outOfRangeDatePicker.IsEnabled,
            $"The DatePicker preview did not preserve valid date visibility or safely ignore invalid/bound values: selected={selectedDatePicker.SelectedDate:O}/{selectedDatePicker.DayVisible},{selectedDatePicker.MonthVisible},{selectedDatePicker.YearVisible}, invalid={invalidDatePicker.SelectedDate:O}/{invalidDatePicker.DayVisible},{invalidDatePicker.MonthVisible},{invalidDatePicker.YearVisible}, bound={boundDatePicker.SelectedDate:O}/{boundDatePicker.DayVisible},{boundDatePicker.MonthVisible},{boundDatePicker.YearVisible}.");
        Check(calendarDatePicker.SelectedDate?.Date == new DateTime(2028, 2, 29)
            && calendarDatePicker.PlaceholderText == "Choose a day"
            && calendarDatePicker.PlaceholderForeground is ISolidColorBrush calendarPlaceholderForeground
            && calendarPlaceholderForeground.Color == Colors.DarkGray
            && calendarDatePicker.UseFloatingPlaceholder
            && !calendarDatePicker.IsTodayHighlighted
            && !calendarDatePicker.IsDropDownOpen
            && !calendarDatePicker.IsEnabled
            && !defaultCalendarDatePicker.UseFloatingPlaceholder
            && invalidCalendarDatePicker.PlaceholderText == "Choose a date"
            && invalidCalendarDatePicker.PlaceholderForeground == defaultCalendarDatePicker.PlaceholderForeground
            && invalidCalendarDatePicker.UseFloatingPlaceholder == defaultCalendarDatePicker.UseFloatingPlaceholder,
            "The CalendarDatePicker preview did not preserve safe date/placeholder settings or open its popup.");
        Check(selectedCalendar.SelectedDate == new DateTime(2028, 2, 29)
            && selectedCalendar.DisplayDate == new DateTime(2028, 2, 1)
            && selectedCalendar.DisplayDateStart == new DateTime(2028, 1, 1)
            && selectedCalendar.DisplayDateEnd == new DateTime(2028, 12, 31)
            && selectedCalendar.DisplayMode == CalendarMode.Year
            && selectedCalendar.SelectionMode == CalendarSelectionMode.SingleDate
            && selectedCalendar.FirstDayOfWeek == DayOfWeek.Sunday
            && !selectedCalendar.IsTodayHighlighted
            && !selectedCalendar.IsEnabled
            && invalidCalendar.SelectedDate is null
            && invalidCalendar.DisplayDateStart is null
            && invalidCalendar.DisplayDateEnd is null
            && invalidCalendar.DisplayMode == CalendarMode.Month
            && invalidCalendar.SelectionMode == CalendarSelectionMode.SingleDate
            && invalidCalendar.FirstDayOfWeek == DayOfWeek.Monday
            && invalidCalendar.IsTodayHighlighted
            && !invalidCalendar.IsEnabled
            && boundCalendar.SelectedDate is null
            && boundCalendar.DisplayDateStart is null
            && boundCalendar.DisplayDateEnd is null
            && boundCalendar.SelectionMode == CalendarSelectionMode.SingleDate
            && boundCalendar.FirstDayOfWeek == DayOfWeek.Monday
            && boundCalendar.IsTodayHighlighted
            && !boundCalendar.IsEnabled
            && outOfRangeCalendar.SelectedDate is null
            && outOfRangeCalendar.DisplayDateStart == new DateTime(2020, 1, 1)
            && outOfRangeCalendar.DisplayDateEnd == new DateTime(2030, 12, 31)
            && !outOfRangeCalendar.IsEnabled
            && multipleRangeCalendar.SelectionMode == CalendarSelectionMode.MultipleRange
            && multipleRangeCalendar.DisplayMode == CalendarMode.Decade
            && multipleRangeCalendar.SelectedDate is null
            && !multipleRangeCalendar.IsEnabled,
            $"The Calendar preview did not preserve safe date selection, navigation bounds or fallback values: valid={selectedCalendar.SelectedDate}/{selectedCalendar.DisplayDate}, invalid={invalidCalendar.SelectedDate}/{invalidCalendar.DisplayDateStart}/{invalidCalendar.DisplayDateEnd}, bound={boundCalendar.SelectedDate}/{boundCalendar.DisplayDateStart}/{boundCalendar.DisplayDateEnd}, out-of-range={outOfRangeCalendar.SelectedDate}.");
        Check(configuredAutoCompleteBox.Text == "Par"
            && configuredAutoCompleteBox.PlaceholderText == "Find a city"
            && configuredAutoCompleteBox.PlaceholderForeground is ISolidColorBrush autoCompletePlaceholderForeground
            && autoCompletePlaceholderForeground.Color == Colors.DarkGray
            && configuredAutoCompleteBox.FilterMode == AutoCompleteFilterMode.Contains
            && configuredAutoCompleteBox.MinimumPrefixLength == 2
            && configuredAutoCompleteBox.IsTextCompletionEnabled
            && !configuredAutoCompleteBox.IsDropDownOpen
            && !configuredAutoCompleteBox.IsEnabled
            && invalidAutoCompleteBox.Text == "Grace"
            && invalidAutoCompleteBox.FilterMode == AutoCompleteFilterMode.StartsWith
            && invalidAutoCompleteBox.MinimumPrefixLength == 1
            && invalidAutoCompleteBox.PlaceholderForeground == defaultAutoCompleteBox.PlaceholderForeground
            && !invalidAutoCompleteBox.IsTextCompletionEnabled
            && !invalidAutoCompleteBox.IsEnabled
            && boundAutoCompleteBox.Text == string.Empty
            && boundAutoCompleteBox.PlaceholderText == string.Empty
            && boundAutoCompleteBox.FilterMode == AutoCompleteFilterMode.StartsWith
            && boundAutoCompleteBox.MinimumPrefixLength == 1
            && boundAutoCompleteBox.PlaceholderForeground == defaultAutoCompleteBox.PlaceholderForeground
            && !boundAutoCompleteBox.IsTextCompletionEnabled
            && !boundAutoCompleteBox.IsEnabled,
            $"The AutoCompleteBox preview did not preserve safe text/filter settings or bound-value fallbacks: valid={configuredAutoCompleteBox.Text}/{configuredAutoCompleteBox.FilterMode}/{configuredAutoCompleteBox.MinimumPrefixLength}, invalid={invalidAutoCompleteBox.FilterMode}/{invalidAutoCompleteBox.MinimumPrefixLength}, bound={boundAutoCompleteBox.Text}/{boundAutoCompleteBox.FilterMode}/{boundAutoCompleteBox.MinimumPrefixLength}.");
        Check(selectedColorPicker.Color == Color.Parse("#FF123456")
            && selectedColorPicker.ColorModel == ColorModel.Hsva
            && !selectedColorPicker.IsAlphaEnabled
            && !selectedColorPicker.IsColorSpectrumVisible
            && !selectedColorPicker.IsEnabled
            && invalidColorPicker.Color == Colors.White
            && invalidColorPicker.ColorModel == ColorModel.Rgba
            && invalidColorPicker.IsAlphaEnabled
            && !invalidColorPicker.IsEnabled
            && boundColorPicker.Color == Colors.White
            && boundColorPicker.ColorModel == ColorModel.Rgba
            && boundColorPicker.IsAlphaEnabled
            && !boundColorPicker.IsEnabled,
            $"The ColorPicker preview did not preserve a valid color or fall back safely: valid={selectedColorPicker.Color}/{selectedColorPicker.ColorModel}, invalid={invalidColorPicker.Color}/{invalidColorPicker.ColorModel}, bound={boundColorPicker.Color}/{boundColorPicker.ColorModel}.");
        Check(selectedTimePicker.SelectedTime == new TimeSpan(13, 45, 30)
            && selectedTimePicker.UseSeconds
            && selectedTimePicker.ClockIdentifier == "12HourClock"
            && selectedTimePicker.MinuteIncrement == 5
            && selectedTimePicker.SecondIncrement == 10
            && !selectedTimePicker.IsEnabled
            && invalidTimePicker.SelectedTime is null
            && !invalidTimePicker.UseSeconds
            && invalidTimePicker.ClockIdentifier == "24HourClock"
            && invalidTimePicker.MinuteIncrement == 1
            && invalidTimePicker.SecondIncrement == 1
            && !invalidTimePicker.IsEnabled
            && boundTimePicker.SelectedTime is null
            && !boundTimePicker.UseSeconds
            && boundTimePicker.ClockIdentifier == "24HourClock"
            && boundTimePicker.MinuteIncrement == 1
            && boundTimePicker.SecondIncrement == 1
            && !boundTimePicker.IsEnabled,
            $"The TimePicker preview did not preserve valid time configuration or safely ignore invalid/bound values: selected={selectedTimePicker.SelectedTime}/{selectedTimePicker.ClockIdentifier}/{selectedTimePicker.MinuteIncrement}/{selectedTimePicker.SecondIncrement}, invalid={invalidTimePicker.SelectedTime}/{invalidTimePicker.ClockIdentifier}/{invalidTimePicker.MinuteIncrement}/{invalidTimePicker.SecondIncrement}, bound={boundTimePicker.SelectedTime}/{boundTimePicker.ClockIdentifier}/{boundTimePicker.MinuteIncrement}/{boundTimePicker.SecondIncrement}.");
        Check(selectedTabControl.SelectedIndex == 1
            && selectedTabControl.Items.Count == 2
            && selectedTabControl.Items[0] is TabItem { Header: "General" }
            && selectedTabControl.Items[1] is TabItem { Header: "Advanced" }
            && !selectedTabControl.IsEnabled
            && invalidTabControl.SelectedIndex == 0
            && invalidTabControl.Items.Count == 1
            && !invalidTabControl.IsEnabled
            && boundTabControl.SelectedIndex == 0
            && !boundTabControl.IsEnabled,
            $"The TabControl preview did not preserve safe tab headers/selection or fall back for invalid and bound indices: valid={selectedTabControl.SelectedIndex}/{selectedTabControl.Items.Count}, invalid={invalidTabControl.SelectedIndex}, bound={boundTabControl.SelectedIndex}.");
        MenuItem fileMenuItem = (MenuItem)previewMenu.Items[0]!;
        Check(!previewMenu.IsEnabled
            && previewMenu.Items.Count == 1
            && fileMenuItem.Header is "File"
            && fileMenuItem.Icon is TextBlock { Text: "F" }
            && fileMenuItem.ToggleType == MenuItemToggleType.CheckBox
            && fileMenuItem.IsChecked
            && !fileMenuItem.IsSubMenuOpen
            && fileMenuItem.StaysOpenOnClick
            && !fileMenuItem.IsEnabled
            && fileMenuItem.Items.Count == 3
            && fileMenuItem.Items[0] is MenuItem
            {
                Header: "Open",
                IsEnabled: false,
                InputGesture.Key: Key.O,
                InputGesture.KeyModifiers: KeyModifiers.Control,
                HotKey: null,
                ToggleType: MenuItemToggleType.Radio,
                GroupName: "EditorMode",
                IsChecked: true,
                Command: null,
            }
            && fileMenuItem.Items[1] is Separator
            && fileMenuItem.Items[2] is MenuItem
            {
                Header: "Exit",
                IsEnabled: false,
                InputGesture.Key: Key.Q,
                InputGesture.KeyModifiers: KeyModifiers.Control,
                HotKey: null,
                Command: null,
                CommandParameter: "File",
            },
            $"The menu preview did not preserve nested headers/separators while remaining inert: items={previewMenu.Items.Count}, fileItems={fileMenuItem.Items.Count}.");
        Check(invalidGestureMenu.Items[0] is MenuItem
            {
                Header: "Invalid",
                Icon: null,
                InputGesture: null,
                HotKey: null,
                Command: null,
                CommandParameter: null,
                ToggleType: MenuItemToggleType.None,
                GroupName: "",
                IsChecked: false,
                IsSubMenuOpen: false,
                StaysOpenOnClick: false,
                IsEnabled: false,
            },
            "The menu preview did not safely fall back for invalid state and input gesture values.");
        MenuItem contextMenuItem = buttonContextMenuPreview.GetVisualDescendants().OfType<MenuItem>().Single();
        MenuItem textBoxContextMenuItem = textBoxContextMenuPreview.GetVisualDescendants().OfType<MenuItem>().Single();
        Check(contextMenuButton.Content is "Context trigger"
            && !contextMenuButton.IsEnabled
            && contextMenuItem.Header is "Copy"
            && contextMenuItem.InputGesture?.Key == Key.C
            && contextMenuItem.InputGesture.KeyModifiers == KeyModifiers.Control
            && !contextMenuItem.IsEnabled
            && buttonContextMenuPreview.GetVisualDescendants().OfType<Separator>().Any()
            && buttonContextMenuPreview.GetVisualDescendants().OfType<TextBlock>()
                .Any(static text => text.Text == "ContextMenu (closed, inert preview)"),
            "The Button ContextMenu was not rendered as an explicitly closed, inert inline preview.");
        Check(contextMenuTextBox.Text is "Context target"
            && textBoxContextMenuItem.Header is "Paste"
            && !textBoxContextMenuItem.IsEnabled
            && textBoxContextMenuPreview.GetVisualDescendants().OfType<TextBlock>()
                .Any(static text => text.Text == "ContextMenu (closed, inert preview)"),
            "The TextBox ContextMenu was not rendered as an explicitly closed, inert inline preview.");
        Check(tooltipButton.Content is "Tooltip target"
            && !tooltipButton.IsEnabled
            && toolTipPreview.GetVisualDescendants().OfType<TextBlock>()
                .Any(static text => text.Text == "ToolTip (closed, inert preview)")
            && toolTipPreview.GetVisualDescendants().OfType<TextBlock>()
                .Any(static text => text.Text == "Helpful text")
            && toolTipPreview.GetVisualDescendants().OfType<TextBlock>()
                .Any(static text => text.Text == "More information"),
            "The ToolTip was not rendered as an explicitly closed, inert inline preview.");
        Check(!staticTree.IsEnabled
            && staticTree.CornerRadius == new CornerRadius(4, 2, 4, 2)
            && staticTree.FontFamily.ToString() == "DejaVu Sans"
            && staticTree.FontSize == 13
            && staticTree.FontStyle == FontStyle.Italic
            && staticTree.FontWeight == FontWeight.SemiBold
            && staticTree.Foreground is ISolidColorBrush treeForeground
            && treeForeground.Color == Colors.DarkGreen
            && projectTreeItem.Header is "Project"
            && projectTreeItem.CornerRadius == new CornerRadius(3)
            && projectTreeItem.FontFamily.ToString() == "Arial"
            && projectTreeItem.FontSize == 14
            && projectTreeItem.FontStyle == FontStyle.Oblique
            && projectTreeItem.FontWeight == FontWeight.Bold
            && projectTreeItem.Foreground is ISolidColorBrush itemForeground
            && itemForeground.Color == Colors.Blue
            && projectTreeItem.IsExpanded
            && !projectTreeItem.IsEnabled
            && sourceTreeItem.Header is "src"
            && sourceFileTreeItem.Header is "Main.cs",
            "The static TreeView hierarchy was not rendered with safe, non-interactive nodes.");
        Check(numericUpDown.Value == 12.5m && numericUpDown.FormatString == "N2"
            && !numericUpDown.AllowSpin && !numericUpDown.ShowButtonSpinner && numericUpDown.ClipValueToMinMax
            && numericUpDown.ButtonSpinnerLocation == Location.Left
            && numericUpDown.TextAlignment == TextAlignment.Center
            && numericUpDown.FontFamily.ToString() == "DejaVu Sans"
            && numericUpDown.HorizontalContentAlignment == HorizontalAlignment.Center
            && numericUpDown.VerticalContentAlignment == VerticalAlignment.Top
            && numericUpDown.ParsingNumberStyle == System.Globalization.NumberStyles.Integer
            && numericUpDown.Margin == new Thickness(3, 4)
            && numericUpDown.HorizontalAlignment == Avalonia.Layout.HorizontalAlignment.Right
            && numericUpDown.VerticalAlignment == Avalonia.Layout.VerticalAlignment.Center
            && numericUpDown.Width == 120 && numericUpDown.Height == 60
            && numericUpDown.MinWidth == 40 && numericUpDown.MaxWidth == 160
            && numericUpDown.MinHeight == 30 && numericUpDown.MaxHeight == 90
            && numericUpDown.Increment == 0.25m && numericUpDown.Minimum == 0.5m && numericUpDown.Maximum == 40m && !numericUpDown.IsEnabled
            && numericUpDown.Background is ISolidColorBrush numericBackground
            && numericBackground.Color == Color.FromRgb(240, 240, 240)
            && numericUpDown.BorderBrush is ISolidColorBrush numericBorderBrush
            && numericBorderBrush.Color == Colors.Green
            && numericUpDown.BorderThickness == new Thickness(2)
            && numericUpDown.CornerRadius == new CornerRadius(3)
            && numericUpDown.FontSize == 15
            && numericUpDown.FontStyle == FontStyle.Italic
            && numericUpDown.FontWeight == FontWeight.Bold
            && numericUpDown.Foreground is ISolidColorBrush numericForeground
            && numericForeground.Color == Colors.DarkGreen
            && numericUpDown.PlaceholderForeground is ISolidColorBrush numericPlaceholderForeground
            && numericPlaceholderForeground.Color == Colors.LimeGreen
            && numericUpDown.Padding == new Thickness(2),
            "The NumericUpDown preview did not preserve its value or remained interactive.");
        Check(defaultNumericUpDown.AllowSpin && defaultNumericUpDown.ShowButtonSpinner
            && !defaultNumericUpDown.ClipValueToMinMax
            && defaultNumericUpDown.ButtonSpinnerLocation == Location.Right
            && defaultNumericUpDown.TextAlignment == TextAlignment.Start
            && defaultNumericUpDown.ParsingNumberStyle == System.Globalization.NumberStyles.Any
            && !defaultNumericUpDown.IsEnabled,
            "The default NumericUpDown preview did not preserve Avalonia spin defaults or remained interactive.");
        Check(invalidFormatNumericUpDown.FormatString == defaultNumericUpDown.FormatString
            && invalidFormatNumericUpDown.ParsingNumberStyle == defaultNumericUpDown.ParsingNumberStyle
            && invalidFormatNumericUpDown.HorizontalAlignment == Avalonia.Layout.HorizontalAlignment.Stretch
            && invalidFormatNumericUpDown.VerticalAlignment == Avalonia.Layout.VerticalAlignment.Stretch
            && invalidFormatNumericUpDown.HorizontalContentAlignment == defaultNumericUpDown.HorizontalContentAlignment
            && invalidFormatNumericUpDown.VerticalContentAlignment == defaultNumericUpDown.VerticalContentAlignment,
            "The NumericUpDown preview applied an invalid format, parsing style or layout alignment.");
        Check(wrapPanel.ItemSpacing == 5 && wrapPanel.LineSpacing == 7
            && wrapPanel.ItemWidth == 40 && wrapPanel.ItemHeight == 20
            && wrapPanel.ItemsAlignment == WrapPanelItemsAlignment.Center
            && wrapPanel.Children.Count == 2,
            "The WrapPanel preview did not preserve its spacing, item size, alignment or children.");
        Check(defaultWrapPanel.ItemSpacing == 0 && defaultWrapPanel.LineSpacing == 0
            && double.IsNaN(defaultWrapPanel.ItemWidth) && double.IsNaN(defaultWrapPanel.ItemHeight)
            && defaultWrapPanel.ItemsAlignment == WrapPanelItemsAlignment.Start,
            "The WrapPanel preview did not preserve its framework defaults.");
        Check(invalidWrapPanel.ItemSpacing == 0 && invalidWrapPanel.LineSpacing == 0
            && double.IsNaN(invalidWrapPanel.ItemWidth) && double.IsNaN(invalidWrapPanel.ItemHeight)
            && invalidWrapPanel.ItemsAlignment == WrapPanelItemsAlignment.Start,
            "The WrapPanel preview applied an invalid layout value.");
        Check(dockPanel.HorizontalSpacing == 4 && dockPanel.VerticalSpacing == 6
            && !dockPanel.LastChildFill && dockPanel.Children.Count == 2
            && DockPanel.GetDock(dockPanel.Children[1]) == Dock.Bottom,
            "The DockPanel preview did not preserve spacing, last-child-fill, child placement or children.");
        Check(invalidDockPanel.HorizontalSpacing == 0 && invalidDockPanel.VerticalSpacing == 0
            && !invalidDockPanel.LastChildFill && invalidDockPanel.Children.Count == 1
            && DockPanel.GetDock(invalidDockPanel.Children[0]) == Dock.Left,
            "The DockPanel preview applied invalid spacing or child placement values.");
        Check(uniformGrid.Rows == 2 && uniformGrid.Columns == 3
            && uniformGrid.RowSpacing == 3 && uniformGrid.ColumnSpacing == 5
            && uniformGrid.Children.Count == 2,
            "The UniformGrid preview did not preserve its row/column spacing or layout.");
        Check(invalidUniformGrid.RowSpacing == 0 && invalidUniformGrid.ColumnSpacing == 0,
            "The UniformGrid preview applied invalid spacing values.");
        Check(grid.RowSpacing == 2 && grid.ColumnSpacing == 4 && grid.ShowGridLines,
            "The Grid preview did not preserve row/column spacing or show-grid-lines.");
        Check(placedGrid.Children.Count == 2
            && Grid.GetRow(placedGrid.Children[0]) == 0
            && Grid.GetColumn(placedGrid.Children[0]) == 0
            && Grid.GetRowSpan(placedGrid.Children[0]) == 2
            && Grid.GetRow(placedGrid.Children[1]) == 2
            && Grid.GetColumn(placedGrid.Children[1]) == 1,
            "The Grid preview did not apply child rows, columns or spans.");
        Check(invalidGridSpacing.RowSpacing == 0 && invalidGridSpacing.ColumnSpacing == 0
            && !invalidGridSpacing.ShowGridLines,
            "The Grid preview applied invalid spacing or show-grid-lines values.");
        Check(scrollViewer.HorizontalScrollBarVisibility == ScrollBarVisibility.Hidden
            && scrollViewer.VerticalScrollBarVisibility == ScrollBarVisibility.Disabled,
            "The ScrollViewer preview did not preserve its scrollbar visibility settings.");
        Check(invalidScrollViewer.HorizontalScrollBarVisibility == ScrollBarVisibility.Auto
            && invalidScrollViewer.VerticalScrollBarVisibility == ScrollBarVisibility.Auto,
            "The ScrollViewer preview applied invalid scrollbar visibility values.");
        Check(viewbox.StretchDirection == StretchDirection.UpOnly
            && invalidViewbox.StretchDirection == StretchDirection.Both,
            "The Viewbox preview did not preserve or safely default its stretch direction.");
        Check(progressBar.Minimum == 10 && progressBar.Maximum == 90 && progressBar.Value == 42.5
            && !progressBar.IsIndeterminate && progressBar.ShowProgressText
            && progressBar.ProgressTextFormat == "{1:0.0}%"
            && progressBar.Orientation == Orientation.Horizontal && !progressBar.IsEnabled
            && progressBar.Background is ISolidColorBrush progressBackground
            && progressBackground.Color == Colors.White
            && progressBar.BorderBrush is ISolidColorBrush progressBorderBrush
            && progressBorderBrush.Color == Colors.Purple
            && progressBar.BorderThickness == new Thickness(1)
            && progressBar.CornerRadius == new CornerRadius(2)
            && progressBar.FontSize == 13
            && progressBar.FontWeight == FontWeight.Normal
            && progressBar.Foreground is ISolidColorBrush progressForeground
            && progressForeground.Color == Colors.Blue
            && progressBar.Padding == new Thickness(1),
            "The ProgressBar preview did not preserve its value/state or remained interactive.");
        Check(indeterminateProgressBar.IsIndeterminate && !indeterminateProgressBar.ShowProgressText
            && indeterminateProgressBar.ProgressTextFormat == "{1:0}%"
            && invalidFormatProgressBar.ProgressTextFormat == "{1:0}%",
            "The ProgressBar preview did not preserve the indeterminate state/default text setting.");
        Check(slider.Minimum == -10 && slider.Maximum == 60 && slider.Value == 35.5
            && slider.TickFrequency == 5 && slider.IsSnapToTickEnabled
            && slider.IsDirectionReversed && slider.TickPlacement == TickPlacement.Outside
            && slider.Orientation == Orientation.Vertical && !slider.IsEnabled
            && slider.Background is ISolidColorBrush sliderBackground
            && sliderBackground.Color == Color.FromRgb(240, 240, 240)
            && slider.BorderBrush is ISolidColorBrush sliderBorderBrush
            && sliderBorderBrush.Color == Colors.Black
            && slider.BorderThickness == new Thickness(3)
            && slider.CornerRadius == new CornerRadius(5)
            && slider.FontSize == 16
            && slider.FontStyle == FontStyle.Oblique
            && slider.FontWeight == FontWeight.SemiBold
            && slider.Foreground is ISolidColorBrush sliderForeground
            && sliderForeground.Color == Colors.Red
            && slider.Padding == new Thickness(4),
            "The Slider preview did not preserve its value/orientation or remained interactive.");
        Check(checkBox.IsChecked == true && checkBox.IsThreeState && !checkBox.IsEnabled
            && checkBox.Command is null && checkBox.CommandParameter is "RememberMe"
            && checkBox.HorizontalContentAlignment == HorizontalAlignment.Left
            && checkBox.VerticalContentAlignment == VerticalAlignment.Center
            && checkBox.Background is ISolidColorBrush checkBoxBackground
            && checkBoxBackground.Color == Color.FromRgb(240, 240, 240)
            && checkBox.BorderBrush is ISolidColorBrush checkBoxBorderBrush
            && checkBoxBorderBrush.Color == Colors.Blue
            && checkBox.BorderThickness == new Thickness(2)
            && checkBox.CornerRadius == new CornerRadius(1, 2, 3, 4)
            && checkBox.FontSize == 16
            && checkBox.FontStyle == FontStyle.Italic
            && checkBox.FontWeight == FontWeight.Medium
            && checkBox.Foreground is ISolidColorBrush checkBoxForeground
            && checkBoxForeground.Color == Colors.White
            && checkBox.Padding == new Thickness(4),
            "The CheckBox preview did not preserve its state or remained interactive.");
        Check(radioButton.IsChecked == false && radioButton.GroupName == "Choices" && !radioButton.IsEnabled
            && radioButton.Command is null && radioButton.CommandParameter is "Personal"
            && radioButton.HorizontalContentAlignment == HorizontalAlignment.Center
            && radioButton.VerticalContentAlignment == VerticalAlignment.Top
            && radioButton.Background is ISolidColorBrush radioBackground
            && radioBackground.Color == Colors.White
            && radioButton.BorderBrush is ISolidColorBrush radioBorderBrush
            && radioBorderBrush.Color == Colors.Purple
            && radioButton.BorderThickness == new Thickness(1)
            && radioButton.CornerRadius == new CornerRadius(4)
            && radioButton.FontSize == 17
            && radioButton.FontStyle == FontStyle.Oblique
            && radioButton.FontWeight == FontWeight.Bold
            && radioButton.Foreground is ISolidColorBrush radioForeground
            && radioForeground.Color == Colors.Black
            && radioButton.Padding == new Thickness(3),
            "The RadioButton preview did not preserve its state or remained interactive.");
        Check(!keyboardActionButton.IsEnabled && !keyboardActionButton.IsDefault && !keyboardActionButton.IsCancel
            && keyboardActionButton.Command is null
            && keyboardActionButton.CommandParameter is "DocumentId"
            && keyboardActionButton.HorizontalContentAlignment == HorizontalAlignment.Left
            && keyboardActionButton.VerticalContentAlignment == VerticalAlignment.Top
            && keyboardActionButton.ClipToBounds && !invalidClipButton.ClipToBounds
            && keyboardActionButton.Opacity == 0.8
            && keyboardActionButton.MinWidth == 100 && keyboardActionButton.MaxWidth == 200
            && keyboardActionButton.MinHeight == 30 && keyboardActionButton.MaxHeight == 60
            && keyboardActionButton.Background is ISolidColorBrush buttonBackground
            && buttonBackground.Color == Color.FromRgb(240, 240, 240)
            && keyboardActionButton.BorderBrush is ISolidColorBrush buttonBorderBrush
            && buttonBorderBrush.Color == Colors.Blue
            && keyboardActionButton.BorderThickness == new Thickness(2)
            && keyboardActionButton.CornerRadius == new CornerRadius(6, 4, 2, 8)
            && keyboardActionButton.FontSize == 18
            && keyboardActionButton.FontStyle == FontStyle.Italic
            && keyboardActionButton.FontWeight == FontWeight.Medium
            && keyboardActionButton.Foreground is ISolidColorBrush buttonForeground
            && buttonForeground.Color == Colors.White
            && keyboardActionButton.Padding == new Thickness(8, 4),
            "The Button preview did not preserve its inert literal command parameter and suppress keyboard/command activation.");
        Check(!boundCommandParameterButton.IsEnabled
            && boundCommandParameterButton.Command is null
            && boundCommandParameterButton.CommandParameter is null,
            "The Button preview evaluated a bound command or command parameter.");
        Check(!hiddenPreviewButton.IsVisible && invalidVisibilityButton.IsVisible,
            "The Button preview did not preserve visibility or use its safe default.");
        Check(styledBorder.BorderBrush is ISolidColorBrush styledBorderBrush
            && styledBorderBrush.Color == Colors.Purple
            && styledBorder.CornerRadius == new CornerRadius(12, 4, 8, 2)
            && styledBorder.BorderThickness == new Thickness(2)
            && styledBorder.Padding == new Thickness(6),
            "The Border preview did not preserve its validated appearance properties.");
        Check(inputTextBox.Text == "Input preview"
            && inputTextBox.MaxLength == 120
            && inputTextBox.MinLines == 2 && inputTextBox.MaxLines == 5
            && inputTextBox.LineHeight == 22 && inputTextBox.UseFloatingPlaceholder
            && inputTextBox.AcceptsReturn
            && !inputTextBox.AcceptsTab
            && inputTextBox.Opacity == 0.75
            && inputTextBox.FontSize == 18
            && inputTextBox.TextAlignment == TextAlignment.Right
            && inputTextBox.TextWrapping == TextWrapping.WrapWithOverflow
            && inputTextBox.PlaceholderForeground is ISolidColorBrush placeholderForeground
            && placeholderForeground.Color == Colors.DarkGray
            && inputTextBox.FontStyle == FontStyle.Italic
            && inputTextBox.FontWeight == FontWeight.Medium
            && inputTextBox.HorizontalAlignment == HorizontalAlignment.Center
            && inputTextBox.VerticalAlignment == VerticalAlignment.Bottom
            && inputTextBox.IsReadOnly
            && inputTextBox.BorderThickness == new Thickness(2)
            && inputTextBox.CornerRadius == new CornerRadius(3, 6, 9, 12)
            && inputTextBox.Padding == new Thickness(8, 4)
            && inputTextBox.Background is ISolidColorBrush inputBackground
            && inputBackground.Color == Color.FromRgb(240, 240, 240)
            && inputTextBox.BorderBrush is ISolidColorBrush inputBorderBrush
            && inputBorderBrush.Color == Colors.Green
            && inputTextBox.Foreground is ISolidColorBrush inputForeground
            && inputForeground.Color == Colors.Blue,
            "The TextBox preview did not preserve its layout/input constraints or remained read-only.");
        Check(string.IsNullOrEmpty(boundPlaceholderTextBox.PlaceholderText),
            "The TextBox preview rendered a binding expression as literal placeholder text.");
        Check(defaultLineCountTextBox.MinLines == 0 && defaultLineCountTextBox.MaxLines == 0
            && invalidLineCountTextBox.MinLines == defaultLineCountTextBox.MinLines
            && invalidLineCountTextBox.MaxLines == defaultLineCountTextBox.MaxLines,
            "The TextBox preview did not preserve the unset line-count defaults or reject invalid values.");
        Check(double.IsNaN(defaultLineCountTextBox.LineHeight)
            && double.IsNaN(invalidLineCountTextBox.LineHeight)
            && !defaultLineCountTextBox.UseFloatingPlaceholder
            && invalidLineCountTextBox.UseFloatingPlaceholder == defaultLineCountTextBox.UseFloatingPlaceholder
            && Equals(invalidLineCountTextBox.PlaceholderForeground, defaultLineCountTextBox.PlaceholderForeground),
            $"The TextBox preview did not preserve line-height/placeholder defaults: line heights {defaultLineCountTextBox.LineHeight}/{invalidLineCountTextBox.LineHeight}, floating placeholders {defaultLineCountTextBox.UseFloatingPlaceholder}/{invalidLineCountTextBox.UseFloatingPlaceholder}.");
        Check(alignedTextBlock.TextAlignment == TextAlignment.Center
            && alignedTextBlock.FontStyle == FontStyle.Italic
            && alignedTextBlock.MaxLines == 2
            && alignedTextBlock.Opacity == 0.5
            && alignedTextBlock.LineHeight == 24
            && alignedTextBlock.TextTrimming == TextTrimming.CharacterEllipsis,
            "The TextBlock preview did not preserve its alignment, font style, line limit, or trimming.");
        Check(alignedTextBlock.TextWrapping == TextWrapping.NoWrap,
            "The TextBlock preview did not preserve its wrapping mode.");
        Check(alignedTextBlock.FontWeight == FontWeight.SemiBold,
            "The TextBlock preview did not preserve its selected font weight.");
        Check(alignedTextBlock.Foreground is ISolidColorBrush foreground && foreground.Color == Colors.Red,
            "The TextBlock preview did not preserve its foreground brush.");
        Check(inheritedFlowTextBlock.FlowDirection == FlowDirection.RightToLeft
            && invalidFlowTextBlock.FlowDirection == FlowDirection.LeftToRight
            && boundFlowTextBlock.FlowDirection == FlowDirection.LeftToRight
            && inheritedFlowTextBlock.UseLayoutRounding
            && invalidFlowTextBlock.UseLayoutRounding == boundFlowTextBlock.UseLayoutRounding,
            "The preview did not inherit explicit flow direction or ignore invalid/bound values.");
        Check(alignedContentControl.HorizontalContentAlignment == HorizontalAlignment.Stretch
            && alignedContentControl.VerticalContentAlignment == VerticalAlignment.Bottom
            && alignedContentControl.Background is ISolidColorBrush contentControlBackground
            && contentControlBackground.Color == Colors.PaleGreen
            && alignedContentControl.BorderBrush is ISolidColorBrush contentControlBorder
            && contentControlBorder.Color == Colors.Blue
            && alignedContentControl.BorderThickness == new Thickness(2)
            && alignedContentControl.CornerRadius == new CornerRadius(3)
            && alignedContentControl.FontSize == 16
            && alignedContentControl.FontStyle == FontStyle.Italic
            && alignedContentControl.FontWeight == FontWeight.Medium
            && alignedContentControl.Foreground is ISolidColorBrush contentControlForeground
            && contentControlForeground.Color == Colors.Navy
            && alignedContentControl.Padding == new Thickness(4)
            && alignedExpander.HorizontalContentAlignment == HorizontalAlignment.Right
            && alignedExpander.VerticalContentAlignment == VerticalAlignment.Top
            && alignedExpander.Background is ISolidColorBrush expanderBackground
            && expanderBackground.Color == Colors.White
            && alignedExpander.BorderBrush is ISolidColorBrush expanderBorder
            && expanderBorder.Color == Colors.Green
            && alignedExpander.BorderThickness == new Thickness(1)
            && alignedExpander.CornerRadius == new CornerRadius(4)
            && alignedExpander.FontSize == 17
            && alignedExpander.FontStyle == FontStyle.Oblique
            && alignedExpander.FontWeight == FontWeight.Bold
            && alignedExpander.Foreground is ISolidColorBrush expanderForeground
            && expanderForeground.Color == Colors.Black
            && alignedExpander.Padding == new Thickness(2)
            && alignedGroupBox.HorizontalContentAlignment == HorizontalAlignment.Left
            && alignedGroupBox.VerticalContentAlignment == VerticalAlignment.Bottom
            && alignedGroupBox.Background is ISolidColorBrush groupBoxBackground
            && groupBoxBackground.Color == Colors.AliceBlue
            && alignedGroupBox.BorderBrush is ISolidColorBrush groupBoxBorder
            && groupBoxBorder.Color == Colors.Purple
            && alignedGroupBox.BorderThickness == new Thickness(3)
            && alignedGroupBox.CornerRadius == new CornerRadius(5)
            && alignedGroupBox.FontSize == 15
            && alignedGroupBox.FontStyle == FontStyle.Italic
            && alignedGroupBox.FontWeight == FontWeight.SemiBold
            && alignedGroupBox.Foreground is ISolidColorBrush groupBoxForeground
            && groupBoxForeground.Color == Colors.DarkGreen
            && alignedGroupBox.Padding == new Thickness(6)
            && styledListBox.BorderBrush is ISolidColorBrush listBoxBorder
            && listBoxBorder.Color == Colors.Orange
            && styledListBox.BorderThickness == new Thickness(2)
            && styledListBox.CornerRadius == new CornerRadius(6)
            && styledListBox.FontSize == 14
            && styledListBox.FontStyle == FontStyle.Italic
            && styledListBox.FontWeight == FontWeight.Medium
            && styledListBox.Foreground is ISolidColorBrush listBoxForeground
            && listBoxForeground.Color == Colors.DarkBlue
            && styledListBox.Padding == new Thickness(5)
            && styledListBox.SelectedIndex == 1
            && Equals(styledListBox.SelectedItem, "Second item")
            && !styledListBox.IsEnabled
            && invalidSelectionListBox.SelectedIndex == -1
            && outOfRangeSelectionListBox.SelectedIndex == -1
            && boundSelectionListBox.SelectedIndex == -1
            && styledListBox.ItemsSource is IEnumerable<string> listBoxItems
            && listBoxItems.SequenceEqual(["First item", "Second item"])
            && !invalidSelectionListBox.IsEnabled
            && !outOfRangeSelectionListBox.IsEnabled
            && !boundSelectionListBox.IsEnabled
            && alignedLabel.HorizontalContentAlignment == HorizontalAlignment.Center
            && alignedLabel.VerticalContentAlignment == VerticalAlignment.Top
            && alignedLabel.FontSize == 18
            && alignedLabel.FontStyle == FontStyle.Italic
            && alignedLabel.FontWeight == FontWeight.SemiBold
            && alignedLabel.Foreground is ISolidColorBrush labelForeground
            && labelForeground.Color == Colors.Blue
            && alignedLabel.Background is ISolidColorBrush labelBackground
            && labelBackground.Color == Color.FromRgb(240, 240, 240)
            && alignedLabel.BorderBrush is ISolidColorBrush labelBorder
            && labelBorder.Color == Colors.Red
            && alignedLabel.BorderThickness == new Thickness(2)
            && alignedLabel.CornerRadius == new CornerRadius(7)
            && alignedLabel.Padding == new Thickness(8)
            && alignedLabel.Content is TextBlock labelText
            && alignedLabel.FontFamily.ToString() == "DejaVu Sans"
            && labelText.FontFamily.ToString() == "DejaVu Sans"
            && labelText.LineHeight == 22
            && labelText.MaxLines == 2
            && labelText.TextAlignment == TextAlignment.Right
            && labelText.TextTrimming == TextTrimming.WordEllipsis
            && labelText.TextWrapping == TextWrapping.NoWrap
            && invalidLabelLayout.Content is TextBlock invalidLabelText
            && double.IsNaN(invalidLabelText.LineHeight)
            && invalidLabelText.MaxLines == 0
            && boundLabelLayout.Content is TextBlock boundLabelText
            && double.IsNaN(boundLabelText.LineHeight)
            && boundLabelText.MaxLines == 0,
            $"The Label content-alignment or typography preview did not preserve its values: alignment={alignedLabel.HorizontalContentAlignment}/{alignedLabel.VerticalContentAlignment}, font={alignedLabel.FontFamily}/{alignedLabel.FontSize}/{alignedLabel.FontStyle}/{alignedLabel.FontWeight}, foreground={alignedLabel.Foreground}.");
        Check(staticItemsControl.ItemsSource is IEnumerable<string> staticItems
            && staticItems.SequenceEqual(["First static item", "Second static item"])
            && !staticItemsControl.IsEnabled,
            "The ItemsControl preview did not preserve its static text items while remaining inert.");
        Check(rectangle.Fill is ISolidColorBrush rectangleFill
            && rectangleFill.Color == Colors.CornflowerBlue
            && rectangle.Stroke is ISolidColorBrush rectangleStroke
            && rectangleStroke.Color == Colors.DarkBlue
            && rectangle.StrokeThickness == 2
            && !rectangle.IsEnabled
            && ellipse.Fill is ISolidColorBrush ellipseFill
            && ellipseFill.Color == Colors.Orange
            && line.StartPoint.Equals(new Point(5, 10))
            && line.Stroke is ISolidColorBrush lineStroke
            && lineStroke.Color == Colors.DarkGreen
            && line.StrokeThickness == 3
            && invalidLine.StartPoint.Equals(new Point(0, 0))
            && invalidLine.EndPoint.Equals(new Point(80, 40))
            && polygon.Points.SequenceEqual([new Point(40, 0), new Point(80, 60), new Point(0, 60)])
            && polyline.Points.SequenceEqual([new Point(5, 5), new Point(30, 30), new Point(60, 5)])
            && invalidPolygon.Points.SequenceEqual([new Point(40, 0), new Point(80, 60), new Point(0, 60)])
            && invalidPolyline.Points.SequenceEqual([new Point(0, 0), new Point(80, 40)]),
            "The safe vector-shape preview did not preserve literal geometry, fill and stroke values.");
        Check(toggleSwitch.Content?.ToString() == "Notifications"
            && toggleSwitch.IsChecked == true
            && toggleSwitch.IsThreeState
            && toggleSwitch.HorizontalContentAlignment == HorizontalAlignment.Right
            && toggleSwitch.VerticalContentAlignment == VerticalAlignment.Center
            && toggleSwitch.OnContent?.ToString() == "Enabled"
            && toggleSwitch.OffContent?.ToString() == "Disabled"
            && toggleSwitch.Background is ISolidColorBrush toggleBackground
            && toggleBackground.Color == Colors.PaleGreen
            && toggleSwitch.BorderBrush is ISolidColorBrush toggleBorderBrush
            && toggleBorderBrush.Color == Colors.DarkGreen
            && toggleSwitch.BorderThickness == new Thickness(3)
            && toggleSwitch.CornerRadius == new CornerRadius(5)
            && toggleSwitch.FontSize == 15
            && toggleSwitch.FontStyle == FontStyle.Italic
            && toggleSwitch.FontWeight == FontWeight.SemiBold
            && toggleSwitch.Foreground is ISolidColorBrush toggleForeground
            && toggleForeground.Color == Colors.DarkGreen
            && toggleSwitch.Padding == new Thickness(2)
            && toggleSwitch.Command is null
            && toggleSwitch.CommandParameter is "Notifications"
            && !toggleSwitch.IsEnabled,
            "The ToggleSwitch preview did not preserve its content/state or remained interactive.");
    }

    private static async Task VerifyGridEditingAsync(
        AxamlDesignerViewModel designer,
        Control palette,
        Control editorView,
        Func<string> getMarkup,
        Func<bool> isDirty)
    {
        string original = getMarkup();
        TreeView structure = palette.GetVisualDescendants().OfType<TreeView>()
            .Single(tree => tree.Name == "StructureTree");
        structure.SelectedItem = designer.PreviewRoot!.Children[0];
        await WaitUntilAsync(() => designer.SelectedNode?.StableName == "Span"
            && designer.IsSelectedGridChild, "Grid child selection");

        Border? FindSpan() => editorView.GetVisualDescendants().OfType<AxamlPreviewControl>()
            .SelectMany(static preview => preview.GetVisualDescendants().OfType<Border>())
            .FirstOrDefault(border => border.Tag is AxamlPreviewNode { StableName: "Span" });
        await WaitUntilAsync(() => FindSpan() is { Bounds.Width: > 0, Bounds.Height: > 0 }, "the spanning Grid preview");
        Border span = FindSpan()!;
        Check(Grid.GetRowSpan(span) == 2 && Grid.GetColumnSpan(span) == 2 && Grid.GetColumn(span) == 0,
            "The Grid preview did not preserve the child's row and column spans.");
        Grid renderedGrid = (Grid)span.Parent!;
        Check(Math.Abs(span.Bounds.Width - renderedGrid.Bounds.Width / 2) < 2
            && Math.Abs(span.Bounds.Height - renderedGrid.Bounds.Height * 2 / 3) < 2,
            "The spanning child was not rendered across its Grid cells.");

        Invoke(palette, "Col +");
        await WaitUntilAsync(() => designer.SelectedNode?.Properties.GetValueOrDefault("Grid.Column") == "1"
            && FindSpan() is { } current && Grid.GetColumn(current) == 1, "Grid column move");
        Check(isDirty() && getMarkup().Contains("av:Grid.Column=\"1\"", StringComparison.Ordinal),
            "Moving a Grid child did not update the shared document.");
        Check(!designer.MoveGridColumnLaterCommand.CanExecute(null),
            "The Grid allowed a span to overlap another child.");
        Invoke(palette, "Row +");
        await WaitUntilAsync(() => designer.SelectedNode?.Properties.GetValueOrDefault("Grid.Row") == "1"
            && FindSpan() is { } current && Grid.GetRow(current) == 1, "Grid row move");
        span = FindSpan()!;
        Check(Grid.GetRowSpan(span) == 2 && Grid.GetColumnSpan(span) == 2,
            "Moving a Grid child changed its rendered span.");
        string moved = getMarkup();

        designer.SelectedNode = designer.PreviewRoot!.Children[1];
        AxamlPropertyRowViewModel width = designer.PropertyRows.Single(row => row.Name == "Width");
        Check(!width.TrySetValue("-2"), "The Grid draft fixture did not retain an invalid value.");
        designer.SelectedNode = designer.PreviewRoot.Children[0];
        Check(designer.UnappliedDrafts.Count == 1 && !designer.MoveGridColumnEarlierCommand.CanExecute(null)
            && !designer.RemoveStructureCommand.CanExecute(null) && !designer.UndoCommand.CanExecute(null),
            "A draft for another Grid child did not block structural edits and history.");
        designer.MoveGridColumnEarlierCommand.Execute(null);
        Check(getMarkup() == moved, "A Grid edit changed the document while a draft was pending.");
        designer.DiscardStoredDraft(designer.UnappliedDrafts.Single());

        designer.UndoCommand.Execute(null);
        designer.UndoCommand.Execute(null);
        Check(getMarkup() == original && !isDirty(), "Grid undo did not restore the clean original.");
        designer.RedoCommand.Execute(null);
        designer.RedoCommand.Execute(null);
        Check(getMarkup() == moved && isDirty(), "Grid redo did not restore both moves.");
        designer.SelectedNode = designer.PreviewRoot!.Children[1];
        Invoke(palette, "Remove");
        Check(designer.PreviewRoot!.Children.Count == 1, "Grid child removal did not update the preview.");
        designer.UndoCommand.Execute(null);
        designer.UndoCommand.Execute(null);
        designer.UndoCommand.Execute(null);
        Check(getMarkup() == original && !isDirty(), "Grid removal and move undo did not restore the original.");

        designer.SelectedNode = designer.PreviewRoot;
        Button addGridRow = palette.GetVisualDescendants().OfType<Button>()
            .Single(button => string.Equals(button.Content?.ToString(), "Add Auto row", StringComparison.Ordinal));
        Check(addGridRow.IsEnabled && addGridRow.Command?.CanExecute(null) == true,
            "The safe Grid did not enable the Add Auto row action.");
        Invoke(palette, "Add Auto row");
        await WaitUntilAsync(() => designer.PreviewRoot!.RowDefinitions.Count == 4
            && getMarkup().Contains("RowDefinitions=\"*,*,*,Auto\"", StringComparison.Ordinal),
            "adding a Grid row definition");
        Invoke(palette, "Add Auto column");
        await WaitUntilAsync(() => designer.PreviewRoot!.ColumnDefinitions.Count == 5
            && getMarkup().Contains("ColumnDefinitions=\"*,*,*,*,Auto\"", StringComparison.Ordinal),
            "adding a Grid column definition");
        Check(isDirty(), "Grid definition additions did not update the shared document.");
        Button removeGridRow = palette.GetVisualDescendants().OfType<Button>()
            .Single(button => string.Equals(button.Content?.ToString(), "Remove last row", StringComparison.Ordinal));
        Button removeGridColumn = palette.GetVisualDescendants().OfType<Button>()
            .Single(button => string.Equals(button.Content?.ToString(), "Remove last column", StringComparison.Ordinal));
        Check(removeGridRow.IsEnabled && removeGridRow.Command?.CanExecute(null) == true
            && removeGridColumn.IsEnabled && removeGridColumn.Command?.CanExecute(null) == true,
            "The unused final Grid row and column were not removable.");
        Invoke(palette, "Remove last row");
        await WaitUntilAsync(() => designer.PreviewRoot!.RowDefinitions.Count == 3,
            "removing an unused final Grid row");
        Invoke(palette, "Remove last column");
        await WaitUntilAsync(() => designer.PreviewRoot!.ColumnDefinitions.Count == 4,
            "removing an unused final Grid column");
        await WaitUntilAsync(() => getMarkup() == original && !isDirty(),
            "removing unused Grid tracks back to the clean document");
        designer.UndoCommand.Execute(null);
        designer.UndoCommand.Execute(null);
        Check(designer.PreviewRoot!.RowDefinitions.Count == 4
            && designer.PreviewRoot.ColumnDefinitions.Count == 5,
            "Grid track removal undo did not restore the expanded definitions.");
        designer.UndoCommand.Execute(null);
        designer.UndoCommand.Execute(null);
        await WaitUntilAsync(() => getMarkup() == original && !isDirty(),
            "undoing Grid definition additions and removals");
        designer.SelectedNode = designer.PreviewRoot;
        designer.SelectedGridTrackOption = "Row 2: *";
        designer.GridTrackSizeValue = "2*";
        Button setGridTrackSize = palette.GetVisualDescendants().OfType<Button>()
            .Single(button => string.Equals(button.Content?.ToString(), "Set track size", StringComparison.Ordinal));
        Check(setGridTrackSize.IsEnabled && setGridTrackSize.Command?.CanExecute(null) == true,
            "The explicit Grid track size editor was not enabled.");
        Invoke(palette, "Set track size");
        await WaitUntilAsync(() => designer.PreviewRoot!.RowDefinitions.ElementAtOrDefault(2) == "2*"
            && getMarkup().Contains("RowDefinitions=\"*,*,2*\"", StringComparison.Ordinal),
            "resizing an explicit Grid row");
        designer.UndoCommand.Execute(null);
        await WaitUntilAsync(() => getMarkup() == original && !isDirty(),
            "undoing an explicit Grid track size");

        AxamlPreviewNode invalidGrid = new AxamlPreviewService().Parse(
            "<Grid RowDefinitions='-1,NaN' ColumnDefinitions='*'><Button Grid.Row='-1' Grid.RowSpan='0' /></Grid>");
        Check(!invalidGrid.HasSafeGridLayout, "An invalid Grid was offered for editing.");
        _ = new AxamlPreviewControl { PreviewNode = invalidGrid };
    }

    private static async Task VerifySingleContentRemovalAsync(
        AxamlDesignerViewModel designer,
        Control palette,
        Func<string> getMarkup,
        Func<bool> isDirty)
    {
        string original = getMarkup();
        TreeView structure = palette.GetVisualDescendants().OfType<TreeView>()
            .Single(tree => tree.Name == "StructureTree");
        structure.SelectedItem = designer.PreviewRoot!.Children[0];
        await WaitUntilAsync(() => designer.SelectedNode?.TypeName == "Button",
            "single-content child selection");
        Check(designer.RemoveStructureCommand.CanExecute(null),
            "The supported single-content child was not available for removal.");
        Invoke(palette, "Remove");
        await WaitUntilAsync(() => designer.PreviewRoot!.Children.Count == 0,
            "single-content child removal");
        Check(isDirty(), "Removing single content did not update the document buffer.");
        designer.UndoCommand.Execute(null);
        await WaitUntilAsync(() => getMarkup() == original && designer.PreviewRoot!.Children.Count == 1,
            "single-content removal undo");
        Check(!isDirty(), "Undoing single-content removal did not restore the clean document.");
    }

    private static ProjectExplorerItemViewModel FindItem(IEnumerable<ProjectExplorerItemViewModel> items, string path)
    {
        foreach (ProjectExplorerItemViewModel item in items)
        {
            if (WorkspacePathIdentity.AreEqual(item.Item.Path, path))
            {
                return item;
            }
            ProjectExplorerItemViewModel? nested = FindItemOrNull(item.Children, path);
            if (nested is not null)
            {
                return nested;
            }
        }
        throw new InvalidOperationException($"Explorer did not contain '{path}'.");
    }

    private static ProjectExplorerItemViewModel? FindItemOrNull(IEnumerable<ProjectExplorerItemViewModel> items, string path)
    {
        foreach (ProjectExplorerItemViewModel item in items)
        {
            if (WorkspacePathIdentity.AreEqual(item.Item.Path, path))
            {
                return item;
            }
            ProjectExplorerItemViewModel? nested = FindItemOrNull(item.Children, path);
            if (nested is not null)
            {
                return nested;
            }
        }
        return null;
    }

    private static Task WaitForActiveAsync(MainWindowViewModel workbench, string path) =>
        WaitUntilAsync(() => workbench.ActiveEditorTab.FilePath is { } active
            && WorkspacePathIdentity.AreEqual(active, path), $"editor for {Path.GetFileName(path)}");

    private static Button FindButton(Window window, string label) =>
        window.GetVisualDescendants().OfType<Button>()
            .Single(button => string.Equals(button.Content?.ToString(), label, StringComparison.Ordinal));

    private static void Click(Window window, string label) =>
        FindButton(window, label).RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

    private static void Invoke(Control parent, string label)
    {
        Button button = parent.GetVisualDescendants().OfType<Button>()
            .Single(item => string.Equals(item.Content?.ToString(), label, StringComparison.Ordinal));
        Check(button.IsEnabled, $"The '{label}' button is disabled.");
        IInvokeProvider provider = ControlAutomationPeer.CreatePeerForElement(button)
            .GetProvider<IInvokeProvider>() ?? throw new InvalidOperationException($"The '{label}' button has no invoke provider.");
        provider.Invoke();
    }

    private static async Task WaitUntilAsync(Func<bool> condition, string description)
    {
        for (int attempt = 0; attempt < 300; attempt++)
        {
            if (condition()) return;
            await Task.Delay(100);
        }
        throw new TimeoutException($"Timed out waiting for {description}.");
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private sealed class BinarySmokeFactory : IDocumentEditorFactory<EditorTabViewModel>
    {
        public string Id => "binary-smoke";
        public int Priority => 1;
        public BinarySmokeSession? LastSession { get; private set; }

        public bool CanOpen(string filePath) => Path.GetExtension(filePath) == ".bin";

        public EditorTabViewModel Create(string filePath)
        {
            LastSession = new BinarySmokeSession();
            return new EditorTabViewModel(
                LastSession,
                new Border { Child = new TextBlock { Text = "Synthetic binary editor" } },
                _ => Task.FromResult<string?>(null),
                Id);
        }
    }

    private sealed class BinarySmokeSession : IDocumentEditorSession
    {
        private byte[] _savedContent = [];
        public string? FilePath { get; private set; }
        public string DisplayName => Path.GetFileName(FilePath) ?? "Untitled.bin";
        public byte[] Content { get; private set; } = [];
        public bool IsDirty => !Content.SequenceEqual(_savedContent);
        public bool IsDisposed { get; private set; }
        public event EventHandler? ContentChanging;
        public event EventHandler? StateChanged;

        public async Task OpenAsync(string filePath, CancellationToken cancellationToken = default)
        {
            Content = await File.ReadAllBytesAsync(filePath, cancellationToken);
            _savedContent = Content.ToArray();
            FilePath = filePath;
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public async Task SaveAsync(string filePath, CancellationToken cancellationToken = default)
        {
            byte[] snapshot = Content.ToArray();
            await File.WriteAllBytesAsync(filePath, snapshot, cancellationToken);
            _savedContent = snapshot;
            FilePath = filePath;
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ReplaceContent(byte[] content)
        {
            ContentChanging?.Invoke(this, EventArgs.Empty);
            Content = content.ToArray();
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose() => IsDisposed = true;
    }

    private sealed class SmokeApplication : Application
    {
        public static ServiceProvider? Services { get; private set; }

        public override void Initialize()
        {
            Styles.Add(new FluentTheme());
            Styles.Add(new StyleInclude(new Uri("avares://AvaloniaEdit/"))
            {
                Source = new Uri("avares://AvaloniaEdit/Themes/Fluent/AvaloniaEdit.xaml"),
            });
        }

        public override void OnFrameworkInitializationCompleted()
        {
            IClassicDesktopStyleApplicationLifetime desktop =
                (IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!;
            ServiceCollection services = new();
            App.ConfigureServices(
                services,
                () => desktop.MainWindow,
                userConfigRoot: Path.Combine(s_root, "config"),
                systemConfigRoot: Path.Combine(s_root, "system"));
            services.AddSingleton<IDocumentEditorFactory<EditorTabViewModel>>(new BinarySmokeFactory());
            Services = services.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true,
            });
            desktop.MainWindow = App.CreateMainWindow(Services);
            base.OnFrameworkInitializationCompleted();
        }
    }
}
