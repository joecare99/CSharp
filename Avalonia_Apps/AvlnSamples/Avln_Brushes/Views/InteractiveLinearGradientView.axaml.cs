using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avln_Brushes.ViewModels;

namespace Avln_Brushes.Views;

public partial class InteractiveLinearGradientView : UserControl
{
    public InteractiveLinearGradientView()
    {
        InitializeComponent();
    }

    private void OnColor1Changed(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox &&
    comboBox.SelectedItem is ComboBoxItem item &&
   DataContext is InteractiveLinearGradientViewModel vm)
      {
         vm.GradientStop1Color = GetColorFromTag(item.Tag?.ToString());
        }
    }

    private void OnColor2Changed(object? sender, SelectionChangedEventArgs e)
    {
  if (sender is ComboBox comboBox &&
     comboBox.SelectedItem is ComboBoxItem item &&
   DataContext is InteractiveLinearGradientViewModel vm)
 {
        vm.GradientStop2Color = GetColorFromTag(item.Tag?.ToString());
        }
 }

    private void OnColor3Changed(object? sender, SelectionChangedEventArgs e)
 {
        if (sender is ComboBox comboBox &&
   comboBox.SelectedItem is ComboBoxItem item &&
 DataContext is InteractiveLinearGradientViewModel vm)
        {
      vm.GradientStop3Color = GetColorFromTag(item.Tag?.ToString());
        }
    }

    private Color GetColorFromTag(string? tag)
    {
      return tag switch
        {
       "Blue" => Colors.Blue,
 "Red" => Colors.Red,
    "Yellow" => Colors.Yellow,
         "Purple" => Colors.Purple,
            "LimeGreen" => Colors.LimeGreen,
 "Orange" => Colors.Orange,
     _ => Colors.Black
        };
    }
}
