using AA06_ValueConverter2.Models.Interfaces;
using NSubstitute;
using Avalonia.Platform;
using System.ComponentModel;
using System.Timers;
using System.Reflection;
using Svg;
using System.Globalization;

namespace AA06_ValueConverter2.Models.Tests;

[TestClass()]
public class ValueConverterModelTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
    private ValueConverterModel testModel;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
    private string sTestLog = "";
    private ICyclTimer? tmr=null;
    private ISysTime? st=null;
    private CultureInfo cc;

    [TestInitialize()]
    public void TestInitialize()
    {
        cc = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        tmr = Substitute.For<ICyclTimer>();
        st = Substitute.For<ISysTime>();
        st.Now.Returns(new DateTime(2025,1,2));
        var pfh = Substitute.For<IPlatformHandle>();
        testModel = new ValueConverterModel(pfh,st,tmr);
        testModel.PropertyChanged += (s, e) => Dolog($"PropChg({s?.GetType().Name}, {e.PropertyName}) = ${s?.GetType().GetProperty(e.PropertyName ?? "")?.GetValue(s)}");
        sTestLog = "";
    }

    [TestCleanup()]
    public void TestCleanup()
    {
        CultureInfo.CurrentCulture = cc;
    }
    private void Dolog(string v)
    {
        sTestLog+=$"{v}\r\n";
    }

    [TestMethod()]
    public void SetupTest()
    {
        Assert.IsNotNull(testModel);
        Assert.IsInstanceOfType(testModel, typeof(ValueConverterModel));
        Assert.IsInstanceOfType(testModel, typeof(IValueConverterModel));
        Assert.IsInstanceOfType(testModel, typeof(INotifyPropertyChanged));
        Assert.IsNotNull(testModel.Now);
        tmr!.Received(1).Start();
    }

    [TestMethod()]
    public void TimerTest()
    {
        st?.Now.Returns(new DateTime(2025, 1, 3));
        // Act
        tmr!.Elapsed += Raise.Event<ElapsedEventHandler>([this,Arg.Any<ElapsedEventArgs>()]);

        // Assert
        Assert.AreEqual("PropChg(ValueConverterModel, Now) = $01/03/2025 00:00:00\r\n", sTestLog);
    }

}
