using System;
using System.Windows.Input;

namespace Avln_Brushes.ViewModels.Interfaces;

public interface ISampleViewerViewModel
{
    event EventHandler DoExit;
   ICommand? TransitionCommand { get; }
}