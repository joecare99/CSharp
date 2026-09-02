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
- Working directory: `C:\Projekte\GitHub\CSharp\CSharpBible`
- Current branch: `master`
- Latest relevant commit: `720a46ae8 test: cover radio button rendering`
- The working tree contains the uncommitted DockPanel rendering regression slice until
  the focused commit is created below.
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

## Latest completed slice: RadioButton rendering regression coverage

The RadioButton slice was completed as a regression-and-validation slice because the
canonical renderer already handled RadioButton through the shared text path. No
dedicated production rendering branch was necessary. The existing control semantics
are preserved and are now explicitly protected by focused renderer tests.

### Covered behavior

- Unchecked controls render the `( ) ` marker followed by their content.
- Checked controls render the `(*) ` marker followed by their content.
- Selecting one RadioButton unselects the previously selected RadioButton in the same
  group.
- The rendered frame reflects the changed selection state after an explicit render
  refresh of the attached render service.
- Disabled RadioButtons use the disabled foreground/background colors.
- Marker text is clipped correctly at the viewport boundary.
- The tests validate frame rows and cell colors rather than host-specific console
  output, keeping the regression coverage host-neutral.

### Validation and repository state

- `ConsoleLib.RenderingTests`: 67 tests passed on `net8.0`, `net9.0`, and `net10.0`.
- `ConsoleLib.Cxaml.DesignerTests`: 25 tests passed.
- Scoped `ControlFrameRenderer` coverage: 490/490 lines, 100% line rate.
- `git diff --check`: passed.
- Working tree: clean.
- Commit: `720a46ae8 test: cover radio button rendering`.
- The commit uses the required Copilot co-author trailer.

This slice confirms that selection changes, disabled-state presentation, and clipping
remain stable without coupling `ControlFrameRenderer` to a host implementation.

## Recent commits

- `720a46ae8 test: cover radio button rendering`
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

## Latest completed slice: DockPanel rendering regression coverage

The DockPanel slice was completed as a regression-only slice. `DockPanel` already
arranges child dimensions correctly, and the generic child-recursion path in
`ControlFrameRenderer` preserves those positions without requiring a dedicated branch.

### Covered behavior

- All four dock directions render at their arranged positions.
- `LastChildFill` and remaining-area composition are preserved in the canonical frame.
- Resizing the attached viewport reflows docked and fill children before rendering.
- Arranged children are clipped to the canonical viewport.
- The tests assert host-neutral cell output through `AttachedRenderService`.

### Validation and repository state

- `ConsoleLib.CoreTests` DockPanel tests: **13 passed**.
- `ConsoleLib.RenderingTests`: **240 passed** on net8.0, net9.0, and net10.0.
- `ConsoleLib.Cxaml.DesignerTests`: **25 passed**.
- Scoped `ControlFrameRenderer` coverage: **490/490 lines, 100%** on net8.0, net9.0, and net10.0.
- `git diff --check`: pending final commit validation.
- Production changes: none; only focused renderer regression tests were added.

## Test baseline

Latest successful targeted runs:

- `ConsoleLib.RenderingTests`: **240 passed**
- `ConsoleLib.Cxaml.DesignerTests`: **25 passed**
- Container composition and dynamic ListBox collection regressions pass on net8.0, net9.0, and net10.0.
- RadioButton rendering regressions pass on net8.0, net9.0, and net10.0.

Existing nullable event-handler warnings in legacy ConsoleLib controls are unrelated to the rendering work.
The Test-Coverage skill reports **100% scoped line coverage** for `ControlFrameRenderer` (490/490 lines).

The current checkout does not contain the previously referenced Test-Coverage script; coverlet collector execution completed successfully and produced scoped Cobertura reports.

Run tests from PowerShell with Windows paths:

```powershell
dotnet test 'C:\Projekte\GitHub\CSharp\CSharpBible\Libraries\ConsoleLib.RenderingTests\ConsoleLib.RenderingTests.csproj' --no-restore --verbosity quiet
dotnet test 'C:\Projekte\GitHub\CSharp\CSharpBible\Libraries\ConsoleLib.Cxaml.DesignerTests\ConsoleLib.Cxaml.DesignerTests.csproj' --no-restore --verbosity quiet
```

Run the two commands sequentially when possible because parallel builds can contend for shared `obj` outputs.

## Recommended next slices

Continue the canonical renderer before migrating hosts:

### 1. Cover remaining form and collection controls

This is the next executable phase. It is complete only when one selected control family has an explicit renderer decision, focused regression coverage, successful validation, committed session bookkeeping, and a clean commit.

#### 1.1 Inventory and select one control family

##### 1.1.1 Identify candidates

