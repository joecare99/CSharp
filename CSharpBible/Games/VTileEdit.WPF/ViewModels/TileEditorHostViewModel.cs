using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using VTileEdit.ViewModels;

namespace VTileEdit.WPF.ViewModels;

/// <summary>
/// Host view-model for the right hand editor area in the main window.
/// </summary>
public sealed partial class TileEditorHostViewModel : ObservableObject
{
    private static readonly string[] PreferredConsoleFonts = new[] { "Consolas", "Cascadia Mono", "Cascadia Code", "Lucida Console", "Courier New" };

    /// <summary>
    /// Initializes a new instance of the <see cref="TileEditorHostViewModel"/> class.
    /// </summary>
    /// <param name="editor">The tile editor view-model (core).</param>
    public TileEditorHostViewModel(TileEditorViewModel editor)
    {
        Editor = editor ?? throw new ArgumentNullException(nameof(editor));

        Palette = new ObservableCollection<ColorSwatchViewModel>(Enum.GetValues<ConsoleColor>().Select(color => new ColorSwatchViewModel(color)));
        AvailableFontFamilies = new ObservableCollection<FontFamily>(GetConsoleFontFamilies());
        SelectedFontFamily = AvailableFontFamilies.FirstOrDefault() ?? SystemFonts.MessageFontFamily;

        ApplyForegroundCommand = new RelayCommand<ColorSwatchViewModel?>(ApplyForeground, swatch => swatch != null);
        ApplyBackgroundCommand = new RelayCommand<ColorSwatchViewModel?>(ApplyBackground, swatch => swatch != null);
    }

    /// <summary>
    /// Gets the editor view-model.
    /// </summary>
    public TileEditorViewModel Editor { get; }

    /// <summary>
    /// Gets the palette used for both foreground and background colors.
    /// </summary>
    public ObservableCollection<ColorSwatchViewModel> Palette { get; }

    /// <summary>
    /// Gets the list of console-friendly font families available to the application.
    /// </summary>
    public ObservableCollection<FontFamily> AvailableFontFamilies { get; }

    [ObservableProperty]
    private FontFamily selectedFontFamily = SystemFonts.MessageFontFamily;

    /// <summary>
    /// Gets the command that applies a foreground color.
    /// </summary>
    public IRelayCommand<ColorSwatchViewModel?> ApplyForegroundCommand { get; }

    /// <summary>
    /// Gets the command that applies a background color.
    /// </summary>
    public IRelayCommand<ColorSwatchViewModel?> ApplyBackgroundCommand { get; }

    private void ApplyForeground(ColorSwatchViewModel? swatch)
    {
        if (swatch == null)
        {
            return;
        }

        Editor.ApplyForegroundCommand.Execute(swatch.Color);
    }

    private void ApplyBackground(ColorSwatchViewModel? swatch)
    {
        if (swatch == null)
        {
            return;
        }

        Editor.ApplyBackgroundCommand.Execute(swatch.Color);
    }

    private static IEnumerable<FontFamily> GetConsoleFontFamilies()
    {
        var installed = Fonts.SystemFontFamilies.ToList();
        var added = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var preferred in PreferredConsoleFonts)
        {
            var match = installed.FirstOrDefault(f => string.Equals(f.Source, preferred, StringComparison.OrdinalIgnoreCase) ||
                                                      f.FamilyNames.Values.Any(name => string.Equals(name, preferred, StringComparison.OrdinalIgnoreCase)));
            if (match != null && added.Add(match.Source))
            {
                yield return match;
            }
        }

        foreach (var candidate in installed.Where(IsConsoleCandidate))
        {
            if (added.Add(candidate.Source))
            {
                yield return candidate;
            }
        }

        if (added.Add(SystemFonts.MessageFontFamily.Source))
        {
            yield return SystemFonts.MessageFontFamily;
        }
    }

    private static bool IsConsoleCandidate(FontFamily family)
    {
        var source = family.Source;
        return source.Contains("Mono", StringComparison.OrdinalIgnoreCase)
            || source.Contains("Console", StringComparison.OrdinalIgnoreCase)
            || source.Contains("Courier", StringComparison.OrdinalIgnoreCase)
            || source.Contains("Code", StringComparison.OrdinalIgnoreCase);
    }
}