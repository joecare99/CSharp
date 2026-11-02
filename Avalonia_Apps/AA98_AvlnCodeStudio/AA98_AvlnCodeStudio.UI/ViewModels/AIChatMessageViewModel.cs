namespace AA98_AvlnCodeStudio.UI.ViewModels;

public sealed class AIChatMessageViewModel(string role, string content)
{
    public string Role { get; } = role;

    public string Content { get; } = content;
}
