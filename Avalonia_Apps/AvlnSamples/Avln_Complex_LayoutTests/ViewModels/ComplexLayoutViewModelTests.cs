using Microsoft.VisualStudio.TestTools.UnitTesting;
using Avln.ComplexLayout.ViewModels;

namespace Avln_Complex_LayoutTests.ViewModels;

[TestClass]
public class ComplexLayoutViewModelTests
{
 [TestMethod]
 [DataRow("Button1 was clicked", true)]
 public void Button1_Shows_Message(string expected, bool visible)
 {
 var vm = new ComplexLayoutViewModel();
 vm.Button1Command.Execute(null);
 Assert.AreEqual(expected, vm.MessageText);
 Assert.AreEqual(visible, vm.ShowMessage);
 }

 [TestMethod]
 [DataRow("Button2 was clicked", true)]
 public void Button2_Shows_Message(string expected, bool visible)
 {
 var vm = new ComplexLayoutViewModel();
 vm.Button2Command.Execute(null);
 Assert.AreEqual(expected, vm.MessageText);
 Assert.AreEqual(visible, vm.ShowMessage);
 }

 [TestMethod]
 public void Msg_OK_Hides_Message()
 {
 var vm = new ComplexLayoutViewModel();
 vm.Button1Command.Execute(null);
 vm.Msg_OKCommand.Execute(null);
 Assert.IsFalse(vm.ShowMessage);
 }
}
