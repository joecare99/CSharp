# Task T-DB-FRAMEWORK-001: Inventory Current DB Abstractions and Couplings

## Parent

- Backlog Item `BI-DB-FRAMEWORK-001` - Assess and Plan Domain-Neutral DB Framework Extraction

## Objective

Create a structured inventory of current DB interfaces, supporting contracts, provider implementations, and domain couplings.

## Scope

- `GenFreeBase\Interfaces\DB`
- `GenDBImplOLEDB`
- direct type and namespace dependencies revealed during inspection

## Findings to capture

- neutral/reusable contracts
- domain-coupled contracts
- provider-specific classes
- misplaced classes
- test coverage gaps

## Notes

- `GenDBImplOLEDB\Data.DB\MySqlStatementRenderer.cs` should be treated as a misplaced file during the inventory.
- Multi-target compatibility must be tracked as part of the inventory.
