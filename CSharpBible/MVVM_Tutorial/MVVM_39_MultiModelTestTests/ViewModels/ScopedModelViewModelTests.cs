using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using MVVM.View.Extension;
using NSubstitute;
using MVVM_39_MultiModelTest.Models;

namespace MVVM_39_MultiModelTest.ViewModels.Tests;

[TestClass()]
public class ScopedModelViewModelTests :BaseTestViewModel<ScopedModelViewModel>
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    private IScopedModel scopedModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

    [TestInitialize]
    public override void Init()
    {
        scopedModel = Substitute.For<IScopedModel>();
        IoC.GetReqSrv = (t) => t switch
        {
            _ when t == typeof(IScopedModel) => scopedModel,
            _ => throw new System.NotImplementedException()
        };
        base.Init();
    }
    [TestMethod()]
    public void SetupTest()
    {
        Assert.IsNotNull(testModel);
        Assert.IsNotNull(testModel2);
        Assert.IsInstanceOfType(testModel, typeof(ScopedModelViewModel));
        Assert.IsInstanceOfType(testModel, typeof(BaseViewModelCT));

    }

    [DataTestMethod()]
    [DataRow(true,new[] { "vmDoClose()\r\n" })]
    [DataRow(false,new[] { "" })]
    public void CloseTest(bool xAct, string[] asExp)
    {
        if (xAct)
        {
            testModel.DoClose = vmDoClose;
        }
        testModel.CloseCommand.Execute(null);
        Assert.AreEqual(asExp[0], DebugLog);

    }

    [DataTestMethod()]
#if NET5_0_OR_GREATER
    [DataRow(true, new[] { "PropChgn(MVVM_39_MultiModelTest.ViewModels.ScopedModelViewModel,FrameName)=/MVVM_39_MultiModelTest_net;component/views/DetailPage1.xaml\r\nPropChg(MVVM_39_MultiModelTest.ViewModels.ScopedModelViewModel,FrameName)=/Views/DetailPage1.xaml\r\n" })]
#else
    [DataRow(true, new[] { "PropChgn(MVVM_39_MultiModelTest.ViewModels.ScopedModelViewModel,FrameName)=/MVVM_39_MultiModelTest;component/views/DetailPage1.xaml\r\nPropChg(MVVM_39_MultiModelTest.ViewModels.ScopedModelViewModel,FrameName)=/Views/DetailPage1.xaml\r\n" })]
#endif
public void Detail1CommandTest(bool xAct, string[] asExp)
    {
        testModel.Detail1Command.Execute(null);
        testModel.FrameName = "/Views/DetailPage1.xaml";
        Assert.AreEqual(asExp[0], DebugLog);
    }

    [DataTestMethod()]
#if NET5_0_OR_GREATER
    [DataRow(true, new[] { "PropChgn(MVVM_39_MultiModelTest.ViewModels.ScopedModelViewModel,FrameName)=/MVVM_39_MultiModelTest_net;component/views/DetailPage1.xaml\r\nPropChg(MVVM_39_MultiModelTest.ViewModels.ScopedModelViewModel,FrameName)=/MVVM_39_MultiModelTest_net;component/views/DetailPage2.xaml\r\nPropChgn(MVVM_39_MultiModelTest.ViewModels.ScopedModelViewModel,FrameName)=/MVVM_39_MultiModelTest_net;component/views/DetailPage2.xaml\r\nPropChg(MVVM_39_MultiModelTest.ViewModels.ScopedModelViewModel,FrameName)=/Views/DetailPage2.xaml\r\n" })]
#else
    [DataRow(true, new[] { "PropChgn(MVVM_39_MultiModelTest.ViewModels.ScopedModelViewModel,FrameName)=/MVVM_39_MultiModelTest;component/views/DetailPage1.xaml\r\nPropChg(MVVM_39_MultiModelTest.ViewModels.ScopedModelViewModel,FrameName)=/MVVM_39_MultiModelTest;component/views/DetailPage2.xaml\r\nPropChgn(MVVM_39_MultiModelTest.ViewModels.ScopedModelViewModel,FrameName)=/MVVM_39_MultiModelTest;component/views/DetailPage2.xaml\r\nPropChg(MVVM_39_MultiModelTest.ViewModels.ScopedModelViewModel,FrameName)=/Views/DetailPage2.xaml\r\n" })]
#endif
    public void Detail2CommandTest(bool xAct, string[] asExp)
    {
        testModel.Detail2Command.Execute(null);
        testModel.FrameName = "/Views/DetailPage2.xaml";
        Assert.AreEqual(asExp[0], DebugLog);
    }

    [DataTestMethod()]
#if NET5_0_OR_GREATER
    [DataRow(true, new[] { "PropChgn(MVVM_39_MultiModelTest.ViewModels.ScopedModelViewModel,FrameName)=/MVVM_39_MultiModelTest_net;component/views/DetailPage1.xaml\r\nPropChg(MVVM_39_MultiModelTest.ViewModels.ScopedModelViewModel,FrameName)=/MVVM_39_MultiModelTest_net;component/views/DetailPage3.xaml\r\nPropChgn(MVVM_39_MultiModelTest.ViewModels.ScopedModelViewModel,FrameName)=/MVVM_39_MultiModelTest_net;component/views/DetailPage3.xaml\r\nPropChg(MVVM_39_MultiModelTest.ViewModels.ScopedModelViewModel,FrameName)=/Views/DetailPage3.xaml\r\n" })]
#else
    [DataRow(true, new[] { "PropChgn(MVVM_39_MultiModelTest.ViewModels.ScopedModelViewModel,FrameName)=/MVVM_39_MultiModelTest;component/views/DetailPage1.xaml\r\nPropChg(MVVM_39_MultiModelTest.ViewModels.ScopedModelViewModel,FrameName)=/MVVM_39_MultiModelTest;component/views/DetailPage3.xaml\r\nPropChgn(MVVM_39_MultiModelTest.ViewModels.ScopedModelViewModel,FrameName)=/MVVM_39_MultiModelTest;component/views/DetailPage3.xaml\r\nPropChg(MVVM_39_MultiModelTest.ViewModels.ScopedModelViewModel,FrameName)=/Views/DetailPage3.xaml\r\n" })]
#endif
    public void Detail3CommandTest(bool xAct, string[] asExp)
    {
        testModel.Detail3Command.Execute(null);
        testModel.FrameName = "/Views/DetailPage3.xaml";
        Assert.AreEqual(asExp[0], DebugLog);
    }

    private void vmDoClose(object obj)
    {
        DoLog("vmDoClose()");
    }
}