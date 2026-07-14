namespace Gen_FreeWin.Services.Models
{
    /// <summary>
    /// Represents a composed heading structure for NamenSuch document output.
    /// </summary>
    public sealed class NamenSuchHeadingDefinition
    {
        /// <summary>
        /// Gets or sets the heading text to render.
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the number of leading newline characters to emit before heading text.
        /// </summary>
        public int LeadingNewlineCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether trailing document whitespace should be trimmed before rendering.
        /// </summary>
        public bool TrimDocumentEndFirst { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether legacy context value <c>UbgT1</c> should be reset after heading rendering.
        /// </summary>
        public bool ResetContextUbgT1 { get; set; }
    }
}
