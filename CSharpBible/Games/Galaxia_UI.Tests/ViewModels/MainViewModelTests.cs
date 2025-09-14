using Galaxia.Models;
using Galaxia.Models.Interfaces;
using Galaxia.UI.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Linq;

namespace Galaxia_UI.Tests.ViewModels
{
    /// <summary>
    /// Tests für das MainViewModel (UI-logiknah, kein Rendering).
    /// </summary>
    [TestClass]
    public class MainViewModelTests
    {
        private MainViewModel CreateVm()
        {
            var space = new Space();
            space.Initialize();
            var firstSector = space.Sectors.Values.First();
            var homeStar = firstSector.Starsystems.First();
            var corp = new Corporation("Test", "Desc", Color.Gold, homeStar, new System.Collections.Generic.List<IStarsystem> { homeStar }, space);
            return new MainViewModel(space, corp);
        }

        [TestMethod]
        public void Initialize_Populates_Sectors()
        {
            var vm = CreateVm();

            vm.InitializeCommand.Execute(null);

            Assert.IsTrue(vm.Sectors.Count > 0, "Sektoren sollten nach Initialisierung vorhanden sein.");
        }

        [TestMethod]
        public void Embark_Command_Changes_Status()
        {
            var vm = CreateVm();
            vm.InitializeCommand.Execute(null);

            vm.EmbarkTestFleetCommand.Execute(null);

            StringAssert.Contains(vm.Status, "Flotte");
        }
    }
}