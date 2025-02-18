using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpBible.AboutEx.ViewModels.Interfaces;

public interface IFrmAboutExMainViewModel: INotifyPropertyChanged
{
    Action<string[]> ShowAboutFrm1 { get; set; }
    Action<string[]> ShowAboutFrm2 { get; set; }

    IRelayCommand ShowAbout1Command { get; }
    IRelayCommand ShowAbout2Command { get; }
}
