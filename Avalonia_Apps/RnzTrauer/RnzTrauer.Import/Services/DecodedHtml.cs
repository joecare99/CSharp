namespace RnzTrauer.Import.Services;

/// <summary>Decoded source text together with the encoding selected for the input bytes.</summary>
public sealed record DecodedHtml(string Text, string EncodingName);
