# LOCAL-F001 Paged MySQL Table View

- Type: Feature
- State: Ready for Review
- Parent: `LOCAL-E001 MySQL Browsing Stability`

## Goal
Provide a responsive table view for MySQL data by loading rows incrementally and reducing payload size for expensive column types.

## Scope
- Page-based loading for table data
- Scroll-triggered incremental append in the data grid
- Preview projection for blob and long-text columns
- Graceful handling of MySQL zero-date values in displayed results

## Assumptions
- The existing WPF `DataGrid` remains the primary presentation control.
- Read-only presentation is acceptable for the paged data view.
- Metadata-based column shaping is sufficient for preview generation.

## Open Questions
- Should paging size be configurable per connection or per user?
- Should the first page show total-row estimation or remain lightweight?
- Should users be able to open a full-value dialog for previewed columns?

## Next Refinement Steps
- Review UX after real-world use with large tables.
- Decide whether row-count estimation is needed.
- Decide whether a detail view is needed for truncated values.
