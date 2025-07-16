using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using BaseLib.Helper;
using System.Globalization;

namespace GenFree.Helper;

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

    /// <summary>
    /// Wandelt ein Datum im Format "yyyyMMdd" in das Format "dd.MM.yyyy" um.
    /// Gibt den ursprünglichen Wert zurück, falls die Eingabe ungültig ist.
    /// </summary>
    /// <param name="dateStr">Datum als Zeichenkette im Format "yyyyMMdd"</param>
    /// <returns>Datum als "dd.MM.yyyy" oder unveränderte Eingabe bei Fehler</returns>
    public static string Date2DotDateStr(this string dateStr)
    {
        if (string.IsNullOrWhiteSpace(dateStr) || dateStr.Length != 8)
            return dateStr;

        if (DateTime.TryParseExact(dateStr, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            return dt.ToString("dd.MM.yyyy");

        return dateStr;
    }

    /// <summary>
    /// Formatiert ein Datum im Format "YYYYMMDD" oder "YYYY" zu "DD.MM.YYYY".
    /// - Wenn das Datum nur 4 Zeichen hat, wird es mit Nullen auf "YYYY0000" ergänzt.
    /// - Wenn das Datum weniger als 8 Zeichen hat, wird es rechts mit Nullen aufgefüllt.
    /// - Ist das Datum numerisch > 0, werden Tag, Monat und Jahr extrahiert und als "TT.MM.JJJJ" formatiert.
    /// - Ungültige oder leere Eingaben führen zu einer leeren Rückgabe.
    /// </summary>
    /// <param name="DDatum">Das zu formatierende Datum als String.</param>
    /// <returns>Das formatierte Datum als "TT.MM.JJJJ" oder ein leerer String.</returns>
    public static string Date2DotDateStr2(this string DDatum)
    {
        // Falls nur Jahr angegeben, mit Nullen auffüllen
        DDatum = DDatum.Trim();
        if (DDatum.Length < 8)
            DDatum = DDatum.PadRight(8, '0');

        // Prüfen, ob das Datum numerisch > 0 ist
        if (long.TryParse(DDatum, out var num) && num > 0)
        {
            // Tag, Monat, Jahr extrahieren
            // und Zusammenbauen im Format "TT.MM.JJJJ"
            return $"{DDatum.Substring(6, 2)}.{DDatum.Substring(4, 2)}.{DDatum.Substring(0, 4)}";
        }
        return string.Empty;
    }
    public static IList<string> IntoString(this DateTime[] adDates, IList<string>? asRes = null, int offs = 0)
    {
        asRes ??= new string[Math.Max(0, adDates.Length + offs)];
        for(var i = 0; i<adDates.Length;i++)
            if (i + offs >= 0 && i + offs < asRes.Count)
                asRes[i+offs] = adDates[i].ToString("yyyy.MM.dd");
        return asRes;
    }

    public static string DayOfWeekStr(this DateTime dDate)
    => DayOfWeekStr(dDate, CultureInfo.CurrentUICulture);

    public static string DayOfWeekStr(this DateTime dDate, CultureInfo ci)
        => ci.DateTimeFormat.AbbreviatedDayNames[(int)ci.Calendar.GetDayOfWeek(dDate)];

}