- The current `ConsoleLib.CommonControls` inventory contains the following types:
  `Application`, `BorderDef`, `Button`, `CheckBox`, `ComboBox`, `CommandControl`,
  `Dialog`, `DockPanel`, `Grid`, `Label`, `ListBox`, `MenuBar`, `MenuItem`,
  `MenuPopup`, `ModalHost`, `Panel`, `Pixel`, `ProgressBar`, `RadioButton`,
  `ScrollBar`, `ScrollViewer`, `StackPanel`, `StatusBar`, `TabControl`, `TabItem`,
  `Terminal`, `TextBox`, `TileItem`, `TileView`, `TreeNode`, and `TreeView`.
- Controls with dedicated canonical renderer branches are already covered:
  `ListBox`, `TabControl`, `TileView`, `TreeView`, `ScrollBar`, `ProgressBar`,
  `MenuBar`, `MenuPopup`, `MenuItem`, `StatusBar`, `TextBox`, `Terminal`,
  `ScrollViewer`, and `Pixel`.
- Controls covered by the generic text or child-composition path are:
  `Button`, `CheckBox`, `ComboBox`, `Label`, `RadioButton`, `Panel`, `Dialog`,
  `ModalHost`, `StackPanel`, `Grid`, and `Application`. `CommandControl` also
  inherits the generic text path; its command execution semantics are interaction
  behavior, not a separate cell-rendering format.
- Non-visual support types are not renderer candidates by themselves:
  `BorderDef`, `RowDefinition`, `ColumnDefinition`, `TabItem`, `TileItem`, and
  `TreeNode`. They affect a visual control's layout or content and must be tested
  through that owning control.
- The concrete uncovered rendering/regression candidates are therefore:
  1. `DockPanel`: dock ordering, `LastChildFill`, remaining-area calculation,
	 resizing, child clipping, and interaction with borders.
  2. `Grid`: multi-row/column placement, `Auto`/`Pixel`/`Star` sizing, spans,
	 alignment, resizing, and clipping. A basic placement test exists, but the full
	 layout contract is not yet covered by the rendering suite.
  3. Generic container composition: explicit regression coverage for `Application`,
	 `Panel`, `StackPanel`, and `ModalHost` lifecycle/visibility combinations where
	 the current tests do not already cover the exact state transition.
  4. `Label` and `CommandControl`: only if host behavior such as `ParentBackground`
	 or command-driven enabled-state changes is intended to become canonical cell
	 semantics. Otherwise they remain fallback-path regression candidates, not new
	 renderer branches.
- `BorderDef`, `TabItem`, `TileItem`, and `TreeNode` are dependencies of later
  control-specific tests, not standalone slices.
- Before selecting one candidate, search ExtCon and Posix for behavior that is not
  represented by the current canonical path. Record text, selection, focus,
  enabled-state, layout, collection, and clipping semantics.

##### 1.1.2 Define the slice boundary

- Select exactly one coherent control family or tightly coupled behavior.
- Exclude unrelated host migration, Designer UI work, and unrelated warnings.
- State whether a new canonical renderer branch is required or whether regression tests for the fallback path are sufficient.

##### 1.1.3 Selection gate

Do not proceed until the candidate, existing behavior, affected projects, expected cell output, and slice boundary are documented. If no dedicated branch is required, document why the fallback path is sufficient.

##### 1.1.4 Current recommended selection

Select `DockPanel` as the next focused slice. It is the clearest remaining visual
container without a dedicated rendering regression in `ConsoleLib.RenderingTests`,
and its layout behavior can be validated entirely through canonical child positions,
cell output, clipping, and composition. Do not add a `DockPanel` branch to
`ControlFrameRenderer`; first test whether the existing child-recursion path already
preserves the arranged positions. Select `Grid` only after the DockPanel slice is
closed, unless inspection proves that the two layout contracts must be tested together.

#### 1.2 Inspect the control contract and renderer path

##### 1.2.1 Inspect model semantics

- Read the control implementation and related interfaces.
- Trace state changes, collection notifications, selection rules, and visibility.
- Identify sizing, child ordering, border, color, and clipping behavior.

##### 1.2.2 Inspect canonical composition

- Trace `ControlFrameRenderer` dispatch, generic text rendering, child recursion, color selection, and viewport clipping.
- Compare the control contract with the current renderer behavior.
- Use host implementations only as behavioral references; do not copy host transport logic into the shared renderer.

##### 1.2.3 Design gate

Before editing production code, choose one outcome:

1. The existing path is correct: add regression tests only.
2. A minimal canonical branch is required: define cell-level output and clipping rules first.
3. The behavior is host-specific: keep canonical rendering unchanged and defer capability handling to a host adapter.

#### 1.3 Implement the canonical behavior or regression slice

##### 1.3.1 Add production rendering only when required

- Preserve immutable frame snapshots and host neutrality.
- Preserve lower-index-front-most child ordering.
- Keep output limited to character, foreground color, and background color cells.
- Handle viewport clipping at the canonical renderer boundary.

