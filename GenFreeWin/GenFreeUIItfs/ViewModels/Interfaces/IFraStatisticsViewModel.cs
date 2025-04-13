﻿using System.ComponentModel;

namespace Gen_FreeWin.ViewModels.Interfaces;

public interface IFraStatisticsViewModel: INotifyPropertyChanged
{
    public int Persons { get; }

    public int Families { get;  }

    public int Places { get; }

    public int Dates { get; }

    public int Texts { get; }
}