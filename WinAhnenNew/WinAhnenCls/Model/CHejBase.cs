using System;
using System.Data;
using System.IO;
using WinAhnenCls.Helper;

namespace WinAhnenCls.Model
{
    public struct SIndexEntry
    {
        int Id;
        DateTime Date;
        long value { get => Date.ToBinary(); set => Date = DateTime.FromBinary(value); }
    }

    public abstract class CHejBase
    {
        int Count => GetCount();
        protected abstract int GetCount();

        public abstract bool TestStreamHeader(Stream st);
        public abstract void Clear();
        public abstract int IndexOf(object Krit);
        public abstract void ReadFromStream(Stream st, CHejBase? cls = null);
        public abstract void WriteToStream(Stream st);
        public abstract void ReadFromDataset(DataSet ds, CHejBase? cls = null);
        public abstract void UpdateDataset(DataSet ds);

        public static string HejDate2DateStr(string Day, string Month, string Year, bool dtOnly = false)
        {
            // Handle Day
            string result = "";
            if (int.TryParse(Day, out _))
                result = Day.PadLeft(2, '0') + ".";
            else if (int.TryParse(Day.RightStr(2), out _))
                if (dtOnly)
                    result = Day.RightStr(2) + ".";
                else
                    result = Day.PadLeft(2, '0') + ".";
            else if (!string.IsNullOrWhiteSpace(Day) && !dtOnly)
                result = Day + " 01.";
            else
                result = "01.";
            // Handle Month
            if (int.TryParse(Month, out _))
                result = result + Month.PadLeft(2, '0') + ".";
            else
                result = result + "01.";
            // Handle Year
            if (int.TryParse(Year, out _))
                result = result + Year.PadLeft(4, '0');
            else if (result != "01.01.")
                result = result + "1900";
            else
                result = result + "01";

            // Check for dummy date
            if (result == "01.01.01")
                result = "";
            return result;
        }

        public static void DateStr2HeyDate(string aDate, out string Day, out string Month, out string Year)
        {
            (Day, Month, Year) = ("", "", "");
            if (aDate == "") return;
            string[] s = aDate.Split('.');
            if (s.Length == 3)
            {
                Day = s[0];
                Month = s[1];
                Year = s[2];
            }
        }
    }
}
