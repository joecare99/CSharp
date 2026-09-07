using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConsoleLib.Showcase.Models;

namespace ConsoleLib.Showcase.Desktop.ViewModels;

/// <summary>Portable calendar state exposed to the Calendar CXAML page.</summary>
public partial class CalendarViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MonthTitle))]
    [NotifyPropertyChangedFor(nameof(DaysText))]
    private int year;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MonthTitle))]
    [NotifyPropertyChangedFor(nameof(DaysText))]
    private int month;

    public CalendarViewModel(DateTime? initialDate = null)
    {
        var date = initialDate ?? DateTime.Today;
        year = date.Year;
        month = date.Month;
    }

    public string MonthTitle => new DateTime(Year, Month, 1).ToString("MMMM yyyy");

    public string DaysText
    {
        get
        {
            var grid = MonthGrid.For(Year, Month);
            var lines = new string[grid.WeekCount];
            for (var week = 0; week < grid.WeekCount; week++)
            {
                var values = new string[7];
                for (var weekday = 0; weekday < 7; weekday++)
                    values[weekday] = grid.Cell(week, weekday) == 0 ? "  " : grid.Cell(week, weekday).ToString("00");
                lines[week] = string.Join(' ', values);
            }

            return string.Join(Environment.NewLine, lines);
        }
    }

    [RelayCommand]
    private void Previous()
    {
        (Year, Month) = MonthGrid.PreviousMonth(Year, Month);
    }

    [RelayCommand]
    private void Next()
    {
        (Year, Month) = MonthGrid.NextMonth(Year, Month);
    }
}
