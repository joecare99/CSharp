namespace MVVM_09a_CTDialogBoxes.View
{
    public interface IDialogWindow
    {
        object DataContext { get; }

        bool? ShowDialog();
    }
}
