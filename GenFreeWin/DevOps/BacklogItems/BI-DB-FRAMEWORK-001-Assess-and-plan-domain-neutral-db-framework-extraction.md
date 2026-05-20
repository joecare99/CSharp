# Backlog Item BI-DB-FRAMEWORK-001: Assess and Plan Domain-Neutral DB Framework Extraction

## Parent

- Feature `F-DB-FRAMEWORK-001` - Extract Domain-Neutral Database Framework

## Objective

Assess the current DB abstraction surface in `GenFreeBase` and `GenDBImplOLEDB`, classify reusable versus domain-coupled parts, and prepare an incremental extraction/migration plan for a general-purpose DB framework.

## Value

- Creates a safe foundation for later technical extraction work
- Makes architectural risks visible before introducing broad project churn
- Preserves existing consumers by planning compatibility paths explicitly

## Scope

- Inventory interfaces and general classes in `GenFreeBase\Interfaces\DB`
- Inventory provider-specific classes in `GenDBImplOLEDB`
- Identify misplaced provider logic and hidden domain dependencies
- Define target package/project boundaries and migration stages

## Deliverables

- Architecture analysis summary
- Proposed target project split
- Compatibility strategy for existing consumers
- Implementation task breakdown for the first execution increments

## Analysis Summary

### Likely neutral today

- `IDbConnectionFactory`
- `IDbStatementRenderer`
- `IDbSelectStatement`
- `IDbInsertStatement`
- `IDbUpdateStatement`
- `IDbFilterClause`
- `DbFilterOperator`

### Needs review or decoupling first

- `IDatabase`
- `IDBEngine`
- `IDBWorkSpace`
- `IRecordset`
- `IField`
- `IFieldsCollection`
- `IFieldDef`
- `IIndexDef`
- `ITableDef`

### Main reasons for decoupling work

- current namespaces use `GenFree.*`
- some contracts depend on `GenFree.Data`
- some contracts depend on application-specific interfaces such as `GenFree.Interfaces.Model.IHasValue`
- provider package contains code that is not strictly `OleDb`-only

## Acceptance Criteria

1. Existing DB contracts are classified into neutral, compatibility, and provider-specific groups.
2. The target neutral naming and project structure are documented.
3. The first technical extraction increment is small enough to execute safely.
4. Testing expectations for the extraction are documented.

## Assumptions

- Planning is performed before broad code movement.
- Existing public behavior should remain functional during early migration increments.

## Open Questions

- How much of the classic DAO-style API should be preserved long-term?
- Should the first increment introduce new namespaces while leaving old ones as forwarding adapters?
