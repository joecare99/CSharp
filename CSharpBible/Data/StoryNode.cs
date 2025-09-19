public class StoryNode
{
    public string Id { get; }
    public string Title { get; set; } // <--- set hinzugef�gt
    public string[] Paragraphs { get; set; } // <--- set hinzugef�gt
    public List<Choice> Choices { get; set; } // <--- set hinzugef�gt

    public StoryNode(string id, string title, string[] paragraphs, List<Choice> choices)
    {
        Id = id;
        Title = title;
        Paragraphs = paragraphs;
        Choices = choices;
    }
}