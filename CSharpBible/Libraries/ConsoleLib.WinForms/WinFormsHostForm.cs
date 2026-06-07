using ConsoleLib.Interfaces;
using System;
using System.Drawing;
using WinFormsForm = System.Windows.Forms.Form;
using WinFormsFormStartPosition = System.Windows.Forms.FormStartPosition;
using WinFormsTimer = System.Windows.Forms.Timer;

namespace ConsoleLib.WinForms;

/// <summary>
/// Default WinForms host form for a ConsoleLib application.
/// </summary>
public sealed class WinFormsHostForm : WinFormsForm
{
    public const int CharacterWidth = 12;
    public const int CharacterHeight = 18;

    private readonly IApplication _application;
    private readonly WinFormsTimer _messageTimer;

    public WinFormsHostForm(IApplication application)
    {
        _application = application;
        Text = string.IsNullOrWhiteSpace(application.Text) ? "ConsoleLibApplication" : application.Text;
        StartPosition = WinFormsFormStartPosition.CenterScreen;
        Width = Math.Max(640, application.size.Width * CharacterWidth);
        Height = Math.Max(480, application.size.Height * CharacterHeight);

        _messageTimer = new WinFormsTimer
        {
            Interval = 15,
            Enabled = true
        };
        _messageTimer.Tick += (_, _) => _application.ProcessPendingMessages();
        FormClosed += (_, _) => _messageTimer.Dispose();
    }

    public static Rectangle ToPixelBounds(Rectangle charBounds)
    {
        return new Rectangle(
            charBounds.X * CharacterWidth,
            charBounds.Y * CharacterHeight,
            Math.Max(1, charBounds.Width * CharacterWidth),
            Math.Max(1, charBounds.Height * CharacterHeight));
    }
}