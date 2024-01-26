namespace DialogBoxes.View
{
    public interface IDialogWindow
    {
        object DataContext { get; }

        bool? ShowDialog();
    }
}
