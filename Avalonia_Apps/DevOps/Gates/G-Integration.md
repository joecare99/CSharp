# G-Integration

## Status
Passed

## Required evidence
- DI composition test from host to component.
- No copied business/UI logic in consuming hosts.
- Config isolation, explorer-to-editor navigation, diagnostics-to-code navigation, or property editing behavior relevant to the task.
- English SVN commit and status handover proof.

## Decision
The prior T-022 fixture passed the component-level diagnostic-to-editor-caret
chain, but the actual `AA98.DevOpsPlanning.Host` does not register an
`ICodeNavigator`. The host consequently resolves the diagnostics consumer
through its parameterless fallback and cannot activate diagnostics at runtime.

T-022 now proves the AA98 CodeStudio workbench composition resolves the
diagnostics consumer with its registered `EditorCodeNavigator` and delegates
activation through the document host and caret service. The DevOps planning
micro-host remains out of this editor-specific integration scope. G-Integration
is passed.

T-014 proves both productive configuration hosts resolve the shared
`ConfigUiViewModel` and `IConfigUiSectionRegistry` through DI. RnzTrauer
exposes its four ordered provider sections; CodeStudio exposes only its
host-owned Workbench section. Their `JC-Soft/RnzTrauer` and
`JC-Soft/AA98-CodeStudio` Config.Service identities derive isolated persistence
roots without duplicating Config.UI views, ViewModels, or storage code.
G-Integration remains passed; next work is T-015.

T-017 proves the CodeStudio workbench resolves the shared Project Explorer
through DI and maps an available file activation through its host-owned opener
to the registered `ICodeNavigator`. The reusable Explorer UI does not access
AvaloniaEdit, and the pre-existing Planning Explorer remains separate.
G-Integration remains passed; next work is T-018.
