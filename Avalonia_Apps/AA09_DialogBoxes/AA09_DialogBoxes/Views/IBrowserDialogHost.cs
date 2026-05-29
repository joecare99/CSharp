using System.Threading.Tasks;
using AA09_DialogBoxes.Messages;

namespace AA09_DialogBoxes.Views;

public interface IBrowserDialogHost
{
    Task<MsgBoxResult> ShowMessageAsync(string title, string content);
    Task<(bool, (string, string))> ShowEditDialogAsync(string name, string email);
}
