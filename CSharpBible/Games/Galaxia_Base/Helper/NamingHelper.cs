using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLib.Models.Interfaces;
using Galaxia.Models.Interfaces;

namespace Galaxia.Helper;

public static class NamingHelper
{
    static readonly string[] stSysNameStart =
        ["An", "Al", "Ar",
                        "Bet", "Bal", "Bir",
                        "Can", "Cel", "Cir",
                        "Den", "Dar", "Dol",
                        "Eg", "El", "Er", "Et",
                        "Fas", "Fal", "Fir", "Fot",
                        "Gal", "Gir", "Gon", "Gat",
                        "Han", "Hal", "Hir", "Hot",
                        "Ith", "Ir", "Il", "Is",
                        "Jan", "Jal", "Jir", "Jot",
                        "Kan", "Kal", "Kir", "Kep",
                        "Len", "Lil", "Lor","Lut"
                ];
    static readonly string[] stSysNameMdl = [
        "an", "en", "in", "on", "un",
        "ar", "er", "ir", "or", "ur",
        "al", "el", "il", "ol", "ul",
        "ag", "ef", "is", "ob", "ul",
    ];
    static readonly string[] stSysNameEnd =
        ["ax", "euze", "ix", "orx", "ulix",
                        "ars", "ers", "irs", "ors", "urs",
                        "als", "els", "ils", "ols", "uls",
                        "axa", "efs", "is", "obs", "ulsa",
                        "ans", "ens", "ins", "ons", "us"
    ];
    public static readonly HashSet<string> existingNames = new HashSet<string>();

    static IRandom _random { get; set; }

    public static void SetRandom(IRandom random) => _random = random;

    public static string GetStarSysName(this string sector)
    {
        if (_random == null)
            throw new InvalidOperationException("Random number generator is not set. Call SetRandom first.");
        var stStart = stSysNameStart.Where(s => s.StartsWith(sector)).ToList();
        var (start, mdl, end) = ("", "", "");
        do
        {
            start = stStart[_random.Next(stStart.Count)] ?? "Reg";
            mdl = (_random.Next(10) > 3) ? stSysNameMdl[_random.Next(stSysNameMdl.Length)] : "";
            end = stSysNameEnd[_random.Next(stSysNameEnd.Length)];
        } while (existingNames.Contains($"{start}{mdl}{end}"));
        existingNames.Add($"{start}{mdl}{end}");
        return $"{start}{mdl}{end}";
    }
}
