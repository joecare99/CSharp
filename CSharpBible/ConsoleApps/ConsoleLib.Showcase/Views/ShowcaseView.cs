using ConsoleLib.CommonControls;
using ConsoleLib.Data;
using ConsoleLib.Interfaces;
using ConsoleLib.Showcase.Services;
using ConsoleLib.Showcase.Terminal.Core;
using ConsoleLib.Showcase.ViewModels;
using System;
using System.ComponentModel;
using System.Drawing;
using Terminal.Core;
using TerminalWidget = ConsoleLib.CommonControls.Terminal;

namespace ConsoleLib.Showcase.Views;

/// <summary>Native ConsoleLib/ExtCon gallery view.</summary>
public sealed class ShowcaseView : Application
{
    private readonly ShowcaseViewModel _viewModel;
    private readonly VisualEffects _effects;
    private readonly IShowcaseTerminalService _terminalService;
    private readonly TerminalSnapshotRenderer _snapshotRenderer;
    private readonly TerminalInputRouter _inputRouter;
    private readonly TerminalMouseNegotiator _mouseNegotiator;
    private Label? _statusLabel;
    private Label? _sectionDescription;
    private Label? _inspectorLabel;
    private EffectPanel? _effectPanel;
    private ProgressBar? _progressBar;
    private Dialog? _aboutDialog;
    private TerminalWidget? _terminal;
    private IDisposable? _effectTimer;
    private int _effectFrame;

    public ShowcaseView(
        ShowcaseViewModel viewModel,
        IWidgetSet widgetSet,
        VisualEffects effects,
        TerminalSnapshotRenderer snapshotRenderer,
        TerminalInputRouter inputRouter,
        TerminalMouseNegotiator mouseNegotiator)
        : base(widgetSet)
    {
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        _effects = effects ?? throw new ArgumentNullException(nameof(effects));
        _snapshotRenderer = snapshotRenderer ?? throw new ArgumentNullException(nameof(snapshotRenderer));
        _inputRouter = inputRouter ?? throw new ArgumentNullException(nameof(inputRouter));
        _mouseNegotiator = mouseNegotiator ?? throw new ArgumentNullException(nameof(mouseNegotiator));
        _terminalService = _viewModel.TerminalService;
        _viewModel.RequestClose += ViewModel_RequestClose;
        _viewModel.RequestAbout += ViewModel_RequestAbout;
        _viewModel.RequestCloseAbout += ViewModel_RequestCloseAbout;
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        _terminalService.SnapshotChanged += TerminalService_SnapshotChanged;
        OnCanvasResize += View_OnCanvasResize;

        Visible = false;
        BorderStyle = ConsoleLib.Data.BorderStyle.Double;
        BorderColor = ConsoleColor.DarkCyan;
        ForeColor = ConsoleColor.Gray;
        BackColor = ConsoleColor.Black;
        Dimension = widgetSet.ClipRect;

        BuildGallery();
        Visible = true;
    }

