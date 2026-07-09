using System;

namespace Gen_FreeWin.Models
{
    /// <summary>
    /// Represents a selectable land-register file entry shown in the first ownership selection list.
    /// </summary>
    public sealed class BesitzAkteListItem : IEquatable<BesitzAkteListItem>
    {
        private readonly int _recordNumber;
        private readonly string _akte;
        private readonly string _kirchspiel;

        /// <summary>
        /// Initializes a new instance of the <see cref="BesitzAkteListItem"/> class.
        /// </summary>
        /// <param name="recordNumber">The technical record number.</param>
        /// <param name="akte">The business file number.</param>
        /// <param name="kirchspiel">The administrative location text.</param>
        public BesitzAkteListItem(int recordNumber, string akte, string kirchspiel)
        {
            _recordNumber = recordNumber;
            _akte = akte;
            _kirchspiel = kirchspiel;
        }

        /// <summary>
        /// Gets the technical record number.
        /// </summary>
        public int RecordNumber => _recordNumber;

        /// <summary>
        /// Gets the business file number.
        /// </summary>
        public string Akte => _akte;

        /// <summary>
        /// Gets the administrative location text.
        /// </summary>
        public string Kirchspiel => _kirchspiel;

        /// <summary>
        /// Gets the display text shown in the UI list.
        /// </summary>
        public string DisplayText => string.Format("{0} {1}", _akte.Trim(), _kirchspiel.Trim()).Trim();

        /// <inheritdoc />
        public override string ToString()
        {
            return DisplayText;
        }

        /// <inheritdoc />
        public bool Equals(BesitzAkteListItem other)
        {
            return other != null
                && _recordNumber == other._recordNumber
                && string.Equals(_akte, other._akte, StringComparison.Ordinal)
                && string.Equals(_kirchspiel, other._kirchspiel, StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as BesitzAkteListItem);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 23) + _recordNumber.GetHashCode();
                hash = (hash * 23) + (_akte != null ? _akte.GetHashCode() : 0);
                hash = (hash * 23) + (_kirchspiel != null ? _kirchspiel.GetHashCode() : 0);
                return hash;
            }
        }
    }

    /// <summary>
    /// Represents a selectable ownership history entry shown in the second selection list.
    /// </summary>
    public sealed class BesitzEntryListItem : IEquatable<BesitzEntryListItem>
    {
        private readonly int _recordNumber;
        private readonly string _year;
        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="BesitzEntryListItem"/> class.
        /// </summary>
        /// <param name="recordNumber">The technical record number.</param>
        /// <param name="year">The entry year text.</param>
        /// <param name="name">The entry description.</param>
        public BesitzEntryListItem(int recordNumber, string year, string name)
        {
            _recordNumber = recordNumber;
            _year = year;
            _name = name;
        }

        /// <summary>
        /// Gets the technical record number.
        /// </summary>
        public int RecordNumber => _recordNumber;

        /// <summary>
        /// Gets the year text.
        /// </summary>
        public string Year => _year;

        /// <summary>
        /// Gets the name text.
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// Gets the display text shown in the UI list.
        /// </summary>
        public string DisplayText => string.Format("{0} {1}", _year.Trim(), TrimName(_name)).Trim();

        /// <inheritdoc />
        public override string ToString()
        {
            return DisplayText;
        }

        /// <inheritdoc />
        public bool Equals(BesitzEntryListItem other)
        {
            return other != null
                && _recordNumber == other._recordNumber
                && string.Equals(_year, other._year, StringComparison.Ordinal)
                && string.Equals(_name, other._name, StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as BesitzEntryListItem);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 23) + _recordNumber.GetHashCode();
                hash = (hash * 23) + (_year != null ? _year.GetHashCode() : 0);
                hash = (hash * 23) + (_name != null ? _name.GetHashCode() : 0);
                return hash;
            }
        }

        private static string TrimName(string value)
        {
            return value.Length <= 20 ? value.Trim() : value.Substring(0, 20).Trim();
        }
    }

    /// <summary>
    /// Contains the detailed data of a selected land-register file.
    /// </summary>
    public sealed class BesitzAkteDetails
    {
        private readonly int _recordNumber;
        private readonly string _akte;
        private readonly string _kirchspiel;
        private readonly string _beschreibung;
        private readonly string _hofklasse;
        private readonly string _flur;
        private readonly string _parzelle;

        /// <summary>
        /// Initializes a new instance of the <see cref="BesitzAkteDetails"/> class.
        /// </summary>
        public BesitzAkteDetails(int recordNumber, string akte, string kirchspiel, string beschreibung, string hofklasse, string flur, string parzelle)
        {
            _recordNumber = recordNumber;
            _akte = akte;
            _kirchspiel = kirchspiel;
            _beschreibung = beschreibung;
            _hofklasse = hofklasse;
            _flur = flur;
            _parzelle = parzelle;
        }

        public int RecordNumber => _recordNumber;
        public string Akte => _akte;
        public string Kirchspiel => _kirchspiel;
        public string Beschreibung => _beschreibung;
        public string Hofklasse => _hofklasse;
        public string Flur => _flur;
        public string Parzelle => _parzelle;

        /// <summary>
        /// Gets the formatted corridor text for the UI.
        /// </summary>
        public string FlurText => string.IsNullOrWhiteSpace(_flur) ? string.Empty : string.Format("Flur: {0}", _flur);

        /// <summary>
        /// Gets the formatted parcel text for the UI.
        /// </summary>
        public string ParzelleText => string.IsNullOrWhiteSpace(_parzelle) ? string.Empty : string.Format("Parzelle: {0}", _parzelle);
    }

    /// <summary>
    /// Contains the detailed data of a selected ownership history entry.
    /// </summary>
    public sealed class BesitzEntryDetails
    {
        private readonly int _recordNumber;
        private readonly string _akte;
        private readonly string _jahr;
        private readonly string _erbaut;
        private readonly string _abgaengig;
        private readonly string _name;
        private readonly string _gebaeudeart;

        /// <summary>
        /// Initializes a new instance of the <see cref="BesitzEntryDetails"/> class.
        /// </summary>
        public BesitzEntryDetails(int recordNumber, string akte, string jahr, string erbaut, string abgaengig, string name, string gebaeudeart)
        {
            _recordNumber = recordNumber;
            _akte = akte;
            _jahr = jahr;
            _erbaut = erbaut;
            _abgaengig = abgaengig;
            _name = name;
            _gebaeudeart = gebaeudeart;
        }

        public int RecordNumber => _recordNumber;
        public string Akte => _akte;
        public string Jahr => _jahr;
        public string Erbaut => _erbaut;
        public string Abgaengig => _abgaengig;
        public string Name => _name;
        public string Gebaeudeart => _gebaeudeart;
    }
}
