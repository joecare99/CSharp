using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GenFreeBrowser.Views.Controls;

public partial class PersonCardControl : UserControl
{
    public PersonCardControl()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty DisplayNameProperty = DependencyProperty.Register(
        nameof(DisplayName), typeof(string), typeof(PersonCardControl), new PropertyMetadata(null));

    public static readonly DependencyProperty SubtitleProperty = DependencyProperty.Register(
        nameof(Subtitle), typeof(string), typeof(PersonCardControl), new PropertyMetadata(null));

    public static readonly DependencyProperty PhotoProperty = DependencyProperty.Register(
        nameof(Photo), typeof(ImageSource), typeof(PersonCardControl), new PropertyMetadata(null));

    public string DisplayName
    {
        get => (string)GetValue(DisplayNameProperty);
        set => SetValue(DisplayNameProperty, value);
    }

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public ImageSource Photo
    {
        get => (ImageSource)GetValue(PhotoProperty);
        set => SetValue(PhotoProperty, value);
    }
}