    private void BuildGallery()
    {
        var menuBar = new MenuBar
        {
            Parent = this,
            Position = new Point(1, 1),
            size = new Size(Math.Max(1, Dimension.Width - 2), 1),
            BackColor = ConsoleColor.DarkGray,
            ForeColor = ConsoleColor.Black
        };
        var helpPopup = new MenuPopup();
        helpPopup.AddItem(new MenuItem { Text = "&About", Command = _viewModel.ShowAboutCommand });
        menuBar.AddRootItem(new MenuItem { Text = "&Help" }, helpPopup);

        var title = new Label
        {
            Parent = this,
            Position = new Point(2, 2),
            size = new Size(Math.Max(1, Dimension.Width - 4), 1),
            Text = "ConsoleLib / ExtCon Showcase",
            ForeColor = ConsoleColor.Yellow,
            ParentBackground = true
        };

        var sections = new ListBox
        {
            Parent = this,
            Position = new Point(2, 4),
            size = new Size(20, Math.Max(4, Dimension.Height - 9)),
            BorderDefinition = new BorderDef { Style = ConsoleLib.Data.BorderStyle.Single },
            ItemsSource = _viewModel.Sections
        };
        sections.BindSelected(_viewModel, nameof(ShowcaseViewModel.SelectedSection));

        var sectionDescription = new Label
        {
            Parent = this,
            Position = new Point(25, 4),
            size = new Size(Math.Max(1, Dimension.Width - 28), 2),
            ForeColor = ConsoleColor.White,
            ParentBackground = true
        };
        _sectionDescription = sectionDescription;

        var inspectorLabel = new Label
        {
            Parent = this,
            Position = new Point(25, 5),
            size = new Size(Math.Max(1, Dimension.Width - 28), 1),
            ForeColor = ConsoleColor.DarkCyan,
            ParentBackground = true
        };
        _inspectorLabel = inspectorLabel;

        var effectPanel = new EffectPanel
        {
            Parent = this,
            Position = new Point(25, 7),
            size = new Size(Math.Max(1, Dimension.Width - 28), 1)
        };
        _effectPanel = effectPanel;

        var progressBar = new ProgressBar
        {
            Parent = this,
            Position = new Point(25, 9),
            size = new Size(Math.Max(1, Dimension.Width - 28), 1),
            Minimum = 0,
            Maximum = 100,
            ForeColor = ConsoleColor.Green,
            BackColor = ConsoleColor.DarkGray
        };
        _progressBar = progressBar;
        progressBar.Text = _effects.CreateProgressFrame(progressBar.Fraction, progressBar.size.Width);

        var terminalWorkspace = new TerminalWidget
        {
            Parent = this,
            Position = new Point(25, 15),
            size = GetTerminalWidgetSize(),
            BorderStyle = ConsoleLib.Data.BorderStyle.Single,
            BorderColor = ConsoleColor.DarkCyan,
            ForeColor = ConsoleColor.Gray,
            BackColor = ConsoleColor.Black,
            Text = "Live terminal"
        };
        _terminal = terminalWorkspace;
        terminalWorkspace.OnKeyPressed += Terminal_OnKeyPressed;
        terminalWorkspace.OnMouseInput += Terminal_OnMouseInput;

        var editor = new TextBox
        {
            Parent = this,
            Position = new Point(25, 10),
            size = new Size(Math.Min(32, Math.Max(1, Dimension.Width - 28)), 1),
            MultiLine = false,
            Text = "Type here",
            ForeColor = ConsoleColor.White,
            BackColor = ConsoleColor.DarkBlue
        };

        var toggleEffects = new Button
        {
            Parent = this,
            Position = new Point(25, 12),
            size = new Size(18, 1),
            Text = "Effects",
            Command = _viewModel.ToggleEffectsCommand
        };
        var advance = new Button
        {
            Parent = this,
            Position = new Point(45, 12),
            size = new Size(18, 1),
            Text = "Advance",
            Command = _viewModel.AdvanceProgressCommand
        };
        var terminal = new Button
        {
            Parent = this,
            Position = new Point(Math.Max(25, Dimension.Width - 20), 12),
            size = new Size(18, 1),
            Text = "Start terminal",
            Command = _viewModel.StartTerminalCommand
        };
        var close = new Button
        {
            Parent = this,
            Position = new Point(Math.Max(2, Dimension.Width - 20), Math.Max(3, Dimension.Height - 3)),
            size = new Size(16, 1),
            Text = "Exit",
            Command = _viewModel.CloseCommand
        };

        _statusLabel = new Label
        {
            Parent = this,
            Position = new Point(2, Math.Max(3, Dimension.Height - 3)),
            size = new Size(Math.Max(1, Dimension.Width - 24), 1),
            ForeColor = ConsoleColor.DarkYellow,
            ParentBackground = true,
            Text = _viewModel.Status
        };

        var aboutDialog = new Dialog
        {
            Parent = this,
            Position = new Point(Math.Max(2, (Dimension.Width - 44) / 2), 6),
            size = new Size(Math.Min(44, Math.Max(10, Dimension.Width - 4)), 6),
            BorderStyle = ConsoleLib.Data.BorderStyle.Single,
            BorderColor = ConsoleColor.Yellow,
            BackColor = ConsoleColor.DarkBlue,
            ForeColor = ConsoleColor.White
        };
        _aboutDialog = aboutDialog;
        new Label
        {
            Parent = aboutDialog,
            Position = new Point(2, 1),
            size = new Size(Math.Max(1, aboutDialog.size.Width - 4), 2),
            Text = "ConsoleLib Showcase\nNative controls, effects, and ConPTY.",
            ParentBackground = true
        };
        new Button
        {
            Parent = aboutDialog,
            Position = new Point(2, 4),
            size = new Size(10, 1),
            Text = "Close",
            Command = _viewModel.CloseAboutCommand
        };

        sectionDescription.Text = _viewModel.SelectedSection?.Description ?? string.Empty;
        inspectorLabel.Text = $"Selected component area: {_viewModel.SelectedSection?.Name}";
        progressBar.Value = _viewModel.Progress;
        effectPanel.SetFrame(_effects.CreateWaveFrame(0, effectPanel.size.Width));
        _effectTimer = Scheduler.Schedule(TickEffect, TimeSpan.FromMilliseconds(120));
    }

