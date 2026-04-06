using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GenInterfaces.Interfaces.Genealogic;
using WinAhnenNew.Services;

namespace WinAhnenNew.ViewModels
{
    /// <summary>
    /// View model for selecting the person that should be edited.
    /// </summary>
    public sealed partial class SelectionPageViewModel : ViewModelBase
    {
        private readonly IPersonSelectionService _personSelectionService;
        private readonly ListCollectionView _personsView;

        public SelectionPageViewModel(IPersonSelectionService personSelectionService)
        {
            _personSelectionService = personSelectionService;

            SortFields =
            [
                "Name",
                "Vorname",
                "ID",
                "Geburtsort",
                "Geburtsdatum"
            ];

            Persons = new ObservableCollection<IGenPerson>(_personSelectionService.GetSelectablePersons());
            _personsView = (ListCollectionView)CollectionViewSource.GetDefaultView(Persons);
            PersonsView = _personsView;
            PersonsView.Filter = FilterPerson;

            SelectedSortField = SortFields.FirstOrDefault() ?? "Name";
            ApplySorting();
            UpdateFilteredPersonCount();
            SelectionStatusText = "Bitte wählen Sie eine Person zur Bearbeitung aus.";
        }

        public ObservableCollection<IGenPerson> Persons { get; }

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
        private IGenPerson? _selectedPerson;

        [ObservableProperty]
        private int _filteredPersonCount;

        [ObservableProperty]
        private string _selectionStatusText = string.Empty;

        public string SelectedPersonId => SelectedPerson is null
            ? string.Empty
            : (!string.IsNullOrWhiteSpace(SelectedPerson.IndRefID)
                ? SelectedPerson.IndRefID
                : SelectedPerson.ID.ToString(CultureInfo.CurrentCulture));

        public string SelectedPersonName => GetPersonDisplayName(SelectedPerson);

        public string SelectedPersonBirthDate => GetBirthDateText(SelectedPerson);

        public string SelectedPersonBirthPlace => SelectedPerson?.BirthPlace?.Name ?? string.Empty;

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

        partial void OnSelectedPersonChanged(IGenPerson? value)
        {          

            SelectionStatusText = value is null
                ? "Bitte wählen Sie eine Person zur Bearbeitung aus."
                : $"Ausgewählt: {GetPersonDisplayName(value)} (ID {SelectedPersonId})";
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

            SelectionStatusText = $"Die Person {GetPersonDisplayName(SelectedPerson)} ist für die Bearbeitung ausgewählt.";
        }

        private bool CanSelectPerson()
        {
            return SelectedPerson is not null;
        }

        private bool FilterPerson(object obj)
        {
            if (obj is not IGenPerson genPerson)
            {
                return false;
            }

            if (OnlyLiving && !IsLiving(genPerson))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(FilterText))
            {
                return true;
            }

            var sFilter = FilterText.Trim();
            var sBirthPlace = genPerson.BirthPlace?.Name ?? string.Empty;
            var sBirthDate = GetBirthDateText(genPerson);
            var sPersonId = !string.IsNullOrWhiteSpace(genPerson.IndRefID)
                ? genPerson.IndRefID
                : genPerson.ID.ToString(CultureInfo.CurrentCulture);

            return sPersonId.Contains(sFilter, StringComparison.OrdinalIgnoreCase)
                || (genPerson.Surname?.Contains(sFilter, StringComparison.OrdinalIgnoreCase) ?? false)
                || (genPerson.GivenName?.Contains(sFilter, StringComparison.OrdinalIgnoreCase) ?? false)
                || (genPerson.Name?.Contains(sFilter, StringComparison.OrdinalIgnoreCase) ?? false)
                || sBirthPlace.Contains(sFilter, StringComparison.OrdinalIgnoreCase)
                || sBirthDate.Contains(sFilter, StringComparison.OrdinalIgnoreCase);
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

        private static string GetPersonDisplayName(IGenPerson? genPerson)
        {
            if (genPerson is null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(genPerson.Surname) || !string.IsNullOrWhiteSpace(genPerson.GivenName))
            {
                return $"{genPerson.Surname}, {genPerson.GivenName}".Trim(' ', ',');
            }

            return genPerson.Name ?? string.Empty;
        }

        private static string GetBirthDateText(IGenPerson? genPerson)
        {
            var genDate = genPerson?.BirthDate;
            if (genDate is null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(genDate.DateText))
            {
                return genDate.DateText;
            }

            return genDate.Date1 == DateTime.MinValue
                ? string.Empty
                : genDate.Date1.ToString("dd.MM.yyyy", CultureInfo.CurrentCulture);
        }

        private static bool IsLiving(IGenPerson genPerson)
            => genPerson.Death is null && genPerson.Burial is null;

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
                var genLeft = x as IGenPerson;
                var genRight = y as IGenPerson;

                if (ReferenceEquals(genLeft, genRight))
                {
                    return 0;
                }

                if (genLeft is null)
                {
                    return -1;
                }

                if (genRight is null)
                {
                    return 1;
                }

                int iResult = _selectedSortField switch
                {
                    "Vorname" => CompareText(genLeft.GivenName, genRight.GivenName),
                    "ID" => genLeft.ID.CompareTo(genRight.ID),
                    "Geburtsort" => CompareText(genLeft.BirthPlace?.Name, genRight.BirthPlace?.Name),
                    "Geburtsdatum" => Nullable.Compare(
                        genLeft.BirthDate?.Date1 == DateTime.MinValue ? null : genLeft.BirthDate?.Date1,
                        genRight.BirthDate?.Date1 == DateTime.MinValue ? null : genRight.BirthDate?.Date1),
                    _ => CompareText(genLeft.Surname, genRight.Surname)
                };

                if (iResult == 0)
                {
                    iResult = CompareText(genLeft.Surname, genRight.Surname);
                }

                if (iResult == 0)
                {
                    iResult = CompareText(genLeft.GivenName, genRight.GivenName);
                }

                if (iResult == 0)
                {
                    iResult = genLeft.ID.CompareTo(genRight.ID);
                }

                return iResult * _sortDirection;
            }

            private static int CompareText(string? left, string? right)
                => StringComparer.CurrentCultureIgnoreCase.Compare(left ?? string.Empty, right ?? string.Empty);
        }
    }
}
