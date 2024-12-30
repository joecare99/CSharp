namespace MVVM_09_DialogBoxes.Views
{
    public interface IDialogWindow
    {
        object DataContext { get; }

        bool? ShowDialog();
    }
}
