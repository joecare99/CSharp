using Galaxia.Models.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Galaxia.UI.ViewModels
{
    /// <summary>
    /// ViewModel für einen Sektor.
    /// </summary>
    public class SectorViewModel
    {
        private readonly ISector _sector;

        public string Name => _sector.Name;
        public string Position => $"{_sector.Position.X},{_sector.Position.Y},{_sector.Position.Z}";
        public IList<string> Starsystems { get; }

        public SectorViewModel(ISector sector)
        {
            _sector = sector;
            Starsystems = sector.Starsystems.Select(s => s.Name).ToList();
        }
    }
}