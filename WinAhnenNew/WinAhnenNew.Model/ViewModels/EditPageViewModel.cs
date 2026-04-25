using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using BaseGenClasses.Helper;
using BaseGenClasses.Model;
using BaseGenClasses.Persistence;
using BaseGenClasses.Persistence.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
using WinAhnenNew.Messages;
using WinAhnenNew.Services;

namespace WinAhnenNew.ViewModels
{
    /// <summary>
    /// View model for the edit tab page.
    /// </summary>
    public sealed partial class EditPageViewModel : ViewModelBase
    {
        private static readonly string[] _arrDefaultReligions = ["ev.", "rk.", "rf.", "lt."];
        private readonly IPersonSelectionService _personSelectionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditPageViewModel"/> class.
        /// </summary>
        /// <param name="personSelectionService">The shared person selection service.</param>
        /// <param name="messenger">The shared application messenger.</param>
        public EditPageViewModel(IPersonSelectionService personSelectionService, IMessenger messenger)
        {
            _personSelectionService = personSelectionService;

            LastNameOptions = [];
            ReligionOptions = [];
            AdoptedByOptions = [];
            BirthPlaceOptions = [];
            BaptismPlaceOptions = [];
            DeathPlaceOptions = [];
            BurialPlaceOptions = [];
            Images = [];
            SpousesAndWeddings = [];
            Children = [];

            LoadOptionLists();
            ApplySelectedPerson(_personSelectionService.SelectedPerson);

            messenger.Register<SelectedPersonChangedMessage>(this, static (objRecipient, msgMessage) =>
            {
                if (objRecipient is EditPageViewModel vmEdit)
                {
                    vmEdit.ApplySelectedPerson(msgMessage.Value);
                }
            });

            messenger.Register<GenealogyChangedMessage>(this, static (objRecipient, _) =>
            {
                if (objRecipient is EditPageViewModel vmEdit)
                {
                    vmEdit.LoadOptionLists();
                    vmEdit.ApplySelectedPerson(vmEdit._personSelectionService.SelectedPerson);
                }
            });
        }

        public ObservableCollection<string> LastNameOptions { get; }

        public ObservableCollection<string> ReligionOptions { get; }

        public ObservableCollection<string> AdoptedByOptions { get; }

        public ObservableCollection<string> BirthPlaceOptions { get; }

        public ObservableCollection<string> BaptismPlaceOptions { get; }

        public ObservableCollection<string> DeathPlaceOptions { get; }

        public ObservableCollection<string> BurialPlaceOptions { get; }

        public ObservableCollection<EditImageItemViewModel> Images { get; }

        public ObservableCollection<EditSpouseWeddingItemViewModel> SpousesAndWeddings { get; }

        public ObservableCollection<EditChildItemViewModel> Children { get; }

        [ObservableProperty]
        private string _personNumber = string.Empty;

        [ObservableProperty]
        private bool _isSelectPersonToggled;

        [ObservableProperty]
        private string _father = string.Empty;

        [ObservableProperty]
        private string _mother = string.Empty;

        [ObservableProperty]
        private string _id = string.Empty;

        [ObservableProperty]
        private string _additionalInfo = string.Empty;

        [ObservableProperty]
        private string _lastName = string.Empty;

        [ObservableProperty]
        private string _firstNames = string.Empty;

        [ObservableProperty]
        private string _occupation = string.Empty;

        [ObservableProperty]
        private string _farmName = string.Empty;

        [ObservableProperty]
        private string _gender = string.Empty;

        [ObservableProperty]
        private string _religion = string.Empty;

        [ObservableProperty]
        private bool _isAlive;

        [ObservableProperty]
        private string _adoptedBy = string.Empty;

        [ObservableProperty]
        private string _birthModifier = string.Empty;

        [ObservableProperty]
        private string _birthDay = "<dd>";

        [ObservableProperty]
        private string _birthMonth = "<MM>";

        [ObservableProperty]
        private string _birthYear = "<yyyy>";

        [ObservableProperty]
        private string _birthPlace = "<Birthplace>";

        [ObservableProperty]
        private string _birthPlaceHidden = "<Birthplace>";

        [ObservableProperty]
        private string _baptismModifier = string.Empty;

        [ObservableProperty]
        private string _baptismDay = string.Empty;

        [ObservableProperty]
        private string _baptismMonth = string.Empty;

        [ObservableProperty]
        private string _baptismYear = string.Empty;

        [ObservableProperty]
        private string _baptismPlace = string.Empty;

