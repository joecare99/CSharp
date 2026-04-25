using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GenFreeBrowser.Views.Controls;

public partial class PersonSummaryControl : UserControl
{
    public PersonSummaryControl()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (PersonImage.Source == null)
        {
            if (ProfileImageSource is ImageSource src)
            {
                PersonImage.Source = src;
            }
            else if (!string.IsNullOrWhiteSpace(ImagePath) && File.Exists(ImagePath))
            {
                try
                {
                    var bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.UriSource = new Uri(ImagePath, UriKind.Absolute);
                    bmp.EndInit();
                    PersonImage.Source = bmp;
                }
                catch { }
            }
            else if (TryFindResource("FallbackSilhouette") is DrawingImage fallback)
            {
                PersonImage.Source = fallback;
            }
        }
    }

    public static readonly DependencyProperty ParentsHeaderProperty = DependencyProperty.Register(
        nameof(ParentsHeader), typeof(string), typeof(PersonSummaryControl), new PropertyMetadata(string.Empty));

    public string ParentsHeader
    {
        get => (string)GetValue(ParentsHeaderProperty);
        set => SetValue(ParentsHeaderProperty, value);
    }

    public static readonly DependencyProperty DisplayNameProperty = DependencyProperty.Register(
        nameof(DisplayName), typeof(string), typeof(PersonSummaryControl), new PropertyMetadata("Unbekannt"));

    public string DisplayName
    {
        get => (string)GetValue(DisplayNameProperty);
        set => SetValue(DisplayNameProperty, value);
    }

    public static readonly DependencyProperty VitalDataProperty = DependencyProperty.Register(
        nameof(VitalData), typeof(string), typeof(PersonSummaryControl), new PropertyMetadata(string.Empty));

    public string VitalData
    {
        get => (string)GetValue(VitalDataProperty);
        set => SetValue(VitalDataProperty, value);
    }

    public static readonly DependencyProperty AdditionalInfoProperty = DependencyProperty.Register(
        nameof(AdditionalInfo), typeof(string), typeof(PersonSummaryControl), new PropertyMetadata(string.Empty));

    public string AdditionalInfo
    {
        get => (string)GetValue(AdditionalInfoProperty);
        set => SetValue(AdditionalInfoProperty, value);
    }

    public static readonly DependencyProperty ImagePathProperty = DependencyProperty.Register(
        nameof(ImagePath), typeof(string), typeof(PersonSummaryControl), new PropertyMetadata(null));

    public string? ImagePath
    {
        get => (string?)GetValue(ImagePathProperty);
        set => SetValue(ImagePathProperty, value);
    }

    public static readonly DependencyProperty ProfileImageSourceProperty = DependencyProperty.Register(
        nameof(ProfileImageSource), typeof(ImageSource), typeof(PersonSummaryControl), new PropertyMetadata(null));

    public ImageSource? ProfileImageSource
    {
        get => (ImageSource?)GetValue(ProfileImageSourceProperty);
        set => SetValue(ProfileImageSourceProperty, value);
    }
}
