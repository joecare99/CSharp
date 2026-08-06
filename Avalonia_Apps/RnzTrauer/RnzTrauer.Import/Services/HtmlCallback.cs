namespace RnzTrauer.Import.Services;

/// <summary>Callback kinds emitted by the narrow Pascal-compatible HTML tokenizer.</summary>
public enum HtmlCallbackKind
{
    StandardText,
    StartTag,
    TagModifier,
    EndTag,
    Comment,
    Script,
}
