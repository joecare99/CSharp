using GenFreeWin;
using GenFree.Interfaces.UI;
using System;

namespace GenFree.Helper
{
    public static class DateHelper2
    {

        public static (int Years, int Months, int Days)? CalculateAgeParts(DateTime date1, DateTime date2)
        {
            if (date1 < date2)
            {
                var temp = date1;
                date1 = date2;
                date2 = temp;
            }

            int years = date1.Year - date2.Year;
            int months = date1.Month - date2.Month;
            int days = date1.Day - date2.Day;

            if (days < 0)
            {
                months--;
                days += DateTime.DaysInMonth(date1.AddMonths(-1).Year, date1.AddMonths(-1).Month);
            }
            if (months < 0)
            {
                years--;
                months += 12;
            }

            if (years >= 120)
                return null;

            return (years, months, days);
        }

        public static string FormatAge((int Years, int Months, int Days)? ageParts, IApplUserTexts IText)
        {
            if (ageParts == null)
                return "";

            var (years, months, days) = ageParts.Value;
            return $"{IText[EUserText.t117]}{years}{IText[EUserText.t118]}{months}{IText[EUserText.t119]}{days}{IText[EUserText.t216]}";
        }

        public static string? CalcAge(string sDatum1, string sDatum2, IApplUserTexts IText)
        {
            if (!DateTime.TryParseExact(sDatum1, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out var date1) ||
            !DateTime.TryParseExact(sDatum2, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out var date2))
            {
                return null;
            }

            var ageParts = CalculateAgeParts(date1, date2);
            return FormatAge(ageParts, IText);
        }
    }
}
