# Workbench architecture smoke test

Run in an interactive desktop session:

```powershell
dotnet run --project tools\WorkbenchArchitectureSmoke\WorkbenchArchitectureSmoke.csproj
```

The test uses temporary synthetic files and opens the real Workbench and
standalone designer windows. It checks editor previews, pinned tabs, external
palette and properties, draft recovery, safe handling of unsupported files,
catalogued ComboBox placeholder, NumericUpDown increment/value/format string, spin options, spinner location, text alignment, allowlisted parsing-number styles and shared margin/alignment, shared minimum/maximum size constraints, ProgressBar text/range
settings and escaped custom text format, Slider direction/range/tick settings and orientation, shared border/typography appearance, CheckBox state/three-state mode,
RadioButton state/group, shared opacity for every catalog control, TextBlock alignment/font style/weight/line height/line limit/trimming/wrapping/foreground, TextBox read-only source value alongside its forced read-only preview, background/border/padding/font/layout/input constraints/font style/weight/foreground and inert Tab behavior,
Button background/border/foreground/thickness/corners/typography/padding plus default/cancel source values without host keyboard activation,
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
