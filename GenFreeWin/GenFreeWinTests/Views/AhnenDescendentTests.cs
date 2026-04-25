using System.Reflection;
using Gen_FreeWin.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using GenFree.Data; // für DataModul-Typen

namespace GenFreeWinTests.Views
{
    [TestClass]
    public class AhnenDescendentTests
    {
        [TestMethod]
        public void PersistDescendentGeneration_WritesToDescendentAndOrtindTables()
        {
            // Arrange
            var form = new Ahnen();

            // Recordsets mocken
            var rsDesc = Substitute.For<GenFree.Interfaces.DB.IRecordset>();
            var rsOrt  = Substitute.For<GenFree.Interfaces.DB.IRecordset>();

            DataModul.DT_DescendentTable = rsDesc;
            DataModul.NB_OrtindTable     = rsOrt;

            // List1 und interne Felder vorbereiten
            form.List1.Items.Add("20200101".PadRight(9) + "123".PadLeft(10)); // Datum + PerNr

            var genField = typeof(Ahnen).GetField("Gen", BindingFlags.Instance | BindingFlags.NonPublic)!;
            genField.SetValue(form, 2);

            var nachnrField = typeof(Ahnen).GetField("_nachnr", BindingFlags.Instance | BindingFlags.NonPublic)!;
            nachnrField.SetValue(form, "1");

            // Act
            var mi = typeof(Ahnen).GetMethod("PersistDescendentGeneration",
                BindingFlags.Instance | BindingFlags.NonPublic)!;
            mi.Invoke(form, null);

            // Assert: AddNew/Update wurden auf beiden Recordsets verwendet
            rsDesc.Received().AddNew();
            rsDesc.Received().Update();
            rsOrt.Received().AddNew();
            rsOrt.Received().Update();
        }
    }
}