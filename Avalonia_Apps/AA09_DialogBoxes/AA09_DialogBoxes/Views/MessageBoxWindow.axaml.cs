using Avalonia.Controls;
using Avalonia.Input;
using AA09_DialogBoxes.Messages;

namespace AA09_DialogBoxes.Views;

public partial class MessageBoxWindow : Window
{
    public MessageBoxWindow(string title, string content)
    {
        InitializeComponent();
        Title = title;
        MessageText.Text = content;
        this.Opened += (_, _) => YesButton.Focus();
        this.KeyDown += MessageBoxWindow_KeyDown;
    }

    private void MessageBoxWindow_KeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.J:
            case Key.Enter:
                Close(MsgBoxResult.Yes);
                e.Handled = true;
                break;
            case Key.N:
            case Key.Escape:
                Close(MsgBoxResult.No);
                e.Handled = true;
                break;
        }
    }

    private void OnYesClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close(MsgBoxResult.Yes);
    }

    private void OnNoClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close(MsgBoxResult.No);
    }
}
