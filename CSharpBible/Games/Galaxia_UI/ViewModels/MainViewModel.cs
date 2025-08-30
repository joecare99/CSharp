using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Galaxia.Models;
using Galaxia.Models.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;

namespace Galaxia.UI.ViewModels
{
    /// <summary>
    /// Zentrales ViewModel (nutzt CommunityToolkit.Mvvm + DI).
    /// </summary>
    public partial class MainViewModel : ObservableObject
    {
        private readonly ISpace _space;
        private readonly ICorporation _corp;

        public ObservableCollection<SectorViewModel> Sectors { get; } = new();

        [ObservableProperty]
        private string status = "Bereit";

        public MainViewModel(ISpace space, ICorporation corporation)
        {
            _space = space;
            _corp = corporation;
            // Bereits initialisiert im DI, aber UI erst nach Klick füllen
        }

        [RelayCommand]
        private void Initialize()
        {
            Sectors.Clear();
            foreach (var sector in _space.Sectors.Values)
                Sectors.Add(new SectorViewModel(sector));

            Status = $"Sektoren geladen: {Sectors.Count}";
        }

        [RelayCommand(CanExecute = nameof(CanEmbark))]
        private void EmbarkTestFleet()
        {
            var firstStar = _space.Sectors.Values.SelectMany(v => v.Starsystems).FirstOrDefault();
            if (firstStar == null)
            {
                Status = "Kein Starsystem gefunden.";
                return;
            }

            var fleet = new Fleet(_corp, 5, firstStar);
            if (_corp.hyperspace.Embark(fleet))
                Status = "Flotte eingeschifft.";
            else
                Status = "Embark fehlgeschlagen.";

            EmbarkTestFleetCommand.NotifyCanExecuteChanged();
        }

        private bool CanEmbark() => _corp.hyperspace.IsAvailible;

        [RelayCommand]
        private void DoubleJumpDemo()
        {
            Status = "DoubleJump-Demo (Platzhalter).";
        }
    }
}