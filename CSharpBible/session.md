# Session Resume

## Current objective

Continue the incremental implementation of a Visual-Studio-like `ConsoleLib.Cxaml.Designer` and the extraction of a shared, host-neutral terminal renderer used by the Designer, ExtCon, and Posix hosts.

Work is delivered in focused slices. Each slice should:

1. Implement one coherent capability.
2. Add extensive regression tests.
3. Update the session plan and SQL todos.
4. Create an English Conventional Commit with the required Copilot co-author trailer.

## Repository and working state

- Repository: `ChristianRosewich/CSharp`
- Working directory: `D:\Projekte\GitHub\CSharp\CSharpBible`
- Current branch: `master`
- Latest relevant commit: pending `feat: sync list collection rendering`
- The working tree was clean when this file was created.
- Session plan: `C:\Users\DEROSCHR\.copilot\session-state\cb26ebc1-c49b-46fe-9e16-66dea4314ece\plan.md`
- SQL session todos: all currently recorded todos are done.

Unrelated changes in neighboring `Avalonia_Apps` directories may appear during concurrent work. Do not revert them and do not include them in ConsoleLib commits unless explicitly requested.

## Completed implementation

### Designer foundation

- Categorized Inspector property metadata.
- Property selection and application through the Designer ViewModel.
- Visual and Console preview modes.
- AvalonEdit-based source editor with caret offset synchronization.
- Preview/source selection synchronization.
- Grid row/column editor with `Auto`, `Pixel`, and `Star` definitions.
- Detailed Grid property-element persistence.
- Grid loader support for:
  - `Grid.RowDefinitions`
  - `Grid.ColumnDefinitions`
  - compact `RowDefinitions` and `ColumnDefinitions` attributes
  - `Grid.Row`, `Grid.Column`, `Grid.RowSpan`, and `Grid.ColumnSpan`
- Console preview viewport choices:
  - `Designer Size`
  - `80x25`
  - `80x50`
  - `132x60`
- Fixed viewport rendering does not mutate CXAML or the loaded control tree.
- Console-cell hit testing selects the topmost visible control and moves the source caret.
- Console selection overlay draws a clipped Unicode outline without mutating the canonical snapshot.

### Shared rendering core

Project: `Libraries\ConsoleLib.Rendering`

- `AttachedRenderService` bound to one root control tree.
- Synchronous canonical frame updates.
- Immutable `IRenderFrameSnapshot` snapshots.
- Monotonically increasing revisions.
- Root resize synchronization.
- Explicit tree subscription refresh.
- Detach/dispose isolation.
- `FrameOutputTracker` for per-host dirty regions.
- Full repaint on first frame, resize, reset, and output failure recovery.
- Host-neutral canonical Unicode cell output.
- Clipping to viewport bounds.
- Invisible controls omitted.
- Existing front-most child order preserved.

### Canonical control output currently covered

- Borders: single and double Unicode box drawing.
- Ellipsis overflow.
- Buttons: centered text.
- CheckBoxes: `[ ]` and `[x]` markers.
- Disabled control colors.
- Shadows using `░`.
- Multiline TextBox word wrapping and overlong-word splitting.
- `ComboBox`: selected value in bracketed form.
- `ListBox`: visible rows, selected-row colors, border-aware content, and clipping.
- `TabControl`: selected header markers and width clipping.
- `TileView`: fixed tile-grid placement, selected colors, blank tile rows, and clipping.
- `TreeView`: hierarchy indentation, expand/collapse markers, selected colors, collapsed descendants, and clipping.
- `ScrollBar`: horizontal/vertical arrows, tracks, proportional thumbs, disabled colors, degenerate ranges, and clipping.
- `ProgressBar`: determinate filled/unfilled cells, fractional values, disabled colors, and clipping.
- `StatusBar`: status text, custom status colors, disabled colors, and clipping.
- `RadioButton`: checked/unchecked markers, disabled colors, and clipping.
- `MenuBar` and `MenuItem`: horizontal layout, mnemonic normalization, accelerator highlighting, active/disabled colors, and clipping.
- `MenuPopup`: bordered popup backgrounds, local item positions, selection colors, separators, and clipping.
- `TextBox`: visible viewport lines, multiline wrapping, disabled colors, caret colors, and clipping.
- `Terminal`: bordered screen-cell output, per-cell colors, empty-cell preservation, and clipping.
- `ScrollViewer`: hosted-content offset translation, viewport/frame clipping, and content color/border preservation.
- `Pixel`: single-cell character output, color preservation, disabled colors, and clipping.
- Container composition: `Dialog` visibility, `ModalHost` popup lifecycle, and `StackPanel` child placement through the shared fallback renderer.
- Dynamic collections: `ListBox` collection updates and selection scrolling are propagated to the shared renderer.

