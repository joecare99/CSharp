# Workbench architecture smoke test

Run in an interactive desktop session:

```powershell
dotnet run --project tools\WorkbenchArchitectureSmoke\WorkbenchArchitectureSmoke.csproj
```

The test uses temporary synthetic files and opens the real Workbench and
standalone designer windows. It checks editor previews, pinned tabs, external
palette and properties, draft recovery, safe handling of unsupported files,
catalogued ComboBox placeholder and safe selection from preview items, including `ComboBoxItem`/`ListBoxItem` attribute, inline, and simple content-property text, inert placeholders for markup-extension content, empty items, and safe labels for complex content, AutoCompleteBox text and filter settings with safe fallback, TabControl/TabItem headers and selected-index fallback in the inert preview, nested Menu/MenuItem headers, separators and validated display-only `InputGesture` hints in a disabled preview, DatePicker ISO date selection, day/month/year visibility and formats, ISO date bounds, Calendar single-date selection and navigation bounds, ColorPicker color/model and visibility options with safe fallback, TimePicker nullable `HH:mm`/`HH:mm:ss` selection, clock mode, increments and safe fallback, NumericUpDown increment/value/format string, spin options, spinner location, text alignment, content alignment and brush/border/padding/typography appearance on supported content controls and ListBox, safe ListBox selection, Label text layout, shared flow direction with RTL inheritance and layout rounding, allowlisted parsing-number styles and shared margin/alignment/clipping/visibility, shared minimum/maximum size constraints, StackPanel/WrapPanel spacing and item sizing/alignment, DockPanel spacing/fill behavior, Grid row/column spacing and show-grid-lines, UniformGrid row/column spacing, ScrollViewer scrollbar visibility, Viewbox stretch direction, ProgressBar text/range. The palette can compose a `TabControl` with `TabItem`s and one supported content control per tab, and nested `Menu`/`MenuItem` structures with separators. The standalone AXAML MVVM generator also emits two-way DatePicker, Calendar, ColorPicker, TimePicker and AutoCompleteBox bindings with nullable properties and a TabControl selected-index binding.
settings and escaped custom text format, Slider direction/range/tick settings and orientation, shared border/typography appearance, CheckBox state/three-state mode,
RadioButton state/group, shared opacity for every catalog control, TextBlock alignment/font style/weight/line height/line limit/trimming/wrapping/foreground, TextBox read-only source value alongside its forced read-only preview, min/max line limits, line height, floating-placeholder mode and safe placeholder foreground, background/border/padding/font/layout/input constraints/font style/weight/foreground and inert Tab behavior,
Button background/border/foreground/thickness/corners/typography/padding and content alignment plus default/cancel source values without host keyboard activation,
Border border brush/corner radius/thickness/padding, and
TextBox border thickness/corners alongside its text/editing constraints, plus shared border/typography appearance on CheckBox, RadioButton and ToggleSwitch while each remains inert,
preview structure selection, source-preserving reorder and designer undo/redo,
Grid cell movement/removal, rendered spans and cross-target draft protection,
single-content container removal and undo,
tab/window close protection, a registered synthetic binary editor and its Save
button, standalone review/undo, and the MCP screenshot source and commands,
including UI-thread dispatch and cancellation of queued commands. Its UI
actions are programmatic; physical keyboard and screen-reader acceptance
remains separate.
ContextMenu and ToolTip property elements on supported controls are checked in
the real preview as closed, inert inline content; the palette can add them to a
selected control, then document Undo/Redo protects the structural changes.