        [ObservableProperty]
        private string _baptismPlaceHidden = string.Empty;

        [ObservableProperty]
        private string _deathModifier = string.Empty;

        [ObservableProperty]
        private string _deathDay = string.Empty;

        [ObservableProperty]
        private string _deathMonth = string.Empty;

        [ObservableProperty]
        private string _deathYear = string.Empty;

        [ObservableProperty]
        private string _deathPlace = string.Empty;

        [ObservableProperty]
        private string _burialModifier = string.Empty;

        [ObservableProperty]
        private string _burialDay = string.Empty;

        [ObservableProperty]
        private string _burialMonth = string.Empty;

        [ObservableProperty]
        private string _burialYear = string.Empty;

        [ObservableProperty]
        private string _burialPlace = string.Empty;

        [ObservableProperty]
        private ImageSource? _personImageSource;

        [ObservableProperty]
        private string _notes = string.Empty;

        [ObservableProperty]
        private string _imagesHeader = "Bilder";

        [ObservableProperty]
        private string _spousesHeader = "Ehepartner";

        [ObservableProperty]
        private string _weddingDataHeader = "Hochzeitsdaten";

        [ObservableProperty]
        private string _childrenHeader = "Kinder";

        private void LoadOptionLists()
        {
            var lstPersons = _personSelectionService.GetSelectablePersons();

            UpdateStringCollection(
                LastNameOptions,
                lstPersons
                    .Select(genPerson => genPerson.Surname)
                    .Where(sValue => !string.IsNullOrWhiteSpace(sValue))
                    .Cast<string>()
                    .Distinct(StringComparer.CurrentCultureIgnoreCase)
                    .OrderBy(sValue => sValue, StringComparer.CurrentCultureIgnoreCase));

            UpdateStringCollection(
                ReligionOptions,
                _arrDefaultReligions.Concat(
                    lstPersons
                        .Select(genPerson => genPerson.Religion)
                        .Where(sValue => !string.IsNullOrWhiteSpace(sValue))
                        .Cast<string>()
                        .OrderBy(sValue => sValue, StringComparer.CurrentCultureIgnoreCase))
                    .Distinct(StringComparer.CurrentCultureIgnoreCase));

            UpdateStringCollection(
                AdoptedByOptions,
                lstPersons
                    .Select(FormatPersonName)
                    .Where(sValue => !string.IsNullOrWhiteSpace(sValue))
                    .Distinct(StringComparer.CurrentCultureIgnoreCase)
                    .OrderBy(sValue => sValue, StringComparer.CurrentCultureIgnoreCase));

            UpdateStringCollection(BirthPlaceOptions, GetPlaceOptions(lstPersons, static genPerson => genPerson.BirthPlace?.Name));
            UpdateStringCollection(BaptismPlaceOptions, GetPlaceOptions(lstPersons, static genPerson => genPerson.BaptPlace?.Name));
            UpdateStringCollection(DeathPlaceOptions, GetPlaceOptions(lstPersons, static genPerson => genPerson.DeathPlace?.Name));
            UpdateStringCollection(BurialPlaceOptions, GetPlaceOptions(lstPersons, static genPerson => genPerson.BurialPlace?.Name));
        }