## Recent commits

- `cc1466796 feat: render progress and status controls`
- `b88ff19e8 feat: render canonical scrollbars`
- `7cff7deea feat: render canonical scroll viewers`
- `fe395582c feat: render TreeView controls`
- `42d4b790a feat: render tabs and tiles`
- `d1130f2d7 feat: render ComboBox and ListBox`
- `715e9cdee feat: highlight console preview selection`
- `4e0848908 feat: add console preview hit testing`
- `ff281901b feat: add Designer console viewports`
- `ef490a0a8 feat: disabled states and z-order`
- `ae6385b2f feat: add canonical text wrapping`
- `cb46edd6 feat: recover host output after failure`
- `eac147c53 feat: add canonical shadows`
- `b997c02a4 feat: render interactive control semantics`
- `ae199d0e0 feat: harden render service lifecycle`
- `3d6dac1d7 feat: add canonical borders and clipping`

Earlier commits established `ConsoleLib.Rendering`, its test project, Grid loading/editor support, and Designer migration.

## Test baseline

Latest successful targeted runs:

- `ConsoleLib.RenderingTests`: **65 passed**
- `ConsoleLib.Cxaml.DesignerTests`: **25 passed**
- Container composition and dynamic ListBox collection regressions pass on net8.0, net9.0, and net10.0.

Existing nullable event-handler warnings in legacy ConsoleLib controls are unrelated to the rendering work.
The Test-Coverage skill reports **100% scoped line coverage** for `ControlFrameRenderer` (490/490 lines).

The current checkout does not contain the previously referenced Test-Coverage script; coverlet collector execution completed successfully and produced scoped Cobertura reports.

Run tests from PowerShell with Windows paths:

```powershell
dotnet test 'D:\Projekte\GitHub\CSharp\CSharpBible\Libraries\ConsoleLib.RenderingTests\ConsoleLib.RenderingTests.csproj' --no-restore --verbosity quiet
dotnet test 'D:\Projekte\GitHub\CSharp\CSharpBible\Libraries\ConsoleLib.Cxaml.DesignerTests\ConsoleLib.Cxaml.DesignerTests.csproj' --no-restore --verbosity quiet
```

Run the two commands sequentially when possible because parallel builds can contend for shared `obj` outputs.

## Recommended next slices

Continue the canonical renderer before migrating hosts:

1. Remaining form/collection controls not yet covered by the shared renderer.
2. Canonical terminal/control-specific output where existing ExtCon or Posix behavior exists.
3. Migrate ExtCon to `ConsoleLib.Rendering`.
4. Migrate Posix to `ConsoleLib.Rendering`.

Designer-specific follow-up work:

- Calculate `Designer Size` from available Avalonia space and terminal-cell font size.
- Persist the selected terminal frame size and cell font size locally.
- Add explicit terminal-cell selection overlays to the visual preview.
- Add Grid removal confirmation and child-placement reflow.
- Add selection support for tree/list/scrollbar content where the Designer surface exposes those controls.

Broader Designer workspace work remains out of scope for the current renderer slices unless explicitly selected:

- Multi-document tabs and Project Files.
- Control Hierarchy structural editing.
- Docking and persisted tool-window layout.
- Localization, settings, diagnostics, and undo/redo.
- Documentation under `Libraries\DevOps` and project READMEs.

## Implementation conventions

- Keep the shared renderer UI-, transport-, and OS-neutral.
- Canonical frames contain cells only: character, foreground, and background.
- Host capability fallbacks belong in host adapters, not in the canonical renderer.
- Preserve the existing ConsoleLib child ordering: lower child index is front-most.
- Use surgical changes and existing project test runners.
- Do not use destructive Git commands.
- Commit messages are English, Conventional Commit style, with a first line no longer than 50 characters:

```text
Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>
```

