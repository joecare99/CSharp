using AA98_AvlnCodeStudio.Model.Workspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class AxamlMvvmBindingAnalyzerTests
{
    [TestMethod]
    public void Analyze_ExtractsSimpleBindingsForSupportedControls()
    {
        const string markup = """
            <Window xmlns="https://github.com/avaloniaui">
              <StackPanel>
                <TextBox Text="{Binding Name, Mode=TwoWay}" />
                <CheckBox IsChecked="{Binding Path=Enabled}" />
                <NumericUpDown Value="{Binding Quantity}" />
                <Slider Value="{Binding Volume}" />
                <ComboBox SelectedIndex="{Binding Country}" />
                <Button Command="{Binding SaveCommand}" />
                <TextBlock Text="{Binding Status, Mode=OneWay}" />
              </StackPanel>
            </Window>
            """;

        AxamlMvvmMemberRequirement[] requirements = new AxamlMvvmBindingAnalyzer()
            .Analyze(markup)
            .ToArray();

        Assert.AreEqual(7, requirements.Length);
        CollectionAssert.AreEqual(
            new[] { "Name", "Enabled", "Quantity", "Volume", "Country", "SaveCommand", "Status" },
            requirements.Select(static requirement => requirement.Name).ToArray());
        Assert.AreEqual("bool?", requirements[1].TypeName);
        Assert.AreEqual("decimal?", requirements[2].TypeName);
        Assert.IsTrue(requirements[5].IsCommand);
        Assert.IsFalse(requirements[0].IsCommand);
    }

    [TestMethod]
    public void Analyze_RejectsComplexBindingPathsInsteadOfGuessing()
    {
        const string markup = """
            <Window xmlns="https://github.com/avaloniaui">
              <TextBox Text="{Binding Customer.Name}" />
            </Window>
            """;

        Assert.ThrowsExactly<InvalidDataException>(() =>
            new AxamlMvvmBindingAnalyzer().Analyze(markup));
    }

    [TestMethod]
    public void Analyze_RejectsBindingsInTemplatesWithDifferentDataContexts()
    {
        const string markup = """
            <Window xmlns="https://github.com/avaloniaui">
              <StackPanel>
                <TextBox Text="{Binding WindowTitle}" />
                <ItemsControl ItemsSource="{Binding Items}">
                  <ItemsControl.ItemTemplate>
                    <DataTemplate>
                      <TextBlock Text="{Binding ItemTitle}" />
                    </DataTemplate>
                  </ItemsControl.ItemTemplate>
                </ItemsControl>
              </StackPanel>
            </Window>
            """;

        InvalidDataException exception = Assert.ThrowsExactly<InvalidDataException>(() =>
            new AxamlMvvmBindingAnalyzer().Analyze(markup));

        StringAssert.Contains(exception.Message, "DataTemplate");
    }

    [TestMethod]
    public void Analyze_RejectsBindingsBelowNestedDataContext()
    {
        const string markup = """
            <Window xmlns="https://github.com/avaloniaui">
              <StackPanel>
                <TextBox Text="{Binding WindowTitle}" />
                <ContentControl DataContext="{Binding SelectedItem}">
                  <TextBlock Text="{Binding ItemTitle}" />
                </ContentControl>
              </StackPanel>
            </Window>
            """;

        InvalidDataException exception = Assert.ThrowsExactly<InvalidDataException>(() =>
            new AxamlMvvmBindingAnalyzer().Analyze(markup));

        StringAssert.Contains(exception.Message, "DataContext");
    }

    [TestMethod]
    public void Analyze_RejectsBindingsWithExplicitSource()
    {
        const string markup = """
            <Window xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
              <TextBox x:Name="SourceTextBox" Text="{Binding Name, ElementName=SourceTextBox}" />
            </Window>
            """;

        InvalidDataException exception = Assert.ThrowsExactly<InvalidDataException>(() =>
            new AxamlMvvmBindingAnalyzer().Analyze(markup));

        StringAssert.Contains(exception.Message, "explicit source");
    }

    [TestMethod]
    public void Analyze_IgnoresNonBindingExpressionsAndUnsupportedControls()
    {
        const string markup = """
            <Window xmlns="https://github.com/avaloniaui"
                    xmlns:custom="using:Example">
              <TextBox Text="{DynamicResource Name}" />
              <CustomControl Value="{Binding Something}" />
              <custom:TextBox Text="{Binding NotAnAvaloniaTextBox}" />
            </Window>
            """;

        Assert.AreEqual(0, new AxamlMvvmBindingAnalyzer().Analyze(markup).Count);
    }

    [TestMethod]
    public void Analyze_RejectsMalformedBindingExpressions()
    {
        const string markup = """
            <Window xmlns="https://github.com/avaloniaui">
              <TextBox Text="{Binding }" />
            </Window>
            """;

        Assert.ThrowsExactly<InvalidDataException>(() =>
            new AxamlMvvmBindingAnalyzer().Analyze(markup));
    }

    [TestMethod]
    public void Analyze_RejectsUnclosedAndInvalidMemberPaths()
    {
        const string unclosed = """
            <Window xmlns="https://github.com/avaloniaui">
              <TextBox Text="{Binding Name" />
            </Window>
            """;
        const string invalidPath = """
            <Window xmlns="https://github.com/avaloniaui">
              <TextBox Text="{Binding first name}" />
            </Window>
            """;

        Assert.ThrowsExactly<InvalidDataException>(() =>
            new AxamlMvvmBindingAnalyzer().Analyze(unclosed));
        Assert.ThrowsExactly<InvalidDataException>(() =>
            new AxamlMvvmBindingAnalyzer().Analyze(invalidPath));
    }
}