        #pragma warning disable MVVMTK0034
        private void ApplySelectedPerson(IGenPerson? genSelectedPerson)
        {
            if (genSelectedPerson is null)
            {
                ClearSelectedPerson();
                return;
            }

            SetProperty(ref _personNumber, genSelectedPerson.ID.ToString(CultureInfo.CurrentCulture), "PersonNumber");
            SetProperty(ref _isSelectPersonToggled, true, "IsSelectPersonToggled");
            SetProperty(ref _father, FormatPersonName(genSelectedPerson.Father), "Father");
            SetProperty(ref _mother, FormatPersonName(genSelectedPerson.Mother), "Mother");
            SetProperty(
                ref _id,
                !string.IsNullOrWhiteSpace(genSelectedPerson.IndRefID)
                    ? genSelectedPerson.IndRefID!
                    : genSelectedPerson.ID.ToString(CultureInfo.CurrentCulture),
                "Id");
            SetProperty(ref _additionalInfo, GetFactData(genSelectedPerson, EFactType.Info), "AdditionalInfo");
            SetProperty(ref _lastName, genSelectedPerson.Surname ?? string.Empty, "LastName");
            SetProperty(ref _firstNames, genSelectedPerson.GivenName ?? string.Empty, "FirstNames");
            SetProperty(ref _occupation, genSelectedPerson.Occupation ?? GetFactData(genSelectedPerson, EFactType.Occupation), "Occupation");
            SetProperty(ref _farmName, GetFactData(genSelectedPerson, EFactType.Property), "FarmName");
            SetProperty(ref _gender, genSelectedPerson.Sex, "Gender");
            SetProperty(ref _religion, genSelectedPerson.Religion ?? GetFactData(genSelectedPerson, EFactType.Religion), "Religion");
            SetProperty(ref _isAlive, genSelectedPerson.Death is null && genSelectedPerson.Burial is null, "IsAlive");
            SetProperty(ref _adoptedBy, GetFactData(genSelectedPerson, EFactType.Adoption), "AdoptedBy");
            SetProperty(ref _notes, GetFactData(genSelectedPerson, EFactType.Description), "Notes");
            SetProperty(ref _personImageSource, null, "PersonImageSource");

            ApplyDateValues(genSelectedPerson.BirthDate, true, static (vmEdit, sModifier, sDay, sMonth, sYear) =>
            {
                vmEdit.SetProperty(ref vmEdit._birthModifier, sModifier, "BirthModifier");
                vmEdit.SetProperty(ref vmEdit._birthDay, sDay, "BirthDay");
                vmEdit.SetProperty(ref vmEdit._birthMonth, sMonth, "BirthMonth");
                vmEdit.SetProperty(ref vmEdit._birthYear, sYear, "BirthYear");
            });

            SetProperty(ref _birthPlace, genSelectedPerson.BirthPlace?.Name ?? "<Birthplace>", "BirthPlace");
            SetProperty(ref _birthPlaceHidden, _birthPlace, "BirthPlaceHidden");

            ApplyDateValues(genSelectedPerson.BaptDate, false, static (vmEdit, sModifier, sDay, sMonth, sYear) =>
            {
                vmEdit.SetProperty(ref vmEdit._baptismModifier, sModifier, "BaptismModifier");
                vmEdit.SetProperty(ref vmEdit._baptismDay, sDay, "BaptismDay");
                vmEdit.SetProperty(ref vmEdit._baptismMonth, sMonth, "BaptismMonth");
                vmEdit.SetProperty(ref vmEdit._baptismYear, sYear, "BaptismYear");
            });

            SetProperty(ref _baptismPlace, genSelectedPerson.BaptPlace?.Name ?? string.Empty, "BaptismPlace");
            SetProperty(ref _baptismPlaceHidden, _baptismPlace, "BaptismPlaceHidden");

            ApplyDateValues(genSelectedPerson.DeathDate, false, static (vmEdit, sModifier, sDay, sMonth, sYear) =>
            {
                vmEdit.SetProperty(ref vmEdit._deathModifier, sModifier, "DeathModifier");
                vmEdit.SetProperty(ref vmEdit._deathDay, sDay, "DeathDay");
                vmEdit.SetProperty(ref vmEdit._deathMonth, sMonth, "DeathMonth");
                vmEdit.SetProperty(ref vmEdit._deathYear, sYear, "DeathYear");
            });

            SetProperty(ref _deathPlace, genSelectedPerson.DeathPlace?.Name ?? string.Empty, "DeathPlace");

            ApplyDateValues(genSelectedPerson.BurialDate, false, static (vmEdit, sModifier, sDay, sMonth, sYear) =>
            {
                vmEdit.SetProperty(ref vmEdit._burialModifier, sModifier, "BurialModifier");
                vmEdit.SetProperty(ref vmEdit._burialDay, sDay, "BurialDay");
                vmEdit.SetProperty(ref vmEdit._burialMonth, sMonth, "BurialMonth");
                vmEdit.SetProperty(ref vmEdit._burialYear, sYear, "BurialYear");
            });

            SetProperty(ref _burialPlace, genSelectedPerson.BurialPlace?.Name ?? string.Empty, "BurialPlace");

            LoadSpousesAndWeddings(genSelectedPerson);
            LoadChildren(genSelectedPerson);
            Images.Clear();
        }

