namespace DataAnalysis.Core.Models;

public enum DimensionKind
{
    Severity,
    Source,
    Hour, // Hour bucket of timestamp
    MessageNormalized,
    ProgID, // Software version extracted from message
    X,
    Y,
    MsgID,
}
