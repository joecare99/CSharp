# Task T-DB-FRAMEWORK-005: Map Current Types to Neutral Target Architecture

## Parent

- Backlog Item `BI-DB-FRAMEWORK-001` - Assess and Plan Domain-Neutral DB Framework Extraction

## Objective

Define a concrete type-by-type mapping from the current `GenFree*` database surface to the future neutral DB framework so the first extraction increment can be executed without broad uncontrolled churn.

## Current Surface Inventory

### Already close to provider-neutral

| Current type | Current location | Proposed target | Notes |
|---|---|---|---|
| `IDbConnectionFactory` | `GenFreeBase\\Interfaces\\DB` | `Db.Core.Abstractions.Connections.IDbConnectionFactory` | Good candidate for direct extraction |
| `IDbStatementRenderer` | `GenFreeBase\\Interfaces\\DB` | `Db.Core.Abstractions.Sql.IDbStatementRenderer` | Good candidate for direct extraction |
| `IDbSelectStatement` | `GenFreeBase\\Interfaces\\DB` | `Db.Core.Abstractions.Sql.IDbSelectStatement` | Good candidate for direct extraction |
| `IDbInsertStatement` | `GenFreeBase\\Interfaces\\DB` | `Db.Core.Abstractions.Sql.IDbInsertStatement` | Good candidate for direct extraction |
| `IDbUpdateStatement` | `GenFreeBase\\Interfaces\\DB` | `Db.Core.Abstractions.Sql.IDbUpdateStatement` | Good candidate for direct extraction |
| `IDbFilterClause` | `GenFreeBase\\Interfaces\\DB` | `Db.Core.Abstractions.Sql.IDbFilterClause` | Good candidate for direct extraction |
| `DbFilterOperator` | `GenFreeBase\\Interfaces\\DB` | `Db.Core.Abstractions.Sql.DbFilterOperator` | Move with SQL abstractions |

### Neutral in intent but still coupled to legacy/solution concepts

| Current type | Coupling | Proposed target | Migration note |
|---|---|---|---|
| `IDatabase` | depends on `IRecordset` and DAO-style operations | `Db.Core.Abstractions.Legacy.IDatabaseSession` or `Db.Core.Compatibility.IDatabase` | Keep as compatibility-first surface initially |
| `IDBEngine` | legacy engine/workspace shape | `Db.Core.Compatibility.IDatabaseEngine` | Keep in compatibility package first |
| `IDBWorkSpace` | legacy transaction/workspace concept | `Db.Core.Compatibility.IDatabaseWorkspace` | Keep in compatibility package first |
| `IRecordset` | depends on `GenFree.Data.RecordsetTypeEnum` | `Db.Core.Compatibility.IRecordset` | Extract together with compatibility enum |
| `IFieldsCollection` | tied to recordset model | `Db.Core.Compatibility.IFieldsCollection` | Move with recordset compatibility layer |
| `IField` | depends on `IHasValue` chain | `Db.Core.Compatibility.IField` | Requires nullability/value-contract review |
| `IFieldDef` | metadata contract but DAO-flavored | `Db.Core.Compatibility.Schema.IFieldDefinition` | Safe to rename later |
| `IIndexDef` | metadata contract | `Db.Core.Compatibility.Schema.IIndexDefinition` | Safe to rename later |
| `ITableDef` | metadata contract + `GenFree.Data` dependency risk | `Db.Core.Compatibility.Schema.ITableDefinition` | Move after dependency cleanup |
| `RecordsetTypeEnum` | currently in `GenFree.Data` | `Db.Core.Compatibility.RecordsetType` | Must move out of `GenFree.Data` |

### Provider-specific implementation types

