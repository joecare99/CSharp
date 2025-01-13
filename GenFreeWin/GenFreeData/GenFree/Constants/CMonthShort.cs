using GenFree.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenFree.Constants
{
    public static class CMonthShort
    {
        public const string cMon_JAN = "JAN";
        public const string cMon_FEB = "FEB";
        public const string cMon_MAR = "MAR";
        public const string cMon_APR = "APR";
        public const string cMon_MAY = "MAY";
        public const string cMon_JUN = "JUN";
        public const string cMon_JUL = "JUL";
        public const string cMon_AUG = "AUG";
        public const string cMon_SEP = "SEP";
        public const string cMon_OCT = "OCT";
        public const string cMon_NOV = "NOV";
        public const string cMon_DEC = "DEC";
        public const string cMon_MAER = "MÄR";
        public const string cMon_MAI = "MAI";
        public const string cMon_OKT = "OKT";
        public const string cMon_DEZ = "DEZ";

        private static Dictionary<string, int> _dicMonthShort = new()
        {
            { cMon_JAN, 1 },
            { cMon_FEB, 2 },
            { cMon_MAR, 3 },
            { cMon_APR, 4 },
            { cMon_MAY, 5 },
            { cMon_JUN, 6 },
            { cMon_JUL, 7 },
            { cMon_AUG, 8 },
            { cMon_SEP, 9 },
            { cMon_OCT, 10 },
            { cMon_NOV, 11 },
            { cMon_DEC, 12 },
            { cMon_MAER, 3 },
            { cMon_MAI, 5 },
            { cMon_OKT, 10 },
            { cMon_DEZ, 12 },
        };

        public static bool IsMonthShort(this string sDateCand)
            => _dicMonthShort.ContainsKey(sDateCand.ToUpper());

        public static int MonthShortToMonth(this string sDateCand, bool xTolerant = false)
            => _dicMonthShort.TryGetValue(sDateCand.ToUpper(), out var iVal)
                ? iVal
            : xTolerant ? 0 :
                throw new ArgumentException($"MonthShortToMonth: {sDateCand} is not a month short");


        public static string MonthToMonthShort(this int iMonth, bool xTolerant = false)
            => iMonth switch
            {
                >= 1 and <= 12 => _dicMonthShort.ElementAt(iMonth - 1).Key,
                _ when xTolerant => "???",
                _ => throw new ArgumentException($"MonthToMonthShort: {iMonth} is not a month"),
            };

        public static string Dat2GedDat(this string text6, string sSep)
        {
            if (text6.Length > 6
                && text6.Substring(2, 1) == sSep
                && text6.Substring(5, 1) == sSep
                && (text6.Substring(3, 2).AsInt() is int i && i > 0))
            {
                text6 = $"{text6.Left(2)} {_dicMonthShort.ElementAt(i - 1).Key} {text6.Substring(6).Right(4)}";
            }

            return text6;
        }

    }



}
