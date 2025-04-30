using BaseLib.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using GenFree.Interfaces.Sys;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.Models.System;

public partial class BaseSystem: ObservableObject, ISystem
{
    private IPersistence _persistence;

    public BaseSystem(IPersistence persistence) {
        _persistence = persistence;
    }

    [ObservableProperty]
    private string _progOwner;
    [ObservableProperty]
    private bool _progIsDemo;

    public Func<string, bool> CheckLicence { get; set; } = (s) => ChecLicInt(s);

    public bool SetLicNr(string licNr)
    {
        if( ChecLicInt(licNr) )
        {
            _persistence.WriteStringProg("IDF.Dat", licNr);

            ProgIsDemo = false;

            var _A = string.Join(" ", _persistence.ReadStringMLProg("Adresse", 3));
            if (_A.Trim() == "")
            {
                _A = "Adresse eingeben";
            }
            ProgOwner = _A.Trim();
            return true;
        }
        return false;
    }

    private static bool ChecLicInt(string licNr)
    {
        var _licSplit = licNr.Split('-');
        if (_licSplit.Length == 4 && _licSplit[1] == "GB")
        {
            if (_licSplit[0].Substring(2, 1).ToUpper() == "Q")
            {
                var iChar = 0;
                var _Li = 0;
                while (iChar < 10)
                {
                    _Li += _licSplit[2].Substring(iChar++, 1).AsInt();
                }
                _Li += _licSplit[3].Left(1).AsInt();

                var _Li1 = (int)Math.Round((_Li - 1) / _licSplit[3].Substring(4, 1).AsDouble());
                if (_Li1 == _licSplit[3].Substring(2, 2).AsInt())
                {                   
                    return true;
                }
            }

        }
        return false;
    }

}