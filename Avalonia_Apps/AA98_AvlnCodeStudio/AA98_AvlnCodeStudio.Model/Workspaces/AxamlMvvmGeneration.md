# AXAML MVVM binding planning

The incremental planner recognizes simple root-ViewModel paths in `Binding`,
`CompiledBinding`, and `ReflectionBinding` expressions on bindable properties
in `AxamlControlCatalog`. It plans missing properties and commands without
rewriting the AXAML; existing members are checked before source edits are
offered for review.

Explicit binding sources, complex paths, and bindings in nested DataContext, x:DataType, or
template scopes are rejected rather than assigned to the root ViewModel.
Unsupported controls and non-binding markup are not planned. Creating a new
root ViewModel refuses existing expressions of all three binding kinds to avoid
redirecting their source; it also refuses named supported controls within a
nested x:DataType scope rather than guessing that scope's static type.
Workspace review and undo/redo remain responsible for
applying and reverting the resulting file changes.
