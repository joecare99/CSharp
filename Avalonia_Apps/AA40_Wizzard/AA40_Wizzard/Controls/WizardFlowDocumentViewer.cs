using System;
using Avalonia;
using Avalonia.Controls;
using Document.Axaml;

namespace AA40_Wizzard.Controls;

/// <summary>
/// Displays FlowDocument XAML through a converted Avalonia-native preview control.
/// </summary>
public sealed class WizardFlowDocumentViewer : UserControl
{
    private static readonly FlowDocumentAxamlConverter Converter = new();

    /// <summary>
    /// Defines the document XAML property.
    /// </summary>
    public static readonly StyledProperty<string?> DocumentXamlProperty =
        AvaloniaProperty.Register<WizardFlowDocumentViewer, string?>(nameof(DocumentXaml));

    /// <summary>
    /// Initializes a new instance of the <see cref="WizardFlowDocumentViewer"/> class.
    /// </summary>
    public WizardFlowDocumentViewer()
    {
    }

    /// <summary>
    /// Gets or sets the FlowDocument XAML to display.
    /// </summary>
    public string? DocumentXaml
    {
        get => GetValue(DocumentXamlProperty);
        set => SetValue(DocumentXamlProperty, value);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == DocumentXamlProperty)
        {
            ApplyDocument(change.GetNewValue<string?>());
        }
    }

    private void ApplyDocument(string? documentXaml)
    {
        if (string.IsNullOrWhiteSpace(documentXaml))
        {
            Content = null;
            return;
        }

        try
        {
            Content = Converter.CreatePreviewControl(documentXaml);
        }
        catch (Exception)
        {
            Content = new TextBlock
            {
                Text = Converter.ExtractPlainText(documentXaml),
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            };
        }
    }
}
