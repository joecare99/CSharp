using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using MVVM.ViewModel;

namespace MVVM_33a_CTEvents_To_Commands.ViewModels.Tests;

[TestClass()]
public class EventsBindingViewModelTests: BaseTestViewModel
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    EventsBindingViewModel testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    [TestInitialize]
    public void Init()
    {
        testModel = new();
        testModel.PropertyChanged += OnVMPropertyChanged;
        if (testModel is INotifyPropertyChanging inpcn)
            inpcn.PropertyChanging += OnVMPropertyChanging;
        ClearLog();
    }

    [TestMethod()]
    public void SetupTest()
    {
        Assert.IsNotNull(testModel);
        Assert.IsInstanceOfType(testModel, typeof(EventsBindingViewModel));
        Assert.IsInstanceOfType(testModel, typeof(BaseViewModelCT));
        Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
        Assert.AreEqual("", DebugLog);
    }

    [TestMethod]
    public void LostFocusCommandTest()
    {
        testModel.LostFocusCommand.Execute(null);
        Assert.AreEqual("Lost focus", testModel.State);
        Assert.AreEqual("PropChgn(MVVM_33a_CTEvents_To_Commands.ViewModels.EventsBindingViewModel,State)=\r\nPropChg(MVVM_33a_CTEvents_To_Commands.ViewModels.EventsBindingViewModel,State)=Lost focus\r\n", DebugLog);
    }

    [TestMethod]
    public void GotFocusCommandTest()
    {
        testModel.GotFocusCommand.Execute(null);
        Assert.AreEqual("Got focus", testModel.State);
        Assert.AreEqual("PropChgn(MVVM_33a_CTEvents_To_Commands.ViewModels.EventsBindingViewModel,State)=\r\nPropChg(MVVM_33a_CTEvents_To_Commands.ViewModels.EventsBindingViewModel,State)=Got focus\r\n", DebugLog);
    }
}
