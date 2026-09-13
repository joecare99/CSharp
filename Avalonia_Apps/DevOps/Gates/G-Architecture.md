# G-Architecture

## Status
Passed

## Required evidence
- Component and dedicated test-project boundaries.
- Dependency diagram and reference audit.
- Public contracts use interfaces/capabilities and DI.
- Base contracts have no product, UI, OS, Model, Planning, or ConsoleLib dependency unless expressly assigned.
- UI text is localizable at UI/registration boundaries.

## Decision
Approved for T-001, T-002, and T-005 preparation. The Config.Service audit found a
standalone net8.0 component with DI abstractions only and no product, UI,
ConsoleLib, CodeStudio, Planning, WinForms, or WPF references. The remaining
persistence and registration edge cases are assigned to T-002 and T-003. The
T-005 Code.Navigation audit confirms the same boundary for the source-location
contract and navigator capability.

T-020 confirms that `AppKomponentBaseLib` remains the shared base for component
contracts, while `Diagnostics.Navigation` references only the shared diagnostic
model and neutral `Code.Navigation` types. No UI or editor dependency was added.

T-008 confirms that `Property.Editor` is a standalone contract component with
a dedicated MSTest project. Its public surface models categories, item metadata,
selectable options, validation, ordering, and value changes without references
to Avalonia, product models, planning, ConsoleLib, or OS-specific APIs. UI
adapters receive localized display text from their registration boundary.
The T-008 consolidation removed the legacy string-only `IPropertyItem` and
`PropertyItem` from `AA98_AvlnCodeStudio.Base.UI`; its remaining
`IHasProperties` capability exposes the canonical neutral interface directly.
The Planning adapter retains only typed display conversion and Planning-model
callbacks.

T-012 defines `Config.UI` as a standalone, product-neutral boundary. Its
section metadata, lifecycle state, and section/registry capabilities reference
only `Property.Editor`; Config.Service and every product-specific adapter
remain outside this contract.

T-013 adds those concerns only in dedicated projects: `Config.UI.ConfigService`
depends on Config.Service and `Config.UI.Avalonia` depends on Avalonia and the
shared Property.Editor UI. Neither dependency flows back into the neutral
Config.UI contract, and host-specific provider models remain at the host
registration boundary.

T-015 defines `Project.Explorer` and its dedicated test project as a
filesystem- and UI-neutral capability boundary. Its immutable hierarchy
records and source, state, and opening interfaces contain no Planning,
Avalonia, product, ConsoleLib, or operating-system implementation references.
The existing Planning Explorer remains a separate domain.

T-018 adopts the established CSharpBible `Code.Navigation` linked-source
pattern for canonical Property.Editor production and test sources. The CXAML
designer consumes only the Property.Editor and CodeLocation contracts through
designer-bound adapters; its preview renderer, control mapping, toolbox,
hit-testing, and CXAML write-back remain ConsoleLib responsibilities.

Post-completion maintenance moved the product-neutral Config.Service and its
dedicated test project from the historical RnzTrauer subtree to `Libraries`.
The component retains no product dependency; all RnzTrauer, CodeStudio, and
Config.UI consumers now reference the canonical library project.

- Reviewer: implementation agent
- Date: 2026-09-13
- Affected tasks: T-001, T-005, T-008, T-012, T-015, T-018, and post-completion
  Config.Service relocation
- Next task status: feature closure remains Done.
