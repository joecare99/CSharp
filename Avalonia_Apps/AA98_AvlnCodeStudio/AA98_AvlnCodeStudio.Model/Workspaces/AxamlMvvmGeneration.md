# AXAML MVVM binding planning

The incremental planner recognizes simple root-ViewModel paths in `Binding`,
`CompiledBinding`, and `ReflectionBinding` expressions on bindable properties
in `AxamlControlCatalog`. It plans missing properties and commands without
rewriting the AXAML; existing members are checked before source edits are
offered for review.

Explicit binding sources, complex paths, and bindings in nested DataContext, x:DataType, or
template scopes are rejected rather than assigned to the root ViewModel.
This includes DataContext declared as a property element: its owning control and
its descendants are a nested scope, not only the property element itself.
Binding-free nested scopes remain untouched.
Unsupported controls and non-binding markup are not planned. Creating a new
root ViewModel refuses existing expressions of all three binding kinds to avoid
redirecting their source; it also refuses named supported controls within a
nested x:DataType scope rather than guessing that scope's static type.
Workspace review and undo/redo remain responsible for
applying and reverting the resulting file changes.

The shared visual binding editor also edits simple `Binding`, `CompiledBinding`
and `ReflectionBinding` paths with `Mode` and `UpdateSourceTrigger`, preserving
the original binding kind through review, apply, undo and redo. Unsupported
options, empty arguments, explicit or nested sources, ambiguous properties,
nested binding scopes and stale source snapshots are read-only. New bindings
use `Binding`; editing may normalize the supported expression's formatting.
During asynchronous member planning, a changed member, mode, trigger or
selection invalidates the pending binding request before markup can be created
or directly applied. Retry with the current row; a second request for the same
row is not planned concurrently.

The generated root `Window.DataContext` (or corresponding root control
property) is accepted only when it constructs one empty `using:` type matching
the root `x:DataType`. Other root contexts and nested contexts remain
read-only. Workspace member creation additionally resolves the declared type
and checks recognized code-behind assignments. This is static validation, not
proof that runtime code cannot later replace the DataContext.

## Workbench boundary evidence

`MainWindowViewModel_IncrementalGenerationRespectsNestedBindingBoundaries`
uses a synthetic Toolkit project and invokes the Workbench incremental command
with an existing root ViewModel constructed by `Window.DataContext`. For each
of `Binding`, `CompiledBinding`, and `ReflectionBinding`, it rejects a binding
on the owner of a nested DataContext property element, on its descendant, under
a DataContext attribute, under a nested `x:DataType`, and inside a template.
A preceding root binding does not result in a partial review. The AXAML and
ViewModel files, project and code-behind files, and dirty AXAML and code-behind
buffers remain unchanged.

A binding-free nested DataContext produces no changes; a normal root binding
alongside that nested scope produces a root ViewModel member proposal. Cancelling
the review leaves both files and dirty buffers unchanged. These tests exercise
static analysis, planning and Workbench review with a generated fixture; the
fixture is not an Avalonia application and its AXAML is not compiled or run.
They do not prove runtime DataContext behavior or cover every markup syntax.
