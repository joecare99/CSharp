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

    public string SelectedVital => BuildVital(AusgewaehltePerson);
    public string PartnerVital => BuildVital(EhePartner);

    // Header text: parents of the selected person
    public string ParentsHeader => AusgewaehltePerson is null
        ? "Eltern der Person"
        : $"Eltern von {AusgewaehltePerson.Vollname}";

    public KernFamilieViewModel()
    {
        Kinder = new ReadOnlyObservableCollection<DispPersones>(_kinder);
    }

    partial void OnAusgewaehltePersonChanged(DispPersones? value)
    {
        _kinder.Clear();
        EhePartner = null;
        OnPropertyChanged(nameof(SelectedVital));
        OnPropertyChanged(nameof(ParentsHeader));
        if (value is null)
        {
            OnPropertyChanged(nameof(HatEhePartner));
            OnPropertyChanged(nameof(HatKinder));
            return;
        }
        // Demo-Daten: 1 Partner, 2 Kinder
        EhePartner = new DispPersones(value.Id + 1000, $"Partner von {value.Vollname}", value.GeburtsDatum);
        _kinder.Add(new DispPersones(value.Id + 2000, $"Kind 1 von {value.Vollname}", value.GeburtsDatum));
        _kinder.Add(new DispPersones(value.Id + 2001, $"Kind 2 von {value.Vollname}", value.GeburtsDatum));
        OnPropertyChanged(nameof(HatEhePartner));
        OnPropertyChanged(nameof(HatKinder));
    }

    partial void OnEhePartnerChanged(DispPersones? value)
    {
        OnPropertyChanged(nameof(PartnerVital));
        OnPropertyChanged(nameof(HatEhePartner));
    }

    private static string BuildVital(DispPersones? p)
    {
        if (p is null) return string.Empty;
        var born = p.GeburtsDatum?.ToString("dd MMM yyyy") ?? "?";
        var died = p.SterbeDatum?.ToString("dd MMM yyyy") ?? string.Empty;
        return string.IsNullOrEmpty(died) ? $"Born: {born}" : $"Born: {born}  –  Died: {died}";
    }

    public void LadeFamilie()
    {
        if (AusgewaehltePerson != null)
        {
            OnAusgewaehltePersonChanged(AusgewaehltePerson);
        }
    }
}
