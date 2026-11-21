# Workbench architecture smoke test

Run in an interactive desktop session:

```powershell
dotnet run --project tools\WorkbenchArchitectureSmoke\WorkbenchArchitectureSmoke.csproj
```

For the focused standalone binding smoke, run from a physical workspace path:

```powershell
$root = Join-Path (Get-Location).Path ("artifacts\standalone-binding-win-" + [guid]::NewGuid().ToString("N"))
.\tools\build\dotnet-isolated.ps1 run --project tools\WorkbenchArchitectureSmoke\WorkbenchArchitectureSmoke.csproj --framework net10.0 "-p:HostArtifactRoot=$root" -- --standalone-binding-only
```

In WSL/Linux, ensure `dotnet` and `DOTNET_ROOT` refer to the Linux SDK, then:

```bash
root="$PWD/artifacts/standalone-binding-linux-$(date +%s%N)"
bash tools/build/dotnet-isolated.sh run --project tools/WorkbenchArchitectureSmoke/WorkbenchArchitectureSmoke.csproj --framework net10.0 "-p:HostArtifactRoot=$root" -- --standalone-binding-only
```

The wrapper supplies the absolute isolation props path to external project
references. Each host must use its own fresh artifact root. The smoke creates,
restores and builds only its disposable synthetic project; its root DataContext
and ViewModel come from the MVVM generator. It opens the real standalone window,
programmatically cancels, then reviews and applies compiled property and reflection
command bindings, checks source preservation and workspace undo/redo, and rejects nested
DataContext, nested type, explicit source and foreign root contexts. It rebuilds
the edited fixture and cleans it up. It does not execute fixture AXAML or claim
physical input or accessibility acceptance. The standalone-only mode uses the
null version-control service and does not open AA05.

The Workbench solution explorer additionally nests a C# file under a sibling
`.resx` when the project declares a literal, unconditional `DependentUpon`.
This is a read-only XML projection, not MSBuild evaluation: conditional,
ambiguous, imported and computed metadata is intentionally not resolved.
It does not attempt to model all MSBuild project items or nested-project
ownership. To include the local AA05 fixture in the focused
`ResourceDependencyProjectExplorerTests`, set `AA05_SOLUTION` to its `.slnx`
path. The fixture is only read, never built. Run the tests through the isolated
build wrappers with a fresh host-specific artifact root.

The read-only AA05 inspection also found solution items, including an external
`Solution.props`, and project references. The filesystem source already shows
the declared solution items and a flat `References` list; reference entries
are declarations, not evaluated MSBuild results. In particular AA05's test
project contains a `$(UpDir)` project reference and imports project properties.
No inferred file ownership, evaluated reference tree, or additional project
membership is claimed from those declarations.

The focused `--resource-explorer-only` window smoke requires `AA05_SOLUTION`
and an interactive desktop. It drives rendered tree containers, selects and
opens the designer with Enter, refreshes and switches projects, and compares
the sample's file hashes and directory inventory before and after. It uses
the null version-control service and detaches the workspace context before
opening the C# file, avoiding MSBuild-backed semantic analysis of the sample;
this is not a test of that analysis or physical input. The `.slnx` and project
XML readers reject DTDs and documents over 1,000,000 characters.

For example, from a physical (non-junction) working directory on Windows:

```powershell
$env:AA05_SOLUTION = (Resolve-Path '..\AA05_CommandParCalc\AA05_CommandParCalc.slnx').Path
$root = Join-Path (Get-Location) 'artifacts\resource-explorer-smoke-win'
.\tools\build\dotnet-isolated.ps1 run --project tools\WorkbenchArchitectureSmoke\WorkbenchArchitectureSmoke.csproj --framework net10.0 "-p:HostArtifactRoot=$root" -- --resource-explorer-only
```

In WSLg, use `AA05_SOLUTION="$PWD/../AA05_CommandParCalc/AA05_CommandParCalc.slnx"`
and the equivalent `tools/build/dotnet-isolated.sh` command with a separate
fresh Linux artifact root.

The complementary `--resource-explorer-semantic-only` mode creates and restores
two disposable SDK projects and solutions, with no package sources or project
references and local `Directory.Build` files stopping parent imports. It retains
the active workspace while exercising the same rendered tree, opening, refresh,
and project-switch flow. A deliberate CS0029 in each designer file confirms
that semantic analysis actually ran for the current fixture project. Only those
fixtures and the installed SDK are evaluated; this mode does not open AA05.
Thus the real-AA05 smoke tests the UI without active semantic analysis, while
this synthetic smoke tests the active-workspace path.

For preview pointer selection and resize cancellation only, pass `-- --pointer-only`.
It opens an Avalonia window and uses the rendered input hit target with synthetic
pointer events; it does not test physical input. The safe preview selects visible
rendered bounds rather than stretched selection wrappers, including overlapping
and nested controls. Empty wrapper space falls through to the containing node.
Changing or detaching the preview cancels an active resize; selection never executes
the document or changes its markup. Generated item containers map to source items
only while their original preview items remain unchanged; otherwise selection falls
back to the owning control. The full smoke also checks pointer-to-tree and source-caret
synchronization in both Workbench and standalone hosts.

