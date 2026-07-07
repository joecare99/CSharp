using BaseLib.Helper;
using GenFree;
using GenFree.Data;
using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using System;

namespace Gen_FreeWin.ViewModels;

public partial class NamenSuchViewModel
{
    public static string[] Datl(int persInArb, bool xShort)
    {
        var result = new string[4];
        var arDates = DataModul.Event.GetPersonDates(persInArb, out _, HndlPlace);
        for (var num = 1; num <= 4; num++)
        {
            if (arDates[num] > DateTime.MinValue)
            {
                result[(num - 1) & 2] = (num & 1) switch
                {
                    1 => $"{arDates[num].Year}",
                    0 when result[(num - 1) & 2] == "" => $"{arDates[num].Year}",
                    _ => result[(num - 1) & 2],
                };
            }
        }

        return result;

        void HndlPlace(EEventArt iEvent, int iPlace, string sPlace)
        {
            if (iPlace > 0)
            {
                string sPlace2 = "";
                if (iPlace > 0)
                {
                    sPlace2 = Place_ReadData(iPlace, 0, 1, xShort);
                    if (sPlace.Trim() != "")
                    {
                        sPlace2 = sPlace2.TrimEnd() + " " + sPlace.AsString().Trim();
                    }
                }

                ;
                result[((int)iEvent - 101) | 1] = ((int)iEvent & 1) switch
                {
                    1 => sPlace2,
                    0 when result[(int)iEvent - 101 - 1 | 1] == "" => sPlace2,
                    _ => result[(int)iEvent - 101 - 1 | 1]
                };
            }
        }
    }
}
