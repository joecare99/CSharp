using AA16_UserControl1.ViewModels.Interfaces;
using Avalonia.ViewModels;
using Avalonia.Views.Extension;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.ComponentModel;

namespace AA16_UserControl1.ViewModels.Tests;

[TestClass()]
public class MainWindowViewModelTests : BaseTestViewModel<MainWindowViewModel>
{
    [TestInitialize]
    public override void Init()
    {
        IoC.GetReqSrv = t =>
        {
            if (t == typeof(IUserControlViewModel))
            {
                return Substitute.For<IUserControlViewModel>();
            }
            throw new System.NotImplementedException($"No registration for type {t}.");
        };
        base.Init();
    }  

    [TestMethod()]
    public void SetupTest()
    {
        Assert.IsNotNull(testModel);
        Assert.IsInstanceOfType(testModel, typeof(MainWindowViewModel));
        Assert.IsInstanceOfType(testModel, typeof(ViewModelBase));
        Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
    }
}