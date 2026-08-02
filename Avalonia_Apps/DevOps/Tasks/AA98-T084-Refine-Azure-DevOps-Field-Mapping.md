# AA98-T084 Refine Azure DevOps Field Mapping

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl046-Azure-DevOps-Planning-Adapter-Baseline.md`

## Goal
Define the minimal Azure DevOps work-item mapping required before adapter implementation.

## Scope
- Map local epics, features, backlog items, tasks, bugs, and test cases to generic Azure DevOps work-item concepts.
- Map local `Id`, `Title`, `Status`, and parent links to provider identifiers, title, state, and hierarchy links.
- Keep Markdown body content optional and outside the first adapter behavior.
- Select a bounded first adapter mode without adding credentials to planning files.

## Minimal Mapping
| Local concept | Azure DevOps concept | Required fields |
| --- | --- | --- |
| Epic | Epic work item | Id, Title, State |
| Feature | Feature work item | Id, Title, State, Parent |
| Backlog Item | Product Backlog Item work item | Id, Title, State, Parent |
| Task | Task work item | Id, Title, State, Parent |
| Bug | Bug work item | Id, Title, State, Parent |
| Test Case | Test Case work item | Id, Title, State, Parent |
| Other kinds | Unmapped | Diagnostic only |

## Decision
- The skeleton reports import and export capability but does not execute synchronization yet.
- A later integration slice chooses the Azure DevOps process template and maps concrete state labels.

## Status
- Completed