        private void ClearSelectedPerson()
        {
            SetProperty(ref _personNumber, string.Empty, "PersonNumber");
            SetProperty(ref _isSelectPersonToggled, false, "IsSelectPersonToggled");
            SetProperty(ref _father, string.Empty, "Father");
            SetProperty(ref _mother, string.Empty, "Mother");
            SetProperty(ref _id, string.Empty, "Id");
            SetProperty(ref _additionalInfo, string.Empty, "AdditionalInfo");
            SetProperty(ref _lastName, string.Empty, "LastName");
            SetProperty(ref _firstNames, string.Empty, "FirstNames");
            SetProperty(ref _occupation, string.Empty, "Occupation");
            SetProperty(ref _farmName, string.Empty, "FarmName");
            SetProperty(ref _gender, string.Empty, "Gender");
            SetProperty(ref _religion, string.Empty, "Religion");
            SetProperty(ref _isAlive, false, "IsAlive");
            SetProperty(ref _adoptedBy, string.Empty, "AdoptedBy");
            SetProperty(ref _birthModifier, string.Empty, "BirthModifier");
            SetProperty(ref _birthDay, "<dd>", "BirthDay");
            SetProperty(ref _birthMonth, "<MM>", "BirthMonth");
            SetProperty(ref _birthYear, "<yyyy>", "BirthYear");
            SetProperty(ref _birthPlace, "<Birthplace>", "BirthPlace");
            SetProperty(ref _birthPlaceHidden, _birthPlace, "BirthPlaceHidden");
            SetProperty(ref _baptismModifier, string.Empty, "BaptismModifier");
            SetProperty(ref _baptismDay, string.Empty, "BaptismDay");
            SetProperty(ref _baptismMonth, string.Empty, "BaptismMonth");
            SetProperty(ref _baptismYear, string.Empty, "BaptismYear");
            SetProperty(ref _baptismPlace, string.Empty, "BaptismPlace");
            SetProperty(ref _baptismPlaceHidden, string.Empty, "BaptismPlaceHidden");
            SetProperty(ref _deathModifier, string.Empty, "DeathModifier");
            SetProperty(ref _deathDay, string.Empty, "DeathDay");
            SetProperty(ref _deathMonth, string.Empty, "DeathMonth");
            SetProperty(ref _deathYear, string.Empty, "DeathYear");
            SetProperty(ref _deathPlace, string.Empty, "DeathPlace");
            SetProperty(ref _burialModifier, string.Empty, "BurialModifier");
            SetProperty(ref _burialDay, string.Empty, "BurialDay");
            SetProperty(ref _burialMonth, string.Empty, "BurialMonth");
            SetProperty(ref _burialYear, string.Empty, "BurialYear");
            SetProperty(ref _burialPlace, string.Empty, "BurialPlace");
            SetProperty(ref _personImageSource, null, "PersonImageSource");
            SetProperty(ref _notes, string.Empty, "Notes");
            Images.Clear();
            SpousesAndWeddings.Clear();
            Children.Clear();
        }
        #pragma warning restore MVVMTK0034

        private void LoadSpousesAndWeddings(IGenPerson genSelectedPerson)
        {
            SpousesAndWeddings.Clear();

            foreach (var genFamily in genSelectedPerson.Marriages)
            {
                if (genFamily is null)
                {
                    continue;
                }

                var genSpouse = ResolveSpouse(genSelectedPerson, genFamily);
                SpousesAndWeddings.Add(new EditSpouseWeddingItemViewModel
                {
                    Spouse = FormatPersonName(genSpouse),
                    WeddingDate = FormatDate(genFamily.MarriageDate),
                    WeddingPlace = genFamily.MarriagePlace?.Name ?? string.Empty
                });
            }

            if (SpousesAndWeddings.Count > 0)
            {
                return;
            }

            foreach (var genSpouse in genSelectedPerson.Spouses)
            {
                if (genSpouse is null)
                {
                    continue;
                }

                SpousesAndWeddings.Add(new EditSpouseWeddingItemViewModel
                {
                    Spouse = FormatPersonName(genSpouse),
                    WeddingDate = string.Empty,
                    WeddingPlace = string.Empty
                });
            }
        }

        private void LoadChildren(IGenPerson genSelectedPerson)
        {
            Children.Clear();

            foreach (var genChild in genSelectedPerson.Children.Where(static genChild => genChild is not null).OrderBy(static genChild => genChild!.BirthDate?.Date1))
            {
                Children.Add(new EditChildItemViewModel
                {
                    Child = FormatPersonName(genChild),
                    BirthDate = FormatDate(genChild!.BirthDate)
                });
            }
        }

        private void ApplyDateValues(
            IGenDate? genDate,
            bool xUseBirthPlaceholder,
            Action<EditPageViewModel, string, string, string, string> actApply)
        {
            var sModifier = GetDateModifierText(genDate);
            var (sDay, sMonth, sYear) = GetDateParts(genDate);

            if (xUseBirthPlaceholder && string.IsNullOrWhiteSpace(sDay) && string.IsNullOrWhiteSpace(sMonth) && string.IsNullOrWhiteSpace(sYear))
            {
                sDay = "<dd>";
                sMonth = "<MM>";
                sYear = "<yyyy>";
            }

            actApply(this, sModifier, sDay, sMonth, sYear);
        }

