using AA98_AvlnCodeStudio.UI.Components;
using AA98_AvlnCodeStudio.UI.Resources;
using AA98_AvlnCodeStudio.AxamlDesigner.UI.Components;
using AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;
using AA98_AvlnCodeStudio.AxamlDesigner.UI.Views;
using AA98_AvlnCodeStudio.Model.Documents;
using AA98_AvlnCodeStudio.Model.Workspaces;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Property.Editor;

namespace AA98_AvlnCodeStudio.UI.ViewModels;

/// <summary>
/// Represents one open editor document and its composed view.
/// </summary>
public sealed partial class EditorTabViewModel : ObservableObject, IDisposable
{
    private string? _diagnosticPath;
    private string? _diagnosticMessage;
    private Control? _diagnosticView;
    private Grid? _editorView;
    private AxamlDesignerSurfaceView? _surface;
    private GridSplitter? _splitter;
    private string _editorKind;
    private readonly bool _hasCustomContributions;
    private readonly IDocumentEditorSession? _session;
    private readonly Control? _inputView;
    private readonly Func<CancellationToken, Task<string?>>? _selectSavePathAsync;
    private IDocumentEditorContributions<Control>? _contributions;
    private AxamlDesignerPaletteView? _paletteInputView;
    private int _hasUnappliedDrafts;
    private int _disposed;

    public EditorTabViewModel(
        IAvaloniaEditorComponent component,
        string editorKind = "text",
        IDocumentEditorContributions<Control>? contributions = null)
        : this(component ?? throw new ArgumentNullException(nameof(component)), null, null, null, editorKind, contributions)
    {
    }

    /// <summary>Composes a non-text document session with its own view and optional tools.</summary>
    public EditorTabViewModel(
        IDocumentEditorSession session,
        Control view,
        Func<CancellationToken, Task<string?>> selectSavePathAsync,
        string editorKind,
        IDocumentEditorContributions<Control>? contributions = null)
        : this(
            null,
            session ?? throw new ArgumentNullException(nameof(session)),
            view ?? throw new ArgumentNullException(nameof(view)),
            selectSavePathAsync ?? throw new ArgumentNullException(nameof(selectSavePathAsync)),
            editorKind,
            contributions)
    {
    }

    private EditorTabViewModel(
        IAvaloniaEditorComponent? component,
        IDocumentEditorSession? session,
        Control? view,
        Func<CancellationToken, Task<string?>>? selectSavePathAsync,
        string editorKind,
        IDocumentEditorContributions<Control>? contributions)
    {
        Component = component;
        _session = session;
        _inputView = component?.View ?? view;
        _selectSavePathAsync = selectSavePathAsync;
        _editorKind = editorKind;
        _hasCustomContributions = contributions is not null;
        Editor = component?.EditorViewModel ?? new EditorViewModel();
        SaveCommand = session is null
            ? Editor.SaveCommand
            : new AsyncRelayCommand(cancellationToken => SaveSessionAsync(saveAs: false, cancellationToken));
        SaveAsCommand = session is null
            ? Editor.SaveAsCommand
            : new AsyncRelayCommand(cancellationToken => SaveSessionAsync(saveAs: true, cancellationToken));
        SetContributions(contributions);
        Editor.PropertyChanged += OnEditorPropertyChanged;
        if (session is null)
        {
            Editor.ContentChanging += OnContentChanging;
        }
        else
        {
            session.ContentChanging += OnContentChanging;
            session.StateChanged += OnSessionStateChanged;
        }

        _inputView?.AddHandler(InputElement.PointerPressedEvent, OnTextPointerPressed, RoutingStrategies.Tunnel, true);
        _inputView?.AddHandler(InputElement.GotFocusEvent, OnTextGotFocus, RoutingStrategies.Bubble, true);
        if (session is null && editorKind == "axaml" && !_hasCustomContributions)
        {
            EnsureDesigner();
        }
    }

    public IAvaloniaEditorComponent? Component { get; }

    /// <summary>Provides the legacy text-editor/status model; non-text sessions do not use its text buffer.</summary>
    public EditorViewModel Editor { get; }

    public IAsyncRelayCommand SaveCommand { get; }

    public IAsyncRelayCommand SaveAsCommand { get; }

    internal Func<string, IDisposable>? AcquireSaveTarget { get; set; }

    public string EditorKind => _editorKind;

    public AxamlDesignerViewModel? Designer { get; private set; }

