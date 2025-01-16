using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using MVVM.ViewModel;
using System.Windows.Input;
using System.Threading;
using System;
using System.Windows.Interop;

namespace MVVM_34a_CTBindingEventArgs.ViewModels.Tests;

[TestClass()]
public class EventBindingViewModelTests : BaseTestViewModel
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
        Assert.AreEqual("PropChgn(MVVM_34a_CTBindingEventArgs.ViewModels.EventsBindingViewModel,State)=\r\nPropChg(MVVM_34a_CTBindingEventArgs.ViewModels.EventsBindingViewModel,State)=Lost focus\r\n", DebugLog);
    }

    [TestMethod]
    public void GotFocusCommandTest()
    {
        testModel.GotFocusCommand.Execute(null);
        Assert.AreEqual("Got focus", testModel.State);
        Assert.AreEqual("PropChgn(MVVM_34a_CTBindingEventArgs.ViewModels.EventsBindingViewModel,State)=\r\nPropChg(MVVM_34a_CTBindingEventArgs.ViewModels.EventsBindingViewModel,State)=Got focus\r\n", DebugLog);
    }

    [TestMethod]
    public void KeyDownCommandTest()
    {
        testModel.KeyDownCommand.Execute(null);
        Assert.AreEqual("TextChanged()", testModel.State);
        Assert.AreEqual("PropChgn(MVVM_34a_CTBindingEventArgs.ViewModels.EventsBindingViewModel,State)=\r\nPropChg(MVVM_34a_CTBindingEventArgs.ViewModels.EventsBindingViewModel,State)=TextChanged()\r\n", DebugLog);
    }

    [TestMethod]
    [DataRow(null, false)]
    [DataRow(Key.None, false)]
    [DataRow(Key.Tab, false)]
    [DataRow(Key.Enter, true)]
    [DataRow(Key.Escape, true)]
    public void CanKeyDownCommandTest(Key? key, bool xExp)
    {
        Assert.AreEqual(xExp, testModel.KeyDownCommand.CanExecute(key is Key k ? GetKeyEventArg(k) : null));
        Assert.AreEqual("", testModel.State);

        static KeyEventArgs GetKeyEventArg(Key key)
        {
            KeyEventArgs? ke = null;
            Func<Key, KeyEventArgs> _Action = (k) => new KeyEventArgs(Keyboard.PrimaryDevice,
        new HwndSource(0, 0, 0, 0, 0, "", IntPtr.Zero),
        0, k)
            {
                RoutedEvent = Keyboard.KeyDownEvent
            };
#if NET5_0_OR_GREATER
            var t = new Thread(()=>ke=_Action(key));
            t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            t.Start();
            Thread.Sleep(10);
            t.Join();
            while (ke == null);
                Thread.Sleep(10);
#else
            ke=_Action(key);
#endif
            return ke;
        }
    }
}
