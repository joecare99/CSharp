namespace Document.Flow.Model;

/// <summary>
/// Represents a run of text.
/// </summary>
public class Run : Inline
{
    /// <summary>
    /// Gets or sets the text content of the run.
    /// </summary>
    public string Text { get; set; }

    public Run(string text = "")
    {
        Text = text;
    }
}
