using CommunityToolkit.Mvvm.ComponentModel;
using Gen_FreeWin.Data;
using Gen_FreeWin.Models;
using Gen_FreeWin.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Gen_FreeWin.ViewModels;

/// <summary>
/// Encapsulates the ownership selection workflow independently from the WinForms view.
/// </summary>
public partial class BesitzViewModel : ObservableObject, IBesitzViewModel
{
    private const string MissingAkteMessage = "Es sind noch keine Hof- und Grundakten eingegeben."
        + "\nEs muß erst ein Eintrag in der Hof- und Grundaktenverwaltung erfolgen,"
        + "\nBitte halten Sie sich an das Muster in der Anleitung."
        + "\nEine Verknüpfung kann erst erfolgen, wenn auch bei >Besitzwchsel< ein Eintrag vorhanden ist. ";

    private readonly IBesitzRepository _repository;
    private BesitzAkteDetails? _selectedAkte;
    private BesitzEntryDetails? _selectedEntry;
    private int _personNumber;

    /// <summary>
    /// Initializes a new instance of the <see cref="BesitzViewModel"/> class.
    /// </summary>
    /// <param name="repository">The repository used for table-based ownership access.</param>
    public BesitzViewModel(IBesitzRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public Action? RequestClose { get; set; }

    /// <inheritdoc />
    public Action<string>? ShowInformation { get; set; }

    /// <inheritdoc />
    public ObservableCollection<BesitzAkteListItem> AkteItems { get; } = new();

    /// <inheritdoc />
    public ObservableCollection<BesitzEntryListItem> EntryItems { get; } = new();

    private bool _akteSelectionVisible;
    private bool _entrySelectionVisible;
    private string _akteHintText = string.Empty;
    private string _entryHintText = string.Empty;
    private string _akteText = string.Empty;
    private string _verwaltungsortText = string.Empty;
    private string _bauernschaftText = string.Empty;
    private string _hofklasseText = string.Empty;
    private string _jahrText = string.Empty;
    private string _erbautText = string.Empty;
    private string _abgaengigText = string.Empty;
    private string _nameText = string.Empty;
    private string _gebaeudeartText = string.Empty;
    private string _flurText = string.Empty;
    private string _parzelleText = string.Empty;

    /// <inheritdoc />
    public bool AkteSelectionVisible
    {
        get => _akteSelectionVisible;
        private set
        {
            if (SetProperty(ref _akteSelectionVisible, value))
            {
                OnVisibilityStateChanged();
            }
        }
    }

    /// <inheritdoc />
    public bool EntrySelectionVisible
    {
        get => _entrySelectionVisible;
        private set
        {
            if (SetProperty(ref _entrySelectionVisible, value))
            {
                OnVisibilityStateChanged();
            }
        }
    }

    /// <inheritdoc />
    public string AkteHintText
    {
        get => _akteHintText;
        private set
        {
            if (SetProperty(ref _akteHintText, value))
            {
                OnPropertyChanged(nameof(AkteHintVisible));
            }
        }
    }

    /// <inheritdoc />
    public string EntryHintText
    {
        get => _entryHintText;
        private set
        {
            if (SetProperty(ref _entryHintText, value))
            {
                OnPropertyChanged(nameof(EntryHintVisible));
            }
        }
    }

    /// <inheritdoc />
    public string AkteText
    {
        get => _akteText;
        private set => SetProperty(ref _akteText, value);
    }

    /// <inheritdoc />
    public string VerwaltungsortText
    {
        get => _verwaltungsortText;
        private set => SetProperty(ref _verwaltungsortText, value);
    }

    /// <inheritdoc />
    public string BauernschaftText
    {
        get => _bauernschaftText;
        private set => SetProperty(ref _bauernschaftText, value);
    }

    /// <inheritdoc />
    public string HofklasseText
    {
        get => _hofklasseText;
        private set => SetProperty(ref _hofklasseText, value);
    }

    /// <inheritdoc />
    public string JahrText
    {
        get => _jahrText;
        private set => SetProperty(ref _jahrText, value);
    }

    /// <inheritdoc />
    public string ErbautText
    {
        get => _erbautText;
        private set => SetProperty(ref _erbautText, value);
    }

    /// <inheritdoc />
    public string AbgaengigText
    {
        get => _abgaengigText;
        private set => SetProperty(ref _abgaengigText, value);
    }

    /// <inheritdoc />
    public string NameText
    {
        get => _nameText;
        private set => SetProperty(ref _nameText, value);
    }

    /// <inheritdoc />
    public string GebaeudeartText
    {
        get => _gebaeudeartText;
        private set => SetProperty(ref _gebaeudeartText, value);
    }

    /// <inheritdoc />
    public string FlurText
    {
        get => _flurText;
        private set => SetProperty(ref _flurText, value);
    }

    /// <inheritdoc />
    public string ParzelleText
    {
        get => _parzelleText;
        private set => SetProperty(ref _parzelleText, value);
    }

    /// <inheritdoc />
    public bool AkteHintVisible => AkteSelectionVisible && !string.IsNullOrWhiteSpace(AkteHintText);

    /// <inheritdoc />
    public bool EntryHintVisible => EntrySelectionVisible && !string.IsNullOrWhiteSpace(EntryHintText);

    /// <inheritdoc />
    public bool DeleteVisible => _selectedEntry is not null;

    /// <inheritdoc />
    public bool CancelVisible => _selectedEntry is not null && EntrySelectionVisible == false;

    /// <inheritdoc />
    public bool ConfirmVisible => _selectedEntry is not null;

    /// <inheritdoc />
    public void Load(int selectedEntryNumber, int personNumber)
    {
        _personNumber = personNumber;
        ResetState();

        if (!_repository.HasAkteRecords())
        {
            ShowInformation?.Invoke(MissingAkteMessage);
            RequestClose?.Invoke();
            return;
        }

        if (selectedEntryNumber > 0)
        {
            ApplyEntry(_repository.LoadEntryDetails(selectedEntryNumber));
            ApplyAkte(_repository.LoadAkteDetailsByAkte(_selectedEntry!.Akte));
            AkteSelectionVisible = false;
            EntrySelectionVisible = false;
            OnPropertyChanged(nameof(DeleteVisible));
            OnPropertyChanged(nameof(CancelVisible));
            OnPropertyChanged(nameof(ConfirmVisible));
            return;
        }

        AkteHintText = "Vorhandene Akten; Bitte auswählen";
        AkteSelectionVisible = true;
        SynchronizeCollection(AkteItems, _repository.LoadAkteList());
    }

    /// <inheritdoc />
    public void SelectAkte(BesitzAkteListItem? item)
    {
        if (item is null)
        {
            return;
        }

        ApplyAkte(_repository.LoadAkteDetailsByRecordNumber(item.RecordNumber));
        SynchronizeCollection(EntryItems, _repository.LoadEntriesForAkte(_selectedAkte!.Akte));

        AkteSelectionVisible = false;
        AkteHintText = string.Empty;
        EntrySelectionVisible = true;
        EntryHintText = EntryItems.Count == 0 ? "kein Eintrag gefunden" : "Eintrag (Jahr) wählen";
    }

    /// <inheritdoc />
    public void SelectEntry(BesitzEntryListItem? item)
    {
        if (item is null)
        {
            return;
        }

        ApplyEntry(_repository.LoadEntryDetails(item.RecordNumber));
        EntryHintText = string.Empty;
        EntrySelectionVisible = false;
        OnPropertyChanged(nameof(DeleteVisible));
        OnPropertyChanged(nameof(CancelVisible));
        OnPropertyChanged(nameof(ConfirmVisible));
    }

    /// <inheritdoc />
    public void ConfirmSelection()
    {
        if (_selectedEntry is null || _selectedAkte is null)
        {
            return;
        }

        _repository.AddPropertyLink(_selectedEntry.RecordNumber, _selectedAkte.Akte, _personNumber);
        RequestClose?.Invoke();
    }

    /// <inheritdoc />
    public void RemoveSelection()
    {
        if (_selectedEntry is null || _selectedAkte is null)
        {
            return;
        }

        _repository.RemovePropertyLink(_selectedEntry.RecordNumber, _selectedAkte.Akte, _personNumber);
        RequestClose?.Invoke();
    }

    private void ResetState()
    {
        _selectedAkte = null;
        _selectedEntry = null;
        AkteText = string.Empty;
        VerwaltungsortText = string.Empty;
        BauernschaftText = string.Empty;
        HofklasseText = string.Empty;
        JahrText = string.Empty;
        ErbautText = string.Empty;
        AbgaengigText = string.Empty;
        NameText = string.Empty;
        GebaeudeartText = string.Empty;
        FlurText = string.Empty;
        ParzelleText = string.Empty;
        AkteHintText = string.Empty;
        EntryHintText = string.Empty;
        AkteSelectionVisible = false;
        EntrySelectionVisible = false;
        SynchronizeCollection(AkteItems, Array.Empty<BesitzAkteListItem>());
        SynchronizeCollection(EntryItems, Array.Empty<BesitzEntryListItem>());
        OnPropertyChanged(nameof(DeleteVisible));
        OnPropertyChanged(nameof(CancelVisible));
        OnPropertyChanged(nameof(ConfirmVisible));
    }

    private void ApplyAkte(BesitzAkteDetails akte)
    {
        _selectedAkte = akte;
        AkteText = akte.Akte;
        VerwaltungsortText = akte.Kirchspiel;
        BauernschaftText = akte.Beschreibung;
        HofklasseText = akte.Hofklasse;
        FlurText = akte.FlurText;
        ParzelleText = akte.ParzelleText;
    }

    private void ApplyEntry(BesitzEntryDetails entry)
    {
        _selectedEntry = entry;
        JahrText = entry.Jahr;
        ErbautText = entry.Erbaut;
        AbgaengigText = entry.Abgaengig;
        NameText = entry.Name;
        GebaeudeartText = entry.Gebaeudeart;
    }

    private void OnVisibilityStateChanged()
    {
        OnPropertyChanged(nameof(AkteHintVisible));
        OnPropertyChanged(nameof(EntryHintVisible));
        OnPropertyChanged(nameof(DeleteVisible));
        OnPropertyChanged(nameof(CancelVisible));
        OnPropertyChanged(nameof(ConfirmVisible));
    }

    private static void SynchronizeCollection<T>(ObservableCollection<T> target, IReadOnlyList<T> source)
    {
        if (source.Count == 0)
        {
            if (target.Count > 0)
            {
                target.Clear();
            }

            return;
        }

        for (int i = 0; i < source.Count; i++)
        {
            if (i < target.Count)
            {
                if (!EqualityComparer<T>.Default.Equals(target[i], source[i]))
                {
                    target[i] = source[i];
                }
            }
            else
            {
                target.Add(source[i]);
            }
        }

        while (target.Count > source.Count)
        {
            target.RemoveAt(target.Count - 1);
        }
    }
}
