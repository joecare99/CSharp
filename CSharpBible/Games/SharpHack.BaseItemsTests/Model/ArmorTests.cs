using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpHack.Base.Model;
using SharpHack.BaseItems.Model;

namespace SharpHack.BaseTests.Model;

[TestClass]
public class ArmorTests
{
    [TestMethod]
    public void DefenseBonus_Default_IsZero()
    {
        var sut = new Armor();
        Assert.AreEqual(0, sut.DefenseBonus);
    }

    [TestMethod]
    public void DefenseBonus_CanBeSet_AndReadBack()
    {
        var sut = new Armor { DefenseBonus = 7 };
        Assert.AreEqual(7, sut.DefenseBonus);
    }
}
