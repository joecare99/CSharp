# Workbench architecture smoke test

Run in an interactive desktop session:

```powershell
dotnet run --project tools\WorkbenchArchitectureSmoke\WorkbenchArchitectureSmoke.csproj
```

For the resource/focus check only, pass `-- --resources-only`. If the read-only
AA05 calculator sample exists at its development path, this checks its visible
button caption, local margin/padding and source focus in both real hosts.

The test uses temporary synthetic files and opens the real Workbench and
standalone designer windows. It checks editor previews, pinned tabs, external
palette and properties, draft recovery, safe handling of unsupported files,
catalogued ComboBox placeholder and safe selection from preview items, including `ComboBoxItem`/`ListBoxItem` attribute, inline, and simple content-property text, inert placeholders for markup-extension content, empty items, and safe labels for complex content. ComboBox/ListBox palettes can append, edit, reorder and remove their matching static item controls with document undo/redo; dynamic sources, templates, selection values and mismatched child controls are rejected. `ItemsControl` supports static `TextBlock`/`Label` entries, with dynamic sources, templates and non-text controls rejected; its preview is disabled. AutoCompleteBox text and filter settings with safe fallback, TabControl/TabItem headers and selected-index fallback in the inert preview, nested Menu/MenuItem headers and literal text icons, separators, validated display-only `InputGesture` hints and inert display fallback from `HotKey` (the preview never registers or invokes hotkeys), checkbox/radio toggle type, group, state, stay-open setting and closed submenus in a disabled preview, DatePicker ISO date selection, day/month/year visibility and formats, ISO date bounds, Calendar single-date selection and navigation bounds, ColorPicker color/model and visibility options with safe fallback, TimePicker nullable `HH:mm`/`HH:mm:ss` selection, clock mode, increments and safe fallback, NumericUpDown increment/value/format string, spin options, spinner location, text alignment, content alignment and brush/border/padding/typography appearance on supported content controls and ListBox, safe ListBox selection, Label text layout, shared flow direction with RTL inheritance and layout rounding, allowlisted parsing-number styles and shared margin/alignment/clipping/visibility, shared minimum/maximum size constraints, StackPanel/WrapPanel spacing and item sizing/alignment, DockPanel spacing/fill behavior, Grid row/column spacing and show-grid-lines, UniformGrid row/column spacing, ScrollViewer scrollbar visibility, Viewbox stretch direction, ProgressBar text/range. The palette can compose a `TabControl` with `TabItem`s and one supported content control per tab, and nested `Menu`/`MenuItem` structures with separators. The View menu can hide the entire workspace sidebar and restore its last width. The standalone AXAML MVVM generator emits commands for Button, MenuItem and toggle controls; literal command parameters produce string-parameter RelayCommands, while commands with bound parameters are left ungenerated. It also emits two-way DatePicker, Calendar, ColorPicker, TimePicker and AutoCompleteBox bindings with nullable properties and a TabControl selected-index binding.
settings and escaped custom text format, Slider direction/range/tick settings and orientation, shared border/typography appearance, CheckBox state/three-state mode,
RadioButton state/group, shared opacity for every catalog control, TextBlock alignment/font family/style/weight/line height/line limit/trimming/wrapping/foreground, TextBox read-only source value alongside its forced read-only preview, min/max line limits, line height, floating-placeholder mode and safe placeholder foreground, shared font-family rendering for text and templated controls, background/border/padding/font/layout/input constraints/font style/weight/foreground and inert Tab behavior,
Button background/border/foreground/thickness/corners/typography/padding and content alignment plus default/cancel source values without host keyboard activation; literal command parameters on Button, MenuItem, CheckBox, RadioButton and ToggleSwitch are displayed inertly, while commands and parameter bindings are never resolved or invoked,
Border border brush/corner radius/thickness/padding, and
TextBox border thickness/corners alongside its text/editing constraints, plus shared border/typography appearance on CheckBox, RadioButton and ToggleSwitch while each remains inert,
preview pointer selection of disabled controls and visible nested ListBox, Tab and
TreeView items, synchronized source caret/tree/inspector, source-preserving reorder and designer undo/redo,
Grid cell movement/removal, safe Auto row/column expansion and removal of unused final tracks, explicit track-size editing, rendered spans and cross-target draft protection,
single-content container removal and undo,
tab/window close protection, a registered synthetic binary editor and its Save
button, standalone review/undo, and the MCP screenshot source and commands,
including UI-thread dispatch and cancellation of queued commands. Its UI
actions are programmatic; physical keyboard and screen-reader acceptance
remains separate.
The Project tab also exercises explicit project selection, MSBuild inspection,
non-executable library handling and configuration-change invalidation.
ContextMenu and ToolTip property elements on supported controls are checked in
the real preview as closed, inert inline content; ToolTip also renders a
validated Border/StackPanel composition with multiple literal TextBlocks.
Border properties and child structure are revalidated, and the palette can
compose the content incrementally; document Undo/Redo protects each structural
change. Static TreeView/TreeViewItem hierarchies render literal headers and
expanded branches in a disabled preview; dynamic item sources and unsupported
children are rejected.
Canvas children can be appended with deterministic offset coordinates; direct-child
Canvas.Left/Canvas.Top literals are validated, edited in the property inspector and
rendered in the inert preview. Unset positions retain Avalonia's defaults, while
bindings and unsupported Canvas content are rejected. Safe Grid children expose
bounded, collision-checked row/column/span placement in the property inspector. Direct
DockPanel children expose
a validated `DockPanel.Dock` enum in the inspector, and the preview applies literal
placements without evaluating bindings.
Rectangle, Ellipse, Line, Polygon and Polyline can be inserted from the palette
with Undo/Redo. They render only literal solid-color fills/strokes; line
endpoints and polygon/polyline point lists are finite and bounded. Brush
resources and dynamic shape bindings remain outside the editable preview subset.
Image controls can be inserted from the palette and edit literal source references
and stretch settings. The preview always uses an inert placeholder; it does not
load local files, network resources or binding expressions.
Specialized catalog controls and their supported static ComboBox, ListBox and
TreeView item types are available from the external palette.
