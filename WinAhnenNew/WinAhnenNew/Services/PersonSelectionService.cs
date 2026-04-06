using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BaseGenClasses.Model;
using CommunityToolkit.Mvvm.Messaging;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
using GenSecure.Contracts;

namespace WinAhnenNew.Services
{
    /// <summary>
    /// Loads the selectable persons from the secure genealogy persistence store.
    /// </summary>
    public sealed class PersonSelectionService : IPersonSelectionService
    {
        private const string DefaultGenealogyId = "winahnen-default";

        private readonly IGenealogySecureStore _genealogySecureStore;
        private readonly IGenealogyModelFactory _genealogyModelFactory;
        private readonly IMessenger _messenger;
        private IGenealogy? _genealogy;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonSelectionService"/> class.
        /// </summary>
        /// <param name="genealogySecureStore">The secure genealogy store.</param>
        /// <param name="genealogyModelFactory">The model factory used for rehydration.</param>
        /// <param name="messenger">The shared application messenger.</param>
        public PersonSelectionService(
            IGenealogySecureStore genealogySecureStore,
            IGenealogyModelFactory genealogyModelFactory,
            IMessenger messenger)
        {
            _genealogySecureStore = genealogySecureStore;
            _genealogyModelFactory = genealogyModelFactory;
            _messenger = messenger;
        }

        /// <inheritdoc />
        public IReadOnlyList<IGenPerson> GetSelectablePersons()
        {
            return LoadGenealogy()
                .Entitys
                .OfType<IGenPerson>()
                .OrderBy(genPerson => genPerson.Surname ?? string.Empty, StringComparer.CurrentCultureIgnoreCase)
                .ThenBy(genPerson => genPerson.GivenName ?? string.Empty, StringComparer.CurrentCultureIgnoreCase)
                .ToArray();
        }

        private IGenealogy LoadGenealogy()
        {
            if (_genealogy is not null)
            {
                return _genealogy;
            }

            if (!_genealogySecureStore.Exists(DefaultGenealogyId))
            {
                _genealogy = CreateDefaultGenealogy();
                _genealogySecureStore.Save(DefaultGenealogyId, _genealogy);
                return _genealogy;
            }

            _genealogy = _genealogySecureStore.Load(DefaultGenealogyId, _genealogyModelFactory);
            return _genealogy;
        }

        private IGenealogy CreateDefaultGenealogy()
        {
            var genGenealogy = new Genealogy(_messenger)
            {
                UId = Guid.NewGuid()
            };

            var genPlaceEnger = CreatePlace(genGenealogy, 1, "Enger");
            var genPlaceBuende = CreatePlace(genGenealogy, 2, "Bünde");
            var genPlaceHerford = CreatePlace(genGenealogy, 3, "Herford");
            var genPlaceMelle = CreatePlace(genGenealogy, 4, "Melle");
            var genPlaceOsnabrueck = CreatePlace(genGenealogy, 5, "Osnabrück");
            var genPlaceLuebbecke = CreatePlace(genGenealogy, 6, "Lübbecke");
            var genPlaceBielefeld = CreatePlace(genGenealogy, 7, "Bielefeld");
            var genPlaceMinden = CreatePlace(genGenealogy, 8, "Minden");

            genGenealogy.Entitys.Add(CreatePerson(genGenealogy, 1, "1", "Anna Maria", "Meyer", "F", new DateTime(1884, 3, 12), genPlaceEnger, false));
            genGenealogy.Entitys.Add(CreatePerson(genGenealogy, 2, "2", "Johann", "Schulze", "M", new DateTime(1879, 11, 8), genPlaceBuende, false));
            genGenealogy.Entitys.Add(CreatePerson(genGenealogy, 3, "3", "Elisabeth", "Krüger", "F", new DateTime(1901, 5, 21), genPlaceHerford, false));
            genGenealogy.Entitys.Add(CreatePerson(genGenealogy, 4, "4", "Karl", "Becker", "M", new DateTime(1938, 7, 14), genPlaceMelle, false));
            genGenealogy.Entitys.Add(CreatePerson(genGenealogy, 5, "5", "Sophie", "Hoffmann", "F", new DateTime(1954, 1, 3), genPlaceOsnabrueck, true));
            genGenealogy.Entitys.Add(CreatePerson(genGenealogy, 6, "6", "Martin", "Wagner", "M", new DateTime(1961, 9, 18), genPlaceLuebbecke, true));
            genGenealogy.Entitys.Add(CreatePerson(genGenealogy, 7, "7", "Helene", "Richter", "F", new DateTime(1972, 12, 30), genPlaceBielefeld, true));
            genGenealogy.Entitys.Add(CreatePerson(genGenealogy, 8, "8", "Friedrich", "Koch", "M", new DateTime(1988, 4, 9), genPlaceMinden, true));

            return genGenealogy;
        }

        private static GenPlace CreatePlace(IGenealogy genGenealogy, int iId, string sName)
        {
            var genPlace = new GenPlace(sName)
            {
                UId = Guid.NewGuid(),
                ID = iId
            };

            ((IHasOwner<IGenealogy>)genPlace).SetOwner(genGenealogy);
            genGenealogy.Places.Add(genPlace);
            return genPlace;
        }

        private static GenPerson CreatePerson(
            IGenealogy genGenealogy,
            int iId,
            string sReferenceId,
            string sGivenName,
            string sSurname,
            string sSex,
            DateTime dtBirthDate,
            IGenPlace genBirthPlace,
            bool xIsLiving)
        {
            var genPerson = new GenPerson
            {
                UId = Guid.NewGuid(),
                ID = iId
            };

            ((IHasOwner<IGenealogy>)genPerson).SetOwner(genGenealogy);

            genPerson.Facts.Add(new GenFact(genPerson, EFactType.Reference)
            {
                UId = Guid.NewGuid(),
                ID = iId * 10 + 1,
                Data = sReferenceId
            });
            genPerson.Facts.Add(new GenFact(genPerson, EFactType.Givenname)
            {
                UId = Guid.NewGuid(),
                ID = iId * 10 + 2,
                Data = sGivenName
            });
            genPerson.Facts.Add(new GenFact(genPerson, EFactType.Surname)
            {
                UId = Guid.NewGuid(),
                ID = iId * 10 + 3,
                Data = sSurname
            });
            genPerson.Facts.Add(new GenFact(genPerson, EFactType.Sex)
            {
                UId = Guid.NewGuid(),
                ID = iId * 10 + 4,
                Data = sSex
            });
            genPerson.Facts.Add(new GenFact(genPerson, EFactType.Birth)
            {
                UId = Guid.NewGuid(),
                ID = iId * 10 + 5,
                Date = new GenDate(dtBirthDate)
                {
                    UId = Guid.NewGuid(),
                    ID = iId * 10 + 6,
                    DateText = dtBirthDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)
                },
                Place = genBirthPlace
            });

            if (!xIsLiving)
            {
                var dtDeathDate = dtBirthDate.AddYears(74);
                genPerson.Facts.Add(new GenFact(genPerson, EFactType.Death)
                {
                    UId = Guid.NewGuid(),
                    ID = iId * 10 + 7,
                    Date = new GenDate(dtDeathDate)
                    {
                        UId = Guid.NewGuid(),
                        ID = iId * 10 + 8,
                        DateText = dtDeathDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)
                    }
                });
            }

            return genPerson;
        }
    }
}