    private void TickEffect()
    {
        if (_viewModel.EffectsRunning)
        {
            _effectFrame++;
            if (_effectPanel is not null)
                _effectPanel.SetFrame(_effects.CreateWaveFrame(_effectFrame, _effectPanel.size.Width));
        }

        _effectTimer = Scheduler.Schedule(TickEffect, TimeSpan.FromMilliseconds(120));
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ShowcaseViewModel.SelectedSection))
        {
            if (_sectionDescription is not null)
            {
                _sectionDescription.Text = _viewModel.SelectedSection?.Description ?? string.Empty;
                _sectionDescription.Invalidate();
            }
            if (_inspectorLabel is not null)
            {
                _inspectorLabel.Text = $"Selected component area: {_viewModel.SelectedSection?.Name}";
                _inspectorLabel.Invalidate();
            }
        }
        else if (e.PropertyName == nameof(ShowcaseViewModel.Status))
        {
            if (_statusLabel is not null)
            {
                _statusLabel.Text = _viewModel.Status;
                _statusLabel.Invalidate();
            }
        }
        else if (e.PropertyName == nameof(ShowcaseViewModel.Progress))
        {
            if (_progressBar is not null)
            {
                _progressBar.Value = _viewModel.Progress;
                _progressBar.Text = _effects.CreateProgressFrame(_viewModel.Progress / 100, _progressBar.size.Width);
                _progressBar.Invalidate();
            }
        }
    }

    private void ViewModel_RequestClose(object? sender, EventArgs e)
    {
        _effectTimer?.Dispose();
        Stop();
    }

    private void ViewModel_RequestAbout(object? sender, EventArgs e) => _aboutDialog?.Show();

    private void ViewModel_RequestCloseAbout(object? sender, EventArgs e) => _aboutDialog?.Hide();

    private void TerminalService_SnapshotChanged(object? sender, TerminalSnapshot snapshot)
    {
        if (_terminal is null)
            return;

        Dispatch(() =>
        {
            if (_terminal is null)
                return;

            _terminal.RenderRows(_snapshotRenderer.Render(snapshot));
            var desiredSize = GetTerminalSessionSize();
            if (_terminalService.IsRunning &&
                (snapshot.Size.Columns != desiredSize.Columns || snapshot.Size.Rows != desiredSize.Rows))
            {
                _ = _terminalService.ResizeAsync(desiredSize);
            }
        });
    }

    private void Terminal_OnKeyPressed(object? sender, IKeyEvent e)
    {
        if (_terminal is null || !_terminal.Active || !_terminalService.IsRunning)
            return;

        var input = _inputRouter.Encode(e.usKeyCode, e.KeyChar, e.dwControlKeyState, e.bKeyDown);
        if (input.Length == 0)
            return;

        e.Handled = true;
        _ = _terminalService.SendInputAsync(input);
    }

    private void Terminal_OnMouseInput(object? sender, IMouseEvent e)
    {
        if (_terminal is null)
            return;

        if (e.ButtonEvent)
            FocusManager.Focus(_terminal);
        if (!_terminal.Active || !_terminalService.IsRunning)
            return;

        var document = _terminalService.Document;
        if (document is null)
            return;

        var innerWidth = Math.Max(1, _terminal.size.Width - 2);
        var innerHeight = Math.Max(1, _terminal.size.Height - 2);
        var column = Math.Clamp(e.MousePos.X - _terminal.RealDim.Left, 1, innerWidth);
        var row = Math.Clamp(e.MousePos.Y - _terminal.RealDim.Top, 1, innerHeight);
        var isRelease = e.ButtonEvent && !e.MouseButtonLeft && !e.MouseButtonMiddle && !e.MouseButtonRight;
        var buttonCode = e.MouseWheel > 0 ? 64
            : e.MouseWheel < 0 ? 65
            : e.MouseButtonLeft ? 0
            : e.MouseButtonMiddle ? 1
            : e.MouseButtonRight ? 2
            : e.MouseMoved ? 35
            : isRelease ? 0
            : -1;
        if (buttonCode < 0)
            return;

        var input = _mouseNegotiator.Encode(document, buttonCode, column, row, isRelease);
        if (!string.IsNullOrEmpty(input))
            _ = _terminalService.SendInputAsync(input);
    }

    private void View_OnCanvasResize(object? sender, Point size)
    {
        Dimension = new Rectangle(0, 0, Math.Max(1, size.X), Math.Max(1, size.Y));
        if (_sectionDescription is not null)
            _sectionDescription.size = new Size(Math.Max(1, Dimension.Width - 28), 2);
        if (_effectPanel is not null)
            _effectPanel.size = new Size(Math.Max(1, Dimension.Width - 28), 1);
        if (_progressBar is not null)
            _progressBar.size = new Size(Math.Max(1, Dimension.Width - 28), 1);
        if (_terminal is not null)
        {
            _terminal.size = GetTerminalWidgetSize();
            if (_terminalService.IsRunning)
                _ = _terminalService.ResizeAsync(GetTerminalSessionSize());
        }
    }

    private Size GetTerminalWidgetSize() =>
        new(Math.Max(20, Dimension.Width - 28), Math.Max(4, Dimension.Height - 18));

    private TerminalSize GetTerminalSessionSize() =>
        new(Math.Max(1, GetTerminalWidgetSize().Width - 2), Math.Max(1, GetTerminalWidgetSize().Height - 2));

    public new void Dispose()
    {
        _effectTimer?.Dispose();
        OnCanvasResize -= View_OnCanvasResize;
        _viewModel.RequestClose -= ViewModel_RequestClose;
        _viewModel.RequestAbout -= ViewModel_RequestAbout;
        _viewModel.RequestCloseAbout -= ViewModel_RequestCloseAbout;
        _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        _terminalService.SnapshotChanged -= TerminalService_SnapshotChanged;
        if (_terminal is not null)
        {
            _terminal.OnKeyPressed -= Terminal_OnKeyPressed;
            _terminal.OnMouseInput -= Terminal_OnMouseInput;
        }
        _ = _terminalService.StopAsync();
        base.Dispose();
    }
}
