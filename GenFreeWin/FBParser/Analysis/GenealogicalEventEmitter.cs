using static FBParser.PascalCompat;

namespace FBParser.Analysis;

/// <summary>
/// Emits validated genealogical parser callbacks through configured delegates.
/// </summary>
internal sealed class GenealogicalEventEmitter : IGenealogicalEventEmitter
{
    private readonly GenealogicalEventEmitterConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenealogicalEventEmitter"/> class.
    /// </summary>
    /// <param name="configuration">The immutable configuration used for validation and event sinks.</param>
    public GenealogicalEventEmitter(GenealogicalEventEmitterConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc />
    public void SetIndiName(string individualId, int nameType, string name)
        => _configuration.OnIndiName(name, individualId, nameType);

    /// <inheritdoc />
    public void SetIndiData(string individualId, ParserEventType eventType, string data)
        => _configuration.OnIndiData(data, individualId, (int)eventType);

    /// <inheritdoc />
    public void SetIndiDate(string individualId, ParserEventType eventType, string date)
    {
        if (!_configuration.IsValidDate(date))
        {
            _configuration.Error(QuotedString(date) + " is no valid Date");
            return;
        }

        _configuration.OnIndiDate(date, individualId, (int)eventType);
    }

    /// <inheritdoc />
    public void SetIndiPlace(string individualId, ParserEventType eventType, string place)
    {
        if (!_configuration.IsValidPlace(place))
        {
            _configuration.Error(QuotedString(place) + " is no valid Place");
        }

        _configuration.OnIndiPlace(place, individualId, (int)eventType);
    }

    /// <inheritdoc />
    public void SetIndiOccu(string individualId, ParserEventType eventType, string occu)
        => _configuration.OnIndiOccu(occu, individualId, (int)eventType);

    /// <inheritdoc />
    public void SetIndiRelat(string individualId, string famRef, int relType, string mainRef)
    {
        if (famRef == "0" && mainRef != "0")
        {
            _configuration.Error(QuotedString(famRef) + " is no valid Ref");
        }

        _configuration.OnIndiRel(famRef, individualId, relType);
    }

    /// <inheritdoc />
    public void SetIndiRef(string individualId, ParserEventType eventType, string reference)
        => _configuration.OnIndiRef(reference, individualId, (int)eventType);

    /// <inheritdoc />
    public void StartFamily(string famRef)
        => _configuration.OnStartFamily(famRef);

    /// <inheritdoc />
    public void SetFamilyType(string famRef, int famType, string data = "")
        => _configuration.OnFamilyType(data, famRef, famType);

    /// <inheritdoc />
    public void SetFamilyDate(string famRef, ParserEventType eventType, string date)
    {
        if (!_configuration.IsValidDate(date))
        {
            _configuration.Error(QuotedString(date) + " is no valid Date");
            return;
        }

        _configuration.OnFamilyDate(date, famRef, (int)eventType);
    }

    /// <inheritdoc />
    public void SetFamilyPlace(string famRef, ParserEventType eventType, string place)
    {
        if (!_configuration.IsValidPlace(place))
        {
            _configuration.Error(QuotedString(place) + " is no valid Place");
        }

        _configuration.OnFamilyPlace(place, famRef, (int)eventType);
    }

    /// <inheritdoc />
    public void SetFamilyData(string famRef, ParserEventType eventType, string data)
        => _configuration.OnFamilyData(data, famRef, (int)eventType);

    /// <inheritdoc />
    public void SetFamilyMember(string famRef, string individualId, int famMember)
        => _configuration.OnFamilyIndiv(individualId, famRef, famMember);

    /// <inheritdoc />
    public void EndOfEntry(string famRef)
        => _configuration.OnEntryEnd(famRef);
}
