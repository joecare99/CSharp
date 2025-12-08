using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranspilerLib.Pascal.Models;

namespace TranspilerLib.Pascal.Tests.Models;

[TestClass]
public class LfmTokenTests
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
    private LfmToken testClass;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.

    [TestInitialize]
    public void Init()
    {
        testClass = new LfmToken(LfmTokenType.OBJECT,"button",2,6);
    }

    [TestMethod]
    public void Setup_tests()
    {
        // Assert
        Assert.IsNotNull(testClass);
        Assert.IsInstanceOfType(testClass, typeof(LfmToken));
    }

    [TestMethod]
    public void ToCode_tests()
    {
        // Act
        var code = testClass.ToString();
        // Assert
        Assert.AreEqual("OBJECT: 'button' at 2:6", code);
    }
}
