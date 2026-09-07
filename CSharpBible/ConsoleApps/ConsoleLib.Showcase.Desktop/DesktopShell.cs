using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using ConsoleLib.Showcase.Services;
using ConsoleLib.Showcase.Desktop.ViewModels;
using ConsoleLib.Showcase.Desktop.Capabilities;

namespace ConsoleLib.Showcase.Desktop;

/// <summary>
/// Portable virtual desktop shell. Hosts CXAML pages and delegates platform
/// behavior to the selected widget set and capability composition.
/// </summary>
public sealed class DesktopShell : Application
{
    private readonly DesktopPage _desktopPage;
    private readonly ShowcasePageLoader _pageLoader;
    private readonly DesktopViewModel _viewModel;
    private readonly IShowcaseHostCapabilities _capabilities;
    private readonly DesktopWindowManager _windowManager = new();
    private readonly Dictionary<string, DesktopAppDescriptor> _descriptors;
    private readonly Dictionary<string, DesktopWindowView> _views = new(StringComparer.Ordinal);
    private readonly Dictionary<string, object> _windowModels = new(StringComparer.Ordinal);
    private readonly List<Button> _taskbarButtons = new();
    private IControl? _desktopRoot;
    private Label? _status;

    public DesktopShell(
        IWidgetSet widgetSet,
        DesktopViewModel viewModel,
        DesktopPage? desktopPage = null,
        ShowcasePageLoader? pageLoader = null,
        IShowcaseHostCapabilities? capabilities = null)
        : base(widgetSet)
    {
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        _desktopPage = desktopPage ?? new DesktopPage();
        _pageLoader = pageLoader ?? new ShowcasePageLoader();
        _capabilities = capabilities ?? new UnavailableShowcaseHostCapabilities();
        _descriptors = CreateDescriptors().ToDictionary(item => item.Id, StringComparer.Ordinal);
        _windowManager.Changed += WindowManager_Changed;
        _windowManager.OnOpened = CreateWindow;
        _windowManager.WindowClosed += WindowManager_WindowClosed;
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        OnCanvasResize += Shell_OnCanvasResize;

        Dimension = widgetSet.ClipRect.Width > 0 && widgetSet.ClipRect.Height > 0
            ? widgetSet.ClipRect
            : new Rectangle(0, 0, 100, 30);
        BuildDesktop();
    }

    /// <summary>Window manager owned by this shell.</summary>
    public DesktopWindowManager WindowManager => _windowManager;

    private void BuildDesktop()
    {
        var page = _desktopPage.Load(_viewModel);
        _desktopRoot = page.Root;
        page.Root.Parent = this;
        page.Root.Position = Point.Empty;
        page.Root.size = new Size(Dimension.Width, Math.Max(1, Dimension.Height - 2));

        foreach (var descriptor in _descriptors.Values)
        {
            if (page.NamedControls.TryGetValue(descriptor.Id, out var control))
            {
                control.OnClick += (_, _) => Open(descriptor.Id);
            }
        }

        _status = new Label
        {
            Parent = this,
            Position = new Point(1, Math.Max(1, Dimension.Height - 1)),
            size = new Size(Math.Max(1, Dimension.Width - 2), 1),
            Text = _viewModel.Status,
            ForeColor = ConsoleColor.DarkCyan
        };

        BuildTaskbar();
    }

    private void BuildTaskbar()
    {
        var index = 0;
        foreach (var descriptor in _descriptors.Values)
        {
            var button = new Button
            {
                Parent = this,
                Position = new Point(1 + index * 14, Math.Max(1, Dimension.Height - 2)),
                size = new Size(12, 1),
                Text = descriptor.Title
            };
            button.OnClick += (_, _) => Toggle(descriptor.Id);
            _taskbarButtons.Add(button);
            index++;
        }
    }

    private void Shell_OnCanvasResize(object? sender, Point size)
    {
        Dimension = new Rectangle(Point.Empty, new Size(Math.Max(1, size.X), Math.Max(1, size.Y)));
        var desktopHeight = Math.Max(1, Dimension.Height - 2);
        if (_desktopRoot is not null)
            _desktopRoot.size = new Size(Dimension.Width, desktopHeight);

        var taskbarY = Math.Max(1, Dimension.Height - 2);
        for (var index = 0; index < _taskbarButtons.Count; index++)
        {
            var button = _taskbarButtons[index];
            button.Position = new Point(1 + index * 14, taskbarY);
            button.size = new Size(Math.Max(1, Math.Min(12, Dimension.Width - button.Position.X - 1)), 1);
        }

        if (_status is not null)
        {
            _status.Position = new Point(1, Math.Max(1, Dimension.Height - 1));
            _status.size = new Size(Math.Max(1, Dimension.Width - 2), 1);
            _status.Text = IsUsableDesktopSize()
                ? _viewModel.Status
                : $"Resize console to at least {MinimumWidth}x{MinimumHeight}.";
        }

        foreach (var view in _views.Values)
            ClampWindow(view);

        Invalidate();
    }