    public Control? PaletteView
    {
        get
        {
            Control? palette = CanShowContributions ? _contributions?.PaletteView : null;
            ObservePalette(palette as AxamlDesignerPaletteView);
            return palette;
        }
    }

    public Control? PropertiesView => CanShowContributions ? _contributions?.PropertiesView : null;

    public IPropertyProvider? SelectedProperties => CanShowContributions ? _contributions?.SelectedProperties : null;

    public bool HasPalette => PaletteView is not null;

    private bool CanShowContributions => !IsDiagnostic && (_hasCustomContributions || _editorKind == "axaml");

    public event Action<EditorTabViewModel, bool>? UserActivated;

    public event EventHandler? EditorSelectionChanged;

    public bool HasTextEditor => _session is null && !IsDiagnostic;

    public Control TextView => _inputView!;

    public Control View => _diagnosticMessage is not null
        ? _diagnosticView ??= new TextBlock
        {
            Text = _diagnosticMessage,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(24),
        }
        : _session is not null ? _inputView! : _editorView ??= CreateEditorView();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Title))]
    private bool _isPreview;

    public bool IsDiagnostic => _diagnosticMessage is not null;

    public bool HasUnappliedDrafts => Volatile.Read(ref _hasUnappliedDrafts) != 0;

    public bool IsDirty => _session?.IsDirty ?? Editor.IsDirty;

    public bool IsProtected => IsDirty || HasUnappliedDrafts;

    public string Title => $"{(_diagnosticPath is null ? _session?.DisplayName ?? Editor.DocumentName ?? Editor.WindowTitle : Path.GetFileName(_diagnosticPath))}{(IsDirty ? UiStrings.WindowTitleDirtySuffix : string.Empty)}{(IsPreview ? " (Preview)" : string.Empty)}";

    public string WindowTitle => _session is null
        ? Editor.WindowTitle
        : $"{UiStrings.ApplicationTitle} - {(_diagnosticPath is null ? _session.DisplayName : Path.GetFileName(_diagnosticPath))}{(IsDirty ? UiStrings.WindowTitleDirtySuffix : string.Empty)}";

    public string? FilePath => _diagnosticPath ?? _session?.FilePath ?? Component?.Workflow?.Document?.FilePath ?? Editor.CurrentFilePath;

    public void Pin() => IsPreview = false;

    public void ActivateByUser(bool designerSelection = false)
    {
        Pin();
        UserActivated?.Invoke(this, designerSelection);
    }

    internal async Task OpenDocumentAsync(string path, CancellationToken cancellationToken)
    {
        if (_session is null)
        {
            await Editor.OpenFileAsync(path, cancellationToken);
            TextDocumentEncoding.ValidateText(Editor.Text);
            return;
        }

        await _session.OpenAsync(path, cancellationToken);
        if (_session.FilePath is not { } openedPath || !WorkspacePathIdentity.AreEqual(openedPath, path))
        {
            throw new InvalidDataException("The editor did not open the requested document path.");
        }
    }

    internal void ShowDiagnostic(string path, string message)
    {
        _diagnosticPath = path;
        _diagnosticMessage = $"{path}{Environment.NewLine}{Environment.NewLine}{message}";
        OnPropertyChanged(nameof(IsDiagnostic));
        OnPropertyChanged(nameof(HasTextEditor));
        OnPropertyChanged(nameof(HasPalette));
        OnPropertyChanged(nameof(PaletteView));
        OnPropertyChanged(nameof(PropertiesView));
        OnPropertyChanged(nameof(SelectedProperties));
        OnPropertyChanged(nameof(FilePath));
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(View));
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        Editor.PropertyChanged -= OnEditorPropertyChanged;
        if (_session is null)
        {
            Editor.ContentChanging -= OnContentChanging;
        }
        else
        {
            _session.ContentChanging -= OnContentChanging;
            _session.StateChanged -= OnSessionStateChanged;
        }

        _inputView?.RemoveHandler(InputElement.PointerPressedEvent, OnTextPointerPressed);
        _inputView?.RemoveHandler(InputElement.GotFocusEvent, OnTextGotFocus);
        if (_surface is not null)
        {
            _surface.RemoveHandler(InputElement.PointerPressedEvent, OnDesignerPointerPressed);
            _surface.RemoveHandler(InputElement.GotFocusEvent, OnDesignerGotFocus);
        }
        if (Designer is not null)
        {
            Designer.MarkupChanged -= OnDesignerMarkupChanged;
        }
        bool sessionIsContribution = ReferenceEquals(_session, _contributions);
        SetContributions(null);
        if (!sessionIsContribution)
        {
            _session?.Dispose();
        }
    }

    private void OnContentChanging(object? sender, EventArgs e) => Pin();

    private void OnSessionStateChanged(object? sender, EventArgs e)
    {
        if (IsDirty)
        {
            Pin();
        }

        OnPropertyChanged(nameof(IsDirty));
        OnPropertyChanged(nameof(IsProtected));
        OnPropertyChanged(nameof(FilePath));
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(WindowTitle));
    }

    private async Task SaveSessionAsync(bool saveAs, CancellationToken cancellationToken)
    {
        try
        {
            if (Volatile.Read(ref _disposed) != 0)
            {
                throw new InvalidOperationException("The document is no longer open.");
            }

            string? path = saveAs ? null : _session!.FilePath;
            if (string.IsNullOrWhiteSpace(path))
            {
                path = await _selectSavePathAsync!(cancellationToken);
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                Editor.StatusText = saveAs ? UiStrings.SaveAsCanceledStatusText : UiStrings.SaveCanceledStatusText;
                Editor.NotificationText = saveAs ? UiStrings.SaveAsCanceledNotificationText : UiStrings.SaveCanceledNotificationText;
                return;
            }

            if (Volatile.Read(ref _disposed) != 0)
            {
                throw new InvalidOperationException("The document is no longer open.");
            }

            string normalizedPath = WorkspacePathIdentity.Normalize(path);
            using IDisposable? reservation = AcquireSaveTarget?.Invoke(normalizedPath);
            await _session!.SaveAsync(normalizedPath, cancellationToken);
            if (_session.FilePath is not { } savedPath || !WorkspacePathIdentity.AreEqual(savedPath, normalizedPath))
            {
                throw new InvalidOperationException("The editor did not update its document path after saving.");
            }

            Editor.StatusText = string.Format(UiStrings.SavedDocumentStatusFormat, _session.DisplayName);
            Editor.NotificationText = string.Format(UiStrings.SavedDocumentNotificationFormat, _session.DisplayName);
        }
        catch (Exception exception) when (exception is IOException or InvalidDataException or UnauthorizedAccessException or InvalidOperationException)
        {
            Editor.StatusText = "Save failed.";
            Editor.NotificationText = exception.Message;
        }
    }

    private void EnsureDesigner()
    {
        if (Designer is not null)
        {
            return;
        }

        Designer = new AxamlDesignerViewModel();
        Designer.MarkupChanged += OnDesignerMarkupChanged;
        SetContributions(new AxamlDesignerContributions(Designer));
        OnPropertyChanged(nameof(Designer));
    }

    private void SetContributions(IDocumentEditorContributions<Control>? contributions)
    {
        ObservePalette(null);

        if (_contributions is not null)
        {
            _contributions.SelectionChanged -= OnContributionSelectionChanged;
            _contributions.DraftStateChanged -= OnContributionDraftStateChanged;
            _contributions.Dispose();
        }

        _contributions = contributions;
        if (_contributions is not null)
        {
            _contributions.SelectionChanged += OnContributionSelectionChanged;
            _contributions.DraftStateChanged += OnContributionDraftStateChanged;
        }

        Volatile.Write(ref _hasUnappliedDrafts, _contributions?.HasUnappliedDrafts == true ? 1 : 0);
        OnPropertyChanged(nameof(PaletteView));
        OnPropertyChanged(nameof(PropertiesView));
        OnPropertyChanged(nameof(SelectedProperties));
        OnPropertyChanged(nameof(HasPalette));
        OnPropertyChanged(nameof(HasUnappliedDrafts));
        OnPropertyChanged(nameof(IsProtected));
    }

    private void ObservePalette(AxamlDesignerPaletteView? palette)
    {
        if (ReferenceEquals(_paletteInputView, palette))
        {
            return;
        }

        if (_paletteInputView is not null)
        {
            _paletteInputView.EditorActivated -= OnPaletteEditorActivated;
        }

        _paletteInputView = palette;
        if (_paletteInputView is not null)
        {
            _paletteInputView.EditorActivated += OnPaletteEditorActivated;
        }
    }

    private Grid CreateEditorView()
    {
        Grid grid = new() { RowDefinitions = new RowDefinitions("0,0,*") };
        Grid.SetRow(_inputView!, 2);
        grid.Children.Add(_inputView!);
        _editorView = grid;
        UpdateEditorView();
        return grid;
    }

    private void UpdateEditorView()
    {
        if (_editorView is not { } grid)
        {
            return;
        }

        bool showDesigner = _editorKind == "axaml" && Designer is not null;
        if (showDesigner && _surface is null)
        {
            _surface = new AxamlDesignerSurfaceView { DataContext = Designer };
            _surface.AddHandler(InputElement.PointerPressedEvent, OnDesignerPointerPressed, RoutingStrategies.Tunnel, true);
            _surface.AddHandler(InputElement.GotFocusEvent, OnDesignerGotFocus, RoutingStrategies.Bubble, true);
            _splitter = new GridSplitter
            {
                Height = 8,
                ResizeDirection = GridResizeDirection.Rows,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            };
            Grid.SetRow(_splitter, 1);
            grid.Children.Add(_surface);
            grid.Children.Add(_splitter);
        }

        if (_surface is not null)
        {
            _surface.IsVisible = showDesigner;
            _splitter!.IsVisible = showDesigner;
        }

        grid.RowDefinitions[0].Height = showDesigner ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
        grid.RowDefinitions[1].Height = showDesigner ? new GridLength(8) : new GridLength(0);
    }

    private void OnTextPointerPressed(object? sender, PointerPressedEventArgs e) =>
        ActivateByUser(designerSelection: _hasCustomContributions);

    private void OnDesignerPointerPressed(object? sender, PointerPressedEventArgs e) => ActivateByUser(designerSelection: true);

    private void OnPaletteEditorActivated(object? sender, EventArgs e) => ActivateByUser(designerSelection: true);

    private void OnTextGotFocus(object? sender, FocusChangedEventArgs e)
    {
        if (e.NavigationMethod != NavigationMethod.Unspecified)
        {
            ActivateByUser(designerSelection: _hasCustomContributions);
        }
    }

    private void OnDesignerGotFocus(object? sender, FocusChangedEventArgs e)
    {
        if (e.NavigationMethod != NavigationMethod.Unspecified)
        {
            ActivateByUser(designerSelection: true);
        }
    }

    private void OnDesignerMarkupChanged(object? sender, AxamlDesignerMarkupChangedEventArgs e)
    {
        Editor.Text = e.Markup;
        Editor.NotificationText = Designer?.StatusText ?? string.Empty;
    }

    private void OnContributionSelectionChanged(object? sender, EventArgs e) =>
        EditorSelectionChanged?.Invoke(this, EventArgs.Empty);

    private void OnContributionDraftStateChanged(object? sender, EventArgs e)
    {
        Volatile.Write(ref _hasUnappliedDrafts, _contributions?.HasUnappliedDrafts == true ? 1 : 0);
        if (HasUnappliedDrafts)
        {
            Pin();
        }
        OnPropertyChanged(nameof(HasUnappliedDrafts));
        OnPropertyChanged(nameof(IsProtected));
    }

    private void OnEditorPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_session is null
            && !_hasCustomContributions
            && e.PropertyName == nameof(EditorViewModel.CurrentFilePath)
            && Editor.CurrentFilePath is { } path)
        {
            string kind = string.Equals(Path.GetExtension(path), ".axaml", StringComparison.OrdinalIgnoreCase)
                ? "axaml"
                : "text";
            if (kind == "axaml")
            {
                EnsureDesigner();
            }

            if (_editorKind != kind)
            {
                _editorKind = kind;
                UpdateEditorView();
                OnPropertyChanged(nameof(EditorKind));
                OnPropertyChanged(nameof(HasPalette));
                OnPropertyChanged(nameof(PaletteView));
                OnPropertyChanged(nameof(PropertiesView));
                OnPropertyChanged(nameof(SelectedProperties));
                OnPropertyChanged(nameof(View));
            }
        }

        if (_session is null
            && (e.PropertyName is nameof(EditorViewModel.Text) or nameof(EditorViewModel.CurrentFilePath))
            && _editorKind == "axaml")
        {
            Designer?.LoadMarkup(Editor.Text);
        }

        if (e.PropertyName is nameof(EditorViewModel.DocumentName)
            or nameof(EditorViewModel.CurrentFilePath)
            or nameof(EditorViewModel.IsDirty))
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(FilePath));
            OnPropertyChanged(nameof(IsDirty));
            OnPropertyChanged(nameof(IsProtected));
            OnPropertyChanged(nameof(WindowTitle));
        }
    }
}
