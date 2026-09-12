using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using ConsoleLib.Showcase.Services;
using ConsoleLib.Showcase.Desktop.Capabilities;
using ConsoleLib.Showcase.Apps;
using ConsoleLib.Showcase.Apps.Calendar;
using ConsoleLib.Showcase.Apps.Gallery;
using ConsoleLib.Showcase.Apps.TicTacToe;
using ConsoleLib.Showcase.Apps.Memory;
using ConsoleLib.Showcase.Apps.Calculator;
using ConsoleLib.Showcase.Apps.Notepad;
using ConsoleLib.Showcase.Apps.Characters;
using ConsoleLib.Showcase.Apps.Clock;
using ConsoleLib.Showcase.Apps.Terminal;
using ConsoleLib.Showcase.Apps.Dialogs;

namespace ConsoleLib.Showcase.Desktop;

/// <summary>
/// Portable virtual desktop shell. Hosts CXAML pages and delegates platform
/// behavior to the selected widget set and capability composition.
/// </summary>
public sealed class DesktopShell : Application
{
    private readonly DesktopPage _desktopPage;
    private readonly DesktopViewModel _viewModel;
    private readonly IShowcaseHostCapabilities _capabilities;
    private readonly DesktopWindowManager _windowManager = new();
    private readonly Dictionary<string, ShowcaseAppDescriptor> _descriptors;
    private readonly Dictionary<string, DesktopWindowView> _views = new(StringComparer.Ordinal);
    private readonly Dictionary<string, object> _windowModels = new(StringComparer.Ordinal);
    private readonly List<Button> _taskbarButtons = new();
    private readonly IServiceProvider _services;
    private Dialog? _aboutDialog;
    private IControl? _desktopRoot;
    private Label? _status;

    public DesktopShell(
        IWidgetSet widgetSet,
        DesktopViewModel viewModel,
        DesktopPage? desktopPage = null,
        IShowcaseHostCapabilities? capabilities = null,
        IEnumerable<IShowcaseAppModule>? modules = null)
        : base(widgetSet)
    {
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        _desktopPage = desktopPage ?? new DesktopPage();
        _capabilities = capabilities ?? new UnavailableShowcaseHostCapabilities();
        _services = new ShowcaseServiceProvider(_capabilities);
        _descriptors = RegisterModules(modules).Apps.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal);
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

        if (page.NamedControls.TryGetValue("About", out var aboutButton))
            aboutButton.OnClick += (_, _) => OpenAboutDialog();

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

    private void OpenAboutDialog()
    {
        if (_aboutDialog is null)
        {
            var result = LoadAboutDialog();
            _aboutDialog = (Dialog)result.Root;
            if (result.NamedControls.TryGetValue("Close", out var closeControl) &&
                closeControl is Button closeButton)
            {
                closeButton.OnClick += (_, _) =>
                {
                    var session = DialogManager.Sessions.FirstOrDefault(
                        candidate => ReferenceEquals(candidate.Dialog, _aboutDialog));
                    DialogManager.Close(session);
                };
            }
        }

        if (_aboutDialog.Parent is null)
        {
            _aboutDialog.Position = new Point(
                Math.Max(1, (Dimension.Width - _aboutDialog.size.Width) / 2),
                Math.Max(2, (Dimension.Height - _aboutDialog.size.Height) / 2));
            DialogManager.Open(_aboutDialog, this, modal: true);
        }
        else
        {
            var session = DialogManager.Sessions.FirstOrDefault(
                candidate => ReferenceEquals(candidate.Dialog, _aboutDialog));
            if (session is not null)
                DialogManager.Activate(session);
        }
    }

    private static CxamlLoadResult LoadAboutDialog()
    {
        var assembly = typeof(DesktopShell).Assembly;
        var resourceName = assembly.GetManifestResourceNames()
            .SingleOrDefault(name => name.EndsWith(".About.cxaml", StringComparison.Ordinal))
            ?? throw new InvalidOperationException("The Desktop About dialog resource is missing.");
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException("The Desktop About dialog resource could not be opened.");
        using var reader = new StreamReader(stream);
        return new CxamlLoader().LoadDialog(reader, new CxamlLoadContext(new object()));
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
        var model = descriptor.CreateViewModel(_services);
        var page = descriptor.LoadPage(_services, model);
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

        if (model is ConsoleLib.Showcase.Apps.Terminal.TerminalViewModel terminalViewModel)
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

    private ShowcaseAppRegistrationContext RegisterModules(IEnumerable<IShowcaseAppModule>? modules)
    {
        var context = new ShowcaseAppRegistrationContext(_services);
        foreach (var module in modules ?? new IShowcaseAppModule[]
        {
            new CalendarAppModule(),
            new GalleryAppModule(),
            new TicTacToeAppModule(),
            new MemoryAppModule(),
            new CalculatorAppModule(),
            new NotepadAppModule(),
            new CharactersAppModule(),
            new ClockAppModule(),
            new TerminalAppModule(),
            new DialogsAppModule()
        })
            module.Register(context);
        return context;
    }

    private sealed class ShowcaseServiceProvider : IServiceProvider
    {
        private readonly IShowcaseHostCapabilities _capabilities;

        public ShowcaseServiceProvider(IShowcaseHostCapabilities capabilities) => _capabilities = capabilities;

        public object? GetService(Type serviceType) =>
            serviceType == typeof(ConsoleLib.Interfaces.IClipboardService) ? _capabilities.Clipboard :
            serviceType == typeof(ConsoleLib.Showcase.Desktop.Capabilities.IShowcaseAlertService) || serviceType == typeof(ConsoleLib.Showcase.Apps.IShowcaseAlertService)
                ? _capabilities.Alert :
            serviceType == typeof(ConsoleLib.Showcase.Desktop.Capabilities.IShowcaseTerminalCapability) || serviceType == typeof(ConsoleLib.Showcase.Apps.IShowcaseTerminalCapability)
                ? _capabilities.Terminal :
            serviceType == typeof(IShowcaseHostCapabilities) ? _capabilities :
            null;
    }

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
