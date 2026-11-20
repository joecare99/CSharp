# Project explorer resource nesting

The workbench wraps the filesystem project/solution explorer with a read-only
projection. On refresh it reads each project XML without running MSBuild and
places a `.cs` file under its sibling `.resx` when a literal `Compile Include`
or `Update` assigns one literal `DependentUpon` element or attribute. Missing
or unavailable files stay in their original positions.

Conditions are not evaluated. Multiple assignments, conditional metadata or
assignments, assignments inside `Choose` or `Target`, and computed or invalid
metadata for the same literal file leave that file unnested. The projection is
best-effort: imports, wildcards, computed item paths and other MSBuild evaluation
are not resolved, so their possible effects on a literal assignment cannot be
determined. Refresh rereads the project; it does not edit project or user files.
