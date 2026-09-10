# T-005 Define Code Location

## Shared library location
The canonical implementation is maintained under
`Avalonia_Apps\Libraries\Code.Navigation`; the parallel
`CSharpBible\Libraries\Code.Navigation` project links the same source files.

## Parent
B-Code-Navigation | Previous: T-004 | Next: T-006

## Status
Done

## Request
Define a minimal, UI- and OS-agnostic CodeLocation value and navigator capability. It represents a path plus optional line, column, and offset and contains no editor, Avalonia, Planning, or ConsoleLib dependency.

## Planned tests
- Valid and invalid path/line/column/offset combinations.
- Equality and normalization behavior.
- Empty path, zero/negative coordinate, and unspecified coordinate behavior.
- Project-reference audit validates the base component boundary.

## Gate and completion
Pass G-Architecture. Before Done: tests/build/documentation pass, English SVN commit is recorded (SVN revision: 1842), status advances to T-006 and parent backlog/feature status is updated.
