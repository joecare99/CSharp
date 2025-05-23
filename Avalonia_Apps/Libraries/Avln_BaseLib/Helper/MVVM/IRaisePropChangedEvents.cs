﻿using System.ComponentModel;

namespace BaseLib.Helper.MVVM;

public interface IRaisePropChangedEvents : INotifyPropertyChanged
{
    void OnPropertyChanged(string? propertyName);
}