    private void ClampWindow(DesktopWindowView view)
    {
        var maxWidth = Math.Max(1, Dimension.Width);
        var maxHeight = Math.Max(1, Dimension.Height - 2);
        var width = Math.Min(view.size.Width, maxWidth);
        var height = Math.Min(view.size.Height, maxHeight);
        view.size = new Size(Math.Max(1, width), Math.Max(1, height));
        view.Position = new Point(
            Math.Max(0, Math.Min(view.Position.X, maxWidth - view.size.Width)),
            Math.Max(0, Math.Min(view.Position.Y, maxHeight - view.size.Height)));
    }

    private bool IsUsableDesktopSize() =>
        Dimension.Width >= MinimumWidth && Dimension.Height >= MinimumHeight;

    private const int MinimumWidth = 80;
    private const int MinimumHeight = 25;

    private void Open(string id)
    {
        if (_descriptors.ContainsKey(id))
            _windowManager.Open(id);
    }

    private void Toggle(string id)
    {
        if (_descriptors.ContainsKey(id))
            _windowManager.Toggle(id);
    }

    private void CreateWindow(DesktopWindow state)
    {
        var descriptor = _descriptors[state.Id];
        var model = CreatePageViewModel(descriptor.Page);
        var page = _pageLoader.Load(descriptor.Page, model);
        var view = new DesktopWindowView(
            state,
            page.Root,
            window => _windowManager.Close(window.Id),
            window => _windowManager.Minimize(window.Id),
            window => _windowManager.BringToFront(window.Id))
        {
            Position = descriptor.InitialPosition,
            size = descriptor.InitialSize
        };
        view.Parent = this;
        _views[state.Id] = view;
        _windowModels[state.Id] = model;
        view.Synchronize();

        if (model is TerminalViewModel terminalViewModel)
            _ = terminalViewModel.StartCommand.ExecuteAsync(null);
    }

    private void WindowManager_Changed()
    {
        foreach (var pair in _views)
            pair.Value.Synchronize();

        foreach (var state in _windowManager.Windows.Where(window => window.IsOpen).OrderBy(window => window.ZIndex))
        {
            if (_views.TryGetValue(state.Id, out var view))
                BringToFront(view);
        }

        Invalidate();
    }

    private void WindowManager_WindowClosed(DesktopWindow state)
    {
        if (_views.Remove(state.Id, out var view))
            Remove(view);

        if (_windowModels.Remove(state.Id, out var model) && model is IDisposable disposable)
            disposable.Dispose();
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DesktopViewModel.Status))
        {
            if (_status is not null)
            {
                _status.Text = _viewModel.Status;
                _status.Invalidate();
            }
        }
    }

    private static IReadOnlyList<DesktopAppDescriptor> CreateDescriptors() =>
        new[]
        {
            new DesktopAppDescriptor("Calendar", "Calendar", ShowcasePage.Calendar, new Point(3, 4), new Size(44, 18)),
            new DesktopAppDescriptor("Calculator", "Calculator", ShowcasePage.Calculator, new Point(10, 6), new Size(36, 18)),
            new DesktopAppDescriptor("Notepad", "Notepad", ShowcasePage.Notepad, new Point(16, 3), new Size(60, 22)),
            new DesktopAppDescriptor("Characters", "Characters", ShowcasePage.Characters, new Point(20, 5), new Size(60, 22)),
            new DesktopAppDescriptor("Clock", "Clock", ShowcasePage.Clock, new Point(12, 3), new Size(38, 22)),
            new DesktopAppDescriptor("Terminal", "Terminal", ShowcasePage.Terminal, new Point(5, 2), new Size(78, 22)),
            new DesktopAppDescriptor("About", "About", ShowcasePage.About, new Point(20, 7), new Size(58, 12)),
        };

    private object CreatePageViewModel(ShowcasePage page) => page switch
    {
        ShowcasePage.Calendar => new CalendarViewModel(),
        ShowcasePage.Calculator => new CalculatorViewModel(),
        ShowcasePage.Notepad => new NotepadViewModel(_capabilities.Clipboard),
        ShowcasePage.Characters => new CharactersViewModel(_capabilities.Clipboard),
        ShowcasePage.Clock => new ClockViewModel(alert: _capabilities.Alert),
        ShowcasePage.Terminal => new TerminalViewModel(_capabilities.Terminal),
        _ => _viewModel
    };

    public new void Dispose()
    {
        _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        OnCanvasResize -= Shell_OnCanvasResize;
        _windowManager.Changed -= WindowManager_Changed;
        _windowManager.WindowClosed -= WindowManager_WindowClosed;
        _windowManager.CloseAll();

        foreach (var pair in _windowModels.ToList())
        {
            if (pair.Value is IDisposable disposable)
                disposable.Dispose();
        }

        _windowModels.Clear();
        base.Dispose();
    }
}
