namespace FBParser.Analysis;

/// <summary>
/// Defines validated callback emission for genealogical parser events.
/// </summary>
internal interface IGenealogicalEventEmitter
{
    /// <summary>
    /// Emits an individual-name event without additional validation.
    /// </summary>
    void SetIndiName(string individualId, int nameType, string name);

    /// <summary>
    /// Emits an individual-data event without additional validation.
    /// </summary>
    void SetIndiData(string individualId, ParserEventType eventType, string data);

    /// <summary>
    /// Emits an individual-date event with date validation.
    /// </summary>
    void SetIndiDate(string individualId, ParserEventType eventType, string date);

    /// <summary>
    /// Emits an individual-place event with place validation.
    /// </summary>
    void SetIndiPlace(string individualId, ParserEventType eventType, string place);

    /// <summary>
    /// Emits an individual-occupation event without additional validation.
    /// </summary>
    void SetIndiOccu(string individualId, ParserEventType eventType, string occu);

    /// <summary>
    /// Emits an individual-relation event with reference validation.
    /// </summary>
    void SetIndiRelat(string individualId, string famRef, int relType, string mainRef);

    /// <summary>
    /// Emits an individual-reference event without additional validation.
    /// </summary>
    void SetIndiRef(string individualId, ParserEventType eventType, string reference);

    /// <summary>
    /// Emits a family-start event.
    /// </summary>
    void StartFamily(string famRef);

    /// <summary>
    /// Emits a family-type event without additional validation.
    /// </summary>
    void SetFamilyType(string famRef, int famType, string data = "");

    /// <summary>
    /// Emits a family-date event with date validation.
    /// </summary>
    void SetFamilyDate(string famRef, ParserEventType eventType, string date);

    /// <summary>
    /// Emits a family-place event with place validation.
    /// </summary>
    void SetFamilyPlace(string famRef, ParserEventType eventType, string place);

    /// <summary>
    /// Emits a family-data event without additional validation.
    /// </summary>
    void SetFamilyData(string famRef, ParserEventType eventType, string data);

    /// <summary>
    /// Emits a family-member event without additional validation.
    /// </summary>
    void SetFamilyMember(string famRef, string individualId, int famMember);

    /// <summary>
    /// Emits an entry-end event.
    /// </summary>
    void EndOfEntry(string famRef);
}
