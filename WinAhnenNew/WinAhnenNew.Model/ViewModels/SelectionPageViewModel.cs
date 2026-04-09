using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using WinAhnenNew.Messages;
using WinAhnenNew.Services;

namespace WinAhnenNew.ViewModels
{
    /// <summary>
    /// View model for selecting the person that should be edited.
    /// </summary>
    public sealed partial class SelectionPageViewModel : ViewModelBase
    {
        private const bool _xDiagnosticsEnabled = false;
        private readonly IPersonSelectionService _personSelectionService;
        private readonly ListCollectionView _personsView;
        private readonly IMessenger _messenger;

        public SelectionPageViewModel(IPersonSelectionService personSelectionService, IMessenger messenger)
        {
            _personSelectionService = personSelectionService;
            _messenger = messenger;

            SortFields =
            [
                "Name",
                "Vorname",
                "ID",
                "Geburtsort",
                "Geburtsdatum"
            ];

            Persons = new ObservableCollection<SelectionPersonItemViewModel>(CreatePersonItems());
            _personsView = (ListCollectionView)CollectionViewSource.GetDefaultView(Persons);
            PersonsView = _personsView;
            PersonsView.Filter = FilterPerson;

            SelectedSortField = SortFields.FirstOrDefault() ?? "Name";
            ApplySorting();
            UpdateFilteredPersonCount();
            SelectionStatusText = "Bitte wählen Sie eine Person zur Bearbeitung aus.";

            _messenger.Register<GenealogyChangedMessage>(this, static (objRecipient, msgMessage) =>
            {
                if (objRecipient is SelectionPageViewModel vmSelection)
                {
                    vmSelection.ReloadPersons(msgMessage.Value);
                }
            });
        }

        public ObservableCollection<SelectionPersonItemViewModel> Persons { get; }

        public ObservableCollection<string> SelectedPersonFacts { get; } = [];

        public bool IsDiagnosticsEnabled => _xDiagnosticsEnabled;

        public ICollectionView PersonsView { get; }

        public ObservableCollection<string> SortFields { get; }

        [ObservableProperty]
        private string _filterText = string.Empty;

        [ObservableProperty]
        private bool _onlyLiving;

        [ObservableProperty]
        private string _selectedSortField = string.Empty;

