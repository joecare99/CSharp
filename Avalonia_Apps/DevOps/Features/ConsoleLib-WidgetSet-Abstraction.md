# ConsoleLib WidgetSet Abstraction

## Parent
- Vision: `DevOps/Vision.md`

## Goal
Introduce a `WidgetSet` abstraction for `ConsoleLib` so the existing public control classes and interfaces remain usable as they are today, while the underlying widget and rendering implementation can later be swapped transparently.

## Scope
- Keep the current `ConsoleLib` class structure intact for consuming applications.
- Introduce a widget-set seam behind the existing controls.
- Provide a default console-backed widget set that preserves the current rendering behavior.
- Route representative control rendering through the new abstraction.

## Included
- Widget set interfaces and default implementation
- Framework-level access point for the active widget set
- Backward-compatible rendering delegation for selected controls
- Direct project build validation for `ConsoleLib`
- Planned WinForms-backed widget-set slice with a separate `ConsoleLib.WinForms` project
- MVVM-oriented host concept that treats `ConsoleLib` controls increasingly as UI descriptions

## Excluded for Now
- Consumer-facing API redesign
- Replacement of the full event/input model
- New non-console rendering backend implementation
- Full migration of every control in a single pass if a thinner seam is sufficient first

## Success Indicators
- Existing consuming code can keep instantiating the same control classes.
- `ConsoleLib` still renders to the console by default.
- A new internal abstraction exists that can later host alternative widget implementations.
- `ConsoleLib` builds successfully after the change.

## Assumptions
- Rendering is the first and most valuable seam to extract.
- `ConsoleFramework` is the correct place to expose the active default widget set.
- A partial migration of representative controls is acceptable as long as the seam is established consistently.

## Open Questions
- Which additional controls should move to the widget-set seam in the next increment after the initial slice?
- Should future widget sets own layout and hit testing too, or remain primarily rendering-oriented?

## Planned WinForms Slice
- Create a separate `ConsoleLib.WinForms` library that references `ConsoleLib` and materializes real `System.Windows.Forms` instances.
- Keep widget-set activation IoC-based through the existing `ConsoleFramework.WidgetSet` access path.
- Extend the architecture beyond drawing so a widget set can host, create, track, and synchronize UI instances.
- Deliver a WinForms host form when a `ConsoleLib` `Application` is created in a WinForms-backed setup.
- Move the architecture toward `ConsoleLib` controls as MVVM-friendly descriptions, with WinForms acting as the concrete view layer.
- Support the currently known control set in the first slice, while allowing reduced initial behavior for especially complex controls such as `Terminal` when needed.
- Use a dedicated `ConsoleMouseApp.WinForms` project, based on `ConsoleMouseApp.Base`, as the first concrete application-level test host for the WinForms widget set.
- Treat this architecture direction as repository-specific and document it in DevOps rather than solution-wide memory.
- For WinForms hosting, interpret `ConsoleLib` dimensions as character cells, not pixels; the current working baseline is approximately width `x12` and height `x18` for native bounds translation.
- Continue reducing `ConsoleLib.CommonControls.Application` as a console-centric type and shift hosting responsibility and the main loop further into the active widget set.

## Tasks
- [x] Inspect rendering hotspots and control responsibilities
- [x] Add widget-set abstractions
- [x] Add default console widget-set implementation
- [x] Route representative controls through the widget-set seam
- [x] Validate `ConsoleLib` build

## Validation Notes
- `ConsoleLib.csproj` builds successfully across the current multi-target set.
- The specialized default render path now also routes `MenuItem`, `MenuBar`, `ListBox`, `ScrollBar`, `TextBox`, and `Terminal` through `ConsoleFramework.WidgetSet`.
- The public control classes remain unchanged for consuming applications.
- The default behavior still uses a console-backed widget set via `ConsoleFramework.WidgetSet`.
- The widget seam now covers the base rendering path plus the currently identified specialized rendering hotspots while preserving the existing console-backed behavior.
- A first `ConsoleLib.WinForms` project now exists with a host form, widget registry, hosted widget-set contract, and an initial adapter slice for core controls using real WinForms instances.
- The current WinForms slice already compiles as a separate project and supports bidirectional baseline synchronization for key control state such as text, enabled/visible state, bounds, button clicks, list selection, and scrollbar values.
- The concrete ExtendedConsole-based default widget set has now been moved out of `ConsoleLib` into `ConsoleLib.ExtCon`, while `ConsoleLib` itself targets only modern .NET core TFMs.

## Status
- Completed
