using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using GenFreeBrowser.Model;
using GenFreeBrowser.ViewModels.Interfaces;

namespace GenFreeBrowser.ViewModels;

public partial class KernFamilieViewModel : ObservableObject, IKernFamilieViewModel
{
    [ObservableProperty]
    private DispPersones? ausgewaehltePerson;

    [ObservableProperty]
    private DispPersones? ehePartner;

    private readonly ObservableCollection<DispPersones> _kinder = new();
    public ReadOnlyObservableCollection<DispPersones> Kinder { get; }

    public bool HatEhePartner => EhePartner != null;
    public bool HatKinder => _kinder.Count > 0;

    public KernFamilieViewModel()
    {
        Kinder = new ReadOnlyObservableCollection<DispPersones>(_kinder);
    }

    partial void OnAusgewaehltePersonChanged(DispPersones? value)
    {
        // Platzhalter-Logik: Ableiten eines Demo-Ehepartners und Kinder
        _kinder.Clear();
        EhePartner = null;
        if (value is null) return;
        EhePartner = new DispPersones(value.Id + 1000, $"Partner von {value.Vollname}", value.GeburtsDatum);
        _kinder.Add(new DispPersones(value.Id + 2000, $"Kind 1 von {value.Vollname}", value.GeburtsDatum));
        _kinder.Add(new DispPersones(value.Id + 2001, $"Kind 2 von {value.Vollname}", value.GeburtsDatum));
        OnPropertyChanged(nameof(HatEhePartner));
        OnPropertyChanged(nameof(HatKinder));
    }

    public void LadeFamilie()
    {
        // Für echte Implementierung: Datenservice aufrufen.
        if (AusgewaehltePerson != null)
        {
            OnAusgewaehltePersonChanged(AusgewaehltePerson);
        }
    }
}
