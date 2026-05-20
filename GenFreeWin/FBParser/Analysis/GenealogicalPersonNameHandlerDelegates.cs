namespace FBParser.Analysis;

/// <summary>
/// Represents the callback used to guess a sex marker from a given-name fragment.
/// </summary>
/// <param name="name">The name fragment to inspect.</param>
/// <param name="learn">A value indicating whether unknown names may be learned.</param>
/// <returns>The resolved sex marker.</returns>
internal delegate char GuessSexOfGivenNameDelegate(string name, bool learn = true);

/// <summary>
/// Represents the callback used to learn a sex marker for a given-name fragment.
/// </summary>
/// <param name="name">The name fragment to learn.</param>
/// <param name="sex">The sex marker.</param>
internal delegate void LearnSexOfGivenNameDelegate(string name, char sex);
