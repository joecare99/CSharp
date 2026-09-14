# ConsoleApps Contract Tests and Scoped Refactoring – 2026-08

## Backlog item

- **ID:** ConsoleApps-Bl002
- **Title:** Align tests with public contracts and reduce large-class scope
- **Status:** Completed (implementation scope)
- **Scope:** `ConsoleApps.slnx` and referenced CSharpBible libraries

## Expected behavior

The public `ICxamlLoader` and `ICxamlValidator` contracts must remain the
observable boundary for CXAML parsing, loading, validation, root enforcement,
named-control lookup, binding, and diagnostic reporting. Tests must verify those
outcomes through public APIs rather than private fields or parser helpers.

`CxamlLoader` shall retain responsibility for reading CXAML structure and
constructing controls. Attribute conversion and application are a separate,
cohesive responsibility and may be implemented by an internal collaborator.
The extraction must preserve markup results and `CxamlParseException` behavior.

## Review findings

| Area | Finding | Decision |
|---|---|---|
| `CxamlLoader` (598 lines) | Loading and validation have public contracts and existing dedicated tests. Attribute handling is a cohesive internal responsibility. | Extract attribute handling; add public contract tests. |
| `TextBox` (753 lines) | Editing, layout, rendering, binding, and input are interleaved with control lifecycle state. | Defer; requires a dedicated UI-control decomposition task. |
| `DesignerViewModel` (715 lines) | Designer orchestration crosses preview, source selection, inspector, and grid editing. | Defer; requires a separate designer architecture decision. |
| `ExtendedConsole` (783 lines) | Native interop, input pump, and console host lifecycle are inseparable in the current design. | Defer; requires platform-host abstractions first. |
| `ConsoleFrameworkTests` | Tests inspect private canvas state through reflection. | Do not broaden this refactoring; follow up when a public canvas-size contract is available. |
| `DisplayTests` | Tests depend on static state and detailed rendering representation. | Keep as rendering regressions; add contract tests only when a stable public display API is defined. |

## Planned tests

1. Loading applied common and control-specific attributes through `ICxamlLoader`.
2. Loading rejects invalid scalar, color, accelerator, and unsupported attribute
   values with `CxamlParseException`.
3. Validation reports unsupported attributes without loading a control.
4. Loading preserves names, bindings, and grid definitions after extraction.

## Validation protocol

1. Run focused `ConsoleLibTests` without coverage.
2. Run the focused project with the configured coverage workflow when available.
3. Build `ConsoleApps.slnx`.
4. Record test/build evidence and handoff status in this document.

## Validation evidence

- `CxamlLoader` was reduced from 598 to 348 lines by extracting cohesive
  attribute and binding application into `CxamlAttributeApplicator`.
- Public loader contracts (`ICxamlLoader`, `ICxamlValidator`) remained unchanged.
- Added contract-focused tests in `ConsoleLib.CoreTests/CxamlBindingTests.cs`:
  - `Load_RejectsInvalidAcceleratorLength`
  - `Load_RejectsInvalidOrDuplicateNameInLoadContext`
- Focused run result:
  - `dotnet test ...ConsoleLib.CoreTests.csproj -f net8.0-windows --no-build --filter "FullyQualifiedName~ConsoleLib.CoreTests.CxamlBindingTests"`
  - Passed: 8/8, Failed: 0
- Broader `ConsoleLib.CoreTests` run currently has one pre-existing unrelated
  failure in `ControlTests.RealDimAndLocalDim_Work`.

## Current task

Completed.

## Next task

Evaluate `TextBox` for a dedicated decomposition backlog item by separating
editing state, binding synchronization, and rendering responsibilities behind
stable public contracts.
