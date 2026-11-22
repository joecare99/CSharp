using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.AxamlDesigner.UI.Controls;
using AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using Avalonia.Layout;
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
    public void TextBlockMaxLines_UsesNativeDefaultAndProtectsSources()
    {
        AxamlPreviewService service = new();
        int nativeDefault = new TextBlock().MaxLines;
        foreach ((string markup, int expected, bool readOnly, bool ambiguous, bool invalid) in new[]
        {
            ("<TextBlock />", nativeDefault, false, false, false),
            ("<TextBlock MaxLines='0' />", 0, false, false, false),
            ("<TextBlock MaxLines='003' />", 3, false, false, false),
            ("<TextBlock MaxLines='2147483647' />", int.MaxValue, false, false, false),
            ("<TextBlock MaxLines='-1' />", nativeDefault, false, false, true),
            ("<TextBlock MaxLines='1.5' />", nativeDefault, false, false, true),
            ("<TextBlock MaxLines='2147483648' />", nativeDefault, false, false, true),
            ("<TextBlock MaxLines='many' />", nativeDefault, false, false, true),
            ("<TextBlock MaxLines='{Binding Count}' />", nativeDefault, true, false, false),
            ("<TextBlock MaxLines='{DynamicResource Count}' />", nativeDefault, true, false, false),
            ("<TextBlock MaxLines='{Binding Broken' />", nativeDefault, true, false, false),
            ("<TextBlock xmlns:a='https://github.com/avaloniaui' a:MaxLines='3' />", nativeDefault, true, true, false),
            ("<TextBlock xmlns:e='urn:foreign' e:MaxLines='3' />", nativeDefault, true, true, false),
            ("<TextBlock xmlns:e='urn:foreign' MaxLines='3' e:MaxLines='2' />", nativeDefault, true, true, false),
            ("<TextBlock MaxLines='3' TextBlock.MaxLines='2' />", nativeDefault, true, true, false),
            ("<TextBlock><TextBlock.MaxLines>3</TextBlock.MaxLines></TextBlock>", nativeDefault, true, true, false),
            ("<TextBlock MaxLines='3'><TextBlock.MaxLines>2</TextBlock.MaxLines></TextBlock>", nativeDefault, true, true, false),
            ("<TextBlock xmlns:e='urn:foreign' MaxLines='3'><e:Other.MaxLines>2</e:Other.MaxLines></TextBlock>", nativeDefault, true, true, false),
        })
        {
            AxamlPreviewNode node = service.Parse(markup);
            AxamlPreviewControl preview = new() { PreviewNode = node };
            Grid wrapper = (Grid)preview.GetLogicalDescendants().OfType<Border>()
                .Single(border => ReferenceEquals(border.Tag, node)).Child!;
            Assert.AreEqual(expected, ((TextBlock)wrapper.Children[0]).MaxLines, markup);
            AxamlPropertyDescriptor descriptor = AxamlPropertyValueValidator.Describe(node, "MaxLines");
            Assert.AreEqual(readOnly, descriptor.IsReadOnly, markup);
            Assert.AreEqual(invalid, descriptor.ValidationError is not null, markup);
            if (markup == "<TextBlock />")
            {
                Assert.AreEqual(nativeDefault.ToString(CultureInfo.InvariantCulture), descriptor.Value);
            }
            if (ambiguous)
            {
                Assert.AreEqual(string.Empty, descriptor.Value, markup);
                Assert.IsFalse(node.RenderProperties.ContainsKey("MaxLines"), markup);
            }
            if (readOnly)
            {
                Assert.ThrowsExactly<InvalidOperationException>(
                    () => service.UpdateAttribute(markup, node, "MaxLines", "2"), markup);
            }
        }
    }

    [TestMethod]
    public void TextBlockMaxLines_RecoveryAndStaleSourcesRemainSafe()
    {
        const string broken = "<StackPanel xmlns:e='urn:foreign'><TextBlock Text='recovery target' MaxLines='3' e:MaxLines='2' /><Button Content=broken /></StackPanel>";
        AxamlPreviewService service = new();
        AxamlPreviewNode root = service.ParseForPreview(broken).Root!;
        Assert.IsTrue(root.IsRecoveryRoot);
        AxamlPreviewNode node = root.Children.Single(child => child.TypeName == "TextBlock");
        AxamlPreviewControl preview = new() { PreviewNode = root };
        TextBlock rendered = preview.GetLogicalDescendants().OfType<TextBlock>()
            .Single(text => text.Text == "recovery target");
        Assert.AreEqual(new TextBlock().MaxLines, rendered.MaxLines);
        Assert.IsFalse(service.CanEditProperty(node, "MaxLines"));

        const string original = "<TextBlock MaxLines='1'></TextBlock>";
        AxamlPreviewNode stale = service.Parse(original);
        Assert.ThrowsExactly<InvalidDataException>(() => service.UpdateAttribute(
            "<TextBlock MaxLines='1'><TextBlock.MaxLines>2</TextBlock.MaxLines></TextBlock>",
            stale, "MaxLines", "3"));
    }

    [TestMethod]
    public void TextBlockTrimming_UsesNativeDefaultAndRejectsAmbiguousSources()
    {
        AxamlPreviewService service = new();
        TextTrimming nativeDefault = new TextBlock().TextTrimming;
        Assert.AreSame(TextTrimming.None, nativeDefault);
        foreach ((string markup, TextTrimming expected, bool readOnly, bool ambiguous, bool invalid) in new[]
        {
            ("<TextBlock />", nativeDefault, false, false, false),
            ("<TextBlock TextTrimming='None' />", TextTrimming.None, false, false, false),
            ("<TextBlock TextTrimming='characterellipsis' />", TextTrimming.CharacterEllipsis, false, false, false),
            ("<TextBlock TextTrimming='WordEllipsis' />", TextTrimming.WordEllipsis, false, false, false),
            ("<TextBlock TextTrimming='0' />", nativeDefault, false, false, true),
            ("<TextBlock TextTrimming='1' />", nativeDefault, false, false, true),
            ("<TextBlock TextTrimming='-1' />", nativeDefault, false, false, true),
            ("<TextBlock TextTrimming='99' />", nativeDefault, false, false, true),
            ("<TextBlock TextTrimming='Unknown' />", nativeDefault, false, false, true),
            ("<TextBlock TextTrimming='{Binding Trimming}' />", nativeDefault, true, false, false),
            ("<TextBlock TextTrimming='{DynamicResource Trimming}' />", nativeDefault, true, false, false),
            ("<TextBlock TextTrimming='{Binding Broken' />", nativeDefault, true, false, false),
            ("<TextBlock xmlns:a='https://github.com/avaloniaui' a:TextTrimming='WordEllipsis' />", nativeDefault, true, true, false),
            ("<TextBlock xmlns:e='urn:foreign' e:TextTrimming='{Binding Trimming}' />", nativeDefault, true, true, false),
            ("<TextBlock xmlns:e='urn:foreign' TextTrimming='WordEllipsis' e:TextTrimming='None' />", nativeDefault, true, true, false),
            ("<TextBlock TextTrimming='WordEllipsis' TextBlock.TextTrimming='None' />", nativeDefault, true, true, false),
            ("<TextBlock><TextBlock.TextTrimming>WordEllipsis</TextBlock.TextTrimming></TextBlock>", nativeDefault, true, true, false),
            ("<TextBlock TextTrimming='WordEllipsis'><TextBlock.TextTrimming>None</TextBlock.TextTrimming></TextBlock>", nativeDefault, true, true, false),
            ("<TextBlock xmlns:e='urn:foreign' TextTrimming='WordEllipsis'><e:Other.TextTrimming>None</e:Other.TextTrimming></TextBlock>", nativeDefault, true, true, false),
        })
        {
            AxamlPreviewNode node = service.Parse(markup);
            AxamlPreviewControl preview = new() { PreviewNode = node };
            Grid wrapper = (Grid)preview.GetLogicalDescendants().OfType<Border>()
                .Single(border => ReferenceEquals(border.Tag, node)).Child!;
            Assert.AreSame(expected, ((TextBlock)wrapper.Children[0]).TextTrimming, markup);
            AxamlPropertyDescriptor descriptor = AxamlPropertyValueValidator.Describe(node, "TextTrimming");
            Assert.AreEqual(readOnly, descriptor.IsReadOnly, markup);
            Assert.AreEqual(invalid, descriptor.ValidationError is not null, markup);
            if (markup == "<TextBlock />")
            {
                Assert.AreEqual("None", descriptor.Value);
            }
            if (ambiguous)
            {
                Assert.AreEqual(string.Empty, descriptor.Value, markup);
                Assert.IsFalse(node.RenderProperties.ContainsKey("TextTrimming"), markup);
            }
            if (readOnly)
            {
                Assert.ThrowsExactly<InvalidOperationException>(
                    () => service.UpdateAttribute(markup, node, "TextTrimming", "None"), markup);
            }
        }
    }

    [TestMethod]
    public void TextBlockTrimming_RecoveryAndStalePropertyElementRemainSafe()
    {
        const string broken = "<StackPanel xmlns:e='urn:foreign'><TextBlock Text='recovery target' TextTrimming='WordEllipsis' e:TextTrimming='None' /><Button Content=broken /></StackPanel>";
        AxamlPreviewService service = new();
        AxamlPreviewNode root = service.ParseForPreview(broken).Root!;
        Assert.IsTrue(root.IsRecoveryRoot);
        AxamlPreviewNode node = root.Children.Single(child => child.TypeName == "TextBlock");
        Assert.IsTrue(node.HasUnsafeTextTrimmingSource);
        AxamlPreviewControl preview = new() { PreviewNode = root };
        TextBlock rendered = preview.GetLogicalDescendants().OfType<TextBlock>()
            .Single(text => text.Text == "recovery target");
        Assert.AreSame(new TextBlock().TextTrimming, rendered.TextTrimming);
        Assert.IsFalse(service.CanEditProperty(node, "TextTrimming"));

        const string original = "<TextBlock TextTrimming='None'></TextBlock>";
        AxamlPreviewNode stale = service.Parse(original);
        Assert.ThrowsExactly<InvalidDataException>(() => service.UpdateAttribute(
            "<TextBlock TextTrimming='None'><TextBlock.TextTrimming>WordEllipsis</TextBlock.TextTrimming></TextBlock>",
            stale, "TextTrimming", "CharacterEllipsis"));
        Assert.ThrowsExactly<InvalidDataException>(() => service.UpdateAttribute(
            "<TextBlock TextTrimming='{Binding Trimming}'></TextBlock>",
            stale, "TextTrimming", "CharacterEllipsis"));
    }

    [TestMethod]
    public void OrientationSourceFlags_PreserveWrapPanelCompatibilityIncludingRecoveryAndInitializationOrder()
    {
        AxamlPreviewService service = new();
        foreach (string type in new[] { "StackPanel", "Slider", "ProgressBar", "WrapPanel" })
        {
            bool isWrapPanel = type == "WrapPanel";
            string child = $"<{type} Orientation='Vertical' e:Orientation='Horizontal' />";
            AxamlPreviewNode normal = service.Parse($"<{type} xmlns:e='urn:foreign' Orientation='Vertical' e:Orientation='Horizontal' />");
            AxamlPreviewNode root = service.ParseForPreview(
                $"<StackPanel xmlns:e='urn:foreign'>{child}<Button Content=broken /></StackPanel>").Root!;
            Assert.IsTrue(root.IsRecoveryRoot, type);
            AxamlPreviewNode recovery = root.Children.Single(node => node.TypeName == type);
            foreach (AxamlPreviewNode node in new[] { normal, recovery })
            {
                Assert.IsTrue(node.HasUnsafeOrientationSource, type);
                Assert.AreEqual(isWrapPanel, node.HasUnsafeWrapPanelOrientationSource, type);
            }

            AxamlPreviewNode safe = service.Parse($"<{type} Orientation='Vertical' />");
            AxamlPreviewNode safeRoot = service.ParseForPreview(
                $"<StackPanel><{type} Orientation='Vertical' /><Button Content=broken /></StackPanel>").Root!;
            Assert.IsTrue(safeRoot.IsRecoveryRoot, type);
            AxamlPreviewNode safeRecovery = safeRoot.Children.Single(node => node.TypeName == type);
            foreach (AxamlPreviewNode node in new[] { safe, safeRecovery })
            {
                Assert.IsFalse(node.HasUnsafeOrientationSource, type);
                Assert.IsFalse(node.HasUnsafeWrapPanelOrientationSource, type);
            }

            foreach (AxamlPreviewNode node in new[]
            {
                new AxamlPreviewNode { TypeName = type, HasUnsafeOrientationSource = true },
                new AxamlPreviewNode { HasUnsafeOrientationSource = true, TypeName = type },
                new AxamlPreviewNode { TypeName = type, HasUnsafeWrapPanelOrientationSource = true },
                new AxamlPreviewNode { HasUnsafeWrapPanelOrientationSource = true, TypeName = type },
            })
            {
                Assert.IsTrue(node.HasUnsafeOrientationSource, type);
                Assert.AreEqual(isWrapPanel, node.HasUnsafeWrapPanelOrientationSource, type);
            }
        }
    }

    [TestMethod]
    public void PanelAndRangeOrientation_UsesNativeDefaultForInvalidAndAmbiguousSources()
    {
        AxamlPreviewService service = new();
        foreach ((string type, Orientation nativeDefault) in new[]
        {
            ("StackPanel", new StackPanel().Orientation),
            ("Slider", new Slider().Orientation),
            ("ProgressBar", new ProgressBar().Orientation),
        })
        {
            Assert.AreEqual(type == "StackPanel" ? Orientation.Vertical : Orientation.Horizontal, nativeDefault);
            foreach ((string attributes, string content, Orientation expected, bool readOnly) in new[]
            {
                ("", "", nativeDefault, false),
                ("Orientation='Horizontal'", "", Orientation.Horizontal, false),
                ("Orientation='vertical'", "", Orientation.Vertical, false),
                ("Orientation='{Binding Direction}'", "", nativeDefault, true),
                ("xmlns:a='https://github.com/avaloniaui' a:Orientation='Horizontal'", "", nativeDefault, true),
                ("xmlns:e='urn:foreign' Orientation='Horizontal' e:Orientation='Vertical'", "", nativeDefault, true),
                ("Orientation='Horizontal' Other.Orientation='Vertical'", "", nativeDefault, true),
                ("Orientation='Horizontal'", "<Other.Orientation>Vertical</Other.Orientation>", nativeDefault, true),
                ("", $"<{type}.Orientation>Horizontal</{type}.Orientation>", nativeDefault, true),
                ("Orientation='Vertical'", $"<{type}.Orientation>Horizontal</{type}.Orientation>", nativeDefault, true),
                ("Orientation='1'", "", nativeDefault, false),
                ("Orientation='0'", "", nativeDefault, false),
                ("Orientation='99'", "", nativeDefault, false),
                ("Orientation='Diagonal'", "", nativeDefault, false),
            })
            {
                string markup = $"<{type} {attributes}>{content}</{type}>";
                AxamlPreviewNode node = service.Parse(markup);
                AxamlPreviewControl preview = new() { PreviewNode = node };
                Grid wrapper = (Grid)preview.GetLogicalDescendants().OfType<Border>()
                    .Single(border => ReferenceEquals(border.Tag, node)).Child!;
                Orientation actual = wrapper.Children[0] switch
                {
                    StackPanel stack => stack.Orientation,
                    Slider slider => slider.Orientation,
                    ProgressBar progress => progress.Orientation,
                    _ => throw new InvalidOperationException(type),
                };
                Assert.AreEqual(expected, actual, markup);
                AxamlPropertyDescriptor descriptor = AxamlPropertyValueValidator.Describe(node, "Orientation");
                Assert.AreEqual(readOnly, descriptor.IsReadOnly, markup);
                if (attributes.Length == 0 && content.Length == 0)
                {
                    Assert.AreEqual(nativeDefault.ToString(), descriptor.Value, markup);
                }
                if (node.HasUnsafeOrientationSource)
                {
                    Assert.AreEqual(string.Empty, descriptor.Value, markup);
                    Assert.IsFalse(node.RenderProperties.ContainsKey("Orientation"), markup);
                    Assert.ThrowsExactly<InvalidOperationException>(
                        () => service.UpdateAttribute(markup, node, "Orientation", "Vertical"), markup);
                }
                if (attributes.Contains("'1'", StringComparison.Ordinal) || attributes.Contains("'0'", StringComparison.Ordinal)
                    || attributes.Contains("'99'", StringComparison.Ordinal)
                    || attributes.Contains("'Diagonal'", StringComparison.Ordinal))
                {
                    Assert.IsNotNull(descriptor.ValidationError, markup);
                }
            }
        }
    }

    [TestMethod]
    public void PanelAndRangeOrientation_RecoveryPreservesAmbiguityProtection()
    {
        foreach (string type in new[] { "StackPanel", "Slider", "ProgressBar" })
        {
            string markup = $"<StackPanel xmlns:e='urn:foreign'><{type} Orientation='Horizontal' e:Orientation='Vertical' />"
                + "<Button Content=broken /></StackPanel>";
            AxamlPreviewNode root = new AxamlPreviewService().ParseForPreview(markup).Root!;
            Assert.IsTrue(root.IsRecoveryRoot, markup);
            AxamlPreviewNode node = root.Children.Single(child => child.TypeName == type);
            Assert.IsTrue(node.HasUnsafeOrientationSource, markup);
            Assert.IsFalse(new AxamlPreviewService().CanEditProperty(node, "Orientation"), markup);
            AxamlPreviewControl preview = new() { PreviewNode = node };
            Orientation actual = type switch
            {
                "StackPanel" => preview.GetLogicalDescendants().OfType<StackPanel>().Single().Orientation,
                "Slider" => preview.GetLogicalDescendants().OfType<Slider>().Single().Orientation,
                "ProgressBar" => preview.GetLogicalDescendants().OfType<ProgressBar>().Single().Orientation,
                _ => throw new InvalidOperationException(type),
            };
            Assert.AreEqual(type == "StackPanel" ? Orientation.Vertical : Orientation.Horizontal, actual, markup);
        }
    }

    [TestMethod]
    public void WrapPanelOrientation_AmbiguousSourcesFallBackInInertPreview()
    {
        AxamlPreviewService service = new();
        foreach (string markup in new[]
        {
            "<WrapPanel xmlns:e='urn:foreign' Orientation='Vertical' e:Orientation='Horizontal' />",
            "<WrapPanel xmlns:a='https://github.com/avaloniaui' a:Orientation='Vertical' />",
            "<WrapPanel xmlns:e='urn:foreign' e:Orientation='{Binding Direction}' />",
            "<WrapPanel Orientation='Vertical' WrapPanel.Orientation='Horizontal' />",
            "<WrapPanel Orientation='Vertical'><WrapPanel.Orientation>Horizontal</WrapPanel.Orientation></WrapPanel>",
            "<WrapPanel Orientation='Vertical'><Other.Orientation>Horizontal</Other.Orientation></WrapPanel>",
            "<WrapPanel Orientation='{Binding Direction}' />",
            "<WrapPanel Orientation='{Binding Broken' />",
        })
        {
            AxamlPreviewNode node = service.Parse(markup);
            AxamlPreviewControl preview = new() { PreviewNode = node };
            Grid wrapper = (Grid)preview.GetLogicalDescendants().OfType<Border>()
                .Single(border => ReferenceEquals(border.Tag, node)).Child!;
            Assert.AreEqual(Orientation.Horizontal, ((WrapPanel)wrapper.Children[0]).Orientation, markup);
            Assert.IsFalse(service.CanEditProperty(node, "Orientation"), markup);
            AxamlPropertyDescriptor descriptor = AxamlPropertyValueValidator.Describe(node, "Orientation");
            Assert.IsTrue(descriptor.IsReadOnly, markup);
            if (node.HasUnsafeWrapPanelOrientationSource)
            {
                Assert.AreEqual(string.Empty, descriptor.Value, markup);
                Assert.IsFalse(node.RenderProperties.ContainsKey("Orientation"), markup);
            }
            Assert.ThrowsExactly<InvalidOperationException>(
                () => service.UpdateAttribute(markup, node, "Orientation", "Horizontal"), markup);
        }
    }

    [TestMethod]
    public void WrapPanelOrientation_RecoveryPreviewRetainsAmbiguityProtection()
    {
        const string markup = "<StackPanel xmlns='https://github.com/avaloniaui' xmlns:e='urn:foreign'>"
            + "<WrapPanel Orientation='Vertical' e:Orientation='Horizontal' /><Button Content=broken /></StackPanel>";
        AxamlPreviewNode root = new AxamlPreviewService().ParseForPreview(markup).Root!;
        Assert.IsTrue(root.IsRecoveryRoot);
        AxamlPreviewNode node = root.Children.Single(child => child.TypeName == "WrapPanel");
        Assert.IsTrue(node.HasUnsafeWrapPanelOrientationSource);
        AxamlPreviewControl preview = new() { PreviewNode = root };
        Assert.AreEqual(Orientation.Horizontal, preview.GetLogicalDescendants().OfType<WrapPanel>().Single().Orientation);
    }

    [TestMethod]
    public void WrapPanelOrientation_InspectorAndPreviewUseNativeDefaultAndRejectInvalidLiterals()
    {
        AxamlPreviewService service = new();
        Orientation nativeDefault = new WrapPanel().Orientation;
        Assert.AreEqual(Orientation.Horizontal, nativeDefault);
        foreach ((string markup, Orientation expected) in new[]
        {
            ("<WrapPanel />", nativeDefault),
            ("<WrapPanel Orientation='Vertical' />", Orientation.Vertical),
            ("<WrapPanel Orientation='1' />", nativeDefault),
            ("<WrapPanel Orientation='99' />", nativeDefault),
            ("<WrapPanel Orientation='Diagonal' />", nativeDefault),
        })
        {
            AxamlPreviewNode node = service.Parse(markup);
            AxamlPropertyDescriptor descriptor = AxamlPropertyValueValidator.Describe(node, "Orientation");
            if (!node.Properties.ContainsKey("Orientation"))
            {
                Assert.AreEqual(nativeDefault.ToString(), descriptor.Value);
            }
            else if (node.Properties["Orientation"] is "1" or "99" or "Diagonal")
            {
                Assert.IsNotNull(descriptor.ValidationError, markup);
            }

            AxamlPreviewControl preview = new() { PreviewNode = node };
            Grid wrapper = (Grid)preview.GetLogicalDescendants().OfType<Border>()
                .Single(border => ReferenceEquals(border.Tag, node)).Child!;
            Assert.AreEqual(expected, ((WrapPanel)wrapper.Children[0]).Orientation, markup);
        }
    }

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
