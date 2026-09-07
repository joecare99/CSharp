using System;

namespace ConsoleLib.Showcase.Models;

/// <summary>
/// Pure month grid for the calendar app. Weeks start on Monday;
/// cells are 1-based day numbers and 0 marks an empty cell.
/// </summary>
public readonly record struct MonthGrid
{
    /// <summary>Creates a grid for the given month.</summary>
    public MonthGrid(int Year, int Month, int[] Cells)
    {
        if (Month is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(Month), Month, "Month must be between 1 and 12.");
        }

        if (Cells is null || Cells.Length % 7 != 0)
        {
            throw new ArgumentException("Cells must have a length that is a multiple of 7.", nameof(Cells));
        }

        this.Year = Year;
        this.Month = Month;
        this.Cells = Cells;
    }

    /// <summary>Year of the grid.</summary>
    public int Year { get; }

    /// <summary>Month (1-12) of the grid.</summary>
    public int Month { get; }

    /// <summary>42 (or fewer) cells in week-major order; 0 = empty.</summary>
    public int[] Cells { get; }

    /// <summary>Number of weeks (rows) in the grid.</summary>
    public int WeekCount => Cells.Length / 7;

    /// <summary>Number of days in the month.</summary>
    public int DayCount => System.DateTime.DaysInMonth(Year, Month);

    /// <summary>Cell value for a 0-based week row and 0-based (Monday-first) weekday column.</summary>
    public int Cell(int week, int weekday) => Cells[(week * 7) + weekday];

    /// <summary>Builds the grid for a month, padded to a full number of weeks.</summary>
    public static MonthGrid For(int year, int month)
    {
        var firstDay = new System.DateTime(year, month, 1);
        var offset = (int)firstDay.DayOfWeek + 6;
        offset %= 7;

        var daysInMonth = System.DateTime.DaysInMonth(year, month);
        var total = ((offset + daysInMonth) + 6) / 7 * 7;
        var cells = new int[total];

        var index = offset;
        for (var day = 1; day <= daysInMonth; day++)
        {
            cells[index++] = day;
        }

        return new MonthGrid(year, month, cells);
    }

    /// <summary>Previous month, wrapped across year boundaries.</summary>
    public static (int Year, int Month) PreviousMonth(int year, int month) =>
        month == 1 ? (year - 1, 12) : (year, month - 1);

    /// <summary>Next month, wrapped across year boundaries.</summary>
    public static (int Year, int Month) NextMonth(int year, int month) =>
        month == 12 ? (year + 1, 1) : (year, month + 1);
}
