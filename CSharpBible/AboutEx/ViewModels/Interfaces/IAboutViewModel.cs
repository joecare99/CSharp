using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;

namespace CSharpBible.AboutEx.ViewModels.Interfaces;

public interface IAboutViewModel: INotifyPropertyChanged
{
    string Title { get; }
    string Version { get; }
    string Description { get; }
    string Author { get; }
    string Company { get; }
    string Copyright { get; }
    string Product { get; }

    IRelayCommand CloseCommand { get; }

    void SetData(string[] strings);
}
