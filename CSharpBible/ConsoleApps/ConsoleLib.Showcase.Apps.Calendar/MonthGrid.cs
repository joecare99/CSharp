using System;

namespace ConsoleLib.Showcase.Apps.Calendar;

/// <summary>Monday-first month grid used exclusively by the reusable calendar feature.</summary>
public readonly record struct MonthGrid(int Year, int Month, int[] Cells)
{
    public int WeekCount => Cells.Length / 7;
    public int Cell(int week, int weekday) => Cells[(week * 7) + weekday];

    public static MonthGrid For(int year, int month)
    {
        var offset = ((int)new DateTime(year, month, 1).DayOfWeek + 6) % 7;
        var days = DateTime.DaysInMonth(year, month);
        var cells = new int[((offset + days + 6) / 7) * 7];
        for (var day = 1; day <= days; day++)
            cells[offset + day - 1] = day;
        return new MonthGrid(year, month, cells);
    }

    public static (int Year, int Month) PreviousMonth(int year, int month) =>
        month == 1 ? (year - 1, 12) : (year, month - 1);

    public static (int Year, int Month) NextMonth(int year, int month) =>
        month == 12 ? (year + 1, 1) : (year, month + 1);
}
