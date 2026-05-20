using System;
using System.Collections.Generic;

namespace BaseGenClasses.Helper;

/// <summary>
/// Provides the shared callback surface used to transform parser events into genealogy model operations.
/// </summary>
public abstract class GenHelperBase
{
    private THlprMsgEvent? _onHlpMessage;
    private IList<string>? _citation;
    private string _citTitle = string.Empty;
    private string _osbHdr = string.Empty;

    /// <summary>
    /// Gets the current citation collection.
    /// </summary>
    protected IList<string>? FCitation => _citation;

    /// <summary>
    /// Gets or sets the normalized citation reference cache.
    /// </summary>
    protected string FCitRefn { get; set; } = string.Empty;

    /// <summary>
    /// Gets the citation title.
    /// </summary>
    protected string FCitTitle => _citTitle;

    /// <summary>
    /// Gets the current header value.
    /// </summary>
    protected string FOsbHdr => _osbHdr;

    /// <summary>
    /// Gets or sets the current citation values.
    /// </summary>
    public IList<string>? Citation
    {
        get => _citation;
        set
        {
            if (ReferenceEquals(_citation, value))
            {
                return;
            }

            _citation = value;
            FCitRefn = string.Empty;
        }
    }

    /// <summary>
    /// Gets or sets the citation title.
    /// </summary>
    public string CitTitle
    {
        get => _citTitle;
        set
        {
            if (string.Equals(_citTitle, value, StringComparison.Ordinal))
            {
                return;
            }

            _citTitle = value ?? string.Empty;
        }
    }

    /// <summary>
    /// Gets or sets the current helper header.
    /// </summary>
    public string OsbHdr
    {
        get => _osbHdr;
        set
        {
            if (string.Equals(_osbHdr, value, StringComparison.Ordinal))
            {
                return;
            }

            _osbHdr = value ?? string.Empty;
        }
    }

    /// <summary>
    /// Gets or sets the helper message callback.
    /// </summary>
    public THlprMsgEvent? OnHlpMessage
    {
        get => _onHlpMessage;
        set
        {
            if (_onHlpMessage == value)
            {
                return;
            }

            _onHlpMessage = value;
        }
    }

    /// <summary>
    /// Clears the current helper state.
    /// </summary>
    public abstract void Clear();

    /// <summary>
    /// Starts a new family callback sequence.
    /// </summary>
    public abstract void StartFamily(object sender, string text, string reference, int subType);

    /// <summary>
    /// Starts a new individual callback sequence.
    /// </summary>
    public abstract void StartIndiv(object sender, string text, string reference, int subType);

    /// <summary>
    /// Emits a family-individual relation callback.
    /// </summary>
    public abstract void FamilyIndiv(object sender, string text, string reference, int subType);

    /// <summary>
    /// Emits a family type callback.
    /// </summary>
    public abstract void FamilyType(object sender, string text, string reference, int subType);

    /// <summary>
    /// Emits a family date callback.
    /// </summary>
    public abstract void FamilyDate(object sender, string text, string reference, int subType);

    /// <summary>
    /// Emits a family data callback.
    /// </summary>
    public abstract void FamilyData(object sender, string text, string reference, int subType);

    /// <summary>
    /// Emits a family place callback.
    /// </summary>
    public abstract void FamilyPlace(object sender, string text, string reference, int subType);

    /// <summary>
    /// Emits an individual data callback.
    /// </summary>
    public abstract void IndiData(object sender, string text, string reference, int subType);

    /// <summary>
    /// Emits an individual date callback.
    /// </summary>
    public abstract void IndiDate(object sender, string text, string reference, int subType);

    /// <summary>
    /// Emits an individual name callback.
    /// </summary>
    public abstract void IndiName(object sender, string text, string reference, int subType);

    /// <summary>
    /// Emits an individual place callback.
    /// </summary>
    public abstract void IndiPlace(object sender, string text, string reference, int subType);

    /// <summary>
    /// Emits an individual reference callback.
    /// </summary>
    public abstract void IndiRef(object sender, string text, string reference, int subType);

    /// <summary>
    /// Emits an individual occupation callback.
    /// </summary>
    public abstract void IndiOccu(object sender, string text, string reference, int subType);

    /// <summary>
    /// Emits an individual relation callback.
    /// </summary>
    public abstract void IndiRel(object sender, string text, string reference, int subType);

