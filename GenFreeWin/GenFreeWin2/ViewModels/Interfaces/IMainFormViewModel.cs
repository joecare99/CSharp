using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace GenFreeWin2.ViewModels.Interfaces;

public interface IMainFormViewModel: INotifyPropertyChanged
{
    Control View { get; set; }
    Action<Control> ShowDialog { get; set; }
}