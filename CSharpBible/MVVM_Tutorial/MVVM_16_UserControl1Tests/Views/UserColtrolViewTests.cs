using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace MVVM_16_UserControl1.Views.Tests;

[TestClass()]
public class UserControlViewTests
{
    [TestMethod()]
    public void UserControlViewTest()
    {
        UserControlView? testView=null;
        var t = new Thread(()=> testView = new());
        t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
        t.Start();
        t.Join(); //Wait for the thread to end
        Assert.IsNotNull(testView);
        Assert.IsInstanceOfType(testView, typeof(UserControlView));    
    }
}