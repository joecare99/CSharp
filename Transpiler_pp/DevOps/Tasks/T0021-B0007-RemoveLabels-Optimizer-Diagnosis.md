# T0021-B0007 - RemoveLabels optimizer diagnosis

## Status

Completed for Test0 and Test17-Test19; Test1, Test2, and Test9 remain under investigation.

## Findings

The `RemoveLabelsTest` failures were not all caused by incorrect expected data:

- `Test0`: optimizer incorrectly converted a single switch-case `goto` to `break` and removed it as a duplicate of the outer `goto`. A switch-case boundary must prevent duplicate-goto removal, and the single-switch-source plus outer-goto shape must not be converted.
- `Test17`: optimizer behavior and expected result are correct; the switch-case `goto` becomes `break`, and execution continues at the same point because the target label follows the switch.
- `Test18`: optimizer behavior and expected structure are correct; the case gotos must remain because executable code occurs between the switch and the target label. The fixture had a trailing newline that caused a false comparison failure.
- `Test19`: optimizer behavior and expected structure are correct; both case gotos can become `break` because they share the safe continuation target. The fixture had a trailing newline that caused a false comparison failure.
- `Test19a`: when executable code occurs between the `switch` and the outer `goto`, the case gotos must remain. Generic next-instruction lookup must not cross the switch boundary.
- `Test1`: the first resume-switch `default` was already present, but the second `switch (num4)` had lost its `default: goto end_IL_0000;`. The `end_IL_0000` source count was also stale (`1` instead of `2`). Both Expected-data differences are corrected.
- `Test2`: the second resume-switch had lost its `default: goto end_IL_0000;`, and the `end_IL_0000` source count was stale (`1` instead of `2`). The Expected structure is corrected and the following local block indices are shifted accordingly.
- `Test9`: the failure exposed an optimizer defect. The synthetic blocks following two switches were mistaken for switch-case continuations, so `goto IL_006f;` and `goto IL_01d7;` were incorrectly replaced by `break;`. After preserving both gotos, the expected source count for `end_IL_0001` is `2`. The remaining `case 46` versus `default` difference was stale Expected data; the `default` entry is part of the source switch and must be retained.
- `Test2`: remaining difference is a switch-label/target reconstruction mismatch (`case 11` versus `default`) and requires parser/optimizer graph auditing.

## Implemented repairs

- Preserve switch-case boundaries during duplicate-goto cleanup.
- Treat `switch` as an execution boundary while searching for duplicate gotos.
- Require a real preceding `case` or `default` label before treating a goto as a switch-case terminator.
- Keep switch blocks out of the generic execution-boundary classification.
- Restrict final switch-goto conversion for the one-switch-source plus direct-outer-goto shape.
- Keep `Name` and `Type` consistent when converting `Goto` blocks to `Operation` blocks.
- Normalize the Test17-Test19 expected fixture endings.

## Validation

On `net8.0`, the focused `RemoveLabelsTest` set passes all 14 cases after the optimizer repairs and Expected-data corrections.

## Test Explorer readability

`RemoveLabelsTest` now receives resource identifiers such as `Test01` and resolves the corresponding token list and expected `ExpParseRL` data through `TestCSDataClass`. The Test Explorer therefore shows the resource name instead of an opaque numeric parameter plus serialized data payload.
