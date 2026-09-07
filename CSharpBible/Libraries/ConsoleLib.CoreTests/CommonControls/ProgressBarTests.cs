using ConsoleLib.CommonControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.Tests;

[TestClass]
public class ProgressBarTests
{
    [TestMethod]
    public void ProgressBar_ClampsValueAndCalculatesFraction()
    {
        var progress = new ProgressBar { Minimum = 0, Maximum = 10, Value = 15 };

        Assert.AreEqual(10, progress.Value);
        Assert.AreEqual(1, progress.Fraction);
    }
}