        private static IEnumerable<string> GetPlaceOptions(
            IEnumerable<IGenPerson> lstPersons,
            Func<IGenPerson, string?> funcSelector)
        {
            return lstPersons
                .Select(funcSelector)
                .Where(sValue => !string.IsNullOrWhiteSpace(sValue))
                .Cast<string>()
                .Distinct(StringComparer.CurrentCultureIgnoreCase)
                .OrderBy(sValue => sValue, StringComparer.CurrentCultureIgnoreCase);
        }

        private static void UpdateStringCollection(ObservableCollection<string> lstTarget, IEnumerable<string> lstValues)
        {
            lstTarget.Clear();
            foreach (var sValue in lstValues)
            {
                lstTarget.Add(sValue);
            }
        }

        private static IGenPerson? ResolveSpouse(IGenPerson genSelectedPerson, IGenFamily genFamily)
        {
            if (ReferenceEquals(genFamily.Husband, genSelectedPerson))
            {
                return genFamily.Wife;
            }

            if (ReferenceEquals(genFamily.Wife, genSelectedPerson))
            {
                return genFamily.Husband;
            }

            return genFamily.Husband?.ID == genSelectedPerson.ID ? genFamily.Wife : genFamily.Husband;
        }

        private static string GetFactData(IGenPerson genPerson, EFactType eFactType)
            => genPerson.Facts.FirstOrDefault(genFact => genFact?.eFactType == eFactType)?.Data ?? string.Empty;

        private static string FormatPersonName(IGenPerson? genPerson)
        {
            if (genPerson is null)
            {
                return string.Empty;
            }

            var sSurname = genPerson.Surname ?? string.Empty;
            var sGivenName = genPerson.GivenName ?? string.Empty;
            var sDisplayName = $"{sSurname}, {sGivenName}".Trim(' ', ',');
            return !string.IsNullOrWhiteSpace(sDisplayName)
                ? sDisplayName
                : genPerson.ID.ToString(CultureInfo.CurrentCulture);
        }

        private static string FormatDate(IGenDate? genDate)
        {
            if (!string.IsNullOrWhiteSpace(genDate?.DateText))
            {
                return genDate.DateText!;
            }

            if (genDate?.Date1 is DateTime dtDate && dtDate != DateTime.MinValue)
            {
                return dtDate.ToString("dd.MM.yyyy", CultureInfo.CurrentCulture);
            }

            return string.Empty;
        }

        private static (string sDay, string sMonth, string sYear) GetDateParts(IGenDate? genDate)
        {
            if (genDate?.Date1 is DateTime dtDate && dtDate != DateTime.MinValue)
            {
                return (
                    dtDate.Day.ToString("00", CultureInfo.CurrentCulture),
                    dtDate.Month.ToString("00", CultureInfo.CurrentCulture),
                    dtDate.Year.ToString(CultureInfo.CurrentCulture));
            }

            if (!string.IsNullOrWhiteSpace(genDate?.DateText)
                && DateTime.TryParse(genDate.DateText, CultureInfo.CurrentCulture, DateTimeStyles.None, out var dtParsedDate))
            {
                return (
                    dtParsedDate.Day.ToString("00", CultureInfo.CurrentCulture),
                    dtParsedDate.Month.ToString("00", CultureInfo.CurrentCulture),
                    dtParsedDate.Year.ToString(CultureInfo.CurrentCulture));
            }

            return (string.Empty, string.Empty, string.Empty);
        }

        private static string GetDateModifierText(IGenDate? genDate)
        {
            return genDate?.eDateModifier switch
            {
                EDateModifier.Before => "vor",
                EDateModifier.After => "nach",
                EDateModifier.Between => "zw.",
                EDateModifier.About => "ca.",
                EDateModifier.Estimated => "ges.",
                EDateModifier.Calculated => "ber.",
                EDateModifier.FromTo => "von-bis",
                EDateModifier.From => "von",
                EDateModifier.To => "bis",
                EDateModifier.Text => "Text",
                _ => string.Empty
            };
        }

        partial void OnIdChanged(string value) => ApplySelectedPersonChangesToModel();

        partial void OnAdditionalInfoChanged(string value) => ApplySelectedPersonChangesToModel();

        partial void OnLastNameChanged(string value) => ApplySelectedPersonChangesToModel();

