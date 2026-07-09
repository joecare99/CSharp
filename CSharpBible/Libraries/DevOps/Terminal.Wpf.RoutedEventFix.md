# Bug: Terminal.Wpf routed event async handling

## Status
- [x] Planned
- [x] Implemented
- [x] Validated
- [x] Closed

## Summary
A WPF host throws `System.InvalidOperationException` with the message indicating that the `RoutedEvent` property cannot be changed while the event is being routed.

## Root Cause Hypothesis
`Terminal.Wpf.Controls.TerminalControl` awaits asynchronous terminal I/O inside routed WPF input handlers and uses `ConfigureAwait(false)`. That allows the handler continuation to run after WPF has already entered routed-event processing, while the handler still mutates event state such as `Handled`.

## Task Breakdown
1. Keep routed-event decisions synchronous inside `TerminalControl`.
2. Move asynchronous terminal write operations behind helper methods that no longer access routed-event args after awaiting.
3. Validate the affected projects and any discoverable related tests.

## Validation Notes
- `Terminal.Wpf.Controls.TerminalControl` now completes routed-event decisions synchronously and queues terminal I/O without awaiting inside the WPF routed handlers.
- No dedicated `Terminal.Wpf` test project or related Test Explorer entries were present in the solution.
- Workspace build completed successfully after the change.
