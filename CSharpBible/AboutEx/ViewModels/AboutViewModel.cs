using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpBible.AboutEx.ViewModels.Interfaces;
using System;

namespace CSharpBible.AboutEx.ViewModels;

public partial class AboutViewModel : ObservableObject, IAboutViewModel
{
    [ObservableProperty]
    public partial string Title { get; private set; }

    [ObservableProperty]
    public partial string Version { get; private set; }

    [ObservableProperty]
    public partial string Description { get; private set; }

    [ObservableProperty]
    public partial string Author { get; private set; }

    [ObservableProperty]
    public partial string Company { get; private set; }

    [ObservableProperty]
    public partial string Copyright { get; private set; }

    [ObservableProperty]
    public partial string Product { get; private set; }

    public void SetData(string[] strings)
    {
        Title = strings[0];
        if (strings.Length < 2) return;
        Version = strings[1];
        if (strings.Length < 3) return;
        Description = strings[2];
        if (strings.Length < 4) return;
        Author = strings[3];
        if (strings.Length < 5) return;
        Company = strings[4];
        if (strings.Length < 6) return;
        Copyright = strings[5];
        if (strings.Length < 7) return;
        Product = strings[6];
    }

    [RelayCommand]
    private void Close() => throw new NotImplementedException();
}
