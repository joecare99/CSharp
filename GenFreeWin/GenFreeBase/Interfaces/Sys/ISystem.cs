using System;
using System.ComponentModel;

namespace GenFree.Interfaces.Sys;

public interface ISystem: INotifyPropertyChanged
{
    string ProgOwner { get; set; }
    Func<string, bool> CheckLicence { get; set; }

    bool SetLicNr(string licNr);
}