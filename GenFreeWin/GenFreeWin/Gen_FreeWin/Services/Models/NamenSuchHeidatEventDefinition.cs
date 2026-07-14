namespace GenFreeWin.Services.Models
{
    /// <summary>
    /// Represents a composed Heidat event definition independent from document rendering.
    /// </summary>
    public sealed class NamenSuchHeidatEventDefinition
    {
        /// <summary>
        /// Gets or sets the event text prefix (for example marriage or divorce label).
        /// </summary>
        public string PrefixText { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether a leading newline should be rendered before the prefix.
        /// </summary>
        public bool LeadingNewline { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an indent should be enforced when rendering the prefix.
        /// </summary>
        public bool EnsureIndent { get; set; }

        /// <summary>
        /// Gets or sets the indent value to apply when <see cref="EnsureIndent"/> is true.
        /// </summary>
        public int IndentValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event marks a divorce.
        /// </summary>
        public bool IsDivorceEvent { get; set; }

        /// <summary>
        /// Gets or sets the witness label for this event (for example Trauzeugen or Zeugen).
        /// </summary>
        public string WitnessLabel { get; set; } = string.Empty;
    }
}
