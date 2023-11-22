using System;
using System.Collections.Generic;
using GenFree.Interfaces.DB;

namespace Helper
{
    public static class StringHelper
    {
#nullable enable
        public static IEnumerable<string> ToStrings<T>(this IEnumerable<T> data, Func<T, string> func)
        {
            foreach (var item in data)
            {
                yield return func(item);
            }
        }

        public static string AsString(this object? data)
            => data switch
            {
                string s => s,
                IField f => f.Value.AsString(),
                null => "",
                object o => o.ToString() ?? "",
            };

        public static string Left(this string data, int iCnt)
            => iCnt >= 0
            ? data.Substring(0, Math.Min(data.Length, iCnt))
            : data.Substring(0, Math.Max(0, data.Length + iCnt));

        public static string Right(this string data, int iCnt)
            => iCnt >= 0
            ? data.Substring(Math.Max(0, data.Length - iCnt))
            : data.Substring(Math.Min(data.Length, -iCnt));
        public static string Uml2Such(this string UbgT)
        {
            List<(string uml, string repl)> Umlauts = new List<(string, string)>() { ("Ü", "U"), ("Ä", "A"), ("Ö", "O"), ("ä", "Ä"), ("à", "À"), ("á", "Á"), ("â", "Â"), ("ß", "ss") };
            int num = 0;
            while (num != -1 && !string.IsNullOrEmpty(UbgT))
            {
                foreach (var u in Umlauts)
                {
                    if ((num = UbgT.IndexOf(u.uml)) >= 0 && /*!*/UbgT.Substring(num, 1) == u.uml)
                    {
                        UbgT = UbgT.Replace(u.uml, u.repl);
                        break;
                    }
                    else
                    {
                        num = -1;
                    }
                }
            }
            return UbgT;
        }

        public static string FrameIfNEoW(this string kont2, params string[] sFrame)
        {
            if (string.IsNullOrWhiteSpace(kont2) || sFrame.Length < 1)
            {
                return kont2;
            }
            return $"{sFrame[0]}{kont2}{sFrame[Math.Min(sFrame.Length - 1, 1)]}";
        }

        public static string[] IntoString(this string[] asData, string[]? asKont = null, int offs = 0)
        {
            asKont ??= new string[Math.Max(0, asData.Length + offs)];
            for (var i = 0; i < asData.Length; i++)
                if (i + offs >= 0 && i + offs < asKont.Length)
                    asKont[i + offs] = asData[i];
            return asKont;
        }
    }
}
