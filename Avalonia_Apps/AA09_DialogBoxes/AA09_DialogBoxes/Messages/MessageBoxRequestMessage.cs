using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging.Messages;
using AA09_DialogBoxes.ViewModels;
using MsBox.Avalonia.Enums;

namespace AA09_DialogBoxes.Messages;

public sealed class MessageBoxRequestMessage : AsyncRequestMessage<ButtonResult>
{
    public string Title { get; }
    public string Content { get; }
    public MessageBoxRequestMessage(string title, string content)
    {
        Title = title;
        Content = content;
    }
}
