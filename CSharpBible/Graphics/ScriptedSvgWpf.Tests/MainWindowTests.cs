using System;
using System.Threading;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ScriptedSvgWpf.Tests;

[TestClass]
public sealed class MainWindowTests
{
    [TestMethod]
    public void ConstructorInitializesWindowMetadata()
    {
        RunOnSta(() =>
        {
            var window = new ScriptedSvgWpf.MainWindow();

            Assert.AreEqual("Scripted SVG", window.Title);
            Assert.AreEqual(1200, window.Width);
            Assert.AreEqual(760, window.Height);
            Assert.AreEqual(850, window.MinWidth);
            Assert.AreEqual(520, window.MinHeight);
        });
    }

    private static void RunOnSta(Action action)
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                failure = exception;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        if (failure is not null)
        {
            throw new AssertFailedException(failure.ToString());
        }
    }
}
