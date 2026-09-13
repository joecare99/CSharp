using System;

namespace Property.Editor;

/// <summary>
/// Describes whether a proposed property value is valid.
/// </summary>
public sealed class PropertyValidationResult
{
    private PropertyValidationResult(bool isValid, string? errorMessage)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    /// <summary>Gets the successful validation result.</summary>
    public static PropertyValidationResult Valid { get; } = new(true, null);

    /// <summary>Gets whether the proposed value is valid.</summary>
    public bool IsValid { get; }

    /// <summary>Gets the consumer-visible error text when validation fails.</summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Creates an invalid validation result.
    /// </summary>
    /// <param name="errorMessage">The consumer-supplied validation error text.</param>
    /// <returns>An invalid validation result.</returns>
    public static PropertyValidationResult Invalid(string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
        {
            throw new ArgumentException("A validation error message is required.", nameof(errorMessage));
        }

        return new PropertyValidationResult(false, errorMessage);
    }
}
