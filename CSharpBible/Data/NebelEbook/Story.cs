using System.Collections.Generic;

namespace NebelEbook;

public sealed class Story
{
    public Dictionary<string, string> Variables { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public List<StoryNode> Nodes { get; set; } = new();
}