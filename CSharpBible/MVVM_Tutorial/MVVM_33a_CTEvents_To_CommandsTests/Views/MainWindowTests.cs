using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace MVVM_33a_CTEvents_To_Commands.Views.Tests;

[TestClass()]
public class MainWindowTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erw�gen Sie die Deklaration als Nullable.
    MainWindow testView;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erw�gen Sie die Deklaration als Nullable.

    [TestInitialize]
    public void Init()
    {
        var t = new Thread(() => testView = new());
        t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
        t.Start();
        t.Join(); //Wait for the thread to end
    }

    [TestMethod()]
    public void MainWindowTest()
    {
        Assert.IsNotNull(testView);
        Assert.IsInstanceOfType(testView, typeof(MainWindow));    
    }
}
