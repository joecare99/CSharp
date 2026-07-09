# Bug: Terminal mouse reporting fallback

## Status
- [x] Planned
- [x] Implemented
- [x] Validated
- [ ] Closed
- [ ] Paused

## Summary
Mouse input did not reach the hosted console application in the WPF and Avalonia hosts when applications enabled mouse reporting through combined private mode sequences.

## Current Findings
- The current WPF and Avalonia terminal controls forward mouse input only when `MouseTrackingMode` and `TerminalMouseProtocol.Sgr` are active.
- The shared parser previously matched only exact single private mode strings such as `?1002` and `?1006`.
- Combined DECSET and DECRST sequences such as `CSI ?1002;1006h` or `CSI ?1000;1006l` were ignored by the parser.
- Because of that, `MouseTrackingMode` and `MouseProtocol` stayed unset and both hosts suppressed mouse forwarding.

## Working Hypothesis
Hosted console applications commonly enable VT mouse tracking and SGR reporting in a single combined private mode sequence. The shared parser therefore needs to split and process each requested mode individually.

## Planned Next Steps
1. Validate the parser fix with targeted terminal parser tests.
2. Validate the affected host projects with targeted builds if needed.

## Resume Notes
- The parser now splits combined private mode sequences in `ApplyMode` and applies each mode independently.
- Regression tests were added for combined mouse enable and disable sequences.
- If runtime issues remain after validation, the next check should be a live host session against a mouse-aware terminal client.
