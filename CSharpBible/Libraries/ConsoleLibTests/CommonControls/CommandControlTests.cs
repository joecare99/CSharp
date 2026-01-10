using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleLib.CommonControls;
using System.Windows.Input;
using NSubstitute;

namespace ConsoleLib.CommonControls.Tests;

[TestClass]
public class CommandControlTests : TestBase
{
    private class TestCmdCtrl : CommandControl { public bool Clicked; public override void Click(){ base.Click(); Clicked = true; } }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Command_Set_Attaches_And_Evaluates_CanExecute(bool initialCanExec)
    {
        var ctrl = new TestCmdCtrl();
        var cmd = Substitute.For<ICommand>();
        cmd.CanExecute(Arg.Any<object?>()).Returns(initialCanExec);
        ctrl.Command = cmd;
        Assert.AreEqual(initialCanExec, ctrl.Enabled);

        cmd.CanExecute(Arg.Any<object?>()).Returns(!initialCanExec);
        cmd.CanExecuteChanged += Raise.Event();
        Assert.AreEqual(!initialCanExec, ctrl.Enabled);
    }

    [TestMethod]
    public void Click_Executes_Command_When_Enabled()
    {
        var ctrl = new TestCmdCtrl();
        var cmd = Substitute.For<ICommand>();
        cmd.CanExecute(Arg.Any<object?>()).Returns(true);
        ctrl.Command = cmd;
        ctrl.Click();
        Assert.IsTrue(ctrl.Clicked);
        cmd.Received(1).Execute(Arg.Any<object?>());
    }

    [TestMethod]
    public void Click_Ignored_When_Disabled()
    {
        var ctrl = new TestCmdCtrl();
        var cmd = Substitute.For<ICommand>();
        cmd.CanExecute(Arg.Any<object?>()).Returns(false);
        ctrl.Command = cmd;
        ctrl.Click();
        cmd.DidNotReceive().Execute(Arg.Any<object?>());
    }
}