##### 1.3.2 Add focused regression tests

- Cover normal/default state.
- Cover state transitions such as selection, focus, expansion, or collection updates where applicable.
- Cover disabled/invisible states and relevant colors.
- Cover borders, child composition, viewport clipping, and degenerate dimensions where applicable.
- Refresh the attached render service explicitly after model mutations when asserting the resulting snapshot.

##### 1.3.3 Implementation gate

Leave implementation only when expected behavior is encoded in tests and no redundant renderer branch has been introduced. New production code must be limited to the selected control family.

#### 1.4 Validate the complete slice

##### 1.4.1 Run focused tests

- Run relevant `ConsoleLib.RenderingTests` tests for all supported target frameworks used by the project, including `net8.0`, `net9.0`, and `net10.0` where available.
- Run the complete `ConsoleLib.RenderingTests` project.

##### 1.4.2 Run integration regression tests

- Run the complete `ConsoleLib.Cxaml.DesignerTests` project.
- Confirm Designer preview rendering and existing selection behavior remain unchanged.

##### 1.4.3 Verify coverage and repository quality

- Recollect scoped Cobertura coverage for `ControlFrameRenderer`.
- Require 100% scoped line coverage, currently 490/490 lines, unless the renderer legitimately changes; if it changes, update the covered-line baseline explicitly.
- Run `git diff --check` and inspect the complete diff.
- Remove temporary coverage artifacts.

##### 1.4.4 Validation gate

Point 1 is not complete if any focused test, full rendering test, Designer test, coverage check, or diff check fails. Fix failures before documentation or commit work; do not continue to Point 2 with known failures.

#### 1.5 Update the session checkpoint

##### 1.5.1 Record the completed slice

- Add the control and covered behavior to the canonical output inventory.
- Record exact test counts, target frameworks, coverage result, and relevant test-side synchronization details.
- Record implementation decisions and known unrelated warnings.

##### 1.5.2 Record the next-slice handoff

- Mark the completed candidate as done.
- Name the next remaining candidate or state that selection is pending.
- Point 2 cannot begin until Point 1.4's validation gate and Point 1.5's documentation update are complete.

#### 1.6 Commit and close Point 1

##### 1.6.1 Create the focused commit

- Use an English Conventional Commit message.
- Keep the first line at 50 characters or fewer.
- Include the required Copilot co-author trailer.
- Include only files belonging to the selected ConsoleLib slice.

##### 1.6.2 Verify the handoff state

- Confirm commit hash and subject.
- Confirm the working tree is clean, excluding unrelated neighboring repository work.
- Confirm all Point 1 completion gates are recorded in this file.

##### 1.6.3 Point 1 completion criterion

Point 1 is complete only after 1.1 through 1.6 are complete: one control slice is selected, behavior is understood, implementation or regressions are present, all tests and scoped coverage pass, documentation is updated, the focused commit exists, and the working tree is clean. Only then may Point 2 begin.

### 2. Add canonical terminal/control-specific output

Begin only after Point 1 is closed with a clean commit and the remaining control inventory has been reduced to behavior that cannot be represented by the current canonical path.

#### 2.1 Compare host behavior

- Select one existing ExtCon or Posix behavior that is semantically important.
- Separate portable cell semantics from transport, input, cursor, and terminal capability concerns.

#### 2.2 Extend the canonical contract minimally

- Add only host-neutral cell output or renderer semantics.
- Keep host capability fallbacks outside `ConsoleLib.Rendering`.
- Add tests for canonical behavior and unsupported-host fallbacks.

#### 2.3 Apply the same validation gate

- Run rendering and Designer tests.
- Reconfirm scoped coverage and diff quality.
- Update this checkpoint and commit before selecting another behavior.

### 3. Migrate ExtCon to `ConsoleLib.Rendering`

Begin only after the canonical renderer contains all required portable semantics from Points 1 and 2.

#### 3.1 Introduce the ExtCon adapter

- Map canonical frame cells to ExtCon output.
- Use `FrameOutputTracker` for dirty-region updates.
- Preserve full repaint, resize, reset, and output-failure recovery.

#### 3.2 Validate ExtCon integration

- Run adapter and rendering regressions.
- Verify no canonical renderer dependency on ExtCon APIs is introduced.
- Commit the migration separately from unrelated control work.

### 4. Migrate Posix to `ConsoleLib.Rendering`

Begin only after the ExtCon migration is independently validated and committed.

#### 4.1 Introduce the Posix adapter

- Map canonical cells to Posix terminal output.
- Preserve clipping, dirty regions, resize handling, and recovery semantics.

#### 4.2 Validate the Posix migration

- Run platform-appropriate adapter tests and the shared rendering suite.
- Confirm host-specific capability handling remains outside the canonical renderer.
- Update the session checkpoint and create a focused migration commit.

