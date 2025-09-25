using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using AsteroidsModern.Engine.Services;
using AsteroidsModern.Engine.Abstractions;
using System.Numerics;

namespace AsteroidsModernEngine.Services.Tests;

/*
Pseudocode / Plan:
- Helper: MakeInput(left, right, fire, thrust) => IGameInput Substitute mit fixen Rückgaben.
- Helper: UpdateFrame(ts, ...) => ts.Update(MakeInput(...)).
- Helper: PressLeft/PressRight/PressFire/PressThrust => 1 Frame mit Taste, dann 1 Frame ohne (Edge-Detection).
- Helper: EnterOptions/EnterHighscores/EnterCredits => Navigation per Right-Presses und Fire.
- Helper: GetScreenName(ts) => Reflection liest private Field "_screen" (enum) und gibt Name zurück.

Tests (MSTest + NSubstitute, DataRow für Einzeltests):
1) StartGame_FireOnTitle_RaisesEvent
   - DataTestMethod mit einer DataRow.
   - Fire drücken => StartGameRequested wurde einmal ausgelöst.

2) StartGame_ThrustOnTitle_RaisesEvent
   - DataTestMethod mit einer DataRow.
   - Thrust drücken => StartGameRequested wurde einmal ausgelöst.

3) Title_NavigateRight_Fire_OpensExpectedScreen
   - DataTestMethod mit DataRows: stepsRight=1->Options, 2->Highscores, 3->Credits.
   - Rechts so oft drücken, dann Fire => Screen reflektieren und vergleichen.

4) Title_NavigateLeft_Wraps_ToCredits
   - DataTestMethod (eine DataRow).
   - Einmal Links, dann Fire => Screen "Credits".

5) Options_ToggleSound_And_ChangeLives_OneStep
   - DataTestMethod mit DataRows: inc=true->StartLives=4, inc=false->StartLives=2.
   - In Optionen: einmal Taste (Sound toggelt), nochmal gleiche Taste (Lives +/-1).

6) Options_StartLives_Clamped
   - DataTestMethod mit DataRows: incCount=+20->9, incCount=-20->1.
   - In Optionen: Wiederhole Paar-Drücke (Togglen+Ändern) entsprechend Inkrement/Dekrement.

7) SetLastScore_UpdatesHighscore_And_ReturnsToTitle
   - DataTestMethod mit DataRow: score=50->HighScore=50, Screen=Title.

8) SetLastScore_DoesNotLowerHighscore
   - DataTestMethod (eine DataRow).
   - Zuerst hoher Score setzen, dann kleiner => HighScore bleibt hoch, Screen=Title.

9) Options_EdgeDetection_NoRepeatOnHold
   - DataTestMethod (eine DataRow).
   - In Optionen: Ein Frame Right=true, direkt danach erneut Right=true ohne Release:
     => Nur ein Toggle (Sound), kein Lives-Ändern; nach Release und erneutem Press wieder Toggle.
*/

[TestClass]
public class TitleScreenTests
{
    // Helpers

    private static IGameInput MakeInput(bool left = false, bool right = false, bool fire = false, bool thrust = false)
    {
        var input = Substitute.For<IGameInput>();
        input.IsDown(GameKey.Left).Returns(left);
        input.IsDown(GameKey.Right).Returns(right);
        input.IsDown(GameKey.Fire).Returns(fire);
        input.IsDown(GameKey.Thrust).Returns(thrust);
        return input;
    }

    private static void UpdateFrame(TitleScreen ts, bool left = false, bool right = false, bool fire = false, bool thrust = false)
        => ts.Update(MakeInput(left, right, fire, thrust));

    private static void PressLeft(TitleScreen ts)
    {
        UpdateFrame(ts, left: true);
        UpdateFrame(ts);
    }

    private static void PressRight(TitleScreen ts)
    {
        UpdateFrame(ts, right: true);
        UpdateFrame(ts);
    }

    private static void PressFire(TitleScreen ts)
    {
        UpdateFrame(ts, fire: true);
        UpdateFrame(ts);
    }

    private static void PressThrust(TitleScreen ts)
    {
        UpdateFrame(ts, thrust: true);
        UpdateFrame(ts);
    }

    private static void EnterOptions(TitleScreen ts)
    {
        PressRight(ts); // -> index 1
        PressFire(ts);
    }

    private static void EnterHighscores(TitleScreen ts)
    {
        PressRight(ts);
        PressRight(ts); // -> index 2
        PressFire(ts);
    }

    private static void EnterCredits(TitleScreen ts)
    {
        PressRight(ts);
        PressRight(ts);
        PressRight(ts); // -> index 3
        PressFire(ts);
    }

    private static string GetScreenName(TitleScreen ts)
    {
        var field = typeof(TitleScreen).GetField("_screen", BindingFlags.NonPublic | BindingFlags.Instance);
        var value = field!.GetValue(ts)!;
        return value.ToString()!;
    }

    // Tests

    [DataTestMethod]
    [DataRow(true)]
    public void StartGame_FireOnTitle_RaisesEvent(bool _)
    {
        var ts = new TitleScreen();
        int startEvents = 0;
        ts.StartGameRequested += () => startEvents++;

        PressFire(ts);

        Assert.AreEqual(1, startEvents, "StartGameRequested sollte genau einmal ausgelöst werden.");
    }