    /// <summary>
    /// Marks the end of the current entry.
    /// </summary>
    public abstract void EndOfEntry(object sender, string text, string reference, int subType);

    /// <summary>
    /// Creates a new output header.
    /// </summary>
    public abstract void CreateNewHeader(string filename);

    /// <summary>
    /// Saves the current result to a file.
    /// </summary>
    public abstract void SaveToFile(string filename);

    /// <summary>
    /// Dispatches a serialized parser event to the matching abstract callback.
    /// </summary>
    /// <param name="sender">The parser sender.</param>
    /// <param name="eventData">The serialized parser event payload.</param>
    public void FireEvent(object sender, string[] eventData)
    {
        if (eventData is null || eventData.Length != 4 || !int.TryParse(eventData[3], out int subType))
        {
            return;
        }

        switch (eventData[0])
        {
            case "ParserStartIndiv":
                StartIndiv(sender, eventData[1], eventData[2], subType);
                break;
            case "ParserStartFamily":
                StartFamily(sender, eventData[1], eventData[2], subType);
                break;
            case "ParserFamilyType":
                FamilyType(sender, eventData[1], eventData[2], subType);
                break;
            case "ParserFamilyDate":
                FamilyDate(sender, eventData[1], eventData[2], subType);
                break;
            case "ParserFamilyData":
                FamilyData(sender, eventData[1], eventData[2], subType);
                break;
            case "ParserFamilyIndiv":
                FamilyIndiv(sender, eventData[1], eventData[2], subType);
                break;
            case "ParserFamilyPlace":
                FamilyPlace(sender, eventData[1], eventData[2], subType);
                break;
            case "ParserIndiData":
                IndiData(sender, eventData[1], eventData[2], subType);
                break;
            case "ParserIndiDate":
                IndiDate(sender, eventData[1], eventData[2], subType);
                break;
            case "ParserIndiName":
                IndiName(sender, eventData[1], eventData[2], subType);
                break;
            case "ParserIndiOccu":
                IndiOccu(sender, eventData[1], eventData[2], subType);
                break;
            case "ParserIndiPlace":
                IndiPlace(sender, eventData[1], eventData[2], subType);
                break;
            case "ParserIndiRef":
                IndiRef(sender, eventData[1], eventData[2], subType);
                break;
            case "ParserIndiRel":
                IndiRel(sender, eventData[1], eventData[2], subType);
                break;
        }
    }

    /// <summary>
    /// Normalizes a citation reference by stripping the leading family or individual marker and left-padding the first numeric block.
    /// </summary>
    /// <param name="text">The raw citation reference.</param>
    /// <returns>The normalized citation reference.</returns>
    protected string NormalCitRef(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        string normalizedText = text.StartsWith("F", StringComparison.Ordinal) || text.StartsWith("I", StringComparison.Ordinal)
            ? text.Substring(1)
            : text;

        int digitIndex = IndexOfAsciiDigit(normalizedText);
        if (digitIndex < 0)
        {
            return normalizedText;
        }

        int endIndex = digitIndex + 1;
        while (endIndex < normalizedText.Length && IsAsciiDigit(normalizedText[endIndex]))
        {
            endIndex++;
        }

        int padCount = 4 - (endIndex - digitIndex);
        return padCount > 0
            ? normalizedText.Insert(digitIndex, new string('0', padCount))
            : normalizedText;
    }

    /// <summary>
    /// Emits a warning message through the configured callback.
    /// </summary>
    protected void Warning(string text, string reference, int mode)
    {
        _onHlpMessage?.Invoke(this, EventType.Warning, text ?? string.Empty, reference ?? string.Empty, mode);
    }

    /// <summary>
    /// Emits an error message through the configured callback.
    /// </summary>
    protected void Error(string text, string reference, int mode)
    {
        _onHlpMessage?.Invoke(this, EventType.Error, text ?? string.Empty, reference ?? string.Empty, mode);
    }

    private static int IndexOfAsciiDigit(string value)
    {
        for (int i = 0; i < value.Length; i++)
        {
            if (IsAsciiDigit(value[i]))
            {
                return i;
            }
        }

        return -1;
    }

    private static bool IsAsciiDigit(char value)
    {
        return value >= '0' && value <= '9';
    }
}
