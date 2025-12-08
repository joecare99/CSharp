using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AA09_DialogBoxes.Messages;

public sealed class EditDialogRequestMessage : AsyncRequestMessage<(bool,(string,string))>
{
    public string Name { get; }
    public string Email { get; }

    public EditDialogRequestMessage(string name, string email)
    {
        Name = name;
        Email = email;
    }
}
