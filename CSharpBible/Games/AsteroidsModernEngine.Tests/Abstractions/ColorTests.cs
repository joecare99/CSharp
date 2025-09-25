using AsteroidsModern.Engine.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidsModernEngine.Abstractions.Tests;
[TestClass]
public class ColorTests
{
    [TestMethod]
    [DataRow(255u, 0u, 0u, 255u, DisplayName = "Rot, voll deckend")] // Rot, voll deckend
    [DataRow(0u, 255u, 0u, 128u, DisplayName = "Grün, halb transparent")] // Grün, halb transparent
    [DataRow(0u, 0u, 255u, 0u, DisplayName = "Blau, voll transparent")]   // Blau, voll transparent
    [DataRow(255u, 255u, 0u, 200u, DisplayName = "Gelb, teilweise transparent")] // Gelb, teilweise transparent
    [DataRow(0u, 0u, 0u, 255u, DisplayName = "Schwarz, voll deckend")]   // Schwarz, voll deckend
    [DataRow(255u, 255u, 255u, 255u, DisplayName = "Weiß, voll deckend")] // Weiß, voll deckend
    public void Color_ShouldInitializeCorrectly(uint uiR, uint uiG, uint uiB, uint uiA)
    {
        (byte R, byte G, byte B, byte A) = (Convert.ToByte(uiR), Convert.ToByte(uiG), Convert.ToByte(uiB), Convert.ToByte(uiA));    
        var color = new Color(R, G, B, A);
        Assert.AreEqual(A, color.A);
        Assert.AreEqual(R, color.R);
        Assert.AreEqual(G, color.G);
        Assert.AreEqual(B, color.B);
    }

    [TestMethod]
    public void Color_StaticColors_ShouldHaveCorrectValues()
    {
        Assert.AreEqual(new Color(0, 0, 0), Color.Black);
        Assert.AreEqual(new Color(255, 255, 255), Color.White);
        Assert.AreEqual(new Color(128, 128, 128), Color.Gray);
    }
}
