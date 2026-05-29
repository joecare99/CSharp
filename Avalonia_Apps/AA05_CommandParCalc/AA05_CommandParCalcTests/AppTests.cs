using NSubstitute;
using Avalonia.Controls.ApplicationLifetimes;

namespace AA05_CommandParCalc.Tests;

class TestApp : App;

[TestClass()]
public class AppTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
    TestApp testApp;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.

    [TestInitialize()]
    public void TestInitialize()
    {
        testApp = new TestApp();
    }

    [TestMethod()]
    public void SetupTest()
    {
        Assert.IsNotNull(testApp);
        Assert.IsInstanceOfType(testApp, typeof(TestApp));
        Assert.IsInstanceOfType(testApp, typeof(App));
    }

    [TestMethod()]
    public void InitializeTest()
    {
        // Act
        testApp.Initialize();

        // Assert
        Assert.IsNull(testApp.ApplicationLifetime);


    }
    [TestMethod()]
    public void InitDesktopTest()
    {
        testApp.ApplicationLifetime = Substitute.For<IClassicDesktopStyleApplicationLifetime>();

        Assert.ThrowsExactly<InvalidOperationException>(() => testApp.OnFrameworkInitializationCompleted());

        Assert.IsNotNull(testApp.Services);
    }

    [TestMethod()]
    public void OnFrameworkInitializationCompletedTest()
    {
        testApp.ApplicationLifetime = Substitute.For<IClassicDesktopStyleApplicationLifetime>();

        // Act
        Assert.ThrowsExactly<InvalidOperationException>(() => testApp.OnFrameworkInitializationCompleted());

        // Assert
        Assert.IsNotNull(testApp.Services);
    }

    [TestMethod()]
    public void OnFrameworkInitializationCompletedTest2()
    {
        testApp.ApplicationLifetime = Substitute.For<IApplicationLifetime>();

        // Act
        testApp.OnFrameworkInitializationCompleted();

        // Assert
        Assert.IsNotNull(testApp.Services);
    }

}
