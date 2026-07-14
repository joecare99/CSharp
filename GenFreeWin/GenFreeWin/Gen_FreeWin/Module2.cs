using BaseLib.Helper;
using GenFreeWin.Views;
using GenFree;
using GenFree.Data;
using GenFree.Interfaces.Sys;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.IO;

namespace GenFreeWin;

[StandardModule]
internal sealed class Module2
{
    static IModul1 Modul1 => _Modul1.Instance;

    /// <summary>
    /// Calculates the "Koelners phonetic"-Index.
    /// </summary>
    /// <param name="Eingabe">The eingabe.</param>
    /// <returns>System.String.</returns>
    public static string Koelner_Phonetic(string Eingabe)
    {
        string text = "";
        Eingabe = Eingabe.Trim();
        Eingabe = Strings.LCase(Eingabe);
        Eingabe = Eingabe.Replace("ph", "f");
        Eingabe = Eingabe.Replace("ü", "u");
        Eingabe = Eingabe.Replace("ä", "a");
        Eingabe = Eingabe.Replace("ö", "o");
        Eingabe = Eingabe.Replace("ß", "ss");
        Eingabe = "#" + Eingabe;
        Eingabe += "#";
        checked
        {
            int num = Eingabe.Length - 2;
            int num2 = 1;
            while (num2 <= num)
            {
                string kette = Strings.Mid(Eingabe, num2, 3);
                string text2 = (num2 != 1) ? Conv_Rest(kette) : Conv_Ersten(kette);
                text += text2;
                num2++;
            }

            text = text.Replace("-", "");
            text = (text.Left(1) != "0") ? text.Replace("0", "") : ("0" + text.Replace("0", ""));
            string Endwert = text.Left(1);
            int num5 = text.Length;
            num2 = 2;
            while (num2 <= num5)
            {
                if (Strings.Mid(text, num2, 1) != Endwert.Right(1))
                {
                    Endwert += Strings.Mid(text, num2, 1);
                }
                num2++;
            }

            Endwert = Endwert.PadRight(6, '0').Left(6);
            return Endwert;
        }
    }

    public static string Conv_Ersten(string Kette)
    {
        return Strings.Mid(Kette, 2, 1) switch
        {
            "a" => "0",
            "e" => "0",
            "i" => "0",
            "j" => "0",
            "y" => "0",
            "o" => "0",
            "u" => "0",
            "c" => Strings.Mid(Kette, 2, 2) switch
            {
                "ca" => "4",
                "ch" => "4",
                "ck" => "4",
                "cl" => "4",
                "co" => "4",
                "cq" => "4",
                "cr" => "4",
                "cu" => "4",
                "cx" => "4",
                _ => 8.AsString()
            },
            _ => Conv_Rest(Kette)
        };
    }


    public static string Conv_Rest(string Kette)
    {
        return Strings.Mid(Kette, 2, 2) switch
        {
            "ds" => "8",
            "dc" => "8",
            "dz" => "8",
            "ts" => "8",
            "tc" => "8",
            "tz" => "8",
            _ => Kette.Left(2) switch
            {
                "cx" => "8",
                "kx" => "8",
                "qx" => "8",
                "sc" => "8",
                "sz" => "8",
                _ => Strings.Mid(Kette, 2, 2) switch
                {
                    "ca" => "4",
                    "co" => "4",
                    "cu" => "4",
                    "ch" => "4",
                    "ck" => "4",
                    "cx" => "4",
                    "cq" => "4",
                    _ => Strings.Mid(Kette, 2, 1) switch
                    {
                        "t" => "2",
                        "d" => "2",
                        "x" => "48",
                        "c" => "8",
                        "a" => "0",
                        "e" => "0",
                        "i" => "0",
                        "j" => "0",
                        "y" => "0",
                        "o" => "0",
                        "u" => "0",
                        "h" => "-",
                        "l" => "5",
                        "r" => "7",
                        "m" => "6",
                        "n" => "6",
                        "s" => "8",
                        "z" => "8",
                        "b" => "1",
                        "p" => "1",
                        "f" => "3",
                        "v" => "3",
                        "w" => "3",
                        "g" => "4",
                        "k" => "4",
                        "q" => "4",
                        _ => "?"
                    }
                },
            },
        };
    }
    /// <summary>
    /// Calculates the SoundEx-Index.
    /// </summary>
    /// <param name="sName">Name of the s.</param>
    /// <returns>System.String.</returns>
    public static string GetSoundEx(string sName)
    {
        if (sName.Length == 0)
        {
            return "";
        }
        sName = sName.ToUpper().Replace("ß", "SS");
        string sCode = sName.Substring(0, 1);
        string text = "";
        checked
        {
            int num = sName.Length - 1;
            int num2 = 1;
            while (true)
            {
                int num3 = num2;
                int num4 = num;
                if (num3 > num4)
                {
                    break;
                }
                string text2 = sName.Substring(num2, 1);
                switch (text2)
                {
                    case "B":
                    case "F":
                    case "P":
                    case "V":
                        text = "1";
                        break;
                    default:
                        switch (text2)
                        {
                            case "C":
                            case "G":
                            case "J":
                            case "K":
                            case "Q":
                            case "S":
                            case "X":
                            case "Z":
                                text = "2";
                                break;
                            default:
                                if (text2 is "D" or "T")
                                {
                                    text = "3";
                                    break;
                                }
                                switch (text2)
                                {
                                    case "L":
                                        text = "4";
                                        break;
                                    case "M":
                                    case "N":
                                        text = "5";
                                        break;
                                    default:
                                        if (text2 == "R")
                                        {
                                            text = "6";
                                        }
                                        break;
                                }
                                break;
                        }
                        break;
                }
                if (sCode.Length == 1 || !sCode.EndsWith(text))
                {
                    sCode += text;
                }
                num2++;
            }
            sCode = sCode + "0000".Left(4);
            return sCode;
        }
    }
    /// <summary>
    /// Select the Entity for the event.
    /// </summary>
    /// <param name="eEventArt">The kind of the event.</param>
    /// <param name="qkenn">The source identifyer.</param>
    /// <returns>System.Int32. The person-No or family-no according to the event and source</returns>
    public static int ZuPerFamNummer(EEventArt eEventArt, short qkenn) => qkenn switch
    {
        1 => Personen.Default.PersonNr, // Person
        2 => Familie.Default.iFamNr, // Familie
        _ when eEventArt <= EEventArt.eA_499 => Personen.Default.PersonNr, // Person
        _ => Familie.Default.iFamNr // Familie
    };


    public static bool IsDriveReady(string sDrive, string Verz2, bool bCheckWriteAccess = false)
    {
        //Discarded unreachable code: IL_0024, IL_0037, IL_003e
        try
        {
            DriveInfo driveInfo = new(sDrive);
            bool isReady = driveInfo.IsReady;
            if (bCheckWriteAccess)
            {
                FileSystem.SetAttr(Verz2, FileAttribute.Normal);
            }
            return isReady;
        }
        catch (Exception ex)
        {
            ProjectData.SetProjectError(ex);
            bool result = false;
            ProjectData.ClearProjectError();
            return result;
        }
    }
}
