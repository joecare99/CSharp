using System;
using System.Globalization;
using System.Linq;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;

namespace WinAhnenNew.ViewModels
{
    /// <summary>
    /// Represents one selectable person row for the selection page.
    /// </summary>
    public sealed class SelectionPersonItemViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionPersonItemViewModel"/> class.
        /// </summary>
        /// <param name="genPerson">The underlying genealogy person.</param>
        public SelectionPersonItemViewModel(IGenPerson genPerson)
        {
            Person = genPerson ?? throw new ArgumentNullException(nameof(genPerson));
        }

        /// <summary>
        /// Gets the underlying genealogy person.
        /// </summary>
        public IGenPerson Person { get; }

        /// <summary>
        /// Gets the displayable person identifier.
        /// </summary>
        public string PersonId => !string.IsNullOrWhiteSpace(GetFactData(EFactType.Reference))
            ? GetFactData(EFactType.Reference)
            : Person.ID.ToString(CultureInfo.CurrentCulture);

        /// <summary>
        /// Gets the surname.
        /// </summary>
        public string Surname => GetFactData(EFactType.Surname);

        /// <summary>
        /// Gets the given name.
        /// </summary>
        public string GivenName => GetFactData(EFactType.Givenname);

        /// <summary>
        /// Gets the display name.
        /// </summary>
        public string DisplayName => $"{Surname}, {GivenName}".Trim(' ', ',');

        /// <summary>
        /// Gets the sex text.
        /// </summary>
        public string Sex => GetFactData(EFactType.Sex);

        /// <summary>
        /// Gets the birth date text.
        /// </summary>
        public string BirthDateText
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Person.BirthDate?.DateText))
                {
                    return Person.BirthDate.DateText;
                }

                var genBirthFact = GetFact(EFactType.Birth);
                if (!string.IsNullOrWhiteSpace(genBirthFact?.Date?.DateText))
                {
                    return genBirthFact.Date.DateText;
                }

                return genBirthFact?.Date?.Date1 is DateTime dtBirthDate && dtBirthDate != DateTime.MinValue
                    ? dtBirthDate.ToString("dd.MM.yyyy", CultureInfo.CurrentCulture)
                    : string.Empty;
            }
        }

        /// <summary>
        /// Gets the birth place text.
        /// </summary>
        public string BirthPlace => GetFact(EFactType.Birth)?.Place?.Name ?? string.Empty;

        /// <summary>
        /// Gets a value indicating whether the person is treated as living.
        /// </summary>
        public bool IsLiving => GetFact(EFactType.Death) is null && GetFact(EFactType.Burial) is null;

        /// <summary>
        /// Gets the numeric local identifier.
        /// </summary>
        public int PersonNumber => Person.ID;

        /// <summary>
        /// Gets the sortable birth date value.
        /// </summary>
        public DateTime? BirthDateValue
        {
            get
            {
                var dtBirthDate = GetFact(EFactType.Birth)?.Date?.Date1;
                return dtBirthDate == DateTime.MinValue ? null : dtBirthDate;
            }
        }

        private IGenFact? GetFact(EFactType eFactType)
            => Person.Facts.FirstOrDefault(genFact => genFact?.eFactType == eFactType);

        private string GetFactData(EFactType eFactType)
            => GetFact(eFactType)?.Data ?? string.Empty;
    }
}
