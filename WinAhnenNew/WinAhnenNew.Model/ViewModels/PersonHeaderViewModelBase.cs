using System.Globalization;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using WinAhnenNew.Messages;
using WinAhnenNew.Services;

namespace WinAhnenNew.ViewModels
{
    /// <summary>
    /// Provides the shared header data for person-related tabs.
    /// </summary>
    public abstract partial class PersonHeaderViewModelBase : ViewModelBase
    {
        private readonly IPersonSelectionService _personSelectionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonHeaderViewModelBase"/> class.
        /// </summary>
        /// <param name="personSelectionService">The shared person selection service.</param>
        /// <param name="messenger">The shared application messenger.</param>
        protected PersonHeaderViewModelBase(IPersonSelectionService personSelectionService, IMessenger messenger)
        {
            _personSelectionService = personSelectionService;

            ApplySelectedPerson(_personSelectionService.SelectedPerson);

            messenger.Register<SelectedPersonChangedMessage>(this, static (objRecipient, msgMessage) =>
            {
                if (objRecipient is PersonHeaderViewModelBase vmHeader)
                {
                    vmHeader.ApplySelectedPerson(msgMessage.Value);
                }
            });

            messenger.Register<GenealogyChangedMessage>(this, static (objRecipient, _) =>
            {
                if (objRecipient is PersonHeaderViewModelBase vmHeader)
                {
                    vmHeader.ApplySelectedPerson(vmHeader._personSelectionService.SelectedPerson);
                }
            });

            messenger.Register<PersonHeaderChangedMessage>(this, static (objRecipient, _) =>
            {
                if (objRecipient is PersonHeaderViewModelBase vmHeader)
                {
                    vmHeader.ApplySelectedPerson(vmHeader._personSelectionService.SelectedPerson);
                }
            });
        }

        [ObservableProperty]
        private string _displayName = string.Empty;

        [ObservableProperty]
        private string _father = string.Empty;

        [ObservableProperty]
        private string _mother = string.Empty;

        [ObservableProperty]
        private string _personNumber = string.Empty;

        [ObservableProperty]
        private string _id = string.Empty;

        [ObservableProperty]
        private string _additionalInfo = string.Empty;

        [ObservableProperty]
        private bool _isSelectPersonToggled;

        private void ApplySelectedPerson(IGenPerson? genSelectedPerson)
        {
            if (genSelectedPerson is null)
            {
                DisplayName = string.Empty;
                Father = string.Empty;
                Mother = string.Empty;
                PersonNumber = string.Empty;
                Id = string.Empty;
                AdditionalInfo = string.Empty;
                IsSelectPersonToggled = false;
                return;
            }

            DisplayName = FormatDisplayName(genSelectedPerson);
            Father = FormatPersonName(genSelectedPerson.Father);
            Mother = FormatPersonName(genSelectedPerson.Mother);
            PersonNumber = genSelectedPerson.ID.ToString(CultureInfo.CurrentCulture);
            Id = !string.IsNullOrWhiteSpace(genSelectedPerson.IndRefID)
                ? genSelectedPerson.IndRefID!
                : genSelectedPerson.ID.ToString(CultureInfo.CurrentCulture);
            AdditionalInfo = GetFactData(genSelectedPerson, EFactType.Info);
            IsSelectPersonToggled = true;
        }

        private static string GetFactData(IGenPerson genPerson, EFactType eFactType)
            => genPerson.Facts.FirstOrDefault(genFact => genFact?.eFactType == eFactType)?.Data ?? string.Empty;

        private static string FormatDisplayName(IGenPerson genPerson)
        {
            var sSurname = genPerson.Surname ?? string.Empty;
            var sGivenName = genPerson.GivenName ?? string.Empty;
            var sDisplayName = $"{sSurname}, {sGivenName}".Trim(' ', ',');
            return !string.IsNullOrWhiteSpace(sDisplayName)
                ? sDisplayName
                : genPerson.ID.ToString(CultureInfo.CurrentCulture);
        }

        private static string FormatPersonName(IGenPerson? genPerson)
        {
            if (genPerson is null)
            {
                return string.Empty;
            }

            return FormatDisplayName(genPerson);
        }
    }
}
