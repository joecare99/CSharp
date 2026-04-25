# LOCAL-E001 MySQL Browsing Stability

- Type: Epic
- State: Ready for Review
- Parent: none

## Goal
Stabilize browsing of large MySQL tables in `MSQBrowser` so that users can inspect schema and row data without client-side crashes or full-table loading.

## Scope
- Large-table browsing in `MSQBrowser`
- Asynchronous or incremental row loading
- Safe handling of large text, blob, and zero-date values
- Regression coverage for changed behavior

## Assumptions
- Only the currently visible rows need to be loaded initially.
- Blob and memo columns can be shown as previews instead of full payloads.
- MySQL zero-date values must not break the UI data load.

## Open Questions
- Should old rows be discarded again while scrolling to limit memory usage?
- Should zero dates be displayed as empty, placeholder text, or a localized marker?
- Is keyset paging preferable to `LIMIT/OFFSET` for very large tables?

## Next Refinement Steps
- Measure memory use during longer scrolling sessions.
- Decide on the final UX for date placeholders.
- Review whether paging should become key-based for large datasets.
