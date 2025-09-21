using System.Collections.Generic;

namespace Story_Base.Data;

public sealed class Story
{
    public string Title { get; set; } = string.Empty;
    public Dictionary<string, string> Variables { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public List<StoryNode> Nodes { get; set; } = new();
}