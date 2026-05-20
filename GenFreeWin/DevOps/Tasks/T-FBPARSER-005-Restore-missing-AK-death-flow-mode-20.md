# Task T-FBPARSER-005 Restore missing AK death flow mode 20

## Summary
Investigate and restore the AK death-entry flow around parser mode `20` in `FBParser\FBEntryParser.cs`, because the parser still switches to mode `20` for AK death markers while the corresponding flow handling appears to be missing or incomplete in the current C# port.

## Parent
- Task family: `FBParser` parser port stabilization and maintainability work
- Related existing item: `T-FBPARSER-004` Refactor FBEntryParser into maintainable components

## Motivation
AK death entries are part of the original family-book parser behavior. The current parser still routes `†` and `+` markers to mode `20`, but the visible state machine no longer shows a corresponding `case 20` implementation. That strongly suggests an incomplete or lost death-flow branch in the AK parsing path.

## Current findings
- `FBParser\FBEntryParser.cs` still transitions to `localMode = 20` from AK entry mode detection.
- The parser now uses the consolidated death marker array `CDeathEntries` for several AK-related checks.
- A visible `case 20` branch is currently not present in the active `Feed` state machine.
- This issue is likely independent from the recent GC helper extraction and should be worked on separately.
- The user explicitly wants this work deferred to another machine.

## Suspected impact
- AK death entries may no longer be parsed through the intended dedicated death flow.
- Death markers in AK samples can be misinterpreted as generic non-person fragments or may fall through to the wrong modes.
- Regression sequences for AK parity samples may remain unstable until the dedicated death branch is restored.

## Proposed investigation
1. Compare the current C# `Feed` mode transitions with the original Pascal mode `20` logic.
2. Reconstruct the expected AK death-entry handling semantics.
3. Reintroduce or repair the dedicated mode `20` branch in a minimal-risk way.
4. Add focused regression tests for AK death entries using both `†` and `+` markers.
5. Re-run targeted AK parity samples after the fix.

## Validation strategy
- Build `FBParser` and `FBParserTests` after the restoration.
- Add focused MSTest coverage for AK death entries.
- Re-run targeted AK sample tests that contain death markers before broad parity validation.

## Notes
- This task was intentionally recorded for later execution on another machine.
- Current local continuation should focus on GC-specific work only.