    [DataTestMethod]
    [DataRow(true)]
    public void StartGame_ThrustOnTitle_RaisesEvent(bool _)
    {
        var ts = new TitleScreen();
        int startEvents = 0;
        ts.StartGameRequested += () => startEvents++;

        PressThrust(ts);

        Assert.AreEqual(1, startEvents, "StartGameRequested sollte genau einmal ausgelöst werden.");
    }

    [DataTestMethod]
    [DataRow(1, "Options")]
    [DataRow(2, "Highscores")]
    [DataRow(3, "Credits")]
    public void Title_NavigateRight_Fire_OpensExpectedScreen(int stepsRight, string expectedScreen)
    {
        var ts = new TitleScreen();

        for (int i = 0; i < stepsRight; i++)
            PressRight(ts);

        PressFire(ts);

        Assert.AreEqual(expectedScreen, GetScreenName(ts));
    }

    [DataTestMethod]
    [DataRow(true)]
    public void Title_NavigateLeft_Wraps_ToCredits(bool _)
    {
        var ts = new TitleScreen();

        PressLeft(ts); // von 0 nach 3 (Wrap)
        PressFire(ts);

        Assert.AreEqual("Credits", GetScreenName(ts));
    }

    [DataTestMethod]
    [DataRow(true, 4)]
    [DataRow(false, 2)]
    public void Options_ToggleSound_And_ChangeLives_OneStep(bool increase, int expectedLives)
    {
        var ts = new TitleScreen();
        EnterOptions(ts);

        bool initialSound = ts.SoundEnabled;

        if (increase)
        {
            // Toggle Sound
            PressRight(ts);
            // Change Lives +1
            PressRight(ts);
        }
        else
        {
            // Toggle Sound
            PressLeft(ts);
            // Change Lives -1
            PressLeft(ts);
        }

        // Sound wurde genau einmal getoggelt
        Assert.AreNotEqual(initialSound, ts.SoundEnabled);
        Assert.AreEqual(expectedLives, ts.StartLives);
    }

    [DataTestMethod]
    [DataRow(20, 9)]
    [DataRow(-20, 1)]
    public void Options_StartLives_Clamped(int steps, int expectedLives)
    {
        var ts = new TitleScreen();
        EnterOptions(ts);

        if (steps >= 0)
        {
            for (int i = 0; i < steps; i++)
            {
                PressRight(ts); // toggle sound
                PressRight(ts); // lives +1
            }
        }
        else
        {
            for (int i = 0; i < -steps; i++)
            {
                PressLeft(ts); // toggle sound
                PressLeft(ts); // lives -1
            }
        }

        Assert.AreEqual(expectedLives, ts.StartLives);
    }

    [DataTestMethod]
    [DataRow(50, 50)]
    public void SetLastScore_UpdatesHighscore_And_ReturnsToTitle(int score, int expectedHigh)
    {
        var ts = new TitleScreen();

        // Simuliere: gehe in Highscores, dann SetLastScore sollte wieder Titel setzen
        EnterHighscores(ts);
        Assert.AreEqual("Highscores", GetScreenName(ts));

        ts.SetLastScore(score);

        Assert.AreEqual("Title", GetScreenName(ts));
        Assert.AreEqual(expectedHigh, ts.HighScore);
    }

    [DataTestMethod]
    [DataRow(true)]
    public void SetLastScore_DoesNotLowerHighscore(bool _)
    {
        var ts = new TitleScreen();

        ts.SetLastScore(100);
        Assert.AreEqual(100, ts.HighScore);

        ts.SetLastScore(50);
        Assert.AreEqual(100, ts.HighScore);
        Assert.AreEqual("Title", GetScreenName(ts));
    }

    [DataTestMethod]
    [DataRow(true)]
    public void Options_EdgeDetection_NoRepeatOnHold(bool _)
    {
        var ts = new TitleScreen();
        EnterOptions(ts);

        bool initialSound = ts.SoundEnabled;

        // Frame 1: Right down (edge -> toggle sound)
        UpdateFrame(ts, right: true);
        Assert.AreNotEqual(initialSound, ts.SoundEnabled);
        var soundAfterFirst = ts.SoundEnabled;

        // Frame 2: Right still down (no edge -> no change)
        UpdateFrame(ts, right: true);
        Assert.AreEqual(soundAfterFirst, ts.SoundEnabled, "Kein weiterer Toggle ohne Release erwartet.");

        // Release
        UpdateFrame(ts);

        // Press again -> edge -> toggle back
        UpdateFrame(ts, right: true);
        Assert.AreEqual(initialSound, ts.SoundEnabled, "Nach erneutem Edge sollte Sound wieder togglen.");
    }

    [TestMethod()]
    [DataRow(TitleScreen.Screen.Title,7)]
    [DataRow(TitleScreen.Screen.Options,5)]
    [DataRow(TitleScreen.Screen.Highscores,4)]
    [DataRow(TitleScreen.Screen.Credits,4)]
    public void RenderTest(TitleScreen.Screen scn,int nT)
    {
        var ts = new TitleScreen();
        // Set screen via Reflection
        var field = typeof(TitleScreen).GetField("_screen", BindingFlags.NonPublic | BindingFlags.Instance);
        field!.SetValue(ts, scn);
        var ctx = Substitute.For<IRenderContext>();

        ts.Render(ctx);
        
     //   ctx.Received().Clear(Color.Black);
        ctx.Received(nT).DrawText(Arg.Any<string>(), Arg.Any<Vector2>(), Arg.Any<Color>(), Arg.Any<float>());
    }

}