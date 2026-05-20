namespace ChaoticEntropyGenerator;

/// <summary>
/// Represents a single generated entropy value and its history.
/// </summary>
public class EntropyPoint
{
    /// <summary>
    /// The timestamp when the point was generated (for historical context).
    /// </summary>
    public DateTime Timestamp { get; }

    /// <summary>
    /// The raw, calculated entropy value.
    /// </summary>
    public double EntropyValue { get; }

    /// <summary>
    /// A string representation of the calculated value.
    /// </summary>
    public string StringRepresentation { get; }

    public EntropyPoint(DateTime timestamp, double entropyValue)
    {
        Timestamp = timestamp;
        EntropyValue = entropyValue;
        // Format the value to provide a cryptic, unique look
        StringRepresentation = $"[{Timestamp:yyyyMMddHHmmss}.{EntropyValue:F4}]";
    }
}
