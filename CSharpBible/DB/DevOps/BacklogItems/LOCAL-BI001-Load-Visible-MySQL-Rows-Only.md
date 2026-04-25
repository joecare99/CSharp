# LOCAL-BI001 Load Visible MySQL Rows Only

- Type: Backlog Item
- State: Ready for Review
- Parent: `LOCAL-F001 Paged MySQL Table View`

## User Value
As a user of `MSQBrowser`, I want only the visible or next-needed MySQL rows to be loaded so that very large tables remain browsable.

## Acceptance Criteria
- Table data is loaded in limited pages instead of full-table `SELECT *` materialization for the grid view.
- Reaching the lower part of the visible grid triggers loading of additional rows.
- Blob columns are shown as lightweight presence or size previews.
- Long text columns are truncated to a preview value.
- MySQL zero-date values no longer throw during data retrieval.
- Existing project tests compile and run against the changed behavior.

## Assumptions
- Paging is sufficient without immediate server-side sorting support.
- Empty display for zero dates is acceptable in the current UI.

## Open Questions
- Should preview length be configurable?
- Should paging respect a primary-key order when available?

## Next Refinement Steps
- Add product decision on preview length.
- Review ordering guarantees for tables without explicit sort criteria.
