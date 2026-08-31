# ConsoleLib Showcase backlog

| ID | Item | Status |
| --- | --- | --- |
| CLS-01 | Native ConsoleLib/ExtCon gallery shell | Done |
| CLS-02 | MVVM and dependency-injection composition | Done |
| CLS-03 | Visual effects and component demonstrations | Done |
| CLS-04 | Showcase-owned Terminal.Core ConPTY bridge | Done |
| CLS-05 | Dedicated MSTest project and validation | Done |
| CLS-06 | Live Terminal widget workspace and input routing | Done |

## Scope boundary

The showcase owns its Windows ConPTY bridge in
`ConsoleApps\ConsoleLib.Showcase.Terminal.Core`. The reusable
`Libraries\Terminal.Core` contracts and models remain provider-neutral.

## Follow-up

- Add screenshot-based acceptance evidence when a Windows console capture
  harness is available.
- Keep interactive terminal end-to-end tests out of the deterministic MSTest
  suite.
