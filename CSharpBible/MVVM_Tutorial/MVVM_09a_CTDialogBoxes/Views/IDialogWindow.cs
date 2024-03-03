namespace MVVM_09a_CTDialogBoxes.Views
{
    public interface IDialogWindow
    {
        object DataContext { get; }

        bool? ShowDialog();
    }
}
