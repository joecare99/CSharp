# GenFreeData

Data access / persistence layer for genealogy applications. Encapsulates database / storage concerns and exposes repository-style or table-like abstractions consumed by higher layers (domain, UI, import/export tools).

## Purpose
Provide a single, tested point for CRUD and query operations decoupled from UI frameworks. Facilitates migration from legacy data engines while enabling modern async paths.

## Responsibilities
- Record/table abstractions (read, iterate, seek).
- Data conversion helpers (string/date parsing, normalization).
- Migration / temporary file handling.
- Integration boundaries to OLE DB / Jet / other providers (with specialized impl projects).

## Key Features
- Pluggable backends via interface-driven approach.
- Utility methods for safe field extraction & null handling.
- Batch iteration patterns used by search utilities.

## Targets
Multi-target: `net481` and modern `net6.0+` variants (see csproj) to support both legacy WinForms host and modern services.

## Example (Pseudo)
```csharp
if (personTable.ReadData(id, out var person))
{
    Console.WriteLine(person.LastName);
}
foreach (var rec in personTable.ReadAll()) { /* ... */ }
```

## Architecture
```
GenFreeData
  Tables / Repositories
  Field helpers
  Backing connection adapters (in separate impl projects)
```

## Extending
- Add adapter: implement required recordset interfaces and register via DI or factory.
- Introduce caching: wrap table access with simple in-memory map by ID.

## Testing
See `GenFreeDataTests` project for representative usage and regression tests.

## Contributing
Prefer small cohesive commits. New public APIs require XML docs & unit tests.

## License
(Insert license.)
