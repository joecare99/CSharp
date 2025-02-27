using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace MVVM_03a_CTNotifyChange.Views.Tests;

[TestClass()]
public class NotifyChangeViewTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erw�gen Sie die Deklaration als Nullable.
    NotifyChangeView testView;
    private object vm;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erw�gen Sie die Deklaration als Nullable.

    [TestInitialize()]
    public void Init()
    {
        Thread thread = new(() =>
        {
            testView = new();
            vm = testView.DataContext;
        });
        thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
        thread.Start();
        thread.Join(); //Wait for the thread to end
    }

    [TestMethod()]
    public void ValidationPageTest()
    {
        Assert.IsNotNull(testView);
        Assert.IsNotNull(vm);
    }
}
