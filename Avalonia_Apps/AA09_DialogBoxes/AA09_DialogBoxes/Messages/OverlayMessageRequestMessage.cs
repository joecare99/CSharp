using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AA09_DialogBoxes.Messages;

public sealed class OverlayMessageRequestMessage : AsyncRequestMessage<MsgBoxResult>
{
    public string Title { get; }
    public string Content { get; }

    public OverlayMessageRequestMessage(string title, string content)
    {
        Title = title;
        Content = content;
    }
}
