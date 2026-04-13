using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AA09_DialogBoxes.Messages;

public enum MsgBoxResult { None, Yes, No }

public sealed class MessageBoxRequestMessage : AsyncRequestMessage<MsgBoxResult>
{
    public string Title { get; }
    public string Content { get; }

    public MessageBoxRequestMessage(string title, string content)
    {
        Title = title;
        Content = content;
    }
}
