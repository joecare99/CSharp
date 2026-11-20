using AA98_AvlnCodeStudio.Model.Workspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class AxamlMvvmBindingAnalyzerTests
{
    [TestMethod]
    public void Analyze_RejectsBindingsUnderNestedDataType()
    {
        const string markup = """
            <Window xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:vm="using:Sample" x:DataType="vm:RootViewModel">
              <StackPanel x:DataType="vm:OtherViewModel">
                <TextBox Text="{CompiledBinding OtherName}" />
              </StackPanel>
            </Window>
            """;

        Assert.ThrowsExactly<InvalidDataException>(() =>
            new AxamlMvvmBindingAnalyzer().Analyze(markup));
        Assert.ThrowsExactly<InvalidDataException>(() =>
            new AxamlMvvmBindingAnalyzer().Analyze(markup.Replace("CompiledBinding", "Binding")));

        string withoutNestedBinding = markup.Replace(
            "<TextBox Text=\"{CompiledBinding OtherName}\" />",
            "<TextBox Text=\"{DynamicResource OtherName}\" />");
        Assert.AreEqual(0, new AxamlMvvmBindingAnalyzer().Analyze(withoutNestedBinding).Count);
    }

    [TestMethod]
    public void Analyze_AcceptsEditableExplicitPathAfterLeadingComma()
    {
        foreach (string kind in new[] { "CompiledBinding", "ReflectionBinding" })
        {
            string markup = $"<TextBox xmlns='https://github.com/avaloniaui' Text='{{{kind}, Path=Caption, Mode=OneWay}}' />";
            Assert.AreEqual("Caption", new AxamlMvvmBindingAnalyzer().Analyze(markup).Single().Name);
        }
    }

    [TestMethod]
    public void Analyze_HandlesExplicitBindingKindsAndRejectsOtherSources()
    {
        const string markup = """
            <Window xmlns="https://github.com/avaloniaui">
              <StackPanel>
                <TextBox Text="{CompiledBinding Name, Mode=TwoWay}" />
                <TextBlock Text="{ReflectionBinding Path=Status}" />
                <Button Command="{CompiledBinding SaveCommand}" />
              </StackPanel>
            </Window>
            """;

        AxamlMvvmMemberRequirement[] requirements = new AxamlMvvmBindingAnalyzer().Analyze(markup).ToArray();
        CollectionAssert.AreEqual(new[] { "Name", "Status", "SaveCommand" },
            requirements.Select(static requirement => requirement.Name).ToArray());
        Assert.IsTrue(requirements[2].IsCommand);

        foreach (string kind in new[] { "CompiledBinding", "ReflectionBinding" })
        {
            foreach (string source in new[] { "ElementName=Other", "Source={StaticResource Other}", "RelativeSource={RelativeSource Self}" })
            {
                string explicitSource = markup.Replace(
                    "{CompiledBinding Name, Mode=TwoWay}",
                    "{" + kind + " Name, " + source + "}");
                Assert.ThrowsExactly<InvalidDataException>(() => new AxamlMvvmBindingAnalyzer().Analyze(explicitSource));
            }
            string nestedScope = $$"""
                <Window xmlns="https://github.com/avaloniaui">
                  <DataTemplate><TextBlock Text="{ {{kind}} Name}" /></DataTemplate>
                </Window>
                """;
            Assert.ThrowsExactly<InvalidDataException>(() => new AxamlMvvmBindingAnalyzer().Analyze(nestedScope));
            string complexPath = markup.Replace(
                "{CompiledBinding Name, Mode=TwoWay}",
                "{" + kind + " Customer.Name}");
            Assert.ThrowsExactly<InvalidDataException>(() => new AxamlMvvmBindingAnalyzer().Analyze(complexPath));
        }
    }

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
