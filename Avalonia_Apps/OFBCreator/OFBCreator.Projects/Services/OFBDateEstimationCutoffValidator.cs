using System;
using System.IO;

namespace OFBCreator.Projects.Services;

/// <summary>
/// Validates the optional minimum source year used for date estimation.
/// </summary>
public static class OFBDateEstimationCutoffValidator
{
    /// <summary>
    /// Validates a cutoff year. Null disables the cutoff; year zero is never valid.
    /// </summary>
    public static void Validate(int? cutoffYear)
    {
        if (cutoffYear is < 1 or > 9999)
            throw new InvalidDataException("The date estimation cutoff year must be between 1 and 9999, or empty to disable it.");
    }
}