| Current type | Current location | Proposed target | Notes |
|---|---|---|---|
| `DBImplementOleDB.DBEngine` | `GenDBImplOLEDB\\Data.DB` | `Db.Provider.OleDb.OleDbDatabaseEngine` | Provider facade |
| `DBImplementOleDB.CWorkSpace` | `GenDBImplOLEDB\\Data.DB` | `Db.Provider.OleDb.OleDbDatabaseWorkspace` | Provider facade |
| `DBImplementOleDB.CDatabase` | `GenDBImplOLEDB\\Data.DB` | `Db.Provider.OleDb.OleDbDatabase` | Provider facade |
| `DBImplementOleDB.Recordset` | `GenDBImplOLEDB\\Data.DB` | `Db.Provider.OleDb.OleDbRecordset` | Compatibility implementation |
| `DBImplementOleDB.FieldsCollection` | `GenDBImplOLEDB\\Data.DB` | `Db.Provider.OleDb.OleDbFieldsCollection` | Compatibility implementation |
| `DBImplementOleDB.Field` | `GenDBImplOLEDB\\Data.DB` | `Db.Provider.OleDb.OleDbField` | Compatibility implementation |
| `MySqlStatementRenderer` | `GenDBImplOLEDB\\Data.DB` | `Db.Provider.MySql.MySqlStatementRenderer` | Misplaced today |

## Recommended Project Split for Increment 1

### `Db.Core.Abstractions`

Contains only contracts that are provider-neutral and free from genealogy/domain references:

- SQL statement abstractions
- renderer abstraction
- connection factory abstraction
- shared SQL enums

### `Db.Core.Compatibility`

Contains the DAO-style compatibility surface needed by current consumers:

- `IDatabase`
- `IDatabaseEngine`
- `IDatabaseWorkspace`
- `IRecordset`
- `IField`
- `IFieldsCollection`
- `RecordsetType`
- optional schema metadata contracts

### `Db.Provider.OleDb`

Contains only `OleDb`-specific implementations and registration helpers.

### Later provider projects

- `Db.Provider.MySql`
- `Db.Provider.SqlServer`
- `Db.Provider.DBase`

## First Technical Extraction Slice

### Slice goal

Create the new neutral abstraction package without breaking existing consumers.

### Slice content

1. Add `Db.Core.Abstractions` project.
2. Move or copy the SQL abstraction contracts into neutral namespaces.
3. Move `MySqlStatementRenderer` out of `GenDBImplOLEDB` into a provider-appropriate location or a temporary neutral holding project if no `MySql` provider project exists yet.
4. Leave legacy `IDatabase`/`IRecordset` contracts untouched for the first increment.
5. Add focused renderer tests for the extracted SQL abstraction path.

### Why this slice is safe

- It avoids immediate churn in recordset-heavy consumers.
- It removes the clearest architectural mismatch first.
- It creates a neutral foundation for later provider and compatibility work.

## Compatibility Strategy

### Short term

- Keep `GenFreeBase` contracts compiling.
- Introduce new neutral contracts in parallel.
- Allow provider implementations to support old and new registrations side-by-side where practical.

### Medium term

- Update consumers gradually from `GenFree.Interfaces.DB` to `Db.Core.*` namespaces.
- Add adapter/wrapper types only where direct dual-implementation is not practical.

### Long term

- Remove `GenFree*` DB contracts after consumer migration is complete.

## Important Gotchas

- `IRecordset` is used by higher-level model helpers such as `CUsesRecordSet`, so changing it early has high blast radius.
- `IField` currently inherits value semantics from existing base interfaces; this may hide domain coupling that must be neutralized carefully.
- `CompactDatabase` in the `OleDb` engine is Windows/COM-specific and must stay provider-specific.
- `System.Data.OleDb` support differs across target frameworks and platforms, so provider packaging must stay explicit.

## Proposed Increment Order

1. Neutral SQL abstractions
2. Neutral namespace and project introduction
3. `MySqlStatementRenderer` relocation
4. Compatibility package for recordset/engine contracts
5. `OleDb` provider cleanup
6. Additional providers

## Execution Update

### Implemented Increment 1 slice

The first technical extraction slice has now been started with concrete repository changes:

- added new neutral project `DbCoreAbstractions`
- added new provider project `DbProviderMySql`
- added new test project `DbProviderMySqlTests`
- copied the provider-neutral SQL abstraction contracts into `DbCoreAbstractions`
- moved `MySqlStatementRenderer` out of `GenDBImplOLEDB` into `DbProviderMySql`
- removed the misplaced `GenDBImplOLEDB\Data.DB\MySqlStatementRenderer.cs`