        partial void OnFirstNamesChanged(string value) => ApplySelectedPersonChangesToModel();

        partial void OnOccupationChanged(string value) => ApplySelectedPersonChangesToModel();

        partial void OnFarmNameChanged(string value) => ApplySelectedPersonChangesToModel();

        partial void OnGenderChanged(string value) => ApplySelectedPersonChangesToModel();

        partial void OnReligionChanged(string value) => ApplySelectedPersonChangesToModel();

        partial void OnIsAliveChanged(bool value) => ApplySelectedPersonChangesToModel();

        partial void OnAdoptedByChanged(string value) => ApplySelectedPersonChangesToModel();

        partial void OnBirthDayChanged(string value) => ApplySelectedPersonChangesToModel();

        partial void OnBirthMonthChanged(string value) => ApplySelectedPersonChangesToModel();

        partial void OnBirthYearChanged(string value) => ApplySelectedPersonChangesToModel();

        partial void OnBirthPlaceChanged(string value)
        {
            BirthPlaceHidden = value;
            ApplySelectedPersonChangesToModel();
        }

        partial void OnBaptismModifierChanged(string value) => ApplySelectedPersonChangesToModel();

        partial void OnBaptismDayChanged(string value) => ApplySelectedPersonChangesToModel();

        partial void OnBaptismMonthChanged(string value) => ApplySelectedPersonChangesToModel();

        partial void OnBaptismYearChanged(string value) => ApplySelectedPersonChangesToModel();

        partial void OnBaptismPlaceChanged(string value)
        {
            BaptismPlaceHidden = value;
            ApplySelectedPersonChangesToModel();
        }

        partial void OnDeathDayChanged(string value) => ApplySelectedPersonChangesToModel();

        partial void OnDeathMonthChanged(string value) => ApplySelectedPersonChangesToModel();

        partial void OnDeathYearChanged(string value) => ApplySelectedPersonChangesToModel();

        partial void OnDeathPlaceChanged(string value) => ApplySelectedPersonChangesToModel();

        partial void OnBurialDayChanged(string value) => ApplySelectedPersonChangesToModel();

        partial void OnBurialMonthChanged(string value) => ApplySelectedPersonChangesToModel();

        partial void OnBurialYearChanged(string value) => ApplySelectedPersonChangesToModel();

        partial void OnBurialPlaceChanged(string value) => ApplySelectedPersonChangesToModel();

        partial void OnNotesChanged(string value) => ApplySelectedPersonChangesToModel();

        public void PersistSelectedPersonChanges()
        {
            ApplySelectedPersonChangesToModel();
            _personSelectionService.SaveChanges();
            LoadOptionLists();
        }

        private void ApplySelectedPersonChangesToModel()
        {
            var genSelectedPerson = _personSelectionService.SelectedPerson;
            if (genSelectedPerson is null)
            {
                return;
            }

            var xHasChanges = false;

            xHasChanges |= SaveSimpleFact(genSelectedPerson, EFactType.Reference, Id);
            xHasChanges |= SaveSimpleFact(genSelectedPerson, EFactType.Info, AdditionalInfo);
            xHasChanges |= SaveSimpleFact(genSelectedPerson, EFactType.Surname, LastName);
            xHasChanges |= SaveSimpleFact(genSelectedPerson, EFactType.Givenname, FirstNames);
            xHasChanges |= SaveSimpleFact(genSelectedPerson, EFactType.Occupation, Occupation);
            xHasChanges |= SaveSimpleFact(genSelectedPerson, EFactType.Property, FarmName);
            xHasChanges |= SaveSimpleFact(genSelectedPerson, EFactType.Sex, Gender);
            xHasChanges |= SaveSimpleFact(genSelectedPerson, EFactType.Religion, Religion);
            xHasChanges |= SaveSimpleFact(genSelectedPerson, EFactType.Adoption, AdoptedBy);
            xHasChanges |= SaveSimpleFact(genSelectedPerson, EFactType.Description, Notes);

            xHasChanges |= SaveEventFact(genSelectedPerson, EFactType.Birth, BirthDay, BirthMonth, BirthYear, BirthModifier, BirthPlace);
            xHasChanges |= SaveEventFact(genSelectedPerson, EFactType.Baptism, BaptismDay, BaptismMonth, BaptismYear, BaptismModifier, BaptismPlace);

            if (IsAlive)
            {
                xHasChanges |= RemoveFact(genSelectedPerson, EFactType.Death);
                xHasChanges |= RemoveFact(genSelectedPerson, EFactType.Burial);
            }
            else
            {
                xHasChanges |= SaveEventFact(genSelectedPerson, EFactType.Death, DeathDay, DeathMonth, DeathYear, DeathModifier, DeathPlace);
                xHasChanges |= SaveEventFact(genSelectedPerson, EFactType.Burial, BurialDay, BurialMonth, BurialYear, BurialModifier, BurialPlace);
            }

            if (xHasChanges)
            {
                MarkGenealogyDirty(genSelectedPerson, "Edit page values were applied to the selected person.");
            }
        }

