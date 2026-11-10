using AA98_AvlnCodeStudio.Model.Workspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class AxamlMvvmGeneratorTests
{
    [TestMethod]
    public void Generate_BindsNamedTextBoxesAndButtonsAndCreatesToolkitViewModel()
    {
        const string markup = """
            <Window xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    x:Class="Sample.Views.LoginView">
              <StackPanel>
                <AutoCompleteBox x:Name="CityAutoCompleteBox" Text="Paris" />
                <TextBox x:Name="UserNameTextBox" Text="initial" />
                <CheckBox x:Name="RememberMeCheckBox" IsChecked="True" CommandParameter="remember-me" />
                <ToggleSwitch x:Name="NotificationsToggleSwitch" IsChecked="False" />
                <RadioButton x:Name="PersonalRadioButton" IsChecked="True" />
                <NumericUpDown x:Name="QuantityNumericUpDown" Value="12.5" />
                <Slider x:Name="VolumeSlider" Value="0.75" />
                <ComboBox x:Name="CountryComboBox" SelectedIndex="1" />
                <DatePicker x:Name="BirthDatePicker" SelectedDate="2024-02-29" />
                <CalendarDatePicker x:Name="EventCalendarDatePicker" SelectedDate="2024-03-01" />
                <TimePicker x:Name="ArrivalTimePicker" SelectedTime="13:45:30" />
                <Calendar x:Name="ReleaseCalendar" SelectedDate="2024-02-29" />
                <ColorPicker x:Name="ThemeColorPicker" Color="#FF123456" />
                <TabControl x:Name="SettingsTabControl" SelectedIndex="1">
                  <TabItem Header="General" />
                  <TabItem Header="Advanced" />
                </TabControl>
                <Menu>
                  <MenuItem x:Name="OpenMenuItem" Header="Open" />
                </Menu>
                <Button x:Name="SubmitButton" Content="Submit" CommandParameter="document-id" />
                <TextBlock Text="Static label" />
                <TextBlock x:Name="WelcomeTextBlock" Text="Welcome" />
              </StackPanel>
            </Window>
            """;

        WorkspaceFileChangeRequest[] changes = new AxamlMvvmGenerator()
            .Generate("Views\\LoginView.axaml", markup)
            .ToArray();

        Assert.AreEqual(2, changes.Length);
        Assert.AreEqual(Path.Combine("Views", "LoginView.axaml"), changes[0].RelativePath);
        Assert.AreEqual(Path.Combine("Views", "LoginViewViewModel.cs"), changes[1].RelativePath);
        StringAssert.Contains(changes[0].ProposedContent, "x:DataType=\"vm:LoginViewViewModel\"");
        StringAssert.Contains(changes[0].ProposedContent, "<Window.DataContext>");
        StringAssert.Contains(changes[0].ProposedContent, "<vm:LoginViewViewModel");
        StringAssert.Contains(changes[0].ProposedContent, "Text=\"{Binding City, Mode=TwoWay}\"");
        StringAssert.Contains(changes[0].ProposedContent, "{Binding UserName, Mode=TwoWay}");
        StringAssert.Contains(changes[0].ProposedContent, "IsChecked=\"{Binding RememberMe, Mode=TwoWay}\"");
        StringAssert.Contains(changes[0].ProposedContent, "IsChecked=\"{Binding Notifications, Mode=TwoWay}\"");
        StringAssert.Contains(changes[0].ProposedContent, "IsChecked=\"{Binding Personal, Mode=TwoWay}\"");
        StringAssert.Contains(changes[0].ProposedContent, "Command=\"{Binding RememberMeCheckBoxCommand}\"");
        StringAssert.Contains(changes[0].ProposedContent, "CommandParameter=\"remember-me\"");
        StringAssert.Contains(changes[0].ProposedContent, "Command=\"{Binding NotificationsToggleSwitchCommand}\"");
        StringAssert.Contains(changes[0].ProposedContent, "Command=\"{Binding PersonalRadioButtonCommand}\"");
        StringAssert.Contains(changes[0].ProposedContent, "Value=\"{Binding Quantity, Mode=TwoWay}\"");
        StringAssert.Contains(changes[0].ProposedContent, "Value=\"{Binding Volume, Mode=TwoWay}\"");
        StringAssert.Contains(changes[0].ProposedContent, "SelectedIndex=\"{Binding Country, Mode=TwoWay}\"");
        StringAssert.Contains(changes[0].ProposedContent, "SelectedDate=\"{Binding Birth, Mode=TwoWay}\"");
        StringAssert.Contains(changes[0].ProposedContent, "SelectedDate=\"{Binding Event, Mode=TwoWay}\"");
        StringAssert.Contains(changes[0].ProposedContent, "SelectedTime=\"{Binding Arrival, Mode=TwoWay}\"");
        StringAssert.Contains(changes[0].ProposedContent, "SelectedDate=\"{Binding Release, Mode=TwoWay}\"");
        StringAssert.Contains(changes[0].ProposedContent, "Color=\"{Binding Theme, Mode=TwoWay}\"");
        StringAssert.Contains(changes[0].ProposedContent, "SelectedIndex=\"{Binding Settings, Mode=TwoWay}\"");
        StringAssert.Contains(changes[0].ProposedContent, "Command=\"{Binding OpenCommand}\"");
        StringAssert.Contains(changes[0].ProposedContent, "{Binding SubmitCommand}");
        StringAssert.Contains(changes[0].ProposedContent, "CommandParameter=\"document-id\"");
        StringAssert.Contains(changes[0].ProposedContent, "Text=\"{Binding Welcome, Mode=OneWay}\"");
        StringAssert.Contains(changes[0].ProposedContent, "Text=\"Static label\"");
        StringAssert.Contains(changes[1].ProposedContent, "namespace Sample.Views;");
        StringAssert.Contains(changes[1].ProposedContent, "private string? _city = \"Paris\";");
        StringAssert.Contains(changes[1].ProposedContent, "private string _userName = \"initial\";");
        StringAssert.Contains(changes[1].ProposedContent, "private bool? _rememberMe = true;");
        StringAssert.Contains(changes[1].ProposedContent, "private bool? _notifications = false;");
        StringAssert.Contains(changes[1].ProposedContent, "private bool? _personal = true;");
        StringAssert.Contains(changes[1].ProposedContent, "private decimal? _quantity = 12.5m;");
        StringAssert.Contains(changes[1].ProposedContent, "private double _volume = 0.75d;");
        StringAssert.Contains(changes[1].ProposedContent, "private global::System.DateTimeOffset? _birth = global::System.DateTimeOffset.ParseExact(\"2024-02-29\", \"yyyy-MM-dd\", global::System.Globalization.CultureInfo.InvariantCulture, global::System.Globalization.DateTimeStyles.None);");
        StringAssert.Contains(changes[1].ProposedContent, "private global::System.DateTime? _event = global::System.DateTime.ParseExact(\"2024-03-01\", \"yyyy-MM-dd\", global::System.Globalization.CultureInfo.InvariantCulture, global::System.Globalization.DateTimeStyles.None);");
        StringAssert.Contains(changes[1].ProposedContent, "private global::System.TimeSpan? _arrival = global::System.TimeSpan.Parse(\"13:45:30\", global::System.Globalization.CultureInfo.InvariantCulture);");
        StringAssert.Contains(changes[1].ProposedContent, "private global::System.DateTime? _release = global::System.DateTime.ParseExact(\"2024-02-29\", \"yyyy-MM-dd\", global::System.Globalization.CultureInfo.InvariantCulture, global::System.Globalization.DateTimeStyles.None);");
        StringAssert.Contains(changes[1].ProposedContent, "private global::Avalonia.Media.Color _theme = global::Avalonia.Media.Color.Parse(\"#FF123456\");");
        StringAssert.Contains(changes[1].ProposedContent, "private int _settings = 1;");
        StringAssert.Contains(changes[1].ProposedContent, "private int _country = 1;");
        StringAssert.Contains(changes[1].ProposedContent, "private string _welcome = \"Welcome\";");
        StringAssert.Contains(changes[1].ProposedContent, "[RelayCommand]");
        StringAssert.Contains(changes[1].ProposedContent, "private void Open()");
        StringAssert.Contains(changes[1].ProposedContent, "private void Submit(string? commandParameter)");
        StringAssert.Contains(changes[1].ProposedContent, "private void RememberMeCheckBox(string? commandParameter)");
        StringAssert.Contains(changes[1].ProposedContent, "private void NotificationsToggleSwitch()");
        StringAssert.Contains(changes[1].ProposedContent, "private void PersonalRadioButton()");
    }

    [TestMethod]
    public void Generate_RejectsExistingBindingsAndDataContext()
    {
        const string alreadyBound = """
            <Window xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    x:Class="Sample.MainWindow">
              <TextBox x:Name="NameTextBox" Text="{Binding ExistingName}" />
              <Button x:Name="SaveButton" Command="{Binding ExistingSaveCommand}" />
              <TextBlock x:Name="WelcomeTextBlock" Text="Welcome" />
            </Window>
            """;
        const string withDataContext = """
            <Window xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    x:Class="Sample.MainWindow"
                    DataContext="{Binding ExistingViewModel}">
              <TextBox x:Name="NameTextBox" />
            </Window>
            """;

        InvalidDataException bindingException = Assert.ThrowsExactly<InvalidDataException>(() =>
            new AxamlMvvmGenerator().Generate("MainWindow.axaml", alreadyBound));
        Assert.ThrowsExactly<InvalidDataException>(() =>
            new AxamlMvvmGenerator().Generate("MainWindow.axaml", withDataContext));
        StringAssert.Contains(bindingException.Message, "already contains Binding expressions");
    }

    [TestMethod]
    public void Generate_RejectsBoundCommandParameterInsteadOfRedirectingItsDataContext()
    {
        const string markup = """
            <Window xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    x:Class="Sample.MainWindow">
              <StackPanel>
                <TextBox x:Name="NameTextBox" />
                <Button x:Name="SaveButton" CommandParameter="{Binding DocumentId}" />
              </StackPanel>
            </Window>
            """;

        InvalidDataException exception = Assert.ThrowsExactly<InvalidDataException>(() =>
            new AxamlMvvmGenerator().Generate("MainWindow.axaml", markup));

        StringAssert.Contains(exception.Message, "could redirect them");
    }

    [TestMethod]
    public void Generate_DoesNotGenerateBindingsForCustomNamespaceLookalikes()
    {
        const string markup = """
            <Window xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:custom="urn:sample"
                    x:Class="Sample.MainWindow">
              <custom:TextBox x:Name="InputTextBox" Text="keep" />
              <custom:Button x:Name="SaveButton" />
            </Window>
            """;

        Assert.ThrowsExactly<InvalidDataException>(() =>
            new AxamlMvvmGenerator().Generate("MainWindow.axaml", markup));
        Assert.IsFalse(markup.Contains("{Binding", System.StringComparison.Ordinal));
    }

    [TestMethod]
    public void Generate_RejectsUnsafeMarkupAndClassNames()
    {
        AxamlMvvmGenerator generator = new();

        Assert.ThrowsExactly<System.Xml.XmlException>(() =>
            generator.Generate("MainWindow.axaml", "<!DOCTYPE x [<!ENTITY e 'x'>]><Window x:Class='Sample.MainWindow' />"));
        Assert.ThrowsExactly<InvalidDataException>(() =>
            generator.Generate(
                "MainWindow.axaml",
                "<Window xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' x:Class='Sample.Bad-Type'><TextBox x:Name='InputTextBox' /></Window>"));
    }

    [TestMethod]
    public void Generate_RejectsGeneratedCommandAndPropertyNameCollisions()
    {
        const string markup = """
            <Window xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    x:Class="Sample.MainWindow">
              <StackPanel>
                <TextBox x:Name="SaveTextBox" />
                <Button x:Name="SaveButton" />
              </StackPanel>
            </Window>
            """;

        Assert.ThrowsExactly<InvalidDataException>(() =>
            new AxamlMvvmGenerator().Generate("MainWindow.axaml", markup));
    }

    [TestMethod]
    public void Generate_RejectsInvalidCheckBoxLiteral()
    {
        const string markup = """
            <Window xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    x:Class="Sample.MainWindow">
              <CheckBox x:Name="EnabledCheckBox" IsChecked="maybe" />
            </Window>
            """;

        Assert.ThrowsExactly<InvalidDataException>(() =>
            new AxamlMvvmGenerator().Generate("MainWindow.axaml", markup));
    }

    [TestMethod]
    public void Generate_RejectsInvalidNumericUpDownLiteral()
    {
        const string markup = """
            <Window xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    x:Class="Sample.MainWindow">
              <NumericUpDown x:Name="QuantityNumericUpDown" Value="1,25" />
            </Window>
            """;

        Assert.ThrowsExactly<InvalidDataException>(() =>
            new AxamlMvvmGenerator().Generate("MainWindow.axaml", markup));
    }

    [TestMethod]
    public void Generate_RejectsNonFiniteSliderValue()
    {
        const string markup = """
            <Window xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    x:Class="Sample.MainWindow">
              <Slider x:Name="VolumeSlider" Value="NaN" />
            </Window>
            """;

        Assert.ThrowsExactly<InvalidDataException>(() =>
            new AxamlMvvmGenerator().Generate("MainWindow.axaml", markup));
    }

    [TestMethod]
    public void Generate_RejectsInvalidComboBoxSelectedIndex()
    {
        const string markup = """
            <Window xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    x:Class="Sample.MainWindow">
              <ComboBox x:Name="CountryComboBox" SelectedIndex="1.5" />
            </Window>
            """;

        Assert.ThrowsExactly<InvalidDataException>(() =>
            new AxamlMvvmGenerator().Generate("MainWindow.axaml", markup));
    }

    [TestMethod]
    public void Generate_RejectsNegativeTabControlSelectedIndex()
    {
        const string markup = """
            <Window xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    x:Class="Sample.MainWindow">
              <TabControl x:Name="SettingsTabControl" SelectedIndex="-2">
                <TabItem Header="General" />
              </TabControl>
            </Window>
            """;

        Assert.ThrowsExactly<InvalidDataException>(() =>
            new AxamlMvvmGenerator().Generate("MainWindow.axaml", markup));
    }

    [TestMethod]
    public void Generate_RejectsInvalidDatePickerSelectedDate()
    {
        const string markup = """
            <Window xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    x:Class="Sample.Views.ScheduleView">
              <DatePicker x:Name="StartDatePicker" SelectedDate="2023-02-29" />
            </Window>
            """;

        Assert.ThrowsExactly<InvalidDataException>(() =>
            new AxamlMvvmGenerator().Generate("Views\\ScheduleView.axaml", markup));
    }

    [TestMethod]
    public void Generate_RejectsInvalidTimePickerSelectedTime()
    {
        const string markup = """
            <Window xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    x:Class="Sample.Views.ScheduleView">
              <TimePicker x:Name="ArrivalTimePicker" SelectedTime="25:00" />
            </Window>
            """;

        Assert.ThrowsExactly<InvalidDataException>(() =>
            new AxamlMvvmGenerator().Generate("Views\\ScheduleView.axaml", markup).ToArray());
    }

    [TestMethod]
    public void Generate_RejectsInvalidCalendarSelectedDate()
    {
        const string markup = """
            <Window xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    x:Class="Sample.Views.ScheduleView">
              <Calendar x:Name="ReleaseCalendar" SelectedDate="2024-02-30" />
            </Window>
            """;

        Assert.ThrowsExactly<InvalidDataException>(() =>
            new AxamlMvvmGenerator().Generate("Views\\ScheduleView.axaml", markup).ToArray());
    }

    [TestMethod]
    public void Generate_RejectsInvalidColorPickerColor()
    {
        const string markup = """
            <Window xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    x:Class="Sample.Views.ThemeView">
              <ColorPicker x:Name="AccentColorPicker" Color="NotAColor" />
            </Window>
            """;

        Assert.ThrowsExactly<InvalidDataException>(() =>
            new AxamlMvvmGenerator().Generate("Views\\ThemeView.axaml", markup).ToArray());
    }
}