        [ObservableProperty]
        private bool _sortDescending;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SelectPersonCommand))]
        [NotifyPropertyChangedFor(nameof(SelectedPersonId))]
        [NotifyPropertyChangedFor(nameof(SelectedPersonName))]
        [NotifyPropertyChangedFor(nameof(SelectedPersonBirthDate))]
        [NotifyPropertyChangedFor(nameof(SelectedPersonBirthPlace))]
        [NotifyPropertyChangedFor(nameof(SelectedPersonSex))]
        [NotifyPropertyChangedFor(nameof(IsPersonSelected))]
        private SelectionPersonItemViewModel? _selectedPerson;

        [ObservableProperty]
        private int _filteredPersonCount;

        [ObservableProperty]
        private string _selectionStatusText = string.Empty;

        public string SelectedPersonId => SelectedPerson?.PersonId ?? string.Empty;

        public string SelectedPersonName => SelectedPerson?.DisplayName ?? string.Empty;

        public string SelectedPersonBirthDate => SelectedPerson?.BirthDateText ?? string.Empty;

        public string SelectedPersonBirthPlace => SelectedPerson?.BirthPlace ?? string.Empty;

        public string SelectedPersonSex => SelectedPerson?.Sex ?? string.Empty;

        public bool IsPersonSelected => SelectedPerson is not null;

        partial void OnFilterTextChanged(string value)
        {
            PersonsView.Refresh();
            UpdateFilteredPersonCount();
        }

        partial void OnOnlyLivingChanged(bool value)
        {
            PersonsView.Refresh();
            UpdateFilteredPersonCount();
        }

        partial void OnSelectedSortFieldChanged(string value)
        {
            ApplySorting();
            UpdateFilteredPersonCount();
        }

        partial void OnSortDescendingChanged(bool value)
        {
            ApplySorting();
            UpdateFilteredPersonCount();
        }

        partial void OnSelectedPersonChanged(SelectionPersonItemViewModel? value)
        {
            UpdateSelectedPersonFacts(value);

            SelectionStatusText = value is null
                ? "Bitte wählen Sie eine Person zur Bearbeitung aus."
                : $"Ausgewählt: {value.DisplayName} (ID {value.PersonId})";
        }

        [RelayCommand]
        private void ResetFilter()
        {
            FilterText = string.Empty;
            OnlyLiving = false;
            SortDescending = false;
            SelectedSortField = SortFields.FirstOrDefault() ?? "Name";
        }

        [RelayCommand(CanExecute = nameof(CanSelectPerson))]
        private void SelectPerson()
        {
            if (SelectedPerson is null)
            {
                return;
            }

            _personSelectionService.SetSelectedPerson(SelectedPerson.Person);
            SelectionStatusText = $"Die Person {SelectedPerson.DisplayName} ist für die Bearbeitung ausgewählt.";
            _messenger.Send(new NavigateToEditTabMessage());
        }

        private bool CanSelectPerson()
        {
            return SelectedPerson is not null;
        }

        private bool FilterPerson(object obj)
        {
            if (obj is not SelectionPersonItemViewModel vmPerson)
            {
                return false;
            }

            if (OnlyLiving && !vmPerson.IsLiving)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(FilterText))
            {
                return true;
            }

            var sFilter = FilterText.Trim();
            return vmPerson.PersonId.Contains(sFilter, StringComparison.OrdinalIgnoreCase)
                || vmPerson.Surname.Contains(sFilter, StringComparison.OrdinalIgnoreCase)
                || vmPerson.GivenName.Contains(sFilter, StringComparison.OrdinalIgnoreCase)
                || vmPerson.DisplayName.Contains(sFilter, StringComparison.OrdinalIgnoreCase)
                || vmPerson.BirthPlace.Contains(sFilter, StringComparison.OrdinalIgnoreCase)
                || vmPerson.BirthDateText.Contains(sFilter, StringComparison.OrdinalIgnoreCase);
        }

        private void ApplySorting()
        {
            _personsView.CustomSort = new PersonSortComparer(SelectedSortField, SortDescending);
            _personsView.Refresh();
        }

        private void UpdateFilteredPersonCount()
        {
            FilteredPersonCount = PersonsView.Cast<object>().Count();
        }

        private void ReloadPersons(int iPersonCount)
        {
            Persons.Clear();
            foreach (var vmPerson in CreatePersonItems())
            {
                Persons.Add(vmPerson);
            }

            SelectedPerson = null;
            UpdateSelectedPersonFacts(null);
            PersonsView.Refresh();
            ApplySorting();
            UpdateFilteredPersonCount();
            SelectionStatusText = $"Demo-Genealogie mit {iPersonCount} Personen wurde erstellt.";
        }

        private SelectionPersonItemViewModel[] CreatePersonItems()
        {
            return _personSelectionService
                .GetSelectablePersons()
                .Select(genPerson => new SelectionPersonItemViewModel(genPerson))
                .ToArray();
        }

        private void UpdateSelectedPersonFacts(SelectionPersonItemViewModel? vmPerson)
        {
            SelectedPersonFacts.Clear();

            if (vmPerson?.Person is null)
            {
                SelectedPersonFacts.Add("Keine Person ausgewählt.");
                return;
            }

            var lstFacts = vmPerson.Person.Facts
                .Where(genFact => genFact is not null)
                .Select(genFact => genFact!)
                .OrderBy(genFact => genFact.eFactType)
                .ThenBy(genFact => genFact.ID)
                .ToArray();

            if (lstFacts.Length == 0)
            {
                SelectedPersonFacts.Add("Keine Fakten/Ereignisse vorhanden.");
                return;
            }

            foreach (var genFact in lstFacts)
            {
                var sDate = string.IsNullOrWhiteSpace(genFact.Date?.DateText)
                    ? (genFact.Date?.Date1 is DateTime dtDate && dtDate != DateTime.MinValue
                        ? dtDate.ToString("dd.MM.yyyy", CultureInfo.CurrentCulture)
                        : string.Empty)
                    : genFact.Date!.DateText!;

                var sPlace = genFact.Place?.Name ?? string.Empty;
                var sData = genFact.Data ?? string.Empty;
                SelectedPersonFacts.Add($"{genFact.eFactType}: Data='{sData}', Date='{sDate}', Place='{sPlace}'");
            }
        }

        private sealed class PersonSortComparer : IComparer
        {
            private readonly string _selectedSortField;
            private readonly int _sortDirection;

            public PersonSortComparer(string selectedSortField, bool sortDescending)
            {
                _selectedSortField = selectedSortField;
                _sortDirection = sortDescending ? -1 : 1;
            }

            public int Compare(object? x, object? y)
            {
                var vmLeft = x as SelectionPersonItemViewModel;
                var vmRight = y as SelectionPersonItemViewModel;

                if (ReferenceEquals(vmLeft, vmRight))
                {
                    return 0;
                }

                if (vmLeft is null)
                {
                    return -1;
                }

                if (vmRight is null)
                {
                    return 1;
                }

                int iResult = _selectedSortField switch
                {
                    "Vorname" => CompareText(vmLeft.GivenName, vmRight.GivenName),
                    "ID" => vmLeft.PersonNumber.CompareTo(vmRight.PersonNumber),
                    "Geburtsort" => CompareText(vmLeft.BirthPlace, vmRight.BirthPlace),
                    "Geburtsdatum" => Nullable.Compare(vmLeft.BirthDateValue, vmRight.BirthDateValue),
                    _ => CompareText(vmLeft.Surname, vmRight.Surname)
                };

                if (iResult == 0)
                {
                    iResult = CompareText(vmLeft.Surname, vmRight.Surname);
                }

                if (iResult == 0)
                {
                    iResult = CompareText(vmLeft.GivenName, vmRight.GivenName);
                }

                if (iResult == 0)
                {
                    iResult = vmLeft.PersonNumber.CompareTo(vmRight.PersonNumber);
                }

                return iResult * _sortDirection;
            }

            private static int CompareText(string? left, string? right)
                => StringComparer.CurrentCultureIgnoreCase.Compare(left ?? string.Empty, right ?? string.Empty);
        }
    }
}
