using AA16_UserControl1.ViewModels.Interfaces;
using Avalonia.ViewModels;
using Avalonia.Views.Extension;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.ComponentModel;

namespace AA16_UserControl1.ViewModels.Tests;

[TestClass()]
public class UserControlViewModelTests : BaseTestViewModel<UserControlViewModel>
{
    protected override Dictionary<string, object?> GetDefaultData() => new() {
            {  "Text", "<Ein Motto>" },
            { "Daten", "<Daten>" },
            { "Do1Command", true },
            { "Do2Command", true }
        };

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
        Assert.IsInstanceOfType(testModel, typeof(UserControlViewModel));
        Assert.IsInstanceOfType(testModel, typeof(ViewModelBase));
        Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
    }

    [TestMethod()]
    public void Do1CommandTest()
    {
        Assert.IsTrue(testModel.Do1Command.CanExecute(null));
        testModel.Do1Command.Execute(null);
        Assert.AreEqual("<Motto>", testModel.Text);
        Assert.AreEqual(string.Empty, testModel.Daten);
        Assert.IsFalse(testModel.Do1Command.CanExecute(null));
        Assert.IsTrue(testModel.Do2Command.CanExecute(null));
    }
    [TestMethod()]
    public void Do2CommandTest()
    {
        Assert.IsTrue(testModel.Do2Command.CanExecute(null));
        testModel.Do2Command.Execute(null);
        Assert.AreEqual("<Daten>", testModel.Daten);
        Assert.AreEqual(string.Empty, testModel.Text);
        Assert.IsFalse(testModel.Do2Command.CanExecute(null));
        Assert.IsTrue(testModel.Do1Command.CanExecute(null));
    }
}