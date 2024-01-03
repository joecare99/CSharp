using GenFree.Helper;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static bool IsMonthShort(this string sDateCand)
            => sDateCand == cMon_JAN
            || sDateCand == cMon_FEB
            || sDateCand == cMon_MAR
            || sDateCand == cMon_MAER
            || sDateCand == cMon_APR
            || sDateCand == cMon_MAY
            || sDateCand == cMon_MAI
            || sDateCand == cMon_JUN
            || sDateCand == cMon_JUL
            || sDateCand == cMon_AUG
            || sDateCand == cMon_SEP
            || sDateCand == cMon_OCT
            || sDateCand == cMon_OKT
            || sDateCand == cMon_NOV
            || sDateCand == cMon_DEC
            || sDateCand == cMon_DEZ;

        public static int MonthShortToMonth(this string sDateCand,bool xTolerant = false)
            => sDateCand switch
            {
                cMon_JAN => 1,
                cMon_FEB => 2,
                cMon_MAR => 3,
                cMon_MAER => 3,
                cMon_APR => 4,
                cMon_MAY => 5,
                cMon_MAI => 5,
                cMon_JUN => 6,
                cMon_JUL => 7,
                cMon_AUG => 8,
                cMon_SEP => 9,
                cMon_OCT => 10,
                cMon_OKT => 10,
                cMon_NOV => 11,
                cMon_DEC => 12,
                cMon_DEZ => 12,
                _ when xTolerant => 0,
                _ => throw new ArgumentException($"MonthShortToMonth: {sDateCand} is not a month short"),
            };

        public static string MonthToMonthShort(this int iMonth, bool xTolerant = false)
            => iMonth switch
            {
                1 => cMon_JAN,
                2 => cMon_FEB,
                3 => cMon_MAR,
                4 => cMon_APR,
                5 => cMon_MAY,
                6 => cMon_JUN,
                7 => cMon_JUL,
                8 => cMon_AUG,
                9 => cMon_SEP,
                10 => cMon_OCT,
                11 => cMon_NOV,
                12 => cMon_DEC,
                _ when xTolerant => "???",
                _ => throw new ArgumentException($"MonthToMonthShort: {iMonth} is not a month"),
            };

        public static string Dat2GedDat(this string text6, string sSep)
        {
            if (text6.Length>6 
                && text6.Substring(2, 1) == sSep 
                && text6.Substring(5, 1) == sSep)
            {
                text6 = text6.Substring(3, 2) switch
                {
                    $"01" => text6.Left(2) + " " + cMon_JAN + " " + text6.Right(4),
                    $"02" => text6.Left(2) + " " + cMon_FEB + " " + text6.Right(4),
                    $"03" => text6.Left(2) + " " + cMon_MAR + " " + text6.Right(4),
                    $"04" => text6.Left(2) + " " + cMon_APR + " " + text6.Right(4),
                    $"05" => text6.Left(2) + " " + cMon_MAY + " " + text6.Right(4),
                    $"06" => text6.Left(2) + " " + cMon_JUN + " " + text6.Right(4),
                    $"07" => text6.Left(2) + " " + cMon_JUL + " " + text6.Right(4),
                    $"08" => text6.Left(2) + " " + cMon_AUG + " " + text6.Right(4),
                    $"09" => text6.Left(2) + " " + cMon_SEP + " " + text6.Right(4),
                    $"10" => text6.Left(2) + " " + cMon_OCT + " " + text6.Right(4),
                    $"11" => text6.Left(2) + " " + cMon_NOV + " " + text6.Right(4),
                    $"12" => text6.Left(2) + " " + cMon_DEC + " " + text6.Right(4),
                    _ => text6,
                };
            }

            return text6;
        }

    }



}
