namespace MSQBrowser.Views;

public interface IDialogWindow
{
    object DataContext { get; }

    bool? ShowDialog();
}