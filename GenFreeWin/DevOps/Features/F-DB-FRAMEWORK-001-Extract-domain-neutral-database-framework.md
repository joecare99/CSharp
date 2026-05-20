# Feature F-DB-FRAMEWORK-001: Extract Domain-Neutral Database Framework

## Parent

- Planned under the local `DevOps` modernization/refactoring track

## Objective

Extract the existing database abstractions and reusable data-access building blocks from the current `GenFree*` solution area into a domain-neutral framework that can be reused outside the genealogy context.

## Value

- Removes genealogy-specific naming and architectural coupling from reusable database code
- Makes provider-specific implementations easier to isolate and maintain
- Enables future database providers such as `OleDb`, `MySql`, `SqlServer`, and `dBase` to follow the same shape
- Reduces the risk that application/domain concerns leak into infrastructure packages

## Scope

- Current abstraction contracts in `GenFreeBase\Interfaces\DB`
- Current provider implementation in `GenDBImplOLEDB`
- Planning for a new neutral database framework split into abstractions, core, and provider facade projects
- Migration guidance for existing `GenFree*` consumers

## Non-Goals

- No full provider migration in the first planning increment
- No immediate breaking rename of all existing consumer code
- No DAO-behavior redesign in the planning-only increment

## Architectural Direction

### Proposed neutral project split

1. `Db.Core.Abstractions`
   - provider-neutral interfaces
   - statement abstractions
   - common enums and metadata contracts
2. `Db.Core`
   - shared base classes and reusable helper logic
   - optional provider-neutral command/statement composition support
3. Provider facade projects
   - `Db.Provider.OleDb`
   - `Db.Provider.MySql`
   - `Db.Provider.SqlServer`
   - `Db.Provider.DBase`
4. Temporary compatibility adapters in `GenFree*`
   - bridge old namespaces/usages during migration

## Initial Analysis Findings

- `GenFreeBase\Interfaces\DB` already contains several generally reusable contracts.
- Some current interfaces are still coupled to `GenFree.Data` types such as `RecordsetTypeEnum`.
- `GenDBImplOLEDB` currently mixes provider-specific infrastructure with more general data-access concerns.
- `GenDBImplOLEDB\Data.DB\MySqlStatementRenderer.cs` appears misplaced and should move to a provider-appropriate or neutral location.
- Namespaces and project names still expose genealogy/domain context and should not remain the public long-term API.

## Acceptance Criteria

1. A documented target architecture exists for a domain-neutral DB framework.
2. Reusable abstractions and provider-specific responsibilities are clearly separated.
3. A migration plan exists for existing `GenFree*` consumers.
4. Initial work items are defined for extraction, compatibility, and test coverage.

## Assumptions

- Multi-targeting support must remain practical for both `.NET Framework` and modern `.NET` targets.
- The first execution increment should minimize breaking changes for current solution consumers.
- Existing recordset-oriented APIs may need a compatibility layer before deeper modernization.

## Open Questions

- Should the neutral framework live inside the current repository first, or be extracted into a separate repository later?
- Should DAO-like naming such as `Recordset` remain as a compatibility surface or be replaced in a later major version?
- Which provider should be normalized first after `OleDb`: `MySql` or `SqlServer`?
