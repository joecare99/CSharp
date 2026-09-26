# T0023-B0009 - Test23 nested goto reduction

## Status

Implemented and validated on `net8.0`.

## Requirement

Add the supplied `IL_0437` / `IL_05f8` NIO code excerpt as Test23. Conditional branches assign `UbgT` and jump to the shared `IL_05f8` continuation. These inner gotos can be removed when an `if`/`else` restructuring preserves the shared continuation.

## Implementation

- Added `Test23Dat.cs` as an embedded test resource.
- Extended common-target conditional reduction to handle a single conditional branch followed by shared fall-through statements and the same target goto.
- The branch-local goto is removed and its following statements are moved into the `else` branch; multiple statements are represented by an explicit block.
- The final common goto to `IL_05f8` remains; the other remaining goto is the resume-switch dispatch entry.
- Updated the Test22 expected output to include the same safe shared-tail restructuring.
- Added `RemoveSingleSourceLabels1_ReducesTest23InnerGotos`, asserting that branch-local gotos disappear while both the dispatcher and common continuation still target `IL_05f8`.
- Switch-label reconstruction is enabled when the target has multiple conditional goto sources. Test01's three mutually exclusive error branches therefore become a clearer `if`/`else if`/`else` chain with one shared continuation goto; its Expected resource was updated after verifying flow equivalence. The Test23 resume-switch dispatch goto remains intact.

## Validation

- Test23 focused regression: passed.
- `CodeOptimizerTests` and `RemoveLabelsTest`: 30/30 passed on `net8.0`.
