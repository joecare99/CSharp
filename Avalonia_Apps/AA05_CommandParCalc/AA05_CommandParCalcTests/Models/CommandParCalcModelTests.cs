using AA05_CommandParCalc.Models.Interfaces;
using NSubstitute;
using Avalonia.Platform;
using System.ComponentModel;

namespace AA05_CommandParCalc.Models.Tests;

[TestClass()]
public class CommandParCalcModelTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
    private CommandParCalcModel testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.

    [TestInitialize()]
    public void TestInitialize()
    {
        var tmr = Substitute.For<ICyclTimer>();
        var st = Substitute.For<ISysTime>();
        st.Now.Returns(new DateTime(2025,1,2));
        var pfh = Substitute.For<IPlatformHandle>();
        testModel = new CommandParCalcModel(pfh,st,tmr);
    }

    [TestMethod()]
    public void SetupTest()
    {
        Assert.IsNotNull(testModel);
        Assert.IsInstanceOfType(testModel, typeof(CommandParCalcModel));
        Assert.IsInstanceOfType(testModel, typeof(ICommandParCalcModel));
        Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
        Assert.IsNotNull(testModel.Now);
    }
}
