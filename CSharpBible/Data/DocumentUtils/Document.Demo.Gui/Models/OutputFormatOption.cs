namespace Document.Demo.Gui.Models;

public sealed class OutputFormatOption
{
    public OutputFormatOption(string key, string displayName, string extension)
    {
        Key = key;
        DisplayName = displayName;
        Extension = extension;
    }

    public string Key { get; }

    public string DisplayName { get; }

    public string Extension { get; }
}