### Validation snapshot

- `dotnet build DbCoreAbstractions\DbCoreAbstractions.csproj -nologo` succeeded
- `dotnet build DbProviderMySql\DbProviderMySql.csproj -nologo` succeeded
- `dotnet build GenDBImplOLEDB\GenDBImplOLEDB.csproj -nologo` succeeded
- `dotnet build DbProviderMySqlTests\DbProviderMySqlTests.csproj -nologo` succeeded
- automated test execution for `DbProviderMySqlTests` is currently blocked by repository test-platform/discovery configuration and still needs follow-up

### Browser integration update

The database browser applications have now also been attached to the new framework direction at project-reference level:

- `MSQBrowser` now references `DbCoreAbstractions` and `DbProviderMySql`
- `MdbBrowser` now references `DbCoreAbstractions` and `GenDBImplOLEDB`
- `MSQBrowser` no longer keeps an unnecessary direct `System.Data.OleDb` package reference only for schema collection naming

### Required compatibility adjustment discovered during integration

The browser projects still target older `.NET Framework` versions (`net462` and `net472`), so the new framework chain had to be widened:

- `DbCoreAbstractions` extended to `net462;net472;net481;net6.0;net7.0;net8.0;net9.0`
- `DbProviderMySql` extended to `net462;net472;net481;net6.0;net7.0;net8.0;net9.0`
- `GenDBImplOLEDB` extended to `net462;net472;net481;net8.0`
- `GenFreeBase` extended to `net462;net472;net481;net6.0;net7.0;net8.0;net9.0`

### Validation update after browser integration

- `dotnet build MSQBrowser\MSQBrowser.csproj -nologo` succeeded
- `dotnet build MdbBrowser\MdbBrowser.csproj -nologo` succeeded
- `dotnet build GenDBImplOLEDB\GenDBImplOLEDB.csproj -nologo` succeeded

### Browser model refactoring update

The browser applications are no longer only attached at project-reference level. Their model classes now start consuming the new provider-facing creation path:

- `MSQBrowser.Models.DBModel` now creates its `DbConnection` through `Db.Provider.MySql.MySqlDbConnectionFactory`
- `MdbBrowser.Models.DBModel` now creates its `OleDbConnection` through `GenFree.Data.DB.OleDbConnectionFactory`
- `DbProviderMySql` now exposes `MySqlDbConnectionFactory`
- `GenDBImplOLEDB` now exposes `OleDbConnectionFactory`
- `GenDBImplOLEDB` also contains an initial `OleDbStatementRenderer` for neutral SQL rendering support

### Scope note

This is intentionally an incremental extraction step:

- connection creation is now routed through neutral/provider factory abstractions
- command, adapter, and paging logic in the browser models are still partly provider-specific and remain a follow-up step
- the UI and ViewModel surface was not widened in this increment

### Validation update after model refactoring

- `dotnet build C:\Projekte\CSharp\Gen_FreeWin\DbProviderMySql\DbProviderMySql.csproj -nologo` succeeded
- `dotnet build C:\Projekte\CSharp\Gen_FreeWin\MSQBrowser\MSQBrowser.csproj -nologo` succeeded
- `dotnet build C:\Projekte\CSharp\Gen_FreeWin\MdbBrowser\MdbBrowser.csproj -nologo` succeeded
- `dotnet build C:\Projekte\CSharp\Gen_FreeWin\MSQBrowserTests\MSQBrowserTests.csproj -nologo -v:minimal` succeeded
- `dotnet build C:\Projekte\CSharp\Gen_FreeWin\MdbBrowserTests\MdbBrowserTests.csproj -nologo` succeeded

### Related workspace impact observed during broader solution builds

Unrelated projects under `WinAhnenNew\RnzTrauer` currently fail because they also expect a `MySqlStatementRenderer` type in their own project context. That is outside the browser refactoring scope and should be handled as a separate integration task.
