using System;
using System.Collections.Generic;
using GenFree.Interfaces.DB;
using Microsoft.VisualBasic;

namespace GenFree.Helper
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
            List<(string uml, string repl)> Umlauts = new List<(string, string)>() {
                ("Ü", "U"), ("Ä", "A"), ("Ö", "O"), ("ä", "Ä"), ("à", "À"), ("á", "Á"), ("â", "Â"), ("ß", "ss") };
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

        public static string ANSELDecode(this string Ubgt1)
        {
            Ubgt1 = Ubgt1.Replace("\xCF", "ß");
            if (Ubgt1.Contains('\xE8'.AsString()))
            {
                foreach (var r in new (string, string)[] {
                ("\xE8u", "ü"), ("\x00E8a", "ä"), ("\xE8o", "ö"), ("\xE8U", "Ü"), ("\x00E8A", "Ä"), ("\xE8O", "Ö")
                }) Ubgt1 = Ubgt1.Replace(r.Item1, r.Item2);
            }
            if (Ubgt1.Contains('\xE2'.AsString()))
            {
                foreach (var r in new (string, string)[] {
                    ("\xE2" + "a", "á"), ("\xE2" + "e", "é"), ("\xE2" + "o", "ó"), ("\xE2" + "i", "í"),
                    ("\xE2" + "u", "ú"), ("\xE2" + "A", "Á"), ("\xE2" + "E", "É"), ("\xE2" + "O", "Ó"),
                    ("\xE2" + "I", "Í"), ("\xE2" + "U", "Ú"),
                }) Ubgt1 = Ubgt1.Replace(r.Item1, r.Item2);
            }
            if (Ubgt1.Contains('\xE3'.AsString()))
            {
                foreach (var r in new (string, string)[] {
                   ("\xE3" + "a", "â"), ("\xE3" + "e", "ê"), ("\xE3" + "o", "ô"), ("\xE3" + "i", "î"),
                   ("\xE3" + "u", "û"), ("\xE3" + "A", "Â"), ("\xE3" + "E", "Ê"), ("\xE3" + "O", "Ô"),
                   ("\xE3" + "I", "Î"), ("\xE3" + "U", "Û"),
                }) Ubgt1 = Ubgt1.Replace(r.Item1, r.Item2);
            }
            if (Ubgt1.Contains('\xC5'.AsString()))
            {
                var num5_ = Strings.InStr(Ubgt1, '\xC5'.AsString());
                Ubgt1 = Ubgt1.Left(num5_ - 1) + "¿" + Strings.Mid(Ubgt1, num5_ + 2, Ubgt1.Length);
            }
            return Ubgt1;
        }

        public static string IBM_DOSDecode(this string Ubgt1)
        {
            Ubgt1 = Ubgt1.Replace((char)0xCF, 'ß');
            Ubgt1 = Ubgt1.Replace((char)255, 'ß');
            Ubgt1 = Ubgt1.Replace((char)225, 'ß');
            Ubgt1 = Ubgt1.Replace((char)0x81, 'ü');
            Ubgt1 = Ubgt1.Replace((char)154, 'Ü');
            Ubgt1 = Ubgt1.Replace((char)148, 'ö');
            Ubgt1 = Ubgt1.Replace((char)153, 'Ö');
            Ubgt1 = Ubgt1.Replace((char)132, 'ä');
            Ubgt1 = Ubgt1.Replace((char)142, 'Ä');
            Ubgt1 = Ubgt1.Replace((char)130, 'é');
            Ubgt1 = Ubgt1.Replace((char)138, 'è');
            Ubgt1 = Ubgt1.Replace((char)140, 'î');
            Ubgt1 = Ubgt1.Replace((char)136, 'ê');
            Ubgt1 = Ubgt1.Replace((char)133, 'à');
            Ubgt1 = Ubgt1.Replace((char)138, 'è');
            Ubgt1 = Ubgt1.Replace((char)138, 'è');
            Ubgt1 = Ubgt1.Replace((char)138, 'è');
            Ubgt1 = Ubgt1.Replace($"{(char)236}", "oo");
            Ubgt1 = Ubgt1.Replace((char)(175), (char)(187));
            Ubgt1 = Ubgt1.Replace("\xA5", "Ñ");
            Ubgt1 = Ubgt1.Replace("\xA4", "ñ");
            Ubgt1 = Ubgt1.Replace('\xA3'.AsString(), "ú");
            Ubgt1 = Ubgt1.Replace('\xA2'.AsString(), "ó");
            Ubgt1 = Ubgt1.Replace('\xA1'.AsString(), "í");
            Ubgt1 = Ubgt1.Replace('\xA0'.AsString(), "á");
            Ubgt1 = Ubgt1.Replace((char)152, 'ÿ');
            Ubgt1 = Ubgt1.Replace((char)151, 'ù');
            Ubgt1 = Ubgt1.Replace((char)150, 'û');
            Ubgt1 = Ubgt1.Replace((char)149, 'ò');
            Ubgt1 = Ubgt1.Replace((char)147, 'ô');
            Ubgt1 = Ubgt1.Replace((char)144, 'É');
            Ubgt1 = Ubgt1.Replace((char)143, 'Å');
            Ubgt1 = Ubgt1.Replace((char)141, 'ì');
            Ubgt1 = Ubgt1.Replace((char)139, 'ï');
            Ubgt1 = Ubgt1.Replace((char)137, 'ë');
            Ubgt1 = Ubgt1.Replace((char)135, 'ç');
            Ubgt1 = Ubgt1.Replace((char)134, 'å');
            Ubgt1 = Ubgt1.Replace((char)131, 'â');
            Ubgt1 = Ubgt1.Replace((char)174, (char)171);
            return Ubgt1;
        }

        public static string UTF8Decode(this string Ubgt1)
        {
            if (Strings.InStr(Ubgt1, '\xC2'.AsString()) != 0)
            {
                Ubgt1 = Ubgt1.Replace($"\xC2\xA2", "¢");
                Ubgt1 = Ubgt1.Replace("\xC2" + '\xA3'.AsString(), "£");
                Ubgt1 = Ubgt1.Replace("\xC2" + '\xA5'.AsString(), "¥");
                Ubgt1 = Ubgt1.Replace("\xC2" + '\xA7'.AsString(), "§");
                Ubgt1 = Ubgt1.Replace("\xC2" + '\xA9'.AsString(), "©");
                Ubgt1 = Ubgt1.Replace("\xC2" + (char)174, "®");
                Ubgt1 = Ubgt1.Replace("\xC2" + (char)0xB0, "°");
                Ubgt1 = Ubgt1.Replace("\xC2" + (char)177, "±");
                Ubgt1 = Ubgt1.Replace("\xC2" + (char)178, "²");
                Ubgt1 = Ubgt1.Replace("\xC2" + (char)179, "³");
                Ubgt1 = Ubgt1.Replace("\xC2" + (char)181, "µ");
                Ubgt1 = Ubgt1.Replace("\xC2" + (char)183, "·");
                Ubgt1 = Ubgt1.Replace("\xC2" + (char)188, "¼");
                Ubgt1 = Ubgt1.Replace("\xC2" + (char)189, "½");
                Ubgt1 = Ubgt1.Replace("\xC2" + (char)190, "¾");
            }
            if (Strings.InStr(Ubgt1, '\xC3'.AsString()) != 0)
            {
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)128, "À");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)129, "Á");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)130, "Â");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)131, "Ã");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)132, "Ä");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)133, "Å");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)134, "Æ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)135, "Ç");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)136, "È");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)137, "É");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)138, "Ê");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)139, "Ë");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)140, "Ì");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)141, "Í");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)142, "Î");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)143, "Ï");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)144, "Ð");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)145, "Ñ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)146, "Ò");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)147, "Ó");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)148, "Ô");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)149, "Õ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)150, "Ö");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)151, "×");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)152, "Ø");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)153, "Ù");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)154, "Ú");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)155, "Û");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)156, "Ü");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)157, "Ý");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)158, "Þ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)159, "ß");
                Ubgt1 = Ubgt1.Replace("\xC3" + '\xA0'.AsString(), "à");
                Ubgt1 = Ubgt1.Replace("\xC3" + '\xA1'.AsString(), "á");
                Ubgt1 = Ubgt1.Replace("\xC3" + '\xA2'.AsString(), "â");
                Ubgt1 = Ubgt1.Replace("\xC3" + '\xA3'.AsString(), "Â");
                Ubgt1 = Ubgt1.Replace("\xC3" + '\xA4'.AsString(), "ä");
                Ubgt1 = Ubgt1.Replace("\xC3" + '\xA5'.AsString(), "å");
                Ubgt1 = Ubgt1.Replace("\xC3" + '\xA6'.AsString(), "æ");
                Ubgt1 = Ubgt1.Replace("\xC3" + '\xA7'.AsString(), "ç");
                Ubgt1 = Ubgt1.Replace("\xC3" + '\xA8'.AsString(), "è");
                Ubgt1 = Ubgt1.Replace("\xC3" + '\xA9'.AsString(), "é");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)170, "ê");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)171, "ë");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)172, "ì");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)173, "í");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)174, "î");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)175, "ï");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)176, "ð");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)177, "ñ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)178, "ò");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)179, "ó");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)180, "ô");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)181, "õ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)182, "ö");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)183, "÷");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)184, "ø");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)185, "ù");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)186, "ú");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)187, "û");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)188, "ü");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)189, "ý");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)191, "ÿ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)192, "Ā");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)193, "ā");
                Ubgt1 = Ubgt1.Replace("\xC3" + '\xC2'.AsString(), "Ă");
                Ubgt1 = Ubgt1.Replace("\xC3" + '\xC3'.AsString(), "ă");
                Ubgt1 = Ubgt1.Replace("\xC3" + '\xC4'.AsString(), "Ą");
                Ubgt1 = Ubgt1.Replace("\xC3" + '\xC5'.AsString(), "ą");
                Ubgt1 = Ubgt1.Replace("\xC3" + '\xC6'.AsString(), "Ć");
                Ubgt1 = Ubgt1.Replace("\xC3" + '\xC7'.AsString(), "ć");
                Ubgt1 = Ubgt1.Replace("\xC3" + '\xC8'.AsString(), "Ĉ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)201, "ĉ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)202, "Ċ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)203, "ċ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)204, "Č");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)205, "č");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)206, "Ď");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)207, "ď");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)208, "Đ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)209, "đ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)210, "Ē");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)211, "ē");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)212, "Ĕ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)213, "ĕ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)214, "Ė");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)215, "ė");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)216, "Ę");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)217, "ę");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)218, "Ě");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)219, "ě");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)220, "Ĝ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)221, "ĝ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)222, "Ğ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)223, "ğ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)224, "Ġ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)225, "ġ");
                Ubgt1 = Ubgt1.Replace("\xC3" + '\xE2'.AsString(), "Ģ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)227, "ģ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)228, "Ĥ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)229, "ĥ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)230, "Ħ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)231, "ħ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)232, "Ĩ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)233, "ĩ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)234, "Ī");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)235, "ī");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)236, "Ĭ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)237, "ĭ");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)238, "Į");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)239, "į");
                Ubgt1 = Ubgt1.Replace("\xC3" + (char)240, "İ");
            }
            if (Ubgt1.Contains('\xC4'.AsString()))
            {
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)130, "Ă");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)131, "ă");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)132, "Ӓ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)133, "ӓ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)134, "Ӕ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)135, "ӕ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)136, "Ӗ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)137, "ӗ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)138, "Ә");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)139, "ә");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)140, "Ӛ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)141, "ӛ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)142, "Ӝ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)143, "ӝ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)144, "Ӟ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)145, "ӟ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)146, "Ӡ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)147, "ӡ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)148, "Ӣ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)149, "ӣ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)150, "Ӥ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)151, "ӥ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)152, "Ӧ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)0x99, "ӧ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)154, "Ө");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)155, "ө");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)156, "Ӫ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)157, "ӫ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)158, "Ӭ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + (char)159, "ӭ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + '\xA0'.AsString(), "Ӯ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + '\xA1'.AsString(), "ӯ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + '\xA2'.AsString(), "Ӱ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + '\xA3'.AsString(), "ӱ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + '\xA4'.AsString(), "Ӳ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + '\xA5'.AsString(), "ӳ");
                Ubgt1 = Ubgt1.Replace('\xC4'.AsString() + '\xA6'.AsString(), "Ӵ");
            }
            if (Ubgt1.Contains('\xC5'.AsString()))
            {
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)140, "Ō");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)141, "ō");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)142, "Ŏ");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)143, "ŏ");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)144, "Ő");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)145, "ő");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)146, "Œ");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)147, "œ");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)148, "Ŕ");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)149, "ŕ");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)150, "Ŗ");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)151, "ŗ");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)152, "Ř");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)153, "ř");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)154, "Ś");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)155, "ś");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)156, "Ŝ");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)157, "ŝ");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)158, "Ş");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)159, "ş");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + '\xA0'.AsString(), "Š");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + '\xA1'.AsString(), "š");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + '\xA2'.AsString(), "Ţ");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + '\xA3'.AsString(), "ţ");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + '\xA4'.AsString(), "Ť");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + '\xA5'.AsString(), "ť");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + '\xA6'.AsString(), "Ŧ");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + '\xA7'.AsString(), "ŧ");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + '\xA8'.AsString(), "Ũ");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + '\xA9'.AsString(), "ũ");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)170, "Ū");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)171, "ū");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)172, "Ŭ");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)173, "ŭ");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)174, "Ů");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)175, "ů");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)176, "Ű");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)177, "ű");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)178, "Ų");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)179, "ų");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)184, "Ÿ");
                Ubgt1 = Ubgt1.Replace('\xC5'.AsString() + (char)0xE5, "†");
            }
            if (Ubgt1.Contains('\xC8'.AsString()))
            {
                Ubgt1 = Ubgt1.Replace('\xC8'.AsString() + (char)0x98, "Ș");
                Ubgt1 = Ubgt1.Replace('\xC8'.AsString() + (char)153, "ș");
                Ubgt1 = Ubgt1.Replace('\xC8'.AsString() + (char)154, "Ț");
                Ubgt1 = Ubgt1.Replace('\xC8'.AsString() + (char)155, "ț");
            }
            if (Ubgt1.Contains('\xE2'.AsString()))
            {
                Ubgt1 = Ubgt1.Replace('\xE2'.AsString() + (char)128 + '\xA0'.AsString(), "†");
                Ubgt1 = Ubgt1.Replace('\xE2'.AsString() + (char)128 + '\xA1'.AsString(), "‡");
                Ubgt1 = Ubgt1.Replace('\xE2'.AsString() + (char)128 + '\xA2'.AsString(), "•");
                Ubgt1 = Ubgt1.Replace('\xE2'.AsString() + (char)128 + (char)176, "‰");
                Ubgt1 = Ubgt1.Replace('\xE2'.AsString() + (char)130 + (char)172, "€");
                Ubgt1 = Ubgt1.Replace('\xE2'.AsString() + (char)133 + (char)147, "⅓");
                Ubgt1 = Ubgt1.Replace('\xE2'.AsString() + (char)133 + (char)148, "⅔");
                Ubgt1 = Ubgt1.Replace('\xE2'.AsString() + (char)134 + (char)146, "→");
                Ubgt1 = Ubgt1.Replace('\xE2'.AsString() + (char)136 + (char)158, "∞");
            }
            return Ubgt1;
        }

    }
}