For supported preview controls, a single unqualified or Avalonia-namespaced `ZIndex`
attribute containing a signed decimal integer from -100000 through 100000 is applied
to the selection wrapper. Higher sibling values render and select in front; equal
values retain source order, and children cannot escape their parent's stacking.
The default is zero. Bindings, resources, property elements, foreign or ambiguous
attributes, whitespace and out-of-range values fall back to zero; they are not
evaluated. This is preview-only, not an inspector/source editing feature. Item
containers and native template internals are not assigned source ZIndex values.
The pointer smoke compares native Canvas/Grid rendering and input hits with preview
rendered pixels and synthetic clicks, including occluded grips and stale resize
cancellation; physical mouse and compositor acceptance remain separate.

For the resource/focus check only, pass `-- --resources-only`. It uses
`AA98_CALCULATOR_SAMPLE_ROOT` as the external, read-only AA05 project directory,
or the original Windows development path when unset. If the sample is absent,
the calculator check is skipped. When present, it checks the visible button
caption, local margin/padding and source focus in both real hosts.

The two host regressions and all `AxamlPreviewResourcesTests` can be run with
the same sample and separate per-run artifact roots. From the repository root:

```powershell
$env:AA98_CALCULATOR_SAMPLE_ROOT = 'C:\path\to\AA05_CommandParCalc'
$root = Join-Path (Get-Location).Path ("artifacts\aa05-windows-" + [guid]::NewGuid().ToString("N"))
$filter = 'FullyQualifiedName~AxamlPreviewResourcesTests|FullyQualifiedName~StandaloneHost_ResolvesReportedCalculatorSample|FullyQualifiedName~Workbench_OpensReportedCalculatorUsingItsProjectContext'
.\tools\build\dotnet-isolated.ps1 test .\AA98_AvlnCodeStudio.Tests\AA98_AvlnCodeStudio.Tests.csproj --framework net10.0 --filter $filter "-p:HostArtifactRoot=$root"
```

```bash
export AA98_CALCULATOR_SAMPLE_ROOT=/path/to/AA05_CommandParCalc
root="$(pwd)/artifacts/aa05-linux-$(date +%s%N)"
filter='FullyQualifiedName~AxamlPreviewResourcesTests|FullyQualifiedName~StandaloneHost_ResolvesReportedCalculatorSample|FullyQualifiedName~Workbench_OpensReportedCalculatorUsingItsProjectContext'
bash tools/build/dotnet-isolated.sh test AA98_AvlnCodeStudio.Tests/AA98_AvlnCodeStudio.Tests.csproj --framework net10.0 --filter "$filter" "-p:HostArtifactRoot=$root"
```

The sample-dependent tests are inconclusive if their view is absent. To run
the optional programmatic window smoke on Windows, from a physical workspace
path (not a junction), with the sample variable set:

```powershell
$root = Join-Path (Get-Location).Path ("artifacts\aa05-smoke-windows-" + [guid]::NewGuid().ToString("N"))
.\tools\build\dotnet-isolated.ps1 run --project tools\WorkbenchArchitectureSmoke\WorkbenchArchitectureSmoke.csproj "-p:HostArtifactRoot=$root" -- --resources-only
```

An available desktop/display is required; this is not physical GUI acceptance.
The smoke opens the sample without saving it.

For the theme check, set `AVLN_THEME_SCREENSHOT_DIR` to an output directory and
pass `-- --theme-only`. It captures the synthetic Workbench as `dark.png` and
`light.png`, verifies representative C# highlighting and menu/editor/status
contrast, switches dark/light/dark through the View menu and checks editor text,
caret and undo/redo. It exercises the menu item's automation invoke and toggle
providers and checks synchronization when the application theme changes.
This uses the Workbench window with Fluent and AvaloniaEdit styles in a smoke
application; it does not test physical display, input or screen-reader behavior.
The product starts in dark mode; the View menu can switch themes for the current
session. The selection is not persisted.

For the C# quick-info check only, pass `-- --quick-info-only`. It restores a
temporary SDK project and verifies the keyboard event, visible information,
pointer-movement hover, unsaved-file protection and dismissal in a real Workbench
window. It also checks filtered completion, provider edits, undo and stale
suggestion rejection, plus cancellation while a hover request waits for the
language service. A second open C# tab supplies unsaved source to quick info.
It also verifies the Ctrl+Shift+Space signature-help command and active parameter.
Pointer and keyboard input are simulated.

For the source test-candidate check only, pass `-- --test-method-only`. It
restores a temporary MSTest project, discovers source candidates and adapter
cases, executes one selected adapter case and one source method through the
Workbench project controls. It also checks that the result tree is bound and
contains the reported passing case.
Pass `-- --xunit-test-method-only` to run the same workflow with xUnit v3,
including its additional test-process attach. Setting `AVLN_NETCOREDBG_PATH`
also exercises debugger breakpoints, continue, stop and selection changes.

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
TreeView items, synchronized source caret/tree/inspector including keyboard focus
return with an unchanged caret, source-preserving reorder and designer undo/redo,
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

The inert resource preview also renders the AA05 `Button.Menu` application-style
gradient with localized RESX captions. It accepts only bounded literal
application gradients and conservative, conflict-free resource lookup; see
`AA98_AvlnCodeStudio.Model/Workspaces/AxamlPreviewResources.md` for limits.
Image controls can be inserted from the palette and edit literal source references
and stretch settings. The preview always uses an inert placeholder; it does not
load local files, network resources or binding expressions.
Specialized catalog controls and their supported static ComboBox, ListBox and
TreeView item types are available from the external palette.
