using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BaseGenClasses.Model;
using BaseGenClasses.Persistence;
using CommunityToolkit.Mvvm.Messaging;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
using GenSecure.Contracts;
using WinAhnenNew.Messages;

namespace WinAhnenNew.Services
{
    /// <summary>
    /// Loads the selectable persons from the secure genealogy persistence store.
    /// </summary>
    public sealed class PersonSelectionService : IPersonSelectionService
    {
        private const string DefaultGenealogyId = "winahnen-default";
        private static readonly string[] _givenNames =
        [
            "Anna", "Johann", "Maria", "Karl", "Sophie", "Martin", "Helene", "Friedrich",
            "Clara", "Wilhelm", "Elisabeth", "Paul", "Therese", "Georg", "Luise", "August"
        ];

        private static readonly string[] _surnames =
        [
            "Meyer", "Schulze", "Krüger", "Becker", "Hoffmann", "Wagner", "Richter", "Koch",
            "Schäfer", "Bauer", "Klein", "Wolf", "Schröder", "Neumann", "Braun", "Hartmann"
        ];

        private static readonly string[] _placeNames =
        [
            "Enger", "Bünde", "Herford", "Melle", "Osnabrück", "Lübbecke", "Bielefeld", "Minden"
        ];

        private readonly IGenealogySecureStore _genealogySecureStore;
        private readonly IGenealogyModelFactory _genealogyModelFactory;
        private readonly IMessenger _messenger;
        private IGenealogy? _genealogy;
        private IGenPerson? _selectedPerson;

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
        public IGenPerson? SelectedPerson => _selectedPerson;

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

        /// <inheritdoc />
        public void SetSelectedPerson(IGenPerson? genSelectedPerson)
        {
            _selectedPerson = genSelectedPerson;
            _messenger.Send(new SelectedPersonChangedMessage(genSelectedPerson));
        }

        /// <inheritdoc />
        public void SaveChanges()
        {
            var genGenealogy = LoadGenealogy();
            if (genGenealogy is IGenealogyPersistenceContext persistenceContext)
            {
                persistenceContext.FlushAsync(_selectedPerson, GenealogyFlushScope.Auto).GetAwaiter().GetResult();
                return;
            }

            _genealogySecureStore.Save(DefaultGenealogyId, genGenealogy);
        }

        /// <inheritdoc />
        public void CreateDemoGenealogy(int iPersonCount)
        {
            if (iPersonCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(iPersonCount));
            }

            _genealogy = CreateConnectedDemoGenealogy(iPersonCount);
            _genealogySecureStore.Save(DefaultGenealogyId, _genealogy);
            SetSelectedPerson(null);
            _messenger.Send(new GenealogyChangedMessage(iPersonCount));
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
                ConfigurePersistence(_genealogy);
                _genealogySecureStore.Save(DefaultGenealogyId, _genealogy);
                return _genealogy;
            }

            _genealogy = _genealogySecureStore.Load(DefaultGenealogyId, _genealogyModelFactory);
            ConfigurePersistence(_genealogy);
            return _genealogy;
        }

        private void ConfigurePersistence(IGenealogy genGenealogy)
        {
            if (genGenealogy is IGenealogyPersistenceContext persistenceContext)
            {
                persistenceContext.AttachPersistenceProvider(new GenealogySecureStorePersistenceProvider(_genealogySecureStore, DefaultGenealogyId));
            }
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


        private IGenealogy CreateConnectedDemoGenealogy(int iPersonCount)
        {
            var genGenealogy = new Genealogy(_messenger)
            {
                UId = Guid.NewGuid()
            };

            var arrPlaces = _placeNames
                .Select((sName, iIndex) => CreatePlace(genGenealogy, iIndex + 1, sName))
                .ToArray();

            GenPerson? genPreviousPerson = null;
            for (var iIndex = 0; iIndex < iPersonCount; iIndex++)
            {
                var iPersonId = iIndex + 1;
                var sGivenName = _givenNames[iIndex % _givenNames.Length];
                var sSurname = _surnames[(iIndex / 2) % _surnames.Length];
                var sSex = iIndex % 2 == 0 ? "M" : "F";
                var dtBirthDate = new DateTime(1820 + iIndex, (iIndex % 12) + 1, (iIndex % 27) + 1);
                var xIsLiving = iIndex >= iPersonCount - 8;
                var genBirthPlace = arrPlaces[iIndex % arrPlaces.Length];

                var genCurrentPerson = CreatePerson(
                    genGenealogy,
                    iPersonId,
                    iPersonId.ToString(CultureInfo.InvariantCulture),
                    $"{sGivenName} {iPersonId}",
                    sSurname,
                    sSex,
                    dtBirthDate,
                    genBirthPlace,
                    xIsLiving);

                if (genPreviousPerson is not null)
                {
                    genCurrentPerson.Connects.Add(new GenConnect
                    {
                        UId = Guid.NewGuid(),
                        eGenConnectionType = EGenConnectionType.Parent,
                        Entity = genPreviousPerson
                    });

                    genPreviousPerson.Connects.Add(new GenConnect
                    {
                        UId = Guid.NewGuid(),
                        eGenConnectionType = EGenConnectionType.Child,
                        Entity = genCurrentPerson
                    });
                }

                genGenealogy.Entitys.Add(genCurrentPerson);
                genPreviousPerson = genCurrentPerson;
            }

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

        private sealed class GenealogySecureStorePersistenceProvider : IGenealogyPersistenceProvider
        {
            private readonly IGenealogySecureStore _genealogySecureStore;
            private readonly string _sGenealogyId;

            public GenealogySecureStorePersistenceProvider(IGenealogySecureStore genealogySecureStore, string sGenealogyId)
            {
                _genealogySecureStore = genealogySecureStore;
                _sGenealogyId = sGenealogyId;
            }

            public Task FlushAsync(
                IGenealogy genGenealogy,
                IGenEntity? genRequestedEntity,
                GenealogyFlushScope eScope,
                System.Threading.CancellationToken cancellationToken = default)
            {
                cancellationToken.ThrowIfCancellationRequested();
                _genealogySecureStore.Save(_sGenealogyId, genGenealogy);
                return Task.CompletedTask;
            }
        }
    }
}
