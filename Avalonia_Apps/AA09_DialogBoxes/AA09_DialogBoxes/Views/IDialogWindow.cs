using Avalonia.Controls;
using System.Threading.Tasks;

namespace AA09_DialogBoxes.Views;

public interface IDialogWindow
{
    object DataContext { get; }

    Task<(bool,(string,string))> ShowDialog(Window o);
}
