# GenFreeBase

Foundational genealogy domain layer: core entities, value objects, shared enums and primitive logic reused by UI and data layers.

## Purpose
Centralize non-persistence business rules and shared definitions (person names, events, places, enumeration sets) to avoid duplication across WinForms, WPF, console, and service projects.

## Scope
- Core domain types (persons, families, events) – structure & invariants.
- Supporting enums / constants.
- Lightweight services (formatting, validation) not tied to a data provider.

## Out of Scope
- UI specific formatting (belongs in presentation layers).
- Direct database code (lives in GenFreeData / impl projects).

## Design Principles
- POCO / record usage where immutability is beneficial.
- Keep side effects minimal (no I/O in entities).
- Leverage nullable reference types for correctness.

## Targets
Multi-target (legacy + modern) to allow consumption from older WinForms host while enabling modern .NET features.

## Example (Illustrative)
```csharp
var person = new Person(name: new PersonName("John","Doe"));
var display = person.Name.Display;
```

## Extending
- Add new value object: ensure validation in constructor and override equality sensibly.
- Introduce domain service: keep stateless or inject only required abstractions.

## Testing
See `GenFreeBaseTests` – add scenarios around invariants and edge conditions.

## Contributing
Public additions require unit tests and doc comments. Avoid leaking persistence concerns.

## License
(Insert license.)
