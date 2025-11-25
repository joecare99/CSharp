using AA16_UserControl1.ViewModels.Interfaces;
using Avalonia.Headless.MSTest;
using Avalonia.Views.Extension;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Threading;

namespace AA16_UserControl1.Views.Tests;

[TestClass()]
public class UserControlViewTests
{

    [TestInitialize]
    public void TestInitialize()
    {
        IoC.GetReqSrv = t =>
        {
            if (t == typeof(IUserControlViewModel))
            {
                return Substitute.For<IUserControlViewModel>();
            }
            throw new System.NotImplementedException($"No registration for type {t}.");
        };
    }

    [AvaloniaTestMethod]
    public void UserControlViewTest()
    {
        UserControlView? testView = new()
        {
            Width = 800,
            Height = 600,
        };
         Assert.IsNotNull(testView);
        Assert.IsInstanceOfType(testView, typeof(UserControlView));    
    }
}