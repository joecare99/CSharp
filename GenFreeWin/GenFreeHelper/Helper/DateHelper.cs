using Microsoft.VisualBasic;
using System;

namespace GenFree.Helper
{
    public static class DateHelper
    {
#nullable enable
        private static Func<DateTime> _getDate = ()=>DateTime.Today;
        public static void SetDate(Func<DateTime> f)
        {
            _getDate = f;
        }
        public static string FormatDatum(this object? anlDatum, string sHeader,int iDefault = 1)
        {
            string kont_0 = anlDatum.AsString();
            if (anlDatum is DateTime dt || DateTime.TryParse(kont_0,out dt))
            {
                kont_0 = $"{sHeader}{dt:d}";
            }
            else if (iDefault == 1)
            {
                kont_0 = $"{sHeader}{_getDate():d}";
            }

            return kont_0;
        }

        public static string Date2DotDateStr(this string Datu)
        {
            Datu = $"{Strings.Mid(Datu, 7, 2)} {Strings.Mid(Datu, 5, 2)} {Datu.Left( 4)}".Trim();
            if (Datu.Length <= 0)
            {
                return Datu;
            }
            return Datu.Replace(" ", ".");
        }
 
        public static string[] IntoString(this DateTime[] adDates, string[]? asRes = null, int offs = 0)
        {
            asRes ??= new string[Math.Max(0, adDates.Length + offs)];
            for(var i = 0; i<adDates.Length;i++)
                if (i + offs >= 0 && i + offs < asRes.Length)
                    asRes[i+offs] = adDates[i].ToString("yyyy.MM.dd");
            return asRes;
        }
    }
}