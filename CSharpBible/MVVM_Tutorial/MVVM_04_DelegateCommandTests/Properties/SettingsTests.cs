using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;

namespace MVVM_04_DelegateCommand.Properties.Tests
{
    [TestClass()]
    public class SettingsTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        Settings testItem;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init()
        {
            testItem = new();
        }

        [TestMethod()]
        public void SetupTest()
        {
            Assert.IsNotNull(testItem);
            Assert.IsInstanceOfType(testItem, typeof(Settings));
            Assert.IsInstanceOfType(testItem, typeof(ApplicationSettingsBase));
        }
        [TestMethod()]
        public void DefaultInstanceTest()
        {
            Assert.IsNotNull(Settings.Default);
            Assert.IsInstanceOfType(Settings.Default, typeof(Settings));
            Assert.IsInstanceOfType(Settings.Default, typeof(ApplicationSettingsBase));
        }
    }
}