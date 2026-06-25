# Bug: Terminal mouse reporting fallback

## Status
- [x] Planned
- [ ] Implemented
- [ ] Validated
- [ ] Closed
- [x] Paused

## Summary
Mouse input still does not reach the hosted console application even though the routed WPF event exception has already been fixed.

## Current Findings
- The current WPF terminal control only forwards mouse input when `TerminalMouseProtocol.Sgr` is active.
- The shared parser reports mouse tracking modes for `?1000`, `?1002`, and `?1003`.
- The shared parser reports SGR protocol only for `?1006`.
- If a console application enables mouse tracking without also enabling `?1006`, the UI control currently suppresses all mouse reports.
- The same SGR-only gate is present in both `Terminal.Wpf.Controls.TerminalControl` and `Terminal.Avalonia.Controls.TerminalControl`.

## Working Hypothesis
Some hosted console applications enable VT mouse tracking but keep the default protocol instead of negotiating SGR explicitly. The terminal host therefore needs a fallback mouse protocol path rather than requiring SGR unconditionally.

## Planned Next Steps
1. Extend the shared mouse protocol model to represent the default VT mouse protocol.
2. Update the ANSI parser and mouse input encoder to support protocol-specific encoding.
3. Update WPF and Avalonia terminal controls to send mouse reports for the negotiated protocol.
4. Add regression tests in `Terminal.Core.Tests`.
5. Validate with targeted tests and a solution build.

## Resume Notes
- Last confirmed user feedback: the routed-event exception is gone and line duplication is fixed.
- A separate unsupported `CSI ... t` sequence was intentionally deferred.
- The next implementation step should start in shared terminal core protocol handling, not in the host app entry point.
