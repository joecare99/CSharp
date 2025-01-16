using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using NSubstitute;
using MVVM.View.Extension;

namespace MVVM_28_1_CTDataGridExt.Views.Tests;

[TestClass()]
public class DataGridViewTests
{
    [TestMethod()]
    public void DataGridViewTest()
    {
        IoC.GetReqSrv = (t) => Substitute.For(new[] { t }, null!);
        DataGridView? testView=null;
        var t = new Thread(()=> testView = new());
        t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
        t.Start();
        t.Join(); //Wait for the thread to end
        Assert.IsNotNull(testView);
        Assert.IsInstanceOfType(testView, typeof(DataGridView));    
    }
}
