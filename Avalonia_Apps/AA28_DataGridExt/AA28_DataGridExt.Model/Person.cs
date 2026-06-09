using System;

namespace AA28_DataGridExt.Model;

/// <summary>
/// Represents one person row in the data grid sample.
/// </summary>
public class Person
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public int Id { get; set; } = -1;

    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the birthday.
    /// </summary>
    public DateTime? Birthday { get; set; }

    /// <summary>
    /// Gets or sets the birthday in the format expected by Avalonia date controls.
    /// </summary>
    public DateTimeOffset? BirthdayOffset
    {
        get => Birthday.HasValue ? new DateTimeOffset(DateTime.SpecifyKind(Birthday.Value.Date, DateTimeKind.Utc)) : null;
        set => Birthday = value?.Date;
    }

    /// <summary>
    /// Gets or sets the related department.
    /// </summary>
    public Department? Department { get; set; }
}