        private static bool SaveSimpleFact(IGenPerson genPerson, EFactType eFactType, string? sValue)
        {
            var sNormalizedValue = NormalizeValue(sValue);
            var genFact = genPerson.Facts.FirstOrDefault(genFact => genFact?.eFactType == eFactType);
            var fctOldValue = FactJournalValue.FromFact(genFact);

            if (genFact is null)
            {
                if (string.IsNullOrEmpty(sNormalizedValue))
                {
                    return false;
                }

                var genNewFact = genPerson.AddFact(eFactType, sNormalizedValue);
                RecordFactJournalEntry(genPerson, genNewFact, null, FactJournalValue.FromFact(genNewFact));
                return true;
            }

            if (string.Equals(genFact.Data ?? string.Empty, sNormalizedValue, StringComparison.CurrentCulture))
            {
                return false;
            }

            genFact.Data = sNormalizedValue;
            RecordFactJournalEntry(genPerson, genFact, fctOldValue, FactJournalValue.FromFact(genFact));
            return true;
        }

        private static bool SaveEventFact(
            IGenPerson genPerson,
            EFactType eFactType,
            string? sDay,
            string? sMonth,
            string? sYear,
            string? sModifier,
            string? sPlaceName)
        {
            var genFact = genPerson.Facts.FirstOrDefault(genFact => genFact?.eFactType == eFactType);
            var dtDate = TryBuildDate(sDay, sMonth, sYear);
            var sNormalizedPlaceName = NormalizePlaceValue(sPlaceName);
            var fctOldValue = FactJournalValue.FromFact(genFact);

            if (dtDate is null && string.IsNullOrWhiteSpace(sNormalizedPlaceName))
            {
                if (genFact is not null)
                {
                    genPerson.Facts.Remove(genFact);
                    RecordFactJournalEntry(genPerson, genFact, fctOldValue, null);
                    return true;
                }

                return false;
            }

            var xChanged = genFact is null;
            genFact ??= genPerson.AddFact(eFactType, string.Empty);
            var genTargetDate = dtDate is null
                ? null
                : new GenDate(MapDateModifier(sModifier), EDateType.Full, dtDate.Value)
                {
                    DateText = dtDate.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)
                };
            var genTargetPlace = GetOrCreatePlace(genPerson, sNormalizedPlaceName);

            if (!AreEquivalentDates(genFact.Date, genTargetDate))
            {
                genFact.Date = genTargetDate;
                xChanged = true;
            }

            if (!AreEquivalentPlaces(genFact.Place, genTargetPlace))
            {
                genFact.Place = genTargetPlace;
                xChanged = true;
            }

            if (xChanged)
            {
                RecordFactJournalEntry(genPerson, genFact, fctOldValue, FactJournalValue.FromFact(genFact));
            }

            return xChanged;
        }

        private static bool RemoveFact(IGenPerson genPerson, EFactType eFactType)
        {
            var genFact = genPerson.Facts.FirstOrDefault(genFact => genFact?.eFactType == eFactType);
            if (genFact is not null)
            {
                var fctOldValue = FactJournalValue.FromFact(genFact);
                genPerson.Facts.Remove(genFact);
                RecordFactJournalEntry(genPerson, genFact, fctOldValue, null);
                return true;
            }

            return false;
        }

        private static IGenPlace? GetOrCreatePlace(IGenPerson genPerson, string? sPlaceName)
        {
            if (string.IsNullOrWhiteSpace(sPlaceName))
            {
                return null;
            }

            var genGenealogy = ((IHasOwner<IGenealogy>)genPerson).Owner;
            if (genGenealogy is null)
            {
                return null;
            }

            var genExistingPlace = genGenealogy.Places.FirstOrDefault(genPlace => string.Equals(genPlace.Name, sPlaceName, StringComparison.CurrentCultureIgnoreCase));
            if (genExistingPlace is not null)
            {
                return genExistingPlace;
            }

            var genNewPlace = new GenPlace(sPlaceName)
            {
                UId = Guid.NewGuid(),
                ID = genGenealogy.Places.Count == 0 ? 1 : genGenealogy.Places.Max(genPlace => genPlace.ID) + 1
            };

            ((IHasOwner<IGenealogy>)genNewPlace).SetOwner(genGenealogy);
            genGenealogy.Places.Add(genNewPlace);
            return genNewPlace;
        }

