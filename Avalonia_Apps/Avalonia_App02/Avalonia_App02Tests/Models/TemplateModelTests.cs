using Avalonia_App02.Models.Interfaces;
using NSubstitute;
using Avalonia.Platform;
using System.ComponentModel;

namespace Avalonia_App02.Models.Tests;

[TestClass()]
public class TemplateModelTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
    private TemplateModel testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.

    [TestInitialize()]
    public void TestInitialize()
    {
        var tmr = Substitute.For<ICyclTimer>();
        var st = Substitute.For<ISysTime>();
        st.Now.Returns(new DateTime(2025,1,2));
        var pfh = Substitute.For<IPlatformHandle>();
        testModel = new TemplateModel(pfh,st,tmr);
    }

    [TestMethod()]
    public void SetupTest()
    {
        Assert.IsNotNull(testModel);
        Assert.IsInstanceOfType(testModel, typeof(TemplateModel));
        Assert.IsInstanceOfType(testModel, typeof(ITemplateModel));
        Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
        Assert.IsNotNull(testModel.Now);
    }
}