        private static void MarkGenealogyDirty(IGenPerson genPerson, string sReason)
        {
            if (((IHasOwner<IGenealogy>)genPerson).Owner is IGenealogyPersistenceContext persistenceContext)
            {
                persistenceContext.MarkDirty(genPerson, sReason);
            }
        }

        private static void RecordFactJournalEntry(IGenPerson genPerson, IGenFact genFact, FactJournalValue? fctOldValue, FactJournalValue? fctNewValue)
        {
            if (((IHasOwner<IGenealogy>)genPerson).Owner is IGenealogyJournalContext journalContext)
            {
                journalContext.RecordJournalEntry(genPerson, genFact, fctNewValue, fctOldValue);
            }
        }

        private static bool AreEquivalentDates(IGenDate? genLeftDate, IGenDate? genRightDate)
        {
            if (ReferenceEquals(genLeftDate, genRightDate))
            {
                return true;
            }

            if (genLeftDate is null || genRightDate is null)
            {
                return false;
            }

            return genLeftDate.eDateModifier == genRightDate.eDateModifier
                && genLeftDate.Date1 == genRightDate.Date1
                && string.Equals(genLeftDate.DateText, genRightDate.DateText, StringComparison.CurrentCulture);
        }

        private static bool AreEquivalentPlaces(IGenPlace? genLeftPlace, IGenPlace? genRightPlace)
        {
            if (ReferenceEquals(genLeftPlace, genRightPlace))
            {
                return true;
            }

            if (genLeftPlace is null || genRightPlace is null)
            {
                return false;
            }

            return string.Equals(genLeftPlace.Name, genRightPlace.Name, StringComparison.CurrentCultureIgnoreCase);
        }

        private static DateTime? TryBuildDate(string? sDay, string? sMonth, string? sYear)
        {
            if (!int.TryParse(sDay, NumberStyles.Integer, CultureInfo.CurrentCulture, out var iDay)
                || !int.TryParse(sMonth, NumberStyles.Integer, CultureInfo.CurrentCulture, out var iMonth)
                || !int.TryParse(sYear, NumberStyles.Integer, CultureInfo.CurrentCulture, out var iYear))
            {
                return null;
            }

            try
            {
                return new DateTime(iYear, iMonth, iDay);
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        private static string NormalizeValue(string? sValue)
            => string.IsNullOrWhiteSpace(sValue) ? string.Empty : sValue.Trim();

        private static string? NormalizePlaceValue(string? sValue)
        {
            var sNormalizedValue = NormalizeValue(sValue);
            if (string.IsNullOrWhiteSpace(sNormalizedValue)
                || string.Equals(sNormalizedValue, "<Birthplace>", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return sNormalizedValue;
        }

        private static EDateModifier MapDateModifier(string? sModifier)
        {
            return NormalizeValue(sModifier) switch
            {
                "vor" => EDateModifier.Before,
                "nach" => EDateModifier.After,
                "zw." => EDateModifier.Between,
                "ca." => EDateModifier.About,
                "ges." => EDateModifier.Estimated,
                "ber." => EDateModifier.Calculated,
                "von-bis" => EDateModifier.FromTo,
                "von" => EDateModifier.From,
                "bis" => EDateModifier.To,
                "Text" => EDateModifier.Text,
                _ => EDateModifier.None
            };
        }
    }

    /// <summary>
    /// Represents a row in the images grid.
    /// </summary>
    public sealed class EditImageItemViewModel
    {
        public string Title { get; init; } = string.Empty;

        public string Description { get; init; } = string.Empty;
    }

    /// <summary>
    /// Represents a spouse and wedding row.
    /// </summary>
    public sealed class EditSpouseWeddingItemViewModel
    {
        public string Spouse { get; init; } = string.Empty;

        public string WeddingDate { get; init; } = string.Empty;

        public string WeddingPlace { get; init; } = string.Empty;
    }

    /// <summary>
    /// Represents a child row.
    /// </summary>
    public sealed class EditChildItemViewModel
    {
        public string Child { get; init; } = string.Empty;

        public string BirthDate { get; init; } = string.Empty;
    }
}
